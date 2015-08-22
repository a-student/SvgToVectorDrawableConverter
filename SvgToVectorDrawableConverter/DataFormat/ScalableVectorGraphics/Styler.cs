using System.Collections.Specialized;
using System.Xml;
using JetBrains.Annotations;
using SvgToVectorDrawableConverter.DataFormat.Common;

namespace SvgToVectorDrawableConverter.DataFormat.ScalableVectorGraphics
{
    internal static class Styler
    {
        [NotNull]
        public static StringDictionary GetStyle([NotNull] Element element)
        {
            const string styleAttributeName = "style";

            var result = Parser.ParseStyle(element.GetAttribute<string>(null, styleAttributeName));
            foreach (XmlAttribute attribute in element.WrappedElement.Attributes)
            {
                if (attribute.Name != styleAttributeName)
                {
                    result[attribute.Name] = attribute.Value;
                }
            }
            return result;
        }
    }
}
