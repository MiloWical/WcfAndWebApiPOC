// ReSharper disable InconsistentNaming
namespace NumberService.WebAPI.Models
{
    using System.Xml.Serialization;

    [XmlRoot("Sum", Namespace = "http://tempuri.org/",
        IsNullable = false)]
    public class SumModel
    {
        [XmlArray(IsNullable = false)]
        [XmlArrayItem(Namespace = "http://schemas.microsoft.com/2003/10/Serialization/Arrays")]
        public int[] values;
    }
}
