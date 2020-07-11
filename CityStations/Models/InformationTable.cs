using System;
using System.Collections.Generic;

namespace CityStations.Models
{
    public enum ServiceType{OLD, NEW }

    public class InformationTable
    {
        public string Id { get; set; }
        public virtual ModuleType ModuleType { get; set; }
        public int WidthWithModule { get; set; }
        public int HeightWithModule { get; set; }
        public int RowCount { get; set; }
        public string AccessCode { get; set; }
        public string IpDevice { get; set; }
        public string UserNameDevice { get; set; }
        public string PasswordDevice { get; set; }
        public ServiceType ServiceType { get; set; }
        public virtual List<Content> Contents { get; set; }

        public InformationTableViewModel GetViewModel(string stationId)
        {
            return new InformationTableViewModel(this, stationId, RowCount);
        }

        public void SetInformationTable(InformationTable informationTable)
        {
            Id = informationTable.Id ?? Guid.NewGuid()
                                            .ToString();
            WidthWithModule = informationTable.WidthWithModule;
            HeightWithModule = informationTable.HeightWithModule;
            RowCount = informationTable.RowCount;
            AccessCode = informationTable.AccessCode;
            IpDevice = informationTable.IpDevice;
            UserNameDevice = informationTable.UserNameDevice;
            PasswordDevice = informationTable.PasswordDevice;
        }

        public string CheckAccessCode()
        {
            if (string.IsNullOrEmpty(AccessCode))
            {
                AccessCode = Guid.NewGuid()
                                 .ToString();
                return AccessCode;
            }

            return AccessCode;
        }

        public InformationTable()
        {
            Contents = new List<Content>();
        }
    }
}