/*
 * sublight.h
 *
 * sublight plugin for AviSynth
 *
 * Copyright 2011 Kryukov Pavel.
*/

#ifndef __SUBLIGHT_H_
#define __SUBLIGHT_H_

// WinAPI
#include <Windows.h>

// Windows socket header
#include <WinSock.h>

// AviSynth API
#include <avisynth/avisynth.h>

class Sublight : public GenericVideoFilter {
  private:
    const unsigned __int16 _port;
    SOCKET _sd;
    static unsigned __int32 YUVtoRGB(unsigned __int64 Y, unsigned __int64 U, unsigned __int64 V);
  public:
    Sublight(PClip child, const unsigned __int16 port);
    ~Sublight();
    PVideoFrame __stdcall GetFrame(int n, IScriptEnvironment* env);
};

#endif  // __SUBLIGHT_H_
