using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using CityStations.Models.StationForecast2020;
using Herald.Models.JsonClassRnisService.GetTransportsByCheckPoint;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CityStations.Models
{
    public class PredictManager : AbstractPredictManager
    {

        public new IEnumerable<IForecast> GetStationForecast(string idStation)
        {
            var manager = new ContextManager();
            var station = manager.GetStation(idStation);
            if (station == null) return new List<StationForecast>();
            var stationForecasts = GetStationForecastGortrans(station.Id).ToList();
            if (station.IdForRnis != null)
                stationForecasts.AddRange(GetStationForecastForR(station.IdForRnis));
            stationForecasts.Sort();
            return stationForecasts;
        }

        public IEnumerable<IForecast> GetStationForecastGortrans(string idStation)
        {
            string result = null;
            var contextManager = new ContextManager();
            var station = contextManager.GetStation(idStation);
            if (station == null) return null;
            try
            {
                var http = station.InformationTable?.ServiceType == null || station.InformationTable.ServiceType == ServiceType.OLD
                         ? new Uri($"http://glonass.ufagortrans.ru/php/getStationForecasts.php?sid={idStation}&type=0&city=ufagortrans&info=12345&_=1517558480816")
                         : new Uri($"http://176.118.208.48:5819/getForecasts.php?id={idStation}");
                var request = (HttpWebRequest)WebRequest.Create(http);
                using (var response = (HttpWebResponse)request.GetResponse())
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
                if (station.InformationTable?.ServiceType == null ||
                    station.InformationTable?.ServiceType == ServiceType.OLD)
                {
                    var jResult = JToken.Parse(result).ToObject<IEnumerable<StationForecast>>()
                        .ToList();
                    jResult.RemoveAll(j => j.Arrt < 20 || j.Arrt > 1201);
                    return jResult;
                }
                else
                {
                    var jResult = JToken.Parse(result).ToObject<Root>();
                    if(jResult == null) return new List<IForecast>();
                    jResult.forecasts.RemoveAll(j => j.arrTime < 20 || j.arrTime > 1201);
                    return jResult.forecasts;
                }
            }
            catch (Exception ex)
            {
                return new List<StationForecast>();
            }
        }

        public new string GetLatLngStationFor(string idStation)
        {
            try
            {
                var manager = new ContextManager();
                var station = manager.GetStation(idStation);
                var lat = station?.Lat
                              .ToString(CultureInfo.InvariantCulture) ?? "";
                var lng = station?.Lng
                              .ToString(CultureInfo.InvariantCulture) ?? "";
                if (lat.IndexOf('.') < 0)
                    lat = lat.Substring(0, 2) + "." + lat.Substring(2, lat.Length - 3);
                if (lng.IndexOf('.') < 0)
                    lng = lng.Substring(0, 2) + "." + lng.Substring(2, lng.Length - 3);
                return lat + ";" + lng;
            }
            catch (Exception e)
            {
                Logger.WriteLog($"ошибка: {e.Message}, дополнительная информация {e.InnerException}, стек вызова {e.StackTrace}", idStation);
                return "56.096734;54.22703";
            }
        }
        private HttpContent CreateHttpContent(string content)
        {
            if (content == null) return null;
            var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            memoryStream.Seek(0, SeekOrigin.Begin);
            var httpContent = new StreamContent(memoryStream);
            return httpContent;
        }

        private double CreateArrivalTimeRnis(string timeOfComing)
        {
            var dtArrival = DateTime.TryParse(timeOfComing, out var dt)
                          ? dt
                          :DateTime.MaxValue;
            return DateTime.Equals(dtArrival,DateTime.MaxValue)
                   ? -1
                   : (dtArrival-DateTime.Now).TotalSeconds;
        }

        public IEnumerable<StationForecast> GetStationForecastForR(string stationId)
        {
            var stationForecasts = new List<StationForecast>();
            try
            {
                var json =
                    @"[{""clientIPAddress"":""0.0.0.0"",""initiator"":""serviceVMS"",""password"":""3VJDgDbe8ma0qtMkk3jd2w\u003d\u003d"",""userName"":""serviceVMS""},""" +
                    stationId + @"""]";
                using (var content = CreateHttpContent(json))
                {
                    using (var client = new HttpClient())
                    {
                        var uri = new Uri(
                            "http://185.235.72.98:47201/vms-ws/rest/InformationBoardWS/getTransportsByCheckPointId");
                        if (content == null) return new List<StationForecast>();
                        var response = client.PostAsync(uri, content)
                            ?.Result;
                        if (response == null) return new List<StationForecast>();
                        var contentBody = response?.Content
                            ?.ReadAsStringAsync()
                            ?.Result;
                        try
                        {
                            var jResult = JToken.Parse(contentBody).ToObject<rootObject>();
                            if (jResult == null) return new List<StationForecast>();
                            var forecasts = jResult.arrivalTimes;
                            if (forecasts == null || forecasts?.Count == 0) return new List<StationForecast>();
                            var index = 0;
                            foreach (var item in forecasts)
                            {
                                index++;
                                var element = new StationForecast()
                                {
                                    Arrt = (int)(CreateArrivalTimeRnis(item.arrivalTime)),
                                    Id = index,
                                    Rnum = item?.route?.num ?? "",
                                    Rtype = "M",
                                    Where = item?.route?.name.Replace(item?.route?.num,"")
                                };
                                stationForecasts.Add(element);
                            }
                            return stationForecasts;
                        }
                        catch (JsonException ex)
                        {
                            Logger.WriteLog("Ошибка десериализации json ответа от РНИС", "GetStationForecastRnis");
                            return new List<StationForecast>();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLog($"{ex.Message} {ex.StackTrace}", "PredictManager");
            }
            return new List<StationForecast>();
        }

    }
}