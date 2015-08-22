using System.Xml;
using JetBrains.Annotations;

namespace SvgToVectorDrawableConverter.DataFormat.Common
{
    abstract class ElementWithChildren : Element
    {
        protected ElementWithChildren([NotNull] XmlElement wrappedElement)
            : base(wrappedElement)
        {
            Children = new ElementCollection(wrappedElement);
        }

        [NotNull]
        public ElementCollection Children { get; }
    }
}
