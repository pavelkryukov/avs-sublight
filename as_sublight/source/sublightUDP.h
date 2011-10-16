/*
 * sublightUDP.h
 *
 * Sublight filter with UDP socket transmission.
 *
 * Copyright 2011 Pavel Kryukov (C)
*/

#ifndef AS_SUBLIGHT_SOURCE_SUBLIGHTUDP_H_
#define AS_SUBLIGHT_SOURCE_SUBLIGHTUDP_H_

// Windows socket header
#include <WinSock.h>

// AviSynth SDK
#include "./sublight.h"

// Types
#include "./types.h"

class SublightUDP : public Sublight {
  private:
    SOCKET _sd;
    sockaddr_in dest_addr;
    virtual void Send(uint64 data) const;
  public:
    SublightUDP(PClip child, const unsigned __int16 port, const char* const ip);
    virtual _stdcall ~SublightUDP();
};

#endif  // AS_SUBLIGHT_SOURCE_SUBLIGHTUDP_H_
