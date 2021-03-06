/*
 * subilightUDP.cpp
 *
 * Sublight filter with UDP socket transmission.
 *
 * Copyright 2011 Pavel Kryukov (C)
*/

#if _MSC_VER
#pragma comment(lib, "Ws2_32.lib")
#endif

#include "./sublightUDP.h"

SublightUDP::SublightUDP(PClip child,
                         port_t port,
                         const char* const ip) : Sublight(child) {
    WSADATA wsadata = {};
    WSAStartup(MAKEWORD(2, 2), &wsadata);

    this->_sd = socket(AF_INET, SOCK_DGRAM, IPPROTO_UDP);

    dest_addr.sin_family = AF_INET;
    dest_addr.sin_port   = htons(port);
    dest_addr.sin_addr.s_addr = inet_addr(ip);

    BOOL bOptVal = TRUE;
    setsockopt(_sd, SOL_SOCKET, SO_BROADCAST,
            reinterpret_cast<char*>(&bOptVal), sizeof(BOOL));
}

SublightUDP::~SublightUDP() {
    closesocket(this->_sd);
}

void SublightUDP::Send(packet_t data) const {
    sendto(_sd, reinterpret_cast<char*>(&data),  sizeof(data), 0,
        reinterpret_cast<const sockaddr*>(&dest_addr), sizeof(dest_addr));
}
