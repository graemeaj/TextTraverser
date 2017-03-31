using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TextTraverser
{
    class TextManipulate
    {

        public static string CleanUpString(string path)//removes special characters in a string for easy reading
        {
            path = Regex.Replace(path, @"\t|\n|\r", "");//turns special characters into ""
            return path;//(path.Contains(" ")) ? "\"" + path + "\"" : path;
        }



    }
}
