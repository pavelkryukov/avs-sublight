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
    static const coord_t SIGNATURE = 0xF0;
    
    // Trim 4-byte number to 1-byte
    static inline uint32 CUTS(sint32 a) {
        return (a > 0xFF) ? 0xFF : (a < 0x00 ? 0x00 : (uint32)a);
    }

    // YUV to RGB converter
    static uint32 YuvToRgb(uint32 Y, uint32 U, uint32 V);

    // Size setters
    void(Sublight::*const _setSizes)(const PVideoFrame src);
    void SetSizesIL24(const PVideoFrame src);
    void SetSizesIL32(const PVideoFrame src);
    void SetSizesYV12(const PVideoFrame src);
    
    // Sizes
    unsigned width;
    unsigned height;
    unsigned line;
    unsigned averageSize;

    // UV sizes
    unsigned widthUV;
    unsigned heightUV;
    unsigned lineUV;
    unsigned averageSizeUV;

    // Average counters
    uint32(Sublight::*const _getAv)(const PVideoFrame src, coord_t xy) const;
    uint32 GetAvRGB24(const PVideoFrame src, coord_t xy) const;
    uint32 GetAvRGB32(const PVideoFrame src, coord_t xy) const;
    uint32 GetAvYUY2 (const PVideoFrame src, coord_t xy) const;
    uint32 GetAvYV12 (const PVideoFrame src, coord_t xy) const;

    virtual void Send(uint32 data) const = 0;

    static const coord_t frames[];
  public:
    explicit Sublight(PClip child);

    // DO NOT make this method 'const'
    PVideoFrame __stdcall GetFrame(int n, IScriptEnvironment* env);
	
    virtual __stdcall ~Sublight() {}
};

#endif  // AS_SUBLIGHT_SOURCE_SUBLIGHT_H_
