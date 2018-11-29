namespace NumberService.WebAPI.Controllers
{
    using System;
    using System.IO;
    using Attributes;
    using Components;
    using Microsoft.AspNetCore.Mvc;

    [Route("process")]
    public class SoapNumberController : Controller
    {
        private readonly INumberProcessor _processor;

        public SoapNumberController(INumberProcessor processor)
        {
            _processor = processor ?? throw new ArgumentNullException(nameof(processor));
        }

        [HttpPost("abs")]
        [SoapAction("\"http://tempuri.org/INumberService/Abs\"")]
        public string Abs()
        {
            //return _processor.AbsoluteValue(value);

            string data;

            using (var reader = new StreamReader(Request.Body))
            {
                data = reader.ReadToEnd();
            }

            return data;
        }

        [HttpPost("sum")]
        [SoapAction("\"http://tempuri.org/INumberService/Sum\"")]
        public string Sum()
        {
            //return _processor.Sum(values);

            string data;

            using (var reader = new StreamReader(Request.Body))
            {
                data = reader.ReadToEnd();
            }

            return data;
        }

        [HttpPost("product")]
        [SoapAction("\"http://tempuri.org/INumberService/Product\"")]
        public string Product()
        {
            //return _processor.Product(values);
            string data;

            using (var reader = new StreamReader(Request.Body))
            {
                data = reader.ReadToEnd();
            }

            return data;
        }
    }
}
