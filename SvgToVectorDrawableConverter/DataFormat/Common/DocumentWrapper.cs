using System.Xml;

namespace SvgToVectorDrawableConverter.DataFormat.Common
{
    class DocumentWrapper<T>
        where T : Element
    {
        public XmlDocument WrappedDocument { get; }
        public T Root { get; }

        internal DocumentWrapper(XmlDocument xmlDocument)
        {
            WrappedDocument = xmlDocument;
            Root = (T)ElementFactory.Wrap(xmlDocument.DocumentElement);
        }
    }
}
