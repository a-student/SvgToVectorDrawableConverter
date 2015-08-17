using System;
using System.Xml;
using SvgToVectorDrawableConverter.DataFormat.Common;

namespace SvgToVectorDrawableConverter.DataFormat.VectorDrawable
{
    class Vector : ElementWithChildren
    {
        public Vector(XmlElement wrappedElement)
        : base(wrappedElement)
        { }

        public string Width
        {
            set
            {
                SetAttribute(value, "android");
                SetAttribute(value, "auto");
                SetAttribute(value, "better");
            }
        }

        public string Height
        {
            set
            {
                SetAttribute(value, "android");
                SetAttribute(value, "auto");
                SetAttribute(value, "better");
            }
        }

        public double ViewportWidth
        {
            set
            {
                SetAttribute(Convert.ToSingle(value), "android");
                SetAttribute(Convert.ToSingle(value), "auto");
                SetAttribute(Convert.ToSingle(value), "better");
            }
        }

        public double ViewportHeight
        {
            set
            {
                SetAttribute(Convert.ToSingle(value), "android");
                SetAttribute(Convert.ToSingle(value), "auto");
                SetAttribute(Convert.ToSingle(value), "better");
            }
        }
    }
}
