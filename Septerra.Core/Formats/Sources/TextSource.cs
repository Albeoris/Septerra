using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Septerra.Core;

namespace Septerra.Core.Sources
{
    public static class TextSource
    {
        public static IReadOnlyList<TXString> ReadStrings(StreamReader input)
        {
            List<TXString> result = new List<TXString>();

            Int32 idx = -1;
            StringBuilder sb = new StringBuilder(1024);
            while (!input.EndOfStream)
            {
                String line = input.ReadLine();
                if (line == null)
                    continue;

                Int32 separatorIndex = line.IndexOf('|', 0, Math.Min(10, line.Length));
                if (separatorIndex < 1)
                {
                    if (String.IsNullOrEmpty(line))
                        continue;

                    throw new NotSupportedException($"Unexpected entry: {line}");
                }

                String prefix = line.Substring(0, separatorIndex);
                String text = line.Substring(separatorIndex + 1);
                if (prefix == "XXXXX")
                {
                    sb.Append(text);
                }
                else if (Int32.TryParse(prefix, NumberStyles.Integer, CultureInfo.InvariantCulture, out var index))
                {
                    Flush();
                        
                    idx = index;

                    sb.Append(text);
                }
                else
                {
                    throw new NotSupportedException($"Unexpected entry: {line}");
                }
            }

            Flush();

            void Flush()
            {
                if (sb.Length <= 0)
                    return;

                result.Add(new TXString(idx, sb.ToString()));
                sb.Clear();
            }

            return result;
        }
    }
}