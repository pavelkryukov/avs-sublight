/*
 * sublight.h
 *
 * AVS filter for founding average color on frame sides
 *
 * Copyright 2011 Kryukov Pavel.
*/

#ifndef AS_SUBLIGHT_SOURCE_SUBLIGHT_H_
#define AS_SUBLIGHT_SOURCE_SUBLIGHT_H_

// WinAPI
#include <Windows.h>

// AviSynth SDK
#include "./avisynth.h"

// Types
#include "./types.h"

class Sublight : public GenericVideoFilter {
  private:
    // Signature of sender
    static const coord_t SIGNATURE = 0x0C;

    // Trim 4-byte number to 1-byte
    static inline packet_t CUTS(signed __int32 a) {
        return (a > 0xFF) ? 0xFF : 
               (a < 0x00) ? 0x00 : static_cast<packet_t>(a);
    }

    static inline packet_t CRC(packet_t source) {
        packet_t sourceOrig = source;
        coord_t sum = 0;
        for (; source > 0; source >>= 1) {
            sum += source & 1; 
        }
        return sourceOrig + (sum & 3);
    }

    // YUV to RGB converter
    static packet_t YuvToRgb(average_t Y, average_t U, average_t V);

    // Size setter
    inline void SetSizes(const PVideoFrame src);

    // Size setter for different colorspaces
    void(Sublight::*const _setSizesBpp)(const PVideoFrame src);
    void SetSizesRGB24(const PVideoFrame src);
    void SetSizesRGB32(const PVideoFrame src);
    void SetSizesYUY2(const PVideoFrame src);
    void SetSizesYV12(const PVideoFrame src);

    // Sizes
    fsize_t width;
    fsize_t height;
    fsize_t line;
    fsize_t averageSize;

    // UV sizes
    fsize_t widthUV;
    fsize_t heightUV;
    fsize_t lineUV;
    fsize_t averageSizeUV;

    // Average counters
    packet_t(Sublight::*const _getAv)(const PVideoFrame src, coord_t xy) const;
    packet_t GetAvRGB24(const PVideoFrame src, coord_t xy) const;
    packet_t GetAvRGB32(const PVideoFrame src, coord_t xy) const;
    packet_t GetAvYUY2(const PVideoFrame src, coord_t xy) const;
    packet_t GetAvYV12(const PVideoFrame src, coord_t xy) const;

    // Sending method (virtual)
    virtual void Send(packet_t data) const = 0;

    // Working frames array
    static const coord_t frames[];
    static const sectorNum_t framesAmount;
  public:
    // Constructor
    explicit Sublight(PClip child);

    // DO NOT make this method 'const'
    PVideoFrame __stdcall GetFrame(int n, IScriptEnvironment* env);

    // Destructor
    virtual __stdcall ~Sublight() {}
};

#endif  // AS_SUBLIGHT_SOURCE_SUBLIGHT_H_
