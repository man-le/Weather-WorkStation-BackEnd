using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeatherWorkStation.Utils
{
    public class NodeInfo
    {
        public String NodeId { get; set; }
        public String NodeLocation { get; set; }
        public double Temperature { get; set; }
        public double SoilMoisture { get; set; }
        public double Humidity { get; set; }
        public double Raining { get; set; }
        public long UpdateTime { get; set; }
    }
}