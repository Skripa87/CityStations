using System.Collections.Generic;

namespace Herald.Models.JsonClassRnisService.GetTransportsByCheckPoint
{
    public class rootObject
    {
        public stop stop { get; set; }
        public List<TransportArrivalTime> arrivalTimes { get; set; }

    }

}
