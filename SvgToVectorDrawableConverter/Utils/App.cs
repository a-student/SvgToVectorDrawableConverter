using System;
using System.IO;

namespace SvgToVectorDrawableConverter.Utils
{
    static class App
    {
        public static string ExePath => Environment.GetCommandLineArgs()[0];

        public static string Directory => Path.GetDirectoryName(ExePath);

        public static string ExeName => Path.GetFileName(ExePath);
    }
}
