using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PBRP
{
    public enum EmsType
    {
        Police = 1,
        Fire = 2
    }
    public enum CallStatus
    {
        Active = 1,
        Clear = 2
    }
    public enum CallType
    {
        Fight = 1,
        Non_Emergency,
        Fraud,
        Rape,
        Assault,
        Armed_Assault,
        Burglary,
        Attempted_Suicide,
        Attempted_Murder,
        Bomb_Threat,
        Alarm,
        Carjacking,
        Kidnapping,
        Drugs,
        Destruction_Of_Property,
        Missing_Person,
        Vehicle_Accident,
        Robbery,
        Tresspassing,
        Hit_And_Run


    }
    [Table("mdc_emscalls")]
    public class EmsCall
    {
        [Key]
        public int id { get; set; }
        public int CallerId { get; set; }
        public DateTime CallTime { get; set; }
        public EmsType RefferedTo { get; set; }
        public string CallerNameGiven { get; set; }
        public string CallDescription { get; set; }
        public CallStatus CallStatus { get; set; }
        public CallType CallType { get; set; }
        public double CallLocationX { get; set; }
        public double CallLocationY { get; set; }
        public double CallLocationZ { get; set; }
        public string PhoneNumber { get; set; }
        public string CallLocationName { get; set; }
        public string CallLocationStreetName { get; set; }




    }
}