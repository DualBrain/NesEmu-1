using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NesEmu
{
    public static class Utility
    {
        public static Random rand = new Random((int)DateTime.Now.ToFileTime());

        public static int generateInt(int max, int min = 1)
        {
            return rand.Next(min, max);
        }
        public static long generateLong(long max, long min = 1)
        {
            return (long)(min + rand.NextDouble() * (max - min));
        }
    }
}
