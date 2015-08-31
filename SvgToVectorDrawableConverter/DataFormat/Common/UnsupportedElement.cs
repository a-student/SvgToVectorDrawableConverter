using System.Xml;
using JetBrains.Annotations;

namespace SvgToVectorDrawableConverter.DataFormat.Common
{
    sealed class UnsupportedElement : Element
    {
        public UnsupportedElement([NotNull] XmlElement wrappedElement)
            : base(wrappedElement)
        { }
    }
}
