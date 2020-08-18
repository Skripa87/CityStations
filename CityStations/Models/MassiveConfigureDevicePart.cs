using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Owin.Security;

namespace CityStations.Models
{
    public class MassiveConfigureDevicePart:IComparable
    {
        public string StationName { get; set; }
        public string StationId { get; set; }
        public string IpAddress { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool IsChecked { get; set; }

        public MassiveConfigureDevicePart(StationModel station)
        {
            if (station == null) return;
            StationName = string.IsNullOrEmpty(station.NameOficial)
                        ? $"{station.Name} {station.Description}"
                        : $"{station.NameOficial} {station.Description}";
            StationId = station.Id;
            IpAddress = station.InformationTable?.IpDevice ?? "";
            UserName = station.InformationTable?.UserNameDevice ?? "";
            Password = station.InformationTable?.PasswordDevice ?? "";
            IsChecked = false;
        }

        public MassiveConfigureDevicePart()
        {

        }

        public int CompareTo(object obj)
        {
            return obj == null
                ? -1
                : string.CompareOrdinal(StationName.ToUpperInvariant(),
                                        ((MassiveConfigureDevicePart)obj).StationName.ToUpperInvariant());
        }
    }
}