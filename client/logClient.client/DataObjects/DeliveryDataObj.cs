using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace logClient.client.DataObjects
{
    internal class DeliveryDataObj
    {
        public int ID { get; set; }
        public string? Content { get; set; }
        public DateTime Delivered { get; set; }
    }
}
