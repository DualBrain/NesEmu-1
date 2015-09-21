using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NesEmu
{
    class RAM
    {
        private int[] w_ram = new int[2048];
        private Mapper mapper;
        private APU apu;
        private PPU ppu;

        public RAM(Mapper _mapper)
        {
            mapper = _mapper;
            w_ram = w_ram.Select(i => i = 0xff).ToArray();
        }

        public void write(int address, int value)
        {
            throw new NotImplementedException();
        }

        public void read()
        {

        }

    }
}
