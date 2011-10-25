/*
 * subilight.cpp
 *
 * AVS filter for founding average color on frame sides
 *
 * Copyright 2011 Pavel Kryukov (C)
*/

#include "./sublight.h"

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

/*
 * Converter form YUV to RGB
*/
packet_t Sublight::YuvToRgb(average_t Y, average_t U, average_t V) {
    const signed __int32 Yp = 298 * ((signed __int32)Y - 16) + 128;
    const signed __int32 Up = (signed __int32)U - 128;
    const signed __int32 Vp = (signed __int32)V - 128;

    const signed __int32 R = (Yp +            409 * Vp) >> 8;
    const signed __int32 G = (Yp - 100 * Up - 208 * Vp) >> 8;
    const signed __int32 B = (Yp + 516 * Up           ) >> 8;

    return (CUTS(R) + (CUTS(G) << 8) + (CUTS(B) << 16)) << 8;
}

packet_t Sublight::GetAvRGB24(const PVideoFrame src, coord_t xy) const {
    register const pixel_t* srcp = src->GetReadPtr()
                                 + this->height * src->GetPitch() * (xy & 3)
                                 + this->width  * (xy >> 2);

    // average stores
    register average_t B = 0;  // Yes I'm a believer
    register average_t G = 0;
    register average_t R = 0;

    for (fsize_t h = 0; h < this->height; h++) {
        for (fsize_t w = 0; w < this->width; w += 3) {
            B += *(srcp++);
            G += *(srcp++);
            R += *(srcp++);
         }
         srcp += line;
    }

    B /= averageSize;
    G /= averageSize;
    R /= averageSize;

    // Collect colors into int32
    return CRC(((B + (G << 8) + (R << 16)) << 8) + 
               (xy << 4) + Sublight::SIGNATURE);
}

packet_t Sublight::GetAvRGB32(const PVideoFrame src, coord_t xy) const {
    register const pixel_t* srcp = src->GetReadPtr()
                                 + this->height * src->GetPitch() * (xy & 3)
                                 + this->width  * (xy >> 2);

    // average stores
    register average_t B = 0;
    register average_t G = 0;
    register average_t R = 0;

    for (fsize_t h = 0; h < this->height; h++) {
        for (fsize_t w = 0; w < this->width; w += 4) {
            B += *(srcp++);
            G += *(srcp++);
            R += *(srcp++);
            ++srcp;
         }
         srcp += line;
    }

    B /= this->averageSize;
    G /= this->averageSize;
    R /= this->averageSize;

    // Collect colors into int32
    return CRC(((B + (G << 8) + (R << 16)) << 8) + 
               (xy << 4) + Sublight::SIGNATURE);
}

packet_t Sublight::GetAvYUY2(const PVideoFrame src, coord_t xy) const {
    register const pixel_t* srcp = src->GetReadPtr()
                                 + this->height * src->GetPitch() * (xy & 3)
                                 + this->width  * (xy >> 2);

    // average stores
    register average_t Y = 0;
    register average_t U = 0;
    register average_t V = 0;

    for (fsize_t h = 0; h < this->height; h++) {
        for (fsize_t w = 0; w < this->width; w += 4) {
            Y += *(srcp++);
            U += *(srcp++);
            Y += *(srcp++);
            V += *(srcp++);
         }
         srcp += line;
    }

    Y /= this->averageSize;
    U /= this->averageSizeUV;
    V /= this->averageSizeUV;

    // If format is RGB, collect colors into int32
    return CRC(Sublight::YuvToRgb(Y, U, V) + 
               (xy << 4) + Sublight::SIGNATURE);
}

packet_t Sublight::GetAvYV12(const PVideoFrame src, coord_t xy) const {
    register const pixel_t* srcp = src->GetReadPtr()
                                 + this->height * src->GetPitch() * (xy & 3)
                                 + this->width * (xy >> 2);

    // Average stores
    register average_t Y = 0;

    // Counting average on Y
    for (fsize_t h = 0; h < this->height; h++) {
        for (fsize_t w = 0; w < this->width; w++) {
            Y += *(srcp++);
        }
        srcp += line;
    }

    // Step offset
    fsize_t offsetUV =  this->heightUV * src->GetPitch(PLANAR_U) * (xy & 3) + 
                        this->widthUV  * (xy >> 2);
    register const pixel_t* srcUp = src->GetReadPtr(PLANAR_U) + offsetUV;
    register const pixel_t* srcVp = src->GetReadPtr(PLANAR_V) + offsetUV;

    register average_t U = 0;
    register average_t V = 0;

    for (fsize_t h = 0; h < this->heightUV; h++) {
        for (fsize_t w = 0; w < this->widthUV; w++) {
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
    return CRC(Sublight::YuvToRgb(Y, U, V) + 
              (xy << 4) + Sublight::SIGNATURE);
}

inline void Sublight::SetSizes(const PVideoFrame src) {
    this->width  = src->GetRowSize() >> 2;
    this->height = src->GetHeight() >> 2;
    this->line   = src->GetPitch() - this->width;

    (this->*_setSizesBpp)(src);
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

    this->averageSizeUV = this->widthUV * this->heightUV;
}


const coord_t Sublight::frames[] = {0x0, 0x4, 0x8, 0xC,
                                    0x1,           0xD,
                                    0x2,           0xE,
                                    0x3,           0xF};

const sectorNum_t Sublight::framesAmount = sizeof(Sublight::frames) /
                                           sizeof(Sublight::frames[0]);

/*
 * Frame generator
 */
PVideoFrame __stdcall Sublight::GetFrame(int n, IScriptEnvironment* env) {
    const PVideoFrame src = child->GetFrame(n, env);

    this->SetSizes(src);

    for (sectorNum_t i = 0; i < Sublight::framesAmount; ++i) {
        this->Send((this->*_getAv)(src, Sublight::frames[i]));
    }

    return src;
}
