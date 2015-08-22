using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Xml;
using JetBrains.Annotations;

namespace SvgToVectorDrawableConverter.DataFormat.Common
{
    abstract class Element
    {
        [NotNull]
        internal XmlElement WrappedElement { get; }

        protected Element([NotNull] XmlElement xmlElement)
        {
            WrappedElement = xmlElement;
        }

        protected internal T GetAttribute<T>(T defaultValue = default(T), [CallerMemberName] string name = null)
        {
            name = FirstCharToLower(name);
            var attribute = WrappedElement.Attributes[name];
            if (attribute == null)
            {
                return defaultValue;
            }
            return (T)Convert.ChangeType(attribute.Value, typeof(T), CultureInfo.InvariantCulture);
        }

        protected void SetAttribute<T>(T value, [NotNull] string prefix, T defaultValue = default(T), [CallerMemberName] string name = null)
        {
            var namespaceUri = WrappedElement.GetNamespaceOfPrefix(prefix);
            if (string.IsNullOrEmpty(namespaceUri))
            {
                return;
            }

            name = FirstCharToLower(name);
            var attribute = WrappedElement.Attributes[name, namespaceUri];
            if (attribute == null)
            {
                attribute = WrappedElement.OwnerDocument.CreateAttribute(name, namespaceUri);
                WrappedElement.Attributes.Append(attribute);
            }
            if (!Equals(value, defaultValue))
            {
                attribute.Value = Convert.ToString(value, CultureInfo.InvariantCulture);
            }
            else
            {
                WrappedElement.Attributes.Remove(attribute);
            }
        }

        private static string FirstCharToLower(string s)
        {
            return char.ToLower(s[0]) + s.Substring(1);
        }

        public override string ToString()
        {
            return WrappedElement.Name;
        }
    }
}
