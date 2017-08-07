using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PBRP.Logs
{
    [Table("attack")]
    public class AttackLog
    {
        public AttackLog(int injuredId)
        {
            InjuredId = injuredId;
            StartOfAttack = Server.Date;
        }
        [Key]
        public int Id { get; set; }
        public int InjuredId { get; set; }
        public bool AcceptedDeath { get; set; }
        public string AttackData { get; set; }
        public DateTime StartOfAttack { get; set; }
        public DateTime TimeOfDeath { get; set; }
        public DateTime RevivalTime { get; set; }
        public DateTime DownedTime { get; set; }
    }
}
