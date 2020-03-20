using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Herald.Models.JsonClassRnisService
{
    public class parkBus
    {
        public object id { get; set; }
        public string name { get; set; }
        public string contactName { get; set; }
        public int createdBy_id { get; set; }
        public string createdDateTime { get; set; }
        public int isDeleted { get; set; }

    }
}
