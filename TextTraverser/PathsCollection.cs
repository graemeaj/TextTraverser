using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace TextTraverser
{
    public class PathsCollection : System.Configuration.ConfigurationElementCollection
    {
        public List<PathsElement> All { get { return this.Cast<PathsElement>().ToList(); } }

        public PathsElement this[int index]
        {
            get
            {
                return base.BaseGet(index) as PathsElement;
            }
            set
            {
                if (base.BaseGet(index) != null)
                {
                    base.BaseRemoveAt(index);
                }
                this.BaseAdd(index, value);
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new PathsElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((PathsElement)element).path;
        }
    }
}
