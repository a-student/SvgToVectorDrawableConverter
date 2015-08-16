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
            foreach (var inputFile in Directory.GetFiles(options.InputDirectory, options.InputMask + ".svg", SearchOption.AllDirectories))
            {
                try
                {
                    var tempFile = Path.GetTempFileName();
                    Inkscape.SimplifySvgSync(options.InkscapeAppPath, inputFile, tempFile);
                    var svgDocument = SvgDocumentWrapper.CreateFromFile(tempFile);
                    File.Delete(tempFile);

                    var outputDocument = SvgToVectorDocumentConverter.Convert(svgDocument).WrappedDocument;

                    var outputFile = PathHelper.Subpath(inputFile, options.InputDirectory);
                    outputFile = Path.Combine(options.OutputDirectory, outputFile);
                    outputFile = Path.ChangeExtension(outputFile, "xml");
                    outputFile = PathHelper.NormalizeFileName(outputFile);

                    Directory.CreateDirectory(Path.GetDirectoryName(outputFile));
                    using (var writer = new StreamWriter(outputFile, false, new UTF8Encoding(false)))
                    {
                        outputDocument.Save(writer);
                    }
                }
                catch (Exception e)
                {
                    PrintError($"{PathHelper.Subpath(inputFile, options.InputDirectory)}: {e.Message}");
                }
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
