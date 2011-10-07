/*
 * subilight.cpp
 *
 * AVS filter for founding average color on frame sides
 *
 * Copyright 2011 Kryukov Pavel.
*/

#include "sublight.h"
/*
 * Constructor
 */
Sublight::Sublight(PClip child) : GenericVideoFilter(child),
                                  getAverage( vi.IsYV12() ? &Sublight::GetAverageYV12 : &Sublight::GetAverageIL) {}

/*
 * Abstract sender
*/
void Sublight::Send(unsigned __int64 data) {}

// Cutting to one byte
#define CUT(a)     ((a > 0xFF) ? 0xFF : (a < 0x00 ? 0x00 : (unsigned __int32)a)) 

// Collecting two __int32 into one __int64
#define PACK(l, r) (((unsigned __int64)l << 32) + r) | 0x000000EB000000A7

/*
 * Converter form YUV to RGB
*/
unsigned __int32 Sublight::YUVtoRGB(unsigned __int32 Y, unsigned __int32 U, unsigned __int32 V) {
    const signed __int32 Yp = 298 * ((signed __int32)Y - 16) + 128;

    const signed __int32 R = (Yp +                                   409 * ((signed __int32)V - 128)) >> 8;
    const signed __int32 G = (Yp - 100 * ((signed __int32)U - 128) - 208 * ((signed __int32)V - 128)) >> 8;
    const signed __int32 B = (Yp + 516 * ((signed __int32)U - 128)                                  ) >> 8;
  
    return (CUT(R) << 8) + (CUT(G) << 16) + (CUT(B) << 24);
}

unsigned __int32 Sublight::GetAverageIL(PVideoFrame src, bool side) const {
            
    // Get sizes
    const unsigned width  = src->GetRowSize();
    const unsigned height = src->GetHeight();
    const unsigned pitch  = src->GetPitch();

    // Get source pointers
    // L - is beginning of the frame
    // R - is 3/4 of the first line
    const unsigned __int8* srcp = side ? src->GetReadPtr() : src->GetReadPtr() + (width >> 1)  + (width >> 2);

    // width_w - working area width
    // we will count average on working area
    const unsigned width_w = width >> 2;

    // Get bpp
    const unsigned __int8 bpp = (this->vi.IsYUY2() || this->vi.IsRGB32()) ? 4 : (this->vi.IsRGB24() ? 3 : 1);

    // averageSize - size of working area in pixels
    const unsigned __int32 averageSize = width_w * height / bpp;

    // average stores
    unsigned __int32 average[4] = {0, 0, 0, 0};

    for (unsigned h = 0; h < height; h++) {
        for (unsigned w = 0; w < width_w;) {
            for (unsigned i = 0; i < bpp; i++) {
                average[i] += *(srcp + w++);
            }
         }
         srcp += pitch;
    }

    for (unsigned i = 0; i < bpp; i++) {
        average[i] = average[i] / averageSize;
    }
        
    if (this->vi.IsRGB24() || this->vi.IsRGB32()) {

        // If format is RGB, collect colors into int32
        return ((unsigned __int32)CUT(average[0]) << 24) + 
               ((unsigned __int32)CUT(average[1]) << 16) + 
               ((unsigned __int32)CUT(average[2]) << 8);
    }
    else if (this->vi.IsYUY2()) {
        // If format isn't RGB, recount colors into RGB
        return YUVtoRGB((average[0] >> 1) + (average[2] >> 1), average[1], average[3]);
    }
    else {
        return 0;
    }
    
    // Collecting two __int32 into one __int64
}

unsigned __int32 Sublight::GetAverageYV12(PVideoFrame src, bool side) const {
            
    // Get sizes
    const unsigned width  = src->GetRowSize();
    const unsigned height = src->GetHeight();
    const unsigned pitch  = src->GetPitch();

    const unsigned widthUV  = src->GetRowSize(PLANAR_U);
    const unsigned pitchUV  = src->GetPitch  (PLANAR_U);
    const unsigned heightUV = src->GetHeight (PLANAR_U);

    // Get source pointers
    // L - is beginning of the frame
    // R - is 3/4 of the first line
    const unsigned __int8* srcp = src->GetReadPtr();
    const unsigned __int8* srcUp = src->GetReadPtr( PLANAR_U);
    const unsigned __int8* srcVp = src->GetReadPtr( PLANAR_V);
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

    // Dividing
    average = average / (width_w * height);

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
        
    averageU = averageU / averageUVSize;

    register unsigned __int32 averageV = 0;
        
    for (unsigned h = 0; h < heightUV; h++) {
         for (unsigned w = 0; w < widthUV_w; w++) {
             averageV += *(srcVp + w);
        }
        srcVp += pitchUV;
    }

    averageV = averageV / averageUVSize;

    // Make output bytes
    return YUVtoRGB(average, averageU, averageV);
}


/*
 * Frame generator
 */
PVideoFrame __stdcall Sublight::GetFrame(int n, IScriptEnvironment* env) {

    const PVideoFrame src = child->GetFrame(n, env);

    this->Send(PACK(
                    (this->*getAverage)(src, true),
                    (this->*getAverage)(src, false)
                    ));

    return src;
}
