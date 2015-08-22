using System.Xml;
using JetBrains.Annotations;

namespace SvgToVectorDrawableConverter.DataFormat.Common
{
    class DocumentWrapper<T>
        where T : Element
    {
        [NotNull]
        public XmlDocument WrappedDocument { get; }

        [NotNull]
        public T Root { get; }

        internal DocumentWrapper([NotNull] XmlDocument xmlDocument)
        {
            WrappedDocument = xmlDocument;
            Root = (T)ElementFactory.Wrap(xmlDocument.DocumentElement);
        }
    }
}
