using System.Collections.Specialized;
using System.Windows;
using System.Xml;
using SvgToVectorDrawableConverter.DataFormat.Common;

namespace SvgToVectorDrawableConverter.DataFormat.ScalableVectorGraphics
{
    class Svg : ElementWithChildren, IStyleableElement
    {
        public Svg(XmlElement wrappedElement)
            : base(wrappedElement)
        { }

        public StringDictionary Style => Styler.GetStyle(this);

        public string Width => GetAttribute<string>();

        public string Height => GetAttribute<string>();

        public Rect ViewBox => Parser.ParseViewBox(GetAttribute<string>());
    }
}
