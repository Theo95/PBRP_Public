using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PBRP
{
    public class Weather
    {
        public static List<Weather> WeatherData = new List<Weather>();

        public int Hour { get; set; }
        public int WeatherType { get; set; }
        public string Description { get; set; }
        public float TempC { get; set; }
        public float TempF { get; set; }
        public float RainLevel { get; set; }
        public float WindSpeed { get; set; }
        public float CloudOpacity { get; set; }

        public int RainPercentage { get; set; }

        public bool SnowActive { get; set; }
        public bool DeepPedTracks { get; set; }
        public bool DeepVehicleTracks { get; set; }
    }
}
