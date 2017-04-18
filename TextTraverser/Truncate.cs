using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextTraverser
{
    public static class Truncate
    {

        public static string TruncateString(string target, int lengthFactor)//lengthFactor decide how much space to have on each side of the truncated sides. try 16 to start
        {
            if (target.Length / 2 <= lengthFactor && target != null)
            {
                return target;
            }
            else if (target != null)
            {
                var truncated = target.Substring(0, lengthFactor) + "..." + target.Substring((target.Length - lengthFactor), lengthFactor);
                return truncated;
            }
            else
            {
                return "error: truncation failed";
            }
        }
    }
}
