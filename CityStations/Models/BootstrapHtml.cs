using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CityStations.Models
{
    public class BootstrapHtml
    {
        public static MvcHtmlString Dropdown(string id, string ajaxId, List<SelectListItem> selectListItems, string label,
                                             string urlAction, string controllerHeadObjectId, string cssClass,
                                             ViewContext viewContext, ViewDataDictionary dataDictionary)
        {
            var dropdown = new TagBuilder("a")
            {
                Attributes =
            {
                {"id", id},
                {"href","#"},
                {"data-toggle","dropdown"}
            }
            };

            dropdown.AddCssClass("dropdown-toggle");
            dropdown.AddCssClass("innerTextForDropdown");
            var idWrapper = id + "WrapperId";
            var wrapper = new TagBuilder("div")
            {
                Attributes =
            {
                {"id", idWrapper },
                { "style","border-color:#eee;"},
                {"onmouseover", String.Format("setCaretDropdown('{0}')",id)},
                {"onmouseout", String.Format("unsetCaretDropdown('{0}')",id)}
            }
            };
            if (selectListItems.Count > 0)
            {
                if (selectListItems.Count(s => s.Selected) == 0)
                {
                    SelectListItem first = null;
                    foreach (var item in selectListItems)
                    {
                        first = item;
                        break;
                    }
                    if (first != null) first.Selected = true;
                }
                dropdown.SetInnerText(selectListItems.FirstOrDefault(s => s.Selected)?.Text + " ");
            }

            wrapper.AddCssClass("dropdown");
            wrapper.AddCssClass("btn");
            wrapper.AddCssClass("btn-link");
            if (!string.IsNullOrEmpty(cssClass))
            {
                wrapper.AddCssClass(cssClass);
            }
            wrapper.AddCssClass("dropdown-wrapper");

            dropdown.InnerHtml += BuildCaret(id);
            wrapper.InnerHtml += dropdown;
            wrapper.InnerHtml += BuildDropdown(id, ajaxId, selectListItems, urlAction, controllerHeadObjectId,
                                               viewContext, dataDictionary);
            return new MvcHtmlString(wrapper.ToString());
        }

        private static string BuildCaret(string id)
        {
            var caret = new TagBuilder("i")
            {
                Attributes =
            {
                {"id", "chevronCaret"+id},
                {"aria-hidden","true"},
                {"style","margin-left:20px;"}
            }
            };
            caret.AddCssClass("fa fa-chevron-left");
            return caret.ToString();
        }

        private static string BuildDropdown(string id, string ajaxId, IEnumerable<SelectListItem> items, string urlAction,
                                            string controllerHeadObjectId, ViewContext viewContext,
                                            ViewDataDictionary dataDictionary)
        {
            var list = new TagBuilder("ul")
            {
                Attributes =
            {
                {"class", "dropdown-menu"},
                {"role", "menu"},
            }
            };

            list.AddCssClass("clearfix");
            list.AddCssClass("dropdown-wrapper");
            var listItem = new TagBuilder("li");
            listItem.Attributes.Add("role", "presentation");
            int iteration = 1;
            foreach (var item in items)
            {
                var formId = "form" + Guid.NewGuid() + iteration;
                list.InnerHtml += "<li role=\"presentation\">" + BuildListRow(id, ajaxId, formId, item, urlAction, controllerHeadObjectId,
                    viewContext, dataDictionary) + "</li>";
                iteration++;
            }
            return list.ToString();
        }

        private static string BuildListRow(string id, string ajaxId, string formId, SelectListItem item, string urlAction,
                                           string controllerHeadObjectId, ViewContext viewContext,
                                           ViewDataDictionary dataDictionary)
        {
            TagBuilder form = null;
            ViewDataContainer dataContainer = new ViewDataContainer(dataDictionary);
            HtmlHelper helper = new HtmlHelper(viewContext, dataContainer);
            var token = helper.AntiForgeryToken();
            TagBuilder anchor;
            if (string.IsNullOrEmpty(urlAction))
            {
                anchor = new TagBuilder("a")
                {
                    Attributes =
            {
                {"role", "menuitem"},
                {"tabindex", "-1"},
                {"href", "#"},
                {"onclick",id+"_onSelect("+item.Value+")"},
                {"id",id+"MenuItem"+item.Value}
            }
                };
            }
            else
            {
                if (ajaxId == null)
                {
                    form = new TagBuilder("form")
                    {
                        Attributes =
                    {
                        {"id",formId},
                        {"action", urlAction},
                        {"method","post"},
                    }
                    };
                }
                else
                {
                    form = new TagBuilder("form")
                    {
                        Attributes =
                    {
                        {"id",formId},
                        {"action", urlAction},
                        {"data-ajax","true"},
                        {"data-ajax-method","POST"},
                        {"data-ajax-mode","replace"},
                        {"data-ajax-update",ajaxId},
                        {"method","post"},
                    }
                    };
                }
                form.InnerHtml += token;
                var parametr = item.Value + ";;" + (controllerHeadObjectId ?? "");
                TagBuilder input = new TagBuilder("input")
                {
                    Attributes =
                    {
                        {"type","text"},
                        {"value",parametr},
                        {"name","parametr"},
                        {"hidden","hidden"}
                    }
                };
                form.InnerHtml += input;
                anchor = new TagBuilder("button")
                {
                    Attributes =
            {
                {"role", "menuitem"},
                {"tabindex", "-1"},
                {"type", "submit"},
                {"style", "min-width:130px"},
                {"class","btn btn-link dropDownMenuItems"}
            }
                };
            }
            anchor.SetInnerText(item.Text);
            if (form != null)
            {
                form.InnerHtml += anchor;
                return form.ToString();
            }
            else
            {
                return anchor.ToString();
            }
        }
    }
}