using CityStations.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CityStations.Controllers
{
    public class AdministratorController : Controller
    {
        
        public PartialViewResult CreateNewCity(CreateNewCityViewModel model)
        {
            if(model == null)
            {
                ViewBag.Error = "Ошибка добавления нового города! Не заполнены обязательные поля ввода!";
                return PartialView();
            }
            var x = model;
            return PartialView();
        }
    }
}