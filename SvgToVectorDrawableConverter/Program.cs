using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using JetBrains.Annotations;
using SvgToVectorDrawableConverter.DataFormat.Converters.SvgToVector;
using SvgToVectorDrawableConverter.DataFormat.Exceptions;
using SvgToVectorDrawableConverter.DataFormat.ScalableVectorGraphics;
using SvgToVectorDrawableConverter.Utils;
using Path = System.IO.Path;

namespace SvgToVectorDrawableConverter
{
    class Program
    {
        public static void Main(string[] args)
        {
            new Program(args).RunAsync().Wait();
        }

        private readonly string[] _args;

        public Program([NotNull] string[] args)
        {
            _args = new string[args.Length];
            args.CopyTo(_args, 0);
        }

        public async Task RunAsync()
        {
            try
            {
                var options = new Options();
                if (CommandLine.Parser.Default.ParseArguments(_args, options))
                {
                    Convert(options);
                    if (!options.NoUpdateCheck)
                    {
                        using (var updateChecker = new UpdateChecker())
                        {
                            await updateChecker.CheckForUpdateAsync();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                PrintError(e.ToString());
            }
        }

        private static void Convert(Options options)
        {
            var converter = new SvgToVectorDocumentConverter(options.BlankVectorDrawablePath, options.FixFillType);

            foreach (var inputFile in Directory.GetFiles(options.InputDirectory, options.InputMask + ".svg", SearchOption.AllDirectories))
            {
                Console.Write(".");

                var subpath = PathHelper.Subpath(inputFile, options.InputDirectory);
                var tempFile = PathHelper.GenerateTempFileName("svg");

                try
                {
                    SvgPreprocessor.Preprocess(inputFile, tempFile);
                    Inkscape.SimplifySvgSync(options.InkscapeAppPath, tempFile, tempFile);

                    var svgDocument = SvgDocumentWrapper.CreateFromFile(tempFile);
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
                catch (FixFillTypeException e)
                {
                    PrintError($"{subpath}: Failure due to the --fix-fill-type option. {e.InnerException.Message}");
                }
                catch (Exception e)
                {
                    PrintError($"{subpath}: {e.Message}");
                }

                File.Delete(tempFile);
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
