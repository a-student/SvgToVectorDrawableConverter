using System.Xml;
using SvgToVectorDrawableConverter.DataFormat.Common;
using SvgToVectorDrawableConverter.Utils;

namespace SvgToVectorDrawableConverter.DataFormat.VectorDrawable
{
    static class VectorDocumentWrapper
    {
        public static DocumentWrapper<Vector> CreateBlank()
        {
            var blankVectorDrawablePath = System.IO.Path.Combine(App.Directory, "BlankVectorDrawable.xml");

            var xmlDocument = new XmlDocument();
            xmlDocument.Load(blankVectorDrawablePath);
            return new DocumentWrapper<Vector>(xmlDocument);
        }
    }
}
