using System;
using System.Collections.Generic;
using System.IO;

namespace Septerra
{
    public sealed class ConvertSpecPreprocessor : ConvertSpec
    {
        public String SourceFile { get; set; }
        public String OutputFile { get; set; }

        public String SourceDirectory { get; set; }
        public String Mask { get; set; }

        private readonly String _sourceExtension;
        private readonly String _targetExtension;

        public ConvertSpecPreprocessor(String sourceExtension, String targetExtension)
        {
            _sourceExtension = sourceExtension;
            _targetExtension = targetExtension;
        }

        public void Preprocess()
        {
            if (SourceFile != null)
            {
                FileMode();
            }
            else if (SourceDirectory != null)
            {
                DirectoryMode();
            }
            else
            {
                throw new ArgumentException("Source file or directory is not specified.");
            }
        }

        private void DirectoryMode()
        {
            if (!Directory.Exists(SourceDirectory))
                throw new DirectoryNotFoundException(SourceDirectory);

            if (String.IsNullOrEmpty(Mask))
                Mask = "*." + _sourceExtension;
            else if (Mask.EndsWith(_targetExtension, StringComparison.CurrentCultureIgnoreCase))
                throw new ArgumentException($"You are trying to convert .{_targetExtension} file instead of .{_sourceExtension}", nameof(SourceFile));

            IEnumerable<(String source, String target)> Enumerate()
            {
                foreach (String file in Directory.EnumerateFiles(SourceDirectory, Mask, SearchOption.TopDirectoryOnly))
                    yield return (file, Path.ChangeExtension(file, '.' + _targetExtension));
            }

            EnumerateFiles = Enumerate;
        }

        private void FileMode()
        {
            if (!File.Exists(SourceFile))
                throw new FileNotFoundException(SourceFile);

            String sourceExtension = Path.GetExtension(SourceFile);
            if (!String.IsNullOrEmpty(sourceExtension))
            {
                if (String.Equals(sourceExtension, '.' + _targetExtension, StringComparison.CurrentCultureIgnoreCase))
                    throw new ArgumentException($"You are trying to convert .{_targetExtension} file instead of .{_sourceExtension}", nameof(SourceFile));
            }

            if (String.IsNullOrEmpty(OutputFile))
            {
                OutputFile = Path.ChangeExtension(SourceFile, '.' + _targetExtension);
            }
            else
            {
                String outputExtension = Path.GetExtension(SourceFile);
                if (!String.IsNullOrEmpty(outputExtension))
                {
                    if (String.Equals(outputExtension, '.' + _sourceExtension, StringComparison.CurrentCultureIgnoreCase))
                        throw new ArgumentException($"You are trying to convert a file to .{_sourceExtension} instead of .{_targetExtension}", nameof(SourceFile));
                }
            }

            IEnumerable<(String source, String target)> Enumerate()
            {
                yield return (SourceFile, OutputFile);
            }

            EnumerateFiles = Enumerate;
        }
    }
}