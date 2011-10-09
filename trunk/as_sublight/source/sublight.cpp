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
unsigned __int32 Sublight::YUVtoRGB(unsigned __int32 Y,
                                    unsigned __int32 U,
                                    unsigned __int32 V) {
    const signed __int32 Yp = 298 * ((signed __int32)Y - 16) + 128;

    const signed __int32 R = (Yp + 409 * ((signed __int32)V - 128)) >> 8;
    const signed __int32 G = (Yp - 100 * ((signed __int32)U - 128) -
                                   208 * ((signed __int32)V - 128)) >> 8;
    const signed __int32 B = (Yp + 516 * ((signed __int32)U - 128)) >> 8;

    return Sublight::PACKRGBS(R, G, B);
}

unsigned __int32 Sublight::GetAverageIL(const PVideoFrame src, bool side) const {
    // Get sizes
    const unsigned width  = src->GetRowSize();
    const unsigned height = src->GetHeight();
    const unsigned pitch  = src->GetPitch();

    // Get source pointers
    // L - is beginning of the frame
    // R - is 3/4 of the first line
    const unsigned __int8* srcp = side ? src->GetReadPtr() :
                           src->GetReadPtr() + (width >> 1)  + (width >> 2);

    // width_w - working area width
    // we will count average on working area
    const unsigned width_w = width >> 2;

    // averageSize - size of working area in pixels
    const unsigned __int32 averageSize = width_w * height / _bpp;

    // average stores
    unsigned __int32 average[4] = {0, 0, 0, 0};

    for (unsigned h = 0; h < height; h++) {
        for (unsigned w = 0; w < width_w;) {
             average[0] += *(srcp + w++);
             average[1] += *(srcp + w++);
             average[2] += *(srcp + w++);
             if (_bpp == 4) {
                 average[3] += *(srcp + w++);
             }
         }
         srcp += pitch;
    }

    average[0] /= averageSize;
    average[1] /= averageSize;
    average[2] /= averageSize;
    average[3] /= averageSize;

    // If format is RGB, collect colors into int32
    return this->vi.IsRGB() ? Sublight::PACKRGB(average[2],
                                                average[1],
                                                average[0]) :
                              Sublight::YUVtoRGB((average[0] >> 1) +
                                                 (average[2] >> 1),
                                                 average[1],
                                                 average[3]);
}

unsigned __int32 Sublight::GetAverageYV12(const PVideoFrame src,
                                            bool side) const {
    // Get sizes
    const unsigned width  = src->GetRowSize();
    const unsigned height = src->GetHeight();
    const unsigned pitch  = src->GetPitch();

    const unsigned widthUV  = src->GetRowSize(PLANAR_U);
    const unsigned pitchUV  = src->GetPitch(PLANAR_U);
    const unsigned heightUV = src->GetHeight(PLANAR_U);

    // Get source pointers
    // L - is beginning of the frame
    // R - is 3/4 of the first line
    const unsigned __int8* srcp = src->GetReadPtr();
    const unsigned __int8* srcUp = src->GetReadPtr(PLANAR_U);
    const unsigned __int8* srcVp = src->GetReadPtr(PLANAR_V);
    if (!side) {
        srcp  += (width >> 1) + (width >> 2);
        srcUp += (widthUV >> 1)  + (widthUV >> 2);
        srcVp += (widthUV >> 1)  + (widthUV >> 2);
    }

    // width_w - working area width
    // we will count average on working area
    const unsigned width_w = width >> 2;

    // Average stores
    register unsigned __int32 average = 0;

    // Counting average on Y
    for (unsigned h = 0; h < height; h++) {
        for (unsigned w = 0; w < width_w; w++) {
            average += *(srcp + w);
        }
        srcp += pitch;
    }

    // If frame is YV12, repeat same operation on planar U and V
    const unsigned widthUV_w = widthUV >> 2;
    const unsigned __int32 averageUVSize = widthUV_w * heightUV;

    register unsigned __int32 averageU = 0;

    for (unsigned h = 0; h < heightUV; h++) {
        for (unsigned w = 0; w < widthUV_w; w++) {
             averageU += *(srcUp + w);
        }
        srcUp += pitchUV;
    }

    register unsigned __int32 averageV = 0;

    for (unsigned h = 0; h < heightUV; h++) {
         for (unsigned w = 0; w < widthUV_w; w++) {
             averageV += *(srcVp + w);
        }
        srcVp += pitchUV;
    }

    // Make output bytes
    return Sublight::YUVtoRGB(average / (width_w * height),
                              averageU / averageUVSize,
                              averageV / averageUVSize);
}


/*
 * Frame generator
 */
PVideoFrame __stdcall Sublight::GetFrame(int n, IScriptEnvironment* env) {
    const PVideoFrame src = child->GetFrame(n, env);

    this->Send(Sublight::PACK((this->*_getAverage)(src, true),
                              (this->*_getAverage)(src, false)));

    return src;
}
