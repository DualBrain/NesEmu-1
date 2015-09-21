using System;

namespace NesEmu
{
    class Program
    {
        static void Main(string[] args)
        {
            //Create new NES Unit
            NesUnit unit = new NesUnit();
            string ut = new Utility().test(256);
            Console.WriteLine(ut);
        }
    }
}
