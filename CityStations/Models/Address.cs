using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CityStations.Models
{
    public class Address
    {
        public string Country { get; set; }
        public string CountryState { get; set; }
        public string StateDistrict { get; set; }
        public string City { get; set; }
        public string CityDistrict { get; set; }
        public string Street { get; set; }
        public string NumberHouse { get; set; }
        public Address() { }

        public Address(string country, string countryState, string stateDistrict, string city, string cityDistrict,
            string street, string numberHouse)
        {
            Country = country;
            CountryState = countryState;
            StateDistrict = stateDistrict;
            City = city;
            CityDistrict = cityDistrict;
            Street = street;
            NumberHouse = numberHouse;
        }

        public Address(string address)
        {
            if (string.IsNullOrEmpty(address)) return;
            var addressList = address.Split(',')
                                     .ToList();
            if (!addressList.Any()) return;
            if (addressList.ElementAt(0)
                           .Trim()
                           .ToUpperInvariant()
                           .Contains("УЛИЦА"))
            {
                Street = addressList.ElementAt(0)
                                    .Replace("Улица", "");
                NumberHouse = addressList.ElementAt(1);
                City = addressList.ElementAt(2);
                Country = "Россия";
                CountryState = addressList.ElementAt(3);
            }
            else
            {
                Street = "";
                Country = "Россия";
                NumberHouse = "";
                CountryState = "";
                City = "Уфа";
            }
            

            //foreach (var item in addressList)
            //{
            //    var buffer = item.Split(' ')
            //                     .ToList();
            //    if (buffer.Count > 1 && buffer.Select(s => s.Trim()
            //                                  .ToUpperInvariant())
            //                                  .Contains("УЛИЦА"))
            //    {
            //        buffer.RemoveAll(r => string.Equals("УЛИЦА", r.Trim().ToUpperInvariant(), new StringComparison()));
            //        string result_str = "";
            //        buffer.ForEach(a => result_str += a.Trim() + " ");
            //        Street = result_str;
            //    }
            //    else
            //    {
            //        Street = "";
            //    }
            //}

        }
    }

    
}