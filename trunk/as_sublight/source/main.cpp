/*
 * main.cpp
 *
 * Main DLL source for AVS-Sublight plugin
 *
 * Copyright 2011 Pavel Kryukov (C)
*/

// Windows SDK
#include <windows.h>

// AviSynth SDK
#include "./avisynth.h"

#include "./sublightUDP.h"

/*
 * Call of constructor
*/
AVSValue __cdecl Create_Sublight(AVSValue args,
                                 void* user_data,
                                 IScriptEnvironment* env) {
    return new SublightUDP(args[0].AsClip(),
                           args[1].AsInt(),
                           args[2].AsString());
}

/*
 * Plugin registering
*/
extern "C"
__declspec(dllexport)
const char* __stdcall AvisynthPluginInit2(IScriptEnvironment* env) {
    env->AddFunction("Sublight", "c[PORT]i[IP]s", Create_Sublight, 0);
    return "`Sublight' Sublight plugin";
}
