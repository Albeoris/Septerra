﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using BitMiracle.LibTiff.Classic;
using Septerra.Core.AM;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Text;

namespace Septerra.Core
{
    public sealed class ImageTarget : ITarget
    {
        public static readonly ImageTarget Instance = new ImageTarget();

        public void Write(ArraySegment<Byte> segment, String outputPath, UInt16 expectedVersion)
        {
            using (MemoryStream sourceFile = new MemoryStream(segment.Array, 0, segment.Count))
            {
                AMHeader header = sourceFile.ReadStruct<AMHeader>();

                String extension = Path.GetExtension(outputPath);
                switch (extension)
                {
                    case ".zip":
                        ExtractZip(sourceFile, outputPath, in header);
                        break;
                    case ".tiff":
                        ExtractTiff(sourceFile, outputPath, in header);
                        break;
                    default:
                        throw new NotSupportedException(extension);
                }
            }
        }

        private unsafe void ExtractZip(MemoryStream sourceFile, String outputPath, in AMHeader header)
        {
            sourceFile.SetPosition(header.AnimationOffset);
            AMAnimation[] animations = sourceFile.ReadStructs<AMAnimation>(header.AnimationCount);

            sourceFile.SetPosition(header.FrameOffset);
            AMFrame[] freames = sourceFile.ReadStructs<AMFrame>(header.FrameCount);

            ColorPalette[] colorPalettes = ReadColorPalettes(sourceFile, in header);
            Byte[][] actPalettes = ReadActPalettes(sourceFile, in header);

            sourceFile.SetPosition(header.ImageHeaderOffset);
            AMImageHeader[] frames = sourceFile.ReadStructs<AMImageHeader>(header.ImageHeaderCount);

            Dictionary<Int32, Int32> frameDefaultPalettes = GetDefaultFramePalettes(freames, in header);

            sourceFile.SetPosition(header.ImageSegmentOffset);
            AMImageSegment[] segments = sourceFile.ReadStructs<AMImageSegment>(header.ImageSegmentCount);

            sourceFile.SetPosition(header.ImageLineOffset);
            AMImageLine[] lines = sourceFile.ReadStructs<AMImageLine>(header.ImageLineCount);

            ImageMeta meta = new ImageMeta();
            meta.SetAnimationType(header.AnimationType);
            meta.SetAnimation(animations);
            meta.SetFrames(freames);
            var xmlMeta = meta.ToXml();

            sourceFile.SetPosition(header.ImageContentOffset);

            fixed (AMImageSegment* linePtr = segments)
            fixed (AMImageLine* lineExPtr = lines)
            {
                using (FileStream output = File.Create(outputPath))
                using (ZipArchive zip = new ZipArchive(output, ZipArchiveMode.Create, leaveOpen: true, Encoding.UTF8))
                {
                    ZipArchiveEntry metaEntry = zip.CreateEntry("Meta.xml");
                    using (Stream entrySteam = metaEntry.Open())
                        entrySteam.Write(xmlMeta, 0, xmlMeta.Length);

                    Byte[] colors = new Byte[256];
                    for (Int32 i = 0; i < colors.Length; i++)
                        colors[i] = checked((Byte) i);

                    for (Int32 paletteIndex = 0; paletteIndex < actPalettes.Length; paletteIndex++)
                    {
                        using Bitmap bitmap = new Bitmap(16, 16, PixelFormat.Format8bppIndexed);
                        {
                            bitmap.Palette = colorPalettes[paletteIndex];

                            var bitmapData = bitmap.LockBits(new Rectangle(Point.Empty, bitmap.Size), ImageLockMode.ReadWrite, bitmap.PixelFormat);
                            Marshal.Copy(colors, 0, bitmapData.Scan0, colors.Length);
                            bitmap.UnlockBits(bitmapData);

                            ZipArchiveEntry gifEntry = zip.CreateEntry($"Palette {paletteIndex:D3}.gif");
                            using (Stream entrySteam = gifEntry.Open())
                                bitmap.Save(entrySteam, System.Drawing.Imaging.ImageFormat.Gif);
                        }

                        Byte[] actPalette = actPalettes[paletteIndex];
                        ZipArchiveEntry actEntry = zip.CreateEntry($"Palette {paletteIndex:D3}.act");
                        using (Stream entrySteam = actEntry.Open())
                            entrySteam.Write(actPalette, 0, actPalette.Length);
                    }

                    for (Int32 frameIndex = 0; frameIndex < frames.Length; frameIndex++)
                    {
                        AMImageHeader frame = frames[frameIndex];

                        using Bitmap bitmap = new Bitmap(frame.Width, frame.Height, PixelFormat.Format8bppIndexed);
                        {
                            if (!frameDefaultPalettes.TryGetValue(frameIndex, out var paletteIndex))
                                paletteIndex = 0;

                            bitmap.Palette = colorPalettes[paletteIndex];
                            
                            Byte[] buff = new Byte[frame.Width];

                            AMImageLine* lineEx = &lineExPtr[Asserts.InRange(frame.ImageLineIndex, 0, (UInt32) header.ImageLineCount)];

                            for (Int32 row = 0; row < frame.Height; row++, lineEx++)
                            {
                                fixed (Byte* buffPtr = buff)
                                    Kernel32.ZeroMemory(buffPtr, buff.Length);

                                Int32 offset = 0;
                                for (Int32 i = 0; i < lineEx->ImageSegmentCount; i++)
                                {
                                    AMImageSegment line = linePtr[Asserts.InRange((Int32) lineEx->ImageSegmentIndex + i, 0, header.ImageSegmentCount)];
                                    offset += line.LeftPadding;

                                    if (line.SizeInBytes > 0)
                                    {
                                        sourceFile.SetPosition(header.ImageContentOffset + line.Offset);
                                        sourceFile.EnsureRead(buff, offset, line.SizeInBytes);
                                        offset += line.SizeInBytes;
                                    }
                                }

                                if (offset != buff.Length)
                                    throw new NotSupportedException();

                                var bitmapData = bitmap.LockBits(new Rectangle(0, row, frame.Width, 1), ImageLockMode.ReadWrite, bitmap.PixelFormat);
                                Marshal.Copy(buff, 0, bitmapData.Scan0, buff.Length);
                                bitmap.UnlockBits(bitmapData);
                            }
                            
                            ZipArchiveEntry gifEntry = zip.CreateEntry($"Image {frameIndex:D3}.gif");
                            using (Stream entrySteam = gifEntry.Open())
                                bitmap.Save(entrySteam, System.Drawing.Imaging.ImageFormat.Gif);
                        }
                    }
                }
            }
        }

        private unsafe void ExtractTiff(MemoryStream sourceFile, String outputPath, in AMHeader header)
        {
            sourceFile.SetPosition(header.AnimationOffset);
            AMAnimation[] animations = sourceFile.ReadStructs<AMAnimation>(header.AnimationCount);

            sourceFile.SetPosition(header.FrameOffset);
            AMFrame[] freames = sourceFile.ReadStructs<AMFrame>(header.FrameCount);

            TiffColorMap[] colorMaps = ReadColorMaps(sourceFile, in header);

            sourceFile.SetPosition(header.ImageHeaderOffset);
            AMImageHeader[] frames = sourceFile.ReadStructs<AMImageHeader>(header.ImageHeaderCount);

            Dictionary<Int32, Int32> frameDefaultPalettes = GetDefaultFramePalettes(freames, in header);

            sourceFile.SetPosition(header.ImageSegmentOffset);
            AMImageSegment[] segments = sourceFile.ReadStructs<AMImageSegment>(header.ImageSegmentCount);

            sourceFile.SetPosition(header.ImageLineOffset);
            AMImageLine[] lines = sourceFile.ReadStructs<AMImageLine>(header.ImageLineCount);

            ImageMeta meta = new ImageMeta();
            meta.SetAnimationType(header.AnimationType);
            meta.SetAnimation(animations);
            meta.SetFrames(freames);
            var xmlMeta = meta.ToXml();

            sourceFile.SetPosition(header.ImageContentOffset);

            fixed (AMImageSegment* linePtr = segments)
            fixed (AMImageLine* lineExPtr = lines)
            {
                using (Tiff tiff = Tiff.Open(outputPath, "w"))
                {
                    tiff.SetField(TiffTag.XMLPACKET, xmlMeta.Length, xmlMeta);

                    Byte[] colors = new Byte[256];
                    for (Int32 i = 0; i < colors.Length; i++)
                        colors[i] = checked((Byte) i);

                    for (Int32 paletteIndex = 0; paletteIndex < colorMaps.Length; paletteIndex++)
                    {
                        tiff.SetField(TiffTag.IMAGEWIDTH, 16);
                        tiff.SetField(TiffTag.IMAGELENGTH, 16);
                        tiff.SetField(TiffTag.ROWSPERSTRIP, 16);
                        tiff.SetField(TiffTag.SAMPLESPERPIXEL, 1);
                        tiff.SetField(TiffTag.BITSPERSAMPLE, 8);
                        tiff.SetField(TiffTag.ORIENTATION, Orientation.TOPLEFT);
                        tiff.SetField(TiffTag.PLANARCONFIG, PlanarConfig.CONTIG);
                        tiff.SetField(TiffTag.PHOTOMETRIC, Photometric.PALETTE);
                        tiff.SetField(TiffTag.COMPRESSION, Compression.NONE);
                        tiff.SetField(TiffTag.FILLORDER, FillOrder.MSB2LSB);
                        tiff.SetField(TiffTag.SUBFILETYPE, FileType.MASK);

                        TiffColorMap colorMap = colorMaps[paletteIndex];

                        tiff.SetField(TiffTag.COLORMAP, colorMap.Red, colorMap.Green, colorMap.Blue);
                        tiff.SetField(TiffTag.PAGENAME, $"Palette {paletteIndex:D3}");
                        for (int y = 0; y < colors.Length / 16; y++)
                            tiff.WriteScanline(colors, y * 16, y, 0);
                        tiff.WriteDirectory();
                    }

                    for (Int32 frameIndex = 0; frameIndex < frames.Length; frameIndex++)
                    {
                        AMImageHeader frame = frames[frameIndex];

                        tiff.SetField(TiffTag.IMAGEWIDTH, frame.Width);
                        tiff.SetField(TiffTag.IMAGELENGTH, frame.Height);
                        tiff.SetField(TiffTag.ROWSPERSTRIP, frame.Height);
                        tiff.SetField(TiffTag.SAMPLESPERPIXEL, 1);
                        tiff.SetField(TiffTag.BITSPERSAMPLE, 8);
                        tiff.SetField(TiffTag.ORIENTATION, Orientation.TOPLEFT);
                        tiff.SetField(TiffTag.PLANARCONFIG, PlanarConfig.CONTIG);
                        tiff.SetField(TiffTag.PHOTOMETRIC, Photometric.PALETTE);
                        tiff.SetField(TiffTag.COMPRESSION, Compression.NONE);
                        tiff.SetField(TiffTag.FILLORDER, FillOrder.MSB2LSB);
                        tiff.SetField(TiffTag.SUBFILETYPE, FileType.PAGE);
                        tiff.SetField(TiffTag.PAGENUMBER, frameIndex, frames.Length);

                        if (!frameDefaultPalettes.TryGetValue(frameIndex, out var paletteIndex))
                            paletteIndex = 0;

                        TiffColorMap colorMap = colorMaps[paletteIndex];

                        tiff.SetField(TiffTag.COLORMAP, colorMap.Red, colorMap.Green, colorMap.Blue);
                        tiff.SetField(TiffTag.PAGENAME, $"Image {frameIndex:D3}, Palette  {paletteIndex:D3}");

                        Byte[] buff = new Byte[frame.Width];

                        AMImageLine* lineEx = &lineExPtr[Asserts.InRange(frame.ImageLineIndex, 0, (UInt32) header.ImageLineCount)];

                        for (Int32 row = 0; row < frame.Height; row++, lineEx++)
                        {
                            fixed (Byte* buffPtr = buff)
                                Kernel32.ZeroMemory(buffPtr, buff.Length);

                            Int32 offset = 0;
                            for (Int32 i = 0; i < lineEx->ImageSegmentCount; i++)
                            {
                                AMImageSegment line = linePtr[Asserts.InRange((Int32) lineEx->ImageSegmentIndex + i, 0, header.ImageSegmentCount)];
                                offset += line.LeftPadding;

                                if (line.SizeInBytes > 0)
                                {
                                    sourceFile.SetPosition(header.ImageContentOffset + line.Offset);
                                    sourceFile.EnsureRead(buff, offset, line.SizeInBytes);
                                    offset += line.SizeInBytes;
                                }
                            }

                            if (offset != buff.Length)
                                throw new NotSupportedException();

                            tiff.WriteScanline(buff, row);
                        }

                        tiff.WriteDirectory();
                    }
                }
            }
        }

        private static Dictionary<Int32, Int32> GetDefaultFramePalettes(AMFrame[] seg2, in AMHeader header)
        {
            Dictionary<Int32, Int32> result = new Dictionary<Int32, Int32>(header.ImageHeaderCount);

            foreach (AMFrame entry in seg2)
            {
                if (result.ContainsKey(entry.ImageIndex))
                    continue;

                result.Add(entry.ImageIndex, entry.PalleteIndex);
            }

            return result;
        }

        private unsafe void ExtractRawImage(MemoryStream sourceFile, String outputPath, in AMHeader header)
        {
            if (header.ImageHeaderCount < 1)
                return;

            sourceFile.Seek(header.ImageHeaderOffset, SeekOrigin.Begin);
            AMImageHeader[] frames = sourceFile.ReadStructs<AMImageHeader>(header.ImageHeaderCount);

            sourceFile.Seek(header.ImageSegmentOffset, SeekOrigin.Begin);
            AMImageSegment[] lines = sourceFile.ReadStructs<AMImageSegment>(header.ImageSegmentCount);

            sourceFile.Seek(header.ImageContentOffset, SeekOrigin.Begin);

            fixed (AMImageSegment* linePtr = lines)
            {
                AMImageSegment* line = linePtr;

                for (Int32 i = 0; i < frames.Length; i++)
                {
                    ref AMImageHeader seg = ref frames[i];

                    Byte[] image = new Byte[seg.Height * seg.Width];

                    for (Int32 oy = 0; oy < seg.Height; oy++, line++)
                    {
                        sourceFile.SetPosition(header.ImageContentOffset + line->Offset);
                        sourceFile.EnsureRead(image, oy * seg.Width + line->LeftPadding, line->SizeInBytes);
                    }

                    String outputRawPath = outputPath + $"_{i:D2}_{seg.Width}x{seg.Height}.raw";
                    File.WriteAllBytes(outputRawPath, image);
                }
            }
        }

        private static unsafe ColorPalette[] ReadColorPalettes(MemoryStream sourceFile, in AMHeader header)
        {
            if (header.PaletteCount < 1)
                return new ColorPalette[0];

            ColorPalette[] result = new ColorPalette[header.PaletteCount];

            sourceFile.Seek(header.PaletteOffset, SeekOrigin.Begin);
            AMPalette[] palettes = sourceFile.ReadStructs<AMPalette>(header.PaletteCount);
            for (Int32 i = 0; i < palettes.Length; i++)
            {
                fixed (ABGRColor* argb = &palettes[i].FirstColor)
                {
                    using Bitmap bitmap = new Bitmap(16, 16, PixelFormat.Format8bppIndexed);

                    ColorPalette palette = bitmap.Palette;
                    for (Int32 k = 0; k < 256; k++)
                    {
                        var source = &(argb + k)->BGR;
                        palette.Entries[k] = Color.FromArgb(source->Red, source->Green, source->Blue);
                    }

                    result[i] = palette;
                }
            }

            return result;
        }

        private static unsafe Byte[][] ReadActPalettes(MemoryStream sourceFile, in AMHeader header)
        {
            if (header.PaletteCount < 1)
                return new Byte[0][];

            Byte[][] result = new Byte[header.PaletteCount][];

            sourceFile.Seek(header.PaletteOffset, SeekOrigin.Begin);
            AMPalette[] palettes = sourceFile.ReadStructs<AMPalette>(header.PaletteCount);
            for (Int32 i = 0; i < palettes.Length; i++)
            {
                fixed (ABGRColor* argb = &palettes[i].FirstColor)
                {
                    Byte[] actContext = new Byte[256 * 3];
                    fixed (Byte* actPtr = actContext)
                    {
                        BGRColor* bgr = (BGRColor*) actPtr;
                        for (Int32 k = 0; k < 256; k++)
                        {
                            var source = &(argb + k)->BGR;
                            var target = bgr + k;

                            // Change BGR to RGB
                            *target = new BGRColor(source->Blue, source->Green, source->Red);
                        }
                    }

                    result[i] = actContext;
                    // String outputActPath = outputPath + $"_{i:D2}.act";
                    // File.WriteAllBytes(outputActPath, actContext);
                }
            }

            return result;
        }

        private static TiffColorMap[] ReadColorMaps(MemoryStream sourceFile, in AMHeader header)
        {
            sourceFile.SetPosition(header.PaletteOffset);
            AMPalette[] palettes = sourceFile.ReadStructs<AMPalette>(header.PaletteCount);

            TiffColorMap[] result = new TiffColorMap[palettes.Length];
            for (Int32 i = 0; i < palettes.Length; i++)
                result[i] = CreateColorMap(palettes[i]);

            return result;
        }

        private static unsafe TiffColorMap CreateColorMap(AMPalette amPalette)
        {
            TiffColorMap result = new TiffColorMap(AMPalette.ColorNumber);

            fixed (ABGRColor* colors = &amPalette.FirstColor)
            {
                for (Int32 i = 0; i < AMPalette.ColorNumber; i++)
                    result[i] = (colors + i)->BGR;
            }

            return result;
        }

    }
}