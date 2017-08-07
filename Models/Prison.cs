using System.ComponentModel.DataAnnotations.Schema;

namespace PBRP
{
    [Table("prison")]
    public class Prison {
        public int Id { get; set; }
        public int CharacterID { get; set; }
        public int Time { get; set; }
        public bool IsPrison { get; set; }
        public int CellNum { get; set; }
        public int JailorID { get; set; }
    }
}
