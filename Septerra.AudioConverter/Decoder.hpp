#pragma once

#include "Windows.h"

#define LOBYTE(x)   (*((BYTE*)&(x)))   // low byte
#define LOWORD(x)   (*((WORD*)&(x)))   // low word
#define HIBYTE(x)   (*((BYTE*)&(x)+1))
#define HIWORD(x)   (*((WORD*)&(x)+1))

#define _BYTE BYTE
#define _WORD WORD
#define _DWORD DWORD

const char g_AudDecodeData1[] =
{
	0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
	0x02, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0x06, 0x00, 0x00, 0x00, 0x08, 0x00, 0x00, 0x00,
	0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
	0x02, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0x06, 0x00, 0x00, 0x00, 0x08, 0x00, 0x00, 0x00
};

const char g_AudDecodeData2_Backing[] =
{
	0x07, 0x00, 0x00, 0x00, 0x08, 0x00, 0x00, 0x00, 0x09, 0x00, 0x00, 0x00, 0x0A, 0x00, 0x00, 0x00,
	0x0B, 0x00, 0x00, 0x00, 0x0C, 0x00, 0x00, 0x00, 0x0D, 0x00, 0x00, 0x00, 0x0E, 0x00, 0x00, 0x00,
	0x10, 0x00, 0x00, 0x00, 0x11, 0x00, 0x00, 0x00, 0x13, 0x00, 0x00, 0x00, 0x15, 0x00, 0x00, 0x00,
	0x17, 0x00, 0x00, 0x00, 0x19, 0x00, 0x00, 0x00, 0x1C, 0x00, 0x00, 0x00, 0x1F, 0x00, 0x00, 0x00,
	0x22, 0x00, 0x00, 0x00, 0x25, 0x00, 0x00, 0x00, 0x29, 0x00, 0x00, 0x00, 0x2D, 0x00, 0x00, 0x00,
	0x32, 0x00, 0x00, 0x00, 0x37, 0x00, 0x00, 0x00, 0x3C, 0x00, 0x00, 0x00, 0x42, 0x00, 0x00, 0x00,
	0x49, 0x00, 0x00, 0x00, 0x50, 0x00, 0x00, 0x00, 0x58, 0x00, 0x00, 0x00, 0x61, 0x00, 0x00, 0x00,
	0x6B, 0x00, 0x00, 0x00, 0x76, 0x00, 0x00, 0x00, 0x82, 0x00, 0x00, 0x00, 0x8F, 0x00, 0x00, 0x00,
	0x9D, 0x00, 0x00, 0x00, 0xAD, 0x00, 0x00, 0x00, 0xBE, 0x00, 0x00, 0x00, 0xD1, 0x00, 0x00, 0x00,
	0xE6, 0x00, 0x00, 0x00, 0xFD, 0x00, 0x00, 0x00, 0x17, 0x01, 0x00, 0x00, 0x33, 0x01, 0x00, 0x00,
	0x51, 0x01, 0x00, 0x00, 0x73, 0x01, 0x00, 0x00, 0x98, 0x01, 0x00, 0x00, 0xC1, 0x01, 0x00, 0x00,
	0xEE, 0x01, 0x00, 0x00, 0x20, 0x02, 0x00, 0x00, 0x56, 0x02, 0x00, 0x00, 0x92, 0x02, 0x00, 0x00,
	0xD4, 0x02, 0x00, 0x00, 0x1C, 0x03, 0x00, 0x00, 0x6C, 0x03, 0x00, 0x00, 0xC3, 0x03, 0x00, 0x00,
	0x24, 0x04, 0x00, 0x00, 0x8E, 0x04, 0x00, 0x00, 0x02, 0x05, 0x00, 0x00, 0x83, 0x05, 0x00, 0x00,
	0x10, 0x06, 0x00, 0x00, 0xAB, 0x06, 0x00, 0x00, 0x56, 0x07, 0x00, 0x00, 0x12, 0x08, 0x00, 0x00,
	0xE0, 0x08, 0x00, 0x00, 0xC3, 0x09, 0x00, 0x00, 0xBD, 0x0A, 0x00, 0x00, 0xD0, 0x0B, 0x00, 0x00,
	0xFF, 0x0C, 0x00, 0x00, 0x4C, 0x0E, 0x00, 0x00, 0xBA, 0x0F, 0x00, 0x00, 0x4C, 0x11, 0x00, 0x00,
	0x07, 0x13, 0x00, 0x00, 0xEE, 0x14, 0x00, 0x00, 0x06, 0x17, 0x00, 0x00, 0x54, 0x19, 0x00, 0x00,
	0xDC, 0x1B, 0x00, 0x00, 0xA5, 0x1E, 0x00, 0x00, 0xB6, 0x21, 0x00, 0x00, 0x15, 0x25, 0x00, 0x00,
	0xCA, 0x28, 0x00, 0x00, 0xDF, 0x2C, 0x00, 0x00, 0x5B, 0x31, 0x00, 0x00, 0x4B, 0x36, 0x00, 0x00,
	0xB9, 0x3B, 0x00, 0x00, 0xB2, 0x41, 0x00, 0x00, 0x44, 0x48, 0x00, 0x00, 0x7E, 0x4F, 0x00, 0x00,
	0x71, 0x57, 0x00, 0x00, 0x2F, 0x60, 0x00, 0x00, 0xCE, 0x69, 0x00, 0x00, 0x62, 0x74, 0x00, 0x00,
	0xFF, 0x7F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
};

const short* g_AudDecodeData2 = (const short*)g_AudDecodeData2_Backing;

struct Mp3Header
{
	uint16_t Type;
	uint16_t Unknown;
	uint32_t SampleCount;
	uint16_t SampleRate;
	uint8_t BitDepth;
	uint8_t ChannelCount;
};

/* 194 */
struct __declspec(align(2)) struct_v12
{
	uint8_t gap0[16];
	uint16_t SampleRate;
	uint8_t Channels;
	uint8_t BitDepth;
};

/* 201 */
struct __declspec(align(4)) source_info
{
	source_info* unk;
	uint32_t DbRecIdx;
	uint32_t TotalPcmLen;
	uint32_t Pcm;
	uint32_t field_10;
	char field_14;
	char field_15;
	char field_16;
	char field_17;
	char field_18;
	char field_19;
	char field_1A;
	char field_1B;
	char field_1C;
	char field_1D;
	char field_1E;
	char field_1F;
	char field_20;
	char field_21;
	char field_22;
	char field_23;
	char field_24;
	char field_25;
	char field_26;
	char field_27;
	char field_28;
	char field_29;
	char field_2A;
	char field_2B;
	uint32_t DataLen;
	uint32_t Format;
	uint32_t field_34;
	short SampleRate;
	uint8_t ChannelCount;
	uint8_t BitDepth;
	char field_3C;
	__declspec(align(4)) char field_40;
	uint32_t IsPCM;
	char field_48;
	__declspec(align(4)) char field_4C;
	__declspec(align(4)) char field_50;
};

/* 202 */
struct __declspec(align(4)) struct_v4
{
	uint32_t* pdword0;
	uint32_t* pdword4;
	uint32_t dword8;
	uint32_t WParam;
	uint32_t LParam;
	uint32_t dword14;
};

/* 192 */
struct __declspec(align(4)) sound
{
	sound* next;
	struct_v12* dword4;
	uint32_t dword8;
	source_info* SourceInfo;
	uint32_t dword10;
	unsigned char* Mp3ReadBuf;
	uint32_t Mp3PlayedLen;
	uint32_t field_1C;
	char* Pcm;
	uint32_t PcmLen;
	uint32_t PcmPlayedLen;
	uint32_t RawPlayedLen;
	uint32_t field_30;
	uint32_t PcmLenLeft;
	uint32_t field_38;
	uint32_t field_3C;
	uint32_t dword40;
	uint8_t gap44[4];
	void* DSBuf;
	uint8_t gap4C[4];
	uint32_t BufBytes;
	uint32_t dword54;
	uint32_t dword58;
	uint8_t gap5C[8];
	uint32_t dword64;
	struct_v4* PStructV4;
	uint32_t State;
	uint32_t field_70;
	char field_74;
	char field_75;
	uint16_t field_76;
	uint32_t dword78;
};

/**
 * Jump cases other than 0x11 and 0x12 are removed;
 * they are never called (we only have 16-bit mono and stereo MP3s)
 */
void Decode(sound* aud, char* out, uint32_t lenNeeded2, char* input)
{
	sound* result; // eax
	source_info* mp3Info; // ecx
	DWORD pcmLen; // edi
	byte* pcm2; // ebx
	unsigned int lenNeeded; // esi
	unsigned int playedLen; // edx
	DWORD mp3PlayedLen; // ecx
	unsigned int lenToCpy; // ecx
	size_t lenToRead; // esi
	int secondOrFourthByte; // edx
	int firstShort2; // ecx
	char fifthByte; // al
	_BYTE* mp3ByteCur; // esi
	_WORD* v18; // edi
	int v19; // eax
	int v20; // eax
	int v21; // eax
	bool v22; // sf
	char v23; // al
	_WORD* v24; // edi
	int v25; // eax
	int v26; // eax
	int v27; // eax
	char v28; // al
	_WORD* v29 = nullptr; // edi
	unsigned __int16 v30; // cx
	int v31; // eax
	int v32; // eax
	int v33; // eax
	char v34; // al
	_WORD* v35; // edi
	int v36; // eax
	int v37; // eax
	int v38; // eax
	size_t readedLen; // [esp+Ch] [ebp-4Ch]
	unsigned __int16 v52; // [esp+18h] [ebp-40h]
	unsigned __int16 v53; // [esp+18h] [ebp-40h]
	unsigned int mp3DataLen; // [esp+20h] [ebp-38h]
	DWORD playedLen2; // [esp+2Ch] [ebp-2Ch]
	unsigned __int8 channelCount; // [esp+30h] [ebp-28h]
	DWORD dbRecordIndex; // [esp+34h] [ebp-24h]
	unsigned __int8 bitDepth; // [esp+38h] [ebp-20h]
	int dstCur; // [esp+3Ch] [ebp-1Ch]
	signed int pcmShortCura; // [esp+40h] [ebp-18h]
	signed int pcmShortCurb; // [esp+40h] [ebp-18h]
	DWORD mp3PlayedLen2; // [esp+44h] [ebp-14h]
	char v68; // [esp+48h] [ebp-10h]
	char v69; // [esp+48h] [ebp-10h]
	char v70; // [esp+48h] [ebp-10h]
	char v71; // [esp+48h] [ebp-10h]
	size_t secondShort; // [esp+4Ch] [ebp-Ch]
	int firstShort; // [esp+50h] [ebp-8h]
	unsigned __int8 sixthByte; // [esp+56h] [ebp-2h]
	char v77; // [esp+57h] [ebp-1h]

	result = aud;
	mp3Info = aud->SourceInfo;
	lenNeeded = lenNeeded2;
	channelCount = mp3Info->ChannelCount;
	dstCur = 0;
	bitDepth = mp3Info->BitDepth;
	mp3DataLen = mp3Info->DataLen;
	dbRecordIndex = mp3Info->DbRecIdx;
	mp3PlayedLen = aud->Mp3PlayedLen;
	mp3PlayedLen2 = aud->Mp3PlayedLen;

	byte mp3[1024] = { 0 };

	// TODO: read the unknown dword
	uint32_t unk = *((DWORD*)(&aud->SourceInfo->field_10));
	lenNeeded = lenNeeded2 = (unk * channelCount * bitDepth) / 8;	// samplecount

	size_t pcmSize;
	switch (bitDepth + channelCount)
	{
	case 0x11:
		pcmSize = 1018 * 4 + 2;
		break;
	case 0x12:
		pcmSize = 1018 * 4 + 4;
		break;
	default:
		printf("The input format is unsupported.\n");
		return;
	}
	byte* pcm = pcm2 = (byte*)malloc(pcmSize);
	pcmLen = playedLen = playedLen2 = pcmSize;
	uint32_t inputCur = 0;

	while (1)
	{
		if (playedLen < pcmLen)
		{
			lenToCpy = pcmLen - playedLen;
			if (pcmLen - playedLen >= lenNeeded)
				lenToCpy = lenNeeded;
			memcpy(out + dstCur, pcm2 + playedLen, lenToCpy);
			dstCur += lenToCpy;
			lenNeeded = lenNeeded2 - lenToCpy;
			playedLen = lenToCpy + playedLen2;
			mp3PlayedLen = mp3PlayedLen2;
			lenNeeded2 = lenNeeded;
		}
		if (!lenNeeded)
			break;
		lenToRead = 1024;
		if (mp3PlayedLen + 1024 > mp3DataLen)
			lenToRead = mp3DataLen - mp3PlayedLen;
		inputCur = mp3PlayedLen;
		memcpy(mp3, input + inputCur, lenToRead);
		readedLen = lenToRead;
		firstShort2 = ((uint16_t*)mp3)[0];
		secondShort = ((uint16_t*)mp3)[1];
		fifthByte = mp3[4];
		sixthByte = mp3[5];
		mp3ByteCur = mp3 + 6;
		firstShort = firstShort2;
		v77 = fifthByte;
		switch (bitDepth + channelCount)
		{
		case 0x11:
			*(_WORD*)pcm2 = firstShort2;
			v29 = (WORD*)(pcm2 + 2);
			pcmShortCurb = 1018;
			break;
		case 0x12:
			*(_WORD*)pcm2 = firstShort2;
			pcmShortCura = 1018;
			*((_WORD*)pcm2 + 1) = secondShort;
			v18 = (WORD*)(pcm2 + 4);
			while (1)
			{
				v68 = *mp3ByteCur >> 4;
				v52 = g_AudDecodeData2[2 * fifthByte];
				v19 = 0;
				if (v68 & 4)
					v19 = v52;
				if (v68 & 2)
					v19 += (signed int)v52 >> 1;
				if (v68 & 1)
					v19 += (signed int)v52 >> 2;
				v20 = ((signed int)v52 >> 3) + v19;
				if (v68 & 8)
					v20 = -v20;
				v21 = (signed __int16)firstShort + v20;
				if (v21 <= 0x7FFF)
				{
					if (v21 < -32768)
						LOWORD(v21) = -32768;
				}
				else
				{
					LOWORD(v21) = 0x7FFF;
				}
				LOWORD(firstShort) = v21;
				v22 = (char)(g_AudDecodeData1[4 * v68] + v77) < 0;
				v23 = g_AudDecodeData1[4 * v68] + v77;
				v77 += g_AudDecodeData1[4 * v68];
				if (v22)
				{
					v77 = 0;
				}
				else if (v23 > 88)
				{
					v77 = 88;
				}
				*v18 = firstShort;
				v24 = v18 + 1;
				v69 = *mp3ByteCur++ & 0xF;
				v25 = 0;
				if (v69 & 4)
					v25 = (unsigned __int16)g_AudDecodeData2[2 * (char)sixthByte];
				if (v69 & 2)
					v25 += (signed int)(unsigned __int16)g_AudDecodeData2[2 * (char)sixthByte] >> 1;
				if (v69 & 1)
					v25 += (signed int)(unsigned __int16)g_AudDecodeData2[2 * (char)sixthByte] >> 2;
				v26 = ((signed int)(unsigned __int16)g_AudDecodeData2[2 * (char)sixthByte] >> 3) + v25;
				if (v69 & 8)
					v26 = -v26;
				v27 = (signed __int16)secondShort + v26;
				if (v27 <= 0x7FFF)
				{
					if (v27 < -32768)
						LOWORD(v27) = -32768;
				}
				else
				{
					LOWORD(v27) = 0x7FFF;
				}
				LOWORD(secondShort) = v27;
				v22 = (char)(g_AudDecodeData1[4 * v69] + sixthByte) < 0;
				v28 = g_AudDecodeData1[4 * v69] + sixthByte;
				sixthByte += g_AudDecodeData1[4 * v69];
				if (v22)
				{
					sixthByte = 0;
				}
				else if (v28 > 88)
				{
					sixthByte = 88;
				}
				*v24 = secondShort;
				v18 = v24 + 1;
				if (!--pcmShortCura)
					break;
				fifthByte = v77;
			}
			goto LABEL_132;
		}
		do
		{
			v70 = *mp3ByteCur >> 4;
			v30 = g_AudDecodeData2[2 * fifthByte];
			v31 = 0;
			if (v70 & 4)
				v31 = v30;
			if (v70 & 2)
				v31 += (signed int)v30 >> 1;
			if (v70 & 1)
				v31 += (signed int)v30 >> 2;
			v32 = ((signed int)v30 >> 3) + v31;
			if (v70 & 8)
				v32 = -v32;
			v33 = (signed __int16)firstShort + v32;
			if (v33 <= 0x7FFF)
			{
				if (v33 < -32768)
					LOWORD(v33) = -32768;
			}
			else
			{
				LOWORD(v33) = 0x7FFF;
			}
			LOWORD(firstShort) = v33;
			v22 = (char)(g_AudDecodeData1[4 * v70] + v77) < 0;
			v34 = g_AudDecodeData1[4 * v70] + v77;
			v77 += g_AudDecodeData1[4 * v70];
			if (v22)
			{
				v34 = 0;
			}
			else
			{
				if (v34 <= 88)
					goto LABEL_66;
				v34 = 88;
			}
			v77 = v34;
		LABEL_66:
			*v29 = firstShort;
			v35 = v29 + 1;
			v71 = *mp3ByteCur++ & 0xF;
			v53 = g_AudDecodeData2[2 * v34];
			v36 = 0;
			if (v71 & 4)
				v36 = v53;
			if (v71 & 2)
				v36 += (signed int)v53 >> 1;
			if (v71 & 1)
				v36 += (signed int)v53 >> 2;
			v37 = ((signed int)v53 >> 3) + v36;
			if (v71 & 8)
				v37 = -v37;
			v38 = (signed __int16)firstShort + v37;
			if (v38 <= 0x7FFF)
			{
				if (v38 < -32768)
					LOWORD(v38) = -32768;
			}
			else
			{
				LOWORD(v38) = 0x7FFF;
			}
			LOWORD(firstShort) = v38;
			v22 = (char)(g_AudDecodeData1[4 * v71] + v77) < 0;
			fifthByte = g_AudDecodeData1[4 * v71] + v77;
			v77 += g_AudDecodeData1[4 * v71];
			if (v22)
			{
				fifthByte = 0;
			}
			else
			{
				if (fifthByte <= 88)
					goto LABEL_83;
				fifthByte = 88;
			}
			v77 = fifthByte;
		LABEL_83:
			*v35 = firstShort;
			v29 = v35 + 1;
			--pcmShortCurb;
		} while (pcmShortCurb);
	LABEL_132:
		mp3PlayedLen2 += readedLen;
		playedLen = 0;
		playedLen2 = 0;
		if (!readedLen)
		{
			result = aud;
			aud->PcmPlayedLen = 0;
			aud->Mp3PlayedLen = mp3PlayedLen2;
			free(pcm);
			return;
		}
		lenNeeded = lenNeeded2;
		result = aud;
		pcmLen = pcmLen;
		mp3PlayedLen = mp3PlayedLen2;
		pcm2 = pcm;
	}
	result->PcmPlayedLen = playedLen;
	result->Mp3PlayedLen = mp3PlayedLen;
	free(pcm);
}