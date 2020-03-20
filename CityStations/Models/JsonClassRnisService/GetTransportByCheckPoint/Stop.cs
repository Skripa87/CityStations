using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Herald.Models.JsonClassRnisService
{
    public class stop
    {
        public string nameOfAudioFile { get; set; }
        public long id { get; set; }
        public string code { get; set; }
        public string description { get; set; }
        public int isDeleted { get; set; }

    }
}
