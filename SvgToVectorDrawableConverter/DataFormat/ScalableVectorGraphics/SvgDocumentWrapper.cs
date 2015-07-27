using System.Xml;
using SvgToVectorDrawableConverter.DataFormat.Common;

namespace SvgToVectorDrawableConverter.DataFormat.ScalableVectorGraphics
{
    static class SvgDocumentWrapper
    {
        public static DocumentWrapper<Svg> CreateFromFile(string filename)
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(filename);
            return new DocumentWrapper<Svg>(xmlDocument);
        }
    }
}
