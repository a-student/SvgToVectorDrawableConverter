using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;

namespace SvgToVectorDrawableConverter.Utils
{
    static class Inkscape
    {
        public static string FindAppPath()
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                return (string)Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\App Paths\inkscape.exe", null, null);
            }
            return "/Applications/Inkscape.app/Contents/Resources/bin/inkscape";
        }

        /// <summary>
        /// Converts objects to paths, exports document without some namespaces and metadata.
        /// </summary>
        public static void SimplifySvgSync(string appPath, string inputPath, string outputPath)
        {
            if (!File.Exists(appPath))
            {
                throw new ApplicationException(string.Format("Inkscape app was not found in the path '{0}'. Please download it from https://inkscape.org/en/download and install it on your system.", appPath));
            }

            var arguments = $"\"{inputPath}\" -T -l \"{outputPath}\"";
            using (var process = Process.Start(new ProcessStartInfo(appPath, arguments) { WorkingDirectory = Path.GetDirectoryName(appPath) }))
            {
                process.WaitForExit();
            }
        }
    }
}
