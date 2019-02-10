using System;
using System.Collections.Generic;
using System.IO;
using BitMiracle.LibTiff.Classic;
using Septerra.Core;
using Septerra.Core.AM;

namespace Septerra.Core.Sources
{
    public sealed class TiffReader
    {
        private readonly Tiff _tiff;

        public TiffReader(Tiff tiff)
        {
            _tiff = tiff ?? throw new ArgumentNullException(nameof(tiff));
        }

        public Int32 Width => GetInt32(TiffTag.IMAGEWIDTH);
        public Int32 Height => GetInt32(TiffTag.IMAGELENGTH);
        public String PageName => GetString(TiffTag.PAGENAME);

        public Int32 GetInt32(TiffTag tag)
        {
            return _tiff.GetField(tag)[0].ToInt();
        }

        public String GetString(TiffTag tag)
        {
            return _tiff.GetField(tag)[0].ToString();
        }
    }

    public sealed class TiffImageReader
    {
        private readonly UnsafeList<AMImageHeader> _images = new UnsafeList<AMImageHeader>();
        private readonly UnsafeList<AMPalette> _palettes = new UnsafeList<AMPalette>();
        private readonly List<Dictionary<UInt32, Byte>> _paletteMaps = new List<Dictionary<UInt32, Byte>>();
        private readonly UnsafeList<AMImageSegment> _segments = new UnsafeList<AMImageSegment>();
        private readonly UnsafeList<AMImageLine> _lines = new UnsafeList<AMImageLine>();
        private readonly UnsafeList<Byte> _bufferHandler = new UnsafeList<Byte>(640);
        private readonly MemoryStream _content = new MemoryStream();
        private readonly MemoryStream _result = new MemoryStream();
        private readonly Byte[] _colorMap = new Byte[AMPalette.ColorNumber];
        
        private Tiff _tiff;
        private TiffReader _reader;
        private Int32 _contentOffset;
        private AMHeader _header;
        private AMPalette _currentPalette;

        private Int32 CurrentPosition => (Int32)_result.Position;

        public TiffImageReader()
        {
            _header.MagicNumber = AMHeader.KnownMagicNumber;
            _header.VersionNumber = AMHeader.KnownVersion;
        }

        public UnsafeList<Byte> Read(String inputPath)
        {
            using (_tiff = Tiff.Open(inputPath, "r"))
                return Read();
        }

        public UnsafeList<Byte> Read(String inputPath, Stream fileStream)
        {
            _tiff = Tiff.ClientOpen(inputPath, "r", fileStream, new TiffStream());
            return Read();
        }

        private UnsafeList<Byte> Read()
        {
            _reader = new TiffReader(_tiff);

            (Int32 animationType, AMAnimation[] animations, AMFrame[] frames) = GetMetadata(_tiff);

            try
            {
                ReadPages();

                _header.AnimationType = animationType;

                _header.AnimationCount = animations.Length;
                _header.FrameCount = frames.Length;
                _header.PaletteCount = _palettes.Count;
                _header.ImageHeaderCount = _images.Count;
                _header.ImageContentSize = (Int32)_content.Length;
                _header.ImageSegmentCount = _segments.Count;
                _header.ImageLineCount = _lines.Count;

                unsafe
                {
                    _result.SetPosition(sizeof(AMHeader));

                    _header.AnimationOffset = CurrentPosition;
                    _result.WriteStructs(animations);

                    _header.FrameOffset = CurrentPosition;
                    _result.WriteStructs(frames);

                    _header.PaletteOffset = CurrentPosition;
                    _result.WriteStructs(_palettes.GetBuffer());

                    _header.ImageHeaderOffset = CurrentPosition;
                    _result.WriteStructs(_images.GetBuffer());

                    _header.ImageContentOffset = CurrentPosition;
                    _content.SetPosition(0);
                    _content.CopyTo(_result);

                    _header.ImageSegmentOffset = CurrentPosition;
                    _result.WriteStructs(_segments.GetBuffer());

                    _header.ImageLineOffset = CurrentPosition;
                    _result.WriteStructs(_lines.GetBuffer());

                    _result.SetPosition(0);
                    _result.WriteStruct(_header);
                }

                if (!_result.TryGetBuffer(out var result))
                    throw new NotSupportedException();

                return new UnsafeList<Byte>(result);
            }
            finally
            {
                _images.Clear();
                _paletteMaps.Clear();
                _palettes.Clear();
                _segments.Clear();
                _lines.Clear();

                _contentOffset = 0;
                _content.SetLength(0);
                _result.SetLength(0);
            }
        }

        private void ReadPages()
        {
            Int16 imageNumber = _tiff.NumberOfDirectories();
            _images.EnsureCapacity(imageNumber * 2);

            do
            {
                String pageName = _reader.PageName;
                if (pageName.StartsWith("Image"))
                {
                    ReadColorMap(pageName);
                    ReadImage();
                }
                else if (pageName.StartsWith("Palette"))
                {
                    ReadPalette();
                }
                else
                {
                    throw new NotSupportedException(pageName);
                }
         
            } while (_tiff.ReadDirectory());
        }

        private static Int32 ParsePaletteNumber(String pageName)
        {
            const String palettePrefix = "Palette ";

            Int32 paletteNumberIndex = pageName.LastIndexOf(palettePrefix, StringComparison.Ordinal);
            if (paletteNumberIndex < 0 || !Int32.TryParse(pageName.Substring(paletteNumberIndex + palettePrefix.Length), out var paletteNumber))
                throw new NotSupportedException($"Cannot parse palette index from the name {pageName} of an image page. Expected format: [Image 000, Palette 000]");
            return paletteNumber;
        }

        private void ReadPalette()
        {
            ReadPalette(ref _currentPalette);

            unsafe
            {
                Dictionary<UInt32, Byte> targetMap = new Dictionary<UInt32, Byte>(AMPalette.ColorNumber);
                fixed (ABGRColor* l = &_currentPalette.LastColor)
                {
                    UInt32* ptr = (UInt32*)l;
                    for (Int32 i = AMPalette.ColorNumber - 1; i >= 0; i--, ptr--)
                        targetMap[*ptr] = checked((Byte)i);
                }

                _paletteMaps.Add(targetMap);
            }

            _palettes.Add(_currentPalette);
        }

        private void ReadColorMap(String pageName)
        {
            Int32 paletteNumber = ParsePaletteNumber(pageName);

            ref AMPalette nativePalette = ref _palettes[paletteNumber];
            Dictionary<UInt32, Byte> nativeMap = _paletteMaps[paletteNumber];

            ReadPalette(ref _currentPalette);
            unsafe
            {
                fixed (ABGRColor* currentF = &_currentPalette.FirstColor)
                fixed (ABGRColor* nativeF = &nativePalette.FirstColor)
                {
                    UInt32* currentPtr = (UInt32*)currentF;
                    UInt32* nativePtr = (UInt32*)nativeF;

                    for (Int32 i = 0; i < AMPalette.ColorNumber; i++, currentPtr++, nativePtr++)
                    {
                        UInt32 currentColor = *currentPtr;
                        UInt32 nativeColor = *nativePtr;

                        if (currentColor == nativeColor)
                        {
                            _colorMap[i] = checked((Byte)i);
                        }
                        else if (nativeMap.TryGetValue(currentColor, out var target))
                        {
                            _colorMap[i] = target;
                        }
                        else
                        {
                            _colorMap[i] = 0;

                            BGRColor color = ((ABGRColor*)currentPtr)->BGR;
                            Log.Warning($"Image color (R: {color.Red:D3} G: {color.Green:D3}, B: {color.Blue:D3} isn't present in the palette {paletteNumber}.");
                        }
                    }
                }
            }
        }

        private void ReadPalette(ref AMPalette palette)
        {
            FieldValue[] colorMap = Asserts.HasElements(_tiff.GetField(TiffTag.COLORMAP), 3);
            UInt16[] red = colorMap[0].ToUShortArray();
            UInt16[] green = colorMap[1].ToUShortArray();
            UInt16[] blue = colorMap[2].ToUShortArray();

            Asserts.Expected(red.Length, 256);
            Asserts.Expected(green.Length, 256);
            Asserts.Expected(blue.Length, 256);

            unsafe
            {
                fixed (UInt16* rPtr = &red[0])
                fixed (UInt16* gPtr = &green[0])
                fixed (UInt16* bPtr = &blue[0])
                fixed (ABGRColor* f = &palette.FirstColor)
                fixed (ABGRColor* l = &palette.LastColor)
                {
                    UInt16* r = rPtr;
                    UInt16* g = gPtr;
                    UInt16* b = bPtr;
                    for (ABGRColor* ptr = f; ptr <= l; ptr++, r++, g++, b++)
                    {
                        BGRColor* bgr = &ptr->BGR;
                        bgr->Red = TiffColorMap.ConvertTo8Bit(*r);
                        bgr->Green = TiffColorMap.ConvertTo8Bit(*g);
                        bgr->Blue = TiffColorMap.ConvertTo8Bit(*b);
                    }
                }
            }
        }

        private void ReadImage()
        {
            AMImageHeader image = new AMImageHeader();
            image.Width = checked((UInt16)_reader.Width);
            image.Height = checked((UInt16)_reader.Height);
            image.ImageLineIndex = (UInt32)_lines.Count;

            Asserts.Expected(_reader.GetInt32(TiffTag.PHOTOMETRIC), (Int32)Photometric.PALETTE);
            _tiff.SetField(TiffTag.PHOTOMETRIC, Photometric.PALETTE);

            UInt16 imageWidth = image.Width;
            Byte[] buff = this._bufferHandler.GetBuffer(imageWidth * 2).Array;

            for (Int32 y = 0; y < image.Height; y++)
            {
                if (!_tiff.ReadScanline(buff, y))
                    throw new EndOfStreamException("Unexpected end of stream.");

                AMImageSegment segment = new AMImageSegment {Offset = _contentOffset};
                AMImageLine line = new AMImageLine {ImageSegmentIndex = (UInt32)_segments.Count};

                Boolean padding = true;
                for (Int32 x = 0; x < imageWidth; x++)
                {
                    Byte value = _colorMap[buff[x]];
                    if (value == 0)
                    {
                        if (!padding)
                        {
                            padding = true;
                            Flush();
                        }

                        segment.LeftPadding++;
                    }
                    else
                    {
                        padding = false;
                        segment.SizeInBytes++;
                        _content.WriteByte(value);
                    }
                }

                Flush();

                line.ImageSegmentCount = (UInt32)(_segments.Count - line.ImageSegmentIndex);
                _lines.Add(line);

                // --------------------------------------------------------------
                void Flush()
                {
                    if (segment.SizeInBytes == 0 && segment.LeftPadding == 0)
                        return;

                    _contentOffset += segment.SizeInBytes;

                    _segments.Add(segment);

                    segment = new AMImageSegment {Offset = _contentOffset};
                }

                // --------------------------------------------------------------
            }

            _images.Add(image);
        }

        private static (Int32 animationType, AMAnimation[] animations, AMFrame[] frames) GetMetadata(Tiff tiff)
        {
            FieldValue[] xmlPacket = Asserts.HasElements(tiff.GetField(TiffTag.XMLPACKET), minCount: 2);

            Int32 dataLength = (Int32)xmlPacket[0].Value;
            Byte[] data = (Byte[])xmlPacket[1].Value;

            if (dataLength != data.Length)
                Asserts.Expected(data.Length, dataLength);

            return ImageMeta.FromXml(data);
        }
    }
}