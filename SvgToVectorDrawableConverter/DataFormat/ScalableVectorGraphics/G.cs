using System.Collections.Specialized;
using System.Xml;
using SvgToVectorDrawableConverter.DataFormat.Common;

namespace SvgToVectorDrawableConverter.DataFormat.ScalableVectorGraphics
{
    class G : ElementWithChildren
    {
        public G(XmlElement wrappedElement)
            : base(wrappedElement)
        { }

        public StringDictionary Style => Styler.GetStyle(this);

        public Transform Transform => Parser.ParseTransform(GetAttribute<string>());
    }
}
