using System.Collections.Specialized;
using System.Windows;
using System.Xml;
using SvgToVectorDrawableConverter.DataFormat.Common;

namespace SvgToVectorDrawableConverter.DataFormat.ScalableVectorGraphics
{
    class Svg : ElementWithChildren
    {
        public Svg(XmlElement wrappedElement)
            : base(wrappedElement)
        { }

        public StringDictionary Style => Parser.ParseStyle(GetAttribute<string>());

        public string Width => GetAttribute<string>();

        public string Height => GetAttribute<string>();

        public Rect ViewBox => Parser.ParseViewBox(GetAttribute<string>());
    }
}
