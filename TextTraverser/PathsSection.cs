using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace TextTraverser
{
    public interface PathsSection
    {
        string Greeting { get; set; }
    }

    public class PathsElement : System.Configuration.ConfigurationElement
    {
        [ConfigurationProperty("path", IsRequired = true)]
        public string path
        {
            get { return (string)this["path"]; }
            set { this["path"] = value; }
        }
    }
}
