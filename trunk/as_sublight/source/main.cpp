/*
 * main.cpp
 *
 * Main DLL source for Ambilight plugin
 *
 * Kryukov Pavel, (C) 2011.
*/

// Windows SDK
#include <windows.h>

// AviSynth SDK
#include <avisynth/avisynth.h>

#include "sublight.h"

/*
 * Call of constructor
*/
AVSValue __cdecl Create_Sublight( AVSValue args, void* user_data, IScriptEnvironment* env) 
{
    return new Sublight( args[0].AsClip());  
}

/*
 * Plugin registering
*/
extern "C" __declspec(dllexport) const char* __stdcall AvisynthPluginInit2( IScriptEnvironment* env) 
{
    env->AddFunction( "Sublight", "c", Create_Sublight, 0);
    return "`Sublight' Sublight plugin";
 }