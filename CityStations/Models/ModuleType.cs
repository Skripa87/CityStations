using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CityStations.Models
{
    public class ModuleType
    {
        public string Id { get; set; }
        public int WidthPx { get; set; }
        public int HeightPx { get; set; }
        public string CssClass { get; set; }
    }
}