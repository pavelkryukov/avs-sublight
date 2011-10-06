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
    static unsigned __int32 YUVtoRGB(unsigned __int32 Y, unsigned __int32 U, unsigned __int32 V);
    
    unsigned __int32 GetAverageYV12(PVideoFrame src, bool side) const;
    unsigned __int32 GetAverageIL(PVideoFrame src, bool side) const;

    unsigned __int32(Sublight::*getAverage)(PVideoFrame src, bool side) const;

    virtual void Send(unsigned __int64 data);
  public:
    Sublight(PClip child);
    PVideoFrame __stdcall GetFrame(int n, IScriptEnvironment* env);
};

#endif  // __SUBLIGHT_H_
