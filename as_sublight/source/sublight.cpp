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
                                  _getAv(vi.IsYV12() ?
                                             &Sublight::GetAvYV12 :
                                         vi.IsYUY2() ?
                                             &Sublight::GetAvYUY2 :
                                         vi.IsRGB24() ?
                                             &Sublight::GetAvRGB24 :
                                             &Sublight::GetAvRGB32) {

}

/*
 * Converter form YUV to RGB
*/
uint32 Sublight::YuvToRgb(uint32 Y, uint32 U, uint32 V) {
    const sint32 Yp = 298 * ((signed __int32)Y - 16) + 128;

    const sint32 R = (Yp + 409 * ((signed __int32)V - 128)) >> 8;
    const sint32 G = (Yp - 100 * ((signed __int32)U - 128) -
                           208 * ((signed __int32)V - 128)) >> 8;
    const sint32 B = (Yp + 516 * ((signed __int32)U - 128)) >> 8;

    return Sublight::PACKRGBS(R, G, B);
}

uint32 Sublight::GetAvRGB24(const PVideoFrame src, bool side, unsigned step) const {
    // Get sizes
    const unsigned width       = src->GetRowSize() >> 2;
    const unsigned height      = src->GetHeight() >> 2;
    const unsigned line        = src->GetPitch() - width;
    const unsigned averageSize = width * height / 3;

    // Get source pointers
    // L - is beginning of the frame
    // R - is 3/4 of the first line
    register const pixel* srcp = side ? src->GetReadPtr() :
                           src->GetReadPtr() + (width << 1) + width;

    // Step offset
    srcp += src->GetPitch() * height * step;

    // average stores
    register uint32 B = 0;
    register uint32 G = 0;
    register uint32 R = 0;

    for (unsigned h = 0; h < height; h++) {
        for (unsigned w = 0; w < width; w += 3) {
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
    return (B + (G << 8) + (R << 16)) << 8;
}

uint32 Sublight::GetAvRGB32(const PVideoFrame src, bool side, unsigned step) const {
    // Get sizes
    const unsigned width       = src->GetRowSize() >> 2;
    const unsigned height      = src->GetHeight() >> 2;
    const unsigned line        = src->GetPitch() - width;
    const unsigned averageSize = (width * height) >> 2;

    // Get source pointers
    // L - is beginning of the frame
    // R - is 3/4 of the first line
    register const pixel* srcp = side ? src->GetReadPtr() :
                           src->GetReadPtr() + (width << 1) + width;

    // Step offset
    srcp += src->GetPitch() * height * step;

    // average stores
    register uint32 B = 0;
    register uint32 G = 0;
    register uint32 R = 0;

    for (unsigned h = 0; h < height; h++) {
        for (unsigned w = 0; w < width; w += 4) {
            B += *(srcp++);
            G += *(srcp++);
            R += *(srcp++);
            ++srcp;
         }
         srcp += line;
    }

    B /= averageSize;
    G /= averageSize;
    R /= averageSize;

    // Collect colors into int32
    return ((B + (G << 8) + (R << 16)) << 8);
}

uint32 Sublight::GetAvYUY2(const PVideoFrame src, bool side, unsigned step) const {
    // Get sizes
    const unsigned width       = src->GetRowSize() >> 2;
    const unsigned height      = src->GetHeight() >> 2;
    const unsigned line        = src->GetPitch() - width;
    const unsigned averageSize = width * height >> 2;

    // Get source pointers
    // L - is beginning of the frame
    // R - is 3/4 of the first line
    register const pixel* srcp = side ? src->GetReadPtr() :
                           src->GetReadPtr() + (width << 1) + width;

    // Step offset
    srcp += src->GetPitch() * height * step;

    // average stores
    register uint32 Y = 0;
    register uint32 U = 0;
    register uint32 V = 0;

    for (unsigned h = 0; h < height; h++) {
        for (unsigned w = 0; w < width; w += 4) {
            Y += *(srcp++);
            U += *(srcp++);
            Y += *(srcp++);
            V += *(srcp++);
         }
         srcp += line;
    }

    Y /= (averageSize << 1);
    U /= averageSize;
    V /= averageSize;

    // If format is RGB, collect colors into int32
    return Sublight::YuvToRgb(Y, U, V);
}

uint32 Sublight::GetAvYV12(const PVideoFrame src, bool side, unsigned step) const {
    // Get sizes
    const unsigned width   = src->GetRowSize() >> 2;
    const unsigned height  = src->GetHeight() >> 2;
    const unsigned line    = src->GetPitch() - width;

    const unsigned widthUV   = src->GetRowSize(PLANAR_U) >> 2;
    const unsigned heightUV  = src->GetHeight(PLANAR_U) >> 2;
    const unsigned lineUV    = src->GetPitch(PLANAR_U) - widthUV;

    // Get source pointers
    // L - is beginning of the frame
    // R - is 3/4 of the first line
    const pixel* srcp = src->GetReadPtr();
    const pixel* srcUp = src->GetReadPtr(PLANAR_U);
    const pixel* srcVp = src->GetReadPtr(PLANAR_V);
    if (!side) {
        srcp  += width    + (width << 1);
        srcUp += widthUV  + (widthUV << 1);
        srcVp += widthUV  + (widthUV << 1);
    }

    // Step offset
    srcp  += src->GetPitch() * height * step;
    srcUp += src->GetPitch(PLANAR_U) * heightUV * step;
    srcVp += src->GetPitch(PLANAR_U) * heightUV * step;

    // Average stores
    register uint32 Y = 0;

    // Counting average on Y
    for (unsigned h = 0; h < height; h++) {
        for (unsigned w = 0; w < width; w++) {
            Y += *(srcp++);
        }
        srcp += line;
    }

    register uint32 U = 0;
    register uint32 V = 0;

    for (unsigned h = 0; h < heightUV; h++) {
        for (unsigned w = 0; w < widthUV; w++) {
             U += *(srcUp++);
             V += *(srcVp++);
        }
        srcUp += lineUV;
    }

    const unsigned UVSize = widthUV * heightUV;

    // Make output bytes
    return Sublight::YuvToRgb(Y / (width * height),
                              U / UVSize,
                              V / UVSize);
}


/*
 * Frame generator
 */
PVideoFrame __stdcall Sublight::GetFrame(int n, IScriptEnvironment* env) {
    const PVideoFrame src = child->GetFrame(n, env);

    for (int i = 0; i < 4; ++i) {
        this->Send((this->*_getAv)(src, true , i) + 0xFC + i);
        this->Send((this->*_getAv)(src, false, i) + 0xF0 + i);
    }

    return src;
}
