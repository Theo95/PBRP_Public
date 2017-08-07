using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace PBRP
{
    public enum PhoneLogType
    {
        Call = 1,
        SMS = 2
    }
    [Table("phonelog")]
    public class PhoneLog
    {
        public int Id { get; set; }
        public long IMEITo { get; set; }
        public long IMEIFrom { get; set; }
        public string NumberFrom { get; set; }
        public string NumberTo { get; set; }
        public PhoneLogType Type { get; set; }
        public int Duration { get; set; }
        public DateTime SentAt { get; set; }
        public string Message { get; set; }
        public bool Viewed { get; set; }
        public bool DeletedFrom { get; set; }
        public bool DeletedTo { get; set; }
    }
}
