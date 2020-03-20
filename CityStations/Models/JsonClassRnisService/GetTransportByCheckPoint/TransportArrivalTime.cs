using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Herald.Models.JsonClassRnisService
{
    public class TransportArrivalTime
    {
        public route route { get; set; }
        public string arrivalTime { get; set; }
        public int delaySeconds { get; set; }
    }
}
