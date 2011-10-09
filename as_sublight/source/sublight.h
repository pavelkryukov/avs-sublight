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

class Sublight : public GenericVideoFilter {
  private:
    // Trim 4-byte number to 1-byte
    static inline unsigned __int32 CUT(unsigned __int32 a) {
        return a > 0xFF ? 0xFF : a;
    }

    static inline unsigned __int32 CUTS(signed __int32 a) {
        return (a > 0xFF) ? 0xFF : (a < 0x00 ? 0x00 : (unsigned __int32)a);
    }

    // Packing r, g, b into one byte
    static inline unsigned __int32 PACKRGB(unsigned __int32 r,
                                           unsigned __int32 g,
                                           unsigned __int32 b) {
        return (CUT(r) + (CUT(g) << 8) + (CUT(b) << 16)) << 8;
    }

    static inline unsigned __int32 PACKRGBS(signed __int32 r,
                                            signed __int32 g,
                                            signed __int32 b) {
        return (CUTS(r) + (CUTS(g) << 8) + (CUTS(b) << 16)) << 8;
    }

    // Collecting two __int32 into one __int64
    static inline unsigned __int64 PACK(unsigned __int32 l,
                                        unsigned __int32 r) {
        return (((unsigned __int64)l << 32) + r) | 0x000000EB000000A7;
    }

    static unsigned __int32 YUVtoRGB(unsigned __int32 Y,
                                     unsigned __int32 U,
                                     unsigned __int32 V);

    unsigned __int32 GetAverageYV12(const PVideoFrame src, bool side) const;
    unsigned __int32 GetAverageIL(const PVideoFrame src, bool side) const;

    unsigned __int32(Sublight::*const _getAverage)(const PVideoFrame src,
                                                   bool side) const;

    virtual void Send(unsigned __int64 data) = 0;

    const unsigned __int8 _bpp;
  public:
    explicit Sublight(PClip child);

    // DO NOT make this method 'const'
    PVideoFrame __stdcall GetFrame(int n, IScriptEnvironment* env);
};

#endif  // AS_SUBLIGHT_SOURCE_SUBLIGHT_H_
