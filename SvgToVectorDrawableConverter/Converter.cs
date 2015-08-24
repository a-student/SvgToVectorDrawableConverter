using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using JetBrains.Annotations;
using SvgToVectorDrawableConverter.DataFormat.Converters.SvgToVector;
using SvgToVectorDrawableConverter.DataFormat.ScalableVectorGraphics;
using SvgToVectorDrawableConverter.DataFormat.Utils;
using SvgToVectorDrawableConverter.Utils;
using Path = System.IO.Path;

namespace SvgToVectorDrawableConverter
{
    class Converter
    {
        private readonly string[] _args;

        public Converter([NotNull] string[] args)
        {
            _args = new string[args.Length];
            args.CopyTo(_args, 0);
        }

        public void Run()
        {
            try
            {
                var options = new Options();
                if (CommandLine.Parser.Default.ParseArguments(_args, options))
                {
                    Convert(options);
                }
            }
            catch (Exception e)
            {
                PrintError(e.ToString());
            }
        }

        private static void Convert(Options options)
        {
            var converter = new SvgToVectorDocumentConverter(options.BlankVectorDrawablePath);

            foreach (var inputFile in Directory.GetFiles(options.InputDirectory, options.InputMask + ".svg", SearchOption.AllDirectories))
            {
                var subpath = PathHelper.Subpath(inputFile, options.InputDirectory);

                try
                {
                    var tempFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
                    tempFile = Path.ChangeExtension(tempFile, "svg");
                    File.Copy(inputFile, tempFile);
                    SvgUseElementInliner.InlineUses(tempFile);
                    Inkscape.SimplifySvgSync(options.InkscapeAppPath, tempFile, tempFile);
                    var svgDocument = SvgDocumentWrapper.CreateFromFile(tempFile);
                    File.Delete(tempFile);

                    var outputDocument = converter.Convert(svgDocument).WrappedDocument;
                    PrintWarnings(subpath, converter.Warnings);

                    var outputFile = Path.Combine(options.OutputDirectory, subpath);
                    outputFile = Path.ChangeExtension(outputFile, "xml");
                    outputFile = PathHelper.NormalizeFileName(outputFile);

                    Directory.CreateDirectory(Path.GetDirectoryName(outputFile));
                    var settings = new XmlWriterSettings
                    {
                        Encoding = new UTF8Encoding(false),
                        Indent = true,
                        IndentChars = new string(' ', 4),
                        NewLineOnAttributes = true
                    };
                    using (var writer = XmlWriter.Create(outputFile, settings))
                    {
                        outputDocument.Save(writer);
                    }
                }
                catch (Exception e)
                {
                    PrintError($"{subpath}: {e.Message}");
                }
            }
        }

        private static void PrintWarnings(string subpath, ICollection<string> warnings)
        {
            if (warnings.Count == 0)
            {
                return;
            }
            var writer = Console.Out;
            writer.WriteLine();
            writer.Write($"[Warning(s)] {subpath}: ");
            foreach (var warning in warnings)
            {
                writer.WriteLine(warning);
            }
        }

        private static void PrintError(string message)
        {
            var writer = Console.Error;
            writer.WriteLine();
            writer.WriteLine("[Error] " + message);
        }
    }
}
