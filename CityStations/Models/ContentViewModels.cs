using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace CityStations.Models
{
    public class TextContent : AbstractContent
    {
        private string Text { get; set; }

        public TextContent(string text, int timeOut, int index)
        {
            Text = text;
            IndexInContent = index;
            ContentType = ContentType.TEXT;
            TimeOut = timeOut;
        }

        public new object PresentContent()
        {
            return Text;
        }
    }

    public class VideoContent : AbstractContent
    {
        private string HttpLinkVideoContent { get; set; }

        public VideoContent(string httpLinkVideoContent, int timeOut, int index)
        {
            HttpLinkVideoContent = httpLinkVideoContent;
            IndexInContent = index;
            TimeOut = timeOut;
            ContentType = ContentType.VIDEO;
        }

        public new object PresentContent()
        {
            return HttpLinkVideoContent;
        }
    }

    public class PictureContent : AbstractContent
    {
        //private string HttpLinkPictureContent { get; }

        public PictureContent(/*string httpLinkPictureContent, */int timeOut, int index)
        {
          //  HttpLinkPictureContent = httpLinkPictureContent;
            IndexInContent = index;
            TimeOut = timeOut;
            ContentType = ContentType.PICTURE;
        }
    }

    public class TickerContent : AbstractContent
    {
        private string TickerText { get; set; }

        public TickerContent(string tickerText, int timeOut, int index)
        {
            TimeOut = timeOut;
            IndexInContent = index;
            TickerText = tickerText;
            ContentType = ContentType.TICKER;
        }

        public new object PresentContent()
        {
            return TickerText;
        }
    }

    public class TickerVerticalContent : AbstractContent
    {
        private string TickerVerticalText { get; set; }

        public TickerVerticalContent(string text, int timeOut, int index)
        {
            TimeOut = timeOut;
            IndexInContent = index;
            TickerVerticalText = text;
            ContentType = ContentType.TICKER;
        }

        public new object PresentContent()
        {
            var result = new List<string>();
            var content = TickerVerticalText.Split('\n')
                                            .ToList();
            foreach (var item in content)
            {
                result.Add(item);
            }
            return result;
        }
    }

    public class DateTimeViewModel
    {
        public string CurrentDate { get; set; }
        public string CurrentTime { get; set; }

        public DateTimeViewModel(string currentDate, string currentTime)
        {
            CurrentDate = currentDate;
            CurrentTime = currentTime;
        }
    }

    public class DateTimeContent : AbstractContent
    {
        public DateTimeContent(int timeOut, int index)
        {
            TimeOut = timeOut;
            IndexInContent = index;
            ContentType = ContentType.DATE_TIME;
        }

        public new object PresentContent()
        {
            var time = DateTime.Now;
            return new DateTimeViewModel(time.ToString("dd:MM:yyyy"),time.ToString("HH:mm"));
        }
    }

    public class WeatherViewModel
    {
        public string CurrentDate { get; set; }
        public string CurrentTime { get; set; }
        public string DirectionAndSpeedWind { get; set; }
        public string SpeedWind { get; set; } 
        public string Temperature { get; set; }
        public string Precipation { get; set; }
        public string Precipation2 { get; set; }
        public string PrecipationIcon { get; set; }

        public WeatherViewModel(WeatherCity weatherCity, AbstractPredictManager manager)
        {
            //var bufferDate = manager.UnixTimeStampToDateTime(weatherCity.dt);
            if (manager == null || (weatherCity?.weather == null || (!weatherCity.weather?.Any() ?? false))) return;
            CurrentDate = DateTime.Now
                                  .ToString("dd.MM.yyyy",CultureInfo.InvariantCulture);//bufferDate.ToShortDateString();
            CurrentTime = DateTime.Now
                                  .ToString("HH:mm", CultureInfo.InvariantCulture);
            Temperature = manager.KelvinToCelcia(kelvinTemperature: weatherCity.main?.temp ?? 0) + "`C";
            DirectionAndSpeedWind = manager.CreateDirectionAndSpeedWind(weatherCity.wind);
            SpeedWind = ((int)(weatherCity.wind?.speed ?? 0)) + "м/с";
            PrecipationIcon = DateTime.Now
                                      .TimeOfDay
                                      .ToString();
            var buffPrecipitation = weatherCity.weather
                                                    .FirstOrDefault()
                                                    ?.description ?? "";
            switch (buffPrecipitation)
            {
                case "overcast clouds":
                    Precipation = "Переменная облачность";
                    Precipation2 = "Облачно";
                    break;
                case "clear sky":
                    Precipation = "Солнечно";
                    break;
                case "few clouds":
                    Precipation = "Переменная облачность";
                    Precipation2 = "Облачно";
                    break;
                case "scattered clouds":
                    Precipation = "Небольшая облачность";
                    Precipation2 = "Облачно";
                    break;
                case "broken clouds":
                    Precipation = "Облачно";
                    Precipation2 = "Облачно";
                    break;
                case "shower rain":
                    Precipation = "Сильный дождь";
                    Precipation2 = "Дождь";
                    break;
                case "rain":
                    Precipation = "Дождь";
                    Precipation2 = "Дождь";
                    break;
                case "thunderstorm":
                    Precipation = "Гроза";
                    Precipation2 = "Гроза";
                    break;
                case "snow":
                    Precipation = "Снег";
                    Precipation2 = "Снег";
                    break;
                case "light snow":
                    Precipation = "Небольшой снег";
                    Precipation2 = "Снег";
                    break;
                case "mist":
                    Precipation = "Туман";
                    Precipation2 = "Туман";
                    break;
                default:
                    Precipation = "Переменная";
                    Precipation2 = "Переменная";
                    break;
            }
            PrecipationIcon = "http://openweathermap.org/img/w/" + (weatherCity.weather.Any() 
                                                                   ? $"{weatherCity.weather.FirstOrDefault()?.icon ?? ""}.png"
                                                                   :"02d.png");
        }
    }

    public class ForecastContent : AbstractContent
    {
        private string StationId { get; set; }
        private int RowCount { get; set; }

        public ForecastContent(int timeOut, string stationId, int rowCount, int index)
        {
            TimeOut = timeOut;
            IndexInContent = index;
            ContentType = ContentType.FORECAST;
            StationId = stationId;
            RowCount = rowCount;
        }

        private object GetPredict(string stationId, int rowCount)
        {
            var stationForecasts = (new PredictManager()).GetStationForecast(stationId) ?? new List<StationForecast>();
            var result = new List<Predict>();
            foreach (var item in stationForecasts)
            {
                var predict = new Predict(item);
                result.Add(predict);
            }
            return result.Count == 0 
                   ? (object)new WeatherDateTimeContent(TimeOut,StationId,IndexInContent)
                   :(result.Count >= rowCount
                    ? result.GetRange(0, rowCount)
                    : result.GetRange(0, result.Count));
        }

        public new object PresentContent()
        {
            return GetPredict(StationId, RowCount);
        }
    }

    public class WeatherDateTimeContent : AbstractContent
    {
        //private Coord Coord { get; set; }
        private AbstractPredictManager Manager { get; set; }

        public WeatherDateTimeContent(int timeOut, string stationId, int index)
        {
            TimeOut = timeOut;
            IndexInContent = index;
            Manager = new PredictManager();
            var latitudeAndLongitude = (new PredictManager()).GetLatLngStationFor(stationId);
            //if (!string.IsNullOrEmpty(latitudeAndLongitude))
            //{
            //    var buff = latitudeAndLongitude.Split(';');
            //    Coord = new Coord()
            //    {
            //        lat = buff[0],
            //        lon = buff[1]
            //    };
            //}
            //else
            //{
            //    Coord = new Coord()
            //    {
            //        lat = "",
            //        lon = ""
            //    };
            //}
            ContentType = ContentType.WEATHER_DATE_TIME;
        }

        public new object PresentContent()
        {
            var weather = AbstractPredictManager.GetWeatherCity();
            return new WeatherViewModel(weather, Manager);
        }
    }

    public class InformationTableViewModel
    {
        public InformationTable InformationTable { get; set; }
        public string InformationTableId { get; set; }
        public string StationId { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string CssClass { get; set; }
        public int RowCount { get; set; }
        public IContent CurrentContent { get; set; }
        public List<IContent> Contents { get; set; }

        private List<IContent> GetContents(string informationTableId)
        {
            var result = new List<IContent>();
            var manager = new ContextManager();
            if (!string.IsNullOrEmpty(informationTableId))
            {
                var contents = manager.GetInformationTable(informationTableId)
                                                 .Contents;
                int index = 0;
                foreach (var item in contents)
                {
                    switch (item.ContentType)
                    {
                        case ContentType.DATE_TIME:
                            result.Add(new DateTimeContent(item.TimeOut, index));
                            break;
                        case ContentType.TEXT:
                            result.Add(new TextContent(item.InnerContent, item.TimeOut, index));
                            break;
                        case ContentType.TICKER:
                            result.Add(new TickerContent(item.InnerContent, item.TimeOut, index));
                            break;
                        case ContentType.VIDEO:
                            result.Add(new VideoContent(item.InnerContent, item.TimeOut, index));
                            break;
                        case ContentType.PICTURE:
                            result.Add(new PictureContent(item.TimeOut, index)); break;
                        case ContentType.WEATHER_DATE_TIME:
                            result.Add(new WeatherDateTimeContent(item.TimeOut, item.InnerContent, index));
                            break;
                        case ContentType.FORECAST:
                            result.Add(new ForecastContent(item.TimeOut, StationId, RowCount, index)); break;
                    }
                    index++;
                }
            }
            return result;
        }



        public InformationTableViewModel(InformationTable informationTable, string stationId, int rowCount)
        {
            InformationTable = informationTable;
            InformationTableId = informationTable?.Id ?? "-1";
            StationId = stationId;
            CssClass = informationTable?.ModuleType
                                       ?.CssClass ?? "";
            Width = (informationTable?.WidthWithModule ?? 0) * (informationTable?.ModuleType
                                                                                ?.WidthPx ?? 0);
            Height = (informationTable?.HeightWithModule
                                      ?? 0) * (informationTable?.ModuleType
                                                               ?.HeightPx ?? 0);
            RowCount = rowCount;
            Contents = GetContents(informationTable?.Id ?? "");
            CurrentContent = Contents.Any()
                           ? Contents.FirstOrDefault()
                           : null;
        }
        public InformationTableViewModel() { }
    }

    public class StationViewModel
    {
        public string StationId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
        public OptionsAndPreviewViewModel OptionsAndPreviewModel { get; set; }

        public StationViewModel(StationModel station, List<ModuleType> moduleTypes, List<ContentType> contentTypes, string selectedModuleTypeId)
        {
            StationId = station?.Id;
            Name = station?.Name;
            Description = station?.Description;
            Active = station?.Active ?? false;
            OptionsAndPreviewModel = new OptionsAndPreviewViewModel(station, moduleTypes, contentTypes, selectedModuleTypeId);
        }
    }

    public class OptionsAndPreviewViewModel
    {
        public OptionsViewModel Options { get; set; }
        public InformationTableViewModel InformationTablePreview { get; set; }
        public OptionsAndPreviewViewModel(StationModel station, List<ModuleType> moduleTypes, List<ContentType> contentTypes, string selectedModuleTypeId)
        {
            Options = new OptionsViewModel(station, moduleTypes, contentTypes, selectedModuleTypeId);
            InformationTablePreview = new InformationTableViewModel(station?.InformationTable, station?.Id ?? "-1", station?.InformationTable
                                                                                                                        ?.RowCount ?? 0);
        }
    }

    public class OptionsViewModel
    {
        public string InformationTableId { get; set; }
        public string StationId { get; set; }
        public string StationName { get; set; }
        public SelectList ModuleTypes { get; set; }
        private string SelectedModuleType { get; set; }
        public int WidthWithModules { get; set; }
        public int HeightWithModules { get; set; }
        public int RowCount { get; set; }
        public string IpAddress { get; set; }
        public List<ContentOption> ContentOptions { get; set; }
        public ContentAddViewModel ContentAddViewModel { get; set; }

        public OptionsViewModel(StationModel station, List<ModuleType> moduleTypes, List<ContentType> contentTypes, string selectedModuleTypeId)
        {
            ContentAddViewModel = new ContentAddViewModel(station.Id, new Content()
            {
                Id = Guid.NewGuid()
                         .ToString(),
                ContentType = ContentType.TEXT,
                InnerContent = "",
                TimeOut = 0
            }, contentTypes);
            InformationTableId = station.InformationTable
                                        ?.Id ?? "";
            StationName = station.Name;
            StationId = station.Id;
            WidthWithModules = station.InformationTable
                                      ?.WidthWithModule ?? 0;
            HeightWithModules = station.InformationTable
                                       ?.HeightWithModule ?? 0;
            RowCount = station.InformationTable
                              ?.RowCount ?? 0;
            IpAddress = station.IpDevice;
            var list = new List<SelectListItem>();
            foreach (var item in moduleTypes)
            {
                var itemSelectList = new SelectListItem()
                {
                    Text = item.Id,
                    Value = item.Id
                };
                list.Add(itemSelectList);
            }
            if (selectedModuleTypeId == null)
            {
                SelectListItem first = null;
                foreach (var item in list)
                {
                    first = item;
                    break;
                }

                if (first != null) first.Selected = true;
            }
            else if (list.Any(l => l.Value == selectedModuleTypeId))
            {
                SelectListItem first = null;
                foreach (var l in list)
                {
                    if (l.Value == selectedModuleTypeId)
                    {
                        first = l;
                        break;
                    }
                }
                if (first != null)
                    first.Selected = true;
            }
            else
            {
                SelectListItem first = null;
                foreach (var item in list)
                {
                    first = item;
                    break;
                }

                if (first != null)
                    first.Selected = true;
            }
            ModuleTypes = new SelectList(list);
            ContentOptions = new List<ContentOption>();
            var contents = station.InformationTable
                                  ?.Contents;
            if (contents != null)
            {
                foreach (var item in contents)
                {
                    var optionContentItem = new ContentOption(item, contentTypes, ((int) item.ContentType).ToString(),
                        (station.Id ?? ""));//, (station.InformationTable
                                                                                                                                                // ?.Id ?? ""));
                    ContentOptions.Add(optionContentItem);
                }
            }
        }

        public void SelectModuleType(string selectedModuleTypeId, List<ModuleType> moduleTypes)
        {
            if (string.IsNullOrEmpty(selectedModuleTypeId))
            {
                SelectListItem first = null;
                foreach (var item in ((List<SelectListItem>) ModuleTypes.Items))
                {
                    first = item;
                    break;
                }
                if (first != null) first.Selected = true;
            }
            else
            {   
                bool any = false;
                foreach (var i in ((List<SelectListItem>) ModuleTypes.Items))
                {
                    if (i.Value == selectedModuleTypeId)
                    {
                        any = true;
                        break;
                    }
                }
                if (any)
                {
                    SelectListItem first = null;
                    foreach (var i in ((List<SelectListItem>) ModuleTypes.Items))
                    {
                        if (i.Value == selectedModuleTypeId)
                        {
                            first = i;
                            break;
                        }
                    }
                    if (first != null) first.Selected = true;
                }
                else
                {
                    SelectListItem first = null;
                    foreach (var item in ((List<SelectListItem>) ModuleTypes.Items))
                    {
                        first = item;
                        break;
                    }
                    if (first != null) first.Selected = true;
                }
            }

            SelectedModuleType = selectedModuleTypeId ?? ((List<SelectListItem>)ModuleTypes.Items)
                                                                                           .FirstOrDefault()
                                                                                           ?.Value;
        }
    }

    public class ContentOption
    {
        public string StationId { get; set; }
        //public string InformationTableId { get; set; }
        public string ContentId { get; set; }
        private SelectList ContentTypes { get; set; }
        public string SelectedContentType { get; set; }
        public int Timeout { get; set; }
        public object InnerContent { get; set; }
        public string Description { get; set; }
        public string InputDescription { get; set; }

        public ContentOption(Content content, List<ContentType> contentTypes, string selectedContentType, string stationId)//, string informationTableId)
        {
            StationId = stationId;
            //InformationTableId = informationTableId;
            ContentId = content.Id;
            Timeout = content.TimeOut;
            InnerContent = content.InnerContent;
            SelectedContentType = selectedContentType;
            var list = new List<SelectListItem>();
            foreach (var item in contentTypes)
            {
                var itemSelectList = new SelectListItem()
                {
                    Text = item.ToString(),
                    Value = ((int)item).ToString()
                };
                list.Add(itemSelectList);
            }
            if (selectedContentType == null)
            {
                if (list.Any())
                {
                    SelectListItem first = null;
                    foreach (var item in list)
                    {
                        first = item;
                        break;
                    }

                    if (first != null) first.Selected = true;
                }
            }
            else if (list.Any(l => l.Value == selectedContentType))
            {
                SelectListItem first = null;
                foreach (var l in list)
                {
                    if (l.Value == selectedContentType)
                    {
                        first = l;
                        break;
                    }
                }

                if (first != null)
                    first.Selected = true;
            }
            ContentTypes = new SelectList(list);
            switch (SelectedContentType)
            {
                case "5":
                    InputDescription = "Введите текст";
                    Description = "Контент - бегущая строка вертикально";
                    break;
                case "6":
                    InputDescription = "";
                    Description = "Контент - дата и время";
                    break;
                case "7":
                    InputDescription = "";
                    Description = "Контент - погода, дата и время";
                    break;
                case "2":
                    InputDescription = "";
                    Description = "Контент - видеоролик";
                    break;
                case "3":
                    InputDescription = "";
                    Description = "Контент - изображение";
                    break;
                case "4":
                    InputDescription = "Введите текст бегущей строки";
                    Description = "Контент - бегущая строка";
                    break;
                case "1":
                    InputDescription = "Введите текст";
                    Description = "Контент - обычный текст";
                    break;
                case "0":
                    InputDescription = "";
                    Description = "Прогноз прибытия транспорта";
                    break;
            }
        }

        public void SelectContentType(string selectedContentType)
        {
            if (selectedContentType == null)
            {
                SelectListItem first = null;
                foreach (var item in ((List<SelectListItem>) ContentTypes.Items))
                {
                    first = item;
                    break;
                }
                if (first != null) first.Selected = true;
            }
            else if (((List<SelectListItem>)ContentTypes.Items)
                                                        .Any(i => i.Value == selectedContentType))
            {
                SelectListItem first = null;
                foreach (var i in ((List<SelectListItem>) ContentTypes.Items))
                {
                    if (i.Value == selectedContentType)
                    {
                        first = i;
                        break;
                    }
                }
                if (first != null) first.Selected = true;
            }
            else
            {
                SelectListItem first = null;
                foreach (var item in ((List<SelectListItem>) ContentTypes.Items))
                {
                    first = item;
                    break;
                }
                if (first != null) first.Selected = true;
            }
        }
    }

    public class ContentAddViewModel
    {
        public string StationId { get; set; }
        public SelectList ContentTypeSelectList { get; set; }
        public string SelectedContentType { get; set; }
        public string ContentId { get; set; }
        public string InnerContent { get; set; }
        public int TimeOut { get; set; }

        public ContentAddViewModel()
        {
            ContentTypeSelectList = new SelectList(new List<SelectListItem>(){new SelectListItem()
                                                                                { Selected = true,Value="",Text=""}});
        }

        public ContentAddViewModel(string stationId, Content content, List<ContentType> contentTypes)
        {
            StationId = stationId;
            var list = new List<SelectListItem>();
            contentTypes.Remove(ContentType.FORECAST);
            foreach (var item in contentTypes)
            {
                var selectListItem = new SelectListItem()
                {
                    Text = item.ToString(),
                    Value = ((int)item).ToString()
                };
                list.Add(selectListItem);
            }
            if (list.Any(l => l.Value == ((int)content.ContentType).ToString()))
            {
                SelectListItem first = null;
                foreach (var l in list)
                {
                    if (l.Value == ((int)content.ContentType).ToString())
                    {
                        first = l;
                        break;
                    }
                }
                if (first != null) first.Selected = true;
                SelectedContentType = first?.Value;
            }
            else
            {
                if (list.Any())
                {
                    SelectListItem first = null;
                    foreach (var item in list)
                    {
                        first = item;
                        break;
                    }
                    if (first != null) first.Selected = true;
                    SelectedContentType = first?.Value;
                }
            }
            ContentTypeSelectList = new SelectList(list);
            ContentId = content.Id;
            TimeOut = content.TimeOut;
            InnerContent = content.InnerContent;
        }

        public void SelectContentType(string selectedContentType)
        {
            if (ContentTypeSelectList.Items
                                    .Cast<SelectListItem>()
                                    .Any())
            {
                ContentTypeSelectList.Items
                                     .Cast<SelectListItem>()
                                     .ToList()
                                     .ForEach(s => s.Selected = false);
            }
            if (string.IsNullOrEmpty(selectedContentType)
                || ContentTypeSelectList.Items
                                        .Cast<SelectListItem>()
                                        .All(c => !string.Equals(c.Value, selectedContentType)))
            {
                if (ContentTypeSelectList.Items
                                         .Cast<SelectListItem>()
                                         .Any())
                {
                    SelectListItem first = null;
                    foreach (SelectListItem item in ContentTypeSelectList.Items)
                    {
                        first = item;
                        break;
                    }
                    if (first != null) first.Selected = true;
                    SelectedContentType = ContentTypeSelectList.Items
                                                               .Cast<SelectListItem>()
                                                               .FirstOrDefault()
                                                               ?.Value;
                }
                return;
            }

            SelectListItem first1 = null;
            foreach (SelectListItem c in ContentTypeSelectList.Items)
            {
                if (string.Equals(c.Value, selectedContentType))
                {
                    first1 = c;
                    break;
                }
            }
            if (first1 != null) first1.Selected = true;
            SelectedContentType = ContentTypeSelectList.Items
                                                       .Cast<SelectListItem>()
                                                       .FirstOrDefault(c => string.Equals(c.Value, selectedContentType))
                                                       ?.Value;
        }
    }
}