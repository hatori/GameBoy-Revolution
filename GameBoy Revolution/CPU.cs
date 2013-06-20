using System.Runtime.InteropServices;
using System.Windows.Forms;
using System;

namespace GameBoy_Revolution
{
    public class CPU
    {
        #region Variables
        private Memory _memory;
        private Video _video;
        private Input _input;

        private byte[] cycles_lookup;
        private byte[] cycles_lookup_CB;
        private byte[] cycles_lookup_10;

        private string[] cycles_lookup_log1;
        private string[] cycles_lookup_CB_log1;
        private string[] cycles_lookup_10_log1;
        private uint[] cycles_lookup_log2;
        private uint[] cycles_lookup_CB_log2;
        private uint[] cycles_lookup_10_log2;
        private System.Text.StringBuilder output;
        public int writeCount;

        [StructLayout(LayoutKind.Explicit)]
        public struct Regs
        {
            [FieldOffset(0)]
            public byte F;

            [FieldOffset(1)]
            public byte A;

            [FieldOffset(2)]
            public byte C;

            [FieldOffset(3)]
            public byte B;

            [FieldOffset(4)]
            public byte E;

            [FieldOffset(5)]
            public byte D;

            [FieldOffset(6)]
            public byte L;

            [FieldOffset(7)]
            public byte H;

            [FieldOffset(0)]
            public ushort AF;

            [FieldOffset(2)]
            public ushort BC;

            [FieldOffset(4)]
            public ushort DE;

            [FieldOffset(6)]
            public ushort HL;
        }

        private Regs regs;
        private ushort pc;
        private ushort sp;

        private byte opcode;
        private byte next_opcode;
        private int cycles;
        private int cyclesThisOp;
        private int cyclesToDiv;
        private int cyclesToTima;
        private byte disable_int;
        private byte enable_int;
        private bool IME;
        private bool halted;
        private bool stopped;

        private int Scanline_Counter;
        private bool special_gfx;
        #endregion

        #region constructor
        public CPU(Form form1_reference, ref Memory memory, ref Video video/*, ref Sound sound*/, ref Input input)
        {
            _memory = memory;
            _video = video;
            _input = input;

            cycles = 0;
            cyclesThisOp = 0;
            cyclesToDiv = 0;
            cyclesToTima = 0;
            Scanline_Counter = 0;
            special_gfx = false;

            regs.A = regs.B = regs.C = regs.D = regs.E = regs.H = regs.L = 0;
            regs.F = 0;
            pc = 0;
            sp = 0;
            opcode = 0;
            next_opcode = 0;
            disable_int = 0;
            enable_int = 0;
            IME = false;
            halted = false;
            stopped = false;

            cycles_lookup = new byte[] {
	        4,12,8,8,4,4,8,4,20,8,8,8,4,4,8,4,
            0,12,8,8,4,4,8,4,12,8,8,8,4,4,8,4,
            8,12,8,8,4,4,8,4,8,8,8,8,4,4,8,4,
            8,12,8,8,12,12,12,4,8,8,8,8,4,4,8,4,
            4,4,4,4,4,4,8,4,4,4,4,4,4,4,8,4,
            4,4,4,4,4,4,8,4,4,4,4,4,4,4,8,4,
            4,4,4,4,4,4,8,4,4,4,4,4,4,4,8,4,
            8,8,8,8,8,8,4,8,4,4,4,4,4,4,8,4,
            4,4,4,4,4,4,8,4,4,4,4,4,4,4,8,4,
            4,4,4,4,4,4,8,4,4,4,4,4,4,4,8,4,
            4,4,4,4,4,4,8,4,4,4,4,4,4,4,8,4,
            4,4,4,4,4,4,8,4,4,4,4,4,4,4,8,4,
            8,12,12,16,12,16,8,16,8,16,12,0,12,24,8,16,
            8,12,12,0,12,16,8,16,8,16,12,0,12,0,8,16,
            12,12,8,0,0,16,8,16,16,4,16,0,0,0,8,16,
            12,12,8,4,0,16,8,16,12,8,16,4,0,0,8,16,
            };

            cycles_lookup_CB = new byte[] {
	        8,8,8,8,8,8,16,8,8,8,8,8,8,8,16,8,
            8,8,8,8,8,8,16,8,8,8,8,8,8,8,16,8,
            8,8,8,8,8,8,16,8,8,8,8,8,8,8,16,8,
            8,8,8,8,8,8,16,8,8,8,8,8,8,8,16,8,
            8,8,8,8,8,8,12,8,8,8,8,8,8,8,12,8,
            8,8,8,8,8,8,12,8,8,8,8,8,8,8,12,8,
            8,8,8,8,8,8,12,8,8,8,8,8,8,8,12,8,
            8,8,8,8,8,8,12,8,8,8,8,8,8,8,12,8,
            8,8,8,8,8,8,16,8,8,8,8,8,8,8,16,8,
            8,8,8,8,8,8,16,8,8,8,8,8,8,8,16,8,
            8,8,8,8,8,8,16,8,8,8,8,8,8,8,16,8,
            8,8,8,8,8,8,16,8,8,8,8,8,8,8,16,8,
            8,8,8,8,8,8,16,8,8,8,8,8,8,8,16,8,
            8,8,8,8,8,8,16,8,8,8,8,8,8,8,16,8,
            8,8,8,8,8,8,16,8,8,8,8,8,8,8,16,8,
            8,8,8,8,8,8,16,8,8,8,8,8,8,8,16,8,
            };

            cycles_lookup_10 = new byte[] { 4 };

            cycles_lookup_log1 = new string[] {
	        "0x00","0x01","0x02","0x03","0x04","0x05","0x06","0x07","0x08","0x09","0x0A","0x0B","0x0C","0x0D","0x0E","0x0F",
            "0x10","0x11","0x12","0x13","0x14","0x15","0x16","0x17","0x18","0x19","0x1A","0x1B","0x1C","0x1D","0x1E","0x1F",
            "0x20","0x21","0x22","0x23","0x24","0x25","0x26","0x27","0x28","0x29","0x2A","0x2B","0x2C","0x2D","0x2E","0x2F",
            "0x30","0x31","0x32","0x33","0x34","0x35","0x36","0x37","0x38","0x39","0x3A","0x3B","0x3C","0x3D","0x3E","0x3F",
            "0x40","0x41","0x42","0x43","0x44","0x45","0x46","0x47","0x48","0x49","0x4A","0x4B","0x4C","0x4D","0x4E","0x4F",
            "0x50","0x51","0x52","0x53","0x54","0x55","0x56","0x57","0x58","0x59","0x5A","0x5B","0x5C","0x5D","0x5E","0x5F",
            "0x60","0x61","0x62","0x63","0x64","0x65","0x66","0x67","0x68","0x69","0x6A","0x6B","0x6C","0x6D","0x6E","0x6F",
            "0x70","0x71","0x72","0x73","0x74","0x75","0x76","0x77","0x78","0x79","0x7A","0x7B","0x7C","0x7D","0x7E","0x7F",
            "0x80","0x81","0x82","0x83","0x84","0x85","0x86","0x87","0x88","0x89","0x8A","0x8B","0x8C","0x8D","0x8E","0x8F",
            "0x90","0x91","0x92","0x93","0x94","0x95","0x96","0x97","0x98","0x99","0x9A","0x9B","0x9C","0x9D","0x9E","0x9F",
            "0xA0","0xA1","0xA2","0xA3","0xA4","0xA5","0xA6","0xA7","0xA8","0xA9","0xAA","0xAB","0xAC","0xAD","0xAE","0xAF",
            "0xB0","0xB1","0xB2","0xB3","0xB4","0xB5","0xB6","0xB7","0xB8","0xB9","0xBA","0xBB","0xBC","0xBD","0xBE","0xBF",
            "0xC0","0xC1","0xC2","0xC3","0xC4","0xC5","0xC6","0xC7","0xC8","0xC9","0xCA","0xCB","0xCC","0xCD","0xCE","0xCF",
            "0xD0","0xD1","0xD2","0xD3","0xD4","0xD5","0xD6","0xD7","0xD8","0xD9","0xDA","0xDB","0xDC","0xDD","0xDE","0xDF",
            "0xE0","0xE1","0xE2","0xE3","0xE4","0xE5","0xE6","0xE7","0xE8","0xE9","0xEA","0xEB","0xEC","0xED","0xEE","0xEF",
            "0xF0","0xF1","0xF2","0xF3","0xF4","0xF5","0xF6","0xF7","0xF8","0xF9","0xFA","0xFB","0xFC","0xFD","0xFE","0xFF",
            };

            cycles_lookup_CB_log1 = new string[] {
	        "0x00","0x01","0x02","0x03","0x04","0x05","0x06","0x07","0x08","0x09","0x0A","0x0B","0x0C","0x0D","0x0E","0x0F",
            "0x10","0x11","0x12","0x13","0x14","0x15","0x16","0x17","0x18","0x19","0x1A","0x1B","0x1C","0x1D","0x1E","0x1F",
            "0x20","0x21","0x22","0x23","0x24","0x25","0x26","0x27","0x28","0x29","0x2A","0x2B","0x2C","0x2D","0x2E","0x2F",
            "0x30","0x31","0x32","0x33","0x34","0x35","0x36","0x37","0x38","0x39","0x3A","0x3B","0x3C","0x3D","0x3E","0x3F",
            "0x40","0x41","0x42","0x43","0x44","0x45","0x46","0x47","0x48","0x49","0x4A","0x4B","0x4C","0x4D","0x4E","0x4F",
            "0x50","0x51","0x52","0x53","0x54","0x55","0x56","0x57","0x58","0x59","0x5A","0x5B","0x5C","0x5D","0x5E","0x5F",
            "0x60","0x61","0x62","0x63","0x64","0x65","0x66","0x67","0x68","0x69","0x6A","0x6B","0x6C","0x6D","0x6E","0x6F",
            "0x70","0x71","0x72","0x73","0x74","0x75","0x76","0x77","0x78","0x79","0x7A","0x7B","0x7C","0x7D","0x7E","0x7F",
            "0x80","0x81","0x82","0x83","0x84","0x85","0x86","0x87","0x88","0x89","0x8A","0x8B","0x8C","0x8D","0x8E","0x8F",
            "0x90","0x91","0x92","0x93","0x94","0x95","0x96","0x97","0x98","0x99","0x9A","0x9B","0x9C","0x9D","0x9E","0x9F",
            "0xA0","0xA1","0xA2","0xA3","0xA4","0xA5","0xA6","0xA7","0xA8","0xA9","0xAA","0xAB","0xAC","0xAD","0xAE","0xAF",
            "0xB0","0xB1","0xB2","0xB3","0xB4","0xB5","0xB6","0xB7","0xB8","0xB9","0xBA","0xBB","0xBC","0xBD","0xBE","0xBF",
            "0xC0","0xC1","0xC2","0xC3","0xC4","0xC5","0xC6","0xC7","0xC8","0xC9","0xCA","0xCB","0xCC","0xCD","0xCE","0xCF",
            "0xD0","0xD1","0xD2","0xD3","0xD4","0xD5","0xD6","0xD7","0xD8","0xD9","0xDA","0xDB","0xDC","0xDD","0xDE","0xDF",
            "0xE0","0xE1","0xE2","0xE3","0xE4","0xE5","0xE6","0xE7","0xE8","0xE9","0xEA","0xEB","0xEC","0xED","0xEE","0xEF",
            "0xF0","0xF1","0xF2","0xF3","0xF4","0xF5","0xF6","0xF7","0xF8","0xF9","0xFA","0xFB","0xFC","0xFD","0xFE","0xFF",
            };

            cycles_lookup_10_log1 = new string[] { "0x00" };

            cycles_lookup_log2 = new uint[] {
	        0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
	        0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
            0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
	        0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
            0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
	        0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
            0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
	        0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
            0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
	        0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
            0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
	        0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
            0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
	        0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
            0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
	        0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
            };

            cycles_lookup_CB_log2 = new uint[] {
	        0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
	        0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
            0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
	        0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
            0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
	        0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
            0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
	        0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
            0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
	        0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
            0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
	        0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
            0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
	        0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
            0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
	        0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
            };

            cycles_lookup_10_log2 = new uint[] { 0 };

            output = new System.Text.StringBuilder(2048000);
            writeCount = 0;

            Startup();
        }
        #endregion

        #region retrieve IME
        public bool retrieve_IME
        {
            get
            {
                return IME;
            }
        }
        #endregion

        #region retrieve cycles
        public int retrieve_Cycles
        {
            get
            {
                return cycles;
            }
        }
        #endregion

        #region retrieve opcode
        public byte retrieve_Opcode
        {
            get
            {
                return opcode;
            }
        }
        #endregion

        #region retrieve cpu status
        public Regs retrieve_cpu_status
        {
            get
            {
                return regs;
            }
        }
        #endregion

        #region retrieve pc
        public ushort retrieve_pc
        {
            get
            {
                return pc;
            }
        }
        #endregion

        #region retrieve sp
        public ushort retrieve_sp
        {
            get
            {
                return sp;
            }
        }
        #endregion

        #region retrieve memory
        public Memory retrieve_memory
        {
            get
            {
                return _memory;
            }
        }
        #endregion

        #region retrieve next opcode
        public byte retrieve_next_opcode
        {
            get
            {
                return next_opcode;
            }
        }
        #endregion

        #region Startup
        private void Startup()
        {
            regs.A = 0x01;
            regs.F = 0xB0;
            regs.BC = 0x0013;
            regs.DE = 0x00D8;
            regs.HL = 0x014D;
            sp = 0xFFFE;
            pc = 0x0100;
            _memory.Retrieve_TIMA = 0x00;
            _memory.Retrieve_TMA = 0x00;
            _memory.Retrieve_TAC = 0x00;
            _memory.Retrieve_NR10 = 0x80;
            _memory.Retrieve_NR11 = 0xBF;
            _memory.Retrieve_NR12 = 0xF3;
            _memory.Retrieve_NR14 = 0xBF;
            _memory.Retrieve_NR21 = 0x3F;
            _memory.Retrieve_NR22 = 0x00;
            _memory.Retrieve_NR24 = 0xBF;
            _memory.Retrieve_NR30 = 0x7F;
            _memory.Retrieve_NR31 = 0xFF;
            _memory.Retrieve_NR32 = 0x9F;
            _memory.Retrieve_NR33 = 0xBF;
            _memory.Retrieve_NR41 = 0xFF;
            _memory.Retrieve_NR42 = 0x00;
            _memory.Retrieve_NR43 = 0x00;
            _memory.Retrieve_NR44 = 0xBF;
            _memory.Retrieve_NR50 = 0x77;
            _memory.Retrieve_NR51 = 0xF3;
            _memory.Retrieve_NR52 = 0xF1;
            _memory.Retrieve_LCDC = 0x91;
            _memory.Retrieve_SCY = 0x00;
            _memory.Retrieve_SCX = 0x00;
            _memory.Retrieve_LYC = 0x00;
            _memory.Retrieve_BGP = 0xFC;
            _memory.Retrieve_OBP0 = 0xFF;
            _memory.Retrieve_OBP1 = 0xFF;
            _memory.Retrieve_WY = 0x00;
            _memory.Retrieve_WX = 0x00;
            _memory.Retrieve_IE = 0x00;
            next_opcode = _memory.read_byte(pc);
        }
        #endregion

        #region Request Interrupt
        private void request_interrupts(int interrupt)
        {
            byte IF = _memory.Retrieve_IF;

            if ((IF & (0x1 << interrupt)) == 0x0)
            {
                IF = (byte)(IF ^ (0x1 << interrupt));
            }

            _memory.Retrieve_IF = IF;
        }
        #endregion

        #region Handle Interrupt
        private void handle_interrupts(int interrupt)
        {
            IME = false;
            halted = false;
            _memory.write_byte((ushort)(sp - 2), (byte)(pc & 0xFF));
            _memory.write_byte((ushort)(sp - 1), (byte)(pc >> 0x8 & 0xFF));
            sp -= 2;

            byte IF = _memory.Retrieve_IF;
            IF ^= (byte)(0x1 << interrupt);
            _memory.Retrieve_IF = IF;

            switch (interrupt)
            {
                case 0x0:
                    {
                        pc = 0x40;
                        break;
                    }
                case 0x1:
                    {
                        pc = 0x48;
                        break;
                    }
                case 0x2:
                    {
                        pc = 0x50;
                        break;
                    }
                case 0x3:
                    {
                        pc = 0x58;
                        break;
                    }
                case 0x4:
                    {
                        stopped = false;
                        pc = 0x60;
                        break;
                    }
            }
        }
        #endregion

        #region Process Interrupts
        private void process_interrupts()
        {
            byte IF = _memory.Retrieve_IF;
            byte IE = _memory.Retrieve_IE;

            if (IME == true)
            {
                for (int i = 0; i < 5; i++)
                {
                    byte current_if = (byte)(IF & (0x1 << i));
                    byte current_ie = (byte)(IE & (0x1 << i));

                    if (current_ie != 0x0 && current_if != 0x0)
                    {
                        handle_interrupts(i);
                    }
                }
            }
        }
        #endregion

        #region Process Timers
        private void process_timers(int cycles)
        {
            int divCyles = 256;
            int timaCycles = load_timer();
            byte timer_control = _memory.Retrieve_TAC;

            cyclesToDiv -= cycles;

            if (cyclesToDiv <= 0)
            {
                int remainder = cyclesToDiv;
                cyclesToDiv = divCyles + remainder;
                _memory.Retrieve_DIV++;
            }

            if ((timer_control & 0x4) != 0x0)
            {
                cyclesToTima -= cycles;

                if (cyclesToTima <= 0)
                {
                    int remainder = cyclesToTima;
                    cyclesToTima = timaCycles + remainder;
                    if (_memory.Retrieve_TIMA != 0xFF)
                    {
                        _memory.Retrieve_TIMA++;
                    }
                    else
                    {
                        _memory.Retrieve_TIMA++;
                        request_interrupts(2);
                        _memory.Retrieve_TIMA = _memory.Retrieve_TMA;
                    }
                }
            }
        }
        #endregion

        #region Process Input
        private void processInput()
        {
            _input.Key_Pressed = _input.retrieve_keyboardstate();
            for (int i = 0; i < 8; i++)
            {
                if (_input.Key_Pressed.IsPressed(_input.Keys[i]))
                {
                    _memory.Retrieve_KeyBits ^= (byte)(1 << i);
                    request_interrupts(4);
                }
            }
            _memory.Retrieve_KeyBits = _memory.Retrieve_KeyBits;
        }
        #endregion

        #region ResetInput
        private void resetInput()
        {
            _memory.Retrieve_KeyBits = 0xFF;
        }
        #endregion

        #region Load Timer
        private int load_timer()
        {
            byte current_timer = _memory.Retrieve_TAC;

            switch (current_timer & 0x3)
            {
                case 0x0:
                    {
                        return 1024;
                    }
                case 0x1:
                    {
                        return 16;
                    }
                case 0x2:
                    {
                        return 64;
                    }
                case 0x3:
                    {
                        return 256;
                    }
                default:
                    {
                        return 0x0;
                    }
            }
        }
        #endregion

        #region process graphics
        private void process_graphics(int cycles)
        {
            byte currentline = 0;

            if (get_lcd_status())
            {
                Scanline_Counter += cycles;
            }
            else
            {
                return;
            }

            update_lcd_status();

            if (Scanline_Counter >= 456 || (Scanline_Counter + 4 >= 456))
            {
                if (Scanline_Counter >= 456)
                {
                    Scanline_Counter -= 456;
                    if (special_gfx != true)
                    {
                        _memory.Retrieve_LY++;
                    }
                    special_gfx = false;
                }
                else if (Scanline_Counter < 456)
                {
                    special_gfx = true;
                    _memory.Retrieve_LY++;
                }

                currentline = _memory.Retrieve_LY;

                if (currentline == 144)
                {
                    request_interrupts(0);
                }
                else if (currentline > 153)
                {
                    _memory.Retrieve_LY = 0;
                }
                else if (currentline < 144)
                {
                    Draw_ScanLine();
                }
            }
        }
        #endregion

        #region draw scanline
        void Draw_ScanLine()
        {
            if ((_memory.Retrieve_LCDC & 0x1) != 0)
            {
                Render_Background();
            }

            if ((_memory.Retrieve_LCDC & 0x2) != 0)
            {
                Render_Sprites();
            }
        }
        #endregion

        #region render background
        void Render_Background()
        {
            ushort Tile_Location;
            ushort Tile_ID_Location;
            bool using_window = false;
            bool signed = false;
            byte Scroll_X = _memory.Retrieve_SCX;
            byte Scroll_Y = _memory.Retrieve_SCY;
            byte window_x = (byte)(_memory.Retrieve_WX - 7);
            byte window_y = _memory.Retrieve_WY;
            byte lcdc_status = _memory.Retrieve_LCDC;
            byte currentline = _memory.Retrieve_LY;

            if ((lcdc_status & 0x10) != 0)
            {
                Tile_Location = 0x8000;
            }
            else
            {
                Tile_Location = 0x8800;
                signed = true;
            }

            if ((lcdc_status >> 5 & 0x1) != 0)
            {
                if (window_y <= currentline)
                {
                    using_window = true;
                }
            }

            if (using_window)
            {
                if ((lcdc_status & 0x40) != 0)
                {
                    Tile_ID_Location = 0x9C00;
                }
                else
                {
                    Tile_ID_Location = 0x9800;
                }
            }
            else
            {
                if ((lcdc_status & 0x8) != 0)
                {
                    Tile_ID_Location = 0x9C00;
                }
                else
                {
                    Tile_ID_Location = 0x9800;
                }
            }

            byte ypos = 0;

            if (using_window)
            {
                ypos = (byte)(currentline - window_y);
            }
            else
            {
                ypos = (byte)(Scroll_Y + currentline);
            }

            ushort tilerow_location = (ushort)((ypos / 8) * 32);

            for (int i = 0; i < 160; i++)
            {
                if (currentline == 0)
                {
                    currentline = currentline;
                }

                byte xpos = (byte)(i + Scroll_X);

                if (using_window)
                {
                    if (xpos >= window_x)
                    {
                        xpos = (byte)(i - window_x);
                    }
                }

                ushort tilecol_location = (ushort)(xpos / 8);

                byte tilenum = 0;
                sbyte signed_tilenum = 0;

                ushort tile_addres = (ushort)(Tile_ID_Location + tilerow_location + tilecol_location);

                if (!signed)
                {
                    tilenum = _memory.read_byte(tile_addres);
                }
                else
                {
                    signed_tilenum = (sbyte)_memory.read_byte(tile_addres);
                }

                ushort tile_address = Tile_Location;

                if (!signed)
                {
                    tile_address += (ushort)(tilenum * 16);
                }
                else
                {
                    tile_address = (ushort)(0x9000 + (signed_tilenum * 16));
                }

                byte cur_line = (byte)(ypos % 8);
                cur_line *= 2;
                byte data1 = _memory.read_byte((ushort)(tile_address + cur_line));
                byte data2 = _memory.read_byte((ushort)(tile_address + cur_line + 1));

                short cur_pixel = (short)(xpos % 8);
                cur_pixel -= 7;
                cur_pixel *= -1;

                byte color = (byte)((((data2 >> cur_pixel) & 0x1) << 1) | ((data1 >> cur_pixel) & 0x1));
                byte true_color = get_color(color);

                switch (true_color)
                {
                    case 0:
                        {
                            _video.set_pixel(i, currentline, (int)_video.Colors[0]);
                            break;
                        }
                    case 1:
                        {
                            _video.set_pixel(i, currentline, (int)_video.Colors[1]);
                            break;
                        }
                    case 2:
                        {
                            _video.set_pixel(i, currentline, (int)_video.Colors[2]);
                            break;
                        }
                    case 3:
                        {
                            _video.set_pixel(i, currentline, (int)_video.Colors[3]);
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }
        }
        #endregion

        #region render sprites
        void Render_Sprites()
        {
            byte lcdc_status = _memory.Retrieve_LCDC;
            byte currentline = _memory.Retrieve_LY;
            bool use8x16mode;

            if ((lcdc_status & 0x4) != 0)
            {
                use8x16mode = true;
            }
            else
            {
                use8x16mode = false;
            }

            for (int sprite = 0; sprite < 40; sprite++)
            {
                byte spriteIndex = (byte)(sprite * 4);
                byte spriteYPos = (byte)(_memory.read_byte((ushort)(0xFE00 + spriteIndex)) - 16);
                byte spriteXPos = (byte)(_memory.read_byte((ushort)(0xFE00 + spriteIndex + 1)) - 8);
                byte spritePattern = _memory.read_byte((ushort)(0xFE00 + spriteIndex + 2));
                byte spriteAttributes = _memory.read_byte((ushort)(0xFE00 + spriteIndex + 3));
                bool spritePriority = ((spriteAttributes >> 7 & 0x1) == 1);
                bool spriteYFlip = ((spriteAttributes >> 6 & 0x1) == 1);
                bool spriteXFlip = ((spriteAttributes >> 5 & 0x1) == 1);
                bool spritePallate1 = ((spriteAttributes >> 4 & 0x1) == 1);
                byte ySize = (byte)(use8x16mode == true ? 16 : 8);

                if (currentline >= spriteYPos && currentline < (spriteYPos + ySize))
                {
                    int spriteLine = currentline - spriteYPos;

                    if (spriteYFlip)
                    {
                        spriteLine -= ySize;
                        spriteLine *= -1;
                    }

                    spriteLine *= 2;
                    ushort spriteAddress = (ushort)(0x8000 + (spritePattern * 16) + spriteLine);
                    byte data1 = _memory.read_byte((ushort)(spriteAddress));
                    byte data2 = _memory.read_byte((ushort)(spriteAddress + 1));

                    for (int spritePixel = 7; spritePixel >= 0; spritePixel--)
                    {
                        int cur_pixel = spritePixel;

                        if (spriteXFlip)
                        {
                            cur_pixel -= 7;
                            cur_pixel *= -1;
                        }

                        byte color = (byte)((((data2 >> cur_pixel) & 0x1) << 1) | ((data1 >> cur_pixel) & 0x1));
                        byte true_color = get_obj_color(color, spritePallate1 == true ? _memory.Retrieve_OBP1 : _memory.Retrieve_OBP0);

                        int pixel = 0 - spritePixel;
                        pixel = spriteXPos + (pixel + 7);

                        if ((currentline >= 0) && (currentline < 144) && (pixel >= 0) && (pixel < 160))
                        {
                            switch (true_color)
                            {
                                case 1:
                                    {
                                        _video.set_pixel(pixel, currentline, (int)_video.Colors[1]);
                                        break;
                                    }
                                case 2:
                                    {
                                        _video.set_pixel(pixel, currentline, (int)_video.Colors[2]);
                                        break;
                                    }
                                case 3:
                                    {
                                        _video.set_pixel(pixel, currentline, (int)_video.Colors[3]);
                                        break;
                                    }
                                default:
                                    {
                                        break;
                                    }
                            }
                        }
                    }    
                }
            }
        }
        #endregion

        #region get color
        byte get_color(byte colornum)
        {
            byte palette = _memory.Retrieve_BGP;

            switch (colornum)
            {
                case 0:
                    {
                        return (byte)(palette & 0x3);
                    }
                case 1:
                    {
                        return (byte)((palette & 0xC) >> 2);
                    }
                case 2:
                    {
                        return (byte)((palette & 0x30) >> 4);
                    }
                case 3:
                    {
                        return (byte)((palette & 0xC0) >> 6);
                    }
                default:
                    {
                        return 0;
                    }
            }
        }
        #endregion

        #region get obj color
        byte get_obj_color(byte colornum, byte palette)
        {
            switch (colornum)
            {
                case 0:
                    {
                        return (byte)(palette & 0x3);
                    }
                case 1:
                    {
                        return (byte)((palette & 0xC) >> 2);
                    }
                case 2:
                    {
                        return (byte)((palette & 0x30) >> 4);
                    }
                case 3:
                    {
                        return (byte)((palette & 0xC0) >> 6);
                    }
                default:
                    {
                        return 0;
                    }
            }
        }
        #endregion

        #region Get LCD Status
        private bool get_lcd_status()
        {
            byte lcd_status = _memory.Retrieve_LCDC;

            if ((lcd_status >> 7) != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region Update LCD Status
        private void update_lcd_status()
        {
            byte status = _memory.Retrieve_STAT;
            byte currentmode;
            byte currentline;
            byte mode;
            bool request_lcd_interrupt = false;
            int mode2 = 80;
            int mode3 = 172;

            if (!get_lcd_status())
            {
                status &= 252;
                _memory.Retrieve_STAT = status;
                Scanline_Counter = 0;
                _memory.Retrieve_LY = 0;
                return;
            }

            currentmode = (byte)(status & 0x3);
            currentline = _memory.Retrieve_LY;

            mode = 0;

            if (currentline >= 144)
            {
                status &= 252;
                status ^= 0x1;
                if ((status & 0x10) != 0)
                {
                    request_lcd_interrupt = true;
                }
                mode = 1;
            }
            else if (currentline == 153)
            {
                _memory.Retrieve_LY = 0;
            }
            else
            {
                if (Scanline_Counter <= mode2)
                {
                    status &= 252;
                    status ^= 0x2;
                    if ((status & 0x20) != 0)
                    {
                        request_lcd_interrupt = true;
                    }
                    mode = 2;
                }
                else if (Scanline_Counter <= mode3)
                {
                    status &= 252;
                    status ^= 0x3;
                    mode = 3;
                }
                else
                {
                    status &= 252;
                    if ((status & 0x8) != 0)
                    {
                        request_lcd_interrupt = true;
                    }
                    mode = 0;
                }
            }

            if (mode != currentmode)
            {
                if (request_lcd_interrupt)
                {
                    request_interrupts(1);
                }
            }

            if (_memory.Retrieve_LYC == currentline)
            {
                status |= 0x4;
                if ((status & 0x40) != 0)
                {
                    request_interrupts(1);
                }
            }
            else if ((status & 0x4) != 0)
            {
                status ^= 0x4;
            }
            _memory.Retrieve_STAT = status;
        }
        #endregion

        #region Execute
        public void execute()
        {
            _video.lock_texture();
            if (cycles <= 0)
            {
                int remainder = cycles;
                cycles = 70224 - remainder;
            }

            processInput();

            while (cycles > 0)
            {
                if (!halted && !stopped)
                {
                    Fetch_Opcode();
                    Decode_Opcode();
                }
                else
                {
                    cyclesThisOp = 4;
                }

                process_timers(cyclesThisOp);

                if (!stopped)
                {
                    process_graphics(cyclesThisOp);
                }

                process_interrupts();

                if (disable_int > 0)
                {
                    disable_int--;
                    if (disable_int == 0)
                        IME = false;
                }

                if (enable_int > 0)
                {
                    enable_int--;
                    if (enable_int == 0)
                        IME = true;
                }
            }
            _video.unlock_texture();
            _video.Renderer();
            resetInput();
        }
        #endregion

        #region Step
        public void step()
        {
            if (cycles <= 0)
            {
                int remainder = cycles;
                cycles = 70224 - remainder;
                _video.lock_texture();
            }

            if (cycles > 0)
            {
                process_interrupts();
                if (!halted && !stopped)
                {
                    Fetch_Opcode();
                    Decode_Opcode();
                }
                else
                {
                    cyclesThisOp = 4;
                }
                process_timers(cyclesThisOp);
                if (!stopped)
                {
                    process_graphics(cyclesThisOp);
                }

                if (disable_int > 0)
                {
                    disable_int--;
                    if (disable_int == 0)
                        IME = false;
                }

                if (enable_int > 0)
                {
                    enable_int--;
                    if (enable_int == 0)
                        IME = true;
                }
            }

            if (cycles <= 0)
            {
                _video.unlock_texture();
                _video.Renderer();
            }
        }
        #endregion

        #region breakpoint
        public void breakpoint(ushort addr)
        {
            while (pc != addr)
            {
                if (cycles <= 0)
                {
                    int remainder = cycles;
                    cycles = 70224 - remainder;
                    _video.lock_texture();
                }

                if (!halted && !stopped)
                {
                    Fetch_Opcode();
                    Decode_Opcode();
                }
                else
                {
                    cyclesThisOp = 4;
                }
                process_timers(cyclesThisOp);
                if (!stopped)
                {
                    process_graphics(cyclesThisOp);
                }
                process_interrupts();

                if (disable_int > 0)
                {
                    disable_int--;
                    if (disable_int == 0)
                        IME = false;
                }

                if (enable_int > 0)
                {
                    enable_int--;
                    if (enable_int == 0)
                        IME = true;
                }

                if (cycles <= 0)
                {
                    _video.unlock_texture();
                    _video.Renderer();
                }
            }
        }
        #endregion

        #region select status check
        private bool select_status_check(string type)
        {
            bool result = false;

            switch (type)
            {
                case "JC":
                case "CC":
                case "RC":
                    {
                        if (check_carry_flag())
                        {
                            result = true;
                        }
                        break;
                    }
                case "JNC":
                case "CNC":
                case "RNC":
                    {
                        if (!check_carry_flag())
                        {
                            result = true;
                        }
                        break;
                    }
                case "JZ":
                case "CZ":
                case "RZ":
                    {

                        if (check_zero_flag())
                        {
                            result = true;
                        }
                        break;
                    }
                case "JNZ":
                case "CNZ":
                case "RNZ":
                    {

                        if (!check_zero_flag())
                        {
                            result = true;
                        }
                        break;
                    }
            }

            return result;
        }
        #endregion

        #region normal carry set
        private void normal_carry_flag_set(ushort result)
        {
            if (result > 0xFF)
            {
                if (!check_carry_flag())
                {
                    set_carry_flag(true);
                }
            }
            else if (check_carry_flag())
            {
                set_carry_flag(false);
            }
        }
        #endregion

        #region sub carry set
        private void sub_carry_flag_set(byte reg1, byte reg2)
        {
            if (reg1 < reg2)
            {
                if (!check_carry_flag())
                {
                    set_carry_flag(true);
                }
            }
            else if (check_carry_flag())
            {
                set_carry_flag(false);
            }
        }
        #endregion

        #region dad carry set
        private void dad_carry_flag_set(uint result)
        {
            if (result > 0xFFFF)
            {
                if (!check_carry_flag())
                {
                    set_carry_flag(true);
                }
            }
            else if (check_carry_flag())
            {
                set_carry_flag(false);
            }
        }
        #endregion

        #region set carry
        private void set_carry_flag(bool set)
        {
            if (set)
            {
                if (!check_carry_flag())
                {
                    regs.F ^= 0x10;
                }
            }
            else if (check_carry_flag())
            {
                regs.F ^= 0x10;
            }
        }
        #endregion

        #region check carry
        public bool check_carry_flag()
        {
            if ((regs.F >> 4 & 0x1) == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region auxiliary carry set
        private void auxiliary_carry_flag_set(byte value, byte test)
        {
            byte htest = (byte)(value & 0xF);
            htest += (byte)(test & 0xF);
            if (htest > 0xF)
                set_auxiliary_carry_flag(true);
            else
                set_auxiliary_carry_flag(false);
        }
        #endregion

        #region sub auxiliary carry set
        private void sub_auxiliary_carry_flag_set(byte test)
        {
            byte htest = (byte)(regs.A & 0xF);
            htest -= (byte)(test & 0xF);
            if (htest > regs.A)
                set_auxiliary_carry_flag(true);
            else
                set_auxiliary_carry_flag(false);
        }
        #endregion

        #region auxiliary carry set
        private void auxiliary_carry_flag_set(ushort value, ushort test)
        {
            ushort htest = (ushort)(value & 0xFFF);
            htest += (ushort)(test & 0xFFF);
            if (htest > 0xFFF)
                set_auxiliary_carry_flag(true);
            else
                set_auxiliary_carry_flag(false);
        }
        #endregion

        #region set auxiliary carry
        private void set_auxiliary_carry_flag(bool set)
        {
            if (set)
            {
                if (!check_auxiliary_flag())
                {
                    regs.F ^= 0x20;
                }
            }
            else if (check_auxiliary_flag())
            {
                regs.F ^= 0x20;
            }
        }
        #endregion

        #region check auxiliary carry
        public bool check_auxiliary_flag()
        {
            if ((regs.F >> 5 & 0x1) == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region zero set
        private void zero_flag_set(byte result)
        {
            if (result == 0)
            {
                if (!check_zero_flag())
                {
                    set_zero_flag(true);
                }
            }
            else if (check_zero_flag())
            {
                set_zero_flag(false);
            }
        }
        #endregion

        #region set zero
        private void set_zero_flag(bool set)
        {
            if (set)
            {
                if (!check_zero_flag())
                {
                    regs.F ^= 0x80;
                }
            }
            else if (check_zero_flag())
            {
                regs.F ^= 0x80;
            }
        }
        #endregion

        #region check zero
        public bool check_zero_flag()
        {
            if ((regs.F >> 7 & 0x1) == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region set negative
        private void set_negative_flag(bool set)
        {
            if (set)
            {
                if (!check_negative_flag())
                {
                    regs.F ^= 0x40;
                }
            }
            else if (check_negative_flag())
            {
                regs.F ^= 0x40;
            }
        }
        #endregion

        #region check negative
        public bool check_negative_flag()
        {
            if ((regs.F >> 6 & 0x1) == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region fetch opcode
        private void Fetch_Opcode()
        {
            opcode = _memory.read_byte(pc);
        }
        #endregion

        #region write opcode log
        public void Write_Opcode_Log()
        {
            System.IO.StreamWriter write = new System.IO.StreamWriter("opcodeLog.txt");
            System.IO.StreamWriter writeInstructions = new System.IO.StreamWriter("instructionLog.txt");

            write.WriteLine("regular opcodes:");

            for (int i = 0; i < cycles_lookup_log1.Length; i++)
            {
                write.WriteLine(cycles_lookup_log1[i] + ": " + cycles_lookup_log2[i].ToString());
            }

            write.WriteLine("CB opcodes:");

            for (int i = 0; i < cycles_lookup_CB_log1.Length; i++)
            {
                write.WriteLine(cycles_lookup_CB_log1[i] + ": " + cycles_lookup_CB_log2[i].ToString());
            }

            write.WriteLine("10 opcodes:");

            for (int i = 0; i < cycles_lookup_10_log1.Length; i++)
            {
                write.WriteLine(cycles_lookup_10_log1[i] + ": " + cycles_lookup_10_log2[i].ToString());
            }

            writeInstructions.Write(output);

            write.Close();
            writeInstructions.Close();
        }
        #endregion

        #region write instruction log
        public void Write_Instruction_Log()
        {
            writeCount++;
            output.AppendFormat("{0}    CPU Status: {1}", writeCount, regs.F).AppendLine();
            //output += writeCount + "    CPU Status: " + regs.F + "\r\n";
            output.AppendFormat("AF: {0}    BC: {1}", regs.AF, regs.BC).AppendLine();
            //output += "AF: " + regs.AF + "    BC: " + regs.BC + "\r\n";
            output.AppendFormat("DE: {0}    HL: {1}", regs.DE, regs.HL).AppendLine();
            //output += "DE: " + regs.DE + "    HL: " + regs.HL + "\r\n";
            output.AppendFormat("SP: {0}", sp).AppendLine();
            //output += "SP: " + sp + "\r\n";
        }
        #endregion

        #region decode opcode
        private void Decode_Opcode()
        {
            cyclesThisOp = cycles_lookup[opcode];
            cycles -= cyclesThisOp;
            cycles_lookup_log2[opcode]++;

            switch (opcode)
            {
                case 0x00:
                    {
                        //NOP();
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x01:
                    {
                        //LD_NN_RR(ref regs.BC);
                        regs.BC = _memory.read_ushort((ushort)(pc + 1));
                        pc += 3;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x02:
                    {
                        //LD_R2_R1(ref regs.A, regs.BC);
                        _memory.write_byte(regs.BC, regs.A);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x03:
                    {
                        //INC_RR(ref regs.BC);
                        regs.BC++;
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x04:
                    {
                        //INC_R(ref regs.B);
                        byte temp1 = regs.B;

                        regs.B++;

                        auxiliary_carry_flag_set(temp1, 0x1);
                        zero_flag_set(regs.B);
                        set_negative_flag(false);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x05:
                    {
                        //DEC_R(ref regs.B);
                        byte temp1 = 1;
                        temp1 = (byte)(-temp1);

                        byte htest = (byte)(regs.B & 0xF);
                        htest -= (byte)(0x1 & 0xF);
                        if (htest > regs.B)
                            set_auxiliary_carry_flag(true);
                        else
                            set_auxiliary_carry_flag(false);

                        regs.B--;

                        zero_flag_set(regs.B);
                        set_negative_flag(true);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x06:
                    {
                        regs.B = _memory.read_byte((ushort)(pc + 1));
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        //LD_N_R(ref regs.B);
                        return;
                    }
                case 0x07:
                    {
                        //RLCA();
                        set_auxiliary_carry_flag(false);
                        set_negative_flag(false);

                        set_carry_flag((regs.A >> 7 & 0x1) != 0);
                        regs.A = (byte)(regs.A << 1 | (regs.A >> 7 & 0x1));

                        set_zero_flag(false);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x08:
                    {
                        //LD_MNN_RR(ref sp);
                        _memory.write_ushort(_memory.read_ushort((ushort)(pc + 1)), sp);
                        pc += 3;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x09:
                    {
                        //ADD_RR(ref regs.BC);
                        auxiliary_carry_flag_set(regs.HL, regs.BC);
                        dad_carry_flag_set((uint)(regs.HL + regs.BC));

                        regs.HL += regs.BC;

                        set_negative_flag(false);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x0A:
                    {
                        //LD_R2_R1(regs.BC, ref regs.A);
                        regs.A = _memory.read_byte(regs.BC); ;
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x0B:
                    {
                        //DEC_RR(ref regs.BC);
                        regs.BC--;
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x0C:
                    {
                        //INC_R(ref regs.C);
                        byte temp1 = regs.C;

                        regs.C++;

                        auxiliary_carry_flag_set(temp1, 0x1);
                        zero_flag_set(regs.C);
                        set_negative_flag(false);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x0D:
                    {
                        //DEC_R(ref regs.C);
                        byte temp1 = 1;
                        temp1 = (byte)(-temp1);

                        byte htest = (byte)(regs.C & 0xF);
                        htest -= (byte)(0x1 & 0xF);
                        if (htest > regs.C)
                            set_auxiliary_carry_flag(true);
                        else
                            set_auxiliary_carry_flag(false);

                        regs.C--;

                        zero_flag_set(regs.C);
                        set_negative_flag(true);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x0E:
                    {
                        regs.C = _memory.read_byte((ushort)(pc + 1));
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        //LD_N_R(ref regs.C);
                        return;
                    }
                case 0x0F:
                    {
                        //RRCA();
                        set_negative_flag(false);
                        set_auxiliary_carry_flag(false);

                        set_carry_flag(Convert.ToBoolean(regs.A & 0x1));
                        byte temp1 = (byte)(regs.A & 0x1);
                        regs.A >>= 1;
                        regs.A |= (byte)((temp1) << 7);

                        set_zero_flag(false);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x10:
                    {
                        Decode_Opcode_10(_memory.read_byte((ushort)(pc + 1)));
                        return;
                    }
                case 0x11:
                    {
                        //LD_NN_RR(ref regs.DE);
                        regs.DE = _memory.read_ushort((ushort)(pc + 1));
                        pc += 3;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x12:
                    {
                        //LD_R2_R1(ref regs.A, regs.DE);
                        _memory.write_byte(regs.DE, regs.A);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x13:
                    {
                        //INC_RR(ref regs.DE);
                        regs.DE++;
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x14:
                    {
                        //INC_R(ref regs.D);
                        byte temp1 = regs.D;

                        regs.D++;

                        auxiliary_carry_flag_set(temp1, 0x1);
                        zero_flag_set(regs.D);
                        set_negative_flag(false);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x15:
                    {
                        //DEC_R(ref regs.D);
                        byte temp1 = 1;
                        temp1 = (byte)(-temp1);

                        byte htest = (byte)(regs.D & 0xF);
                        htest -= (byte)(0x1 & 0xF);
                        if (htest > regs.D)
                            set_auxiliary_carry_flag(true);
                        else
                            set_auxiliary_carry_flag(false);

                        regs.D--;

                        zero_flag_set(regs.D);
                        set_negative_flag(true);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x16:
                    {
                        regs.D = _memory.read_byte((ushort)(pc + 1));
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        //LD_N_R(ref regs.D);
                        return;
                    }
                case 0x17:
                    {
                        //RLA();
                        byte temp1 = (byte)((regs.F & 0x10) >> 4);

                        set_auxiliary_carry_flag(false);
                        set_negative_flag(false);

                        set_carry_flag(Convert.ToBoolean(regs.A >> 7 & 0x1));
                        regs.A <<= 1;
                        regs.A |= temp1;

                        set_zero_flag(false);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x18:
                    {
                        //JR_N();
                        sbyte temp1 = (sbyte)(_memory.read_byte((ushort)(pc + 1)));
                        pc += 2;
                        pc = (ushort)(pc + temp1);
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x19:
                    {
                        //ADD_RR(ref regs.DE);
                        auxiliary_carry_flag_set(regs.HL, regs.DE);
                        dad_carry_flag_set((uint)(regs.HL + regs.DE));

                        regs.HL += regs.DE;

                        set_negative_flag(false);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x1A:
                    {
                        //LD_R2_R1(regs.DE, ref regs.A);
                        regs.A = _memory.read_byte(regs.DE); ;
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x1B:
                    {
                        //DEC_RR(ref regs.DE);
                        regs.DE--;
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x1C:
                    {
                        //INC_R(ref regs.E);
                        byte temp1 = regs.E;

                        regs.E++;

                        auxiliary_carry_flag_set(temp1, 0x1);
                        zero_flag_set(regs.E);
                        set_negative_flag(false);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x1D:
                    {
                        //DEC_R(ref regs.E);
                        byte temp1 = 1;
                        temp1 = (byte)(-temp1);

                        byte htest = (byte)(regs.E & 0xF);
                        htest -= (byte)(0x1 & 0xF);
                        if (htest > regs.E)
                            set_auxiliary_carry_flag(true);
                        else
                            set_auxiliary_carry_flag(false);

                        regs.E--;

                        zero_flag_set(regs.E);
                        set_negative_flag(true);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x1E:
                    {
                        regs.E = _memory.read_byte((ushort)(pc + 1));
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        //LD_N_R(ref regs.E);
                        return;
                    }
                case 0x1F:
                    {
                        //RRA();
                        byte temp1 = (byte)((regs.F & 0x10) >> 4);

                        set_negative_flag(false);
                        set_auxiliary_carry_flag(false);

                        set_carry_flag(Convert.ToBoolean(regs.A & 0x1));
                        regs.A >>= 1;
                        regs.A |= (byte)(temp1 << 7);

                        set_zero_flag(false);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x20:
                    {
                        //JR_NZ_N();
                        pc += 2;
                        if (!check_zero_flag())
                        {
                            sbyte temp1 = (sbyte)(_memory.read_byte((ushort)(pc - 1)));
                            pc = (ushort)(pc + temp1);
                            cycles -= 4;
                            cyclesThisOp += 4;
                            next_opcode = _memory.read_byte(pc);
                        }
                        else
                        {
                            next_opcode = _memory.read_byte(pc);
                        }
                        return;
                    }
                case 0x21:
                    {
                        //LD_NN_RR(ref regs.HL);
                        regs.HL = _memory.read_ushort((ushort)(pc + 1));
                        pc += 3;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x22:
                    {
                        //LDI_R_M(ref regs.A, ref regs.HL);
                        _memory.write_byte(regs.HL, regs.A);
                        regs.HL++;
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x23:
                    {
                        //INC_RR(ref regs.HL);
                        regs.HL++;
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x24:
                    {
                        //INC_R(ref regs.H);
                        byte temp1 = regs.H;

                        regs.H++;

                        auxiliary_carry_flag_set(temp1, 0x1);
                        zero_flag_set(regs.H);
                        set_negative_flag(false);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x25:
                    {
                        //DEC_R(ref regs.H);
                        byte temp1 = 1;
                        temp1 = (byte)(-temp1);

                        byte htest = (byte)(regs.H & 0xF);
                        htest -= (byte)(0x1 & 0xF);
                        if (htest > regs.H)
                            set_auxiliary_carry_flag(true);
                        else
                            set_auxiliary_carry_flag(false);

                        regs.H--;

                        zero_flag_set(regs.H);
                        set_negative_flag(true);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x26:
                    {
                        regs.H = _memory.read_byte((ushort)(pc + 1));
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        //LD_N_R(ref regs.H);
                        return;
                    }
                case 0x27:
                    {
                        //DAA();
                        if (check_negative_flag())
                        {
                            byte temp = regs.A;
                            if (check_auxiliary_flag())
                            {
                                temp -= 0x6;
                            }
                            if (check_carry_flag())
                            {
                                temp -= 0x60;
                            }
                            regs.A = temp;
                        }
                        else
                        {
                            byte temp = regs.A;
                            if (((regs.A & 0xF) > 0x9) || (check_auxiliary_flag()))
                            {
                                temp += 0x6;
                            }
                            if (regs.A > 0x99 || check_carry_flag())
                            {
                                set_carry_flag(true);

                                temp += 0x60;
                            }
                            regs.A = temp;
                        }

                        set_auxiliary_carry_flag(false);
                        zero_flag_set(regs.A);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x28:
                    {
                        //JR_Z_N();
                        pc += 2;
                        if (check_zero_flag())
                        {
                            sbyte temp1 = (sbyte)(_memory.read_byte((ushort)(pc - 1)));
                            pc = (ushort)(pc + temp1);
                            cycles -= 4;
                            cyclesThisOp += 4;
                            next_opcode = _memory.read_byte(pc);
                        }
                        else
                        {
                            next_opcode = _memory.read_byte(pc);
                        }
                        return;
                    }
                case 0x29:
                    {
                        //ADD_RR(ref regs.HL);
                        auxiliary_carry_flag_set(regs.HL, regs.HL);
                        dad_carry_flag_set((uint)(regs.HL + regs.HL));

                        regs.HL += regs.HL;

                        set_negative_flag(false);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x2A:
                    {
                        //LDI_M_R(ref regs.HL, ref regs.A);
                        regs.A = _memory.read_byte(regs.HL);
                        regs.HL++;
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x2B:
                    {
                        //DEC_RR(ref regs.HL);
                        regs.HL--;
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x2C:
                    {
                        //INC_R(ref regs.L);
                        byte temp1 = regs.L;

                        regs.L++;

                        auxiliary_carry_flag_set(temp1, 0x1);
                        zero_flag_set(regs.L);
                        set_negative_flag(false);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x2D:
                    {
                        //DEC_R(ref regs.L);
                        byte temp1 = 1;
                        temp1 = (byte)(-temp1);

                        byte htest = (byte)(regs.L & 0xF);
                        htest -= (byte)(0x1 & 0xF);
                        if (htest > regs.L)
                            set_auxiliary_carry_flag(true);
                        else
                            set_auxiliary_carry_flag(false);

                        regs.L--;

                        zero_flag_set(regs.L);
                        set_negative_flag(true);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x2E:
                    {
                        regs.L = _memory.read_byte((ushort)(pc + 1));
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        //LD_N_R(ref regs.L);
                        return;
                    }
                case 0x2F:
                    {
                        //CPL();
                        regs.A = (byte)~(regs.A);

                        set_auxiliary_carry_flag(true);
                        set_negative_flag(true);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x30:
                    {
                        //JR_NC_N();
                        pc += 2;
                        if (!check_carry_flag())
                        {
                            sbyte temp1 = (sbyte)(_memory.read_byte((ushort)(pc - 1)));
                            pc = (ushort)(pc + temp1);
                            cycles -= 4;
                            cyclesThisOp += 4;
                            next_opcode = _memory.read_byte(pc);
                        }
                        else
                        {
                            next_opcode = _memory.read_byte(pc);
                        }
                        return;
                    }
                case 0x31:
                    {
                        //LD_NN_RR(ref sp);
                        sp = _memory.read_ushort((ushort)(pc + 1));
                        pc += 3;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x32:
                    {
                        //LDD_R_M(ref regs.A, ref regs.HL);
                        _memory.write_byte(regs.HL, regs.A);
                        regs.HL--;
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x33:
                    {
                        //INC_RR(ref sp);
                        sp++;
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x34:
                    {
                        //INC_M(regs.HL);
                        byte temp1 = _memory.read_byte(regs.HL);
                        byte temp2 = _memory.read_byte(regs.HL);

                        temp2++;
                        _memory.write_byte(regs.HL, temp2);

                        auxiliary_carry_flag_set(temp1, 0x1);
                        zero_flag_set(_memory.read_byte(regs.HL));
                        set_negative_flag(false);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x35:
                    {
                        //DEC_M(regs.HL);
                        byte temp1 = _memory.read_byte(regs.HL);

                        byte htest = (byte)(temp1 & 0xF);
                        htest -= (byte)(0x1 & 0xF);
                        if (htest > temp1)
                            set_auxiliary_carry_flag(true);
                        else
                            set_auxiliary_carry_flag(false);

                        byte temp2 = temp1;
                        temp2--;
                        _memory.write_byte(regs.HL, temp2);

                        zero_flag_set(_memory.read_byte(regs.HL));
                        set_negative_flag(true);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x36:
                    {
                        //LD_R2_R1(regs.HL);
                        _memory.write_byte(regs.HL, _memory.read_byte((ushort)(pc + 1)));
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x37:
                    {
                        //SCF();
                        set_carry_flag(true);

                        set_negative_flag(false);
                        set_auxiliary_carry_flag(false);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x38:
                    {
                        //JR_C_N();
                        pc += 2;
                        if (check_carry_flag())
                        {
                            sbyte temp1 = (sbyte)(_memory.read_byte((ushort)(pc - 1)));
                            pc = (ushort)(pc + temp1);
                            cycles -= 4;
                            cyclesThisOp += 4;
                            next_opcode = _memory.read_byte(pc);
                        }
                        else
                        {
                            next_opcode = _memory.read_byte(pc);
                        }
                        return;
                    }
                case 0x39:
                    {
                        //ADD_RR(ref sp);
                        auxiliary_carry_flag_set(regs.HL, sp);
                        dad_carry_flag_set((uint)(regs.HL + sp));

                        regs.HL += sp;

                        set_negative_flag(false);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x3A:
                    {
                        //LDD_M_R(ref regs.HL, ref regs.A);
                        regs.A = _memory.read_byte(regs.HL);
                        regs.HL--;
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x3B:
                    {
                        //DEC_RR(ref sp);
                        sp--;
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x3C:
                    {
                        //INC_R(ref regs.A);
                        byte temp1 = regs.A;

                        regs.A++;

                        auxiliary_carry_flag_set(temp1, 0x1);
                        zero_flag_set(regs.A);
                        set_negative_flag(false);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x3D:
                    {
                        //DEC_R(ref regs.A);
                        byte htest = (byte)(regs.A & 0xF);
                        htest -= (byte)(0x1 & 0xF);
                        if (htest > regs.A)
                            set_auxiliary_carry_flag(true);
                        else
                            set_auxiliary_carry_flag(false);

                        regs.A--;

                        zero_flag_set(regs.A);
                        set_negative_flag(true);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x3E:
                    {
                        //LD_N_R(ref regs.A);
                        regs.A = _memory.read_byte((ushort)(pc + 1)); ;
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x3F:
                    {
                        //CCF();
                        regs.F ^= 0x10;

                        set_negative_flag(false);
                        set_auxiliary_carry_flag(false);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x40:
                    {
                        //LD_R2_R1(ref regs.B, ref regs.B);
                        regs.B = regs.B;
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x41:
                    {
                        //LD_R2_R1(ref regs.C, ref regs.B);
                        regs.B = regs.C;
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x42:
                    {
                        //LD_R2_R1(ref regs.D, ref regs.B);
                        regs.B = regs.D;
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x43:
                    {
                        //LD_R2_R1(ref regs.E, ref regs.B);
                        regs.B = regs.E;
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x44:
                    {
                        //LD_R2_R1(ref regs.H, ref regs.B);
                        regs.B = regs.H;
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x45:
                    {
                        //LD_R2_R1(ref regs.L, ref regs.B);
                        regs.B = regs.L;
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x46:
                    {
                        //LD_R2_R1(regs.HL, ref regs.B);
                        regs.B = _memory.read_byte(regs.HL);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x47:
                    {
                        //LD_R2_R1(ref regs.A, ref regs.B);
                        regs.B = regs.A;
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x48:
                    {
                        //LD_R2_R1(ref regs.B, ref regs.C);
                        regs.C = regs.B;
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x49:
                    {
                        //LD_R2_R1(ref regs.C, ref regs.C);
                        regs.C = regs.C;
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x4A:
                    {
                        //LD_R2_R1(ref regs.D, ref regs.C);
                        regs.C = regs.D;
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x4B:
                    {
                        //LD_R2_R1(ref regs.E, ref regs.C);
                        regs.C = regs.E;
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x4C:
                    {
                        //LD_R2_R1(ref regs.H, ref regs.C);
                        regs.C = regs.H;
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x4D:
                    {
                        //LD_R2_R1(ref regs.L, ref regs.C);
                        regs.C = regs.L;
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x4E:
                    {
                        //LD_R2_R1(regs.HL, ref regs.C);
                        regs.C = _memory.read_byte(regs.HL); ;
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x4F:
                    {
                        //LD_R2_R1(ref regs.A, ref regs.C);
                        regs.C = regs.A;
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x50:
                    {
                        //LD_R2_R1(ref regs.B, ref regs.D);
                        regs.D = regs.B;
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x51:
                    {
                        //LD_R2_R1(ref regs.C, ref regs.D);
                        regs.D = regs.C;
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x52:
                    {
                        //LD_R2_R1(ref regs.D, ref regs.D);
                        regs.D = regs.D;
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x53:
                    {
                        //LD_R2_R1(ref regs.E, ref regs.D);
                        regs.D = regs.E;
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x54:
                    {
                        //LD_R2_R1(ref regs.H, ref regs.D);
                        regs.D = regs.H;
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x55:
                    {
                        //LD_R2_R1(ref regs.L, ref regs.D);
                        regs.D = regs.L;
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x56:
                    {
                        //LD_R2_R1(regs.HL, ref regs.D);
                        regs.D = _memory.read_byte(regs.HL); ;
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x57:
                    {
                        //LD_R2_R1(ref regs.A, ref regs.D);
                        regs.D = regs.A;
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x58:
                    {
                        //LD_R2_R1(ref regs.B, ref regs.E);
                        regs.E = regs.B;
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x59:
                    {
                        //LD_R2_R1(ref regs.C, ref regs.E);
                        regs.E = regs.C;
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x5A:
                    {
                        //LD_R2_R1(ref regs.D, ref regs.E);
                        regs.E = regs.D;
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x5B:
                    {
                        //LD_R2_R1(ref regs.E, ref regs.E);
                        regs.E = regs.E;
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x5C:
                    {
                        //LD_R2_R1(ref regs.H, ref regs.E);
                        regs.E = regs.H;
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x5D:
                    {
                        //LD_R2_R1(ref regs.L, ref regs.E);
                        regs.E = regs.L;
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x5E:
                    {
                        //LD_R2_R1(regs.HL, ref regs.E);
                        regs.E = _memory.read_byte(regs.HL);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x5F:
                    {
                        //LD_R2_R1(ref regs.A, ref regs.E);
                        regs.E = regs.A;
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x60:
                    {
                        //LD_R2_R1(ref regs.B, ref regs.H);
                        regs.H = regs.B;
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x61:
                    {
                        //LD_R2_R1(ref regs.C, ref regs.H);
                        regs.H = regs.C;
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x62:
                    {
                        //LD_R2_R1(ref regs.D, ref regs.H);
                        regs.H = regs.D;
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x63:
                    {
                        //LD_R2_R1(ref regs.E, ref regs.H);
                        regs.H = regs.E;
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x64:
                    {
                        //LD_R2_R1(ref regs.H, ref regs.H);
                        regs.H = regs.H;
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x65:
                    {
                        //LD_R2_R1(ref regs.L, ref regs.H);
                        regs.H = regs.L;
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x66:
                    {
                        //LD_R2_R1(regs.HL, ref regs.H);
                        regs.H = _memory.read_byte(regs.HL);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x67:
                    {
                        //LD_R2_R1(ref regs.A, ref regs.H);
                        regs.H = regs.A;
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x68:
                    {
                        //LD_R2_R1(ref regs.B, ref regs.L);
                        regs.L = regs.B;
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x69:
                    {
                        //LD_R2_R1(ref regs.C, ref regs.L);
                        regs.L = regs.C;
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x6A:
                    {
                        //LD_R2_R1(ref regs.D, ref regs.L);
                        regs.L = regs.D;
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x6B:
                    {
                        //LD_R2_R1(ref regs.E, ref regs.L);
                        regs.L = regs.E;
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x6C:
                    {
                        //LD_R2_R1(ref regs.H, ref regs.L);
                        regs.L = regs.H;
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x6D:
                    {
                        //LD_R2_R1(ref regs.L, ref regs.L);
                        regs.L = regs.L;
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x6E:
                    {
                        //LD_R2_R1(regs.HL, ref regs.L);
                        regs.L = _memory.read_byte(regs.HL);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x6F:
                    {
                        //LD_R2_R1(ref regs.A, ref regs.L);
                        regs.L = regs.A;
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x70:
                    {
                        //LD_R2_R1(ref regs.B, regs.HL);
                        _memory.write_byte(regs.HL, regs.B);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x71:
                    {
                        //LD_R2_R1(ref regs.C, regs.HL);
                        _memory.write_byte(regs.HL, regs.C);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x72:
                    {
                        //LD_R2_R1(ref regs.D, regs.HL);
                        _memory.write_byte(regs.HL, regs.D);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x73:
                    {
                        //LD_R2_R1(ref regs.E, regs.HL);
                        _memory.write_byte(regs.HL, regs.E);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x74:
                    {
                        //LD_R2_R1(ref regs.H, regs.HL);
                        _memory.write_byte(regs.HL, regs.H);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x75:
                    {
                        //LD_R2_R1(ref regs.L, regs.HL);
                        _memory.write_byte(regs.HL, regs.L);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x76:
                    {
                        //HALT();
                        pc++;
                        halted = true;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x77:
                    {
                        //LD_R2_R1(ref regs.A, regs.HL);
                        _memory.write_byte(regs.HL, regs.A);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x78:
                    {
                        //LD_R2_R1(ref regs.B, ref regs.A);
                        regs.A = regs.B;
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x79:
                    {
                        //LD_R2_R1(ref regs.C, ref regs.A);
                        regs.A = regs.C;
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x7A:
                    {
                        //LD_R2_R1(ref regs.D, ref regs.A);
                        regs.A = regs.D;
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x7B:
                    {
                        //LD_R2_R1(ref regs.E, ref regs.A);
                        regs.A = regs.E;
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x7C:
                    {
                        //LD_R2_R1(ref regs.H, ref regs.A);
                        regs.A = regs.H;
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x7D:
                    {
                        //LD_R2_R1(ref regs.L, ref regs.A);
                        regs.A = regs.L;
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x7E:
                    {
                        //LD_R2_R1(regs.HL, ref regs.A);
                        regs.A = _memory.read_byte(regs.HL); ;
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x7F:
                    {
                        //LD_R2_R1(ref regs.A, ref regs.A);
                        regs.A = regs.A;
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x80:
                    {
                        //ADD_R(ref regs.B);
                        auxiliary_carry_flag_set(regs.A, regs.B);
                        normal_carry_flag_set((ushort)(regs.A + regs.B));

                        regs.A += regs.B;

                        set_negative_flag(false);
                        zero_flag_set(regs.A);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x81:
                    {
                        //ADD_R(ref regs.C);
                        auxiliary_carry_flag_set(regs.A, regs.C);
                        normal_carry_flag_set((ushort)(regs.A + regs.C));

                        regs.A += regs.C;

                        set_negative_flag(false);
                        zero_flag_set(regs.A);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x82:
                    {
                        //ADD_R(ref regs.D);
                        auxiliary_carry_flag_set(regs.A, regs.D);
                        normal_carry_flag_set((ushort)(regs.A + regs.D));

                        regs.A += regs.D;

                        set_negative_flag(false);
                        zero_flag_set(regs.A);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x83:
                    {
                        //ADD_R(ref regs.E);
                        auxiliary_carry_flag_set(regs.A, regs.E);
                        normal_carry_flag_set((ushort)(regs.A + regs.E));

                        regs.A += regs.E;

                        set_negative_flag(false);
                        zero_flag_set(regs.A);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x84:
                    {
                        //ADD_R(ref regs.H);
                        auxiliary_carry_flag_set(regs.A, regs.H);
                        normal_carry_flag_set((ushort)(regs.A + regs.H));

                        regs.A += regs.H;

                        set_negative_flag(false);
                        zero_flag_set(regs.A);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x85:
                    {
                        //ADD_R(ref regs.L);
                        auxiliary_carry_flag_set(regs.A, regs.L);
                        normal_carry_flag_set((ushort)(regs.A + regs.L));

                        regs.A += regs.L;

                        set_negative_flag(false);
                        zero_flag_set(regs.A);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x86:
                    {
                        //ADD_M(regs.HL);
                        auxiliary_carry_flag_set(regs.A, _memory.read_byte(regs.HL));
                        normal_carry_flag_set((ushort)(regs.A + _memory.read_byte(regs.HL)));

                        regs.A += _memory.read_byte(regs.HL);

                        set_negative_flag(false);
                        zero_flag_set(regs.A);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x87:
                    {
                        //ADD_R(ref regs.A);
                        auxiliary_carry_flag_set(regs.A, regs.A);
                        normal_carry_flag_set((ushort)(regs.A + regs.A));

                        regs.A += regs.A;

                        set_negative_flag(false);
                        zero_flag_set(regs.A);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x88:
                    {
                        //ADC_R(ref regs.B);
                        byte temp1 = regs.A;
                        if (check_carry_flag())
                        {
                            if ((temp1 & 0xF) + (regs.B & 0xF) > 0xE)
                            {
                                set_auxiliary_carry_flag(true);
                            }
                            else
                            {
                                set_auxiliary_carry_flag(false);
                            }
                        }
                        else
                        {
                            if ((temp1 & 0xF) + (regs.B & 0xF) > 0xF)
                            {
                                set_auxiliary_carry_flag(true);
                            }
                            else
                            {
                                set_auxiliary_carry_flag(false);
                            }
                        }

                        regs.A += (byte)(regs.B + (regs.F >> 4 & 0x1));

                        normal_carry_flag_set((ushort)(temp1 + regs.B + (regs.F >> 4 & 0x1)));
                        zero_flag_set(regs.A);
                        set_negative_flag(false);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x89:
                    {
                        //ADC_R(ref regs.C);
                        byte temp1 = regs.A;
                        if (check_carry_flag())
                        {
                            if ((temp1 & 0xF) + (regs.C & 0xF) > 0xE)
                            {
                                set_auxiliary_carry_flag(true);
                            }
                            else
                            {
                                set_auxiliary_carry_flag(false);
                            }
                        }
                        else
                        {
                            if ((temp1 & 0xF) + (regs.C & 0xF) > 0xF)
                            {
                                set_auxiliary_carry_flag(true);
                            }
                            else
                            {
                                set_auxiliary_carry_flag(false);
                            }
                        }

                        regs.A += (byte)(regs.C + (regs.F >> 4 & 0x1));

                        normal_carry_flag_set((ushort)(temp1 + regs.C + (regs.F >> 4 & 0x1)));
                        zero_flag_set(regs.A);
                        set_negative_flag(false);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x8A:
                    {
                        //ADC_R(ref regs.D);
                        byte temp1 = regs.A;
                        if (check_carry_flag())
                        {
                            if ((temp1 & 0xF) + (regs.D & 0xF) > 0xE)
                            {
                                set_auxiliary_carry_flag(true);
                            }
                            else
                            {
                                set_auxiliary_carry_flag(false);
                            }
                        }
                        else
                        {
                            if ((temp1 & 0xF) + (regs.D & 0xF) > 0xF)
                            {
                                set_auxiliary_carry_flag(true);
                            }
                            else
                            {
                                set_auxiliary_carry_flag(false);
                            }
                        }

                        regs.A += (byte)(regs.D + (regs.F >> 4 & 0x1));

                        normal_carry_flag_set((ushort)(temp1 + regs.D + (regs.F >> 4 & 0x1)));
                        zero_flag_set(regs.A);
                        set_negative_flag(false);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x8B:
                    {
                        //ADC_R(ref regs.E);
                        byte temp1 = regs.A;
                        if (check_carry_flag())
                        {
                            if ((temp1 & 0xF) + (regs.E & 0xF) > 0xE)
                            {
                                set_auxiliary_carry_flag(true);
                            }
                            else
                            {
                                set_auxiliary_carry_flag(false);
                            }
                        }
                        else
                        {
                            if ((temp1 & 0xF) + (regs.E & 0xF) > 0xF)
                            {
                                set_auxiliary_carry_flag(true);
                            }
                            else
                            {
                                set_auxiliary_carry_flag(false);
                            }
                        }

                        regs.A += (byte)(regs.E + (regs.F >> 4 & 0x1));

                        normal_carry_flag_set((ushort)(temp1 + regs.E + (regs.F >> 4 & 0x1)));
                        zero_flag_set(regs.A);
                        set_negative_flag(false);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x8C:
                    {
                        //ADC_R(ref regs.H);
                        byte temp1 = regs.A;
                        if (check_carry_flag())
                        {
                            if ((temp1 & 0xF) + (regs.H & 0xF) > 0xE)
                            {
                                set_auxiliary_carry_flag(true);
                            }
                            else
                            {
                                set_auxiliary_carry_flag(false);
                            }
                        }
                        else
                        {
                            if ((temp1 & 0xF) + (regs.H & 0xF) > 0xF)
                            {
                                set_auxiliary_carry_flag(true);
                            }
                            else
                            {
                                set_auxiliary_carry_flag(false);
                            }
                        }

                        regs.A += (byte)(regs.H + (regs.F >> 4 & 0x1));

                        normal_carry_flag_set((ushort)(temp1 + regs.H + (regs.F >> 4 & 0x1)));
                        zero_flag_set(regs.A);
                        set_negative_flag(false);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x8D:
                    {
                        //ADC_R(ref regs.L);
                        byte temp1 = regs.A;
                        if (check_carry_flag())
                        {
                            if ((temp1 & 0xF) + (regs.L & 0xF) > 0xE)
                            {
                                set_auxiliary_carry_flag(true);
                            }
                            else
                            {
                                set_auxiliary_carry_flag(false);
                            }
                        }
                        else
                        {
                            if ((temp1 & 0xF) + (regs.L & 0xF) > 0xF)
                            {
                                set_auxiliary_carry_flag(true);
                            }
                            else
                            {
                                set_auxiliary_carry_flag(false);
                            }
                        }

                        regs.A += (byte)(regs.L + (regs.F >> 4 & 0x1));

                        normal_carry_flag_set((ushort)(temp1 + regs.L + (regs.F >> 4 & 0x1)));
                        zero_flag_set(regs.A);
                        set_negative_flag(false);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x8E:
                    {
                        //ADC_M(regs.HL);
                        byte temp1 = regs.A;

                        if (check_carry_flag())
                        {
                            if ((temp1 & 0xF) + ((byte)(_memory.read_byte(regs.HL)) & 0xF) > 0xE)
                            {
                                set_auxiliary_carry_flag(true);
                            }
                            else
                            {
                                set_auxiliary_carry_flag(false);
                            }
                        }
                        else
                        {
                            if ((temp1 & 0xF) + ((byte)(_memory.read_byte(regs.HL)) & 0xF) > 0xF)
                            {
                                set_auxiliary_carry_flag(true);
                            }
                            else
                            {
                                set_auxiliary_carry_flag(false);
                            }
                        }

                        regs.A += (byte)(_memory.read_byte(regs.HL) + (regs.F >> 4 & 0x1));

                        normal_carry_flag_set((ushort)(temp1 + _memory.read_byte(regs.HL) + (regs.F >> 4 & 0x1)));
                        zero_flag_set(regs.A);
                        set_negative_flag(false);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x8F:
                    {
                        //ADC_R(ref regs.A);
                        byte temp1 = regs.A;
                        if (check_carry_flag())
                        {
                            if ((temp1 & 0xF) + (regs.A & 0xF) > 0xE)
                            {
                                set_auxiliary_carry_flag(true);
                            }
                            else
                            {
                                set_auxiliary_carry_flag(false);
                            }
                        }
                        else
                        {
                            if ((temp1 & 0xF) + (regs.A & 0xF) > 0xF)
                            {
                                set_auxiliary_carry_flag(true);
                            }
                            else
                            {
                                set_auxiliary_carry_flag(false);
                            }
                        }

                        regs.A += (byte)(regs.A + (regs.F >> 4 & 0x1));

                        normal_carry_flag_set((ushort)(temp1 + temp1 + (regs.F >> 4 & 0x1)));
                        zero_flag_set(regs.A);
                        set_negative_flag(false);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x90:
                    {
                        //SUB_R(ref regs.B);
                        byte temp1 = (byte)(regs.B);

                        sub_auxiliary_carry_flag_set(regs.B);
                        sub_carry_flag_set(regs.A, temp1);

                        regs.A -= temp1;

                        zero_flag_set(regs.A);
                        set_negative_flag(true);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x91:
                    {
                        //SUB_R(ref regs.C);
                        byte temp1 = (byte)(regs.C);

                        sub_auxiliary_carry_flag_set(regs.C);
                        sub_carry_flag_set(regs.A, temp1);

                        regs.A -= temp1;

                        zero_flag_set(regs.A);
                        set_negative_flag(true);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x92:
                    {
                        //SUB_R(ref regs.D);
                        byte temp1 = (byte)(regs.D);

                        sub_auxiliary_carry_flag_set(regs.D);
                        sub_carry_flag_set(regs.A, temp1);

                        regs.A -= temp1;

                        zero_flag_set(regs.A);
                        set_negative_flag(true);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x93:
                    {
                        //SUB_R(ref regs.E);
                        byte temp1 = (byte)(regs.E);

                        sub_auxiliary_carry_flag_set(regs.E);
                        sub_carry_flag_set(regs.A, temp1);

                        regs.A -= temp1;

                        zero_flag_set(regs.A);
                        set_negative_flag(true);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x94:
                    {
                        //SUB_R(ref regs.H);
                        byte temp1 = (byte)(regs.H);

                        sub_auxiliary_carry_flag_set(regs.H);
                        sub_carry_flag_set(regs.A, temp1);

                        regs.A -= temp1;

                        zero_flag_set(regs.A);
                        set_negative_flag(true);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x95:
                    {
                        //SUB_R(ref regs.L);
                        byte temp1 = (byte)(regs.L);

                        sub_auxiliary_carry_flag_set(regs.L);
                        sub_carry_flag_set(regs.A, temp1);

                        regs.A -= temp1;

                        zero_flag_set(regs.A);
                        set_negative_flag(true);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x96:
                    {
                        //SUB_M(regs.HL);
                        byte temp1 = (byte)(_memory.read_byte(regs.HL));

                        sub_auxiliary_carry_flag_set(_memory.read_byte(regs.HL));
                        sub_carry_flag_set(regs.A, temp1);

                        regs.A -= temp1;

                        zero_flag_set(regs.A);
                        set_negative_flag(true);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x97:
                    {
                        //SUB_R(ref regs.A);
                        byte temp1 = (byte)(regs.A);

                        sub_auxiliary_carry_flag_set(regs.A);
                        sub_carry_flag_set(regs.A, temp1);

                        regs.A -= temp1;

                        zero_flag_set(regs.A);
                        set_negative_flag(true);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x98:
                    {
                        //SBC_R(ref regs.B);
                        byte temp1 = (byte)(regs.B + (regs.F >> 4 & 0x1));

                        if (check_carry_flag())
                        {
                            if ((regs.A & 0xF) - (regs.B & 0xF) < 0x1)
                            {
                                set_auxiliary_carry_flag(true);
                            }
                            else
                            {
                                set_auxiliary_carry_flag(false);
                            }

                            if (regs.A - regs.B < 0x1)
                            {
                                set_carry_flag(true);
                            }
                            else
                            {
                                set_carry_flag(false);
                            }
                        }
                        else
                        {
                            if ((regs.A & 0xF) < (regs.B & 0xF))
                            {
                                set_auxiliary_carry_flag(true);
                            }
                            else
                            {
                                set_auxiliary_carry_flag(false);
                            }

                            if (regs.A < regs.B)
                            {
                                set_carry_flag(true);
                            }
                            else
                            {
                                set_carry_flag(false);
                            }
                        }

                        regs.A -= temp1;

                        zero_flag_set(regs.A);
                        set_negative_flag(true);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x99:
                    {
                        //SBC_R(ref regs.C);
                        byte temp1 = (byte)(regs.C + (regs.F >> 4 & 0x1));

                        if (check_carry_flag())
                        {
                            if ((regs.A & 0xF) - (regs.C & 0xF) < 0x1)
                            {
                                set_auxiliary_carry_flag(true);
                            }
                            else
                            {
                                set_auxiliary_carry_flag(false);
                            }

                            if (regs.A - regs.C < 0x1)
                            {
                                set_carry_flag(true);
                            }
                            else
                            {
                                set_carry_flag(false);
                            }
                        }
                        else
                        {
                            if ((regs.A & 0xF) < (regs.C & 0xF))
                            {
                                set_auxiliary_carry_flag(true);
                            }
                            else
                            {
                                set_auxiliary_carry_flag(false);
                            }

                            if (regs.A < regs.C)
                            {
                                set_carry_flag(true);
                            }
                            else
                            {
                                set_carry_flag(false);
                            }
                        }

                        regs.A -= temp1;

                        zero_flag_set(regs.A);
                        set_negative_flag(true);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x9A:
                    {
                        //SBC_R(ref regs.D);
                        byte temp1 = (byte)(regs.D + (regs.F >> 4 & 0x1));

                        if (check_carry_flag())
                        {
                            if ((regs.A & 0xF) - (regs.D & 0xF) < 0x1)
                            {
                                set_auxiliary_carry_flag(true);
                            }
                            else
                            {
                                set_auxiliary_carry_flag(false);
                            }

                            if (regs.A - regs.D < 0x1)
                            {
                                set_carry_flag(true);
                            }
                            else
                            {
                                set_carry_flag(false);
                            }
                        }
                        else
                        {
                            if ((regs.A & 0xF) < (regs.D & 0xF))
                            {
                                set_auxiliary_carry_flag(true);
                            }
                            else
                            {
                                set_auxiliary_carry_flag(false);
                            }

                            if (regs.A < regs.D)
                            {
                                set_carry_flag(true);
                            }
                            else
                            {
                                set_carry_flag(false);
                            }
                        }

                        regs.A -= temp1;

                        zero_flag_set(regs.A);
                        set_negative_flag(true);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x9B:
                    {
                        //SBC_R(ref regs.E);
                        byte temp1 = (byte)(regs.E + (regs.F >> 4 & 0x1));

                        if (check_carry_flag())
                        {
                            if ((regs.A & 0xF) - (regs.E & 0xF) < 0x1)
                            {
                                set_auxiliary_carry_flag(true);
                            }
                            else
                            {
                                set_auxiliary_carry_flag(false);
                            }

                            if (regs.A - regs.E < 0x1)
                            {
                                set_carry_flag(true);
                            }
                            else
                            {
                                set_carry_flag(false);
                            }
                        }
                        else
                        {
                            if ((regs.A & 0xF) < (regs.E & 0xF))
                            {
                                set_auxiliary_carry_flag(true);
                            }
                            else
                            {
                                set_auxiliary_carry_flag(false);
                            }

                            if (regs.A < regs.E)
                            {
                                set_carry_flag(true);
                            }
                            else
                            {
                                set_carry_flag(false);
                            }
                        }

                        regs.A -= temp1;

                        zero_flag_set(regs.A);
                        set_negative_flag(true);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x9C:
                    {
                        //SBC_R(ref regs.H);
                        byte temp1 = (byte)(regs.H + (regs.F >> 4 & 0x1));

                        if (check_carry_flag())
                        {
                            if ((regs.A & 0xF) - (regs.H & 0xF) < 0x1)
                            {
                                set_auxiliary_carry_flag(true);
                            }
                            else
                            {
                                set_auxiliary_carry_flag(false);
                            }

                            if (regs.A - regs.H < 0x1)
                            {
                                set_carry_flag(true);
                            }
                            else
                            {
                                set_carry_flag(false);
                            }
                        }
                        else
                        {
                            if ((regs.A & 0xF) < (regs.H & 0xF))
                            {
                                set_auxiliary_carry_flag(true);
                            }
                            else
                            {
                                set_auxiliary_carry_flag(false);
                            }

                            if (regs.A < regs.H)
                            {
                                set_carry_flag(true);
                            }
                            else
                            {
                                set_carry_flag(false);
                            }
                        }

                        regs.A -= temp1;

                        zero_flag_set(regs.A);
                        set_negative_flag(true);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x9D:
                    {
                        //SBC_R(ref regs.L);
                        byte temp1 = (byte)(regs.L + (regs.F >> 4 & 0x1));

                        if (check_carry_flag())
                        {
                            if ((regs.A & 0xF) - (regs.L & 0xF) < 0x1)
                            {
                                set_auxiliary_carry_flag(true);
                            }
                            else
                            {
                                set_auxiliary_carry_flag(false);
                            }

                            if (regs.A - regs.L < 0x1)
                            {
                                set_carry_flag(true);
                            }
                            else
                            {
                                set_carry_flag(false);
                            }
                        }
                        else
                        {
                            if ((regs.A & 0xF) < (regs.L & 0xF))
                            {
                                set_auxiliary_carry_flag(true);
                            }
                            else
                            {
                                set_auxiliary_carry_flag(false);
                            }

                            if (regs.A < regs.L)
                            {
                                set_carry_flag(true);
                            }
                            else
                            {
                                set_carry_flag(false);
                            }
                        }

                        regs.A -= temp1;

                        zero_flag_set(regs.A);
                        set_negative_flag(true);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x9E:
                    {
                        //SBC_M(regs.HL);
                        byte temp1 = (byte)(_memory.read_byte(regs.HL) + (regs.F >> 4 & 0x1));

                        if (check_carry_flag())
                        {
                            if ((regs.A & 0xF) - ((byte)(_memory.read_byte(regs.HL)) & 0xF) < 0x1)
                            {
                                set_auxiliary_carry_flag(true);
                            }
                            else
                            {
                                set_auxiliary_carry_flag(false);
                            }

                            if (regs.A - (byte)(_memory.read_byte(regs.HL)) < 0x1)
                            {
                                set_carry_flag(true);
                            }
                            else
                            {
                                set_carry_flag(false);
                            }
                        }
                        else
                        {
                            if ((regs.A & 0xF) < ((byte)(_memory.read_byte(regs.HL)) & 0xF))
                            {
                                set_auxiliary_carry_flag(true);
                            }
                            else
                            {
                                set_auxiliary_carry_flag(false);
                            }

                            if (regs.A < (byte)(_memory.read_byte(regs.HL)))
                            {
                                set_carry_flag(true);
                            }
                            else
                            {
                                set_carry_flag(false);
                            }
                        }

                        regs.A -= temp1;

                        zero_flag_set(regs.A);
                        set_negative_flag(true);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x9F:
                    {
                        //SBC_R(ref regs.A);
                        byte temp1 = (byte)(regs.A + (regs.F >> 4 & 0x1));

                        if (check_carry_flag())
                        {
                            if ((regs.A & 0xF) - (regs.A & 0xF) < 0x1)
                            {
                                set_auxiliary_carry_flag(true);
                            }
                            else
                            {
                                set_auxiliary_carry_flag(false);
                            }

                            if (regs.A - regs.A < 0x1)
                            {
                                set_carry_flag(true);
                            }
                            else
                            {
                                set_carry_flag(false);
                            }
                        }
                        else
                        {
                            if ((regs.A & 0xF) < (regs.A & 0xF))
                            {
                                set_auxiliary_carry_flag(true);
                            }
                            else
                            {
                                set_auxiliary_carry_flag(false);
                            }

                            if (regs.A < regs.A)
                            {
                                set_carry_flag(true);
                            }
                            else
                            {
                                set_carry_flag(false);
                            }
                        }

                        regs.A -= temp1;

                        zero_flag_set(regs.A);
                        set_negative_flag(true);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xA0:
                    {
                        //AND_R(ref regs.B);
                        regs.A &= regs.B;

                        set_auxiliary_carry_flag(true);
                        set_carry_flag(false);
                        zero_flag_set(regs.A);
                        set_negative_flag(false);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xA1:
                    {
                        //AND_R(ref regs.C);
                        regs.A &= regs.C;

                        set_auxiliary_carry_flag(true);
                        set_carry_flag(false);
                        zero_flag_set(regs.A);
                        set_negative_flag(false);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xA2:
                    {
                        //AND_R(ref regs.D);
                        regs.A &= regs.D;

                        set_auxiliary_carry_flag(true);
                        set_carry_flag(false);
                        zero_flag_set(regs.A);
                        set_negative_flag(false);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xA3:
                    {
                        //AND_R(ref regs.E);
                        regs.A &= regs.E;

                        set_auxiliary_carry_flag(true);
                        set_carry_flag(false);
                        zero_flag_set(regs.A);
                        set_negative_flag(false);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xA4:
                    {
                        //AND_R(ref regs.H);
                        regs.A &= regs.H;

                        set_auxiliary_carry_flag(true);
                        set_carry_flag(false);
                        zero_flag_set(regs.A);
                        set_negative_flag(false);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xA5:
                    {
                        //AND_R(ref regs.L);
                        regs.A &= regs.L;

                        set_auxiliary_carry_flag(true);
                        set_carry_flag(false);
                        zero_flag_set(regs.A);
                        set_negative_flag(false);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xA6:
                    {
                        //AND_M(regs.HL);
                        regs.A &= _memory.read_byte(regs.HL);

                        set_auxiliary_carry_flag(true);
                        set_carry_flag(false);
                        zero_flag_set(regs.A);
                        set_negative_flag(false);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xA7:
                    {
                        //AND_R(ref regs.A);
                        regs.A &= regs.A;

                        set_auxiliary_carry_flag(true);
                        set_carry_flag(false);
                        zero_flag_set(regs.A);
                        set_negative_flag(false);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xA8:
                    {
                        //XOR_R(ref regs.B);
                        regs.A ^= regs.B;

                        set_auxiliary_carry_flag(false);
                        set_carry_flag(false);
                        set_negative_flag(false);
                        zero_flag_set(regs.A);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xA9:
                    {
                        //XOR_R(ref regs.C);
                        regs.A ^= regs.C;

                        set_auxiliary_carry_flag(false);
                        set_carry_flag(false);
                        set_negative_flag(false);
                        zero_flag_set(regs.A);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xAA:
                    {
                        //XOR_R(ref regs.D);
                        regs.A ^= regs.D;

                        set_auxiliary_carry_flag(false);
                        set_carry_flag(false);
                        set_negative_flag(false);
                        zero_flag_set(regs.A);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xAB:
                    {
                        //XOR_R(ref regs.E);
                        regs.A ^= regs.E;

                        set_auxiliary_carry_flag(false);
                        set_carry_flag(false);
                        set_negative_flag(false);
                        zero_flag_set(regs.A);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xAC:
                    {
                        //XOR_R(ref regs.H);
                        regs.A ^= regs.H;

                        set_auxiliary_carry_flag(false);
                        set_carry_flag(false);
                        set_negative_flag(false);
                        zero_flag_set(regs.A);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xAD:
                    {
                        //XOR_R(ref regs.L);
                        regs.A ^= regs.L;

                        set_auxiliary_carry_flag(false);
                        set_carry_flag(false);
                        set_negative_flag(false);
                        zero_flag_set(regs.A);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xAE:
                    {
                        //XOR_M(regs.HL);
                        regs.A ^= _memory.read_byte(regs.HL);

                        set_auxiliary_carry_flag(false);
                        set_carry_flag(false);
                        set_negative_flag(false);
                        zero_flag_set(regs.A);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xAF:
                    {
                        //XOR_R(ref regs.A);
                        regs.A ^= regs.A;

                        set_auxiliary_carry_flag(false);
                        set_carry_flag(false);
                        set_negative_flag(false);
                        zero_flag_set(regs.A);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xB0:
                    {
                        //OR_R(ref regs.B);
                        regs.A |= regs.B;

                        set_auxiliary_carry_flag(false);
                        set_carry_flag(false);
                        set_negative_flag(false);
                        zero_flag_set(regs.A);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xB1:
                    {
                        //OR_R(ref regs.C);
                        regs.A |= regs.C;

                        set_auxiliary_carry_flag(false);
                        set_carry_flag(false);
                        set_negative_flag(false);
                        zero_flag_set(regs.A);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xB2:
                    {
                        //OR_R(ref regs.D);
                        regs.A |= regs.D;

                        set_auxiliary_carry_flag(false);
                        set_carry_flag(false);
                        set_negative_flag(false);
                        zero_flag_set(regs.A);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xB3:
                    {
                        //OR_R(ref regs.E);
                        regs.A |= regs.E;

                        set_auxiliary_carry_flag(false);
                        set_carry_flag(false);
                        set_negative_flag(false);
                        zero_flag_set(regs.A);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xB4:
                    {
                        //OR_R(ref regs.H);
                        regs.A |= regs.H;

                        set_auxiliary_carry_flag(false);
                        set_carry_flag(false);
                        set_negative_flag(false);
                        zero_flag_set(regs.A);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xB5:
                    {
                        //OR_R(ref regs.L);
                        regs.A |= regs.L;

                        set_auxiliary_carry_flag(false);
                        set_carry_flag(false);
                        set_negative_flag(false);
                        zero_flag_set(regs.A);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xB6:
                    {
                        //OR_M(regs.HL);
                        regs.A |= _memory.read_byte(regs.HL);

                        set_auxiliary_carry_flag(false);
                        set_carry_flag(false);
                        set_negative_flag(false);
                        zero_flag_set(regs.A);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xB7:
                    {
                        //OR_R(ref regs.A);
                        regs.A |= regs.A;

                        set_auxiliary_carry_flag(false);
                        set_carry_flag(false);
                        set_negative_flag(false);
                        zero_flag_set(regs.A);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xB8:
                    {
                        //CP_R(ref regs.B);
                        byte temp1 = (byte)(-regs.B);
                        byte temp2 = (byte)(regs.A + temp1);

                        sub_auxiliary_carry_flag_set(regs.B);
                        sub_carry_flag_set(regs.A, regs.B);
                        zero_flag_set(temp2);
                        set_negative_flag(true);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xB9:
                    {
                        //CP_R(ref regs.C);
                        byte temp1 = (byte)(-regs.C);
                        byte temp2 = (byte)(regs.A + temp1);

                        sub_auxiliary_carry_flag_set(regs.C);
                        sub_carry_flag_set(regs.A, regs.C);
                        zero_flag_set(temp2);
                        set_negative_flag(true);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xBA:
                    {
                        //CP_R(ref regs.D);
                        byte temp1 = (byte)(-regs.D);
                        byte temp2 = (byte)(regs.A + temp1);

                        sub_auxiliary_carry_flag_set(regs.D);
                        sub_carry_flag_set(regs.A, regs.D);
                        zero_flag_set(temp2);
                        set_negative_flag(true);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xBB:
                    {
                        //CP_R(ref regs.E);
                        byte temp1 = (byte)(-regs.E);
                        byte temp2 = (byte)(regs.A + temp1);

                        sub_auxiliary_carry_flag_set(regs.E);
                        sub_carry_flag_set(regs.A, regs.E);
                        zero_flag_set(temp2);
                        set_negative_flag(true);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xBC:
                    {
                        //CP_R(ref regs.H);
                        byte temp1 = (byte)(-regs.H);
                        byte temp2 = (byte)(regs.A + temp1);

                        sub_auxiliary_carry_flag_set(regs.H);
                        sub_carry_flag_set(regs.A, regs.H);
                        zero_flag_set(temp2);
                        set_negative_flag(true);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xBD:
                    {
                        //CP_R(ref regs.L);
                        byte temp1 = (byte)(-regs.L);
                        byte temp2 = (byte)(regs.A + temp1);

                        sub_auxiliary_carry_flag_set(regs.L);
                        sub_carry_flag_set(regs.A, regs.L);
                        zero_flag_set(temp2);
                        set_negative_flag(true);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xBE:
                    {
                        //CP_M(regs.HL);
                        byte temp1 = (byte)(-_memory.read_byte(regs.HL));
                        byte temp2 = (byte)(regs.A + temp1);

                        sub_auxiliary_carry_flag_set(_memory.read_byte(regs.HL));
                        sub_carry_flag_set(regs.A, _memory.read_byte(regs.HL));
                        zero_flag_set(temp2);
                        set_negative_flag(true);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xBF:
                    {
                        //CP_R(ref regs.A);
                        byte temp1 = (byte)(-regs.A);
                        byte temp2 = (byte)(regs.A + temp1);

                        sub_auxiliary_carry_flag_set(regs.A);
                        sub_carry_flag_set(regs.A, regs.A);
                        zero_flag_set(temp2);
                        set_negative_flag(true);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xC0:
                    {
                        //RET_NZ();
                        if (!check_zero_flag())
                        {
                            pc = (ushort)((_memory.read_byte((ushort)(sp + 1)) << 8) | (_memory.read_byte((ushort)(sp))));
                            sp += 2;
                            cycles -= 12;
                            cyclesThisOp += 12;
                            next_opcode = _memory.read_byte(pc);
                        }
                        else
                        {
                            pc++;
                            next_opcode = _memory.read_byte(pc);
                        }
                        return;
                    }
                case 0xC1:
                    {
                        //POP_RR(ref regs.B, ref regs.C);
                        regs.C = _memory.read_byte((ushort)(sp));
                        regs.B = _memory.read_byte((ushort)(sp + 1));
                        sp += 2;
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xC2:
                    {
                        //JP_NZ_NN();
                        if (!check_zero_flag())
                        {
                            pc = (ushort)((_memory.read_byte((ushort)(pc + 2)) << 8) | _memory.read_byte((ushort)(pc + 1)));
                            cycles -= 4;
                            cyclesThisOp += 4;
                            next_opcode = _memory.read_byte(pc);
                        }
                        else
                        {
                            pc += 3;
                            next_opcode = _memory.read_byte(pc);
                        }
                        return;
                    }
                case 0xC3:
                    {
                        //JP_NN();
                        pc = (ushort)((_memory.read_byte((ushort)(pc + 2)) << 8) | _memory.read_byte((ushort)(pc + 1)));
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xC4:
                    {
                        //CALL_NZ_NN();
                        pc += 3;
                        next_opcode = _memory.read_byte(pc);

                        if (!check_zero_flag())
                        {
                            _memory.write_byte((ushort)(sp - 2), (byte)(pc & 0xFF));
                            _memory.write_byte((ushort)(sp - 1), (byte)(pc >> 8 & 0xFF));
                            sp -= 2;
                            pc = (ushort)((_memory.read_byte((ushort)(pc - 1)) << 8) | (_memory.read_byte((ushort)(pc - 2))));
                            cycles -= 12;
                            cyclesThisOp += 12;
                            next_opcode = _memory.read_byte(pc);
                        }
                        return;
                    }
                case 0xC5:
                    {
                        //PUSH_RR(ref regs.B, ref regs.C);
                        if (regs.BC == 0x1200)
                        {
                            regs.BC = regs.BC;
                        }
                        _memory.write_byte((ushort)(sp - 2), regs.C);
                        _memory.write_byte((ushort)(sp - 1), regs.B);
                        sp -= 2;
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xC6:
                    {
                        //ADD_M2((ushort)(pc + 1));
                        auxiliary_carry_flag_set(regs.A, _memory.read_byte((ushort)(pc + 1)));
                        normal_carry_flag_set((ushort)(regs.A + _memory.read_byte((ushort)(pc + 1))));

                        regs.A += _memory.read_byte((ushort)(pc + 1));

                        set_negative_flag(false);
                        zero_flag_set(regs.A);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xC7:
                    {
                        //RST_N(0x0);
                        pc++;
                        _memory.write_byte((ushort)(sp - 1), (byte)(pc >> 8 & 0xFF));
                        _memory.write_byte((ushort)(sp - 2), (byte)(pc & 0xFF));
                        sp -= 2;

                        pc = (ushort)(0x0000 + 0x0);
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xC8:
                    {
                        //RET_Z();
                        if (check_zero_flag())
                        {
                            pc = (ushort)((_memory.read_byte((ushort)(sp + 1)) << 8) | (_memory.read_byte((ushort)(sp))));
                            sp += 2;
                            cycles -= 12;
                            cyclesThisOp += 12;
                            next_opcode = _memory.read_byte(pc);
                        }
                        else
                        {
                            pc++;
                            next_opcode = _memory.read_byte(pc);
                        }
                        return;
                    }
                case 0xC9:
                    {
                        //RET();
                        pc = (ushort)((_memory.read_byte((ushort)(sp + 1)) << 8) | (_memory.read_byte((ushort)(sp))));
                        sp += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xCA:
                    {
                        //JP_Z_NN();
                        if (check_zero_flag())
                        {
                            pc = (ushort)((_memory.read_byte((ushort)(pc + 2)) << 8) | _memory.read_byte((ushort)(pc + 1)));
                            cycles -= 4;
                            cyclesThisOp += 4;
                            next_opcode = _memory.read_byte(pc);
                        }
                        else
                        {
                            pc += 3;
                            next_opcode = _memory.read_byte(pc);
                        }
                        return;
                    }
                case 0xCB:
                    {
                        Decode_Opcode_CB(_memory.read_byte((ushort)(pc + 1)));
                        return;
                    }
                case 0xCC:
                    {
                        //CALL_Z_NN();
                        pc += 3;
                        next_opcode = _memory.read_byte(pc);

                        if (check_zero_flag())
                        {
                            _memory.write_byte((ushort)(sp - 2), (byte)(pc & 0xFF));
                            _memory.write_byte((ushort)(sp - 1), (byte)(pc >> 8 & 0xFF));
                            sp -= 2;
                            pc = (ushort)((_memory.read_byte((ushort)(pc - 1)) << 8) | (_memory.read_byte((ushort)(pc - 2))));
                            cycles -= 12;
                            cyclesThisOp += 12;
                            next_opcode = _memory.read_byte(pc);
                        }
                        return;
                    }
                case 0xCD:
                    {
                        //CALL_NN();
                        pc += 3;
                        _memory.write_byte((ushort)(sp - 2), (byte)(pc & 0xFF));
                        _memory.write_byte((ushort)(sp - 1), (byte)(pc >> 8 & 0xFF));
                        sp -= 2;
                        pc = (ushort)((_memory.read_byte((ushort)(pc - 1)) << 8) | (_memory.read_byte((ushort)(pc - 2))));
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xCE:
                    {
                        //ADC_M2((ushort)(pc + 1));
                        byte temp1 = regs.A;
                        if (check_carry_flag())
                        {
                            if ((temp1 & 0xF) + ((byte)(_memory.read_byte((ushort)(pc + 1))) & 0xF) > 0xE)
                            {
                                set_auxiliary_carry_flag(true);
                            }
                            else
                            {
                                set_auxiliary_carry_flag(false);
                            }
                        }
                        else
                        {
                            if ((temp1 & 0xF) + ((byte)(_memory.read_byte((ushort)(pc + 1))) & 0xF) > 0xF)
                            {
                                set_auxiliary_carry_flag(true);
                            }
                            else
                            {
                                set_auxiliary_carry_flag(false);
                            }
                        }

                        regs.A += (byte)(_memory.read_byte((ushort)(pc + 1)) + (regs.F >> 4 & 0x1));

                        normal_carry_flag_set((ushort)(temp1 + _memory.read_byte((ushort)(pc + 1)) + (regs.F >> 4 & 0x1)));
                        zero_flag_set(regs.A);
                        set_negative_flag(false);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xCF:
                    {
                        //RST_N(0x08);
                        pc++;
                        _memory.write_byte((ushort)(sp - 1), (byte)(pc >> 8 & 0xFF));
                        _memory.write_byte((ushort)(sp - 2), (byte)(pc & 0xFF));
                        sp -= 2;

                        pc = (ushort)(0x0000 + 0x08);
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xD0:
                    {
                        //RET_NC();
                        if (!check_carry_flag())
                        {
                            pc = (ushort)((_memory.read_byte((ushort)(sp + 1)) << 8) | (_memory.read_byte((ushort)(sp))));
                            sp += 2;
                            cycles -= 12;
                            cyclesThisOp += 12;
                            next_opcode = _memory.read_byte(pc);
                        }
                        else
                        {
                            pc++;
                            next_opcode = _memory.read_byte(pc);
                        }
                        return;
                    }
                case 0xD1:
                    {
                        //POP_RR(ref regs.D, ref regs.E);
                        regs.E = _memory.read_byte((ushort)(sp));
                        regs.D = _memory.read_byte((ushort)(sp + 1));
                        sp += 2;
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xD2:
                    {
                        //JP_NC_NN();
                        if (!check_carry_flag())
                        {
                            pc = (ushort)((_memory.read_byte((ushort)(pc + 2)) << 8) | _memory.read_byte((ushort)(pc + 1)));
                            cycles -= 4;
                            cyclesThisOp += 4;
                            next_opcode = _memory.read_byte(pc);
                        }
                        else
                        {
                            pc += 3;
                            next_opcode = _memory.read_byte(pc);
                        }
                        return;
                    }
                case 0xD4:
                    {
                        //CALL_NC_NN();
                        pc += 3;
                        next_opcode = _memory.read_byte(pc);

                        if (!check_carry_flag())
                        {
                            _memory.write_byte((ushort)(sp - 2), (byte)(pc & 0xFF));
                            _memory.write_byte((ushort)(sp - 1), (byte)(pc >> 8 & 0xFF));
                            sp -= 2;
                            pc = (ushort)((_memory.read_byte((ushort)(pc - 1)) << 8) | (_memory.read_byte((ushort)(pc - 2))));
                            cycles -= 12;
                            cyclesThisOp += 12;
                            next_opcode = _memory.read_byte(pc);
                        }
                        return;
                    }
                case 0xD5:
                    {
                        //PUSH_RR(ref regs.D, ref regs.E);
                        _memory.write_byte((ushort)(sp - 2), regs.E);
                        _memory.write_byte((ushort)(sp - 1), regs.D);
                        sp -= 2;
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xD6:
                    {
                        //SUB_M2((ushort)(pc + 1));
                        byte temp1 = (byte)(_memory.read_byte((ushort)(pc + 1)));

                        sub_auxiliary_carry_flag_set(_memory.read_byte((ushort)(pc + 1)));
                        sub_carry_flag_set(regs.A, temp1);

                        regs.A -= temp1;

                        zero_flag_set(regs.A);
                        set_negative_flag(true);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xD7:
                    {
                        //RST_N(0x10);
                        pc++;
                        _memory.write_byte((ushort)(sp - 1), (byte)(pc >> 8 & 0xFF));
                        _memory.write_byte((ushort)(sp - 2), (byte)(pc & 0xFF));
                        sp -= 2;

                        pc = (ushort)(0x0000 + 0x10);
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xD8:
                    {
                        //RET_C();
                        if (check_carry_flag())
                        {
                            pc = (ushort)((_memory.read_byte((ushort)(sp + 1)) << 8) | (_memory.read_byte((ushort)(sp))));
                            sp += 2;
                            cycles -= 12;
                            cyclesThisOp += 12;
                            next_opcode = _memory.read_byte(pc);
                        }
                        else
                        {
                            pc++;
                            next_opcode = _memory.read_byte(pc);
                        }
                        return;
                    }
                case 0xD9:
                    {
                        //RETI();
                        pc = (ushort)((_memory.read_byte((ushort)(sp + 1)) << 8) | (_memory.read_byte((ushort)(sp))));
                        sp += 2;
                        IME = true;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xDA:
                    {
                        //JP_C_NN();
                        if (check_carry_flag())
                        {
                            pc = (ushort)((_memory.read_byte((ushort)(pc + 2)) << 8) | _memory.read_byte((ushort)(pc + 1)));
                            cycles -= 4;
                            cyclesThisOp += 4;
                            next_opcode = _memory.read_byte(pc);
                        }
                        else
                        {
                            pc += 3;
                            next_opcode = _memory.read_byte(pc);
                        }
                        return;
                    }
                case 0xDC:
                    {
                        //CALL_C_NN();
                        pc += 3;
                        next_opcode = _memory.read_byte(pc);

                        if (check_carry_flag())
                        {
                            _memory.write_byte((ushort)(sp - 2), (byte)(pc & 0xFF));
                            _memory.write_byte((ushort)(sp - 1), (byte)(pc >> 8 & 0xFF));
                            sp -= 2;
                            pc = (ushort)((_memory.read_byte((ushort)(pc - 1)) << 8) | (_memory.read_byte((ushort)(pc - 2))));
                            cycles -= 12;
                            cyclesThisOp += 12;
                            next_opcode = _memory.read_byte(pc);
                        }
                        return;
                    }
                case 0xDE:
                    {
                        //SBC_M2((ushort)(pc + 1));
                        byte temp1 = (byte)(_memory.read_byte((ushort)(pc + 1)) + (regs.F >> 4 & 0x1));

                        if (check_carry_flag())
                        {
                            if ((regs.A & 0xF) - ((byte)(_memory.read_byte((ushort)(pc + 1))) & 0xF) < 0x1)
                            {
                                set_auxiliary_carry_flag(true);
                            }
                            else
                            {
                                set_auxiliary_carry_flag(false);
                            }

                            if (regs.A - (byte)(_memory.read_byte((ushort)(pc + 1))) < 0x1)
                            {
                                set_carry_flag(true);
                            }
                            else
                            {
                                set_carry_flag(false);
                            }
                        }
                        else
                        {
                            if ((regs.A & 0xF) < ((byte)(_memory.read_byte((ushort)(pc + 1))) & 0xF))
                            {
                                set_auxiliary_carry_flag(true);
                            }
                            else
                            {
                                set_auxiliary_carry_flag(false);
                            }

                            if (regs.A < (byte)(_memory.read_byte((ushort)(pc + 1))))
                            {
                                set_carry_flag(true);
                            }
                            else
                            {
                                set_carry_flag(false);
                            }
                        }

                        regs.A -= temp1;

                        zero_flag_set(regs.A);
                        set_negative_flag(true);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xDF:
                    {
                        //RST_N(0x18);
                        pc++;
                        _memory.write_byte((ushort)(sp - 1), (byte)(pc >> 8 & 0xFF));
                        _memory.write_byte((ushort)(sp - 2), (byte)(pc & 0xFF));
                        sp -= 2;

                        pc = (ushort)(0x0000 + 0x18);
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xE0:
                    {
                        //LD_R_MR(ref regs.A);
                        _memory.write_byte((ushort)(0xFF00 + _memory.read_byte((ushort)(pc + 1))), regs.A);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xE1:
                    {
                        //POP_RR(ref regs.H, ref regs.L);
                        regs.L = _memory.read_byte((ushort)(sp));
                        regs.H = _memory.read_byte((ushort)(sp + 1));
                        sp += 2;
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xE2:
                    {
                        //LD_R_MR(ref regs.A, ref regs.C);
                        _memory.write_byte((ushort)(0xFF00 + regs.C), regs.A);
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xE5:
                    {
                        //PUSH_RR(ref regs.H, ref regs.L);
                        _memory.write_byte((ushort)(sp - 2), regs.L);
                        _memory.write_byte((ushort)(sp - 1), regs.H);
                        sp -= 2;
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xE6:
                    {
                        //AND_M2((ushort)(pc + 1));
                        regs.A &= _memory.read_byte((ushort)(pc + 1));

                        set_auxiliary_carry_flag(true);
                        set_carry_flag(false);
                        zero_flag_set(regs.A);
                        set_negative_flag(false);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xE7:
                    {
                        //RST_N(0x20);
                        pc++;
                        _memory.write_byte((ushort)(sp - 1), (byte)(pc >> 8 & 0xFF));
                        _memory.write_byte((ushort)(sp - 2), (byte)(pc & 0xFF));
                        sp -= 2;

                        pc = (ushort)(0x0000 + 0x20);
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xE8:
                    {
                        //ADD_SP_N();
                        sbyte temp = (sbyte)(_memory.read_byte((ushort)(pc + 1)));
                        ushort temp_s = sp;
                        set_zero_flag(false);
                        set_negative_flag(false);
                        if (((sp ^ temp ^ (sp + temp)) & 0x100) != 0)
                        {
                            set_carry_flag(true);
                        }
                        else
                        {
                            set_carry_flag(false);
                        }
                        ushort htest = (ushort)(temp_s & 0xF);
                        htest += (ushort)(temp & 0xF);
                        if(htest > 0xF)
                        {
                            set_auxiliary_carry_flag(true);
                        }
                        else
                        {
                            set_auxiliary_carry_flag(false);
                        }

                        sp = (ushort)(sp + temp);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xE9:
                    {
                        //JP_HL();
                        pc = regs.HL;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xEA:
                    {
                        //LD_R_NN(ref regs.A);
                        _memory.write_byte(_memory.read_ushort((ushort)(pc + 1)), regs.A);
                        pc += 3;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xEE:
                    {
                        //XOR_M2((ushort)(pc + 1));
                        regs.A ^= _memory.read_byte((ushort)(pc + 1));

                        set_auxiliary_carry_flag(false);
                        set_carry_flag(false);
                        set_negative_flag(false);
                        zero_flag_set(regs.A);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xEF:
                    {
                        //RST_N(0x28);
                        pc++;
                        _memory.write_byte((ushort)(sp - 1), (byte)(pc >> 8 & 0xFF));
                        _memory.write_byte((ushort)(sp - 2), (byte)(pc & 0xFF));
                        sp -= 2;

                        pc = (ushort)(0x0000 + 0x28);
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xF0:
                    {
                        //LD_MR_R(ref regs.A);
                        regs.A = _memory.read_byte((ushort)(0xFF00 + _memory.read_byte((ushort)(pc + 1))));
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xF1:
                    {
                        //POP_RR(ref regs.A, ref regs.F);
                        //Write_Instruction_Log();
                        regs.F = _memory.read_byte((ushort)(sp));
                        regs.A = _memory.read_byte((ushort)(sp + 1));
                        sp += 2;
                        //Write_Instruction_Log();
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xF2:
                    {
                        //LD_MR_R(ref regs.C, ref regs.A);
                        regs.A = _memory.read_byte((ushort)(0xFF00 + regs.C));
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xF3:
                    {
                        //DI();
                        disable_int = 2;
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xF5:
                    {
                        //PUSH_RR(ref regs.A, ref regs.F);
                        _memory.write_byte((ushort)(sp - 1), regs.A);
                        _memory.write_byte((ushort)(sp - 2), regs.F);
                        sp -= 2;
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xF6:
                    {
                        //OR_M2((ushort)(pc + 1));
                        regs.A |= _memory.read_byte((ushort)(pc + 1));

                        set_auxiliary_carry_flag(false);
                        set_carry_flag(false);
                        set_negative_flag(false);
                        zero_flag_set(regs.A);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xF7:
                    {
                        //RST_N(0x30);
                        pc++;
                        _memory.write_byte((ushort)(sp - 1), (byte)(pc >> 8 & 0xFF));
                        _memory.write_byte((ushort)(sp - 2), (byte)(pc & 0xFF));
                        sp -= 2;

                        pc = (ushort)(0x0000 + 0x30);
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xF8:
                    {
                        //LD_RR_RR(ref sp, ref regs.HL);
                        sbyte temp = (sbyte)(_memory.read_byte((ushort)(pc + 1)));
                        ushort temp_s = sp;
                        set_zero_flag(false);
                        set_negative_flag(false);
                        if (((sp ^ temp ^ (sp + temp)) & 0x100) != 0)
                        {
                            set_carry_flag(true);
                        }
                        else
                        {
                            set_carry_flag(false);
                        }
                        ushort htest = (ushort)(temp_s & 0xF);
                        htest += (ushort)(temp & 0xF);
                        if(htest > 0xF)
                        {
                            set_auxiliary_carry_flag(true);
                        }
                        else
                        {
                            set_auxiliary_carry_flag(false);
                        }

                        regs.HL = (ushort)(sp + temp);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xF9:
                    {
                        //LD_RR_RR(ref regs.HL, ref sp);
                        sp = regs.HL;
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xFA:
                    {
                        //LD_NN_R(ref regs.A);
                        regs.A = _memory.read_byte(_memory.read_ushort((ushort)(pc + 1)));
                        pc += 3;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xFB:
                    {
                        //EI();
                        enable_int = 2;
                        pc++;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xFE:
                    {
                        //CP_M2((ushort)(pc + 1));
                        byte temp1 = (byte)(-_memory.read_byte((ushort)(pc + 1)));
                        byte temp2 = (byte)(regs.A + temp1);

                        sub_auxiliary_carry_flag_set(_memory.read_byte((ushort)(pc + 1)));
                        sub_carry_flag_set(regs.A, _memory.read_byte((ushort)(pc + 1)));
                        zero_flag_set(temp2);
                        set_negative_flag(true);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xFF:
                    {
                        //RST_N(0x38);
                        pc++;
                        _memory.write_byte((ushort)(sp - 1), (byte)(pc >> 8 & 0xFF));
                        _memory.write_byte((ushort)(sp - 2), (byte)(pc & 0xFF));
                        sp -= 2;

                        pc = (ushort)(0x0000 + 0x38);
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                default:
                    {
                        pc++;
                        return;
                    }
            }
        }
        #endregion

        #region decode opcode CB
        private void Decode_Opcode_CB(byte opcode2)
        {
            cyclesThisOp = cycles_lookup_CB[opcode2];
            cycles -= cyclesThisOp;
            cycles_lookup_CB_log2[opcode2]++;

            switch (opcode2)
            {
                case 0x00:
                    {
                        //RLC_R(ref regs.B);
                        set_auxiliary_carry_flag(false);
                        set_negative_flag(false);

                        set_carry_flag((regs.B >> 7 & 0x1) != 0);
                        regs.B = (byte)(regs.B << 1 | (regs.B >> 7 & 0x1));

                        zero_flag_set(regs.B);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x01:
                    {
                        //RLC_R(ref regs.C);
                        set_auxiliary_carry_flag(false);
                        set_negative_flag(false);

                        set_carry_flag((regs.C >> 7 & 0x1) != 0);
                        regs.C = (byte)(regs.C << 1 | (regs.C >> 7 & 0x1));

                        zero_flag_set(regs.C);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x02:
                    {
                        //RLC_R(ref regs.D);
                        set_auxiliary_carry_flag(false);
                        set_negative_flag(false);

                        set_carry_flag((regs.D >> 7 & 0x1) != 0);
                        regs.D = (byte)(regs.D << 1 | (regs.D >> 7 & 0x1));

                        zero_flag_set(regs.D);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x03:
                    {
                        //RLC_R(ref regs.E);
                        set_auxiliary_carry_flag(false);
                        set_negative_flag(false);

                        set_carry_flag((regs.E >> 7 & 0x1) != 0);
                        regs.E = (byte)(regs.E << 1 | (regs.E >> 7 & 0x1));

                        zero_flag_set(regs.E);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x04:
                    {
                        //RLC_R(ref regs.H);
                        set_auxiliary_carry_flag(false);
                        set_negative_flag(false);

                        set_carry_flag((regs.H >> 7 & 0x1) != 0);
                        regs.H = (byte)(regs.H << 1 | (regs.H >> 7 & 0x1));

                        zero_flag_set(regs.H);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x05:
                    {
                        //RLC_R(ref regs.L);
                        set_auxiliary_carry_flag(false);
                        set_negative_flag(false);

                        set_carry_flag((regs.L >> 7 & 0x1) != 0);
                        regs.L = (byte)(regs.L << 1 | (regs.L >> 7 & 0x1));

                        zero_flag_set(regs.L);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x06:
                    {
                        //RLC_M(regs.HL);
                        set_auxiliary_carry_flag(false);
                        set_negative_flag(false);
                        byte temp1 = _memory.read_byte(regs.HL);

                        set_carry_flag((temp1 >> 7 & 0x1) != 0);
                        temp1 = (byte)(temp1 << 1 | (temp1 >> 7 & 0x1));
                        _memory.write_byte(regs.HL, temp1);

                        zero_flag_set(temp1);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x07:
                    {
                        //RLC_R(ref regs.A);
                        set_auxiliary_carry_flag(false);
                        set_negative_flag(false);

                        set_carry_flag((regs.A >> 7 & 0x1) != 0);
                        regs.A = (byte)(regs.A << 1 | (regs.A >> 7 & 0x1));

                        zero_flag_set(regs.A);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x08:
                    {
                        //RRC_R(ref regs.B);
                        set_negative_flag(false);
                        set_auxiliary_carry_flag(false);

                        set_carry_flag(Convert.ToBoolean(regs.B & 0x1));
                        byte temp1 = (byte)(regs.B & 0x1);
                        regs.B >>= 1;
                        regs.B |= (byte)((temp1) << 7);

                        zero_flag_set(regs.B);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x09:
                    {
                        //RRC_R(ref regs.C);
                        set_negative_flag(false);
                        set_auxiliary_carry_flag(false);

                        set_carry_flag(Convert.ToBoolean(regs.C & 0x1));
                        byte temp1 = (byte)(regs.C & 0x1);
                        regs.C >>= 1;
                        regs.C |= (byte)((temp1) << 7);

                        zero_flag_set(regs.C);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x0A:
                    {
                        //RRC_R(ref regs.D);
                        set_negative_flag(false);
                        set_auxiliary_carry_flag(false);

                        set_carry_flag(Convert.ToBoolean(regs.D & 0x1));
                        byte temp1 = (byte)(regs.D & 0x1);
                        regs.D >>= 1;
                        regs.D |= (byte)((temp1) << 7);

                        zero_flag_set(regs.D);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x0B:
                    {
                        //RRC_R(ref regs.E);
                        set_negative_flag(false);
                        set_auxiliary_carry_flag(false);

                        set_carry_flag(Convert.ToBoolean(regs.E & 0x1));
                        byte temp1 = (byte)(regs.E & 0x1);
                        regs.E >>= 1;
                        regs.E |= (byte)((temp1) << 7);

                        zero_flag_set(regs.E);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x0C:
                    {
                        //RRC_R(ref regs.H);
                        set_negative_flag(false);
                        set_auxiliary_carry_flag(false);

                        set_carry_flag(Convert.ToBoolean(regs.H & 0x1));
                        byte temp1 = (byte)(regs.H & 0x1);
                        regs.H >>= 1;
                        regs.H |= (byte)((temp1) << 7);

                        zero_flag_set(regs.H);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x0D:
                    {
                        //RRC_R(ref regs.L);
                        set_negative_flag(false);
                        set_auxiliary_carry_flag(false);

                        set_carry_flag(Convert.ToBoolean(regs.L & 0x1));
                        byte temp1 = (byte)(regs.L & 0x1);
                        regs.L >>= 1;
                        regs.L |= (byte)((temp1) << 7);

                        zero_flag_set(regs.L);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x0E:
                    {
                        //RRC_M(regs.HL);
                        set_negative_flag(false);
                        set_auxiliary_carry_flag(false);
                        byte temp2 = _memory.read_byte(regs.HL);

                        set_carry_flag(Convert.ToBoolean(temp2 & 0x1));
                        byte temp1 = (byte)(temp2 & 0x1);
                        temp2 >>= 1;
                        temp2 |= (byte)((temp1) << 7);
                        _memory.write_byte(regs.HL, temp2);

                        zero_flag_set(temp2);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x0F:
                    {
                        //RRC_R(ref regs.A);
                        set_negative_flag(false);
                        set_auxiliary_carry_flag(false);

                        set_carry_flag(Convert.ToBoolean(regs.A & 0x1));
                        byte temp1 = (byte)(regs.A & 0x1);
                        regs.A >>= 1;
                        regs.A |= (byte)((temp1) << 7);

                        zero_flag_set(regs.A);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x10:
                    {
                        //RL_R(ref regs.B);
                        byte temp1 = (byte)((regs.F & 0x10) >> 4);

                        set_auxiliary_carry_flag(false);
                        set_negative_flag(false);

                        set_carry_flag(Convert.ToBoolean(regs.B >> 7 & 0x1));
                        regs.B <<= 1;
                        regs.B |= temp1;

                        zero_flag_set(regs.B);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x11:
                    {
                        //RL_R(ref regs.C);
                        byte temp1 = (byte)((regs.F & 0x10) >> 4);

                        set_auxiliary_carry_flag(false);
                        set_negative_flag(false);

                        set_carry_flag(Convert.ToBoolean(regs.C >> 7 & 0x1));
                        regs.C <<= 1;
                        regs.C |= temp1;

                        zero_flag_set(regs.C);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x12:
                    {
                        //RL_R(ref regs.D);
                        byte temp1 = (byte)((regs.F & 0x10) >> 4);

                        set_auxiliary_carry_flag(false);
                        set_negative_flag(false);

                        set_carry_flag(Convert.ToBoolean(regs.D >> 7 & 0x1));
                        regs.D <<= 1;
                        regs.D |= temp1;

                        zero_flag_set(regs.D);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x13:
                    {
                        //RL_R(ref regs.E);
                        byte temp1 = (byte)((regs.F & 0x10) >> 4);

                        set_auxiliary_carry_flag(false);
                        set_negative_flag(false);

                        set_carry_flag(Convert.ToBoolean(regs.E >> 7 & 0x1));
                        regs.E <<= 1;
                        regs.E |= temp1;

                        zero_flag_set(regs.E);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x14:
                    {
                        //RL_R(ref regs.H);
                        byte temp1 = (byte)((regs.F & 0x10) >> 4);

                        set_auxiliary_carry_flag(false);
                        set_negative_flag(false);

                        set_carry_flag(Convert.ToBoolean(regs.H >> 7 & 0x1));
                        regs.H <<= 1;
                        regs.H |= temp1;

                        zero_flag_set(regs.H);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x15:
                    {
                        //RL_R(ref regs.L);
                        byte temp1 = (byte)((regs.F & 0x10) >> 4);

                        set_auxiliary_carry_flag(false);
                        set_negative_flag(false);

                        set_carry_flag(Convert.ToBoolean(regs.L >> 7 & 0x1));
                        regs.L <<= 1;
                        regs.L |= temp1;

                        zero_flag_set(regs.L);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x16:
                    {
                        //RL_M(regs.HL);
                        byte temp1 = (byte)((regs.F & 0x10) >> 4);
                        byte temp2 = _memory.read_byte(regs.HL);

                        set_auxiliary_carry_flag(false);
                        set_negative_flag(false);

                        set_carry_flag(Convert.ToBoolean(temp2 >> 7 & 0x1));
                        temp2 <<= 1;
                        temp2 |= temp1;
                        _memory.write_byte(regs.HL, temp2);

                        zero_flag_set(temp2);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x17:
                    {
                        //RL_R(ref regs.A);
                        byte temp1 = (byte)((regs.F & 0x10) >> 4);

                        set_auxiliary_carry_flag(false);
                        set_negative_flag(false);

                        set_carry_flag(Convert.ToBoolean(regs.A >> 7 & 0x1));
                        regs.A <<= 1;
                        regs.A |= temp1;

                        zero_flag_set(regs.A);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x18:
                    {
                        //RR_R(ref regs.B);
                        byte temp1 = (byte)((regs.F & 0x10) >> 4);

                        set_negative_flag(false);
                        set_auxiliary_carry_flag(false);

                        set_carry_flag(Convert.ToBoolean(regs.B & 0x1));
                        regs.B >>= 1;
                        regs.B |= (byte)(temp1 << 7);

                        zero_flag_set(regs.B);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x19:
                    {
                        //RR_R(ref regs.C);
                        byte temp1 = (byte)((regs.F & 0x10) >> 4);

                        set_negative_flag(false);
                        set_auxiliary_carry_flag(false);

                        set_carry_flag(Convert.ToBoolean(regs.C & 0x1));
                        regs.C >>= 1;
                        regs.C |= (byte)(temp1 << 7);

                        zero_flag_set(regs.C);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x1A:
                    {
                        //RR_R(ref regs.D);
                        byte temp1 = (byte)((regs.F & 0x10) >> 4);

                        set_negative_flag(false);
                        set_auxiliary_carry_flag(false);

                        set_carry_flag(Convert.ToBoolean(regs.D & 0x1));
                        regs.D >>= 1;
                        regs.D |= (byte)(temp1 << 7);

                        zero_flag_set(regs.D);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x1B:
                    {
                        //RR_R(ref regs.E);
                        byte temp1 = (byte)((regs.F & 0x10) >> 4);

                        set_negative_flag(false);
                        set_auxiliary_carry_flag(false);

                        set_carry_flag(Convert.ToBoolean(regs.E & 0x1));
                        regs.E >>= 1;
                        regs.E |= (byte)(temp1 << 7);

                        zero_flag_set(regs.E);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x1C:
                    {
                        //RR_R(ref regs.H);
                        byte temp1 = (byte)((regs.F & 0x10) >> 4);

                        set_negative_flag(false);
                        set_auxiliary_carry_flag(false);

                        set_carry_flag(Convert.ToBoolean(regs.H & 0x1));
                        regs.H >>= 1;
                        regs.H |= (byte)(temp1 << 7);

                        zero_flag_set(regs.H);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x1D:
                    {
                        //RR_R(ref regs.L);
                        byte temp1 = (byte)((regs.F & 0x10) >> 4);

                        set_negative_flag(false);
                        set_auxiliary_carry_flag(false);

                        set_carry_flag(Convert.ToBoolean(regs.L & 0x1));
                        regs.L >>= 1;
                        regs.L |= (byte)(temp1 << 7);

                        zero_flag_set(regs.L);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x1E:
                    {
                        //RR_M(regs.HL);
                        byte temp1 = (byte)((regs.F & 0x10) >> 4);
                        byte temp2 = _memory.read_byte(regs.HL);

                        set_negative_flag(false);
                        set_auxiliary_carry_flag(false);

                        set_carry_flag(Convert.ToBoolean(temp2 & 0x1));
                        temp2 >>= 1;
                        temp2 |= (byte)(temp1 << 7);
                        _memory.write_byte(regs.HL, temp2);

                        zero_flag_set(temp2);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x1F:
                    {
                        //RR_R(ref regs.A);
                        byte temp1 = (byte)((regs.F & 0x10) >> 4);

                        set_negative_flag(false);
                        set_auxiliary_carry_flag(false);

                        set_carry_flag(Convert.ToBoolean(regs.A & 0x1));
                        regs.A >>= 1;
                        regs.A |= (byte)(temp1 << 7);

                        zero_flag_set(regs.A);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x20:
                    {
                        //SLA_R(ref regs.B);
                        set_auxiliary_carry_flag(false);
                        set_negative_flag(false);

                        set_carry_flag(Convert.ToBoolean(regs.B >> 7 & 0x1));
                        regs.B <<= 1;

                        zero_flag_set(regs.B);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x21:
                    {
                        //SLA_R(ref regs.C);
                        set_auxiliary_carry_flag(false);
                        set_negative_flag(false);

                        set_carry_flag(Convert.ToBoolean(regs.C >> 7 & 0x1));
                        regs.C <<= 1;

                        zero_flag_set(regs.C);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x22:
                    {
                        //SLA_R(ref regs.D);
                        set_auxiliary_carry_flag(false);
                        set_negative_flag(false);

                        set_carry_flag(Convert.ToBoolean(regs.D >> 7 & 0x1));
                        regs.D <<= 1;

                        zero_flag_set(regs.D);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x23:
                    {
                        //SLA_R(ref regs.E);
                        set_auxiliary_carry_flag(false);
                        set_negative_flag(false);

                        set_carry_flag(Convert.ToBoolean(regs.E >> 7 & 0x1));
                        regs.E <<= 1;

                        zero_flag_set(regs.E);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x24:
                    {
                        //SLA_R(ref regs.H);
                        set_auxiliary_carry_flag(false);
                        set_negative_flag(false);

                        set_carry_flag(Convert.ToBoolean(regs.H >> 7 & 0x1));
                        regs.H <<= 1;

                        zero_flag_set(regs.H);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x25:
                    {
                        //SLA_R(ref regs.L);
                        set_auxiliary_carry_flag(false);
                        set_negative_flag(false);

                        set_carry_flag(Convert.ToBoolean(regs.L >> 7 & 0x1));
                        regs.L <<= 1;

                        zero_flag_set(regs.L);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x26:
                    {
                        //SLA_M(regs.HL);
                        set_auxiliary_carry_flag(false);
                        set_negative_flag(false);
                        byte temp1 = _memory.read_byte(regs.HL);

                        set_carry_flag(Convert.ToBoolean(temp1 >> 7 & 0x1));
                        temp1 <<= 1;
                        _memory.write_byte(regs.HL, temp1);

                        zero_flag_set(temp1);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x27:
                    {
                        //SLA_R(ref regs.A);
                        set_auxiliary_carry_flag(false);
                        set_negative_flag(false);

                        set_carry_flag(Convert.ToBoolean(regs.A >> 7 & 0x1));
                        regs.A <<= 1;

                        zero_flag_set(regs.A);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x28:
                    {
                        //SRA_R(ref regs.B);
                        byte temp1 = regs.B;

                        set_negative_flag(false);
                        set_auxiliary_carry_flag(false);

                        set_carry_flag(Convert.ToBoolean(regs.B & 0x1));
                        regs.B >>= 1;
                        regs.B |= (byte)(temp1 & 0x80);

                        zero_flag_set(regs.B);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x29:
                    {
                        //SRA_R(ref regs.C);
                        byte temp1 = regs.C;

                        set_negative_flag(false);
                        set_auxiliary_carry_flag(false);

                        set_carry_flag(Convert.ToBoolean(regs.C & 0x1));
                        regs.C >>= 1;
                        regs.C |= (byte)(temp1 & 0x80);

                        zero_flag_set(regs.C);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x2A:
                    {
                        //SRA_R(ref regs.D);
                        byte temp1 = regs.D;

                        set_negative_flag(false);
                        set_auxiliary_carry_flag(false);

                        set_carry_flag(Convert.ToBoolean(regs.D & 0x1));
                        regs.D >>= 1;
                        regs.D |= (byte)(temp1 & 0x80);

                        zero_flag_set(regs.D);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x2B:
                    {
                        //SRA_R(ref regs.E);
                        byte temp1 = regs.E;

                        set_negative_flag(false);
                        set_auxiliary_carry_flag(false);

                        set_carry_flag(Convert.ToBoolean(regs.E & 0x1));
                        regs.E >>= 1;
                        regs.E |= (byte)(temp1 & 0x80);

                        zero_flag_set(regs.E);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x2C:
                    {
                        //SRA_R(ref regs.H);
                        byte temp1 = regs.H;

                        set_negative_flag(false);
                        set_auxiliary_carry_flag(false);

                        set_carry_flag(Convert.ToBoolean(regs.H & 0x1));
                        regs.H >>= 1;
                        regs.H |= (byte)(temp1 & 0x80);

                        zero_flag_set(regs.H);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x2D:
                    {
                        //SRA_R(ref regs.L);
                        byte temp1 = regs.L;

                        set_negative_flag(false);
                        set_auxiliary_carry_flag(false);

                        set_carry_flag(Convert.ToBoolean(regs.L & 0x1));
                        regs.L >>= 1;
                        regs.L |= (byte)(temp1 & 0x80);

                        zero_flag_set(regs.L);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x2E:
                    {
                        //SRA_M(regs.HL);
                        byte temp2 = _memory.read_byte(regs.HL);
                        byte temp1 = temp2;

                        set_negative_flag(false);
                        set_auxiliary_carry_flag(false);

                        set_carry_flag(Convert.ToBoolean(temp2 & 0x1));
                        temp2 >>= 1;
                        temp2 |= (byte)(temp1 & 0x80);
                        _memory.write_byte(regs.HL, temp2);

                        zero_flag_set(temp2);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x2F:
                    {
                        //SRA_R(ref regs.A);
                        byte temp1 = regs.A;

                        set_negative_flag(false);
                        set_auxiliary_carry_flag(false);

                        set_carry_flag(Convert.ToBoolean(regs.A & 0x1));
                        regs.A >>= 1;
                        regs.A |= (byte)(temp1 & 0x80);

                        zero_flag_set(regs.A);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x30:
                    {
                        //SWAP_R(ref regs.B);
                        byte temp1 = regs.B;
                        byte temp2 = (byte)(temp1 >> 4);

                        temp1 = (byte)((temp1 << 4) | temp2);
                        regs.B = temp1;

                        set_negative_flag(false);
                        set_carry_flag(false);
                        set_auxiliary_carry_flag(false);
                        zero_flag_set(regs.B);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x31:
                    {
                        //SWAP_R(ref regs.C);
                        byte temp1 = regs.C;
                        byte temp2 = (byte)(temp1 >> 4);

                        temp1 = (byte)((temp1 << 4) | temp2);
                        regs.C = temp1;

                        set_negative_flag(false);
                        set_carry_flag(false);
                        set_auxiliary_carry_flag(false);
                        zero_flag_set(regs.C);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x32:
                    {
                        //SWAP_R(ref regs.D);
                        byte temp1 = regs.D;
                        byte temp2 = (byte)(temp1 >> 4);

                        temp1 = (byte)((temp1 << 4) | temp2);
                        regs.D = temp1;

                        set_negative_flag(false);
                        set_carry_flag(false);
                        set_auxiliary_carry_flag(false);
                        zero_flag_set(regs.D);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x33:
                    {
                        //SWAP_R(ref regs.E);
                        byte temp1 = regs.E;
                        byte temp2 = (byte)(temp1 >> 4);

                        temp1 = (byte)((temp1 << 4) | temp2);
                        regs.E = temp1;

                        set_negative_flag(false);
                        set_carry_flag(false);
                        set_auxiliary_carry_flag(false);
                        zero_flag_set(regs.E);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x34:
                    {
                        //SWAP_R(ref regs.H);
                        byte temp1 = regs.H;
                        byte temp2 = (byte)(temp1 >> 4);

                        temp1 = (byte)((temp1 << 4) | temp2);
                        regs.H = temp1;

                        set_negative_flag(false);
                        set_carry_flag(false);
                        set_auxiliary_carry_flag(false);
                        zero_flag_set(regs.H);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x35:
                    {
                        //SWAP_R(ref regs.L);
                        byte temp1 = regs.L;
                        byte temp2 = (byte)(temp1 >> 4);

                        temp1 = (byte)((temp1 << 4) | temp2);
                        regs.L = temp1;

                        set_negative_flag(false);
                        set_carry_flag(false);
                        set_auxiliary_carry_flag(false);
                        zero_flag_set(regs.L);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x36:
                    {
                        //SWAP_M(regs.HL);
                        byte temp1 = _memory.read_byte(regs.HL);
                        byte temp2 = (byte)(temp1 >> 4);

                        temp1 = (byte)((temp1 << 4) | temp2);
                        _memory.write_byte(regs.HL, temp1);

                        set_negative_flag(false);
                        set_carry_flag(false);
                        set_auxiliary_carry_flag(false);
                        zero_flag_set(_memory.read_byte(regs.HL));
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x37:
                    {
                        //SWAP_R(ref regs.A);
                        byte temp1 = regs.A;
                        byte temp2 = (byte)(temp1 >> 4);

                        temp1 = (byte)((temp1 << 4) | temp2);
                        regs.A = temp1;

                        set_negative_flag(false);
                        set_carry_flag(false);
                        set_auxiliary_carry_flag(false);
                        zero_flag_set(regs.A);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x38:
                    {
                        //SRL_R(ref regs.B);
                        byte temp1 = regs.B;

                        set_negative_flag(false);
                        set_auxiliary_carry_flag(false);

                        set_carry_flag(Convert.ToBoolean(regs.B & 0x1));
                        regs.B >>= 1;

                        zero_flag_set(regs.B);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x39:
                    {
                        //SRL_R(ref regs.C);
                        byte temp1 = regs.C;

                        set_negative_flag(false);
                        set_auxiliary_carry_flag(false);

                        set_carry_flag(Convert.ToBoolean(regs.C & 0x1));
                        regs.C >>= 1;

                        zero_flag_set(regs.C);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x3A:
                    {
                        //SRL_R(ref regs.D);
                        byte temp1 = regs.D;

                        set_negative_flag(false);
                        set_auxiliary_carry_flag(false);

                        set_carry_flag(Convert.ToBoolean(regs.D & 0x1));
                        regs.D >>= 1;

                        zero_flag_set(regs.D);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x3B:
                    {
                        //SRL_R(ref regs.E);
                        byte temp1 = regs.E;

                        set_negative_flag(false);
                        set_auxiliary_carry_flag(false);

                        set_carry_flag(Convert.ToBoolean(regs.E & 0x1));
                        regs.E >>= 1;

                        zero_flag_set(regs.E);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x3C:
                    {
                        //SRL_R(ref regs.H);
                        byte temp1 = regs.H;

                        set_negative_flag(false);
                        set_auxiliary_carry_flag(false);

                        set_carry_flag(Convert.ToBoolean(regs.H & 0x1));
                        regs.H >>= 1;

                        zero_flag_set(regs.H);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x3D:
                    {
                        //SRL_R(ref regs.L);
                        byte temp1 = regs.L;

                        set_negative_flag(false);
                        set_auxiliary_carry_flag(false);

                        set_carry_flag(Convert.ToBoolean(regs.L & 0x1));
                        regs.L >>= 1;

                        zero_flag_set(regs.L);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x3E:
                    {
                        //SRL_M(regs.HL);
                        byte temp2 = _memory.read_byte(regs.HL);
                        byte temp1 = temp2;

                        set_negative_flag(false);
                        set_auxiliary_carry_flag(false);

                        set_carry_flag(Convert.ToBoolean(temp2 & 0x1));
                        temp2 >>= 1;
                        _memory.write_byte(regs.HL, temp2);

                        zero_flag_set(temp2);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x3F:
                    {
                        //SRL_R(ref regs.A);
                        byte temp1 = regs.A;

                        set_negative_flag(false);
                        set_auxiliary_carry_flag(false);

                        set_carry_flag(Convert.ToBoolean(regs.A & 0x1));
                        regs.A >>= 1;

                        zero_flag_set(regs.A);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x40:
                    {
                        //BIT_B_R(0, ref regs.B);
                        zero_flag_set((byte)(regs.B & (0x1 << 0)));
                        set_negative_flag(false);
                        set_auxiliary_carry_flag(true);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x41:
                    {
                        //BIT_B_R(0, ref regs.C);
                        zero_flag_set((byte)(regs.C & (0x1 << 0)));
                        set_negative_flag(false);
                        set_auxiliary_carry_flag(true);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x42:
                    {
                        //BIT_B_R(0, ref regs.D);
                        zero_flag_set((byte)(regs.D & (0x1 << 0)));
                        set_negative_flag(false);
                        set_auxiliary_carry_flag(true);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x43:
                    {
                        //BIT_B_R(0, ref regs.E);
                        zero_flag_set((byte)(regs.E & (0x1 << 0)));
                        set_negative_flag(false);
                        set_auxiliary_carry_flag(true);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x44:
                    {
                        //BIT_B_R(0, ref regs.H);
                        zero_flag_set((byte)(regs.H & (0x1 << 0)));
                        set_negative_flag(false);
                        set_auxiliary_carry_flag(true);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x45:
                    {
                        //BIT_B_R(0, ref regs.L);
                        zero_flag_set((byte)(regs.L & (0x1 << 0)));
                        set_negative_flag(false);
                        set_auxiliary_carry_flag(true);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x46:
                    {
                        //BIT_B_M(0, regs.HL);
                        zero_flag_set((byte)(_memory.read_byte(regs.HL) & (0x1 << 0)));
                        set_negative_flag(false);
                        set_auxiliary_carry_flag(true);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x47:
                    {
                        //BIT_B_R(0, ref regs.A);
                        zero_flag_set((byte)(regs.A & (0x1 << 0)));
                        set_negative_flag(false);
                        set_auxiliary_carry_flag(true);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x48:
                    {
                        //BIT_B_R(1, ref regs.B);
                        zero_flag_set((byte)(regs.B & (0x1 << 1)));
                        set_negative_flag(false);
                        set_auxiliary_carry_flag(true);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x49:
                    {
                        //BIT_B_R(1, ref regs.C);
                        zero_flag_set((byte)(regs.C & (0x1 << 1)));
                        set_negative_flag(false);
                        set_auxiliary_carry_flag(true);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x4A:
                    {
                        //BIT_B_R(1, ref regs.D);
                        zero_flag_set((byte)(regs.D & (0x1 << 1)));
                        set_negative_flag(false);
                        set_auxiliary_carry_flag(true);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x4B:
                    {
                        //BIT_B_R(1, ref regs.E);
                        zero_flag_set((byte)(regs.E & (0x1 << 1)));
                        set_negative_flag(false);
                        set_auxiliary_carry_flag(true);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x4C:
                    {
                        //BIT_B_R(1, ref regs.H);
                        zero_flag_set((byte)(regs.H & (0x1 << 1)));
                        set_negative_flag(false);
                        set_auxiliary_carry_flag(true);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x4D:
                    {
                        //BIT_B_R(1, ref regs.L);
                        zero_flag_set((byte)(regs.L & (0x1 << 1)));
                        set_negative_flag(false);
                        set_auxiliary_carry_flag(true);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x4E:
                    {
                        //BIT_B_M(1, regs.HL);
                        zero_flag_set((byte)(_memory.read_byte(regs.HL) & (0x1 << 1)));
                        set_negative_flag(false);
                        set_auxiliary_carry_flag(true);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x4F:
                    {
                        //BIT_B_R(1, ref regs.A);
                        zero_flag_set((byte)(regs.A & (0x1 << 1)));
                        set_negative_flag(false);
                        set_auxiliary_carry_flag(true);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x50:
                    {
                        //BIT_B_R(2, ref regs.B);
                        zero_flag_set((byte)(regs.B & (0x1 << 2)));
                        set_negative_flag(false);
                        set_auxiliary_carry_flag(true);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x51:
                    {
                        //BIT_B_R(2, ref regs.C);
                        zero_flag_set((byte)(regs.C & (0x1 << 2)));
                        set_negative_flag(false);
                        set_auxiliary_carry_flag(true);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x52:
                    {
                        //BIT_B_R(2, ref regs.D);
                        zero_flag_set((byte)(regs.D & (0x1 << 2)));
                        set_negative_flag(false);
                        set_auxiliary_carry_flag(true);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x53:
                    {
                        //BIT_B_R(2, ref regs.E);
                        zero_flag_set((byte)(regs.E & (0x1 << 2)));
                        set_negative_flag(false);
                        set_auxiliary_carry_flag(true);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x54:
                    {
                        //BIT_B_R(2, ref regs.H);
                        zero_flag_set((byte)(regs.H & (0x1 << 2)));
                        set_negative_flag(false);
                        set_auxiliary_carry_flag(true);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x55:
                    {
                        //BIT_B_R(2, ref regs.L);
                        zero_flag_set((byte)(regs.L & (0x1 << 2)));
                        set_negative_flag(false);
                        set_auxiliary_carry_flag(true);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x56:
                    {
                        //BIT_B_M(2, regs.HL);
                        zero_flag_set((byte)(_memory.read_byte(regs.HL) & (0x1 << 2)));
                        set_negative_flag(false);
                        set_auxiliary_carry_flag(true);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x57:
                    {
                        //BIT_B_R(2, ref regs.A);
                        zero_flag_set((byte)(regs.A & (0x1 << 2)));
                        set_negative_flag(false);
                        set_auxiliary_carry_flag(true);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x58:
                    {
                        //BIT_B_R(3, ref regs.B);
                        zero_flag_set((byte)(regs.B & (0x1 << 3)));
                        set_negative_flag(false);
                        set_auxiliary_carry_flag(true);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x59:
                    {
                        //BIT_B_R(3, ref regs.C);
                        zero_flag_set((byte)(regs.C & (0x1 << 3)));
                        set_negative_flag(false);
                        set_auxiliary_carry_flag(true);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x5A:
                    {
                        //BIT_B_R(3, ref regs.D);
                        zero_flag_set((byte)(regs.D & (0x1 << 3)));
                        set_negative_flag(false);
                        set_auxiliary_carry_flag(true);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x5B:
                    {
                        //BIT_B_R(3, ref regs.E);
                        zero_flag_set((byte)(regs.E & (0x1 << 3)));
                        set_negative_flag(false);
                        set_auxiliary_carry_flag(true);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x5C:
                    {
                        //BIT_B_R(3, ref regs.H);
                        zero_flag_set((byte)(regs.H & (0x1 << 3)));
                        set_negative_flag(false);
                        set_auxiliary_carry_flag(true);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x5D:
                    {
                        //BIT_B_R(3, ref regs.L);
                        zero_flag_set((byte)(regs.L & (0x1 << 3)));
                        set_negative_flag(false);
                        set_auxiliary_carry_flag(true);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x5E:
                    {
                        //BIT_B_M(3, regs.HL);
                        zero_flag_set((byte)(_memory.read_byte(regs.HL) & (0x1 << 3)));
                        set_negative_flag(false);
                        set_auxiliary_carry_flag(true);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x5F:
                    {
                        //BIT_B_R(3, ref regs.A);
                        zero_flag_set((byte)(regs.A & (0x1 << 3)));
                        set_negative_flag(false);
                        set_auxiliary_carry_flag(true);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x60:
                    {
                        //BIT_B_R(4, ref regs.B);
                        zero_flag_set((byte)(regs.B & (0x1 << 4)));
                        set_negative_flag(false);
                        set_auxiliary_carry_flag(true);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x61:
                    {
                        //BIT_B_R(4, ref regs.C);
                        zero_flag_set((byte)(regs.C & (0x1 << 4)));
                        set_negative_flag(false);
                        set_auxiliary_carry_flag(true);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x62:
                    {
                        //BIT_B_R(4, ref regs.D);
                        zero_flag_set((byte)(regs.D & (0x1 << 4)));
                        set_negative_flag(false);
                        set_auxiliary_carry_flag(true);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x63:
                    {
                        //BIT_B_R(4, ref regs.E);
                        zero_flag_set((byte)(regs.E & (0x1 << 4)));
                        set_negative_flag(false);
                        set_auxiliary_carry_flag(true);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x64:
                    {
                        //BIT_B_R(4, ref regs.H);
                        zero_flag_set((byte)(regs.H & (0x1 << 4)));
                        set_negative_flag(false);
                        set_auxiliary_carry_flag(true);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x65:
                    {
                        //BIT_B_R(4, ref regs.L);
                        zero_flag_set((byte)(regs.L & (0x1 << 4)));
                        set_negative_flag(false);
                        set_auxiliary_carry_flag(true);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x66:
                    {
                        //BIT_B_M(4, regs.HL);
                        zero_flag_set((byte)(_memory.read_byte(regs.HL) & (0x1 << 4)));
                        set_negative_flag(false);
                        set_auxiliary_carry_flag(true);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x67:
                    {
                        //BIT_B_R(4, ref regs.A);
                        zero_flag_set((byte)(regs.A & (0x1 << 4)));
                        set_negative_flag(false);
                        set_auxiliary_carry_flag(true);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x68:
                    {
                        //BIT_B_R(5, ref regs.B);
                        zero_flag_set((byte)(regs.B & (0x1 << 5)));
                        set_negative_flag(false);
                        set_auxiliary_carry_flag(true);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x69:
                    {
                        //BIT_B_R(5, ref regs.C);
                        zero_flag_set((byte)(regs.C & (0x1 << 5)));
                        set_negative_flag(false);
                        set_auxiliary_carry_flag(true);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x6A:
                    {
                        //BIT_B_R(5, ref regs.D);
                        zero_flag_set((byte)(regs.D & (0x1 << 5)));
                        set_negative_flag(false);
                        set_auxiliary_carry_flag(true);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x6B:
                    {
                        //BIT_B_R(5, ref regs.E);
                        zero_flag_set((byte)(regs.E & (0x1 << 5)));
                        set_negative_flag(false);
                        set_auxiliary_carry_flag(true);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x6C:
                    {
                        //BIT_B_R(5, ref regs.H);
                        zero_flag_set((byte)(regs.H & (0x1 << 5)));
                        set_negative_flag(false);
                        set_auxiliary_carry_flag(true);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x6D:
                    {
                        //BIT_B_R(5, ref regs.L);
                        zero_flag_set((byte)(regs.L & (0x1 << 5)));
                        set_negative_flag(false);
                        set_auxiliary_carry_flag(true);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x6E:
                    {
                        //BIT_B_M(5, regs.HL);
                        zero_flag_set((byte)(_memory.read_byte(regs.HL) & (0x1 << 5)));
                        set_negative_flag(false);
                        set_auxiliary_carry_flag(true);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x6F:
                    {
                        //BIT_B_R(5, ref regs.A);
                        zero_flag_set((byte)(regs.A & (0x1 << 5)));
                        set_negative_flag(false);
                        set_auxiliary_carry_flag(true);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x70:
                    {
                        //BIT_B_R(6, ref regs.B);
                        zero_flag_set((byte)(regs.B & (0x1 << 6)));
                        set_negative_flag(false);
                        set_auxiliary_carry_flag(true);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x71:
                    {
                        //BIT_B_R(6, ref regs.C);
                        zero_flag_set((byte)(regs.C & (0x1 << 6)));
                        set_negative_flag(false);
                        set_auxiliary_carry_flag(true);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x72:
                    {
                        //BIT_B_R(6, ref regs.D);
                        zero_flag_set((byte)(regs.D & (0x1 << 6)));
                        set_negative_flag(false);
                        set_auxiliary_carry_flag(true);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x73:
                    {
                        //BIT_B_R(6, ref regs.E);
                        zero_flag_set((byte)(regs.E & (0x1 << 6)));
                        set_negative_flag(false);
                        set_auxiliary_carry_flag(true);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x74:
                    {
                        //BIT_B_R(6, ref regs.H);
                        zero_flag_set((byte)(regs.H & (0x1 << 6)));
                        set_negative_flag(false);
                        set_auxiliary_carry_flag(true);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x75:
                    {
                        //BIT_B_R(6, ref regs.L);
                        zero_flag_set((byte)(regs.L & (0x1 << 6)));
                        set_negative_flag(false);
                        set_auxiliary_carry_flag(true);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x76:
                    {
                        //BIT_B_M(6, regs.HL);
                        zero_flag_set((byte)(_memory.read_byte(regs.HL) & (0x1 << 6)));
                        set_negative_flag(false);
                        set_auxiliary_carry_flag(true);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x77:
                    {
                        //BIT_B_R(6, ref regs.A);
                        zero_flag_set((byte)(regs.A & (0x1 << 6)));
                        set_negative_flag(false);
                        set_auxiliary_carry_flag(true);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x78:
                    {
                        //BIT_B_R(7, ref regs.B);
                        zero_flag_set((byte)(regs.B & (0x1 << 7)));
                        set_negative_flag(false);
                        set_auxiliary_carry_flag(true);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x79:
                    {
                        //BIT_B_R(7, ref regs.C);
                        zero_flag_set((byte)(regs.C & (0x1 << 7)));
                        set_negative_flag(false);
                        set_auxiliary_carry_flag(true);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x7A:
                    {
                        //BIT_B_R(7, ref regs.D);
                        zero_flag_set((byte)(regs.D & (0x1 << 7)));
                        set_negative_flag(false);
                        set_auxiliary_carry_flag(true);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x7B:
                    {
                        //BIT_B_R(7, ref regs.E);
                        zero_flag_set((byte)(regs.E & (0x1 << 7)));
                        set_negative_flag(false);
                        set_auxiliary_carry_flag(true);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x7C:
                    {
                        //BIT_B_R(7, ref regs.H);
                        zero_flag_set((byte)(regs.H & (0x1 << 7)));
                        set_negative_flag(false);
                        set_auxiliary_carry_flag(true);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x7D:
                    {
                        //BIT_B_R(7, ref regs.L);
                        zero_flag_set((byte)(regs.L & (0x1 << 7)));
                        set_negative_flag(false);
                        set_auxiliary_carry_flag(true);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x7E:
                    {
                        //BIT_B_M(7, regs.HL);
                        zero_flag_set((byte)(_memory.read_byte(regs.HL) & (0x1 << 7)));
                        set_negative_flag(false);
                        set_auxiliary_carry_flag(true);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x7F:
                    {
                        //BIT_B_R(7, ref regs.A);
                        zero_flag_set((byte)(regs.A & (0x1 << 7)));
                        set_negative_flag(false);
                        set_auxiliary_carry_flag(true);
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x80:
                    {
                        //RES_B_R(0, ref regs.B);
                        if ((regs.B & (0x1 << 0)) != 0x0)
                        {
                            regs.B ^= (byte)(0x1 << 0);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x81:
                    {
                        //RES_B_R(0, ref regs.C);
                        if ((regs.C & (0x1 << 0)) != 0x0)
                        {
                            regs.C ^= (byte)(0x1 << 0);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x82:
                    {
                        //RES_B_R(0, ref regs.D);
                        if ((regs.D & (0x1 << 0)) != 0x0)
                        {
                            regs.D ^= (byte)(0x1 << 0);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x83:
                    {
                        //RES_B_R(0, ref regs.E);
                        if ((regs.E & (0x1 << 0)) != 0x0)
                        {
                            regs.E ^= (byte)(0x1 << 0);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x84:
                    {
                        //RES_B_R(0, ref regs.H);
                        if ((regs.H & (0x1 << 0)) != 0x0)
                        {
                            regs.H ^= (byte)(0x1 << 0);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x85:
                    {
                        //RES_B_R(0, ref regs.L);
                        if ((regs.L & (0x1 << 0)) != 0x0)
                        {
                            regs.L ^= (byte)(0x1 << 0);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x86:
                    {
                        //RES_B_M(0, regs.HL);
                        byte temp1 = _memory.read_byte(regs.HL);
                        if ((temp1 & (0x1 << 0)) != 0x0)
                        {
                            temp1 ^= (byte)(0x1 << 0);
                            _memory.write_byte(regs.HL, temp1);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x87:
                    {
                        //RES_B_R(0, ref regs.A);
                        if ((regs.A & (0x1 << 0)) != 0x0)
                        {
                            regs.A ^= (byte)(0x1 << 0);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x88:
                    {
                        //RES_B_R(1, ref regs.B);
                        if ((regs.B & (0x1 << 1)) != 0x0)
                        {
                            regs.B ^= (byte)(0x1 << 1);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x89:
                    {
                        //RES_B_R(1, ref regs.C);
                        if ((regs.C & (0x1 << 1)) != 0x0)
                        {
                            regs.C ^= (byte)(0x1 << 1);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x8A:
                    {
                        //RES_B_R(1, ref regs.D);
                        if ((regs.D & (0x1 << 1)) != 0x0)
                        {
                            regs.D ^= (byte)(0x1 << 1);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x8B:
                    {
                        //RES_B_R(1, ref regs.E);
                        if ((regs.E & (0x1 << 1)) != 0x0)
                        {
                            regs.E ^= (byte)(0x1 << 1);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x8C:
                    {
                        //RES_B_R(1, ref regs.H);
                        if ((regs.H & (0x1 << 1)) != 0x0)
                        {
                            regs.H ^= (byte)(0x1 << 1);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x8D:
                    {
                        //RES_B_R(1, ref regs.L);
                        if ((regs.L & (0x1 << 1)) != 0x0)
                        {
                            regs.L ^= (byte)(0x1 << 1);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x8E:
                    {
                        //RES_B_M(1, regs.HL);
                        byte temp1 = _memory.read_byte(regs.HL);
                        if ((temp1 & (0x1 << 1)) != 0x0)
                        {
                            temp1 ^= (byte)(0x1 << 1);
                            _memory.write_byte(regs.HL, temp1);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x8F:
                    {
                        //RES_B_R(1, ref regs.A);
                        if ((regs.A & (0x1 << 1)) != 0x0)
                        {
                            regs.A ^= (byte)(0x1 << 1);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x90:
                    {
                        //RES_B_R(2, ref regs.B);
                        if ((regs.B & (0x1 << 2)) != 0x0)
                        {
                            regs.B ^= (byte)(0x1 << 2);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x91:
                    {
                        //RES_B_R(2, ref regs.C);
                        if ((regs.C & (0x1 << 2)) != 0x0)
                        {
                            regs.C ^= (byte)(0x1 << 2);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x92:
                    {
                        //RES_B_R(2, ref regs.D);
                        if ((regs.D & (0x1 << 2)) != 0x0)
                        {
                            regs.D ^= (byte)(0x1 << 2);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x93:
                    {
                        //RES_B_R(2, ref regs.E);
                        if ((regs.E & (0x1 << 2)) != 0x0)
                        {
                            regs.E ^= (byte)(0x1 << 2);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x94:
                    {
                        //RES_B_R(2, ref regs.H);
                        if ((regs.H & (0x1 << 2)) != 0x0)
                        {
                            regs.H ^= (byte)(0x1 << 2);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x95:
                    {
                        //RES_B_R(2, ref regs.L);
                        if ((regs.L & (0x1 << 2)) != 0x0)
                        {
                            regs.L ^= (byte)(0x1 << 2);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x96:
                    {
                        //RES_B_M(2, regs.HL);
                        byte temp1 = _memory.read_byte(regs.HL);
                        if ((temp1 & (0x1 << 2)) != 0x0)
                        {
                            temp1 ^= (byte)(0x1 << 2);
                            _memory.write_byte(regs.HL, temp1);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x97:
                    {
                        //RES_B_R(2, ref regs.A);
                        if ((regs.A & (0x1 << 2)) != 0x0)
                        {
                            regs.A ^= (byte)(0x1 << 2);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x98:
                    {
                        //RES_B_R(3, ref regs.B);
                        if ((regs.B & (0x1 << 3)) != 0x0)
                        {
                            regs.B ^= (byte)(0x1 << 3);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x99:
                    {
                        //RES_B_R(3, ref regs.C);
                        if ((regs.C & (0x1 << 3)) != 0x0)
                        {
                            regs.C ^= (byte)(0x1 << 3);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x9A:
                    {
                        //RES_B_R(3, ref regs.D);
                        if ((regs.D & (0x1 << 3)) != 0x0)
                        {
                            regs.D ^= (byte)(0x1 << 3);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x9B:
                    {
                        //RES_B_R(3, ref regs.E);
                        if ((regs.E & (0x1 << 3)) != 0x0)
                        {
                            regs.E ^= (byte)(0x1 << 3);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x9C:
                    {
                        //RES_B_R(3, ref regs.H);
                        if ((regs.H & (0x1 << 3)) != 0x0)
                        {
                            regs.H ^= (byte)(0x1 << 3);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x9D:
                    {
                        //RES_B_R(3, ref regs.L);
                        if ((regs.L & (0x1 << 3)) != 0x0)
                        {
                            regs.L ^= (byte)(0x1 << 3);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x9E:
                    {
                        //RES_B_M(3, regs.HL);
                        byte temp1 = _memory.read_byte(regs.HL);
                        if ((temp1 & (0x1 << 3)) != 0x0)
                        {
                            temp1 ^= (byte)(0x1 << 3);
                            _memory.write_byte(regs.HL, temp1);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0x9F:
                    {
                        //RES_B_R(3, ref regs.A);
                        if ((regs.A & (0x1 << 3)) != 0x0)
                        {
                            regs.A ^= (byte)(0x1 << 3);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xA0:
                    {
                        //RES_B_R(4, ref regs.B);
                        if ((regs.B & (0x1 << 4)) != 0x0)
                        {
                            regs.B ^= (byte)(0x1 << 4);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xA1:
                    {
                        //RES_B_R(4, ref regs.C);
                        if ((regs.C & (0x1 << 4)) != 0x0)
                        {
                            regs.C ^= (byte)(0x1 << 4);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xA2:
                    {
                        //RES_B_R(4, ref regs.D);
                        if ((regs.D & (0x1 << 4)) != 0x0)
                        {
                            regs.D ^= (byte)(0x1 << 4);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xA3:
                    {
                        //RES_B_R(4, ref regs.E);
                        if ((regs.E & (0x1 << 4)) != 0x0)
                        {
                            regs.E ^= (byte)(0x1 << 4);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xA4:
                    {
                        //RES_B_R(4, ref regs.H);
                        if ((regs.H & (0x1 << 4)) != 0x0)
                        {
                            regs.H ^= (byte)(0x1 << 4);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xA5:
                    {
                        //RES_B_R(4, ref regs.L);
                        if ((regs.L & (0x1 << 4)) != 0x0)
                        {
                            regs.L ^= (byte)(0x1 << 4);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xA6:
                    {
                        //RES_B_M(4, regs.HL);
                        byte temp1 = _memory.read_byte(regs.HL);
                        if ((temp1 & (0x1 << 4)) != 0x0)
                        {
                            temp1 ^= (byte)(0x1 << 4);
                            _memory.write_byte(regs.HL, temp1);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xA7:
                    {
                        //RES_B_R(4, ref regs.A);
                        if ((regs.A & (0x1 << 4)) != 0x0)
                        {
                            regs.A ^= (byte)(0x1 << 4);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xA8:
                    {
                        //RES_B_R(5, ref regs.B);
                        if ((regs.B & (0x1 << 5)) != 0x0)
                        {
                            regs.B ^= (byte)(0x1 << 5);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xA9:
                    {
                        //RES_B_R(5, ref regs.C);
                        if ((regs.C & (0x1 << 5)) != 0x0)
                        {
                            regs.C ^= (byte)(0x1 << 5);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xAA:
                    {
                        //RES_B_R(5, ref regs.D);
                        if ((regs.D & (0x1 << 5)) != 0x0)
                        {
                            regs.D ^= (byte)(0x1 << 5);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xAB:
                    {
                        //RES_B_R(5, ref regs.E);
                        if ((regs.E & (0x1 << 5)) != 0x0)
                        {
                            regs.E ^= (byte)(0x1 << 5);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xAC:
                    {
                        //RES_B_R(5, ref regs.H);
                        if ((regs.H & (0x1 << 5)) != 0x0)
                        {
                            regs.H ^= (byte)(0x1 << 5);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xAD:
                    {
                        //RES_B_R(5, ref regs.L);
                        if ((regs.L & (0x1 << 5)) != 0x0)
                        {
                            regs.L ^= (byte)(0x1 << 5);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xAE:
                    {
                        //RES_B_M(5, regs.HL);
                        byte temp1 = _memory.read_byte(regs.HL);
                        if ((temp1 & (0x1 << 5)) != 0x0)
                        {
                            temp1 ^= (byte)(0x1 << 5);
                            _memory.write_byte(regs.HL, temp1);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xAF:
                    {
                        //RES_B_R(5, ref regs.A);
                        if ((regs.A & (0x1 << 5)) != 0x0)
                        {
                            regs.A ^= (byte)(0x1 << 5);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xB0:
                    {
                        //RES_B_R(6, ref regs.B);
                        if ((regs.B & (0x1 << 6)) != 0x0)
                        {
                            regs.B ^= (byte)(0x1 << 6);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xB1:
                    {
                        //RES_B_R(6, ref regs.C);
                        if ((regs.C & (0x1 << 6)) != 0x0)
                        {
                            regs.C ^= (byte)(0x1 << 6);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xB2:
                    {
                        //RES_B_R(6, ref regs.D);
                        if ((regs.D & (0x1 << 6)) != 0x0)
                        {
                            regs.D ^= (byte)(0x1 << 6);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xB3:
                    {
                        //RES_B_R(6, ref regs.E);
                        if ((regs.E & (0x1 << 6)) != 0x0)
                        {
                            regs.E ^= (byte)(0x1 << 6);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xB4:
                    {
                        //RES_B_R(6, ref regs.H);
                        if ((regs.H & (0x1 << 6)) != 0x0)
                        {
                            regs.H ^= (byte)(0x1 << 6);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xB5:
                    {
                        //RES_B_R(6, ref regs.L);
                        if ((regs.L & (0x1 << 6)) != 0x0)
                        {
                            regs.L ^= (byte)(0x1 << 6);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xB6:
                    {
                        //RES_B_M(6, regs.HL);
                        byte temp1 = _memory.read_byte(regs.HL);
                        if ((temp1 & (0x1 << 6)) != 0x0)
                        {
                            temp1 ^= (byte)(0x1 << 6);
                            _memory.write_byte(regs.HL, temp1);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xB7:
                    {
                        //RES_B_R(6, ref regs.A);
                        if ((regs.A & (0x1 << 6)) != 0x0)
                        {
                            regs.A ^= (byte)(0x1 << 6);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xB8:
                    {
                        //RES_B_R(7, ref regs.B);
                        if ((regs.B & (0x1 << 7)) != 0x0)
                        {
                            regs.B ^= (byte)(0x1 << 7);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xB9:
                    {
                        //RES_B_R(7, ref regs.C);
                        if ((regs.C & (0x1 << 7)) != 0x0)
                        {
                            regs.C ^= (byte)(0x1 << 7);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xBA:
                    {
                        //RES_B_R(7, ref regs.D);
                        if ((regs.D & (0x1 << 7)) != 0x0)
                        {
                            regs.D ^= (byte)(0x1 << 7);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xBB:
                    {
                        //RES_B_R(7, ref regs.E);
                        if ((regs.E & (0x1 << 7)) != 0x0)
                        {
                            regs.E ^= (byte)(0x1 << 7);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xBC:
                    {
                        //RES_B_R(7, ref regs.H);
                        if ((regs.H & (0x1 << 7)) != 0x0)
                        {
                            regs.H ^= (byte)(0x1 << 7);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xBD:
                    {
                        //RES_B_R(7, ref regs.L);
                        if ((regs.L & (0x1 << 7)) != 0x0)
                        {
                            regs.L ^= (byte)(0x1 << 7);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xBE:
                    {
                        //RES_B_M(7, regs.HL);
                        byte temp1 = _memory.read_byte(regs.HL);
                        if ((temp1 & (0x1 << 7)) != 0x0)
                        {
                            temp1 ^= (byte)(0x1 << 7);
                            _memory.write_byte(regs.HL, temp1);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xBF:
                    {
                        //RES_B_R(7, ref regs.A);
                        if ((regs.A & (0x1 << 7)) != 0x0)
                        {
                            regs.A ^= (byte)(0x1 << 7);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xC0:
                    {
                        //SET_B_R(0, ref regs.B);
                        if ((regs.B & (0x1 << 0)) == 0x0)
                        {
                            regs.B ^= (byte)(0x1 << 0);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xC1:
                    {
                        //SET_B_R(0, ref regs.C);
                        if ((regs.C & (0x1 << 0)) == 0x0)
                        {
                            regs.C ^= (byte)(0x1 << 0);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xC2:
                    {
                        //SET_B_R(0, ref regs.D);
                        if ((regs.D & (0x1 << 0)) == 0x0)
                        {
                            regs.D ^= (byte)(0x1 << 0);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xC3:
                    {
                        //SET_B_R(0, ref regs.E);
                        if ((regs.E & (0x1 << 0)) == 0x0)
                        {
                            regs.E ^= (byte)(0x1 << 0);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xC4:
                    {
                        //SET_B_R(0, ref regs.H);
                        if ((regs.H & (0x1 << 0)) == 0x0)
                        {
                            regs.H ^= (byte)(0x1 << 0);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xC5:
                    {
                        //SET_B_R(0, ref regs.L);
                        if ((regs.L & (0x1 << 0)) == 0x0)
                        {
                            regs.L ^= (byte)(0x1 << 0);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xC6:
                    {
                        //SET_B_M(0, regs.HL);
                        byte temp1 = _memory.read_byte(regs.HL);
                        if ((temp1 & (0x1 << 0)) == 0x0)
                        {
                            temp1 ^= (byte)(0x1 << 0);
                            _memory.write_byte(regs.HL, temp1);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xC7:
                    {
                        //SET_B_R(0, ref regs.A);
                        if ((regs.A & (0x1 << 0)) == 0x0)
                        {
                            regs.A ^= (byte)(0x1 << 0);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xC8:
                    {
                        //SET_B_R(1, ref regs.B);
                        if ((regs.B & (0x1 << 1)) == 0x0)
                        {
                            regs.B ^= (byte)(0x1 << 1);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xC9:
                    {
                        //SET_B_R(1, ref regs.C);
                        if ((regs.C & (0x1 << 1)) == 0x0)
                        {
                            regs.C ^= (byte)(0x1 << 1);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xCA:
                    {
                        //SET_B_R(1, ref regs.D);
                        if ((regs.D & (0x1 << 1)) == 0x0)
                        {
                            regs.D ^= (byte)(0x1 << 1);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xCB:
                    {
                        //SET_B_R(1, ref regs.E);
                        if ((regs.E & (0x1 << 1)) == 0x0)
                        {
                            regs.E ^= (byte)(0x1 << 1);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xCC:
                    {
                        //SET_B_R(1, ref regs.H);
                        if ((regs.H & (0x1 << 1)) == 0x0)
                        {
                            regs.H ^= (byte)(0x1 << 1);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xCD:
                    {
                        //SET_B_R(1, ref regs.L);
                        if ((regs.L & (0x1 << 1)) == 0x0)
                        {
                            regs.L ^= (byte)(0x1 << 1);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xCE:
                    {
                        //SET_B_M(1, regs.HL);
                        byte temp1 = _memory.read_byte(regs.HL);
                        if ((temp1 & (0x1 << 1)) == 0x0)
                        {
                            temp1 ^= (byte)(0x1 << 1);
                            _memory.write_byte(regs.HL, temp1);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xCF:
                    {
                        //SET_B_R(1, ref regs.A);
                        if ((regs.A & (0x1 << 1)) == 0x0)
                        {
                            regs.A ^= (byte)(0x1 << 1);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xD0:
                    {
                        //SET_B_R(2, ref regs.B);
                        if ((regs.B & (0x1 << 2)) == 0x0)
                        {
                            regs.B ^= (byte)(0x1 << 2);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xD1:
                    {
                        //SET_B_R(2, ref regs.C);
                        if ((regs.C & (0x1 << 2)) == 0x0)
                        {
                            regs.C ^= (byte)(0x1 << 2);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xD2:
                    {
                        //SET_B_R(2, ref regs.D);
                        if ((regs.D & (0x1 << 2)) == 0x0)
                        {
                            regs.D ^= (byte)(0x1 << 2);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xD3:
                    {
                        //SET_B_R(2, ref regs.E);
                        if ((regs.E & (0x1 << 2)) == 0x0)
                        {
                            regs.E ^= (byte)(0x1 << 2);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xD4:
                    {
                        //SET_B_R(2, ref regs.H);
                        if ((regs.H & (0x1 << 2)) == 0x0)
                        {
                            regs.H ^= (byte)(0x1 << 2);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xD5:
                    {
                        //SET_B_R(2, ref regs.L);
                        if ((regs.L & (0x1 << 2)) == 0x0)
                        {
                            regs.L ^= (byte)(0x1 << 2);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xD6:
                    {
                        //SET_B_M(2, regs.HL);
                        byte temp1 = _memory.read_byte(regs.HL);
                        if ((temp1 & (0x1 << 2)) == 0x0)
                        {
                            temp1 ^= (byte)(0x1 << 2);
                            _memory.write_byte(regs.HL, temp1);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xD7:
                    {
                        //SET_B_R(2, ref regs.A);
                        if ((regs.A & (0x1 << 2)) == 0x0)
                        {
                            regs.A ^= (byte)(0x1 << 2);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xD8:
                    {
                        //SET_B_R(3, ref regs.B);
                        if ((regs.B & (0x1 << 3)) == 0x0)
                        {
                            regs.B ^= (byte)(0x1 << 3);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xD9:
                    {
                        //SET_B_R(3, ref regs.C);
                        if ((regs.C & (0x1 << 3)) == 0x0)
                        {
                            regs.C ^= (byte)(0x1 << 3);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xDA:
                    {
                        //SET_B_R(3, ref regs.D);
                        if ((regs.D & (0x1 << 3)) == 0x0)
                        {
                            regs.D ^= (byte)(0x1 << 3);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xDB:
                    {
                        //SET_B_R(3, ref regs.E);
                        if ((regs.E & (0x1 << 3)) == 0x0)
                        {
                            regs.E ^= (byte)(0x1 << 3);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xDC:
                    {
                        //SET_B_R(3, ref regs.H);
                        if ((regs.H & (0x1 << 3)) == 0x0)
                        {
                            regs.H ^= (byte)(0x1 << 3);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xDD:
                    {
                        //SET_B_R(3, ref regs.L);
                        if ((regs.L & (0x1 << 3)) == 0x0)
                        {
                            regs.L ^= (byte)(0x1 << 3);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xDE:
                    {
                        //SET_B_M(3, regs.HL);
                        byte temp1 = _memory.read_byte(regs.HL);
                        if ((temp1 & (0x1 << 3)) == 0x0)
                        {
                            temp1 ^= (byte)(0x1 << 3);
                            _memory.write_byte(regs.HL, temp1);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xDF:
                    {
                        //SET_B_R(3, ref regs.A);
                        if ((regs.A & (0x1 << 3)) == 0x0)
                        {
                            regs.A ^= (byte)(0x1 << 3);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xE0:
                    {
                        //SET_B_R(4, ref regs.B);
                        if ((regs.B & (0x1 << 4)) == 0x0)
                        {
                            regs.B ^= (byte)(0x1 << 4);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xE1:
                    {
                        //SET_B_R(4, ref regs.C);
                        if ((regs.C & (0x1 << 4)) == 0x0)
                        {
                            regs.C ^= (byte)(0x1 << 4);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xE2:
                    {
                        //SET_B_R(4, ref regs.D);
                        if ((regs.D & (0x1 << 4)) == 0x0)
                        {
                            regs.D ^= (byte)(0x1 << 4);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xE3:
                    {
                        //SET_B_R(4, ref regs.E);
                        if ((regs.E & (0x1 << 4)) == 0x0)
                        {
                            regs.E ^= (byte)(0x1 << 4);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xE4:
                    {
                        //SET_B_R(4, ref regs.H);
                        if ((regs.H & (0x1 << 4)) == 0x0)
                        {
                            regs.H ^= (byte)(0x1 << 4);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xE5:
                    {
                        //SET_B_R(4, ref regs.L);
                        if ((regs.L & (0x1 << 4)) == 0x0)
                        {
                            regs.L ^= (byte)(0x1 << 4);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xE6:
                    {
                        //SET_B_M(4, regs.HL);
                        byte temp1 = _memory.read_byte(regs.HL);
                        if ((temp1 & (0x1 << 4)) == 0x0)
                        {
                            temp1 ^= (byte)(0x1 << 4);
                            _memory.write_byte(regs.HL, temp1);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xE7:
                    {
                        //SET_B_R(4, ref regs.A);
                        if ((regs.A & (0x1 << 4)) == 0x0)
                        {
                            regs.A ^= (byte)(0x1 << 4);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xE8:
                    {
                        //SET_B_R(5, ref regs.B);
                        if ((regs.B & (0x1 << 5)) == 0x0)
                        {
                            regs.B ^= (byte)(0x1 << 5);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xE9:
                    {
                        //SET_B_R(5, ref regs.C);
                        if ((regs.C & (0x1 << 5)) == 0x0)
                        {
                            regs.C ^= (byte)(0x1 << 5);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xEA:
                    {
                        //SET_B_R(5, ref regs.D);
                        if ((regs.D & (0x1 << 5)) == 0x0)
                        {
                            regs.D ^= (byte)(0x1 << 5);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xEB:
                    {
                        //SET_B_R(5, ref regs.E);
                        if ((regs.E & (0x1 << 5)) == 0x0)
                        {
                            regs.E ^= (byte)(0x1 << 5);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xEC:
                    {
                        //SET_B_R(5, ref regs.H);
                        if ((regs.H & (0x1 << 5)) == 0x0)
                        {
                            regs.H ^= (byte)(0x1 << 5);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xED:
                    {
                        //SET_B_R(5, ref regs.L);
                        if ((regs.L & (0x1 << 5)) == 0x0)
                        {
                            regs.L ^= (byte)(0x1 << 5);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xEE:
                    {
                        //SET_B_M(5, regs.HL);
                        byte temp1 = _memory.read_byte(regs.HL);
                        if ((temp1 & (0x1 << 5)) == 0x0)
                        {
                            temp1 ^= (byte)(0x1 << 5);
                            _memory.write_byte(regs.HL, temp1);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xEF:
                    {
                        //SET_B_R(5, ref regs.A);
                        if ((regs.A & (0x1 << 5)) == 0x0)
                        {
                            regs.A ^= (byte)(0x1 << 5);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xF0:
                    {
                        //SET_B_R(6, ref regs.B);
                        if ((regs.B & (0x1 << 6)) == 0x0)
                        {
                            regs.B ^= (byte)(0x1 << 6);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xF1:
                    {
                        //SET_B_R(6, ref regs.C);
                        if ((regs.C & (0x1 << 6)) == 0x0)
                        {
                            regs.C ^= (byte)(0x1 << 6);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xF2:
                    {
                        //SET_B_R(6, ref regs.D);
                        if ((regs.D & (0x1 << 6)) == 0x0)
                        {
                            regs.D ^= (byte)(0x1 << 6);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xF3:
                    {
                        //SET_B_R(6, ref regs.E);
                        if ((regs.E & (0x1 << 6)) == 0x0)
                        {
                            regs.E ^= (byte)(0x1 << 6);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xF4:
                    {
                        //SET_B_R(6, ref regs.H);
                        if ((regs.H & (0x1 << 6)) == 0x0)
                        {
                            regs.H ^= (byte)(0x1 << 6);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xF5:
                    {
                        //SET_B_R(6, ref regs.L);
                        if ((regs.L & (0x1 << 6)) == 0x0)
                        {
                            regs.L ^= (byte)(0x1 << 6);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xF6:
                    {
                        //SET_B_M(6, regs.HL);
                        byte temp1 = _memory.read_byte(regs.HL);
                        if ((temp1 & (0x1 << 6)) == 0x0)
                        {
                            temp1 ^= (byte)(0x1 << 6);
                            _memory.write_byte(regs.HL, temp1);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xF7:
                    {
                        //SET_B_R(6, ref regs.A);
                        if ((regs.A & (0x1 << 6)) == 0x0)
                        {
                            regs.A ^= (byte)(0x1 << 6);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xF8:
                    {
                        //SET_B_R(7, ref regs.B);
                        if ((regs.B & (0x1 << 7)) == 0x0)
                        {
                            regs.B ^= (byte)(0x1 << 7);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xF9:
                    {
                        //SET_B_R(7, ref regs.C);
                        if ((regs.C & (0x1 << 7)) == 0x0)
                        {
                            regs.C ^= (byte)(0x1 << 7);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xFA:
                    {
                        //SET_B_R(7, ref regs.D);
                        if ((regs.D & (0x1 << 7)) == 0x0)
                        {
                            regs.D ^= (byte)(0x1 << 7);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xFB:
                    {
                        //SET_B_R(7, ref regs.E);
                        if ((regs.E & (0x1 << 7)) == 0x0)
                        {
                            regs.E ^= (byte)(0x1 << 7);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xFC:
                    {
                        //SET_B_R(7, ref regs.H);
                        if ((regs.H & (0x1 << 7)) == 0x0)
                        {
                            regs.H ^= (byte)(0x1 << 7);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xFD:
                    {
                        //SET_B_R(7, ref regs.L);
                        if ((regs.L & (0x1 << 7)) == 0x0)
                        {
                            regs.L ^= (byte)(0x1 << 7);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xFE:
                    {
                        //SET_B_M(7, regs.HL);
                        byte temp1 = _memory.read_byte(regs.HL);
                        if ((temp1 & (0x1 << 7)) == 0x0)
                        {
                            temp1 ^= (byte)(0x1 << 7);
                            _memory.write_byte(regs.HL, temp1);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                case 0xFF:
                    {
                        //SET_B_R(7, ref regs.A);
                        if ((regs.A & (0x1 << 7)) == 0x0)
                        {
                            regs.A ^= (byte)(0x1 << 7);
                        }
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                default:
                    {
                        pc += 2;
                        return;
                    }
            }
        }
        #endregion

        #region decode opcode 10
        private void Decode_Opcode_10(byte opcode2)
        {
            cyclesThisOp = cycles_lookup_10[opcode2];
            cycles -= cyclesThisOp;
            cycles_lookup_10_log2[opcode2]++;

            switch (opcode2)
            {
                case 0x00:
                    {
                        //STOP();
                        pc += 2;
                        next_opcode = _memory.read_byte(pc);
                        return;
                    }
                default:
                    {
                        pc += 2;
                        return;
                    }
            }
        }
        #endregion

        #region LD n, r
        /// <summary>
        /// Loads a byte into the specified register.
        /// </summary>
        private void LD_N_R(ref byte dst)
        {
            dst = _memory.read_byte((ushort)(pc + 1));
            pc += 2;
            next_opcode = _memory.read_byte(pc);
        }
        #endregion

        #region LD r2, r1
        /// <summary>
        /// Loads a specified register into another register.
        /// </summary>
        private void LD_R2_R1(ref byte src, ref byte dst)
        {
            dst = src;
            pc++;
            next_opcode = _memory.read_byte(pc);
        }
        #endregion

        #region LD m, r
        /// <summary>
        ///  Loads a byte from the specified address into another register.
        /// </summary>
        private void LD_R2_R1(ushort addr, ref byte dst)
        {
            dst = _memory.read_byte(addr);
            pc++;
            next_opcode = _memory.read_byte(pc);
        }
        #endregion

        #region LD (nn), r
        /// <summary>
        ///  Loads a byte from the specified immediate address into another register.
        /// </summary>
        private void LD_NN_R(ref byte dst)
        {
            dst = _memory.read_byte(_memory.read_ushort((ushort)(pc + 1)));
            pc += 3;
            next_opcode = _memory.read_byte(pc);
        }
        #endregion

        #region LD r, M
        /// <summary>
        ///  Loads a register into the specified memory address.
        /// </summary>
        private void LD_R2_R1(ref byte data, ushort addr)
        {
            _memory.write_byte(addr, data);
            pc++;
            next_opcode = _memory.read_byte(pc);
        }
        #endregion

        #region LD n, m
        /// <summary>
        ///  Loads an immediate value into the specified memory address.
        /// </summary>
        private void LD_R2_R1(ushort addr)
        {
            _memory.write_byte(addr, _memory.read_byte((ushort)(pc + 1)));
            pc += 2;
            next_opcode = _memory.read_byte(pc);
        }
        #endregion

        #region LD r, (nn)
        /// <summary>
        ///  Loads a register into the specified immediate memory address.
        /// </summary>
        private void LD_R_NN(ref byte data)
        {
            _memory.write_byte(_memory.read_ushort((ushort)(pc + 1)), data);
            pc += 3;
            next_opcode = _memory.read_byte(pc);
        }
        #endregion

        #region LD (r), r
        /// <summary>
        ///  Loads a byte from 0xFF00 + the specified register into the specified register.
        /// </summary>
        private void LD_MR_R(ref byte src, ref byte dst)
        {
            dst = _memory.read_byte((ushort)(0xFF00 + src));
            pc++;
            next_opcode = _memory.read_byte(pc);
        }
        #endregion

        #region LD r, (r)
        /// <summary>
        ///  Loads a byte from the specified register into the 0xFF00 + the specified register address.
        /// </summary>
        private void LD_R_MR(ref byte src, ref byte dst)
        {
            _memory.write_byte((ushort)(0xFF00 + dst), src);
            pc++;
            next_opcode = _memory.read_byte(pc);
        }
        #endregion

        #region LDD m, r
        /// <summary>
        ///  Loads a byte from the specified address into the specified register and decrements the src address.
        /// </summary>
        private void LDD_M_R(ref ushort src, ref byte dst)
        {
            dst = _memory.read_byte(src);
            src--;
            pc++;
            next_opcode = _memory.read_byte(pc);
        }
        #endregion

        #region LDD r, m
        /// <summary>
        ///  Loads a byte from the specified register into the specified address and decrements the destination address.
        /// </summary>
        private void LDD_R_M(ref byte src, ref ushort dst)
        {
            _memory.write_byte(dst, src);
            dst--;
            pc++;
            next_opcode = _memory.read_byte(pc);
        }
        #endregion

        #region LDI m, r
        /// <summary>
        ///  Loads a byte from the specified address into the specified register and increments the src address.
        /// </summary>
        private void LDI_M_R(ref ushort src, ref byte dst)
        {
            dst = _memory.read_byte(src);
            src++;
            pc++;
            next_opcode = _memory.read_byte(pc);
        }
        #endregion

        #region LDI r, m
        /// <summary>
        ///  Loads a byte from the specified register into the specified address and increments the destination address.
        /// </summary>
        private void LDI_R_M(ref byte src, ref ushort dst)
        {
            _memory.write_byte(dst, src);
            dst++;
            pc++;
            next_opcode = _memory.read_byte(pc);
        }
        #endregion

        #region LD (n), r
        /// <summary>
        ///  Loads a byte from 0xFF00 + the immediate value into the specified register.
        /// </summary>
        private void LD_MR_R(ref byte dst)
        {
            dst = _memory.read_byte((ushort)(0xFF00 + _memory.read_byte((ushort)(pc + 1))));
            pc += 2;
            next_opcode = _memory.read_byte(pc);
        }
        #endregion

        #region LD r, (n)
        /// <summary>
        ///  Loads a register into the address 0xFF00 + the immediate value.
        /// </summary>
        private void LD_R_MR(ref byte src)
        {
            _memory.write_byte((ushort)(0xFF00 + _memory.read_byte((ushort)(pc + 1))), src);
            pc += 2;
            next_opcode = _memory.read_byte(pc);
        }
        #endregion

        #region LD nn, rr
        /// <summary>
        /// Loads a ushort into the specified register.
        /// </summary>
        private void LD_NN_RR(ref ushort dst)
        {
            dst = _memory.read_ushort((ushort)(pc + 1));
            pc += 3;
            next_opcode = _memory.read_byte(pc);
        }
        #endregion

        #region LD rr2, rr1
        /// <summary>
        /// Loads the specified ushort register into the specified ushort register.
        /// </summary>
        private void LD_RR_RR(ref ushort src, ref ushort dst)
        {
            dst = src;
            pc++;
            next_opcode = _memory.read_byte(pc);
        }
        #endregion

        #region LD hl, sp + n
        /// <summary>
        /// Loads SP + a signed immediate byte into the HL register.
        /// </summary>
        private void LD_HL_SP_N()
        {
            sbyte temp = (sbyte)(_memory.read_byte((ushort)(pc + 1)));
            ushort temp_s = sp;
            set_zero_flag(false);
            set_negative_flag(false);
            dad_carry_flag_set((uint)(temp_s + temp));
            if ((sp & 0xFFF) > ((temp_s + temp) & 0xFFF))
            {
                set_auxiliary_carry_flag(true);
            }
            else
            {
                set_auxiliary_carry_flag(false);
            }

            regs.HL = (ushort)(sp + temp);
            pc++;
            next_opcode = _memory.read_byte(pc);
        }
        #endregion

        #region LD (nn), rr
        /// <summary>
        /// Loads the specified ushort register into the specified address.
        /// </summary>
        private void LD_MNN_RR(ref ushort src)
        {
            _memory.write_ushort(_memory.read_ushort((ushort)(pc + 1)), src);
            pc += 3;
            next_opcode = _memory.read_byte(pc);
        }
        #endregion

        #region PUSH rr
        /// <summary>
        /// Puts the specified ushort register onto the stack.
        /// </summary>
        private void PUSH_RR(ref byte reg1, ref byte reg2)
        {
            _memory.write_byte((ushort)(sp - 1), reg2);
            _memory.write_byte((ushort)(sp - 2), reg1);
            sp -= 2;
            pc++;
            next_opcode = _memory.read_byte(pc);
        }
        #endregion

        #region POP rr
        /// <summary>
        /// Takes 2 bytes off the stack and puts them into the specified ushort register.
        /// </summary>
        private void POP_RR(ref byte reg1, ref byte reg2)
        {
            reg2 = _memory.read_byte((ushort)(sp + 1));
            reg1 = _memory.read_byte((ushort)(sp));
            sp += 2;
            pc++;
            next_opcode = _memory.read_byte(pc);
        }
        #endregion

        #region ADD r
        /// <summary>
        /// Adds the specified register to the register A.
        /// </summary>
        private void ADD_R(ref byte reg)
        {
            auxiliary_carry_flag_set(regs.A, (byte)((regs.A + reg)));
            normal_carry_flag_set((ushort)(regs.A + reg));

            regs.A += reg;

            set_negative_flag(false);
            zero_flag_set(regs.A);
            pc++;
            next_opcode = _memory.read_byte(pc);
        }
        #endregion

        #region ADD m
        /// <summary>
        /// Adds the byte at the specified memory address to the register A.
        /// </summary>
        private void ADD_M(ushort addr)
        {
            byte temp = _memory.read_byte(addr);
            auxiliary_carry_flag_set(regs.A, (byte)((regs.A + temp)));
            normal_carry_flag_set((ushort)(regs.A + temp));

            regs.A += temp;

            set_negative_flag(false);
            zero_flag_set(regs.A);
            pc++;
            next_opcode = _memory.read_byte(pc);
        }
        #endregion

        #region ADD m
        /// <summary>
        /// Adds the immediate byte to the register A.
        /// </summary>
        private void ADD_M2(ushort addr)
        {
            byte temp = _memory.read_byte(addr);
            auxiliary_carry_flag_set(regs.A, (byte)((regs.A + temp)));
            normal_carry_flag_set((ushort)(regs.A + temp));

            regs.A += temp;

            set_negative_flag(false);
            zero_flag_set(regs.A);
            pc += 2;
            next_opcode = _memory.read_byte(pc);
        }
        #endregion

        #region ADC r
        /// <summary>
        /// Adds the specified register to the register A plus the value of the carry flag.
        /// </summary>
        private void ADC_R(ref byte reg)
        {
            byte temp1 = regs.A;
            auxiliary_carry_flag_set(regs.A, (byte)(temp1 + reg + (regs.F >> 4 & 0x1)));

            regs.A += (byte)(reg + (regs.F >> 4 & 0x1));

            normal_carry_flag_set((ushort)(temp1 + reg + (regs.F >> 4 & 0x1)));
            zero_flag_set(regs.A);
            set_negative_flag(false);
            pc++;
            next_opcode = _memory.read_byte(pc);
        }
        #endregion

        #region ADC m
        /// <summary>
        /// Adds the value at the specified address to the register A plus the value of the carry flag.
        /// </summary>
        private void ADC_M(ushort addr)
        {
            byte temp1 = regs.A;
            byte temp2 = _memory.read_byte(addr);
            auxiliary_carry_flag_set(regs.A, (byte)(temp1 + temp2 + (regs.F >> 4 & 0x1)));

            regs.A += (byte)(temp2 + (regs.F >> 4 & 0x1));

            normal_carry_flag_set((ushort)(temp1 + temp2 + (regs.F >> 4 & 0x1)));
            zero_flag_set(regs.A);
            set_negative_flag(false);
            pc++;
            next_opcode = _memory.read_byte(pc);
        }
        #endregion

        #region ADC m
        /// <summary>
        /// Adds the immediate value at the specified address to the register A plus the value of the carry flag.
        /// </summary>
        private void ADC_M2(ushort addr)
        {
            byte temp1 = regs.A;
            byte temp2 = _memory.read_byte(addr);
            auxiliary_carry_flag_set(regs.A, (byte)(temp1 + temp2 + ((regs.F >> 4) & 0x1)));

            regs.A += (byte)(temp2 + ((regs.F >> 4) & 0x1));

            normal_carry_flag_set((ushort)(temp1 + temp2 + ((regs.F >> 4) & 0x1)));
            zero_flag_set(regs.A);
            set_negative_flag(false);
            pc += 2;
            next_opcode = _memory.read_byte(pc);
        }
        #endregion

        #region SUB r
        /// <summary>
        /// Subtracts the specified register from the register A.
        /// </summary>
        private void SUB_R(ref byte reg)
        {
            byte temp1 = (byte)(-reg);

            auxiliary_carry_flag_set(regs.A, (byte)(regs.A + temp1));
            sub_carry_flag_set(regs.A, temp1);

            regs.A += temp1;

            zero_flag_set(regs.A);
            set_negative_flag(true);
            pc++;
            next_opcode = _memory.read_byte(pc);
        }
        #endregion

        #region SUB m
        /// <summary>
        /// Subtracts the value at the specified memory address from the reigtser A.
        /// </summary>
        private void SUB_M(ushort addr)
        {
            byte temp1 = (byte)(-_memory.read_byte(addr));

            auxiliary_carry_flag_set(regs.A, (byte)(regs.A + temp1));
            sub_carry_flag_set(regs.A, temp1);

            regs.A += temp1;

            zero_flag_set(regs.A);
            set_negative_flag(true);
            pc++;
            next_opcode = _memory.read_byte(pc);
        }
        #endregion

        #region SUB m
        /// <summary>
        /// Subtracts the immediate value at the specified address from the register A.
        /// </summary>
        private void SUB_M2(ushort addr)
        {
            byte temp1 = (byte)(-_memory.read_byte(addr));

            auxiliary_carry_flag_set(regs.A, (byte)(regs.A + temp1));
            sub_carry_flag_set(regs.A, temp1);

            regs.A += temp1;

            zero_flag_set(regs.A);
            set_negative_flag(true);
            pc += 2;
            next_opcode = _memory.read_byte(pc);
        }
        #endregion

        #region SBC r
        /// <summary>
        /// Subtracts the specified register plus the value of the carry flag from the register A.
        /// </summary>
        private void SBC_R(ref byte reg)
        {
            byte temp1 = (byte)(-reg + (regs.F >> 4 & 0x1));

            auxiliary_carry_flag_set(regs.A, (byte)(regs.A + temp1));
            sub_carry_flag_set(regs.A, temp1);

            regs.A += temp1;

            zero_flag_set(regs.A);
            set_negative_flag(true);
            pc++;
            next_opcode = _memory.read_byte(pc);
        }
        #endregion

        #region SBC m
        /// <summary>
        /// Subtracts the value at the specified memory address plus the value fo the carry flag from the register A.
        /// </summary>
        private void SBC_M(ushort addr)
        {
            byte temp1 = (byte)(-_memory.read_byte(addr) + (regs.F >> 4 & 0x1));

            auxiliary_carry_flag_set(regs.A, (byte)(regs.A + temp1));
            sub_carry_flag_set(regs.A, temp1);

            regs.A += temp1;

            zero_flag_set(regs.A);
            set_negative_flag(true);
            pc++;
            next_opcode = _memory.read_byte(pc);
        }
        #endregion

        #region SBC m
        /// <summary>
        /// Subtracts the immediate value at the specified address plus the carry flag from the register A.
        /// </summary>
        private void SBC_M2(ushort addr)
        {
            byte temp1 = (byte)(-_memory.read_byte(addr) + (regs.F >> 4 & 0x1));

            auxiliary_carry_flag_set(regs.A, (byte)(regs.A + temp1));
            sub_carry_flag_set(regs.A, temp1);

            regs.A += temp1;

            zero_flag_set(regs.A);
            set_negative_flag(true);
            pc += 2;
            next_opcode = _memory.read_byte(pc);
        }
        #endregion

        #region AND r
        /// <summary>
        /// Logically AND the specified register with the register A.
        /// </summary>
        private void AND_R(ref byte reg)
        {
            regs.A &= reg;

            set_auxiliary_carry_flag(true);
            set_carry_flag(false);
            zero_flag_set(regs.A);
            set_negative_flag(false);
            pc++;
            next_opcode = _memory.read_byte(pc);
        }
        #endregion

        #region AND m
        /// <summary>
        /// Logically AND the byte at the specified memory address with the register A.
        /// </summary>
        private void AND_M(ushort addr)
        {
            byte temp = _memory.read_byte(addr);

            regs.A &= temp;

            set_auxiliary_carry_flag(true);
            set_carry_flag(false);
            zero_flag_set(regs.A);
            set_negative_flag(false);
            pc++;
            next_opcode = _memory.read_byte(pc);
        }
        #endregion

        #region AND m
        /// <summary>
        /// Logically AND the immediate value at the specified memory address with the register A.
        /// </summary>
        private void AND_M2(ushort addr)
        {
            byte temp = _memory.read_byte(addr);

            regs.A &= temp;

            set_auxiliary_carry_flag(true);
            set_carry_flag(false);
            zero_flag_set(regs.A);
            set_negative_flag(false);
            pc += 2;
            next_opcode = _memory.read_byte(pc);
        }
        #endregion

        #region OR r
        /// <summary>
        /// Logically OR the specified register with the register A.
        /// </summary>
        private void OR_R(ref byte reg)
        {
            regs.A |= reg;

            set_auxiliary_carry_flag(false);
            set_carry_flag(false);
            set_negative_flag(false);
            zero_flag_set(regs.A);
            pc++;
            next_opcode = _memory.read_byte(pc);
        }
        #endregion

        #region OR m
        /// <summary>
        /// Logically OR the byte at the specified memory address with the register A.
        /// </summary>
        private void OR_M(ushort addr)
        {
            byte temp = _memory.read_byte(addr);

            regs.A |= temp;

            set_auxiliary_carry_flag(false);
            set_carry_flag(false);
            set_negative_flag(false);
            zero_flag_set(regs.A);
            pc++;
            next_opcode = _memory.read_byte(pc);
        }
        #endregion

        #region OR m
        /// <summary>
        /// Logically OR the immediate value at the specified memory address with the register A.
        /// </summary>
        private void OR_M2(ushort addr)
        {
            byte temp = _memory.read_byte(addr);

            regs.A |= temp;

            set_auxiliary_carry_flag(false);
            set_carry_flag(false);
            set_negative_flag(false);
            zero_flag_set(regs.A);
            pc += 2;
            next_opcode = _memory.read_byte(pc);
        }
        #endregion

        #region XOR r
        /// <summary>
        /// Logically exclusive OR the specified register with the register A.
        /// </summary>
        private void XOR_R(ref byte reg)
        {
            regs.A ^= reg;

            set_auxiliary_carry_flag(false);
            set_carry_flag(false);
            set_negative_flag(false);
            zero_flag_set(regs.A);
            pc++;
            next_opcode = _memory.read_byte(pc);
        }
        #endregion

        #region XOR m
        /// <summary>
        /// Logically exclusive OR the byte at the specified memory address with the register A.
        /// </summary>
        private void XOR_M(ushort addr)
        {
            byte temp = _memory.read_byte(addr);

            regs.A ^= temp;

            set_auxiliary_carry_flag(false);
            set_carry_flag(false);
            set_negative_flag(false);
            zero_flag_set(regs.A);
            pc++;
            next_opcode = _memory.read_byte(pc);
        }
        #endregion

        #region XOR m
        /// <summary>
        /// Logically exclusive OR the immediate value at the specified memory address with the register A.
        /// </summary>
        private void XOR_M2(ushort addr)
        {
            byte temp = _memory.read_byte(addr);

            regs.A ^= temp;

            set_auxiliary_carry_flag(false);
            set_carry_flag(false);
            set_negative_flag(false);
            zero_flag_set(regs.A);
            pc += 2;
            next_opcode = _memory.read_byte(pc);
        }
        #endregion

        #region CP r
        /// <summary>
        /// Compares the specified register with the register A.
        /// </summary>
        private void CP_R(ref byte reg)
        {
            byte temp1 = (byte)(-reg);
            byte temp2 = (byte)(regs.A + temp1);

            auxiliary_carry_flag_set(regs.A, (byte)(regs.A + temp1));
            sub_carry_flag_set(regs.A, temp1);
            zero_flag_set(temp2);
            set_negative_flag(true);
            pc++;
            next_opcode = _memory.read_byte(pc);
        }
        #endregion

        #region CP m
        /// <summary>
        /// Compares the byte at the specified memory address with the register A.
        /// </summary>
        private void CP_M(ushort addr)
        {
            byte temp1 = (byte)(-_memory.read_byte(addr));
            byte temp2 = (byte)(regs.A + temp1);

            auxiliary_carry_flag_set(regs.A, (byte)(regs.A + temp1));
            sub_carry_flag_set(regs.A, temp1);
            zero_flag_set(temp2);
            set_negative_flag(true);
            pc++;
            next_opcode = _memory.read_byte(pc);
        }
        #endregion

        #region CP m
        /// <summary>
        /// Compares the immediate value at the specified memory address with the register A.
        /// </summary>
        private void CP_M2(ushort addr)
        {
            byte temp1 = (byte)(_memory.read_byte(addr));
            byte temp2 = (byte)(regs.A + (-temp1));

            auxiliary_carry_flag_set(regs.A, (byte)(regs.A + (-temp1)));
            sub_carry_flag_set(regs.A, temp1);
            zero_flag_set(temp2);
            set_negative_flag(true);
            pc += 2;
            next_opcode = _memory.read_byte(pc);
        }
        #endregion

        #region INC r
        /// <summary>
        /// Increments the specified register.
        /// </summary>
        private void INC_R(ref byte reg)
        {
            byte temp1 = reg;

            reg++;

            if ((temp1 & 0xF) > (reg & 0xF))
            {
                set_auxiliary_carry_flag(true);
            }
            else
            {
                set_auxiliary_carry_flag(false);
            }

            zero_flag_set(reg);
            set_negative_flag(false);
            pc++;
            next_opcode = _memory.read_byte(pc);
        }
        #endregion

        #region INC m
        /// <summary>
        /// Increments the byte at the specified memory address.
        /// </summary>
        private void INC_M(ushort addr)
        {
            byte temp1 = _memory.read_byte(addr);
            byte temp2 = temp1;

            temp2++;

            if ((temp1 & 0xF) > (temp2 & 0xF))
            {
                set_auxiliary_carry_flag(true);
            }
            else
            {
                set_auxiliary_carry_flag(false);
            }

            zero_flag_set(temp2);
            _memory.write_byte(addr, temp2);
            pc++;
            next_opcode = _memory.read_byte(pc);
        }
        #endregion

        #region DEC r
        /// <summary>
        /// Decrements the specified register.
        /// </summary>
        private void DEC_R(ref byte reg)
        {
            byte temp1 = 1;
            temp1 = (byte)(-temp1);

            if (((reg & 0xF) + (temp1 & 0xF)) > 15)
            {
                set_auxiliary_carry_flag(true);
            }
            else
            {
                set_auxiliary_carry_flag(false);
            }

            reg--;

            zero_flag_set(reg);
            set_negative_flag(true);
            pc++;
            next_opcode = _memory.read_byte(pc);
        }
        #endregion

        #region DEC m
        /// <summary>
        /// Decrements the byte at the specified memory address.
        /// </summary>
        private void DEC_M(ushort addr)
        {
            byte temp1 = 1;
            temp1 = (byte)(-temp1);
            byte temp2 = _memory.read_byte(addr);

            if (((temp2 & 0xF) + (temp1 & 0xF)) > 15)
            {
                set_auxiliary_carry_flag(true);
            }
            else
            {
                set_auxiliary_carry_flag(false);
            }

            temp2--;

            zero_flag_set(temp2);
            set_negative_flag(true);
            _memory.write_byte(addr, temp2);
            pc++;
            next_opcode = _memory.read_byte(pc);
        }
        #endregion

        #region ADD rr
        /// <summary>
        /// Adds the specified ushort register to the register HL.
        /// </summary>
        private void ADD_RR(ref ushort reg)
        {
            auxiliary_carry_flag_set(regs.HL, reg);
            dad_carry_flag_set((uint)(regs.HL + reg));

            regs.HL += reg;

            set_negative_flag(false);
            pc++;
            next_opcode = _memory.read_byte(pc);
        }
        #endregion

        #region ADD sp, n
        /// <summary>
        /// Adds the immediate value at the specified memory address to the stack pointer.
        /// </summary>
        private void ADD_SP_N()
        {
            sbyte temp = (sbyte)(_memory.read_byte((ushort)(pc + 1)));
            ushort temp_s = sp;
            set_zero_flag(false);
            set_negative_flag(false);
            dad_carry_flag_set((uint)(temp_s + temp));
            if ((sp & 0xFFF) > ((temp_s + temp) & 0xFFF))
            {
                set_auxiliary_carry_flag(true);
            }
            else
            {
                set_auxiliary_carry_flag(false);
            }

            sp = (ushort)(sp + temp);
            pc += 2;
            next_opcode = _memory.read_byte(pc);
        }
        #endregion

        #region INC rr
        /// <summary>
        /// Increments the specified ushort register.
        /// </summary>
        private void INC_RR(ref ushort reg)
        {
            reg++;
            pc++;
            next_opcode = _memory.read_byte(pc);
        }
        #endregion

        #region DEC rr
        /// <summary>
        /// Decrements the specified ushort register.
        /// </summary>
        private void DEC_RR(ref ushort reg)
        {
            reg--;
            pc++;
            next_opcode = _memory.read_byte(pc);
        }
        #endregion

        #region SWAP r
        /// <summary>
        /// Swaps the nibbles of the specified register.
        /// </summary>
        private void SWAP_R(ref byte reg)
        {
            byte temp1 = reg;
            byte temp2 = (byte)(temp1 >> 4);

            temp1 = (byte)((temp1 << 4) | temp2);
            reg = temp1;

            set_negative_flag(false);
            set_carry_flag(false);
            set_auxiliary_carry_flag(false);
            zero_flag_set(reg);
            pc++;
            next_opcode = _memory.read_byte(pc);
        }
        #endregion

        #region SWAP m
        /// <summary>
        /// Swaps the nibbles of the byte at the specified memory address.
        /// </summary>
        private void SWAP_M(ushort addr)
        {
            byte temp1 = _memory.read_byte(addr);
            byte temp2 = (byte)(temp1 >> 4);

            temp1 = (byte)((temp1 << 4) | temp2);
            _memory.write_byte(addr, temp1);

            set_negative_flag(false);
            set_carry_flag(false);
            set_auxiliary_carry_flag(false);
            zero_flag_set(temp1);
            pc++;
            next_opcode = _memory.read_byte(pc);
        }
        #endregion

        #region DAA
        /// <summary>
        /// Decimal Adjust register A.
        /// </summary>
        private void DAA()
        {
            if (((regs.A & 0xF) > 0x09) || (check_auxiliary_flag()))
            {
                byte temp1 = (byte)((regs.A & 0xF) + 6);
                regs.A += 6;
            }
            if ((regs.A >> 4 & 0xF) > 0x09 || check_carry_flag())
            {
                normal_carry_flag_set((ushort)(regs.A + 96));

                regs.A += 96;
            }

            set_auxiliary_carry_flag(false);
            zero_flag_set(regs.A);
            pc++;
            next_opcode = _memory.read_byte(pc);
        }
        #endregion

        #region CPL
        /// <summary>
        /// Compliment register A.
        /// </summary>
        private void CPL()
        {
            regs.A = (byte)~(regs.A);

            set_auxiliary_carry_flag(true);
            set_negative_flag(true);
            pc++;
            next_opcode = _memory.read_byte(pc);
        }
        #endregion

        #region CCF
        /// <summary>
        /// Compliment carry flag.
        /// </summary>
        private void CCF()
        {
            regs.F ^= 0x10;

            set_negative_flag(false);
            set_auxiliary_carry_flag(false);
            pc++;
            next_opcode = _memory.read_byte(pc);
        }
        #endregion

        #region SCF
        /// <summary>
        /// Set carry flag.
        /// </summary>
        private void SCF()
        {
            set_carry_flag(true);

            set_negative_flag(false);
            set_auxiliary_carry_flag(false);
            pc++;
            next_opcode = _memory.read_byte(pc);
        }
        #endregion

        #region NOP
        /// <summary>
        /// No operation.
        /// </summary>
        private void NOP()
        {
            pc++;
            next_opcode = _memory.read_byte(pc);
        }
        #endregion

        #region HALT
        /// <summary>
        /// Halt cpu until an interrupt occurs.
        /// </summary>
        private void HALT()
        {
            pc++;
            if (IME)
            {
                halted = true;
            }
            next_opcode = _memory.read_byte(pc);
        }
        #endregion

        #region STOP
        /// <summary>
        /// Halt cpu and lcd display until key is pressed. (not finished)
        /// </summary>
        private void STOP()
        {
            pc += 2;
            next_opcode = _memory.read_byte(pc);
        }
        #endregion

        #region DI
        /// <summary>
        /// Disable interrupts.
        /// </summary>
        private void DI()
        {
            disable_int = 2;
            pc++;
            next_opcode = _memory.read_byte(pc);
        }
        #endregion

        #region EI
        /// <summary>
        /// Enable interrupts.
        /// </summary>
        private void EI()
        {
            enable_int = 2;
            pc++;
            next_opcode = _memory.read_byte(pc);
        }
        #endregion

        #region RLCA
        /// <summary>
        /// Rotate A left.
        /// </summary>
        private void RLCA()
        {
            set_carry_flag(false);
            set_auxiliary_carry_flag(false);
            set_negative_flag(false);

            regs.F |= (byte)((regs.A >> 7 & 0x1) << 4);
            regs.A <<= 1;
            regs.A |= (byte)((regs.F & 0x10) >> 4);

            zero_flag_set(regs.A);
            pc++;
            next_opcode = _memory.read_byte(pc);
        }
        #endregion

        #region RLA
        /// <summary>
        /// Rotate A left through carry flag.
        /// </summary>
        private void RLA()
        {
            byte temp1 = (byte)((regs.F & 0x10) >> 4);

            set_carry_flag(false);
            set_auxiliary_carry_flag(false);
            set_negative_flag(false);

            regs.F |= (byte)((regs.A >> 7 & 0x1) << 4);
            regs.A <<= 1;
            regs.A |= temp1;

            zero_flag_set(regs.A);
            pc++;
            next_opcode = _memory.read_byte(pc);
        }
        #endregion

        #region RRCA
        /// <summary>
        /// Rotate A right.
        /// </summary>
        private void RRCA()
        {
            set_carry_flag(false);
            set_negative_flag(false);
            set_auxiliary_carry_flag(false);

            regs.F |= (byte)((regs.A & 0x1) << 4);
            regs.A >>= 1;
            regs.A |= (byte)((regs.F & 0x10) << 3);

            zero_flag_set(regs.A);
            pc++;
            next_opcode = _memory.read_byte(pc);
        }
        #endregion

        #region RRA
        /// <summary>
        /// Rotate A right through carry flag.
        /// </summary>
        private void RRA()
        {
            byte temp1 = (byte)(regs.F & 0x10);

            set_carry_flag(false);
            set_negative_flag(false);
            set_auxiliary_carry_flag(false);

            regs.F |= (byte)((regs.A & 0x1) << 4);
            regs.A >>= 1;
            regs.A |= (byte)(temp1 << 3);

            zero_flag_set(regs.A);
            pc++;
            next_opcode = _memory.read_byte(pc);
        }
        #endregion

        #region RLC r
        /// <summary>
        /// Rotate the specified register left.
        /// </summary>
        private void RLC_R(ref byte reg)
        {
            set_carry_flag(false);
            set_auxiliary_carry_flag(false);
            set_negative_flag(false);

            regs.F |= (byte)((reg >> 7 & 0x1) << 4);
            reg <<= 1;
            reg |= (byte)((regs.F & 0x10) >> 4);

            zero_flag_set(reg);
            pc += 2;
            next_opcode = _memory.read_byte(pc);
        }
        #endregion

        #region RLC m
        /// <summary>
        /// Rotate the byte at the specified memory location left.
        /// </summary>
        private void RLC_M(ushort addr)
        {
            byte temp1 = _memory.read_byte(addr);

            set_carry_flag(false);
            set_auxiliary_carry_flag(false);
            set_negative_flag(false);

            regs.F |= (byte)((temp1 >> 7 & 0x1) << 4);
            temp1 <<= 1;
            temp1 |= (byte)((regs.F & 0x10) >> 4);

            zero_flag_set(temp1);

            _memory.write_byte(addr, temp1);
            pc += 2;
            next_opcode = _memory.read_byte(pc);
        }
        #endregion

        #region RL r
        /// <summary>
        /// Rotate the specified register left through the carry flag.
        /// </summary>
        private void RL_R(ref byte reg)
        {
            byte temp1 = (byte)((regs.F & 0x10) >> 4);

            set_carry_flag(false);
            set_auxiliary_carry_flag(false);
            set_negative_flag(false);

            regs.F |= (byte)((reg >> 7 & 0x1) << 4);
            reg <<= 1;
            reg |= temp1;

            zero_flag_set(reg);
            pc += 2;
            next_opcode = _memory.read_byte(pc);
        }
        #endregion

        #region RL m
        /// <summary>
        /// Rotate the byte at the memory address left through the carry flag.
        /// </summary>
        private void RL_M(ushort addr)
        {
            byte temp1 = (byte)((regs.F & 0x10) >> 4);
            byte temp2 = _memory.read_byte(addr);

            set_carry_flag(false);
            set_auxiliary_carry_flag(false);
            set_negative_flag(false);

            regs.F |= (byte)((temp2 >> 7 & 0x1) << 4);
            temp2 <<= 1;
            temp2 |= temp1;

            zero_flag_set(temp2);

            _memory.write_byte(addr, temp2);
            pc += 2;
            next_opcode = _memory.read_byte(pc);
        }
        #endregion

        #region RRC r
        /// <summary>
        /// Rotate the specified register right.
        /// </summary>
        private void RRC_R(ref byte reg)
        {
            set_carry_flag(false);
            set_negative_flag(false);
            set_auxiliary_carry_flag(false);

            regs.F |= (byte)((reg & 0x1) << 4);
            reg >>= 1;
            reg |= (byte)((regs.F & 0x10) << 3);

            zero_flag_set(reg);
            pc += 2;
            next_opcode = _memory.read_byte(pc);
        }
        #endregion

        #region RRC m
        /// <summary>
        /// Rotate the byte at the specified memory location right.
        /// </summary>
        private void RRC_M(ushort addr)
        {
            byte temp1 = _memory.read_byte(addr);

            set_carry_flag(false);
            set_negative_flag(false);
            set_auxiliary_carry_flag(false);

            regs.F |= (byte)((temp1 & 0x1) << 4);
            temp1 >>= 1;
            temp1 |= (byte)((regs.F & 0x10) << 3);

            zero_flag_set(temp1);
            _memory.write_byte(addr, temp1);
            pc += 2;
            next_opcode = _memory.read_byte(pc);
        }
        #endregion

        #region RR r
        /// <summary>
        /// Rotate the specified register right through the carry flag.
        /// </summary>
        private void RR_R(ref byte reg)
        {
            byte temp1 = (byte)(regs.F & 0x10);

            set_carry_flag(false);
            set_negative_flag(false);
            set_auxiliary_carry_flag(false);

            regs.F |= (byte)((reg & 0x1) << 4);
            reg >>= 1;
            reg |= (byte)(temp1 << 3);

            zero_flag_set(reg);
            pc += 2;
            next_opcode = _memory.read_byte(pc);
        }
        #endregion

        #region RR m
        /// <summary>
        /// Rotate the byte at the memory address right through the carry flag.
        /// </summary>
        private void RR_M(ushort addr)
        {
            byte temp1 = (byte)(regs.F & 0x10);
            byte temp2 = _memory.read_byte(addr);

            set_carry_flag(false);
            set_negative_flag(false);
            set_auxiliary_carry_flag(false);

            regs.F |= (byte)((temp2 & 0x1) << 4);
            temp2 >>= 1;
            temp2 |= (byte)(temp1 << 3);

            zero_flag_set(temp2);
            _memory.write_byte(addr, temp2);
            pc += 2;
            next_opcode = _memory.read_byte(pc);
        }
        #endregion

        #region SLA r
        /// <summary>
        /// Shift the specified register left into the carry flag.
        /// </summary>
        private void SLA_R(ref byte reg)
        {
            set_carry_flag(false);
            set_auxiliary_carry_flag(false);
            set_negative_flag(false);

            regs.F |= (byte)((reg >> 7 & 0x1) << 4);
            reg <<= 1;

            zero_flag_set(reg);
            pc += 2;
            next_opcode = _memory.read_byte(pc);
        }
        #endregion

        #region SLA m
        /// <summary>
        /// Shift the byte at the specified memory address left into the carry flag.
        /// </summary>
        private void SLA_M(ushort addr)
        {
            byte temp1 = _memory.read_byte(addr);

            set_carry_flag(false);
            set_auxiliary_carry_flag(false);
            set_negative_flag(false);

            regs.F |= (byte)((temp1 >> 7 & 0x1) << 4);
            temp1 <<= 1;

            zero_flag_set(temp1);

            _memory.write_byte(addr, temp1);
            pc += 2;
            next_opcode = _memory.read_byte(pc);
        }
        #endregion

        #region SRA r
        /// <summary>
        /// Shift the specified register right into the carry flag, sign does not change.
        /// </summary>
        private void SRA_R(ref byte reg)
        {
            byte temp1 = reg;

            set_carry_flag(false);
            set_negative_flag(false);
            set_auxiliary_carry_flag(false);

            regs.F |= (byte)((reg & 0x1) << 4);
            reg >>= 1;
            reg |= (byte)(temp1 & 0x80);

            zero_flag_set(reg);
            pc += 2;
            next_opcode = _memory.read_byte(pc);
        }
        #endregion

        #region SRA m
        /// <summary>
        /// Shift the byte at the specified memory address right into the carry flag, sign does not change.
        /// </summary>
        private void SRA_M(ushort addr)
        {
            byte temp1 = _memory.read_byte(addr);
            byte temp2 = temp1;

            set_carry_flag(false);
            set_negative_flag(false);
            set_auxiliary_carry_flag(false);

            regs.F |= (byte)((temp1 & 0x1) << 4);
            temp1 >>= 1;
            temp1 |= (byte)(temp2 & 0x80);

            zero_flag_set(temp1);
            _memory.write_byte(addr, temp1);
            pc += 2;
            next_opcode = _memory.read_byte(pc);
        }
        #endregion

        #region SRL r
        /// <summary>
        /// Shift the specified register right into the carry flag.
        /// </summary>
        private void SRL_R(ref byte reg)
        {
            byte temp1 = reg;

            set_carry_flag(false);
            set_negative_flag(false);
            set_auxiliary_carry_flag(false);

            regs.F |= (byte)((reg & 0x1) << 4);
            reg >>= 1;

            zero_flag_set(reg);
            pc += 2;
            next_opcode = _memory.read_byte(pc);
        }
        #endregion

        #region SRL m
        /// <summary>
        /// Shift the byte at the specified memory address right into the carry flag.
        /// </summary>
        private void SRL_M(ushort addr)
        {
            byte temp1 = _memory.read_byte(addr);
            byte temp2 = temp1;

            set_carry_flag(false);
            set_negative_flag(false);
            set_auxiliary_carry_flag(false);

            regs.F |= (byte)((temp1 & 0x1) << 4);
            temp1 >>= 1;

            zero_flag_set(temp1);
            _memory.write_byte(addr, temp1);
            pc += 2;
            next_opcode = _memory.read_byte(pc);
        }
        #endregion

        #region BIT b, r
        /// <summary>
        /// Test the specified bit in the specified register.
        /// </summary>
        void BIT_B_R(byte bit, ref byte reg)
        {
            zero_flag_set((byte)(reg & (0x1 << bit)));
            set_negative_flag(false);
            set_auxiliary_carry_flag(true);
            pc += 2;
            next_opcode = _memory.read_byte(pc);
        }
        #endregion

        #region BIT b, m
        /// <summary>
        /// Test the specified bit in the byte at the specified memory address.
        /// </summary>
        void BIT_B_M(byte bit, ushort addr)
        {

            zero_flag_set((byte)(_memory.read_byte(addr) & (0x1 << bit)));
            set_negative_flag(false);
            set_auxiliary_carry_flag(true);
            pc += 2;
            next_opcode = _memory.read_byte(pc);
        }
        #endregion

        #region SET b, r
        /// <summary>
        /// Set the specified bit in the specified register.
        /// </summary>
        void SET_B_R(byte bit, ref byte reg)
        {
            if ((reg & (0x1 << bit)) == 0x0)
            {
                reg ^= (byte)(0x1 << bit);
            }
            pc += 2;
            next_opcode = _memory.read_byte(pc);
        }
        #endregion

        #region SET b, m
        /// <summary>
        /// Set the specified bit in the byte at the specified memory address.
        /// </summary>
        void SET_B_M(byte bit, ushort addr)
        {
            byte temp1 = _memory.read_byte(addr);

            if ((temp1 & (0x1 << bit)) == 0x0)
            {
                temp1 ^= (byte)(0x1 << bit);
            }

            _memory.write_byte(addr, temp1);
            pc += 2;
            next_opcode = _memory.read_byte(pc);
        }
        #endregion

        #region RES b, r
        /// <summary>
        /// Reset the specified bit in the specified register.
        /// </summary>
        void RES_B_R(byte bit, ref byte reg)
        {
            if ((reg & (0x1 << bit)) != 0x0)
            {
                reg ^= (byte)(0x1 << bit);
            }
            pc += 2;
            next_opcode = _memory.read_byte(pc);
        }
        #endregion

        #region RES b, m
        /// <summary>
        /// Reset the specified bit in the byte at the specified memory address.
        /// </summary>
        void RES_B_M(byte bit, ushort addr)
        {
            byte temp1 = _memory.read_byte(addr);

            if ((temp1 & (0x1 << bit)) != 0x0)
            {
                temp1 ^= (byte)(0x1 << bit);
            }

            _memory.write_byte(addr, temp1);
            pc += 2;
            next_opcode = _memory.read_byte(pc);
        }
        #endregion

        #region JP nn
        /// <summary>
        /// Jump to the specified memory address.
        /// </summary>
        void JP_NN()
        {
            pc = (ushort)((_memory.read_byte((ushort)(pc + 2)) << 8) | _memory.read_byte((ushort)(pc + 1)));
            next_opcode = _memory.read_byte(pc);
        }
        #endregion

        #region JP c, nn
        /// <summary>
        /// Jump to the specified memory address when carry flag is set.
        /// </summary>
        void JP_C_NN()
        {
            if (check_carry_flag())
            {
                pc = (ushort)((_memory.read_byte((ushort)(pc + 2)) << 8) | _memory.read_byte((ushort)(pc + 1)));
                next_opcode = _memory.read_byte(pc);
            }
            else
            {
                pc += 3;
                next_opcode = _memory.read_byte(pc);
            }
        }
        #endregion

        #region JP nc, nn
        /// <summary>
        /// Jump to the specified address when the carry flag is not set.
        /// </summary>
        void JP_NC_NN()
        {
            if (!check_carry_flag())
            {
                pc = (ushort)((_memory.read_byte((ushort)(pc + 2)) << 8) | _memory.read_byte((ushort)(pc + 1)));
                next_opcode = _memory.read_byte(pc);
            }
            else
            {
                pc += 3;
                next_opcode = _memory.read_byte(pc);
            }
        }
        #endregion

        #region JP nz, nn
        /// <summary>
        /// Jump to the specified address when the zero flag is not set.
        /// </summary>
        void JP_NZ_NN()
        {
            if (!check_zero_flag())
            {
                pc = (ushort)((_memory.read_byte((ushort)(pc + 2)) << 8) | _memory.read_byte((ushort)(pc + 1)));
                next_opcode = _memory.read_byte(pc);
            }
            else
            {
                pc += 3;
                next_opcode = _memory.read_byte(pc);
            }
        }
        #endregion

        #region JP z. nn
        /// <summary>
        /// Jump to the specified address when the zero flag is set.
        /// </summary>
        void JP_Z_NN()
        {
            if (check_zero_flag())
            {
                pc = (ushort)((_memory.read_byte((ushort)(pc + 2)) << 8) | _memory.read_byte((ushort)(pc + 1)));
                next_opcode = _memory.read_byte(pc);
            }
            else
            {
                pc += 3;
                next_opcode = _memory.read_byte(pc);
            }
        }
        #endregion

        #region JP {hl}
        /// <summary>
        /// Jump to the specified address in the register HL.
        /// </summary>
        void JP_HL()
        {
            pc = regs.HL;
            next_opcode = _memory.read_byte(pc);
        }
        #endregion

        #region JR n
        /// <summary>
        /// Add the specified signed immediate value to the program counter and jump to the address.
        /// </summary>
        void JR_N()
        {
            pc += 2;
            sbyte temp1 = (sbyte)( _memory.read_byte((ushort)(pc - 1)));
            pc = (ushort)(pc + temp1);
            next_opcode = _memory.read_byte(pc);
        }
        #endregion

        #region JR c, n
        /// <summary>
        /// Add the specified signed immediate value to the program counter and jump to the address when the carry flag is set.
        /// </summary>
        void JR_C_N()
        {
            pc += 2;
            if (check_carry_flag())
            {
                sbyte temp1 = (sbyte)(_memory.read_byte((ushort)(pc - 1)));
                pc = (ushort)(pc + temp1);
                next_opcode = _memory.read_byte(pc);
            }
            else
            {
                next_opcode = _memory.read_byte(pc);
            }
        }
        #endregion

        #region JR nc, n
        /// <summary>
        /// Add the specified signed immediate value to the program counter and jump to the address when the carry flag is not set.
        /// </summary>
        void JR_NC_N()
        {
            pc += 2;
            if (!check_carry_flag())
            {
                sbyte temp1 = (sbyte)(_memory.read_byte((ushort)(pc - 1)));
                pc = (ushort)(pc + temp1);
                next_opcode = _memory.read_byte(pc);
            }
            else
            {
                next_opcode = _memory.read_byte(pc);
            }
        }
        #endregion

        #region JR nz, n
        /// <summary>
        /// Add the specified signed immediate value to the program counter and jump to the address when the zero flag is not set.
        /// </summary>
        void JR_NZ_N()
        {
            pc += 2;
            if (!check_zero_flag())
            {
                sbyte temp1 = (sbyte)(_memory.read_byte((ushort)(pc - 1)));
                pc = (ushort)(pc + temp1);
                next_opcode = _memory.read_byte(pc);                
            }
            else
            {
                next_opcode = _memory.read_byte(pc);
            }
        }
        #endregion

        #region JR z, n
        /// <summary>
        /// Add the specified signed immediate value to the program counter and jump to the address when the zero flag is set.
        /// </summary>
        void JR_Z_N()
        {
            pc += 2;
            if (check_zero_flag())
            {
                sbyte temp1 = (sbyte)(_memory.read_byte((ushort)(pc - 1)));
                pc = (ushort)(pc + temp1);
                next_opcode = _memory.read_byte(pc);                
            }
            else
            {
                next_opcode = _memory.read_byte(pc);
            }
        }
        #endregion

        #region CALL nn
        /// <summary>
        /// Push the next instruction onto the stack and jump to the specified address.
        /// </summary>
        void CALL_NN()
        {
            pc += 3;
            _memory.write_byte((ushort)(sp - 1), (byte)(pc & 0xFF));
            _memory.write_byte((ushort)(sp - 2), (byte)(pc >> 8 & 0xFF));
            sp -= 2;
            pc = (ushort)((_memory.read_byte((ushort)(pc - 1)) << 8) | (_memory.read_byte((ushort)(pc - 2))));
            next_opcode = _memory.read_byte(pc);
        }
        #endregion

        #region CALL c, nn
        /// <summary>
        /// Push the next instruction onto the stack and jump to the specified address when the carry flag is set.
        /// </summary>
        void CALL_C_NN()
        {
            pc += 3;
            next_opcode = _memory.read_byte(pc);
            
            if (check_carry_flag())
            {
                _memory.write_byte((ushort)(sp - 1), (byte)(pc & 0xFF));
                _memory.write_byte((ushort)(sp - 2), (byte)(pc >> 8 & 0xFF));
                sp -= 2;
                pc = (ushort)((_memory.read_byte((ushort)(pc - 1)) << 8) | (_memory.read_byte((ushort)(pc - 2))));
                next_opcode = _memory.read_byte(pc);
            }
        }
        #endregion

        #region CALL nc, nn
        /// <summary>
        /// Push the next instruction onto the stack and jump to the specified address when the carry flag is not set.
        /// </summary>
        void CALL_NC_NN()
        {
            pc += 3;
            next_opcode = _memory.read_byte(pc);
            
            if (!check_carry_flag())
            {
                _memory.write_byte((ushort)(sp - 1), (byte)(pc & 0xFF));
                _memory.write_byte((ushort)(sp - 2), (byte)(pc >> 8 & 0xFF));
                sp -= 2;
                pc = (ushort)((_memory.read_byte((ushort)(pc - 1)) << 8) | (_memory.read_byte((ushort)(pc - 2))));
                next_opcode = _memory.read_byte(pc);
            }
        }
        #endregion

        #region CALL nz, nn
        /// <summary>
        /// Push the next instruction onto the stack and jump to the specified address when the zero flag is not set.
        /// </summary>
        void CALL_NZ_NN()
        {
            pc += 3;
            next_opcode = _memory.read_byte(pc);
            
            if (!check_zero_flag())
            {
                _memory.write_byte((ushort)(sp - 1), (byte)(pc & 0xFF));
                _memory.write_byte((ushort)(sp - 2), (byte)(pc >> 8 & 0xFF));
                sp -= 2;
                pc = (ushort)((_memory.read_byte((ushort)(pc - 1)) << 8) | (_memory.read_byte((ushort)(pc - 2))));
                next_opcode = _memory.read_byte(pc);
            }
        }
        #endregion

        #region CALL z, nn
        /// <summary>
        /// Push the next instruction onto the stack and jump to the specified address when the zero flag is set.
        /// </summary>
        void CALL_Z_NN()
        {
            pc += 3;
            next_opcode = _memory.read_byte(pc);
            
            if (check_zero_flag())
            {
                _memory.write_byte((ushort)(sp - 1), (byte)(pc & 0xFF));
                _memory.write_byte((ushort)(sp - 2), (byte)(pc >> 8 & 0xFF));
                sp -= 2;
                pc = (ushort)((_memory.read_byte((ushort)(pc - 1)) << 8) | (_memory.read_byte((ushort)(pc - 2))));
                next_opcode = _memory.read_byte(pc);
            }
        }
        #endregion

        #region RST n
        /// <summary>
        /// Push the next instruction onto the stack and jump to the specified interrupt.
        /// </summary>
        void RST_N(byte exp)
        {
            pc++;
            if (IME == true)
            {
                IME = false;
                _memory.write_byte((ushort)(sp - 1), (byte)(pc & 0xFF));
                _memory.write_byte((ushort)(sp - 2), (byte)(pc >> 8 & 0xFF));
                sp -= 2;

                pc = (ushort)(0x0000 + exp);
                next_opcode = _memory.read_byte(pc);
            }
            else
            {
                next_opcode = _memory.read_byte(pc);
            }
        }
        #endregion

        #region RET
        /// <summary>
        /// Pop the next instruction off of the stack and jump to the specified address.
        /// </summary>
        void RET()
        {
            pc = (ushort)((_memory.read_byte((ushort)(sp)) << 8) | (_memory.read_byte((ushort)(sp + 1))));
            sp += 2;
            next_opcode = _memory.read_byte(pc);
        }
        #endregion

        #region RET c
        /// <summary>
        /// Pop the next instruction off of the stack and jump to the specified address when the carry flag is set.
        /// </summary>
        void RET_C()
        {
            if (check_carry_flag())
            {
                pc = (ushort)((_memory.read_byte((ushort)(sp)) << 8) | (_memory.read_byte((ushort)(sp + 1))));
                sp += 2;
                next_opcode = _memory.read_byte(pc);
            }
            else
            {
                pc++;
                next_opcode = _memory.read_byte(pc);
            }
        }
        #endregion

        #region RET nc
        /// <summary>
        /// Pop the next instruction off of the stack and jump to the specified address when the carry flag is not set.
        /// </summary>
        void RET_NC()
        {
            if (!check_carry_flag())
            {
                pc = (ushort)((_memory.read_byte((ushort)(sp)) << 8) | (_memory.read_byte((ushort)(sp + 1))));
                sp += 2;
                next_opcode = _memory.read_byte(pc);
            }
            else
            {
                pc++;
                next_opcode = _memory.read_byte(pc);
            }
        }
        #endregion

        #region RET nz
        /// <summary>
        /// Pop the next instruction off of the stack and jump to the specified address when the zero flag is not set.
        /// </summary>
        void RET_NZ()
        {
            if (!check_zero_flag())
            {
                pc = (ushort)((_memory.read_byte((ushort)(sp)) << 8) | (_memory.read_byte((ushort)(sp + 1))));
                sp += 2;
                next_opcode = _memory.read_byte(pc);
            }
            else
            {
                pc++;
                next_opcode = _memory.read_byte(pc);
            }
        }
        #endregion

        #region RET z
        /// <summary>
        /// Pop the next instruction off of the stack and jump to the specified address when the zero flag is set.
        /// </summary>
        void RET_Z()
        {
            if (check_zero_flag())
            {
                pc = (ushort)((_memory.read_byte((ushort)(sp)) << 8) | (_memory.read_byte((ushort)(sp + 1))));
                sp += 2;
                next_opcode = _memory.read_byte(pc);
            }
            else
            {
                pc++;
                next_opcode = _memory.read_byte(pc);
            }
        }
        #endregion

        #region RETI
        /// <summary>
        /// Pop the next instruction off of the stack and jump to the specified address and enable interrupts.
        /// </summary>
        void RETI()
        {
            pc = (ushort)((_memory.read_byte((ushort)(sp)) << 8) | (_memory.read_byte((ushort)(sp + 1))));
            sp += 2;
            IME = true;
            next_opcode = _memory.read_byte(pc);
        }
        #endregion
    }
}
