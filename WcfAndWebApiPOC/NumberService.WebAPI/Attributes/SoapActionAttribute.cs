using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NumberService.WebAPI.Attributes
{
    using Microsoft.AspNetCore.Mvc.Abstractions;
    using Microsoft.AspNetCore.Mvc.ActionConstraints;
    using Microsoft.AspNetCore.Mvc.ApplicationModels;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.AspNetCore.Routing;

    public class SoapActionAttribute : ActionMethodSelectorAttribute
    {
        public SoapActionAttribute(string soapAction)
        {
            Action = soapAction ?? throw new ArgumentNullException(nameof(soapAction));
        }

        public string Action { get; }

        public bool ActionAppliesTo(string action)
        {
            return Action.Equals(action, StringComparison.OrdinalIgnoreCase);
        }

        public override bool IsValidForRequest(RouteContext routeContext, ActionDescriptor action)
        {
            string soapAction = routeContext.HttpContext.Request.Headers["SOAPAction"];

            return ActionAppliesTo(soapAction);
        }
    }
}
