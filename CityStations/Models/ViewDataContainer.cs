using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CityStations.Models
{
    public class ViewDataContainer : IViewDataContainer
    {
        public ViewDataDictionary ViewData { get; set; }
        public ViewDataContainer(ViewDataDictionary viewData)
        {
            ViewData = new ViewDataDictionary(viewData);
        }
    }
}