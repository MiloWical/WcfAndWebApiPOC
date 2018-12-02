using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NumberService.WebAPI.Attributes
{
    public class RootNodeNotFoundException : Exception
    {
        private readonly string _xml;

        public RootNodeNotFoundException(string xml)
        {
            _xml = xml ?? "<null>";
        }

        public override string Message =>
            $"Root node missing. XML: {Environment.NewLine}{_xml}{Environment.NewLine}{base.Message}";
    }
}
