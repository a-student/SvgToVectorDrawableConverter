using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using PathFillTypeConverter.Utils;
using SvgToVectorDrawableConverter.DataFormat.Exceptions;

namespace SvgToVectorDrawableConverter.DataFormat.Converters.SvgToVector
{
    internal static class PathDataFixer
    {
        /// <summary>
        /// Android does not understand implicit lineto command -
        /// make it explicit and capitalize the first 'm' command.
        /// </summary>
        [CanBeNull]
        public static string Fix([CanBeNull] string pathData)
        {
            const char moveToUpper = 'M';

            pathData = pathData?.Trim();
            if (string.IsNullOrEmpty(pathData))
            {
                return pathData;
            }
            if (char.ToUpper(pathData[0]) != moveToUpper)
            {
                throw new UnsupportedFormatException("Path data must begin with a moveto command.");
            }

            var result = new StringBuilder();
            foreach (var split in PathDataSplitter.SplitByCommands(pathData))
            {
                var command = split[0];
                var parameters = PathDataSplitter.SplitParameters(split.Substring(1)).ToArray();
                Func<IEnumerable<string>, string> join = x => string.Join(" ", x);

                result.Append(command);
                if (char.ToUpper(command) == moveToUpper && parameters.Length > 2)
                {
                    var lineTo = char.IsUpper(command) ? 'L' : 'l';
                    result.Append(join(parameters.Take(2)));
                    result.Append(lineTo);
                    result.Append(join(parameters.Skip(2)));
                }
                else
                {
                    result.Append(join(parameters));
                }
            }

            result[0] = moveToUpper;
            return result.ToString();
        }
    }
}
