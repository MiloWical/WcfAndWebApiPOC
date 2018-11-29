using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace NumberService.WCF
{
    using Components;

    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class NumberService : INumberService
    {
        INumberProcessor _processor;

        public NumberService()
            : this(new NumberProcessor())
        { }

        public NumberService(INumberProcessor processor)
        {
            if (processor == null) throw new ArgumentNullException("processor");
            _processor = processor;
        }

        public int Sum(int[] values)
        {
            return _processor.Sum(values);
        }

        public int Product(int[] values)
        {
            return _processor.Product(values);
        }

        public int Abs(string value)
        {
            return _processor.AbsoluteValue(int.Parse(value));
        }
    }
}
