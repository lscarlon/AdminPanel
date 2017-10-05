using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminPanel.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CommandNameAttribute : Attribute
    {
        public readonly string CommandName;
        public CommandNameAttribute(string Name) 
        {
            this.CommandName = Name;
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class DisplayActionMenuAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class TreeViewSettingsAttribute : Attribute
    {
        public readonly string[] TreeViewSettings;

        //https://msdn.microsoft.com/en-us/library/w5zay9db.aspx
        public TreeViewSettingsAttribute(params string[] treeviewsettings)
        {
            this.TreeViewSettings = treeviewsettings;
        }

    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class TreeViewAttribute : Attribute
    {
        public readonly string ElementType;
        public readonly string ElementClass;
        public readonly string ElementValue;

        public TreeViewAttribute(string elementType, string elementClass, string elementValue)
        {
            this.ElementType = elementType;
            this.ElementClass = elementClass;
            this.ElementValue = elementValue;
        }
    }

    [AttributeUsage(AttributeTargets.All)]
    public class ScriptAfterPartialViewAttribute : Attribute
    {
        public readonly string ScriptToRun;
        public ScriptAfterPartialViewAttribute(string scriptoRun)  // What script to run after loading a partial view
        {
            this.ScriptToRun = scriptoRun;
        }
    }

    [AttributeUsage(AttributeTargets.All)]
    public class DisplayOrderAttribute : Attribute
    {
        public readonly int DisplayOrder;
        public DisplayOrderAttribute(int order)  // order is a positional parameter
        {
            this.DisplayOrder = order;
        }
    }

    [AttributeUsage(AttributeTargets.All)]
    public class DisplayImageAttribute : Attribute
    {
        public readonly string DisplayImage;
        public DisplayImageAttribute(string image)  //image from http://fontawesome.io/icons/ to display
        {
            this.DisplayImage = image;
        }
    }


}
