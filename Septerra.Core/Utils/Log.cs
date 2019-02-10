using System;
using System.IO;
using System.Text;
using System.Threading;

namespace Septerra.Core
{
    public sealed class Log
    {
        #region Lazy

        public static readonly String LogFileName = @"Septerra.log";

        private static readonly Log Instance = Initialize();

        private static Log Initialize()
        {
            try
            {
                return new Log(new FileStream(LogFileName, FileMode.Create, FileAccess.Write, FileShare.Read));
            }
            catch
            {
                Environment.Exit(1);
                return null;
            }
        }

        #endregion

        public static void Message(String message)
        {
            Instance.Write('M', 0, message);
        }

        public static void Warning(String message)
        {
            Instance.Write('W', 0, message);
        }

        public static void Error(String message)
        {
            Instance.Write('E', 0, message);
        }

        public static void Warning(Exception ex, String message)
        {
            Instance.Write('W', 0, FormatException(ex, message));
        }

        public static void Error(Exception ex, String message)
        {
            Instance.Write('E', 0, FormatException(ex, message));
        }

        private static String FormatException(Exception ex, String message)
        {
            StringBuilder sb = new StringBuilder(256);
            if (!String.IsNullOrEmpty(message))
                sb.AppendLine(message);

            sb.Append(ex);
            return sb.ToString();
        }

        #region Instance

        private readonly StreamWriter _sw;

        private Log(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            _sw = new StreamWriter(stream);
        }

        public void Write(Char type, Int32 offset, String text)
        {
            Monitor.Enter(_sw);
            try
            {
                if (String.IsNullOrEmpty(text))
                    return;

                DateTime time = DateTime.Now;

                WritePrefix(time, type, offset);

                for (Int32 i = 0; i < text.Length; i++)
                {
                    Char ch = text[i];
                    if (ch == '\n')
                    {
                        _sw.WriteLine();
                        if (i + 2 < text.Length && text[i + 2] != '\r' || i + 1 < text.Length && text[i + 1] != '\r')
                            WritePrefix(time, type, offset);
                    }
                    else if (ch != '\r')
                    {
                        _sw.Write(ch);
                    }
                }

                _sw.WriteLine();
                _sw.Flush();
            }
            catch
            {
                // Do nothing
            }
            finally
            {
                Monitor.Exit(_sw);
            }
        }

        private void WritePrefix(DateTime time, Char type, Int32 offset)
        {
            _sw.Write(time.ToString("dd.MM.yyyy hh:mm:ss "));
            _sw.Write('|');
            _sw.Write(type);
            _sw.Write("| ");
            while (offset-- > 0)
                _sw.Write('\t');
        }

        #endregion
    }
}