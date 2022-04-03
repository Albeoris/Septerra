#include <fstream>
#include <iostream>
#include <filesystem>
#include "Dsound.h"
#include "Decoder.hpp"

using namespace std;
namespace fs = std::filesystem;

struct PathInfo
{
	string Directory;
	string FileName;
	string FileNameWithoutExtension;
};

PathInfo GetPathInfo(fs::path path)
{
	PathInfo info = {};
	string str = path.string();
	int i = str.rfind('\\');	// Windows sep
	if (i == string::npos)
	{
		i = str.rfind('/');	// Unix sep
	}
	if (i == string::npos)
	{
		i = -1;
	}

	info.Directory = str.substr(0, i + 1);
	info.FileName = str.substr(i + 1, str.length() - i);

	int j = str.rfind('.');
	if (j == string::npos)
	{
		j = str.length();
	}
	info.FileNameWithoutExtension = str.substr(i + 1, j - i - 1);

	return info;
}

void convert(fs::path file)
{
	// Check file
	ifstream mp3(file, ios::binary | ios::ate);
	if (!mp3.good())
	{
		cout << "Can't open the specified MP3." << endl;
		return;
	}

	int len = mp3.tellg();
	mp3.seekg(0, mp3.beg);
	if (len < sizeof(Mp3Header))
	{
		cout << "The specified MP3 is too short to be valid." << endl;
		return;
	}

	// Check VSSF header
	uint32_t tmp = 0;
	mp3.read((char*)&tmp, sizeof(uint32_t));
	if (tmp != 0x46535356)
	{
		mp3.seekg(0, mp3.beg);
	}
	else
	{
		cout << "Found VSSF header." << endl;
	}

	Mp3Header hdr{};
	mp3.read((char*)&hdr, sizeof(Mp3Header));

	// Check type
	if (hdr.Type == 0x0003)
	{
		cout << "The specified MP3 is a dialogue and can be opened directly." << endl;
		return;
	}
	uint8_t flag = hdr.BitDepth + hdr.ChannelCount;
	if (flag != 0x11 && flag != 0x12)
	{
		cout << "The specified MP3 has an unsupported bit depth/channel count." << endl;
		return;
	}

	// Print info
	PathInfo info = GetPathInfo(file);
	cout << "File: " << info.FileName
		<< ", " << +hdr.BitDepth << "-bit " << hdr.SampleRate << " hz, "
		<< +hdr.ChannelCount << " channel(s), "
		<< hdr.SampleCount << " samples" << endl;

	// Create input struct
	uint32_t pcmLen = hdr.SampleCount * hdr.ChannelCount * hdr.BitDepth / 8;
	uint32_t mp3Len = len - (int)mp3.tellg();

	struct_v12 v12 = {};
	v12.BitDepth = hdr.BitDepth;
	v12.Channels = hdr.ChannelCount;
	v12.SampleRate = hdr.SampleRate;

	source_info si = {};
	si.BitDepth = hdr.BitDepth;
	si.ChannelCount = hdr.ChannelCount;
	si.DataLen = mp3Len;
	si.field_10 = hdr.SampleCount;
	si.SampleRate = hdr.SampleRate;
	si.TotalPcmLen = pcmLen;

	sound s = {};
	s.dword4 = &v12;
	s.SourceInfo = &si;

	// Create output buffer
	char* input = (char*)malloc(mp3Len);
	char* output = (char*)malloc(pcmLen);
	mp3.read((char*)input, mp3Len);
	Decode(&s, output, pcmLen, input);

	// Create output file
	string wavPath = info.Directory + "converted_" + info.FileNameWithoutExtension + ".wav";
	ofstream out(wavPath, ios::binary);

	// Make WAV header
	// https://docs.microsoft.com/en-us/previous-versions/windows/desktop/ee419050(v=vs.85)
	WAVEFORMATEX wav{};
	wav.wFormatTag = WAVE_FORMAT_PCM;
	wav.nSamplesPerSec = hdr.SampleRate;
	wav.wBitsPerSample = hdr.BitDepth;
	wav.nChannels = hdr.ChannelCount;
	wav.nBlockAlign =
		wav.nChannels * (wav.wBitsPerSample / 8);
	wav.nAvgBytesPerSec =
		wav.nBlockAlign * wav.nSamplesPerSec;

	// Write file
	uint32_t wavLen = sizeof(wav);
	// data, data length, data tag, format, format length, format tag, WAVE tag
	uint32_t riffLen = pcmLen + 4 + 4 + wavLen + 4 + 4 + 4;
	out.write("RIFF", 4);
	out.write((char*)&riffLen, sizeof(uint32_t));
	out.write("WAVEfmt ", 8);
	out.write((char*)&wavLen, sizeof(uint32_t));
	out.write((char*)&wav, wavLen);
	out.write("data", 4);
	out.write((char*)&pcmLen, sizeof(uint32_t));
	out.write(output, pcmLen);
	cout << "Dumped sfx/mus " << wavPath << ", " << pcmLen << " bytes of samples" << endl;

	out.close();
	mp3.close();
}

int main(int argc, char* argv[])
{
	// Sanity checks
	if (argc < 2)
	{
		cout << "Usage: Septerra.AudioConverter.exe [path]" << endl;
		cout << "Path can be either a single MP3 file or a folder of MP3s." << endl;
		system("pause");
		return 1;
	}

	if (!fs::exists(argv[1]))
	{
		cout << "Error: the specified file or directory doesn't exist." << endl;
		return 1;
	}

	if (fs::is_regular_file(argv[1]))
	{
		// Convert a single file
		cout << "Converting " << argv[1] << endl;
		convert(argv[1]);
	}
	else if (fs::is_directory(argv[1]))
	{
		// Convert all files under directory
		cout << "Input directory: " << argv[1] << endl;
		auto iter = fs::directory_iterator(argv[1]);
		for (auto& entry : iter)
		{
			if (entry.is_regular_file() && entry.path().extension() == ".mp3")
			{
				cout << "Converting " << entry.path() << endl;
				convert(entry.path());
			}
			else
			{
				cout << "Skipping " << entry.path() << " (not MP3 file)" << endl;
			}
		}
	}
	else
	{
		cout << "Error: input is neither a file nor a directory, what is it?" << endl;
		return 1;
	}

	cout << "Done." << endl;
	return 0;
}