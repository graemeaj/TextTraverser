using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Configuration;

namespace TextTraverser
{
    class Config
    {
        public string previousPath;
        public string secondPath;
        public string thirdPath;
        public string fourthPath;
        public string fifthPath;
        public string sixthPath;

        public int windowWidth;
        public int windowHeight;
        public int windowLocationX;
        public int windowLocationY;

        public Config()
        {

            var appSettings = ConfigurationManager.AppSettings;
            //System.Console.Write("\n" + appSettings[previousPath] + "\n");

            foreach (var key in appSettings.AllKeys)
            {
                Console.WriteLine("Key: {0} Value: {1}", key, appSettings[key]);
            }

            setSettings();
        }

        private void setSettings()
        {
            previousPath = ConfigurationManager.AppSettings["previousPath"];
            secondPath = ConfigurationManager.AppSettings["2ndPath"];
            thirdPath = ConfigurationManager.AppSettings["3rdPath"];
            fourthPath = ConfigurationManager.AppSettings["4thPath"];
            fifthPath = ConfigurationManager.AppSettings["5thPath"];
            sixthPath = ConfigurationManager.AppSettings["6thPath"];

            windowWidth = Convert.ToInt32(ConfigurationManager.AppSettings["windowWidth"]);
            windowHeight = Convert.ToInt32(ConfigurationManager.AppSettings["windowHeight"]);
            windowLocationX = Convert.ToInt32(ConfigurationManager.AppSettings["windowLocationX"]);
            windowLocationY = Convert.ToInt32(ConfigurationManager.AppSettings["windowLocationY"]);
    }

        public void changeSetting(string key, string value)
        {
            if (ConfigurationManager.AppSettings[key] != null)
            {
                ConfigurationManager.AppSettings[key] = value;
                System.Console.Write("\n" + "success! the key is " + ConfigurationManager.AppSettings[key] + "\n");
            }
            else
            {
                System.Console.Write("\n" + "error! no such key " + key + "\n");
            }
        }




    }
}
