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
    static inline packet_t CUTS(signed __int32 a);
    // YUV to RGB converter
    static inline packet_t YuvToRgb(average_t Y, average_t U, average_t V);

    // Control sum checking and adding
    static inline packet_t CRC(packet_t source);

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
    fsize_t yOffset;

    // UV sizes
    fsize_t widthUV;
    fsize_t heightUV;
    fsize_t lineUV;
    fsize_t averageSizeUV;
    fsize_t yOffsetUV;

    // Average counters
    packet_t(Sublight::*const _getAv)(const PVideoFrame src, coord_t xy) const;
    packet_t GetAvRGB24(const PVideoFrame src, coord_t xy) const;
    packet_t GetAvRGB32(const PVideoFrame src, coord_t xy) const;
    packet_t GetAvYUY2(const PVideoFrame src, coord_t xy) const;
    packet_t GetAvYV12(const PVideoFrame src, coord_t xy) const;

    // Sending method (virtual)
    virtual void Send(packet_t data) const = 0;

    // Working frames array

    /*  | 0  4  8  C |
     *  | 1  5  9  D |
     *  | 2  6  A  E |
     *  | 3  7  B  F |
     */
    static const frames_t frames = 0xF3E2C1D840;
  public:
    // Constructor
    explicit Sublight(PClip child);

    // DO NOT make this method 'const'
    PVideoFrame __stdcall GetFrame(int n, IScriptEnvironment* env);

    // Destructor
    virtual __stdcall ~Sublight() {}
};

#endif  // AS_SUBLIGHT_SOURCE_SUBLIGHT_H_
