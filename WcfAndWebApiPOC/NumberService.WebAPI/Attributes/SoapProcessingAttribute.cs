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
    using Constants;
    using Extensions;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    public class SoapProcessingAttribute : ActionFilterAttribute
    {
        private readonly Lazy<XmlNamespaceManager> _namespaceManager;
        private readonly Lazy<string> _payloadXPath;

        public string ServiceNamespace { get; set; }
        public string OperationName { get; set; }

        public bool RemoveSoapEnvelopeFromRequest { get; set; }
        public bool RemoveNamespacesFromRequest { get; set; }

        public bool WrapResponseInSoapEnvelope { get; set; }
        public bool WrapResponseInOperationResultTag { get; set; }
        public bool WrapResponseInOperationResponseTag { get; set; }

        public SoapProcessingAttribute()
        {
            _namespaceManager = new Lazy<XmlNamespaceManager>(() =>
            {
                var namespaceManager = new XmlNamespaceManager(new NameTable());
                namespaceManager.AddNamespace(SoapConstants.SoapNamespacePrefix, SoapConstants.SoapEnvelopeNamespace);
                namespaceManager.AddNamespace(SoapConstants.ServiceNamespacePrefix, ServiceNamespace);

                return namespaceManager;
            });

            _payloadXPath = new Lazy<string>(() => $"/{SoapConstants.SoapNamespacePrefix}:Envelope/{SoapConstants.SoapNamespacePrefix}:Body/{SoapConstants.ServiceNamespacePrefix}:{OperationName}");

            RemoveSoapEnvelopeFromRequest = true;
            RemoveNamespacesFromRequest = true;

            WrapResponseInSoapEnvelope = false;
            WrapResponseInOperationResultTag = false;
            WrapResponseInOperationResponseTag = false;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (ServiceNamespace == null) throw new ArgumentNullException(nameof(ServiceNamespace));
            if (OperationName == null) throw new ArgumentNullException(nameof(OperationName));

            var document = XDocument.Load(context.HttpContext.Request.Body);

            var operationNode = GetRequestRootXElement(document);

            context.HttpContext.Request.Body = operationNode.ToMemoryStream();
        }

        //C.f. https://stackoverflow.com/questions/16688248/modify-httpcontent-actionexecutedcontext-response-content-in-onactionexecuted
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            var body = ((ObjectResult) context.Result)
                .Value
                .ToString();

            body = ProcessResponseFormatting(body);

            var newResult = new ObjectResult(body);

            context.Result = newResult;
        }

        private string ProcessResponseFormatting(string response)
        {
            var modifiedResponse = response;

            if (WrapResponseInOperationResultTag)
                modifiedResponse = modifiedResponse.WrapInOperationTag(OperationName,
                    SoapConstants.ServiceNamespacePrefix, ServiceNamespace, SoapConstants.OperationResultSuffix);

            if (WrapResponseInOperationResponseTag)
                modifiedResponse = modifiedResponse.WrapInOperationTag(OperationName,
                    SoapConstants.ServiceNamespacePrefix, ServiceNamespace, SoapConstants.OperationResponseSuffix);

            if (WrapResponseInSoapEnvelope)
                modifiedResponse = modifiedResponse.WrapInSoapEnvelope();

            return modifiedResponse;
        }

        private XElement GetRequestRootXElement(XDocument soapRequest)
        {
            var processingRoot = RemoveSoapEnvelopeFromRequest ? soapRequest.XPathSelectElement(_payloadXPath.Value, _namespaceManager.Value) : soapRequest.Root;

            if(RemoveNamespacesFromRequest)
                processingRoot.RemoveNamespaces();

            return processingRoot;
        }
    }
}
