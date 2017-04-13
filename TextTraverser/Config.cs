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


        public Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

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
            previousPath = config.AppSettings.Settings["previousPath"].Value;
            secondPath = config.AppSettings.Settings["2ndPath"].Value;
            thirdPath = config.AppSettings.Settings["3rdPath"].Value;
            fourthPath = config.AppSettings.Settings["4thPath"].Value;
            fifthPath = config.AppSettings.Settings["5thPath"].Value;
            sixthPath = config.AppSettings.Settings["6thPath"].Value;

            windowWidth = Convert.ToInt32(config.AppSettings.Settings["windowWidth"].Value);
            windowHeight = Convert.ToInt32(config.AppSettings.Settings["windowHeight"].Value);
            windowLocationX = Convert.ToInt32(config.AppSettings.Settings["windowLocationX"].Value);
            windowLocationY = Convert.ToInt32(config.AppSettings.Settings["windowLocationY"].Value);
    }

        public void changeSetting(string key, string value, Configuration mainSettings)
        {
            if (mainSettings.AppSettings.Settings[key] != null)
            {
                mainSettings.AppSettings.Settings[key].Value = value;
                //System.Console.Write("\n" + "success! the size is " + config.AppSettings.Settings[key].Value + "\n");
                mainSettings.Save(ConfigurationSaveMode.Modified);
            }
            else
            {
                System.Console.Write("\n" + "error! no such key " + key + "\n");
            }
        }

        static public void UpdatePreviousPaths(string newPath, Configuration mainSettings)
        {
            mainSettings.AppSettings.Settings["6thPath"].Value = mainSettings.AppSettings.Settings["5thPath"].Value;
            mainSettings.AppSettings.Settings["5thPath"].Value = mainSettings.AppSettings.Settings["4thPath"].Value;
            mainSettings.AppSettings.Settings["4thPath"].Value = mainSettings.AppSettings.Settings["3rdPath"].Value;
            mainSettings.AppSettings.Settings["3rdPath"].Value = mainSettings.AppSettings.Settings["2ndPath"].Value;
            mainSettings.AppSettings.Settings["2ndPath"].Value = mainSettings.AppSettings.Settings["previousPath"].Value;
            mainSettings.AppSettings.Settings["previousPath"].Value = newPath;

            /*System.Console.Write("\n " + mainSettings.AppSettings.Settings["6thPath"].Value);
            System.Console.Write("\n " + mainSettings.AppSettings.Settings["5thPath"].Value);
            System.Console.Write("\n " + mainSettings.AppSettings.Settings["4thPath"].Value);
            System.Console.Write("\n " + mainSettings.AppSettings.Settings["3rdPath"].Value);
            System.Console.Write("\n " + mainSettings.AppSettings.Settings["2ndPath"].Value);
            System.Console.Write("\n " + mainSettings.AppSettings.Settings["previousPath"].Value + "\n");*/
            mainSettings.Save(ConfigurationSaveMode.Modified);
        }




    }
}
