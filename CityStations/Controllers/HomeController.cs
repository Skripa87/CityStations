using CityStations.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Speech.Synthesis;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using CityStations.Models.StationForecast2020;

//using System.Speech.Synthesis;

namespace CityStations.Controllers
{
    public class HomeController : Controller
    {
        public HomeController()
        {
            try
            {
                _manager = new ContextManager();
                _moduleTypes = _manager.GetModuleTypes();
                _contentTypes = Enum.GetValues(typeof(ContentType))
                                    .Cast<ContentType>()
                                    .ToList();
            }
            catch (Exception e)
            {
                Logger.WriteLog(
                    $"Произошла ошибка {e.Message}, внутренне исключение {e.InnerException}, виновник события {e.Source}, подробности {e.StackTrace}",
                    User?.Identity == null ? "HomeController" : User?.Identity.GetUserId());
                RedirectToAction("Index", "Offline");
            }
        }

        private ContextManager _manager;

        private static string _selectedModuleTypeId;

        private static List<ModuleType> _moduleTypes;

        private static List<ContentType> _contentTypes;
        private static StationViewModel Station { get; set; }
        private static OptionsAndPreviewViewModel OptionsAndPreviewViewModel { get; set; }
        private static OptionsViewModel OptionsViewModel { get; set; }
        private static ContentAddViewModel ContentAddViewModel { get; set; }
        private static InformationTableViewModel InformationTableViewModel { get; set; }

        [HttpGet]
        public ActionResult Index()
        {
            ViewData["MyAcc"] = User?.Identity
                                    ?.Name == "skripinalexey1987@gmail.com";
            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult ReconfigurateAllDevice()
        {
            var devManager = new DeviceManager();
            devManager.ConfigurateAllDevice();
            return RedirectToAction("IndexAuthtorize");
        }

        [HttpGet]
        [Authorize]
        public ActionResult RebootAllStations()
        {
            var devManager = new DeviceManager();
            devManager.RebootStations();
            return RedirectToAction("IndexAuthtorize");
        }

        [HttpGet]
        [Authorize]
        public ActionResult IndexAuthtorize()
        {
            Logger.WriteLog($"Пользователь {User?.Identity?.GetUserName() ?? "Не определеный пользователь"} зашел в систему!", User?.Identity?.GetUserId() ?? "HomeController");
            ViewData["MyAcc"] = User?.Identity
                                    ?.Name == "skripinalexey1987@gmail.com";
            _manager = new ContextManager();
            _manager.CheckAllAccessCodeInInformationTable();
            return View();
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public PartialViewResult SearchBlockPart(string searchBoxText)
        {
            _manager = new ContextManager();
            return string.Equals(searchBoxText, "onlyActivateStationAndNothingMore", new StringComparison())
                         ? PartialView(_manager.GetActivStationsAsync().Result)
                         : PartialView(_manager.FindStationOnNamePartAsync(searchBoxText).Result);
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public PartialViewResult SelectStation(string stationId)
        {
            try
            {
                _manager = new ContextManager();
                var station = _manager.GetStation(stationId);
                if (station == null)
                {
                    return PartialView();
                }
                ContentAddViewModel = new ContentAddViewModel(station.Id, new Content()
                {
                    ContentType = ContentType.TEXT,
                    InnerContent = "",
                    TimeOut = 0,
                    Id = Guid.NewGuid()
                        .ToString()
                }, _contentTypes);
                _moduleTypes = _manager.GetModuleTypes();
                _selectedModuleTypeId = station.InformationTable
                                            ?.ModuleType
                                            ?.Id ?? "0";
                Station = new StationViewModel(station, _moduleTypes, _contentTypes, _selectedModuleTypeId);
                InformationTableViewModel = Station?.OptionsAndPreviewModel?.InformationTablePreview;
                ViewData["timeOutNextContent"] = GetTimeOutNextContent(Station, ContainerClassType.STATION,
                    out var cssClass, out var centralPosition);
                ViewData["CssClass"] = cssClass;
                ViewData["CentralPosition"] = centralPosition;
                ViewData["stationId"] = station.Id;
                ViewData["informationTableId"] = station.InformationTable?.Id;
                Logger.WriteLog(
                    $"Пользователь {User.Identity.GetUserName()} перешел в окно детальной настройки остановочного павильона с идентификатором {stationId} - {Station?.Name ?? ""}",
                    User.Identity.GetUserId());
                return PartialView(Station);
            }
            catch (Exception ex)
            {
                Logger.WriteLog(
                    $"Пользователь {User.Identity.GetUserId()} - при попытке доступа к настройкам остановочного павильона с идентификатором {stationId} - {Station?.Name} инициировал ошибку {ex.Message}, подробности {ex.StackTrace}",
                    User?.Identity?.GetUserId() ?? "HomeController");
                ViewData["StationId"] = stationId;
                return PartialView("Error");
            }
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public PartialViewResult SaveChangeOptions(string stationId, string informationTableId, int widthWithModules = 0, int heightWithModules = 0,
                                                   int rowCount = 0, int timeOutPredictShow = 0, string ip = "")
        {
            try
            {
                var contextManager = new ContextManager();
                contextManager.SetIpAddressDevice(stationId, ip);
                contextManager.ChangeInformationTable(informationTableId, new InformationTable()
                {
                    HeightWithModule = heightWithModules,
                    RowCount = rowCount,
                    WidthWithModule = widthWithModules
                });
                var station = contextManager.GetStation(stationId);
                _selectedModuleTypeId = station?.InformationTable
                                            ?.ModuleType
                                            ?.Id ?? "0";
                OptionsAndPreviewViewModel = new OptionsAndPreviewViewModel(station, _moduleTypes,
                    _contentTypes, _selectedModuleTypeId);
                InformationTableViewModel = OptionsAndPreviewViewModel?.InformationTablePreview;
                OptionsViewModel = OptionsAndPreviewViewModel?.Options;
                ViewData["timeOutNextContent"] = GetTimeOutNextContent(OptionsAndPreviewViewModel,
                    ContainerClassType.OPTION, out var cssClass, out var centralPosition);
                ViewData["CssClass"] = cssClass;
                ViewData["CentralPosition"] = centralPosition;
                ViewData["stationId"] = station?.Id;
                ViewData["informationTableId"] = station?.InformationTable?.Id;
                Logger.WriteLog(
                    $"Пользователь {User.Identity.GetUserId()} выполнил сохранение данных настроек остановочного павильона с идентификатором {stationId} - {station?.Name}",
                    station?.Id ?? "0");
            }
            catch (Exception ex)
            {
                Logger.WriteLog(
                    $"Попытка пользователя {User.Identity.GetUserId()} завершилась неудачей в результате ошибки {ex.Message} подробности {ex.InnerException}, стек вызова {ex.StackTrace}", User.Identity.GetUserId());
                ViewData[stationId] = stationId;
            }
            return PartialView("PreviewAndOptionsBlock", OptionsAndPreviewViewModel);
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public PartialViewResult SelectModuleType(string parametr)
        {
            try
            {
                var parametrList = parametr.Split(';');
                _selectedModuleTypeId = parametrList[0];
                var informationTableId = parametrList[2];
                var stationId = parametrList[3];
                var contextManager = new ContextManager();
                contextManager.SetModuleType(informationTableId, _selectedModuleTypeId);
                var station = contextManager.GetStation(stationId);
                OptionsAndPreviewViewModel = new OptionsAndPreviewViewModel(station, _moduleTypes,
                    _contentTypes, _selectedModuleTypeId);
                ViewData["timeOutNextContent"] = GetTimeOutNextContent(OptionsAndPreviewViewModel,
                    ContainerClassType.OPTION, out var cssClass, out var centralPosition);
                ViewData["CssClass"] = cssClass;
                ViewData["CentralPosition"] = centralPosition;
                Logger.WriteLog(
                    $"Пользователь с идентификатором {User.Identity.GetUserId()} выполнил выбор типа модуля,  в процессе изменени настроик информационного табло",
                    User.Identity.GetUserId());
            }
            catch (Exception ex)
            {
                Logger.WriteLog(
                    $" В результате попытки выбора типа модуля произошла ошибка, инициатор - {User.Identity.GetUserId()}, описание ошибки {ex.Message} подробности {ex.InnerException}, стек вызова {ex.StackTrace}",
                    User.Identity.GetUserId());
            }
            return PartialView("PreviewAndOptionsBlock", OptionsAndPreviewViewModel);
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public PartialViewResult SelectContentType(string parametr)
        {
            try
            {
                var index = parametr.Split(';')[0];
                ContentAddViewModel.SelectContentType(index);
            }
            catch (Exception ex)
            {
                Logger.WriteLog(
                    $"Пользователь с идентификатором {User.Identity.GetUserId()}, попытка выбора типа контента привела к ошибке, в результате {ex.Message}, подробности {ex.InnerException}, стек вызова {ex.StackTrace} ",
                    User.Identity.GetUserId());
            }
            return PartialView(ContentAddViewModel);
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult ActivateInformationTable(string stationId, string isActive)
        {
            try
            {
                _manager = new ContextManager();
                var station = _manager.ActivateInformationTable(stationId);
                _moduleTypes = _manager.GetModuleTypes();
                Station = new StationViewModel(station, _moduleTypes, _contentTypes,
                    (_moduleTypes?.FirstOrDefault()?.Id ?? ""));
                InformationTableViewModel = Station?.OptionsAndPreviewModel
                    ?.InformationTablePreview;
                _manager = null;
                Logger.WriteLog(
                    $"На остановочном павильоне {Station.Name}, выполнена активация информационного табло, идентификатор = {Station.StationId}",
                    Station.StationId);
            }
            catch (Exception ex)
            {
                Logger.WriteLog(
                    $"На остановочном павильоне {Station.Name}, во время активации информационного табло, идентификатор = {Station.StationId}, произошла ошибка {ex.Message}, подробности {ex.InnerException}, стек вызова {ex.StackTrace}",
                    Station.StationId);
            }
            return View("SelectStation", Station);
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult DeactivateInformationTable(string stationId)
        {
            try
            {
                _manager = new ContextManager();
                var station = _manager.DeactivateInformationTable(stationId);
                Station = new StationViewModel(station, _moduleTypes, _contentTypes, _selectedModuleTypeId);
                Logger.WriteLog(
                    $"Пользователь {User.Identity.GetUserId()} выполнил деактивацию информационного табло на остановке с идетнитификатором {stationId}",
                    User.Identity.GetUserId());
            }
            catch (Exception ex)
            {
                Logger.WriteLog(
                    $"Пользователь {User.Identity.GetUserId()}, произошла ошибка в процессе деактивации остановки с идетнитификатором {stationId}, ошибка {ex.Message} подробнее {ex.InnerException} стек вызова {ex.StackTrace}",
                    User.Identity.GetUserId());
            }
            return View("SelectStation", Station);
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public PartialViewResult CreateAdditionalContent(string stationId, ContentAddViewModel model)
        {
            _manager = new ContextManager();
            var contentType = _contentTypes.Find(c => string.Equals(((int)c).ToString(), model.SelectedContentType));
            var content = new Content()
            {
                ContentType = contentType,
                Id = model.ContentId,
                InnerContent = model.InnerContent,
                TimeOut = model.TimeOut
            };
            _manager.CreateContent(stationId, content);
            var station = _manager.GetStation(stationId);
            var sModuleType = station?.InformationTable
                                     ?.ModuleType?.Id ?? "";
            OptionsViewModel = new OptionsViewModel(station, _moduleTypes, _contentTypes, sModuleType);
            InformationTableViewModel = new InformationTableViewModel(station?.InformationTable, station?.Id ?? "", station?.InformationTable
                                                                                                                          ?.RowCount ?? 0);
            return PartialView("InformationTableOptions", OptionsViewModel);
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult ContentAdd(string stationId)
        {
            var content = new Content()
            {
                ContentType = ContentType.TEXT,
                Id = Guid.NewGuid()
                         .ToString(),
                InnerContent = "",
                TimeOut = 0
            };
            ContentAddViewModel = new ContentAddViewModel(stationId, content, _contentTypes);
            return View("ContentAdd", ContentAddViewModel);
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public PartialViewResult ContentOption(string contentId, string stationId, string contentType, string innerContent, int contentTimeout)
        {
            StationModel station = null;
            try
            {
                var newContent = new Content()
                {
                    InnerContent = contentType == "0"
                        ? stationId
                        : innerContent,
                    TimeOut = contentTimeout,
                    ContentType = contentType == "0"
                        ? ContentType.FORECAST
                        : _contentTypes.Find(f => string.Equals(((int)f).ToString(), contentType))
                };
                _manager = new ContextManager();
                _manager.ChangeContent(contentId, newContent);
                var content = _manager.GetContent(contentId);
                station = _manager.GetStation(stationId);
                //var informationTableId = station?.InformationTable
                //                             ?.Id ?? "-1";
                var contentOption = new ContentOption(content, _contentTypes, ((int)content.ContentType).ToString(),
                    stationId);
                OptionsAndPreviewViewModel =
                    new OptionsAndPreviewViewModel(station, _moduleTypes, _contentTypes, _selectedModuleTypeId);
                OptionsViewModel = OptionsAndPreviewViewModel?.Options;
                InformationTableViewModel = OptionsAndPreviewViewModel?.InformationTablePreview;
                Logger.WriteLog(
                    $"Пользователь {User.Identity.GetUserId()} выполнил манипуляции по изменению свойств контента на останвочном павильоне с идентификатором {stationId} - {station?.Name}",
                    User.Identity.GetUserId());
                return PartialView(contentOption);
            }
            catch (Exception ex)
            {
                Logger.WriteLog(
                    $"В результате попытки изменени свойств контента сотанвочного павильона с идентификатором {stationId} - {station?.Name}, произошла ошибка {ex.Message}, подробно {ex.InnerException}, стек вызова {ex.StackTrace}",
                    User.Identity.GetUserId());
                ViewData["StationId"] = stationId;
                return PartialView("Error");
            }
        }

        //[Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public PartialViewResult CurrentContentView(int index, string informationTableId, string stationId)
        {
            try
            {
                _manager = new ContextManager();
                var informationTable = _manager.GetInformationTable(informationTableId);
                var station = _manager.GetStation(stationId);
                var stationName = station?.Name ?? "";
                var rowCount = informationTable?.RowCount ?? 0;
                var informationTableViewModel = new InformationTableViewModel(informationTable, stationId, rowCount);
                var contents = informationTableViewModel.Contents;
                index++;
                if (contents != null && index >= contents.Count)
                {
                    index = 0;
                }
                var modelContent = informationTableViewModel.Contents
                                                            .ElementAt(index);
                var indexNext = index + 1;
                if (contents != null && indexNext >= contents.Count)
                {
                    indexNext = 0;
                }
                var nextContent = informationTableViewModel.Contents
                                                           .ElementAt(indexNext);
                //ViewData["informationTableViewData"] = informationTableViewModel;
                ViewData["timeOutNextContent"] = nextContent?.TimeOut ?? 0;
                ViewData["stationId"] = informationTableViewModel.StationId;
                ViewData["informationTableId"] = informationTableViewModel.InformationTableId;
                ViewData["CssClass"] = informationTableViewModel.CssClass ?? "";
                ViewData["CentralPosition"] = ((informationTableViewModel.Height) / 2 - 10) + "px";
                ViewData["WidthTablo"] = informationTableViewModel.Width;
                ViewData["HeightTablo"] = informationTableViewModel.Height;
                Logger.WriteLog(
                    $"Выполнен запрос на изменение текущего контента остановкой {stationId} - {stationName}",
                    stationId);
                return PartialView(modelContent);
            }
            catch (Exception ex)
            {
                Logger.WriteLog($"Ошибка отображения содержимого у остановки {stationId}, {ex.Message} подробности {ex.StackTrace}", stationId);
                ViewData["StationId"] = stationId;
                return PartialView("Error");
            }
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public PartialViewResult RemoveContent(string contentId, string stationId)
        {
            try
            {
                _manager = new ContextManager();
                _manager.RemoveContent(stationId, contentId);
                var station = _manager.GetStation(stationId);
                Station = new StationViewModel(station, _moduleTypes, _contentTypes, _selectedModuleTypeId);
                OptionsAndPreviewViewModel = Station?.OptionsAndPreviewModel;
                OptionsViewModel = Station?.OptionsAndPreviewModel?.Options;
                InformationTableViewModel = Station?.OptionsAndPreviewModel?.InformationTablePreview;
                ViewData["timeOutNextContent"] = GetTimeOutNextContent(Station, ContainerClassType.STATION,
                    out var cssClass, out var centralPosition);
                ViewData["CssClass"] = cssClass;
                ViewData["CentralPosition"] = centralPosition;
                Logger.WriteLog(
                    $"Пользователь {User.Identity.GetUserName()} выполнил удаление контента на остановке с идентификатором {stationId} - {Station?.Name}",
                    User.Identity.GetUserId());
                return PartialView("InformationTableOptions", OptionsViewModel);
            }
            catch (Exception ex)
            {
                Logger.WriteLog(
                    $"Не удалось удалить контент из списка активного контента у остановочного павильона с идентификатором {stationId} - {Station.Name}, причина: {ex.Message}, подробности {ex.StackTrace}",
                    User.Identity.GetUserId());
                ViewData["StationId"] = stationId;
                return PartialView("Error");
            }
        }

        public ActionResult SetConfiguration(string stationId, string ip)
        {
            var deviceManager = new DeviceManager();
            if (string.IsNullOrEmpty(ip)) return PartialView("SelectStation", Station);
            deviceManager.ConfigurateDevice(ip, stationId);
            _manager = new ContextManager();
            var station = _manager.GetStation(stationId);
            Station = new StationViewModel(station, _moduleTypes, _contentTypes, _selectedModuleTypeId);
            OptionsAndPreviewViewModel = Station?.OptionsAndPreviewModel;
            InformationTableViewModel = OptionsAndPreviewViewModel?.InformationTablePreview;
            OptionsViewModel = OptionsAndPreviewViewModel?.Options;
            ViewData["timeOutNextContent"] =
                GetTimeOutNextContent(Station, ContainerClassType.STATION, out _, out _);
            ViewData["stationId"] = Station?.StationId;
            ViewData["InformationTable"] = Station?.OptionsAndPreviewModel?.InformationTablePreview;
            ViewData["informationTableId"] =
                Station?.OptionsAndPreviewModel?.InformationTablePreview?.InformationTableId ?? "-1";
            ViewData["CssClass"] = Station?.OptionsAndPreviewModel?.InformationTablePreview?.CssClass;
            ViewData["CentralPosition"] =
                (((Station?.OptionsAndPreviewModel?.InformationTablePreview?.Height ?? 0) / 2) - 10) + "px";
            ViewData["WidthTablo"] = InformationTableViewModel?.Width ?? 0;
            ViewData["HeightTablo"] = InformationTableViewModel?.Height ?? 0;
            Logger.WriteLog($"Поступил запрос от остановки с идентификатором {stationId} - {Station?.Name}",
                stationId);
            return View("SelectStation", Station);
        }

        [HttpPost]
        [Authorize]
        //[ValidateAntiForgeryToken]
        public ActionResult Monitoring(string mode)
        {
            List<MonitoringViewModel> model = new List<MonitoringViewModel>();
            try
            {
                _manager = new ContextManager();
                var stations = _manager.GetActivStations();
                //_manager.RemoveOldEvents();
                var eventsByFiveMinuts = _manager.GetActulEvents();
                var errorEvents = _manager.GetErrors();
                foreach (var station in stations)
                {
                    var events = eventsByFiveMinuts.FindAll(e => string.Equals(e.Initiator, station?.Id, new StringComparison()));
                    var eventsE =
                        errorEvents.FindAll(e => string.Equals(e.Initiator, station.Id, new StringComparison()));
                    var monitoring = new MonitoringViewModel(station, events, eventsE);
                    model.Add(monitoring);
                }
                return View(model);
            }
            catch (Exception ex)
            {
                Logger.WriteLog($"{ex.Message} {ex.StackTrace}", "Monitoring");
                return RedirectToAction("Error");
            }
        }


        public FileResult CreateConfigFile(string stationId)
        {
            byte[] buffer = new byte[1024];
            try
            {
                _manager = new ContextManager();
                var text = "http://92.50.187.210/" + this.HttpContext.Request.ApplicationPath + "/Home/DisplayInformationTable?stationId=" + stationId + "&accessCode=" + _manager.SetAccessCode(stationId);
                var file = System.IO.File.Create("d:\\cromium.desktop");
                file.Close();
                using (var streamWriter = new StreamWriter(file.Name))
                {
                    streamWriter.WriteLine("[Desktop Entry]");
                    streamWriter.WriteLine("Encoding=UTF-8");
                    streamWriter.WriteLine("Name=Connect");
                    streamWriter.WriteLine("Comment=Checks internet connectivity");
                    streamWriter.WriteLine("Exec=/usr/bin/chromium-browser -incognito --noerrdialogs --kiosk " + text);
                    streamWriter.Close();
                }

                file = System.IO.File.Open("d:\\cromium.desktop", FileMode.Open);
                file.Read(buffer, 0, 1024);
                file.Close();
                Logger.WriteLog(
                    $"Пользователь {User.Identity.GetUserName()} выполнил загрузку конфигурационного файла для остановки с идентификатором {stationId} - {Station?.Name}",
                    User.Identity.GetUserId());
            }
            catch (Exception ex)
            {
                Logger.WriteLog(
                    $"Ошибка в процессе попытки пользователя {User.Identity.GetUserName()} выполнить загрузку конфигурационного файла для остановки с идентификатором {stationId} - {Station?.Name}, по причине {ex.Message}, подробности {ex.InnerException}, стек вызова {ex.StackTrace}",
                    User.Identity.GetUserId());
            }
            return File(buffer, "text/txt", "cromium.desktop");
        }

        [HttpGet]
        public ActionResult DisplayInformationTable(string stationId, string accessCode)
        {
            try
            {
                _manager = new ContextManager();
                _moduleTypes = _manager.GetModuleTypes();
                _contentTypes = Enum.GetValues(typeof(ContentType))
                                    .Cast<ContentType>()
                                    .ToList();
                var station = _manager.GetStation(stationId);
                if (station?.InformationTable == null)
                {
                    return RedirectToAction("Error");
                }
                if (accessCode != station.InformationTable.AccessCode || station.Active == false)
                {
                    ViewData["StationId"] = stationId;
                    return RedirectToAction("Error");
                }
                Station = new StationViewModel(station, _moduleTypes, _contentTypes, _selectedModuleTypeId);
                OptionsAndPreviewViewModel = Station?.OptionsAndPreviewModel;
                InformationTableViewModel = OptionsAndPreviewViewModel?.InformationTablePreview;
                OptionsViewModel = OptionsAndPreviewViewModel?.Options;
                ViewData["timeOutNextContent"] =
                    GetTimeOutNextContent(Station, ContainerClassType.STATION, out _, out _);
                ViewData["stationId"] = Station?.StationId;
                ViewData["InformationTable"] = Station?.OptionsAndPreviewModel?.InformationTablePreview;
                ViewData["informationTableId"] =
                    Station?.OptionsAndPreviewModel?.InformationTablePreview?.InformationTableId ?? "-1";
                ViewData["CssClass"] = Station?.OptionsAndPreviewModel?.InformationTablePreview?.CssClass;
                ViewData["CentralPosition"] =
                    (((Station?.OptionsAndPreviewModel?.InformationTablePreview?.Height ?? 0) / 2) - 10) + "px";
                ViewData["WidthTablo"] = InformationTableViewModel?.Width ?? 0;
                ViewData["HeightTablo"] = InformationTableViewModel?.Height ?? 0;
                var eventId = Logger.WriteLog($"Поступил запрос от остановки с идентификатором {stationId} - {Station?.Name}",
                    stationId);
                //CurrentRequestSituations.UpdateRequest(stationId,eventId);
                return View();
            }
            catch (Exception e)
            {
                Logger.WriteLog($"Запрос от остановки с идентификатором {stationId} - {Station?.Name} завершился с ошибкой {e.Message}, подробности {e.InnerException}, стек вызова {e.StackTrace}",
                    stationId);
                ViewData["StationId"] = stationId;
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult TryReloadInformationTableAfterError(string stationId)
        {
            try
            {
                var manager = new ContextManager();
                var station = manager.GetStation(stationId);
                if (station?.InformationTable.AccessCode == null)
                {
                    ViewData["stationId"] = stationId;
                    return View("Error");
                }
                Logger.WriteLog("Выполнено восстановление после критической ошибки!", stationId);
                return RedirectToAction("DisplayInformationTable",
                    new { stationId = station.Id, accessCode = station.InformationTable.AccessCode });
            }
            catch (Exception ex)
            {
                Logger.WriteLog($"Восстановление после критической ошибки не удалось, требуется повторная попытка востановления! Причина {ex.Message}, подробности {ex.InnerException}, стек вызова {ex.StackTrace}", stationId);
                ViewData["stationId"] = stationId;
                return View("Error");
            }
        }

        [HttpPost]
        public PartialViewResult Upload(HttpPostedFileBase upload)
        {
            if (upload != null)
            {
                string fileName = Path.GetFileName(upload.FileName);
                upload.SaveAs(Server.MapPath("~/Files/" + fileName));
            }
            return PartialView("FileLoad");
        }

        [HttpGet]
        public ActionResult Error(string stationId = "Error")
        {
            StationModel station = null;
            try
            {
                _manager = new ContextManager();
                _moduleTypes = _manager.GetModuleTypes();
                _contentTypes = Enum.GetValues(typeof(ContentType))
                    .Cast<ContentType>()
                    .ToList();
                station = _manager.GetStation(stationId);
            }
            catch (Exception ex)
            {
                Logger.WriteLog($"Проблемы с базой {ex.Message} {ex.StackTrace}", "HomeController");
            }
            station = station ?? new StationModel
            {
                Id = "Error",
                Active = true,
                Description = "",
                IdForRnis = null,
                NameForRnis = "",
                Lng = 55.982370,
                Lat = 54.735950,
                Type = false,
                Name = "Error",
                InformationTable = new InformationTable
                {
                    Id = "Error",
                    HeightWithModule = 2,
                    WidthWithModule = 4,
                    AccessCode = "$olnechniKrug",
                    ModuleType = new ModuleType
                    {
                        CssClass = "p4cssClass",
                        Id = "Q4Y10V5H",
                        WidthPx = 80,
                        HeightPx = 40
                    },
                    RowCount = 4,
                    Contents = new List<Content>
                            {
                                new Content()
                                {
                                    Id = "Error",
                                    ContentType = ContentType.DATE_TIME,
                                    TimeOut = 15,
                                    InnerContent = "Error"
                                }
                            }
                }
            };
            _contentTypes = _contentTypes ?? new List<ContentType>() { ContentType.DATE_TIME };
            _moduleTypes = _moduleTypes ?? new List<ModuleType>() { new ModuleType { CssClass = "p4cssClass", Id = "Q4Y10V5H", WidthPx = 80, HeightPx = 40 } };
            Station = new StationViewModel(station, _moduleTypes, _contentTypes, "Q4Y10V5H");
            OptionsAndPreviewViewModel = Station?.OptionsAndPreviewModel;
            InformationTableViewModel = OptionsAndPreviewViewModel?.InformationTablePreview;
            OptionsViewModel = OptionsAndPreviewViewModel?.Options;
            ViewData["timeOutNextContent"] = GetTimeOutNextContent(Station, ContainerClassType.STATION, out _, out _);
            ViewData["stationId"] = Station?.StationId;
            ViewData["InformationTable"] = Station?.OptionsAndPreviewModel?.InformationTablePreview;
            ViewData["informationTableId"] =
                Station?.OptionsAndPreviewModel?.InformationTablePreview?.InformationTableId ?? "-1";
            ViewData["CssClass"] = Station?.OptionsAndPreviewModel?.InformationTablePreview?.CssClass;
            ViewData["CentralPosition"] = (((Station?.OptionsAndPreviewModel?.InformationTablePreview?.Height ?? 0) / 2) - 10) + "px";
            ViewData["WidthTablo"] = InformationTableViewModel?.Width ?? 0;
            ViewData["HeightTablo"] = InformationTableViewModel?.Height ?? 0;
            Logger.WriteLog($"Поступил запрос от остановки с идентификатором {stationId} - {Station?.Name}",
               stationId);
            return View("DisplayInformationTable", Station);
        }

        private int GetTimeOutNextContent(object container, ContainerClassType key, out string cssClass, out string centralPosition)
        {
            int timeOutNextContent;
            cssClass = "";
            centralPosition = "";
            IContent currentContent = null;
            int contentsCount = -1;
            if (key == ContainerClassType.OPTION)
            {
                currentContent = ((OptionsAndPreviewViewModel)container)?.InformationTablePreview
                                                                         ?.CurrentContent;
                cssClass = ((OptionsAndPreviewViewModel)container)?.InformationTablePreview
                                                                   .CssClass ?? "";
                centralPosition = ((((OptionsAndPreviewViewModel)container)?.InformationTablePreview
                                    ?.Height ?? 0) / 2 - 10) + "px";
                contentsCount = ((OptionsAndPreviewViewModel)container)?.InformationTablePreview
                                                                       ?.Contents
                                                                       ?.Count ?? 0;
            }
            else if (key == ContainerClassType.STATION)
            {
                currentContent = ((StationViewModel)container)?.OptionsAndPreviewModel
                                                              ?.InformationTablePreview
                                                              ?.CurrentContent;
                cssClass = ((StationViewModel)container)?.OptionsAndPreviewModel
                                                        ?.InformationTablePreview
                                                        ?.CssClass ?? "";
                centralPosition = ((((StationViewModel)container)?.OptionsAndPreviewModel
                                    ?.InformationTablePreview
                                    ?.Height ?? 0) / 2 - 10) + "px";
                contentsCount = ((StationViewModel)container)?.OptionsAndPreviewModel
                                                             ?.InformationTablePreview
                                                             ?.Contents
                                                             ?.Count ?? 0;
            }
            var indexNextContent = (currentContent?.IndexInContent + 1 ?? 0) >= contentsCount
                                 ? 0
                                 : currentContent?.IndexInContent + 1 ?? 0;
            if (contentsCount > 0)
            {

                if (key == ContainerClassType.OPTION)
                {
                    timeOutNextContent = ((OptionsAndPreviewViewModel)container)?.InformationTablePreview
                                                                                ?.Contents
                                                                                ?.ElementAt(indexNextContent)
                                                                                ?.TimeOut ?? 0;
                }
                else
                {
                    timeOutNextContent = ((StationViewModel)container)?.OptionsAndPreviewModel
                                                                      ?.InformationTablePreview
                                                                      ?.Contents
                                                                      ?.ElementAt(indexNextContent)
                                                                      ?.TimeOut ?? 0;
                }
            }
            else
            {
                timeOutNextContent = 0;
            }
            return timeOutNextContent;
        }

        [HttpGet]
        public static FileStreamResult GetAudioPredictFree(string stationId)
        {
            var streamAudio = new MemoryStream();
            var manager = new PredictManager();
            var predictNotObject = manager.GetStationForecast(stationId).ToList();
            predictNotObject = predictNotObject.Count() > 4
                             ? predictNotObject.GetRange(0, 4)
                             : predictNotObject;
            var contextManager = new ContextManager();
            var station = contextManager.GetStation(stationId);
            var isNewService = station?.InformationTable?.ServiceType == ServiceType.NEW;
            var text = "Уважаемые пасажиры!" + '\n';
            if (!isNewService)
            {
                foreach (StationForecast item in predictNotObject)
                {
                    var time = item.Arrt != null
                        ? (((int) item.Arrt / 60) == 0 ? "1" : ((int) item.Arrt / 60).ToString())
                        : "";
                    var resultTime = "";
                    switch (time)
                    {
                        case "1":
                            resultTime = "одну минуту";
                            break;
                        case "2":
                            resultTime = "две минуты";
                            break;
                        case "3":
                            resultTime = "три минуты";
                            break;
                        case "4":
                            resultTime = "четыре минуты";
                            break;
                        case "5":
                            resultTime = "пять минут";
                            break;
                        case "6":
                            resultTime = "шесть минут";
                            break;
                        case "7":
                            resultTime = "семь минут";
                            break;
                        case "8":
                            resultTime = "восемь минут";
                            break;
                        case "9":
                            resultTime = "девять минут";
                            break;
                        case "10":
                            resultTime = "десять минут";
                            break;
                        case "11":
                            resultTime = "одинадцать минут";
                            break;
                        case "12":
                            resultTime = "двенадцать минут";
                            break;
                        case "13":
                            resultTime = "тринадцать минут";
                            break;
                        case "14":
                            resultTime = "четырнадцать минут";
                            break;
                        case "15":
                            resultTime = "пятнадцать минут";
                            break;
                        case "16":
                            resultTime = "шестнадцать минут";
                            break;
                        case "17":
                            resultTime = "семнадцать минут";
                            break;
                        case "18":
                            resultTime = "восемнадцать минут";
                            break;
                        case "19":
                            resultTime = "девятнадцать минут";
                            break;
                        case "20":
                            resultTime = "двадцать минут";
                            break;
                    }

                    text += $"Через {resultTime} , ожидаем прибытие маршрута номер {item.Rnum}.";
                    text += '\n';
                }
            }
            else
            {
                foreach (ForecastsItem item in predictNotObject)
                {
                    var time = item.arrTime != null
                        ? (((int)item.arrTime / 60) == 0 ? "1" : ((int)item.arrTime / 60).ToString())
                        : "";
                    var resultTime = "";
                    switch (time)
                    {
                        case "1":
                            resultTime = "одну минуту";
                            break;
                        case "2":
                            resultTime = "две минуты";
                            break;
                        case "3":
                            resultTime = "три минуты";
                            break;
                        case "4":
                            resultTime = "четыре минуты";
                            break;
                        case "5":
                            resultTime = "пять минут";
                            break;
                        case "6":
                            resultTime = "шесть минут";
                            break;
                        case "7":
                            resultTime = "семь минут";
                            break;
                        case "8":
                            resultTime = "восемь минут";
                            break;
                        case "9":
                            resultTime = "девять минут";
                            break;
                        case "10":
                            resultTime = "десять минут";
                            break;
                        case "11":
                            resultTime = "одинадцать минут";
                            break;
                        case "12":
                            resultTime = "двенадцать минут";
                            break;
                        case "13":
                            resultTime = "тринадцать минут";
                            break;
                        case "14":
                            resultTime = "четырнадцать минут";
                            break;
                        case "15":
                            resultTime = "пятнадцать минут";
                            break;
                        case "16":
                            resultTime = "шестнадцать минут";
                            break;
                        case "17":
                            resultTime = "семнадцать минут";
                            break;
                        case "18":
                            resultTime = "восемнадцать минут";
                            break;
                        case "19":
                            resultTime = "девятнадцать минут";
                            break;
                        case "20":
                            resultTime = "двадцать минут";
                            break;
                    }

                    text += $"Через {resultTime} , ожидаем прибытие маршрута номер {item.num}.";
                    text += '\n';
                }
            }
            using (SpeechSynthesizer synth = new SpeechSynthesizer())
            {
                //var mSoundPlayer = new SoundPlayer();
                //synth.SetOutputToWaveStream(streamAudio);
                synth.Speak(text);
                streamAudio.Position = 0;
                synth.SetOutputToNull();
            }
            return new FileStreamResult(streamAudio, "audio/wav");
        }

        private char IsHaveCharacter(string input, out string str)
        {
            char result = ' ';
            str = "";
            foreach (var item in input)
            {
                if (!('0' <= item && item <= '9'))
                {
                    result = item;
                    break;
                }
            }
            input.ToList()
                 .RemoveAll(c => !(c <= '9' && c >= '0'));
            foreach (var ch in input)
            {
                str += ch;
            }
            return result;
        }

        public async Task<FileStreamResult> GetAudioPredictYa(string stationId)
        {
            const string iamKey = "AQVNzRBjik29VT1yshMtwaVKrQBsNfO_Fo0ragsL";
            IPredictManager manager;
            List<IForecast> predictNotObject;
            manager = new PredictManager();
            predictNotObject = ((PredictManager)manager).GetStationForecast(stationId).ToList();
            predictNotObject = predictNotObject.Count() > 4
                             ? predictNotObject.GetRange(0, 4)
                             : predictNotObject;
            var contextManager = new ContextManager();
            var station = contextManager.GetStation(stationId);
            var isNewService = station?.InformationTable?.ServiceType == ServiceType.NEW;
            var text = "Уважаемые пасажиры!" + '\n';
            if (isNewService)
            {
                foreach (StationForecast item in predictNotObject)
                {
                    var time = item.Arrt != null
                        ? (((int) item.Arrt / 60) == 0 ? "1" : ((int) item.Arrt / 60).ToString())
                        : "";
                    var resultTime = "";
                    switch (time)
                    {
                        case "1":
                            resultTime = "одну минуту";
                            break;
                        case "2":
                            resultTime = "две минуты";
                            break;
                        case "3":
                            resultTime = "три минуты";
                            break;
                        case "4":
                            resultTime = "четыре минуты";
                            break;
                        case "5":
                            resultTime = "пять минут";
                            break;
                        case "6":
                            resultTime = "шесть минут";
                            break;
                        case "7":
                            resultTime = "семь минут";
                            break;
                        case "8":
                            resultTime = "восемь минут";
                            break;
                        case "9":
                            resultTime = "девять минут";
                            break;
                        case "10":
                            resultTime = "десять минут";
                            break;
                        case "11":
                            resultTime = "одинадцать минут";
                            break;
                        case "12":
                            resultTime = "двенадцать минут";
                            break;
                        case "13":
                            resultTime = "тринадцать минут";
                            break;
                        case "14":
                            resultTime = "четырнадцать минут";
                            break;
                        case "15":
                            resultTime = "пятнадцать минут";
                            break;
                        case "16":
                            resultTime = "шестнадцать минут";
                            break;
                        case "17":
                            resultTime = "семнадцать минут";
                            break;
                        case "18":
                            resultTime = "восемнадцать минут";
                            break;
                        case "19":
                            resultTime = "девятнадцать минут";
                            break;
                        case "20":
                            resultTime = "двадцать минут";
                            break;
                    }

                    /*732*/
                    var charx = IsHaveCharacter(item.Rnum, out var str);
                    text += $"Через {resultTime}, ожидаем прибытие маршрута номер {str} {charx}.";
                    text += '\n';
                }
            }
            else
            {
                foreach (ForecastsItem item in predictNotObject)
                {
                    var time = item.arrTime != null
                        ? (((int)item.arrTime / 60) == 0 ? "1" : ((int)item.arrTime / 60).ToString())
                        : "";
                    var resultTime = "";
                    switch (time)
                    {
                        case "1":
                            resultTime = "одну минуту";
                            break;
                        case "2":
                            resultTime = "две минуты";
                            break;
                        case "3":
                            resultTime = "три минуты";
                            break;
                        case "4":
                            resultTime = "четыре минуты";
                            break;
                        case "5":
                            resultTime = "пять минут";
                            break;
                        case "6":
                            resultTime = "шесть минут";
                            break;
                        case "7":
                            resultTime = "семь минут";
                            break;
                        case "8":
                            resultTime = "восемь минут";
                            break;
                        case "9":
                            resultTime = "девять минут";
                            break;
                        case "10":
                            resultTime = "десять минут";
                            break;
                        case "11":
                            resultTime = "одинадцать минут";
                            break;
                        case "12":
                            resultTime = "двенадцать минут";
                            break;
                        case "13":
                            resultTime = "тринадцать минут";
                            break;
                        case "14":
                            resultTime = "четырнадцать минут";
                            break;
                        case "15":
                            resultTime = "пятнадцать минут";
                            break;
                        case "16":
                            resultTime = "шестнадцать минут";
                            break;
                        case "17":
                            resultTime = "семнадцать минут";
                            break;
                        case "18":
                            resultTime = "восемнадцать минут";
                            break;
                        case "19":
                            resultTime = "девятнадцать минут";
                            break;
                        case "20":
                            resultTime = "двадцать минут";
                            break;
                    }

                    /*732*/
                    var charx = IsHaveCharacter(item.num, out var str);
                    text += $"Через {resultTime}, ожидаем прибытие маршрута номер {str} {charx}.";
                    text += '\n';
                }
            }

            text = predictNotObject.Count > 0 
                 ? text + " ."
                 : text + "Информация о прибытии транспорта на текущий момент осутствует!";
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Api-Key " + iamKey);
            var values = new Dictionary<string, string>
            {
                {"text",text },
                {"lang","ru-RU" },
            };
            var content = new FormUrlEncodedContent(values);
            var response = await client.PostAsync("https://tts.api.cloud.yandex.net/speech/v1/tts:synthesize", content);
            var resultstream = (MemoryStream)await response.Content.ReadAsStreamAsync();
            return new FileStreamResult(resultstream, "application / ogg");
        }
    }
}