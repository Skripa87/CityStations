using System.Collections.Generic;

namespace CityStations
{
    public static class CurrentRequestSituations
    {
        public static Dictionary<string,string> CurrentRequests { get;}

        public static void UpdateRequest(string stationId, string eventId)
        {
            CurrentRequests[stationId] = eventId;
        }
    }
}