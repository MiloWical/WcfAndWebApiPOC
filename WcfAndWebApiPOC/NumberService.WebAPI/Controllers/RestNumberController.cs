namespace NumberService.WebAPI.Controllers
{
    using System;
    using Components;
    using Microsoft.AspNetCore.Mvc;

    [Route("process")]
    public class RestNumberController : Controller
    {
        private readonly INumberProcessor _processor;

        public RestNumberController(INumberProcessor processor)
        {
            _processor = processor ?? throw new ArgumentNullException(nameof(processor));
        }

        [HttpGet("abs/{value}")]
        public int Abs(int value)
        {
            return _processor.AbsoluteValue(value);
        }

        [HttpPost("sum")]
        public int Sum(int[] values)
        {
            return _processor.Sum(values);
        }

        [HttpPost("product")]
        public int Product(int[] values)
        {
            return _processor.Product(values);
        }
    }
}
