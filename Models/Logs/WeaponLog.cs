using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PBRP.Logs
{
    [Table("weapon")]
    public class WeaponLog
    {
        public WeaponLog(int currentOwner, int weapId, WeaponType weapType, int newOwnerId)
        {
            CurrentOwnerId = currentOwner;
            WeaponId = weapId;
            WeaponType = weapType;
            NewOwnerId = newOwnerId;
            DateOfTransaction = Server.Date;
        }

        [Key]
        public int Id { get; set; }
        public int CurrentOwnerId { get; set; }
        public WeaponOwnerType CurrentOwnerType { get; set; }
        public int WeaponId { get; set; }
        public WeaponType WeaponType { get; set; }
        public WeaponOwnerType NewOwnerType { get; set; }
        public int NewOwnerId { get; set; }
        public DateTime DateOfTransaction { get; set; }

    }
}
