/*
 * sublight.h
 *
 * AVS filter for founding average color on frame sides
 *
 * Copyright 2011 Kryukov Pavel.
*/

#ifndef __SUBLIGHT_H_
#define __SUBLIGHT_H_

// WinAPI
#include <Windows.h>

// AviSynth SDK
#include "./avisynth.h"

class Sublight : public GenericVideoFilter {
  private:
    static const unsigned __int8  MAXBPP      = 4;
    static const unsigned __int64 CONTROLMASK = 0x000000EB000000A7;
    static unsigned __int32 YUVtoRGB(unsigned __int32 Y, unsigned __int32 U, unsigned __int32 V);

    unsigned __int64 GetAverageYV12(PVideoFrame src) const;
    unsigned __int64 GetAverageIL(PVideoFrame src) const;

    typedef unsigned __int64(Sublight::*GetAveragePtr)(PVideoFrame src) const;
    GetAveragePtr getAverage;

    virtual void Send(unsigned __int64 data);
  public:
    Sublight(PClip child);
    PVideoFrame __stdcall GetFrame(int n, IScriptEnvironment* env);
};

#endif  // __SUBLIGHT_H_
