/*
 * sublightUDP.h
 *
 * Sublight filter with UDP socket transmission.
 *
 * Copyright 2011 Kryukov Pavel.
*/

#ifndef __SUBLIGHT_UDP_H_
#define __SUBLIGHT_UDP_H_

// Windows socket header
#include <WinSock.h>

// AviSynth SDK
#include "sublight.h"

class SublightUDP : public Sublight {
  private:
    SOCKET _sd;
	sockaddr_in dest_addr;
    virtual void Send(unsigned __int64 data);
  public:
    SublightUDP(PClip child, const unsigned __int16 port, const char* const ip);
    ~SublightUDP();
};

#endif  // __SUBLIGHT_UDP_H_