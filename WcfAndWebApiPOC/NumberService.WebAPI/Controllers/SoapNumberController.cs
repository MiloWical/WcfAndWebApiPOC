namespace NumberService.WebAPI.Controllers
{
    using System;
    using System.IO;
    using System.Xml;
    using Attributes;
    using Components;
    using Extensions;
    using Microsoft.AspNetCore.Mvc;

    [Route("process")]
    public class SoapNumberController : Controller
    {
        private const string ServiceNamespacePrefix = "svc";
        private const string ServiceNamespace = "http://tempuri.org/";

        private const string ArrayNamespacePrefix = "arr";
        private const string ArrayNamespace = "http://schemas.microsoft.com/2003/10/Serialization/Arrays";

        private readonly XmlNamespaceManager _namespaceManager;

        private const string AbsValueXPath = "/Abs/value";
        private const string SumValueXPath = "/Sum/values/int";
        private const string ProductValueXPath = "/svc:Product/svc:values/arr:int";


        private readonly INumberProcessor _processor;

        public SoapNumberController(INumberProcessor processor)
        {
            _processor = processor ?? throw new ArgumentNullException(nameof(processor));

            _namespaceManager = new XmlNamespaceManager(new NameTable());
            _namespaceManager.AddNamespace(ServiceNamespacePrefix, ServiceNamespace);
            _namespaceManager.AddNamespace(ArrayNamespacePrefix, ArrayNamespace);
        }

        [HttpPost("abs")]
        [SoapAction("\"http://tempuri.org/INumberService/Abs\"")]
        [SoapEnvelopeFilter(ServiceNamespace, "Abs")]
        public string Abs()
        {
            var value = Request.Body.ReadSingleIntFrom(AbsValueXPath);

            return _processor.AbsoluteValue(value).ToString();
        }

        [HttpPost("sum")]
        [SoapAction("\"http://tempuri.org/INumberService/Sum\"")]
        [SoapEnvelopeFilter(ServiceNamespace, "Sum")]
        public string Sum()
        {
            var values = Request.Body.ReadIntArrayFrom(SumValueXPath);

            return _processor.Sum(values).ToString();
        }

        [HttpPost("product")]
        [SoapAction("\"http://tempuri.org/INumberService/Product\"")]
        [SoapEnvelopeFilter(ServiceNamespace, "Product", false)]
        public string Product()
        {
            var values = Request.Body.ReadIntArrayFrom(ProductValueXPath, _namespaceManager);

            return _processor.Product(values).ToString();
        }
    }
}
