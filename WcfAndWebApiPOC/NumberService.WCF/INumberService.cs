using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace NumberService.WCF
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface INumberService
    {
        [OperationContract]
        [WebInvoke(UriTemplate = "process/sum", ResponseFormat = WebMessageFormat.Json)]
        int Sum(int[] values);

        [OperationContract]
        [WebInvoke(UriTemplate = "process/average", ResponseFormat = WebMessageFormat.Json)]
        int Product(int[] values);

        [OperationContract]
        [WebGet(UriTemplate = "process/abs/{value}", ResponseFormat = WebMessageFormat.Json)]
        int Abs(string value);
    }


    
}
