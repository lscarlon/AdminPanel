using AdminPanel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminPanel.Models
{
    public class ActionInfo
    {
        public string ActionName { get; set; }
        public string DisplayName { get; set; }
        public string DisplayImage { get; set; }
        public string ScriptAfterPartialView { get; set; }
        public TreeViewAttribute TreeViewSettings{ get; set; }
        public TreeViewSettingsAttribute TreeViewSettings2 { get; set; }
    }
}
