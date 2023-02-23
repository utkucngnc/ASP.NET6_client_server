using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace logServer.main
{
    internal class LogDataObj
    {
        public int ID { get; set; }
        [Required]
        public string? LogName { get; set; }
        public DateTime Created { get; set; }
        public DateTime DoneAt { get; set; }
        public Status Status { get; set; }
    }
    public enum Status { Waiting, Success, Failure }
}
