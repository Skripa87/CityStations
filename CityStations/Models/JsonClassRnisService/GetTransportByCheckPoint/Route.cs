using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Herald.Models.JsonClassRnisService
{
    public class route
    {
        public object id { get; set; }
        public string num { get; set; }
        public double distance { get; set; }
        public int routeDurInMin { get; set; }
        public double distanceSub { get; set; }
        public int routeDurInMinSub { get; set; }
        public string name { get; set; }
        public routeType routeType { get; set; }
        public int oldID { get; set; }
        public fleet fleet { get; set; }
        public int minOffsetInSecDef { get; set; }
        public int maxOffsetInSecDef { get; set; }
        public int minFactInSecDef { get; set; }
        public int maxFactInSecDef { get; set; }
        public parkBus parkBus { get; set; }
        public int color { get; set; }
        public int active { get; set; }
        public int showInInforming { get; set; }
        public int createdBy_id { get; set; }
        public string createdDateTime { get; set; }
        public int changeBy_id { get; set; }
        public string changedDateTime { get; set; }
        public int isDeleted { get; set; }
        public administrativeUnits administrativeUnits { get; set; }
        public int? regRate { get; set; }
        public int? factRate { get; set; }
        public int? beforeDurInMin { get; set; }
        public int? afterDurInMin { get; set; }
        public int? additionalInMin { get; set; }
    }
}
