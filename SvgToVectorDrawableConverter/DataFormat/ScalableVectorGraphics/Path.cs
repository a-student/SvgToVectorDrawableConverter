using System.Collections.Specialized;
using System.Xml;
using SvgToVectorDrawableConverter.DataFormat.Common;

namespace SvgToVectorDrawableConverter.DataFormat.ScalableVectorGraphics
{
    class Path : Element, IStyleableElement
    {
        public Path(XmlElement wrappedElement)
            : base(wrappedElement)
        { }

        public StringDictionary Style => Styler.GetStyle(this);

        public Transform Transform => Parser.ParseTransform(GetAttribute<string>());

        public string D => GetAttribute<string>();
    }
}
