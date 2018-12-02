namespace NumberService.WebAPI.Controllers
{
    using System;
    using System.IO;
    using System.Xml;
    using System.Xml.Serialization;
    using Attributes;
    using Components;
    using Extensions;
    using Microsoft.AspNetCore.Mvc;
    using Models;

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

        private readonly XmlSerializer _sumSerializer;

        private readonly INumberProcessor _processor;

        public SoapNumberController(INumberProcessor processor)
        {
            _processor = processor ?? throw new ArgumentNullException(nameof(processor));

            _namespaceManager = new XmlNamespaceManager(new NameTable());
            _namespaceManager.AddNamespace(ServiceNamespacePrefix, ServiceNamespace);
            _namespaceManager.AddNamespace(ArrayNamespacePrefix, ArrayNamespace);

            _sumSerializer = new XmlSerializer(typeof(SumModel));
        }

        [HttpPost("abs")]
        [SoapAction("\"http://tempuri.org/INumberService/Abs\"")]
        [SoapProcessing(ServiceNamespace = ServiceNamespace, 
            OperationName = "Abs")]
        public string Abs()
        {
            var value = Request.Body.ReadSingleIntFrom(AbsValueXPath);

            return _processor.AbsoluteValue(value).ToString();
        }

        [HttpPost("sum")]
        [SoapAction("\"http://tempuri.org/INumberService/Sum\"")]
        [SoapProcessing(ServiceNamespace = ServiceNamespace, 
            OperationName = "Sum", 
            RemoveNamespacesFromRequest = false)]
        public string Sum()
        {
            //Uncomment this if you want to strip namespaces (RemoveNamespacesFromRequest = true)
            //and use XPath to get values.
            //-------------------------------------------------------------------------------
            //var values = Request.Body.ReadIntArrayFrom(SumValueXPath);
            //-------------------------------------------------------------------------------

            //Uncomment this if you want to preverse namespaces (RemoveNamespacesFromRequest = false)
            //and use an XmlSerializer to get values.
            //-----------------------------------------------------------------------------------
            var deserializedModel = (SumModel) _sumSerializer.Deserialize(Request.Body);
            var values = deserializedModel.values;
            //-----------------------------------------------------------------------------------

            return _processor.Sum(values).ToString();
        }

        [HttpPost("product")]
        [SoapAction("\"http://tempuri.org/INumberService/Product\"")]
        [SoapProcessing(ServiceNamespace = ServiceNamespace,
            OperationName = "Product",
            RemoveNamespacesFromRequest = false,
            WrapResponseInSoapEnvelope = true,
            WrapResponseInOperationResponseTag = true,
            WrapResponseInOperationResultTag = true)]
        public string Product()
        {
            var values = Request.Body.ReadIntArrayFrom(ProductValueXPath, _namespaceManager);

            return _processor.Product(values).ToString();
        }
    }
}
