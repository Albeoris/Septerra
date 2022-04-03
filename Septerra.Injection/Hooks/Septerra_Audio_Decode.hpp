#pragma once

// ReSharper disable CppCStyleCast

#include "Dsound.h"

using namespace Septerra::Core;

namespace SepterraInjection
{
	/**
	 * Structs here are incomplete,
	 * for complete structs see
	 * Septerra.AudioConverter
	 */

	/* 194 */
	struct __declspec(align(2)) struct_v12
	{
		BYTE gap0[16];
		WORD SampleRate;
		BYTE Channels;
		BYTE BitDepth;
	};

	/* 201 */
	struct __declspec(align(4)) source_info
	{
		source_info* unk;
		DWORD DbRecIdx;
		DWORD TotalPcmLen;
		DWORD Pcm;
		char field_10[28];
		DWORD DataLen;
		DWORD Format;
		DWORD field_34;
		short SampleRate;
		BYTE ChannelCount;
		BYTE BitDepth;
	};

	/* 192 */
	struct __declspec(align(4)) sound
	{
		sound* next;
		struct_v12* dword4;
		DWORD dword8;
		source_info* SourceInfo;
		uint32_t dword10;
		unsigned char* Mp3ReadBuf;
		uint32_t Mp3PlayedLen;
	};

	typedef int(__cdecl* Septerra_Audio_Decode)(sound* sound, Byte* out, UInt32 lenNeeded);

	Septerra_Audio_Decode O_Septerra_Audio_Decode = (Septerra_Audio_Decode)0x468540;

	Septerra_Audio_Decode O_Septerra_Audio_Decode_MP2 = (Septerra_Audio_Decode)0x468D60;

	int __cdecl Hook_Decode(sound* sound, Byte* out, UInt32 lenNeeded)
	{
		if (!sound->Mp3PlayedLen)
		{
			// Only log on first decoder call
			char msg[128];
			snprintf(msg, sizeof(msg),
				"Beginning decode of mus/sfx record ID 0x%08x: %d-bit, %d channel(s), %d Hz, %d PCM bytes",
				sound->SourceInfo->DbRecIdx, sound->SourceInfo->BitDepth,
				sound->SourceInfo->ChannelCount, sound->SourceInfo->SampleRate,
				sound->SourceInfo->TotalPcmLen);
			Log::Message(gcnew String(msg));
		}
		return O_Septerra_Audio_Decode(sound, out, lenNeeded);
	}

	Septerra_Audio_Decode H_Septerra_Audio_Decode = Hook_Decode;
}
