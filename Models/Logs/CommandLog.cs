using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PBRP.Logs
{
    [Table("commands")]
    public class CommandLog
    {
        public CommandLog(int mID, int pID, string command)
        {
            MasterId = mID;
            PlayerId = pID;
            Command = command;
            CommandTime = Server.Date;
        }
        [Key]
        public int Id { get; set; }
        public int MasterId { get; set; } 
        public int PlayerId { get; set; }
        public string Command { get; set; }
        public DateTime CommandTime { get; set; }

    }
}
