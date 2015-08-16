using System.IO;
using System.Text.RegularExpressions;

namespace SvgToVectorDrawableConverter.Utils
{
    static class PathHelper
    {
        public static string Subpath(string filePath, string directoryPath)
        {
            return filePath.Substring(directoryPath.Length)
                .TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        }

        public static string NormalizeFileName(string filePath)
        {
            var fileName = Path.GetFileNameWithoutExtension(filePath);

            fileName = Regex.Replace(fileName, @"[^\w\$]", "_");
            if (char.IsDigit(fileName[0]))
            {
                fileName = '_' + fileName;
            }

            return Path.Combine(Path.GetDirectoryName(filePath), fileName + Path.GetExtension(filePath));
        }
    }
}
