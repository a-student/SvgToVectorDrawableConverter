using System.Xml;
using SvgToVectorDrawableConverter.DataFormat.Common;

namespace SvgToVectorDrawableConverter.DataFormat.VectorDrawable
{
    static class VectorDocumentWrapper
    {
        public static DocumentWrapper<Vector> CreateFromFile(string filename)
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(filename);
            return new DocumentWrapper<Vector>(xmlDocument);
        }
    }
}
