using System;
using System.Linq;
using System.Xml;
using JetBrains.Annotations;
using SvgToVectorDrawableConverter.DataFormat.Exceptions;

namespace SvgToVectorDrawableConverter.DataFormat.Common
{
    static class ElementFactory
    {
        private static string GetXmlName(Type elementType)
        {
            return elementType.Name.ToLower();
        }

        [NotNull]
        public static T Create<T>([NotNull] XmlDocument xmlDocument, [NotNull] out XmlElement xmlElement)
            where T : Element
        {
            xmlElement = xmlDocument.CreateElement(GetXmlName(typeof(T)));
            return (T)Wrap(xmlElement);
        }

        private static readonly Type[] ElementTypes = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(x => x.GetTypes())
            .Where(x => typeof(Element).IsAssignableFrom(x))
            .Where(x => !x.IsAbstract)
            .Where(x => x.GetConstructor(new[] { typeof(XmlElement) }) != null)
            .ToArray();

        [NotNull]
        public static Element Wrap([NotNull] XmlElement xmlElement)
        {
            try
            {
                var rootType = ElementTypes
                    .Single(x => GetXmlName(x) == xmlElement.OwnerDocument.DocumentElement.Name);
                var type = ElementTypes
                    .Where(x => x.Namespace == rootType.Namespace)
                    .Single(x => GetXmlName(x) == xmlElement.Name);
                return (Element)Activator.CreateInstance(type, xmlElement);
            }
            catch (InvalidOperationException e)
            {
                throw new UnsupportedFormatException($"Element '{xmlElement.Name}' is not supported.", e);
            }
        }
    }
}
