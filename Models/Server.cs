using GrandTheftMultiplayer.Shared;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace PBRP
{
    [Table("server")]
    public class Server
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public static DateTime Date { get; set; }
    }
}
