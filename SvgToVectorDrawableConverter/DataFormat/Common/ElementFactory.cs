using System;
using System.Linq;
using System.Xml;
using JetBrains.Annotations;
using SvgToVectorDrawableConverter.DataFormat.Extensions;

namespace SvgToVectorDrawableConverter.DataFormat.Common
{
    static class ElementFactory
    {
        private static string[] GetXmlNames(Type elementType)
        {
            return elementType.GetCustomAttributes(typeof(XmlNamesAttribute), false)
                .Cast<XmlNamesAttribute>()
                .SingleOrDefault()
                ?.Names
                ?? new[] { elementType.Name.FirstCharToLower() };
        }

        [NotNull]
        public static T Create<T>([NotNull] XmlDocument xmlDocument, [NotNull] out XmlElement xmlElement)
            where T : Element
        {
            xmlElement = xmlDocument.CreateElement(GetXmlNames(typeof(T))[0]);
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
                    .Single(x => GetXmlNames(x).Contains(xmlElement.OwnerDocument.DocumentElement.Name));
                var type = ElementTypes
                    .Where(x => x.Namespace == rootType.Namespace)
                    .Single(x => GetXmlNames(x).Contains(xmlElement.Name));
                return (Element)Activator.CreateInstance(type, xmlElement);
            }
            catch (InvalidOperationException)
            {
                return new UnsupportedElement(xmlElement);
            }
        }
    }
}
