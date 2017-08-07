using System.ComponentModel.DataAnnotations.Schema;

namespace PBRP
{
    public enum ArmourySignoffType {
        Armoury = 0,
    }

    [Table("pdlog")]
    public class PDLog {
        public int Id { get; set; }
        public int ItemID { get; set; }
        public int LogType { get; set; }

        public string DateSignedOut { get; set; }
        public string DateSignedIn { get; set; }

        public string TimeIn { get; set; }
        public string TimeOut { get; set; }

        public string SigneeName { get; set; }
        public string ProductName { get; set; }
        public string Serial { get; set; }
    }
}
