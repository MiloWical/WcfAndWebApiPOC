using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NumberService.WebAPI.Attributes
{
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.XPath;
    using Extensions;
    using Microsoft.AspNetCore.Mvc.Filters;
    public class SoapEnvelopeFilterAttribute : ActionFilterAttribute
    {
        private const string SoapNamespacePrefix = "soap-env";
        private const string ServiceNamespacePrefix = "svc";

        private const string SoapEnvelopeNamespace = "http://schemas.xmlsoap.org/soap/envelope/";
        private readonly XmlNamespaceManager _namespaceManager;
        private readonly string _payloadXPath;
        private readonly bool _stripNamespacesFromBody;

        public SoapEnvelopeFilterAttribute(string serviceNamespace, string operationName, bool stripNamespacesFromBody = true)
        {
            if(serviceNamespace == null) throw new ArgumentNullException(nameof(serviceNamespace));
            if(operationName == null) throw new ArgumentNullException(nameof(operationName));

            _namespaceManager = new XmlNamespaceManager(new NameTable());
            _namespaceManager.AddNamespace("soap-env", SoapEnvelopeNamespace);
            _namespaceManager.AddNamespace("svc", serviceNamespace);

            _payloadXPath =
                $"/{SoapNamespacePrefix}:Envelope/{SoapNamespacePrefix}:Body/{ServiceNamespacePrefix}:{operationName}";

            _stripNamespacesFromBody = stripNamespacesFromBody;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var document = XDocument.Load(context.HttpContext.Request.Body);
            var operationNode = document.XPathSelectElement(_payloadXPath, _namespaceManager);

            if(_stripNamespacesFromBody)
                operationNode.RemoveNamespaces();

            var outputStream = new MemoryStream();
            operationNode.Save(outputStream);

            //Reset the stream - the Save operation doesn't do it for you.
            outputStream.Seek(0, SeekOrigin.Begin);

            context.HttpContext.Request.Body = outputStream;
        }
    }
}
