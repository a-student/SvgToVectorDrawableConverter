using System.Xml;
using JetBrains.Annotations;
using SvgToVectorDrawableConverter.DataFormat.Common;

namespace SvgToVectorDrawableConverter.DataFormat.ScalableVectorGraphics
{
    static class SvgDocumentWrapper
    {
        [NotNull]
        public static DocumentWrapper<Svg> CreateFromFile([NotNull] string filename)
        {
            var xmlDocument = new XmlDocument { XmlResolver = null };
            xmlDocument.Load(filename);
            return new DocumentWrapper<Svg>(xmlDocument);
        }
    }
}
