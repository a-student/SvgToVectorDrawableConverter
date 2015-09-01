using System.Xml;
using JetBrains.Annotations;
using SvgToVectorDrawableConverter.DataFormat.Common;

namespace SvgToVectorDrawableConverter.DataFormat.VectorDrawable
{
    static class VectorDocumentWrapper
    {
        [NotNull]
        public static DocumentWrapper<Vector> CreateFromFile([NotNull] string filename)
        {
            var xmlDocument = new XmlDocument { XmlResolver = null };
            xmlDocument.Load(filename);
            return new DocumentWrapper<Vector>(xmlDocument);
        }
    }
}
