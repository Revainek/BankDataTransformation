using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace BankDataTransformationLogic.Models
{
    [Serializable,XmlInclude(typeof(object))]
    public class ObjectPair <T,U>
    {
        public T First { get; set; }
        public U Second { get; set; }
        public ObjectPair()
        {
                
        }
        public ObjectPair(T first,U second)
        {
            First = first;
            Second = second;
        }
    }
}
