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
                                  _setSizes(vi.IsYV12() ?
                                             &Sublight::SetSizesYV12 :
                                         vi.IsRGB24() ?
                                             &Sublight::SetSizesIL24 :
                                             &Sublight::SetSizesIL32),
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

    return (CUTS(R) + (CUTS(G) << 8) + (CUTS(B) << 16)) << 8;
}

uint32 Sublight::GetAvRGB24(const PVideoFrame src, coord_t xy) const {
    register const pixel* srcp = src->GetReadPtr();

    // Step offset
    srcp += src->GetPitch() * height * (xy & 3) + width * (xy >> 2);

    // average stores
    register uint32 B = 0; // Yes I'm a believer
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
    return ((B + (G << 8) + (R << 16)) << 8) + SIGNATURE + xy;
}

uint32 Sublight::GetAvRGB32(const PVideoFrame src, coord_t xy) const {
    register const pixel* srcp = src->GetReadPtr();

    // Step offset
    srcp += src->GetPitch() * height * (xy & 3) + width * (xy >> 2);

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
    return ((B + (G << 8) + (R << 16)) << 8) + SIGNATURE + xy;
}

uint32 Sublight::GetAvYUY2(const PVideoFrame src, coord_t xy) const {
    register const pixel* srcp = src->GetReadPtr();

    // Step offset
    srcp += src->GetPitch() * height * (xy & 3) + width * (xy >> 2);

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
    return Sublight::YuvToRgb(Y, U, V) + SIGNATURE + xy;
}

uint32 Sublight::GetAvYV12(const PVideoFrame src, coord_t xy) const {
    register const pixel* srcp = src->GetReadPtr();
 
    // Step offset
    srcp += src->GetPitch() * height * (xy & 3) + width * (xy >> 2);

    // Average stores
    register uint32 Y = 0;

    // Counting average on Y
    for (unsigned h = 0; h < height; h++) {
        for (unsigned w = 0; w < width; w++) {
            Y += *(srcp++);
        }
        srcp += line;
    }

    register const pixel* srcUp = src->GetReadPtr(PLANAR_U);
    register const pixel* srcVp = src->GetReadPtr(PLANAR_V);
 
    // Step offset
    srcUp += src->GetPitch(PLANAR_U) * heightUV * (xy & 3) + widthUV * (xy >> 2);
    srcVp += src->GetPitch(PLANAR_V) * heightUV * (xy & 3) + widthUV * (xy >> 2);

    register uint32 U = 0;
    register uint32 V = 0;

    for (unsigned h = 0; h < heightUV; h++) {
        for (unsigned w = 0; w < widthUV; w++) {
             U += *(srcUp++);
             V += *(srcVp++);
        }
        srcUp += lineUV;
    }

    // Make output bytes
    return Sublight::YuvToRgb(Y / averageSize,
                              U / averageSizeUV,
                              V / averageSizeUV) + SIGNATURE + xy;
}


void Sublight::SetSizesIL24(const PVideoFrame src) {
    width  = src->GetRowSize() >> 2;
    height = src->GetHeight() >> 2;
    line   = src->GetPitch() - width;

    averageSize = width * height / 3;
}

void Sublight::SetSizesIL32(const PVideoFrame src) {
    width  = src->GetRowSize() >> 2;
    height = src->GetHeight() >> 2;
    line   = src->GetPitch() - width;

    averageSize = width * height >> 2;
}

void Sublight::SetSizesYV12(const PVideoFrame src) {
    width  = src->GetRowSize() >> 2;
    height = src->GetHeight() >> 2;
    line   = src->GetPitch() - width;

    averageSize = width * height;

    widthUV  = src->GetRowSize(PLANAR_U) >> 2;
    heightUV = src->GetHeight(PLANAR_U) >> 2;
    lineUV   = src->GetPitch(PLANAR_U) - widthUV;

    averageSizeUV = widthUV * heightUV;
}


const coord_t Sublight::frames[] = {0x0, 0x4, 0x8, 0xC, 
                                    0x1,           0xD, 
                                    0x2,           0xE, 
                                    0x3,           0xF};

/*
 * Frame generator
 */
PVideoFrame __stdcall Sublight::GetFrame(int n, IScriptEnvironment* env) {
    const PVideoFrame src = child->GetFrame(n, env);

    (this->*_setSizes)(src);

    const unsigned amount = sizeof(frames) / sizeof(frames[0]);

    for (unsigned i = 0; i < amount; ++i) {
        this->Send((this->*_getAv)(src, frames[i]));
    }

    return src;
}
