/*
 * subilight.cpp
 *
 * AVS filter for founding average color on frame sides
 *
 * Copyright 2011 Pavel Kryukov (C)
*/

#include "./sublight.h"
#include "./sublight.inl"

/*
 * Constructor
 */
Sublight::Sublight(PClip child) : GenericVideoFilter(child),
                                  _setSizesBpp(vi.IsYV12() ?
                                                &Sublight::SetSizesYV12 :
                                               vi.IsYUY2() ?
                                                &Sublight::SetSizesYUY2 :
                                               vi.IsRGB24() ?
                                                &Sublight::SetSizesRGB24 :
                                                &Sublight::SetSizesRGB32),
                                  _getAv(vi.IsYV12() ?
                                             &Sublight::GetAvYV12 :
                                         vi.IsYUY2() ?
                                             &Sublight::GetAvYUY2 :
                                         vi.IsRGB24() ?
                                             &Sublight::GetAvRGB24 :
                                             &Sublight::GetAvRGB32) {}

packet_t Sublight::GetAvRGB24(const PVideoFrame src, coord_t xy) const {
    register const pixel_t* srcp = src->GetReadPtr()
                                 + this->yOffset * (xy & 3)
                                 + this->width   * (xy >> 2);

    // average stores
    register average_t B = 0;  // Yes I'm a believer
    register average_t G = 0;
    register average_t R = 0;

    fsize_t h = this->height;
    while (h--) {
        fsize_t w = this->width;
        while (w) {
            B += *(srcp++);
            G += *(srcp++);
            R += *(srcp++);
            w -= 3;
         }
         srcp += line;
    }

    B /= averageSize;
    G /= averageSize;
    R /= averageSize;

    // Collect colors into int32
    return ((B + (G << 8) + (R << 16)) << 8) + (xy << 4) + Sublight::SIGNATURE;
}

packet_t Sublight::GetAvRGB32(const PVideoFrame src, coord_t xy) const {
    register const pixel_t* srcp = src->GetReadPtr()
                                 + this->yOffset * (xy & 3)
                                 + this->width   * (xy >> 2);

    // average stores
    register average_t B = 0;
    register average_t G = 0;
    register average_t R = 0;

    fsize_t h = this->height;
    while (h--) {
        fsize_t w = this->width;
        while (w) {
            B += *(srcp++);
            G += *(srcp++);
            R += *(srcp++);
            ++srcp;
            w -= 4;
         }
         srcp += line;
    }

    B /= this->averageSize;
    G /= this->averageSize;
    R /= this->averageSize;

    return ((B + (G << 8) + (R << 16)) << 8) + (xy << 4) + Sublight::SIGNATURE;
}

packet_t Sublight::GetAvYUY2(const PVideoFrame src, coord_t xy) const {
    register const pixel_t* srcp = src->GetReadPtr()
                                 + this->yOffset * (xy & 3)
                                 + this->width   * (xy >> 2);

    // average stores
    register average_t Y = 0;
    register average_t U = 0;
    register average_t V = 0;

    fsize_t h = this->height;
    while (h--) {
        fsize_t w = this->width;
        while (w) {
            Y += *(srcp++);
            U += *(srcp++);
            Y += *(srcp++);
            V += *(srcp++);
            w -= 4;
         }
         srcp += line;
    }

    Y /= this->averageSize;
    U /= this->averageSizeUV;
    V /= this->averageSizeUV;

    return Sublight::YuvToRgb(Y, U, V) + (xy << 4) + Sublight::SIGNATURE;
}

packet_t Sublight::GetAvYV12(const PVideoFrame src, coord_t xy) const {
    register const pixel_t* srcp = src->GetReadPtr()
                                 + this->yOffset * (xy & 3)
                                 + this->width   * (xy >> 2);

    // Average stores
    register average_t Y = 0;

    // Counting average on Y
    fsize_t h = this->height;
    while (h--) {
        fsize_t w = this->width;
        while (w--) {
            Y += *(srcp++);
        }
        srcp += line;
    }

    // Step offset
    const fsize_t offsetUV = this->yOffsetUV * (xy & 3) +
                             this->widthUV   * (xy >> 2);
    register const pixel_t* srcUp = src->GetReadPtr(PLANAR_U) + offsetUV;
    register const pixel_t* srcVp = src->GetReadPtr(PLANAR_V) + offsetUV;

    register average_t U = 0;
    register average_t V = 0;

    h = this->heightUV;
    while (h--) {
        fsize_t w = this->widthUV;
        while (w--) {
             U += *(srcUp++);
             V += *(srcVp++);
        }
        srcUp += lineUV;
        srcVp += lineUV;
    }

    Y /= this->averageSize;
    U /= this->averageSizeUV;
    V /= this->averageSizeUV;

    // Make output bytes
    return Sublight::YuvToRgb(Y, U, V) + (xy << 4) + Sublight::SIGNATURE;
}

void Sublight::SetSizesRGB24(const PVideoFrame src) {
    this->averageSize = this->width * this->height / 3;
}

void Sublight::SetSizesRGB32(const PVideoFrame src) {
    this->averageSize = this->width * this->height >> 2;
}

void Sublight::SetSizesYUY2(const PVideoFrame src) {
    this->averageSize = this->width * this->height >> 1;
    this->averageSizeUV = this->averageSize >> 1;
}

void Sublight::SetSizesYV12(const PVideoFrame src) {
    this->averageSize = this->width * this->height;

    this->widthUV  = src->GetRowSize(PLANAR_U) >> 2;
    this->heightUV = src->GetHeight(PLANAR_U) >> 2;
    this->lineUV   = src->GetPitch(PLANAR_U) - this->widthUV;
    this->yOffsetUV  = src->GetPitch(PLANAR_U) * this->heightUV;

    this->averageSizeUV = this->widthUV * this->heightUV;
}

/*
 * Frame generator
 */
PVideoFrame __stdcall Sublight::GetFrame(int n, IScriptEnvironment* env) {
    const PVideoFrame src = child->GetFrame(n, env);

    this->SetSizes(src);

    frames_t frameBuff = this->frames;

    while (frameBuff) {
        this->Send(CRC((this->*_getAv)(src, static_cast<coord_t>(frameBuff & 0xF))));
        frameBuff >>= 4;
    }

    return src;
}
