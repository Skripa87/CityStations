using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CityStations.Models
{
    public class UserOption
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public bool GroupByState { get; set; }
        public bool OnlyActiveStations { get; set; }
        public string SelectedSortParametrs { get; set; }

        public UserOption() { }
    }
}