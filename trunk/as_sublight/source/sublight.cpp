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


#define CUT(a) ((a > 255) ? 255 : (a < 0 ? 0 : (unsigned __int32)a)) 

/*
 * Converter form YUV to RGB
*/
unsigned __int32 Sublight::YUVtoRGB(unsigned __int32 Y, unsigned __int32 U, unsigned __int32 V) {
    const signed __int32 Yp = 298 * ((signed __int32)Y - 16) + 128;

    const signed __int32 R = (Yp +                                   409 * ((signed __int32)V - 128)) >> 8;
    const signed __int32 G = (Yp - 100 * ((signed __int32)U - 128) - 208 * ((signed __int32)V - 128)) >> 8;
    const signed __int32 B = (Yp + 516 * ((signed __int32)U - 128)                                  ) >> 8;
  
    unsigned __int32 out = 0x00000000;
    out += (CUT(R) << 8) + (CUT(G) << 16) + (CUT(B) << 24);

    return out;
}

unsigned __int64 Sublight::GetAverageIL(PVideoFrame src) const {
            
    // Get sizes
    const unsigned width  = src->GetRowSize();
    const unsigned height = src->GetHeight();
    const unsigned pitch  = src->GetPitch();

    // Get source pointers
    // L - is beginning of the frame
    // R - is 3/4 of the first line
    const unsigned __int8* srcLp = src->GetReadPtr();
    const unsigned __int8* srcRp = srcLp + (width >> 1)  + (width >> 2);

    // width_w - working area width
    // we will count average on working area
    const unsigned width_w = width >> 2;

    // output 32bits
    unsigned __int32 outL;
    unsigned __int32 outR;

    // Get bpp
    const unsigned __int8 bpp = (this->vi.IsYUY2() || this->vi.IsRGB32()) ? 4 : (this->vi.IsRGB24() ? 3 : 1);

    // averageSize - size of working area in pixels
    const unsigned __int32 averageSize = width_w * height / bpp;

    // average stores
    unsigned __int32 averageL[MAXBPP];
    unsigned __int32 averageR[MAXBPP];

    for (unsigned i = 0; i < bpp; i++) {
        averageL[i] = 0;
        averageR[i] = 0;
    }

    for (unsigned h = 0; h < height; h++) {
        for (unsigned w = 0; w < width_w;) {
            for (unsigned i = 0; i < bpp; i++) {
                averageL[i] += *(srcLp + w);
                averageR[i] += *(srcRp + w);
                
                w++;
            }
         }
         
        srcLp += pitch;
        srcRp += pitch;
    }

    for (unsigned i = 0; i < bpp; i++) {
        averageL[i] = averageL[i] / averageSize;
        averageR[i] = averageR[i] / averageSize;
    }
        
    if (this->vi.IsRGB24() || this->vi.IsRGB32()) {

        // If format is RGB, collect colors into int32
        outL = ((unsigned __int8)averageL[0] << 24) + 
               ((unsigned __int8)averageL[1] << 16) + 
               ((unsigned __int8)averageL[2] << 8);

        outR = ((unsigned __int8)averageR[0] << 24) + 
               ((unsigned __int8)averageR[1] << 16) + 
               ((unsigned __int8)averageR[2] << 8);
    }
    else if (this->vi.IsYUY2()) {
        // If format isn't RGB, recount colors into RGB
        outL = YUVtoRGB((averageL[0] >> 1) + (averageL[2] >> 1), averageL[1], averageL[3]);
        outR = YUVtoRGB((averageR[0] >> 1) + (averageR[2] >> 1), averageR[1], averageR[3]);
    }
    else {
        // Unknown format
//        env->ThrowError("ASSERT: Invalid file type");
    }
    
    // Collecting two __int32 into one __int64
    return (((unsigned __int64)outL << 32) + outR) | Sublight::CONTROLMASK;
}

unsigned __int64 Sublight::GetAverageYV12(PVideoFrame src) const {
            
    // Get sizes
    const unsigned width  = src->GetRowSize();
    const unsigned height = src->GetHeight();
    const unsigned pitch  = src->GetPitch();

    // Get source pointers
    // L - is beginning of the frame
    // R - is 3/4 of the first line
    const unsigned __int8* srcLp = src->GetReadPtr();
    const unsigned __int8* srcRp = srcLp + (width >> 1)  + (width >> 2);

    // width_w - working area width
    // we will count average on working area
    const unsigned width_w = width >> 2;

    // output 32bits
    unsigned __int32 outL;
    unsigned __int32 outR;

    // Average stores
    unsigned __int32 averageL = 0;
    unsigned __int32 averageR = 0;

    // Counting average on Y
    for (unsigned h = 0; h < height; h++) {
        for (unsigned w = 0; w < width_w; w++) {
            averageL += *(srcLp + w);
            averageR += *(srcRp + w);
        }
        srcLp += pitch;
        srcRp += pitch;
    }

    // Dividing
    const unsigned __int32 averageSize = width_w * height;

    averageL = averageL / averageSize;
    averageR = averageR / averageSize;

    // If frame is YV12, repeat same operation on planar U and V
    const unsigned widthUV  = src->GetRowSize(PLANAR_U);
    const unsigned pitchUV  = src->GetPitch  (PLANAR_U);
    const unsigned heightUV = src->GetHeight (PLANAR_U);
        
    const unsigned widthUV_w = widthUV >> 2;    
    const unsigned __int32 averageUVSize = widthUV_w * heightUV;
 
    const unsigned offset = (widthUV >> 1)  + (widthUV >> 2);

    const unsigned __int8* srcULp = src->GetReadPtr( PLANAR_U);
    const unsigned __int8* srcURp = srcULp + offset;

    unsigned __int32 averageUL = 0;
    unsigned __int32 averageUR = 0;

    for (unsigned h = 0; h < heightUV; h++) {
        for (unsigned w = 0; w < widthUV_w; w++) {
             averageUL += *(srcULp + w);
             averageUR += *(srcURp + w);
        }
        srcULp += pitchUV;
        srcURp += pitchUV;
    }
        
    averageUL = averageUL / averageUVSize;
    averageUR = averageUR / averageUVSize;

    const unsigned __int8* srcVLp = src->GetReadPtr( PLANAR_V);
    const unsigned __int8* srcVRp = srcVLp + offset;
    
    unsigned __int32 averageVL = 0;
    unsigned __int32 averageVR = 0;
        
    for (unsigned h = 0; h < heightUV; h++) {
         for (unsigned w = 0; w < widthUV_w; w++) {
             averageVL += *(srcVLp + w);
             averageVR += *(srcVRp + w);
        }
        srcVLp += pitchUV;
        srcVRp += pitchUV;
    }

    averageVL = averageVL / averageUVSize;
    averageVR = averageVR / averageUVSize;

    // Make output bytes
    outL = YUVtoRGB(averageL, averageUL, averageVL);
    outR = YUVtoRGB(averageR, averageUR, averageVR);

    // Collecting two __int32 into one __int64
    return (((unsigned __int64)outL << 32) + outR) | Sublight::CONTROLMASK;
}


/*
 * Frame generator
 */
PVideoFrame __stdcall Sublight::GetFrame(int n, IScriptEnvironment* env) {

    const PVideoFrame src = child->GetFrame(n, env);

    this->Send((this->*getAverage)(src));

    return src;
}
