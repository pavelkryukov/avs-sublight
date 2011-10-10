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
    // Trim 4-byte number to 1-byte
    static inline uint32 CUTS(sint32 a) {
        return (a > 0xFF) ? 0xFF : (a < 0x00 ? 0x00 : (uint32)a);
    }

    static inline uint32 PACKRGBS(sint32 r, sint32 g, sint32 b) {
        return (CUTS(r) + (CUTS(g) << 8) + (CUTS(b) << 16)) << 8;
    }

    // Collecting two __int32 into one __int64
    static inline uint64 PACK(uint32 l, uint32 r) {
        return (((uint64)l << 32) + r) | 0x000000EB000000A7;
    }

    static uint32 YuvToRgb(uint32 Y, uint32 U, uint32 V);

    static const unsigned STEPS = 1;

    uint32 GetAverageYV12(const PVideoFrame src, bool side, unsigned step) const;
    uint32 GetAverageIL(const PVideoFrame src, bool side, unsigned step) const;

    uint32(Sublight::*const _getAverage)(const PVideoFrame src, 
                                         bool side,
                                         unsigned step) const;

    virtual void Send(uint64 data) const = 0;

    const bpp_t _bpp;
  public:
    explicit Sublight(PClip child);

    // DO NOT make this method 'const'
    PVideoFrame __stdcall GetFrame(int n, IScriptEnvironment* env);
};

#endif  // AS_SUBLIGHT_SOURCE_SUBLIGHT_H_
