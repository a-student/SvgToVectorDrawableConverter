using System;

namespace SvgToVectorDrawableConverter.DataFormat.Common
{
    [AttributeUsage(AttributeTargets.Class)]
    class XmlNameAttribute : Attribute
    {
        public string Name { get; }

        public XmlNameAttribute(string name)
        {
            Name = name;
        }
    }
}
