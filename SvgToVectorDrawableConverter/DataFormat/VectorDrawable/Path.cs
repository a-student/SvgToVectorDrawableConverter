using System;
using System.Xml;
using SvgToVectorDrawableConverter.DataFormat.Common;

namespace SvgToVectorDrawableConverter.DataFormat.VectorDrawable
{
    class Path : Element
    {
        public Path(XmlElement wrappedElement)
            : base(wrappedElement)
        { }

        public string PathData
        {
            set { SetAttribute(value, "android"); }
        }

        public string FillColor
        {
            get { return GetAttribute<string>(); }
            set { SetAttribute(value, "android"); }
        }

        public string StrokeColor
        {
            get { return GetAttribute<string>(); }
            set { SetAttribute(value, "android"); }
        }

        public float StrokeWidth
        {
            get { return GetAttribute<float>(); }
            set { SetAttribute(value, "android"); }
        }

        public float StrokeAlpha
        {
            get { return GetAttribute(1f); }
            set { SetAttribute(value, "android", 1); }
        }

        public float FillAlpha
        {
            get { return GetAttribute(1f); }
            set { SetAttribute(value, "android", 1); }
        }

        public double TrimPathStart
        {
            set { SetAttribute(Convert.ToSingle(value), "android"); }
        }

        public double TrimPathEnd
        {
            set { SetAttribute(Convert.ToSingle(value), "android"); }
        }

        public double TrimPathOffset
        {
            set { SetAttribute(Convert.ToSingle(value), "android"); }
        }

        /// <summary>
        /// Sets the linecap for a stroked path: butt, round, square.
        /// </summary>
        public string StrokeLineCap
        {
            set { SetAttribute(value, "android"); }
        }

        /// <summary>
        /// Sets the lineJoin for a stroked path: miter, round, bevel.
        /// </summary>
        public string StrokeLineJoin
        {
            set { SetAttribute(value, "android"); }
        }

        public string StrokeMiterLimit
        {
            set { SetAttribute(value, "android"); }
        }
    }
}
