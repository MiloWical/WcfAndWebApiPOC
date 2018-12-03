namespace NumberService.WebAPI.Extensions
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.XPath;
    using Attributes;
    using Constants;

    public static class XmlProcessingExtensions
    {
        private static readonly XmlNamespaceManager SoapNamespaceManager = new XmlNamespaceManager(new NameTable());

        static XmlProcessingExtensions()
        {
            SoapNamespaceManager.AddNamespace(SoapConstants.SoapNamespacePrefix, SoapConstants.SoapEnvelopeNamespace);
        }

        //C.f.: https://stackoverflow.com/questions/30190246/remove-element-namespace-prefixes-from-xml-string-fragment
        public static void RemoveNamespaces(this XElement node)
        {
            node.Name = node.Name.LocalName;

            foreach (var child in node.Elements())
            {
                RemoveNamespaces(child);
            }

            foreach (var attr in node.Attributes())
            {
                if (attr.IsNamespaceDeclaration)
                {
                    attr.Remove();
                }
            }
        }

        public static int ReadSingleIntFrom(this Stream xmlStream, string xPath, IXmlNamespaceResolver resolver = null)
        {
            var document = XDocument.Load(xmlStream);

            var value = resolver == null
                ? document.XPathSelectElement(xPath).Value
                : document.XPathSelectElement(xPath, resolver).Value;


            return int.Parse(value);
        }

        public static int[] ReadIntArrayFrom(this Stream xmlStream, string xPath, IXmlNamespaceResolver resolver = null)
        {
            var document = XDocument.Load(xmlStream);

            var nodes = resolver == null 
                ? document.XPathSelectElements(xPath)
                : document.XPathSelectElements(xPath, resolver);

            return nodes.Select(node => int.Parse(node.Value)).ToArray();
        }

        public static MemoryStream ToMemoryStream(this XElement element)
        {
            var outputStream = new MemoryStream();
            element.Save(outputStream);

            //Reset the stream - the Save operation doesn't do it for you.
            outputStream.Seek(0, SeekOrigin.Begin);

            return outputStream;
        }

        public static MemoryStream ToMemoryStream(this string data)
        {
            var outputStream = new MemoryStream(Encoding.UTF8.GetBytes(data));

            //Reset the stream, just in case
            outputStream.Seek(0, SeekOrigin.Begin);

            return outputStream;
        }

        public static string ReadToString(this Stream stream)
        {
            //Intentionally not using a using block because I don't want to close the stream
            var reader = new StreamReader(stream);
            var output = reader.ReadToEnd();

            //Reset the stream, just in case
            stream.Seek(0, SeekOrigin.Begin);

            return output;
        }

        public static string WrapInSoapEnvelope(this string content)
        {
            return string.Format(SoapConstants.SoapEnvelopeXmlTemplate, content);
        }

        public static string WrapInOperationTag(this string response, string operation, string serviceNamespacePrefix,
            string serviceNamespace, string tagSuffix = "")
        {
            var printColon = !string.IsNullOrEmpty(serviceNamespacePrefix);

            var tagName = $"{serviceNamespacePrefix}{(printColon ? ":" : string.Empty)}{operation}{tagSuffix}";
            var tagNamespaceDeclaration = $"xmlns{(printColon ? ":" : string.Empty)}{serviceNamespacePrefix}";
            var tagContents = string.IsNullOrWhiteSpace(response) ? string.Empty : response;

            var xml =
                $"<{tagName} {tagNamespaceDeclaration}=\"{serviceNamespace}\">{tagContents}</{tagName}>";

            return xml;
        }

        private static void AddXElementData(XElement element, string data)
        {
            if (data == null)
                element.Value = string.Empty;
            else if (!data.TrimStart().StartsWith('<')) //If it's not XML, don't try to parse it
                element.Value = data;
            else
                element.Add(XElement.Parse(data));
        }
    }
}
