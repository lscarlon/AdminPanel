using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;

namespace AdminPanel.Models
{
    public class Menu
    {
        public int MenuID { get; set; }

        public Menu Parent { get; set; }

        [Required]
        public int DisplayOrder { get; set; }

        [Required]
        [MaxLength(30)]
        public string DisplayName { get; set; }

        [MaxLength(30)]
        public string DisplayImage { get; set; }

        [MaxLength(30)]
        public string Controller { get; set; }

        [MaxLength(30)]
        public string Action { get; set; }

        public string QueryString { get; set; }

        public string Link { get; set; }

        public string Href
        {
            get
            {
                if ((this.Link!="") && !(this.Link is null)) return "/" + this.Link;

                if (!(this.Controller is null) && !(this.Action is null)) { 
                    string href = "/" + Controller + "/" + Action;
                    if ((this.QueryString != "") && !(this.QueryString is null)) href = href + "?" + this.QueryString;
                    return href;
                }

                return "/#";
            }
        }

        public string ScriptAfterPartialView
        {
            get
            {
                if (!(this.Controller is null) && !(this.Action is null)) { 
                return Assembly.GetExecutingAssembly().GetTypes()
                    .Where(type => typeof(Controller).IsAssignableFrom(type))
                    .Where(c => c.Name.Replace("Controller", "") == this.Controller)
                    .SelectMany(type => type.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public))
                    .Where(m => m.Name == this.Action)
                    .Select(c => c.CustomAttributes.First(ca => ca.AttributeType.Name == "ScriptAfterPartialViewAttribute"))
                    .FirstOrDefault().ConstructorArguments[0].Value.ToString();
                }
                else {
                    return null;
                }

            }
        }

        [NotMapped]
        public bool Active { get; set; }

    }
}
