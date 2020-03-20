using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CityStations.Models
{
    public class CreateNewUserViewModel
    {
        public string Email { get; set; }
        public string CityName { get; set; }
    }

    public class CreateNewCityViewModel
    {
        public string CityName { get; set; }
        public string CityKey { get; set; }
    }
}