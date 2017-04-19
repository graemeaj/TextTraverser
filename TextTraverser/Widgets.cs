using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace TextTraverser
{
    public class Widgets : System.Configuration.ConfigurationSection
    {
        public static Widgets Widget => ConfigurationManager.GetSection("widgets") as Widgets;

        [ConfigurationProperty("PathsWidget", IsRequired = true)]
        public PathsCollection PathsWidget
        {
            get { return (PathsCollection)this["PathsWidget"]; }
            set { this["PathsWidget"] = value; }
        }
    }
}
