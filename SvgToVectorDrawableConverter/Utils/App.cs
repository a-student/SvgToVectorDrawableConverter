using System;
using System.IO;
using System.Reflection;

namespace SvgToVectorDrawableConverter.Utils
{
    static class App
    {
        public static string ExePath => Environment.GetCommandLineArgs()[0];

        public static string Directory => Path.GetDirectoryName(ExePath);

        public static string ExeName => Path.GetFileName(ExePath);

        public static DateTime GetBuildDate()
        {
            var result = new DateTime(2000, 1, 1);
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            result = result.AddDays(version.Build);
            result = result.AddSeconds(version.Revision * 2);
            return result;
        }
    }
}
