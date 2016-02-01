using System;
using System.Collections.Specialized;
using System.Windows;
using System.Xml;
using JetBrains.Annotations;
using SvgToVectorDrawableConverter.DataFormat.Common;
using SvgToVectorDrawableConverter.DataFormat.Converters.SvgToVector;
using SvgToVectorDrawableConverter.DataFormat.Exceptions;

namespace SvgToVectorDrawableConverter.DataFormat.ScalableVectorGraphics
{
    class Svg : ElementWithChildren
    {
        public Svg([NotNull] XmlElement wrappedElement)
            : base(wrappedElement)
        { }

        public StringDictionary Style => Styler.GetStyle(this);

        public string Width => GetAttribute<string>();

        public string Height => GetAttribute<string>();

        public Rect ViewBox
        {
            get
            {
                var value = GetAttribute<string>();
                if (!string.IsNullOrEmpty(value))
                {
                    return Parser.ParseViewBox(value);
                }
                try
                {
                    return Parser.ParseViewBox($"0 0 {UnitConverter.ConvertToPx(Width, -1)} {UnitConverter.ConvertToPx(Height, -1)}");
                }
                catch (Exception e)
                {
                    throw new UnsupportedFormatException("The viewBox attribute must be set.", e);
                }
            }
        }
    }
}
