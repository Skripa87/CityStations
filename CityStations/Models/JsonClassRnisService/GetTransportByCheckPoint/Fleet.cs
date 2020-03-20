using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Herald.Models.JsonClassRnisService
{
    public class fleet
    {
        public object id { get; set; }
        public string fullName { get; set; }
        public string name { get; set; }
        public int parkType { get; set; }
        public int possibility { get; set; }
        public int createdBy_id { get; set; }
        public string createdDateTime { get; set; }
        public int isDeleted { get; set; }

    }
}
