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

            fileName = fileName.ToLowerInvariant();
            fileName = Regex.Replace(fileName, "\\W", "_");
            if (char.IsDigit(fileName[0]))
            {
                fileName = '_' + fileName;
            }

            return Path.Combine(Path.GetDirectoryName(filePath), fileName + Path.GetExtension(filePath));
        }

        public static string GenerateTempFileName(string extension = null)
        {
            var result = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            if (extension != null)
            {
                result = Path.ChangeExtension(result, extension == string.Empty ? null : extension);
            }
            return result;
        }
    }
}
