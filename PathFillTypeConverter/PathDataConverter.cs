using JetBrains.Annotations;
using PathFillTypeConverter.Algorithms;

namespace PathFillTypeConverter
{
    public static class PathDataConverter
    {
        [CanBeNull]
        public static string ConvertFillTypeFromEvenOddToWinding([CanBeNull] string pathData, out bool separatePathForStroke)
        {
            if (!string.IsNullOrEmpty(pathData))
            {
                var path = new PathParser().Parse(pathData);
                path = PathPreprocessor.Preprocess(path);
                var convert0 = PathFormatter.ToString(path);
                path = PathConverter.EliminateIntersections(path);
                var convert1 = PathFormatter.ToString(path);
                path = PathConverter.FixDirections(path);
                var convert2 = PathFormatter.ToString(path);
                if (convert2 != convert0)
                {
                    separatePathForStroke = convert1 != convert0;
                    return convert2;
                }
            }
            separatePathForStroke = false;
            return pathData;
        }
    }
}
