using System.Xml;

namespace SvgToVectorDrawableConverter.DataFormat.Common
{
    abstract class ElementWithChildren : Element
    {
        protected ElementWithChildren(XmlElement wrappedElement)
            : base(wrappedElement)
        {
            Children = new ElementCollection(wrappedElement);
        }

        public ElementCollection Children { get; private set; }
    }
}
