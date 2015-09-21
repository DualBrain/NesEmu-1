using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NesEmu
{
    class CPU
    {
        #region Memory
        private RAM ram;
        #endregion Memory

        #region Registers
        private int a { get; set; }
        private int x { get; set; }
        private int y { get; set; }
        private int pc { get; set; }
        private int s { get; set; }
        private int p { get; set; }

        public void setPC(int value)
        {
            pc = value & 0xffff;
        }
        #endregion Registers

        #region StatusFlags
        private bool carry = false;
        private bool zero = false;
        private bool interruptDisable = true;
        private bool dec = false;
        private bool overflow = false;
        private bool negative = false;
        #endregion StatusFlags

        #region Clock
        private int cycles { get; set; }
        public int clock { get; set; }
        #endregion Clock


        #region OpcodeDef
        private Dictionary<string,string> _opcodes { get; set; }

        public Dictionary<string, string> Opcodes
        {
            get
            {
                if (_opcodes != null)
                    return _opcodes;
                var opcodes = new Dictionary<string, string>();

                for (int i = 0; i < 0x100; i++)
                    opcodes.Add(i.ToHex(), "");

                opcodes["0x00"] = "BRK";
                opcodes["0x01"] = "ORA $(d,x)";
                opcodes["0x02"] = "STP";
                opcodes["0x03"] = "SLO $(d,x)";
                opcodes["0x04"] = "NOP $(d)";
                opcodes["0x05"] = "ORA $(d)";
                opcodes["0x06"] = "ASL $(d)";
                opcodes["0x07"] = "SLO $(d)";
                opcodes["0x08"] = "PHP";
                opcodes["0x09"] = "ORA $(#i)";
                opcodes["0x0A"] = "ASL";
                opcodes["0x0B"] = "ANC $(#i)";
                opcodes["0x0C"] = "NOP $(a)";
                opcodes["0x0D"] = "ORA $(a)";
                opcodes["0x0E"] = "ASL $(a)";
                opcodes["0x0F"] = "SLO $(a)";
                opcodes["0x10"] = "BPL $(*+d)";
                opcodes["0x11"] = "ORA $(d,y)";
                opcodes["0x12"] = "STP";
                opcodes["0x13"] = "SLO $(d),y)";
                opcodes["0x14"] = "NOP $(d,x)";
                opcodes["0x15"] = "ORA $(d,x)";
                opcodes["0x16"] = "ASL $(d,x)";
                opcodes["0x17"] = "SLO $(d,x)";
                opcodes["0x18"] = "CLC";
                opcodes["0x19"] = "ORA $(a,y)";
                opcodes["0x1A"] = "NOP";
                opcodes["0x1B"] = "SLO $(a,y)";
                opcodes["0x1C"] = "NOP $(a,x)";
                opcodes["0x1D"] = "ORA $(a,x)";
                opcodes["0x1E"] = "ASL $(a,x)";
                opcodes["0x1F"] = "SLO $(a,x)";
                opcodes["0x20"] = "JSR $(a)";
                opcodes["0x21"] = "AND $(d,x)";
                opcodes["0x22"] = "STP";
                opcodes["0x23"] = "RLA $(d,x)";
                opcodes["0x24"] = "BIT $(d)";
                opcodes["0x25"] = "AND $(d)";
                opcodes["0x26"] = "ROL $(d)";
                opcodes["0x27"] = "RLA $(d)";
                opcodes["0x28"] = "PLP";
                opcodes["0x29"] = "AND $(#i)";
                opcodes["0x2A"] = "ROL";
                opcodes["0x2B"] = "ANC $(#i)";
                opcodes["0x2C"] = "BIT $(a)";
                opcodes["0x2D"] = "AND $(a)";
                opcodes["0x2E"] = "ROL $(a)";
                opcodes["0x2F"] = "RLA $(a)";
                opcodes["0x30"] = "BMI $(*+d)";
                opcodes["0x31"] = "AND $(d,y)";
                opcodes["0x32"] = "STP";
                opcodes["0x33"] = "RLA $(d,y)";
                opcodes["0x34"] = "NOP $(d,x)";
                opcodes["0x35"] = "AND $(d,x)";
                opcodes["0x36"] = "ROL $(d,x)";
                opcodes["0x37"] = "RLA $(d,x)";
                opcodes["0x38"] = "SEC";
                opcodes["0x39"] = "AND $(a,y)";
                opcodes["0x3A"] = "NOP";
                opcodes["0x3B"] = "RLA $(a,y)";
                opcodes["0x3C"] = "NOP $(a,x)";
                opcodes["0x3D"] = "AND $(a,x)";
                opcodes["0x3E"] = "ROL $(a,x)";
                opcodes["0x3F"] = "RLA $(a,x)";
                opcodes["0x40"] = "RTI";
                opcodes["0x41"] = "EOR $(d,x)";
                opcodes["0x42"] = "STP";
                opcodes["0x43"] = "SRE $(d,x)";
                opcodes["0x44"] = "NOP $(d)";
                opcodes["0x45"] = "EOR $(d)";
                opcodes["0x46"] = "LSR $(d)";
                opcodes["0x47"] = "SRE $(d)";
                opcodes["0x48"] = "PHA";
                opcodes["0x49"] = "EOR $(#i)";
                opcodes["0x4A"] = "LSR";
                opcodes["0x4B"] = "ALR $(#i)";
                opcodes["0x4C"] = "JMP $(a)";
                opcodes["0x4D"] = "EOR $(a)";
                opcodes["0x4E"] = "LSR $(a)";
                opcodes["0x4F"] = "SRE $(a)";
                opcodes["0x50"] = "BVC $(*+d)";
                opcodes["0x51"] = "EOR $(d,y)";
                opcodes["0x52"] = "STP";
                opcodes["0x53"] = "SRE $(d,y)";
                opcodes["0x54"] = "NOP $(d,x)";
                opcodes["0x55"] = "EOR $(d,x)";
                opcodes["0x56"] = "LSR $(d,x)";
                opcodes["0x57"] = "SRE $(d,x)";
                opcodes["0x58"] = "CLI";
                opcodes["0x59"] = "EOR $(a,y)";
                opcodes["0x5A"] = "NOP";
                opcodes["0x5B"] = "SRE $(a,y)";
                opcodes["0x5C"] = "NOP $(a,x)";
                opcodes["0x5D"] = "EOR $(a,x)";
                opcodes["0x5E"] = "LSR $(a,x)";
                opcodes["0x5F"] = "SRE $(a,x)";
                opcodes["0x60"] = "RTS";
                opcodes["0x61"] = "ADC $(d,x)";
                opcodes["0x62"] = "STP";
                opcodes["0x63"] = "RRA $(d,x)";
                opcodes["0x64"] = "NOP $(d)";
                opcodes["0x65"] = "ADC $(d)";
                opcodes["0x66"] = "ROR $(d)";
                opcodes["0x67"] = "RRA $(d)";
                opcodes["0x68"] = "PLA";
                opcodes["0x69"] = "ADC $(#i)";
                opcodes["0x6A"] = "ROR";
                opcodes["0x6B"] = "ARR $(#i)";
                opcodes["0x6C"] = "JMP $(a)";
                opcodes["0x6D"] = "ADC $(a)";
                opcodes["0x6E"] = "ROR $(a)";
                opcodes["0x6F"] = "RRA $(a)";
                opcodes["0x70"] = "BVS $(*+d)";
                opcodes["0x71"] = "ADC $(d),y)";
                opcodes["0x72"] = "STP";
                opcodes["0x73"] = "RRA $(d,y)";
                opcodes["0x74"] = "NOP $(d,x)";
                opcodes["0x75"] = "ADC $(d,x)";
                opcodes["0x76"] = "ROR $(d,x)";
                opcodes["0x77"] = "RRA $(d,x)";
                opcodes["0x78"] = "SEI";
                opcodes["0x79"] = "ADC $(a,y)";
                opcodes["0x7A"] = "NOP";
                opcodes["0x7B"] = "RRA $(a,y)";
                opcodes["0x7C"] = "NOP $(a,x)";
                opcodes["0x7D"] = "ADC $(a,x)";
                opcodes["0x7E"] = "ROR $(a,x)";
                opcodes["0x7F"] = "RRA $(a,x)";
                opcodes["0x80"] = "NOP $(#i)";
                opcodes["0x81"] = "STA $(d,x)";
                opcodes["0x82"] = "NOP $(#i)";
                opcodes["0x83"] = "SAX $(d,x)";
                opcodes["0x84"] = "STY $(d)";
                opcodes["0x85"] = "STA $(d)";
                opcodes["0x86"] = "STX $(d)";
                opcodes["0x87"] = "SAX $(d)";
                opcodes["0x88"] = "DEY";
                opcodes["0x89"] = "NOP $(#i)";
                opcodes["0x8A"] = "TXA";
                opcodes["0x8B"] = "XAA $(#i)";
                opcodes["0x8C"] = "STY $(a)";
                opcodes["0x8D"] = "STA $(a)";
                opcodes["0x8E"] = "STX $(a)";
                opcodes["0x8F"] = "SAX $(a)";
                opcodes["0x90"] = "BCC $(*+d)";
                opcodes["0x91"] = "STA $(d,y)";
                opcodes["0x92"] = "STP";
                opcodes["0x93"] = "AHX $(d,y)";
                opcodes["0x94"] = "STY $(d,x)";
                opcodes["0x95"] = "STA $(d,x)";
                opcodes["0x96"] = "STX $(d,y)";
                opcodes["0x97"] = "SAX $(d,y)";
                opcodes["0x98"] = "TYA";
                opcodes["0x99"] = "STA $(a,y)";
                opcodes["0x9A"] = "TXS";
                opcodes["0x9B"] = "TAS $(a,y)";
                opcodes["0x9C"] = "SHY $(a,x)";
                opcodes["0x9D"] = "STA $(a,x)";
                opcodes["0x9E"] = "SHX $(a,y)";
                opcodes["0x9F"] = "AHX $(a,y)";
                opcodes["0xA0"] = "LDY $(#i)";
                opcodes["0xA1"] = "LDA $(d,x)";
                opcodes["0xA2"] = "LDX $(#i)";
                opcodes["0xA3"] = "LAX $(d,x)";
                opcodes["0xA4"] = "LDY $(d)";
                opcodes["0xA5"] = "LDA $(d)";
                opcodes["0xA6"] = "LDX $(d)";
                opcodes["0xA7"] = "LAX $(d)";
                opcodes["0xA8"] = "TAY";
                opcodes["0xA9"] = "LDA $(#i)";
                opcodes["0xAA"] = "TAX";
                opcodes["0xAB"] = "LAX $(#i)";
                opcodes["0xAC"] = "LDY $(a)";
                opcodes["0xAD"] = "LDA $(a)";
                opcodes["0xAE"] = "LDX $(a)";
                opcodes["0xAF"] = "LAX $(a)";
                opcodes["0xB0"] = "BCS $(*+d)";
                opcodes["0xB1"] = "LDA $(d,y)";
                opcodes["0xB2"] = "STP";
                opcodes["0xB3"] = "LAX $(d,y)";
                opcodes["0xB4"] = "LDY $(d,x)";
                opcodes["0xB5"] = "LDA $(d,x)";
                opcodes["0xB6"] = "LDX $(d,y)";
                opcodes["0xB7"] = "LAX $(d,y)";
                opcodes["0xB8"] = "CLV";
                opcodes["0xB9"] = "LDA $(a,y)";
                opcodes["0xBA"] = "TSX";
                opcodes["0xBB"] = "LAS $(a,y)";
                opcodes["0xBC"] = "LDY $(a,x)";
                opcodes["0xBD"] = "LDA $(a,x)";
                opcodes["0xBE"] = "LDX $(a,y)";
                opcodes["0xBF"] = "LAX $(a,y)";
                opcodes["0xC0"] = "CPY $(#i)";
                opcodes["0xC1"] = "CMP $(d,x)";
                opcodes["0xC2"] = "NOP $(#i)";
                opcodes["0xC3"] = "DCP $(d,x)";
                opcodes["0xC4"] = "CPY $(d)";
                opcodes["0xC5"] = "CMP $(d)";
                opcodes["0xC6"] = "DEC $(d)";
                opcodes["0xC7"] = "DCP $(d)";
                opcodes["0xC8"] = "INY";
                opcodes["0xC9"] = "CMP $(#i)";
                opcodes["0xCA"] = "DEX";
                opcodes["0xCB"] = "AXS $(#i)";
                opcodes["0xCC"] = "CPY $(a)";
                opcodes["0xCD"] = "CMP $(a)";
                opcodes["0xCE"] = "DEC $(a)";
                opcodes["0xCF"] = "DCP $(a)";
                opcodes["0xD0"] = "BNE $(*+d)";
                opcodes["0xD1"] = "CMP $(d,y)";
                opcodes["0xD2"] = "STP";
                opcodes["0xD3"] = "DCP $(d,y)";
                opcodes["0xD4"] = "NOP $(d,x)";
                opcodes["0xD5"] = "CMP $(d,x)";
                opcodes["0xD6"] = "DEC $(d,x)";
                opcodes["0xD7"] = "DCP $(d,x)";
                opcodes["0xD8"] = "CLD";
                opcodes["0xD9"] = "CMP $(a,y)";
                opcodes["0xDA"] = "NOP";
                opcodes["0xDB"] = "DCP $(a,y)";
                opcodes["0xDC"] = "NOP $(a,x)";
                opcodes["0xDD"] = "CMP $(a,x)";
                opcodes["0xDE"] = "DEC $(a,x)";
                opcodes["0xDF"] = "DCP $(a,x)";
                opcodes["0xE0"] = "CPX $(#i)";
                opcodes["0xE1"] = "SBC $(d,x)";
                opcodes["0xE2"] = "NOP $(#i)";
                opcodes["0xE3"] = "ISC $(d,x)";
                opcodes["0xE4"] = "CPX $(d)";
                opcodes["0xE5"] = "SBC $(d)";
                opcodes["0xE6"] = "INC $(d)";
                opcodes["0xE7"] = "ISC $(d)";
                opcodes["0xE8"] = "INX";
                opcodes["0xE9"] = "SBC $(#i)";
                opcodes["0xEA"] = "NOP";
                opcodes["0xEB"] = "SBC $(#i)";
                opcodes["0xEC"] = "CPX $(a)";
                opcodes["0xED"] = "SBC $(a)";
                opcodes["0xEE"] = "INC $(a)";
                opcodes["0xEF"] = "ISC $(a)";
                opcodes["0xF0"] = "BEQ $(*+d)";
                opcodes["0xF1"] = "SBC $(d,y)";
                opcodes["0xF2"] = "STP";
                opcodes["0xF3"] = "ISC $(d,y)";
                opcodes["0xF4"] = "NOP $(d,x)";
                opcodes["0xF5"] = "SBC $(d,x)";
                opcodes["0xF6"] = "INC $(d,x)";
                opcodes["0xF7"] = "ISC $(d,x)";
                opcodes["0xF8"] = "SED";
                opcodes["0xF9"] = "SBC $(a,y)";
                opcodes["0xFA"] = "NOP";
                opcodes["0xFB"] = "ISC $(a,y)";
                opcodes["0xFC"] = "NOP $(a,x)";
                opcodes["0xFD"] = "SBC $(a,x)";
                opcodes["0xFE"] = "INC $(a,x)";
                opcodes["0xFF"] = "ISC $(a,x)";

                _opcodes = opcodes;
                return _opcodes;
            }
        }
        #endregion OpcodeDef

        public CPU(RAM _ram)
        {
            ram = _ram;
            init();
        }

        public void init()
        {
            for (int i = 0; i < 0x800; ++i)
                ram.write(i, 0xff);

            ram.write(0x0008, Utility.generateInt(257));
            ram.write(0x0009, Utility.generateInt(257));
            ram.write(0x000A, Utility.generateInt(257));
            ram.write(0x000F, Utility.generateInt(257));

            for (int i = 0x4000; i < 0x4018; ++i)
                ram.write(i, 0x00);

            a = 0;
            x  = 0;
            y  = 0;
            pc = 0;
            s  = 0;
            p = 0;
        }
    }
}
