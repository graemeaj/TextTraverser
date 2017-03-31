using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TextTraverser
{
    class Config
    {
        public string path;
        public Config()
        {
            //path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "\\settings.cfg");
            path = string.Concat(AppDomain.CurrentDomain.BaseDirectory, "\\TextTraverser.config");
            System.Console.Write("\n" + path + "\n" + AppDomain.CurrentDomain.BaseDirectory);
            if (File.Exists(path))
            {
                System.Console.Write("\nyes\n");
            }
            else
            {
                System.Console.Write("\nno\n");
            }
        }

    }
}
