/*
 * subilight.cpp
 *
 *
 * Copyright 2011 Kryukov Pavel.
*/

// Standard exception
#include <exception>

#include "sublight.h"

/*
 * Constructor
 */
Sublight::Sublight(PClip child, unsigned __int16 port) : GenericVideoFilter(child), _port(port) {
    WSADATA wsadata = {};
    WSAStartup (MAKEWORD (2, 2), &wsadata);

    this->_sd = socket (AF_INET, SOCK_DGRAM, IPPROTO_UDP);

    sockaddr_in addr = {};
    addr.sin_family = AF_INET;
    addr.sin_addr.S_un.S_addr = inet_addr ("255.255.255.255");
    addr.sin_port = _port;

    connect (this->_sd, (sockaddr*)&addr, sizeof(sockaddr_in));
}

/*
 * Destructor
 */
Sublight::~Sublight() {
    closesocket (this->_sd);
}

/*
 * Converter form YUV to RGB
*/
unsigned __int32 Sublight::YUVtoRGB(unsigned __int64 Y, unsigned __int64 U, unsigned __int64 V) {
    const signed __int32 R = (298 * (Y - 16) + 409 * (V - 128) + 128) >> 8;
    __int32 out = R > 255 ? 255 : (R < 0 ? 0 : R) << 8;

    const signed __int32 G = ((298 * (Y - 16) - 100 * (U - 128) - 208 * (V - 128) + 128) >> 8);
    out += (G > 255 ? 255 : (G < 0 ? 0 : G)) << 16;

    const signed __int32 B = ((298 * (Y - 16) + 516 * (U - 128) + 128) >> 8);
    out += (B > 255 ? 255 : (B < 0 ? 0 : B)) << 24;

    out |= 0xFF;

    return out;
}

/*
 * Frame generator
 */
PVideoFrame __stdcall Sublight::GetFrame(int n, IScriptEnvironment* env) {

    // Get source
    const PVideoFrame src = child->GetFrame(n, env);

    // Get bpp
    const unsigned __int8 bpp = (this->vi.IsYUY2() || this->vi.IsRGB32()) ? 4 : (this->vi.IsRGB24() ? 3 : 1);

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

    // averageSize - size of working area in pixels
    const unsigned __int32 averageSize = width_w * height / bpp;

    // average stores
    unsigned __int32* const averageL = new unsigned __int32[bpp];
    unsigned __int32* const averageR = new unsigned __int32[bpp];

    for (unsigned i = 0; i < bpp; i++) {
        averageL[i] = 0;
        averageR[i] = 0;
    }

    // counting average on main frame
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

    // divide
    for (unsigned i = 0; i < bpp; i++) {
        averageL[i] = averageL[i] / averageSize;
        averageR[i] = averageR[i] / averageSize;
    }

    // output 32bits
    unsigned __int32 outL;
    unsigned __int32 outR;

    if (this->vi.IsYV12()) {

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
        outL = YUVtoRGB(averageL[0], averageUL, averageVL);
        outR = YUVtoRGB(averageR[0], averageUR, averageVR);
    }
    else if (this->vi.IsRGB24() || this->vi.IsRGB32()) {

        // If format is RGB, collect colors into int32
        outL = ((unsigned __int8)averageL[0] << 24) + 
               ((unsigned __int8)averageL[1] << 16) + 
               ((unsigned __int8)averageL[2] << 8);
        outL |= 0xFF;

        outR = ((unsigned __int8)averageR[0] << 24) + 
               ((unsigned __int8)averageR[1] << 16) + 
               ((unsigned __int8)averageR[2] << 8);
        outR |= 0xFF;
    }
    else if (this->vi.IsYUY2()) {

        // If format isn't RGB, recount colors into RGB
        outL = YUVtoRGB((averageL[0] >> 1) + (averageL[2] >> 1), averageL[1], averageL[3]);
        outR = YUVtoRGB((averageR[0] >> 1) + (averageR[2] >> 1), averageR[1], averageR[3]);
    }
    else {

        // Unknown format
        delete[] averageL;
        delete[] averageR;
        throw new std::exception("Invalid file type");
    }

    // Cleaning
    delete[] averageL;
    delete[] averageR;

    // Collecting two __int32 into one __int64
    unsigned __int64 data = ((unsigned __int64)outL << 32) + outR;

    // Sending to socket
    send (_sd, (char*)&data, sizeof(data), 0);

    // Return source
    return src;
}
