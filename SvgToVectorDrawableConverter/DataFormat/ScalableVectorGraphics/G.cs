using System.Collections.Specialized;
using System.Xml;
using JetBrains.Annotations;
using SvgToVectorDrawableConverter.DataFormat.Common;

namespace SvgToVectorDrawableConverter.DataFormat.ScalableVectorGraphics
{
    class G : ElementWithChildren, IStyleableElement
    {
        public G([NotNull] XmlElement wrappedElement)
            : base(wrappedElement)
        { }

        public StringDictionary Style => Styler.GetStyle(this);

        public Transform Transform => Parser.ParseTransform(GetAttribute<string>());
    }
}
