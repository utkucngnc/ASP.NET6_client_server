using System.ComponentModel.DataAnnotations;

namespace logServer.Models
{
    public class Log
    {
        public int ID { get; set; }
        [Required]

        public string? LogName { get; set; }

        public int Content { get; set; }

        public DateTime Created { get; set; }

        public DateTime DoneAt { get; set; }

        public Status Status{ get; set; }
    }

    public enum Status { Waiting, Success, Failure }
}
