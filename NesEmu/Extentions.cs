using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NesEmu
{
    public static class Extentions
    {
        public static string ToHex(this int num)
        {
            return "0x" + num.ToString("X");
        }
        public static string ToHex(this long num)
        {
            return "0x" + num.ToString("X");
        }
    }
}
