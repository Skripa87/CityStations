using CityStations.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace CityStations
{
    public class ContextManager
    {
        private CityStationsDbContext db;
            //this test
        public ContextManager()
        {
            try
            {
                db = new CityStationsDbContext();
            }
            catch (Exception e)
            {
                Logger.WriteLog(
                    $"Произошла ошибка {e.Message}, внутреннее исключение {e.InnerException}, источник ошибки {e.Source}, подробности {e.StackTrace}",
                    "ContextManager");
            }
        }

        //public List<AspNetUser> GetUsers() 
        //{
        //    return db.AspNetUsers
        //             .ToList();
        //} 

        public List<Event> GetEvents()
        {
            return db.Events
                     .ToList();
        }

        public StationModel ChangePassword(StationModel station, string password)
        {
            if (station == null) return null;
            var informationTable = db.InformationTables
                                     .Any(i => string.Equals(i.Id, station.InformationTable.Id))
                                 ? db.InformationTables
                                     .FirstOrDefault(i=>string.Equals(i.Id,station.InformationTable.Id))
                                 :null;
            if (informationTable == null) return null;
            informationTable.PasswordDevice = password;
            try
            {
                db.SaveChanges();
                return db.Stations.FirstOrDefault(s => string.Equals(s.Id, station.Id));
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                Exception raise = dbEx;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        string message = $"{validationErrors.Entry.Entity}:{validationError.ErrorMessage}";
                        // raise a new exception nesting
                        // the current instance as InnerException
                        raise = new InvalidOperationException(message, raise);
                    }
                }
            }
            return null;
        }

        public string CreateEvent(string msg, string initiator)
        {
            if (string.IsNullOrEmpty(msg))
            {
                return null;
            }
            if (string.IsNullOrEmpty(initiator))
            {
                initiator = "-9999";
            }
            var eventDb = new Event(msg, initiator);
            db.Events
              .Add(eventDb);
            try
            {
                db.SaveChanges();
                return eventDb.Id;
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                Exception raise = dbEx;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        string message = $"{validationErrors.Entry.Entity}:{validationError.ErrorMessage}";
                        // raise a new exception nesting
                        // the current instance as InnerException
                        raise = new InvalidOperationException(message, raise);
                    }
                }
            }
            return null;
        }

        public List<Event> GetEventFromDate(DateTime? dateStart, DateTime? dateEnd, EventType? eventType)
        {
            var events = db.Events
                           .ToList();
            return dateStart == null && dateEnd == null && eventType != null
                ? events.FindAll(e => e.EventType == eventType)
                : (dateStart != null && dateEnd == null && eventType != null
                  ? events.FindAll(e => e.EventType == eventType && e.Date > (DateTime)dateStart)
                  : (dateStart != null && dateEnd != null && eventType != null
                    ? events.FindAll(e => e.Date > (DateTime)dateStart && e.Date < (DateTime)dateEnd && e.EventType == eventType)
                    : events.FindAll(e => dateEnd != null && (dateStart != null && (e.Date > (DateTime)dateStart && e.Date < (DateTime)dateEnd)))));
        }

        public List<InformationTable> GetInformationTables()
        {
            return db.InformationTables.ToList();
        }

        public InformationTable GetInformationTable(string informationTableId)
        {
            return db.InformationTables
                     .Any(i => string.Equals(i.Id, informationTableId))
                  ? db.InformationTables
                      .FirstOrDefault(i => string.Equals(i.Id, informationTableId))
                  : null;
        }

        public void CheckAllAccessCodeInInformationTable()
        {
            foreach (var item in db.InformationTables)
            {
                item.CheckAccessCode();
            }
            try
            {
                db.SaveChanges();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                Exception raise = dbEx;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        string message = $"{validationErrors.Entry.Entity}:{validationError.ErrorMessage}";
                        // raise a new exception nesting
                        // the current instance as InnerException
                        raise = new InvalidOperationException(message, raise);
                    }
                }
            }
        }

        public string SetAccessCode(string stationId)
        {
            var station = db.Stations.Any(s => s.Id == stationId)
                        ? db.Stations.FirstOrDefault(s => s.Id == stationId)
                        : null;
            if (station?.InformationTable == null) return null;
            var informationTable = db.InformationTables
                                     .Any(i => string.Equals(i.Id, station.InformationTable.Id))
                                 ? db.InformationTables
                                     .FirstOrDefault(i => string.Equals(i.Id, station.InformationTable.Id))
                                 : null;
            if (informationTable == null) return null;
            var accessCode = string.IsNullOrEmpty(informationTable.AccessCode)
                           ? Guid.NewGuid()
                                 .ToString()
                           : informationTable.AccessCode;
            if (string.IsNullOrEmpty(informationTable.AccessCode))
            {
                informationTable.AccessCode = accessCode;
                try
                {
                    db.SaveChanges();
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
                {
                    Exception raise = dbEx;
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            string message = $"{validationErrors.Entry.Entity}:{validationError.ErrorMessage}";
                            // raise a new exception nesting
                            // the current instance as InnerException
                            raise = new InvalidOperationException(message, raise);
                        }
                    }
                }
            }
            return accessCode;
        }

        public List<StationModel> GetStations()
        {
            return db.Stations.ToList();
        }

        public async Task<List<StationModel>> GetStationAsync()
        {
            return await db.Stations
                           .ToListAsync()
                           .ConfigureAwait(false);
        }

        public async Task<List<StationModel>> GetActivStationsAsync()
        {
            return await db.Stations
                           .Where(s => s.Active)
                           .ToListAsync()
                           .ConfigureAwait(false);
        }

        public List<StationModel> GetActivStations()
        {
            return db.Stations
                     .Where(s => s.Active)
                     .ToList();
        }

        public List<StationModel> FindStationOnNamePart(string finderString)
        {
            if (string.IsNullOrEmpty(finderString)) return db.Stations
                                                             .ToList();
            var normalFinderString = finderString.Trim(' ')
                                                 .ToUpperInvariant();
            return db.Stations
                     .Any(s => s.Name
                                          .Trim(' ')
                                          .ToUpperInvariant()
                                          .Contains(normalFinderString)
                                     || normalFinderString.Contains(s.Name
                                                          .Trim(' ')
                                                          .ToUpperInvariant()))
                ? db.Stations
                    .Where(s => s.Name
                                    .Trim(' ')
                                    .ToUpperInvariant()
                                    .Contains(normalFinderString)
                                || normalFinderString.Contains(s.Name
                                    .Trim(' ')
                                    .ToUpperInvariant())).ToList()
                : db.Stations
                    .Where(s => s.Name
                                    .Trim(' ')
                                    .ToUpperInvariant()
                                    .Contains(normalFinderString.Substring(0,normalFinderString.Length/2))
                                || normalFinderString.Contains(s.Name
                                    .Trim(' ')
                                    .ToUpperInvariant()))
                                    .ToList();

        }

        public async Task<List<StationModel>> FindActiveStationOnNamePartAsync(string finderString)
        {
            if (string.IsNullOrEmpty(finderString)) return db.Stations
                                                             .Any(s=>s.Active)
                                                   ? await db.Stations
                                                             .Where(s=>s.Active)
                                                             .ToListAsync()
                                                             .ConfigureAwait(true)
                                                   : null;
            var normalFinderString = finderString.Trim(' ')
                                                 .ToUpperInvariant();
            var stations = db.Stations
                             .Any(s=>s.Active)
                         ? await db.Stations
                                   .ToListAsync()
                                   .ConfigureAwait(true)
                         : null;
            return stations != null 
                 ?( stations.Any(s => s.Name
                                         .Trim(' ')
                                         .ToUpperInvariant()
                                         .Contains(normalFinderString)
                                     || normalFinderString.Contains(s.Name
                                         .Trim(' ')
                                         .ToUpperInvariant()))
                ? stations
                    .FindAll(s => s.Name
                                      .Trim(' ')
                                      .ToUpperInvariant()
                                      .Contains(normalFinderString)
                                  || normalFinderString.Contains(s.Name
                                      .Trim(' ')
                                      .ToUpperInvariant()))
                : stations.FindAll(
                    s => s.Name
                             .Trim(' ')
                             .ToUpperInvariant()
                             .Contains(normalFinderString.Substring(0, normalFinderString.Length / 2))
                         || normalFinderString.Contains(s.Name
                             .Trim(' ')
                             .ToUpperInvariant())))
                 : new List<StationModel>();
        }


        public async Task<List<StationModel>> FindStationOnNamePartAsync(string finderString)
        {
            if (string.IsNullOrEmpty(finderString)) return await db.Stations
                                                                   .ToListAsync()
                                                                   .ConfigureAwait(true);
            var normalFinderString = finderString.Trim(' ')
                .ToUpperInvariant();
            var stations = db.Stations.ToList();
            return stations
                .Any(s => s.Name
                              .Trim(' ')
                              .ToUpperInvariant()
                              .Contains(normalFinderString)
                          || normalFinderString.Contains(s.Name
                              .Trim(' ')
                              .ToUpperInvariant()))
                ? stations
                    .FindAll(s => s.Name
                                    .Trim(' ')
                                    .ToUpperInvariant()
                                    .Contains(normalFinderString)
                                || normalFinderString.Contains(s.Name
                                    .Trim(' ')
                                    .ToUpperInvariant()))
                : stations.FindAll(
                    s => s.Name
                                    .Trim(' ')
                                    .ToUpperInvariant()
                                    .Contains(normalFinderString.Substring(0, normalFinderString.Length / 2))
                                || normalFinderString.Contains(s.Name
                                    .Trim(' ')
                                    .ToUpperInvariant()));
        }

        public StationModel DeactivateInformationTable(string stationId)
        {
            var station = db.Stations.Any(s => string.Equals(s.Id, stationId))
                        ? db.Stations.FirstOrDefault(s => string.Equals(s.Id, stationId))
                        : null;
            if (station == null) return null;
            station.Active = false;
            try
            {
                db.SaveChanges();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                Exception raise = dbEx;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        string message = $"{validationErrors.Entry.Entity}:{validationError.ErrorMessage}";
                        raise = new InvalidOperationException(message, raise);
                    }
                }
            }
            return station;
        }

        public StationModel GetStation(string stationId)
        {
            var station = db.Stations.Any(s => s.Id == stationId)
                ? db.Stations.FirstOrDefault(s => s.Id == stationId)
                : null;
            if (stationId == "Error" && station == null)
            {
                station = new StationModel
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
                        AccessCode = "$olnechniKrug",
                        HeightWithModule = 2,
                        WidthWithModule = 4,
                        ModuleType = db.ModuleTypes.Find("Q4Y10V5H"),
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
                db.Stations.Add(station);
                try
                {
                    db.SaveChanges();
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
                {
                    Exception raise = dbEx;
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            string message = $"{validationErrors.Entry.Entity}:{validationError.ErrorMessage}";
                            raise = new InvalidOperationException(message, raise);
                        }
                    }
                }
            }
            return station;
        }

        public List<ModuleType> GetModuleTypes()
        {
            return db.ModuleTypes.ToList();
        }

        public List<Content> GetContents()
        {
            return db.Contents.ToList();
        }

        public List<Content> GetContents(string informationTableId)
        {
            return db.Contents
                     .Any(i=>i.InformationTables.Select(s=>s.Id)
                                                .Contains(informationTableId))
                   ? db.Contents
                       .Where(i => i.InformationTables.Select(s => s.Id)
                                                      .Contains(informationTableId))
                       .ToList()
                   : new List<Content>();
        }

        public Content GetContent(string contentId)
        {
            return db.Contents.Any(c => c.Id == contentId)
                 ? db.Contents.FirstOrDefault(c => c.Id == contentId)
                 : null;
        }

        public void ChangeContent(string contentId, Content newContent)
        {
            var content = db.Contents
                            .Any(c => string.Equals(c.Id, contentId))
                        ? db.Contents
                            .FirstOrDefault(c => string.Equals(c.Id, contentId))
                        : null;
            if (content == null) return;
            content.ContentType = newContent.ContentType;
            content.InnerContent = newContent.InnerContent;
            content.TimeOut = newContent.TimeOut;
            try
            {
                db.SaveChanges();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                Exception raise = dbEx;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        string message = $"{validationErrors.Entry.Entity}:{validationError.ErrorMessage}";
                        raise = new InvalidOperationException(message, raise);
                    }
                }
            }
        }

        public void RemoveContent(string stationId, string contentId)
        {
            var station = db.Stations.Any(s => string.Equals(s.Id, stationId))
                        ? db.Stations.FirstOrDefault(s => string.Equals(s.Id, stationId))
                        : null;
            if (station?.InformationTable == null)
            {
                return;
            }
            var content = db.Contents.Any(c => string.Equals(c.Id, contentId))
                        ? db.Contents.FirstOrDefault(c => string.Equals(c.Id, contentId))
                        : null;
            if (content == null)
            {
                return;
            }
            station.InformationTable
                   .Contents
                   .Remove(content);
            try
            {
                db.SaveChanges();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                Exception raise = dbEx;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        string message = $"{validationErrors.Entry.Entity}:{validationError.ErrorMessage}";
                        raise = new InvalidOperationException(message, raise);
                    }
                }
            }
        }

        public Content CreateContent(string stationId, Content newContent)
        {
            if (newContent == null) return null;
            var station = db.Stations
                            .Any(s => string.Equals(s.Id, stationId))
                        ? db.Stations
                            .FirstOrDefault(s => string.Equals(s.Id, stationId))
                        : null;
            if (station == null) return null;
            var informationTable = station.InformationTable;
            if (informationTable == null) return null;
            var content = new Content()
            {
                ContentType = newContent.ContentType,
                Id = newContent.Id ?? Guid.NewGuid()
                                          .ToString(),
                InnerContent = newContent.InnerContent ?? "",
                TimeOut = newContent.TimeOut
            };
            db.Contents.Add(content);
            try
            {
                db.SaveChanges();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                Exception raise = dbEx;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        string message = string.Format("{0}:{1}",
                            validationErrors.Entry
                                .Entity,
                            validationError.ErrorMessage);
                        raise = new InvalidOperationException(message, raise);
                    }
                }
            }
            var newDbContent = db.Contents.Any(c => string.Equals(c.Id, content.Id))
                           ? db.Contents.FirstOrDefault(c => string.Equals(c.Id, content.Id))
                           : null;
            if (newDbContent == null) return null;
            informationTable.Contents.Add(newDbContent);
            try
            {
                db.SaveChanges();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                Exception raise = dbEx;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        string message = $"{validationErrors.Entry.Entity}:{validationError.ErrorMessage}";
                        raise = new InvalidOperationException(message, raise);
                    }
                }
            }
            return newDbContent;
        }

        public void SetModuleType(string informationTableId, string moduleTypeId)
        {
            var informationTable = db.InformationTables
                                     .Any(i => string.Equals(i.Id, informationTableId))
                                 ? db.InformationTables
                                     .FirstOrDefault(i => string.Equals(i.Id, informationTableId))
                                 : null;
            var moduleType = db.ModuleTypes
                               .Any(m => string.Equals(m.Id, moduleTypeId))
                           ? db.ModuleTypes
                               .FirstOrDefault(m => string.Equals(m.Id, moduleTypeId))
                           : null;
            if (informationTable == null || moduleType == null) return;
            informationTable.ModuleType = moduleType;
            try
            {
                db.SaveChanges();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                Exception raise = dbEx;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        string message = $"{validationErrors.Entry.Entity}:{validationError.ErrorMessage}";
                        raise = new InvalidOperationException(message, raise);
                    }
                }
            }
        }

        public void SetIpAddressDevice(string stationId, string ipAddress)
        {
            if (string.IsNullOrEmpty(stationId)) return;
            var station = db.Stations.Any(s => string.Equals(s.Id, stationId))
                ? db.Stations.FirstOrDefault(s => string.Equals(s.Id, stationId))
                : null;
            if (station?.InformationTable == null) return;
            var informationTable = db.InformationTables
                                     .Any(i => string.Equals(i.Id, station.InformationTable.Id))
                                 ? db.InformationTables
                                     .FirstOrDefault(i => string.Equals(i.Id, station.InformationTable.Id))
                                 : null;
            if (informationTable == null) return;
            informationTable.IpDevice = string.IsNullOrEmpty(informationTable.IpDevice) || !string.Equals(informationTable.IpDevice,ipAddress) 
                             ? ipAddress
                             :informationTable.IpDevice;
            try
            {
                db.SaveChanges();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                Exception raise = dbEx;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        string message = $"{validationErrors.Entry.Entity}:{validationError.ErrorMessage}";
                        raise = new InvalidOperationException(message, raise);
                    }
                }
            }
        }

        public void ChangeInformationTable(string informationTableId, InformationTable newInformationTable)
        {
            var informationTable = db.InformationTables
                                     .Any(i => i.Id == informationTableId)
                                 ? db.InformationTables
                                     .FirstOrDefault(i => i.Id == informationTableId)
                                 : null;
            if (informationTable == null || newInformationTable == null) return;
            informationTable.HeightWithModule = newInformationTable.HeightWithModule;
            informationTable.RowCount = newInformationTable.RowCount;
            informationTable.WidthWithModule = newInformationTable.WidthWithModule;
            informationTable.IpDevice = newInformationTable.IpDevice;
            informationTable.PasswordDevice = newInformationTable.PasswordDevice;
            informationTable.UserNameDevice = newInformationTable.UserNameDevice;
            try
            {
                db.SaveChanges();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                Exception raise = dbEx;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        string message = $"{validationErrors.Entry.Entity}:{validationError.ErrorMessage}";
                        raise = new InvalidOperationException(message, raise);
                    }
                }
            }
        }

        public StationModel ActivateInformationTable(string stationId)
        {
            InformationTable defaultInformationTable;
            var defaultContent = new Content()
            {
                ContentType = ContentType.FORECAST,
                Id = Guid.NewGuid().ToString(),
                InnerContent = stationId,
                TimeOut = 15
            };
            var station = db.Stations
                            .Any(s => s.Id == stationId)
                        ? db.Stations
                            .FirstOrDefault(s => s.Id == stationId)
                        : null;
            var moduleType = db.ModuleTypes
                               .Any()
                           ? db.ModuleTypes
                               .FirstOrDefault()
                           : null;
            if (station == null || moduleType == null) return null;
            if (station.InformationTable == null)
            {
                defaultInformationTable = new InformationTable()
                {
                    Id = Guid.NewGuid()
                             .ToString(),
                    AccessCode = Guid.NewGuid()
                                     .ToString(),
                    ServiceType = 0,
                    PasswordDevice = "",
                    IpDevice = "",
                    UserNameDevice = "",
                    HeightWithModule = 2,
                    WidthWithModule = 4,
                    RowCount = 4
                };
                defaultInformationTable.Contents
                                       .Add(defaultContent);
                db.InformationTables.Add(defaultInformationTable);
                try
                {
                    db.SaveChanges();
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
                {
                    Exception raise = dbEx;
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            string message = $"{validationErrors.Entry.Entity}:{validationError.ErrorMessage}";
                            raise = new InvalidOperationException(message, raise);
                        }
                    }
                }
                var informationTable = db.InformationTables
                                         .Any(i => string.Equals(i.Id, defaultInformationTable.Id))
                                     ? db.InformationTables
                                         .FirstOrDefault(i => string.Equals(i.Id, defaultInformationTable.Id))
                                     : null;
                if (informationTable == null) return null;
                informationTable.ModuleType = moduleType;
                station.InformationTable = informationTable;
            }
            station.Active = true;
            try
            {
                db.SaveChanges();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                Exception raise = dbEx;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        string message = $"{validationErrors.Entry.Entity}:{validationError.ErrorMessage}";
                        raise = new InvalidOperationException(message, raise);
                    }
                }
            }
            return station;
        }

        public void RemoveOldEvents()
        {
            var oldEvents = db.Events
                              .Where(e => e.Date < DateTime.Today && e.EventType != EventType.ERROR)
                              .ToList();
            List<Event> partOldEvents;
            bool marker = false;
            while (true)
            {
                try
                {
                    partOldEvents = oldEvents.GetRange(0, 100000);
                    oldEvents.RemoveRange(0,100000);
                }
                catch (Exception)
                {
                    var count = oldEvents.Count == 0 
                              ? 0
                              : oldEvents.Count - 1;
                    partOldEvents = oldEvents.GetRange(0, count);
                    oldEvents.RemoveRange(0,count);
                    marker = true;
                }
                db.Events.RemoveRange(partOldEvents);
                try
                {
                    db.SaveChanges();
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
                {
                    Exception raise = dbEx;
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            string message = $"{validationErrors.Entry.Entity}:{validationError.ErrorMessage}";
                            raise = new InvalidOperationException(message, raise);
                        }
                    }
                }
                if (marker) break;
            }
        }

        public List<Event> GetActulEvents()
        {
            //var allEvents = GetEvents();
            //if (allEvents == null) return new List<Event>();
            //var currentDateTime = DateTime.Now;
            //var currentDateTimeBeforeFiveMinuts = currentDateTime.Subtract(TimeSpan.FromMinutes(5.0));
            //var actualEvents = 
            //    db.Events
            //      .Where(e => e.Date > currentDateTimeBeforeFiveMinuts && e.Date < currentDateTime);
            //return actualEvents.ToList();
            return db.Events
                     .Take(50000)
                     .ToList();
        }
        public List<Event> GetErrors()
        {
            return db.Events
                     .Take(50000)
                     .ToList()
                     .FindAll(e => e.EventType == EventType.ERROR);
        }

        public UserOption GetUserOptions(string userId)
        {
            return db.UserOptions
                     .Any(u => string.Equals(u.UserId, userId))
                   ? db.UserOptions
                       .FirstOrDefault(u => string.Equals(u.UserId, userId))
                   : null;
        }

        public UserOption CreateUserOption(string userId, bool onlyActiveStation, bool groupByState, string selectedSortParametr)
        {
            var userOption = new UserOption()
            {
                UserId = userId,
                OnlyActiveStations = onlyActiveStation,
                GroupByState = groupByState,
                SelectedSortParametrs = selectedSortParametr
            };
            db.UserOptions.Add(userOption);
            try
            {
                db.SaveChanges();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                Exception raise = dbEx;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        string message = $"{validationErrors.Entry.Entity}:{validationError.ErrorMessage}";
                        raise = new InvalidOperationException(message, raise);
                    }
                }
            }
            return userOption;
        }

        public void SaveUserOption(UserOption userOption)
        {
            if (userOption == null) return; 
            var userOptionDb = db.UserOptions
                                 .Any(u => string.Equals(userOption.UserId, u.UserId, new StringComparison()))
                             ? db.UserOptions
                                 .FirstOrDefault(u => string.Equals(userOption.UserId, u.UserId, new StringComparison()))
                             : null;
            if (userOptionDb == null) return;
            userOptionDb.GroupByState = userOption.GroupByState;
            userOptionDb.OnlyActiveStations = userOption.OnlyActiveStations;
            userOptionDb.SelectedSortParametrs = userOption.SelectedSortParametrs;
            try
            {
                db.SaveChanges();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                Exception raise = dbEx;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        string message = $"{validationErrors.Entry.Entity}:{validationError.ErrorMessage}";
                        raise = new InvalidOperationException(message, raise);
                    }
                }
            }
        }
    }
}