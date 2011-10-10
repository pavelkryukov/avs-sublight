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
                                  _getAverage(vi.IsYV12() ?
                                                &Sublight::GetAverageYV12 :
                                                &Sublight::GetAverageIL),
                                 _bpp(vi.IsRGB24() ? 3 : 4)                              
                                                {}

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

uint32 Sublight::GetAverageIL(const PVideoFrame src, bool side, unsigned step) const {
    // Get sizes
    const unsigned width       = src->GetRowSize();
    const unsigned width_w     = width >> 2;
    const unsigned width_w_bpp = width_w / _bpp;
    const unsigned height      = src->GetHeight() / Sublight::STEPS;
    const unsigned line        = src->GetPitch() - width_w;
    const unsigned averageSize = width_w_bpp * height;

    // Get source pointers
    // L - is beginning of the frame
    // R - is 3/4 of the first line
    const pixel* srcp = side ? src->GetReadPtr() :
                           src->GetReadPtr() + (width >> 1)  + (width >> 2);

    // Step offset
    srcp += height * step;

    // average stores
    uint32 average[4] = {0, 0, 0, 0};

    for (unsigned h = 0; h < height; h++) {
        for (unsigned w = 0; w < width_w_bpp; w++) {
             average[0] += *(srcp++);
             average[1] += *(srcp++);
             average[2] += *(srcp++);
             if (_bpp == 4) {
                 average[3] += *(srcp++);
             }
         }
         srcp += line;
    }

    average[0] /= averageSize;
    average[1] /= averageSize;
    average[2] /= averageSize;
    average[3] /= averageSize;

    // If format is RGB, collect colors into int32
    return this->vi.IsRGB() ? (average[2] +
                              (average[1] << 8) +
                              (average[0] << 16)) << 8 :
    // Else, if YUY2, convert to RGB
                              Sublight::YuvToRgb((average[0] >> 1) +
                                                 (average[2] >> 1),
                                                 average[1],
                                                 average[3]);
}

uint32 Sublight::GetAverageYV12(const PVideoFrame src, bool side, unsigned step) const {
    // Get sizes
    const unsigned width   = src->GetRowSize();
    const unsigned width_w = width >> 2;
    const unsigned height  = src->GetHeight() / Sublight::STEPS;
    const unsigned line    = src->GetPitch() - width_w;

    const unsigned widthUV   = src->GetRowSize(PLANAR_U);
    const unsigned widthUV_w = widthUV >> 2;
    const unsigned heightUV  = src->GetHeight(PLANAR_U) / Sublight::STEPS;
    const unsigned lineUV    = src->GetPitch(PLANAR_U) - widthUV_w;

    // Get source pointers
    // L - is beginning of the frame
    // R - is 3/4 of the first line
    const pixel* srcp = src->GetReadPtr();
    const pixel* srcUp = src->GetReadPtr(PLANAR_U);
    const pixel* srcVp = src->GetReadPtr(PLANAR_V);
    if (!side) {
        srcp  += width_w   + (width_w << 1);
        srcUp += widthUV_w + (widthUV_w << 1);
        srcVp += (widthUV >> 1)  + (widthUV >> 2);
    }

    // Step offset
    srcp  += height * step;
    srcUp += heightUV * step;
    srcVp += heightUV * step;

    // Average stores
    register uint32 average = 0;

    // Counting average on Y
    for (unsigned h = 0; h < height; h++) {
        for (unsigned w = 0; w < width_w; w++) {
            average += *(srcp++);
        }
        srcp += line;
    }

    register uint32 averageU = 0;

    for (unsigned h = 0; h < heightUV; h++) {
        for (unsigned w = 0; w < widthUV_w; w++) {
             averageU += *(srcUp++);
        }
        srcUp += lineUV;
    }

    register uint32 averageV = 0;

    for (unsigned h = 0; h < heightUV; h++) {
         for (unsigned w = 0; w < widthUV_w; w++) {
             averageV += *(srcVp++);
        }
        srcVp += lineUV;
    }

    const unsigned averageUVSize = widthUV_w * heightUV;

    // Make output bytes
    return Sublight::YuvToRgb(average / (width_w * height),
                              averageU / averageUVSize,
                              averageV / averageUVSize);
}


/*
 * Frame generator
 */
PVideoFrame __stdcall Sublight::GetFrame(int n, IScriptEnvironment* env) {
    const PVideoFrame src = child->GetFrame(n, env);

    this->Send(Sublight::PACK((this->*_getAverage)(src, true, 0),
                              (this->*_getAverage)(src, false, 0)));

    return src;
}
