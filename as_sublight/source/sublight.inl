/*
 * sublight.inl
 *
 * AVS filter for founding average color on frame sides
 *
 * Copyright 2011 Kryukov Pavel.
*/

#ifndef AS_SUBLIGHT_SOURCE_SUBLIGHT_INL_
#define AS_SUBLIGHT_SOURCE_SUBLIGHT_INL_

#include "./sublight.h"

inline packet_t Sublight::CRC(packet_t source) {
    packet_t sourceOrig = source;
    coord_t sum = 0;
    while (source > 0) {
        sum += source & 1;
        source >>= 1;
    }
    return sourceOrig + (sum & 3);
}

inline packet_t Sublight::CUTS(signed __int32 a) {
    return (a > 0xFF) ? 0xFF :
           (a < 0x00) ? 0x00 : static_cast<packet_t>(a);
}

inline packet_t Sublight::YuvToRgb(average_t Y, average_t U, average_t V) {
    const signed __int32 Yp = 298 * ((signed __int32)Y - 16) + 128;
    const signed __int32 Up = (signed __int32)U - 128;
    const signed __int32 Vp = (signed __int32)V - 128;

    const signed __int32 R = (Yp +            409 * Vp) >> 8;
    const signed __int32 G = (Yp - 100 * Up - 208 * Vp) >> 8;
    const signed __int32 B = (Yp + 516 * Up)            >> 8;

    return (CUTS(R) + (CUTS(G) << 8) + (CUTS(B) << 16)) << 8;
}

inline void Sublight::SetSizes(const PVideoFrame src) {
    this->width   = src->GetRowSize() >> 2;
    this->height  = src->GetHeight() >> 2;
    this->line    = src->GetPitch() - this->width;
    this->yOffset = src->GetPitch() * this->height;

    (this->*_setSizesBpp)(src);
}

inline const pixel_t* Sublight::getPointer(const PVideoFrame src,
                                           coord_t xy) const {
    return src->GetReadPtr() + this->yOffset * (xy & 3) +
        this->width * (xy >> 2);
}

#endif  // AS_SUBLIGHT_SOURCE_SUBLIGHT_INL_
