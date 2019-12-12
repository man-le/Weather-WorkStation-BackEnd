using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeatherWorkStation.Utils
{
   
    public class PostNode
    {
        public String NodeId { get; set; }
        public double Humidity { get; set; }
        public double Temperature { get; set; }
        public double SoilMoisture { get; set; }
        public int Raining { get; set; }
        public String NodeLocation { get; set; }

    }
}