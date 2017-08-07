using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PBRP.Logs
{
    [Table("connections")]
    public class ConnectionLog
    {
        public ConnectionLog(int mID, int pID, string connIP)
        {
            MasterId = mID;
            PlayerId = pID;
            ConnectTime = Server.Date;
            ConnectionIP = connIP;
            DisconnectTime = Server.Date;
        }

        [Key]
        public int Id { get; set; }
        public int MasterId { get; set; }
        public int PlayerId { get; set; }
        public DateTime ConnectTime { get; set; }
        public string ConnectionIP { get; set; }
        public DateTime DisconnectTime { get; set; }
    }
}
