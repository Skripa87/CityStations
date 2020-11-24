using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using CityStations.Models.JsonGoogleMapsGeocoder;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace CityStations.Models
{
    public class HeraldGeocoder
    {
        public HeraldGeocoder(string apiKey)
        {
            ApiKey = apiKey;
            TranslitterDictionary = new Dictionary<string, string>();
            TranslitterDictionary.Add("а", "a");TranslitterDictionary.Add("б", "b");TranslitterDictionary.Add("в", "v");TranslitterDictionary.Add("г", "g");TranslitterDictionary.Add("д", "d");
            TranslitterDictionary.Add("е", "e");TranslitterDictionary.Add("ё", "yo");TranslitterDictionary.Add("ж", "zh");TranslitterDictionary.Add("з", "z");TranslitterDictionary.Add("и", "i");
            TranslitterDictionary.Add("й", "j");TranslitterDictionary.Add("к", "k");TranslitterDictionary.Add("л", "l");TranslitterDictionary.Add("м", "m");TranslitterDictionary.Add("н", "n");
            TranslitterDictionary.Add("о", "o");TranslitterDictionary.Add("п", "p");TranslitterDictionary.Add("р", "r");TranslitterDictionary.Add("с", "s");TranslitterDictionary.Add("т", "t");
            TranslitterDictionary.Add("у", "u");TranslitterDictionary.Add("ф", "f");TranslitterDictionary.Add("х", "kh");TranslitterDictionary.Add("ц", "ts");TranslitterDictionary.Add("ч", "ch");
            TranslitterDictionary.Add("ш", "sh");TranslitterDictionary.Add("щ", "sch");TranslitterDictionary.Add("ъ", "j");TranslitterDictionary.Add("ы", "i");TranslitterDictionary.Add("ь", "j");
            TranslitterDictionary.Add("э", "e");TranslitterDictionary.Add("ю", "yu");TranslitterDictionary.Add("я", "ya");TranslitterDictionary.Add("А", "A");TranslitterDictionary.Add("Б", "B");
            TranslitterDictionary.Add("В", "V");TranslitterDictionary.Add("Г", "G");TranslitterDictionary.Add("Д", "D");TranslitterDictionary.Add("Е", "E");TranslitterDictionary.Add("Ё", "Yo");
            TranslitterDictionary.Add("Ж", "Zh");TranslitterDictionary.Add("З", "Z");TranslitterDictionary.Add("И", "I");TranslitterDictionary.Add("Й", "J");TranslitterDictionary.Add("К", "K");
            TranslitterDictionary.Add("Л", "L");TranslitterDictionary.Add("М", "M");TranslitterDictionary.Add("Н", "N");TranslitterDictionary.Add("О", "O");TranslitterDictionary.Add("П", "P");
            TranslitterDictionary.Add("Р", "R");TranslitterDictionary.Add("С", "S");TranslitterDictionary.Add("Т", "T");TranslitterDictionary.Add("У", "U");TranslitterDictionary.Add("Ф", "F");
            TranslitterDictionary.Add("Х", "H");TranslitterDictionary.Add("Ц", "Ts");TranslitterDictionary.Add("Ч", "Ch");TranslitterDictionary.Add("Ш", "Sh");TranslitterDictionary.Add("Щ", "Sch");
            TranslitterDictionary.Add("Ъ", "J");TranslitterDictionary.Add("Ы", "I");TranslitterDictionary.Add("Ь", "J");TranslitterDictionary.Add("Э", "E");TranslitterDictionary.Add("Ю", "Yu");
            TranslitterDictionary.Add("Я", "Ya");
        }

        private string ApiKey { get; }
        private Dictionary<string, string> TranslitterDictionary { get; }
        private string Translitter(string val)
        {
            if (string.IsNullOrEmpty(val)) return null;
            var tripleCharVal = TranslitterDictionary.Where(kvp => kvp.Value.Length == 3);
            var doubleCharVal = TranslitterDictionary.Where(kvp => kvp.Value.Length == 2);
            var overCharVal = TranslitterDictionary.Where(kvp => kvp.Value.Length == 1);
            foreach (KeyValuePair<string,string> pair in tripleCharVal)
            {
                val = val.Replace(pair.Value, pair.Key);
            }
            foreach (KeyValuePair<string, string> pair in doubleCharVal)
            {
                val = val.Replace(pair.Value, pair.Key);
            }
            foreach (KeyValuePair<string, string> pair in overCharVal)
            {
                val = val.Replace(pair.Value, pair.Key);
            }
            return val;
        }

        private Address GetAddressFromLatLng(double lat, double lng)
        {
            string result = null;
            var http = new Uri($"https://maps.googleapis.com/maps/api/geocode/json?latlng={lat},{lng}&key={ApiKey}");
            try
            {
                var request = (HttpWebRequest) WebRequest.Create(http);
                using (var response = (HttpWebResponse) request.GetResponse())
                using (var stream = response.GetResponseStream())
                using (var reader = new StreamReader(stream ?? throw new InvalidOperationException()))
                {
                    try
                    {
                        result = reader.ReadToEnd();
                    }
                    catch (Exception e)
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            try
            {
                var jResult = JToken.Parse(result).ToObject<Example>();
                var results = jResult?.results;
                if (results == null || !results.Any()) return null;
                var maxCountAddressComponent = results.Select(s => s.address_components
                                                                    .Count)
                                                      .Max();
                var addressTotal = results.FirstOrDefault(r => r.address_components.Count == maxCountAddressComponent);
                if (addressTotal == null) return null;
                var address = Translitter(addressTotal.formatted_address);
                return new Address(address);
            }
            catch (JsonSerializationException ex)
            {
                return null;
            }
        }

        public void ExtractAddressFromGoogleResponceToLatLng(string stationId)
        {
            if (string.IsNullOrEmpty(stationId)) return;
            var manager = new ContextManager();
            var station = manager.GetStation(stationId);
            if (station == null) return; 
            manager.SetStationAddress(GetAddressFromLatLng(station.Lat,station.Lng),stationId);
        }
    }
}
