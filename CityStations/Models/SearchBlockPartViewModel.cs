using System.Collections.Generic;
using System.Web.Mvc;

namespace CityStations.Models
{
    public class SearchBlockPartViewModel
    {
        public bool OnlyActiveStations { get; set; }
        public bool GroupByState { get; set; }
        public SelectList SortVariableList { get; set; }
        public List<StationModel> StationModels { get;}

        public SearchBlockPartViewModel(bool onlyActiveStations, bool grouoByState, List<StationModel> stations, string selectedItem)
        {
            StationModels = new List<StationModel>();
            StationModels.AddRange(stations);
            //if (stations != null)
            //{
            //    foreach (var station in stations)
            //    {
            //        StationModels.Add(new StationModel(station));
            //    }
            //}
            var variants = new List<string> {"#", "Идентификатор", "Наименование", "Район", "Контент"};
            SortVariableList = new SelectList(variants,selectedItem);
            OnlyActiveStations = onlyActiveStations;
            GroupByState = grouoByState;
        }
        public SearchBlockPartViewModel() 
        { }
    }
}