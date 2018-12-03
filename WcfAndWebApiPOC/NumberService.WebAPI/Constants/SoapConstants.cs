using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NumberService.WebAPI.Constants
{
    public class SoapConstants
    {
        public const string SoapNamespacePrefix = "e";
        public const string ServiceNamespacePrefix = "s";
        public const string SoapEnvelopeNamespace = "http://schemas.xmlsoap.org/soap/envelope/";

        public static readonly string SoapEnvelopeXmlTemplate =
            $"<{SoapNamespacePrefix}:Envelope xmlns:{SoapNamespacePrefix}=\"{SoapEnvelopeNamespace}\"><{SoapNamespacePrefix}:Body>{{0}}</{SoapNamespacePrefix}:Body></{SoapNamespacePrefix}:Envelope>";

        public static readonly string SoapEnvelopeBodyXPath = $"/{SoapNamespacePrefix}:Envelope/{SoapNamespacePrefix}:Body";

        public const string OperationResponseSuffix = "Response";
        public const string OperationResultSuffix = "Result";
    }
}
