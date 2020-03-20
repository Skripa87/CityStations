using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Herald.Models.JsonClassRnisService
{
    public class administrativeUnits
    {
        public object id { get; set; }
        public string name { get; set; }
        public int okato { get; set; }
        public string okatos { get; set; }
        public string okatoName { get; set; }
        public int oktmo { get; set; }
        public string oktmos { get; set; }
        public string oktmoName { get; set; }
        public string latinName { get; set; }
        public type type { get; set; }
        public int population { get; set; }
        public int active { get; set; }
        public int externalId { get; set; }
        public int createdBy_id { get; set; }
        public string createdDateTime { get; set; }
        public int changeBy_id { get; set; }
        public string changedDateTime { get; set; }
        public int isDeleted { get; set; }

    }
}
