using System;
using System.IO;
using System.Text;
using SvgToVectorDrawableConverter.DataFormat.Converters.SvgToVector;
using SvgToVectorDrawableConverter.DataFormat.ScalableVectorGraphics;
using SvgToVectorDrawableConverter.Utils;
using Path = System.IO.Path;

namespace SvgToVectorDrawableConverter
{
    class Converter
    {
        private readonly Options _options = new Options();
        private readonly string[] _args;

        public Converter(string[] args)
        {
            _args = new string[args.Length];
            args.CopyTo(_args, 0);
        }

        public void Run()
        {
            try
            {
                if (!CommandLine.Parser.Default.ParseArguments(_args, _options))
                {
                    return;
                }

                foreach (var inputFile in Directory.GetFiles(_options.InputDirectory, _options.InputMask + ".svg", SearchOption.AllDirectories))
                {
                    try
                    {
                        var tempFile = Path.GetTempFileName();
                        Inkscape.SimplifySvgSync(_options.InkscapeAppPath, inputFile, tempFile);
                        var svgDocument = SvgDocumentWrapper.CreateFromFile(tempFile);
                        File.Delete(tempFile);

                        var outputDocument = SvgToVectorDocumentConverter.Convert(svgDocument).WrappedDocument;

                        var outputFile = inputFile.Substring(_options.InputDirectory.Length)
                            .TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                        outputFile = Path.Combine(_options.OutputDirectory, outputFile);
                        outputFile = Path.ChangeExtension(outputFile, "xml");

                        Directory.CreateDirectory(Path.GetDirectoryName(outputFile));
                        using (var writer = new StreamWriter(outputFile, false, new UTF8Encoding(false)))
                        {
                            outputDocument.Save(writer);
                        }
                    }
                    catch (Exception e)
                    {
                        PrintError(string.Format("{0}: {1}", inputFile, e.Message));
                    }
                }
            }
            catch (Exception e)
            {
                PrintError(e.ToString());
            }
        }

        private static void PrintError(string message)
        {
            var writer = Console.Error;
            writer.WriteLine();
            writer.WriteLine(message);
        }
    }
}
