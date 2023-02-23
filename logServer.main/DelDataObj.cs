using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace logServer.main
{
    internal class DelDataObj
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string? Content { get; set; }
        public bool Response { get; set; }
        public DateTime Delivered { get; set; }
    }
}
