using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Text;
using System.Threading;
using Septerra.Core;

namespace Septerra
{
    class Program
    {
        static Int32 Main(String[] rawArgs)
        {
            try
            {
                ProgramArguments arguments = new ProgramArguments(rawArgs);
                
                if (!arguments.TryGetNext(out var command))
                    return RunGame(arguments);

                switch (command)
                {
                    case "run":
                        return RunGame(arguments);
                    case "unpack":
                        return Unpack(arguments);
                    case "convert":
                        return Convert(arguments);
                    default:
                        return ShowHelp();
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("----------------------------");
                Console.Error.WriteLine("Failed to extract game data.");
                Console.Error.WriteLine("----------------------------");
                Console.Error.WriteLine(ex);
                Console.Error.WriteLine("----------------------------");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
                return 1;
            }
        }

        private static Int32 RunGame(ProgramArguments arguments)
        {
            try
            {
                var spec = new RunGameSpecPreprocessor();

                if (arguments.TryGetNext(out String gameDirectoryPath))
                {
                    spec.GameDirectoryPath = gameDirectoryPath;

                    StringBuilder sb = new StringBuilder();
                    while(arguments.TryGetNext(out var arg))
                    {
                        sb.Append(arg);
                        sb.Append(' ');
                    }

                    if (sb.Length != 0)
                        sb.Length--;

                    spec.GameArguments = sb.ToString();
                }
                else
                {
                    spec.GameDirectory = GameDirectoryProvider.GetDefault();
                }

                spec.Preprocess();
                
                RunGameCoroutine coroutine = new RunGameCoroutine(spec);
                coroutine.Execute();

                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("----------------------------");
                Console.Error.WriteLine("Failed to run game.");
                Console.Error.WriteLine("----------------------------");
                Console.Error.WriteLine(ex);
                Console.Error.WriteLine("----------------------------");
                ShowRunHelp();
                Console.WriteLine();
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
                return 255;
            }
        }

        private static Int32 Unpack(ProgramArguments arguments)
        {
            try
            {
                var spec = new UnpackGamePackagesSpecPreprocessor();
                if (arguments.TryGetNext(out String gameDirectoryPath))
                {
                    spec.GameDirectoryPath = gameDirectoryPath;
                }
                else
                {
                    spec.GameDirectory = GameDirectoryProvider.GetDefault();
                }

                if (arguments.TryGetNext(out String outputPath))
                {
                    spec.OutputDirectory = outputPath;
                }
                else
                {
                    spec.OutputDirectory = $".\\Data";
                }

                while (arguments.TryGetNext(out String arg))
                {
                    switch (arg)
                    {
                        case "-convert":
                            spec.Convert = true;
                            break;
                        case "-rename":
                            spec.Rename = true;
                            break;
                        default:
                            throw new ArgumentException(arg, nameof(arguments));
                    }
                }

                spec.Preprocess();
                
                UnpackGamePackagesCoroutine coroutine = new UnpackGamePackagesCoroutine(spec);
                coroutine.Execute();

                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("----------------------------");
                Console.Error.WriteLine("Failed to unpack game data.");
                Console.Error.WriteLine("----------------------------");
                Console.Error.WriteLine(ex);
                Console.Error.WriteLine("----------------------------");
                ShowUnpackHelp();
                Console.WriteLine();
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
                return 255;
            }
        }

        private static Int32 ShowHelp()
        {
            Console.WriteLine("Starting without parameters will lead to the launch of the game from the current folder or the path form the windows registry.");
            Console.WriteLine();

            ShowRunHelp();
            ShowUnpackHelp();
            ShowConvertHelp();
            Console.WriteLine();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
            return 1;
        }

        private static Int32 Convert(ProgramArguments args)
        {
            try
            {
                if (!args.TryGetNext(out var mode))
                    throw new ArgumentException(nameof(args));

                String[] split = mode.Split('2');
                if (split.Length != 2)
                    throw new ArgumentException(mode, nameof(args));

                ConvertSpecPreprocessor spec = new ConvertSpecPreprocessor(split[0], split[1]);

                if (!args.TryGetNext(out var path))
                    throw new ArgumentException(nameof(args));

                if (Directory.Exists(path))
                {
                    spec.SourceDirectory = path;
                    if (args.TryGetNext(out var mask))
                        spec.Mask = mask;
                }
                else if (File.Exists(path))
                {
                    spec.SourceFile = path;
                    if (args.TryGetNext(out var outputFile))
                        spec.OutputFile = outputFile;
                }
                else
                {
                    throw new FileNotFoundException(path);
                }

                spec.Preprocess();

                ICoroutine coroutine;
                switch (mode)
                {
                    case "tx2txt":
                        coroutine = new ConvertTxToTxtCoroutine(spec);
                        break;
                    case "am2tiff":
                        coroutine = new ConvertAmToTiffCoroutine(spec);
                        break;
                    case "tiff2am":
                        coroutine = new ConvertTiffToAmCoroutine(spec);
                        break;
                    case "vssf2mp3":
                        coroutine = new ConvertVssfToMp3Coroutine(spec);
                        break;
                    case "mp32vssf":
                        coroutine = new ConvertMp3ToVssfCoroutine(spec);
                        break;
                    default:
                        throw new ArgumentException(mode, nameof(args));
                }

                coroutine.Execute();
                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("----------------------------");
                Console.Error.WriteLine("Failed to convert game data.");
                Console.Error.WriteLine("----------------------------");
                Console.Error.WriteLine(ex);
                Console.Error.WriteLine("----------------------------");
                ShowConvertHelp();
                Console.WriteLine();
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
                return 255;
            }
        }

        private static void ShowRunHelp()
        {
            Console.WriteLine("run [gameFolder] [gameArgs]");
            Console.WriteLine("  Example: run . -M");
            Console.WriteLine("  will start the game from the current directory with -M argument to disable movies.");
            Console.WriteLine();
        }

        private static void ShowUnpackHelp()
        {
            Console.WriteLine("unpack [gameFolder] [outputFolder] [-convert] [-rename]");
            Console.WriteLine(" Example: unpack . .\\Data -convert");
            Console.WriteLine("  will unpack resources from the current folder to the \\Data folder, convert all known resources to the user-friendly formats and rename known files.");
            Console.WriteLine();
        }

        private static void ShowConvertHelp()
        {
            Console.WriteLine("convert type sourceFolder [sourceMask]");
            Console.WriteLine("convert type sourceFile [targetFile]");
            Console.WriteLine("  Types: tx2txt, am2tiff, tiff2am");
            Console.WriteLine();
            Console.WriteLine("  Example: convert am2tiff .\\Data\\anim *.am");
            Console.WriteLine("  will convert all *.am files to TIFFs in the \\Data\\anim folder.");
            Console.WriteLine();
            Console.WriteLine("  Example: convert tiff2am 08219990.tiff");
            Console.WriteLine("  will convert 08219990.tiff to 08219990.am");
            Console.WriteLine();
            Console.WriteLine("  Example: convert tx2txt 0500000B.tx \"Azziz's Temple.txt\"");
            Console.WriteLine("  will convert credits.tiff to Azziz's Temple.txt");
            Console.WriteLine();
        }
    }
}