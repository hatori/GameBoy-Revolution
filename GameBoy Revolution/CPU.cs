using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace GameBoy_Revolution
{
    public class CPU
    {
        #region Variables
        private Memory _memory;
        private Video _video;

        private byte[] cycles_lookup;
        private byte[] cycles_lookup_CB;
        private byte[] cycles_lookup_10;

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
        #endregion

        #region constructor
        public CPU(Form form1_reference, ref Memory memory, ref Video video/*, ref Sound sound, ref Input input*/)
        {
            _memory = memory;
            _video = video;
            cycles = 0;
            cyclesThisOp = 0;
            cyclesToDiv = 0;
            cyclesToTima = 0;
            Scanline_Counter = 0;

            regs.A = regs.B = regs.C = regs.D = regs.E = regs.H = regs.L = 0;
            regs.F = 0;
            pc = 0;
            sp = 0;
            opcode = 0;
            next_opcode = 0;
            disable_int = 0;
            enable_int = 0;
            IME = true;
            halted = false;
            stopped = false;

            cycles_lookup = new byte[] {
	        4,12,8,8,4,4,8,4,20,8,8,8,4,4,8,4,
            0,12,8,8,4,4,8,4,8,8,8,8,4,4,8,4,
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
            8,12,12,12,12,16,8,32,8,8,12,0,12,12,8,32,
            8,12,12,0,12,16,8,32,8,8,12,0,12,0,8,32,
            12,12,8,0,0,16,8,32,16,4,16,0,0,0,8,32,
            12,12,8,4,0,16,8,32,12,8,16,4,0,0,8,32,
            };

            cycles_lookup_CB = new byte[] {
	        8,8,8,8,8,8,16,8,8,8,8,8,8,8,16,8,
            8,8,8,8,8,8,16,8,8,8,8,8,8,8,16,8,
            8,8,8,8,8,8,16,8,8,8,8,8,8,8,16,8,
            8,8,8,8,8,8,16,8,8,8,8,8,8,8,16,8,
            8,8,8,8,8,8,16,8,8,8,8,8,8,8,16,8,
            8,8,8,8,8,8,16,8,8,8,8,8,8,8,16,8,
            8,8,8,8,8,8,16,8,8,8,8,8,8,8,16,8,
            8,8,8,8,8,8,16,8,8,8,8,8,8,8,16,8,
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

            Startup();
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
            _memory.write_ushort(sp, pc);
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
            update_lcd_status();
            byte currentline = 0;

            if (get_lcd_status())
            {
                Scanline_Counter -= cycles;
            }
            else
            {
                return;
            }

            if (Scanline_Counter <= 0)
            {
                Scanline_Counter = 456;

                _memory.Retrieve_LY++;
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
                            _video.set_pixel(i, currentline, (uint)_video.Colors[0]);
                            break;
                        }
                    case 1:
                        {
                            _video.set_pixel(i, currentline, (uint)_video.Colors[1]);
                            break;
                        }
                    case 2:
                        {
                            _video.set_pixel(i, currentline, (uint)_video.Colors[2]);
                            break;
                        }
                    case 3:
                        {
                            _video.set_pixel(i, currentline, (uint)_video.Colors[3]);
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
            int mode2 = 376;
            int mode3 = 204;

            if (!get_lcd_status())
            {
                status &= 252;
                status ^= 0x1;
                _memory.Retrieve_STAT = status;
                Scanline_Counter = 456;
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
            else
            {
                if (Scanline_Counter >= mode2)
                {
                    status &= 252;
                    status ^= 0x2;
                    if ((status & 0x20) != 0)
                    {
                        request_lcd_interrupt = true;
                    }
                    mode = 2;
                }
                else if (Scanline_Counter >= mode3)
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
                cycles = 69905 + remainder;
            }

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
        }
        #endregion

        #region Step
        public void step()
        {
            if (cycles <= 0)
            {
                int remainder = cycles;
                cycles = 69905 + remainder;
                _video.lock_texture();
            }

            if (cycles > 0)
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
                    cycles = 69905 + remainder;
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
        private void sub_carry_flag_set(ushort result)
        {
            if (result < 0x100)
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
        private void auxiliary_carry_flag_set(byte result)
        {
            if ((regs.A & 0xF) > (result & 0xF))
            {
                if (!check_auxiliary_flag())
                {
                    set_auxiliary_carry_flag(true);
                }
            }
            else
            {
                if (check_auxiliary_flag())
                {
                    set_auxiliary_carry_flag(false);
                }
            }
        }
        #endregion

        #region auxiliary carry set
        private void auxiliary_carry_flag_set(ushort result)
        {
            if ((regs.A & 0xFFF) > (result & 0xFFF))
            {
                if (!check_auxiliary_flag())
                {
                    set_auxiliary_carry_flag(true);
                }
            }
            else
            {
                if (check_auxiliary_flag())
                {
                    set_auxiliary_carry_flag(false);
                }
            }
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

        #region decode opcode
        private void Decode_Opcode()
        {
            cyclesThisOp = cycles_lookup[opcode];
            cycles -= cyclesThisOp;

            switch (opcode)
            {
                case 0x00:
                    {
                        NOP();
                        return;
                    }
                case 0x01:
                    {
                        LD_NN_RR(ref regs.BC);
                        return;
                    }
                case 0x02:
                    {
                        LD_R2_R1(ref regs.A, regs.BC);
                        return;
                    }
                case 0x03:
                    {
                        INC_RR(ref regs.BC);
                        return;
                    }
                case 0x04:
                    {
                        INC_R(ref regs.B);
                        return;
                    }
                case 0x05:
                    {
                        DEC_R(ref regs.B);
                        return;
                    }
                case 0x06:
                    {
                        LD_N_R(ref regs.B);
                        return;
                    }
                case 0x07:
                    {
                        RLCA();
                        return;
                    }
                case 0x08:
                    {
                        LD_MNN_RR(ref sp);
                        return;
                    }
                case 0x09:
                    {
                        ADD_RR(ref regs.BC);
                        return;
                    }
                case 0x0A:
                    {
                        LD_R2_R1(regs.BC, ref regs.A);
                        return;
                    }
                case 0x0B:
                    {
                        DEC_RR(ref regs.BC);
                        return;
                    }
                case 0x0C:
                    {
                        INC_R(ref regs.C);
                        return;
                    }
                case 0x0D:
                    {
                        DEC_R(ref regs.C);
                        return;
                    }
                case 0x0E:
                    {
                        LD_N_R(ref regs.C);
                        return;
                    }
                case 0x0F:
                    {
                        RRCA();
                        return;
                    }
                case 0x10:
                    {
                        Decode_Opcode_10(_memory.read_byte((ushort)(pc + 1)));
                        return;
                    }
                case 0x11:
                    {
                        LD_NN_RR(ref regs.DE);
                        return;
                    }
                case 0x12:
                    {
                        LD_R2_R1(ref regs.A, regs.DE);
                        return;
                    }
                case 0x13:
                    {
                        INC_RR(ref regs.DE);
                        return;
                    }
                case 0x14:
                    {
                        INC_R(ref regs.D);
                        return;
                    }
                case 0x15:
                    {
                        DEC_R(ref regs.D);
                        return;
                    }
                case 0x16:
                    {
                        LD_N_R(ref regs.D);
                        return;
                    }
                case 0x17:
                    {
                        RLA();
                        return;
                    }
                case 0x18:
                    {
                        JR_N();
                        return;
                    }
                case 0x19:
                    {
                        ADD_RR(ref regs.DE);
                        return;
                    }
                case 0x1A:
                    {
                        LD_R2_R1(regs.DE, ref regs.A);
                        return;
                    }
                case 0x1B:
                    {
                        DEC_RR(ref regs.DE);
                        return;
                    }
                case 0x1C:
                    {
                        INC_R(ref regs.E);
                        return;
                    }
                case 0x1D:
                    {
                        DEC_R(ref regs.E);
                        return;
                    }
                case 0x1E:
                    {
                        LD_N_R(ref regs.E);
                        return;
                    }
                case 0x1F:
                    {
                        RRA();
                        return;
                    }
                case 0x20:
                    {
                        JR_NZ_N();
                        return;
                    }
                case 0x21:
                    {
                        LD_NN_RR(ref regs.HL);
                        return;
                    }
                case 0x22:
                    {
                        LDI_R_M(ref regs.A, ref regs.HL);
                        return;
                    }
                case 0x23:
                    {
                        INC_RR(ref regs.HL);
                        return;
                    }
                case 0x24:
                    {
                        INC_R(ref regs.H);
                        return;
                    }
                case 0x25:
                    {
                        DEC_R(ref regs.H);
                        return;
                    }
                case 0x26:
                    {
                        LD_N_R(ref regs.H);
                        return;
                    }
                case 0x27:
                    {
                        DAA();
                        return;
                    }
                case 0x28:
                    {
                        JR_Z_N();
                        return;
                    }
                case 0x29:
                    {
                        ADD_RR(ref regs.HL);
                        return;
                    }
                case 0x2A:
                    {
                        LDI_M_R(ref regs.HL, ref regs.A);
                        return;
                    }
                case 0x2B:
                    {
                        DEC_RR(ref regs.HL);
                        return;
                    }
                case 0x2C:
                    {
                        INC_R(ref regs.L);
                        return;
                    }
                case 0x2D:
                    {
                        DEC_R(ref regs.L);
                        return;
                    }
                case 0x2E:
                    {
                        LD_N_R(ref regs.L);
                        return;
                    }
                case 0x2F:
                    {
                        CPL();
                        return;
                    }
                case 0x30:
                    {
                        JR_NC_N();
                        return;
                    }
                case 0x31:
                    {
                        LD_NN_RR(ref sp);
                        return;
                    }
                case 0x32:
                    {
                        LDD_R_M(ref regs.A, ref regs.HL);
                        return;
                    }
                case 0x33:
                    {
                        INC_RR(ref sp);
                        return;
                    }
                case 0x34:
                    {
                        INC_M(regs.HL);
                        return;
                    }
                case 0x35:
                    {
                        DEC_M(regs.HL);
                        return;
                    }
                case 0x36:
                    {
                        LD_R2_R1(regs.HL);
                        return;
                    }
                case 0x37:
                    {
                        SCF();
                        return;
                    }
                case 0x38:
                    {
                        JR_C_N();
                        return;
                    }
                case 0x39:
                    {
                        ADD_RR(ref sp);
                        return;
                    }
                case 0x3A:
                    {
                        LDD_M_R(ref regs.HL, ref regs.A);
                        return;
                    }
                case 0x3B:
                    {
                        DEC_RR(ref sp);
                        return;
                    }
                case 0x3C:
                    {
                        INC_R(ref regs.A);
                        return;
                    }
                case 0x3D:
                    {
                        DEC_R(ref regs.A);
                        return;
                    }
                case 0x3E:
                    {
                        LD_N_R(ref regs.A);
                        return;
                    }
                case 0x3F:
                    {
                        CCF();
                        return;
                    }
                case 0x40:
                    {
                        LD_R2_R1(ref regs.B, ref regs.B);
                        return;
                    }
                case 0x41:
                    {
                        LD_R2_R1(ref regs.C, ref regs.B);
                        return;
                    }
                case 0x42:
                    {
                        LD_R2_R1(ref regs.D, ref regs.B);
                        return;
                    }
                case 0x43:
                    {
                        LD_R2_R1(ref regs.E, ref regs.B);
                        return;
                    }
                case 0x44:
                    {
                        LD_R2_R1(ref regs.H, ref regs.B);
                        return;
                    }
                case 0x45:
                    {
                        LD_R2_R1(ref regs.L, ref regs.B);
                        return;
                    }
                case 0x46:
                    {
                        LD_R2_R1(regs.HL, ref regs.B);
                        return;
                    }
                case 0x47:
                    {
                        LD_R2_R1(ref regs.A, ref regs.B);
                        return;
                    }
                case 0x48:
                    {
                        LD_R2_R1(ref regs.B, ref regs.C);
                        return;
                    }
                case 0x49:
                    {
                        LD_R2_R1(ref regs.C, ref regs.C);
                        return;
                    }
                case 0x4A:
                    {
                        LD_R2_R1(ref regs.D, ref regs.C);
                        return;
                    }
                case 0x4B:
                    {
                        LD_R2_R1(ref regs.E, ref regs.C);
                        return;
                    }
                case 0x4C:
                    {
                        LD_R2_R1(ref regs.H, ref regs.C);
                        return;
                    }
                case 0x4D:
                    {
                        LD_R2_R1(ref regs.L, ref regs.C);
                        return;
                    }
                case 0x4E:
                    {
                        LD_R2_R1(regs.HL, ref regs.C);
                        return;
                    }
                case 0x4F:
                    {
                        LD_R2_R1(ref regs.A, ref regs.C);
                        return;
                    }
                case 0x50:
                    {
                        LD_R2_R1(ref regs.B, ref regs.D);
                        return;
                    }
                case 0x51:
                    {
                        LD_R2_R1(ref regs.C, ref regs.D);
                        return;
                    }
                case 0x52:
                    {
                        LD_R2_R1(ref regs.D, ref regs.D);
                        return;
                    }
                case 0x53:
                    {
                        LD_R2_R1(ref regs.E, ref regs.D);
                        return;
                    }
                case 0x54:
                    {
                        LD_R2_R1(ref regs.H, ref regs.D);
                        return;
                    }
                case 0x55:
                    {
                        LD_R2_R1(ref regs.L, ref regs.D);
                        return;
                    }
                case 0x56:
                    {
                        LD_R2_R1(regs.HL, ref regs.D);
                        return;
                    }
                case 0x57:
                    {
                        LD_R2_R1(ref regs.A, ref regs.D);
                        return;
                    }
                case 0x58:
                    {
                        LD_R2_R1(ref regs.B, ref regs.E);
                        return;
                    }
                case 0x59:
                    {
                        LD_R2_R1(ref regs.C, ref regs.E);
                        return;
                    }
                case 0x5A:
                    {
                        LD_R2_R1(ref regs.D, ref regs.E);
                        return;
                    }
                case 0x5B:
                    {
                        LD_R2_R1(ref regs.E, ref regs.E);
                        return;
                    }
                case 0x5C:
                    {
                        LD_R2_R1(ref regs.H, ref regs.E);
                        return;
                    }
                case 0x5D:
                    {
                        LD_R2_R1(ref regs.L, ref regs.E);
                        return;
                    }
                case 0x5E:
                    {
                        LD_R2_R1(regs.HL, ref regs.E);
                        return;
                    }
                case 0x5F:
                    {
                        LD_R2_R1(ref regs.A, ref regs.E);
                        return;
                    }
                case 0x60:
                    {
                        LD_R2_R1(ref regs.B, ref regs.H);
                        return;
                    }
                case 0x61:
                    {
                        LD_R2_R1(ref regs.C, ref regs.H);
                        return;
                    }
                case 0x62:
                    {
                        LD_R2_R1(ref regs.D, ref regs.H);
                        return;
                    }
                case 0x63:
                    {
                        LD_R2_R1(ref regs.E, ref regs.H);
                        return;
                    }
                case 0x64:
                    {
                        LD_R2_R1(ref regs.H, ref regs.H);
                        return;
                    }
                case 0x65:
                    {
                        LD_R2_R1(ref regs.L, ref regs.H);
                        return;
                    }
                case 0x66:
                    {
                        LD_R2_R1(regs.HL, ref regs.H);
                        return;
                    }
                case 0x67:
                    {
                        LD_R2_R1(ref regs.A, ref regs.H);
                        return;
                    }
                case 0x68:
                    {
                        LD_R2_R1(ref regs.B, ref regs.L);
                        return;
                    }
                case 0x69:
                    {
                        LD_R2_R1(ref regs.C, ref regs.L);
                        return;
                    }
                case 0x6A:
                    {
                        LD_R2_R1(ref regs.D, ref regs.L);
                        return;
                    }
                case 0x6B:
                    {
                        LD_R2_R1(ref regs.E, ref regs.L);
                        return;
                    }
                case 0x6C:
                    {
                        LD_R2_R1(ref regs.H, ref regs.L);
                        return;
                    }
                case 0x6D:
                    {
                        LD_R2_R1(ref regs.L, ref regs.L);
                        return;
                    }
                case 0x6E:
                    {
                        LD_R2_R1(regs.HL, ref regs.L);
                        return;
                    }
                case 0x6F:
                    {
                        LD_R2_R1(ref regs.A, ref regs.L);
                        return;
                    }
                case 0x70:
                    {
                        LD_R2_R1(ref regs.B, regs.HL);
                        return;
                    }
                case 0x71:
                    {
                        LD_R2_R1(ref regs.C, regs.HL);
                        return;
                    }
                case 0x72:
                    {
                        LD_R2_R1(ref regs.D, regs.HL);
                        return;
                    }
                case 0x73:
                    {
                        LD_R2_R1(ref regs.E, regs.HL);
                        return;
                    }
                case 0x74:
                    {
                        LD_R2_R1(ref regs.H, regs.HL);
                        return;
                    }
                case 0x75:
                    {
                        LD_R2_R1(ref regs.L, regs.HL);
                        return;
                    }
                case 0x76:
                    {
                        HALT();
                        return;
                    }
                case 0x77:
                    {
                        LD_R2_R1(ref regs.A, regs.HL);
                        return;
                    }
                case 0x78:
                    {
                        LD_R2_R1(ref regs.B, ref regs.A);
                        return;
                    }
                case 0x79:
                    {
                        LD_R2_R1(ref regs.C, ref regs.A);
                        return;
                    }
                case 0x7A:
                    {
                        LD_R2_R1(ref regs.D, ref regs.A);
                        return;
                    }
                case 0x7B:
                    {
                        LD_R2_R1(ref regs.E, ref regs.A);
                        return;
                    }
                case 0x7C:
                    {
                        LD_R2_R1(ref regs.H, ref regs.A);
                        return;
                    }
                case 0x7D:
                    {
                        LD_R2_R1(ref regs.L, ref regs.A);
                        return;
                    }
                case 0x7E:
                    {
                        LD_R2_R1(regs.HL, ref regs.A);
                        return;
                    }
                case 0x7F:
                    {
                        LD_R2_R1(ref regs.A, ref regs.A);
                        return;
                    }
                case 0x80:
                    {
                        ADD_R(ref regs.B);
                        return;
                    }
                case 0x81:
                    {
                        ADD_R(ref regs.C);
                        return;
                    }
                case 0x82:
                    {
                        ADD_R(ref regs.D);
                        return;
                    }
                case 0x83:
                    {
                        ADD_R(ref regs.E);
                        return;
                    }
                case 0x84:
                    {
                        ADD_R(ref regs.H);
                        return;
                    }
                case 0x85:
                    {
                        ADD_R(ref regs.L);
                        return;
                    }
                case 0x86:
                    {
                        ADD_M(regs.HL);
                        return;
                    }
                case 0x87:
                    {
                        ADD_R(ref regs.A);
                        return;
                    }
                case 0x88:
                    {
                        ADC_R(ref regs.B);
                        return;
                    }
                case 0x89:
                    {
                        ADC_R(ref regs.C);
                        return;
                    }
                case 0x8A:
                    {
                        ADC_R(ref regs.D);
                        return;
                    }
                case 0x8B:
                    {
                        ADC_R(ref regs.E);
                        return;
                    }
                case 0x8C:
                    {
                        ADC_R(ref regs.H);
                        return;
                    }
                case 0x8D:
                    {
                        ADC_R(ref regs.L);
                        return;
                    }
                case 0x8E:
                    {
                        ADC_M(regs.HL);
                        return;
                    }
                case 0x8F:
                    {
                        ADC_R(ref regs.A);
                        return;
                    }
                case 0x90:
                    {
                        SUB_R(ref regs.B);
                        return;
                    }
                case 0x91:
                    {
                        SUB_R(ref regs.C);
                        return;
                    }
                case 0x92:
                    {
                        SUB_R(ref regs.D);
                        return;
                    }
                case 0x93:
                    {
                        SUB_R(ref regs.E);
                        return;
                    }
                case 0x94:
                    {
                        SUB_R(ref regs.H);
                        return;
                    }
                case 0x95:
                    {
                        SUB_R(ref regs.L);
                        return;
                    }
                case 0x96:
                    {
                        SUB_M(regs.HL);
                        return;
                    }
                case 0x97:
                    {
                        SUB_R(ref regs.A);
                        return;
                    }
                case 0x98:
                    {
                        SBC_R(ref regs.B);
                        return;
                    }
                case 0x99:
                    {
                        SBC_R(ref regs.C);
                        return;
                    }
                case 0x9A:
                    {
                        SBC_R(ref regs.D);
                        return;
                    }
                case 0x9B:
                    {
                        SBC_R(ref regs.E);
                        return;
                    }
                case 0x9C:
                    {
                        SBC_R(ref regs.H);
                        return;
                    }
                case 0x9D:
                    {
                        SBC_R(ref regs.L);
                        return;
                    }
                case 0x9E:
                    {
                        SBC_M(regs.HL);
                        return;
                    }
                case 0x9F:
                    {
                        SBC_R(ref regs.A);
                        return;
                    }
                case 0xA0:
                    {
                        AND_R(ref regs.B);
                        return;
                    }
                case 0xA1:
                    {
                        AND_R(ref regs.C);
                        return;
                    }
                case 0xA2:
                    {
                        AND_R(ref regs.D);
                        return;
                    }
                case 0xA3:
                    {
                        AND_R(ref regs.E);
                        return;
                    }
                case 0xA4:
                    {
                        AND_R(ref regs.H);
                        return;
                    }
                case 0xA5:
                    {
                        AND_R(ref regs.L);
                        return;
                    }
                case 0xA6:
                    {
                        AND_M(regs.HL);
                        return;
                    }
                case 0xA7:
                    {
                        AND_R(ref regs.A);
                        return;
                    }
                case 0xA8:
                    {
                        XOR_R(ref regs.B);
                        return;
                    }
                case 0xA9:
                    {
                        XOR_R(ref regs.C);
                        return;
                    }
                case 0xAA:
                    {
                        XOR_R(ref regs.D);
                        return;
                    }
                case 0xAB:
                    {
                        XOR_R(ref regs.E);
                        return;
                    }
                case 0xAC:
                    {
                        XOR_R(ref regs.H);
                        return;
                    }
                case 0xAD:
                    {
                        XOR_R(ref regs.L);
                        return;
                    }
                case 0xAE:
                    {
                        XOR_M(regs.HL);
                        return;
                    }
                case 0xAF:
                    {
                        XOR_R(ref regs.A);
                        return;
                    }
                case 0xB0:
                    {
                        OR_R(ref regs.B);
                        return;
                    }
                case 0xB1:
                    {
                        OR_R(ref regs.C);
                        return;
                    }
                case 0xB2:
                    {
                        OR_R(ref regs.D);
                        return;
                    }
                case 0xB3:
                    {
                        OR_R(ref regs.E);
                        return;
                    }
                case 0xB4:
                    {
                        OR_R(ref regs.H);
                        return;
                    }
                case 0xB5:
                    {
                        OR_R(ref regs.L);
                        return;
                    }
                case 0xB6:
                    {
                        OR_M(regs.HL);
                        return;
                    }
                case 0xB7:
                    {
                        OR_R(ref regs.A);
                        return;
                    }
                case 0xB8:
                    {
                        CP_R(ref regs.B);
                        return;
                    }
                case 0xB9:
                    {
                        CP_R(ref regs.C);
                        return;
                    }
                case 0xBA:
                    {
                        CP_R(ref regs.D);
                        return;
                    }
                case 0xBB:
                    {
                        CP_R(ref regs.E);
                        return;
                    }
                case 0xBC:
                    {
                        CP_R(ref regs.H);
                        return;
                    }
                case 0xBD:
                    {
                        CP_R(ref regs.L);
                        return;
                    }
                case 0xBE:
                    {
                        CP_M(regs.HL);
                        return;
                    }
                case 0xBF:
                    {
                        CP_R(ref regs.A);
                        return;
                    }
                case 0xC0:
                    {
                        RET_NZ();
                        return;
                    }
                case 0xC1:
                    {
                        POP_RR(ref regs.B, ref regs.C);
                        return;
                    }
                case 0xC2:
                    {
                        JP_NZ_NN();
                        return;
                    }
                case 0xC3:
                    {
                        JP_NN();
                        return;
                    }
                case 0xC4:
                    {
                        CALL_NZ_NN();
                        return;
                    }
                case 0xC5:
                    {
                        PUSH_RR(ref regs.B, ref regs.C);
                        return;
                    }
                case 0xC6:
                    {
                        ADD_M2((ushort)(pc + 1));
                        return;
                    }
                case 0xC7:
                    {
                        RST_N(0x0);
                        return;
                    }
                case 0xC8:
                    {
                        RET_Z();
                        return;
                    }
                case 0xC9:
                    {
                        RET();
                        return;
                    }
                case 0xCA:
                    {
                        JP_Z_NN();
                        return;
                    }
                case 0xCB:
                    {
                        Decode_Opcode_CB(_memory.read_byte((ushort)(pc + 1)));
                        return;
                    }
                case 0xCC:
                    {
                        CALL_Z_NN();
                        return;
                    }
                case 0xCD:
                    {
                        CALL_NN();
                        return;
                    }
                case 0xCE:
                    {
                        ADC_M2((ushort)(pc + 1));
                        return;
                    }
                case 0xCF:
                    {
                        RST_N(0x08);
                        return;
                    }
                case 0xD0:
                    {
                        RET_NC();
                        return;
                    }
                case 0xD1:
                    {
                        POP_RR(ref regs.D, ref regs.E);
                        return;
                    }
                case 0xD2:
                    {
                        JP_NC_NN();
                        return;
                    }
                case 0xD4:
                    {
                        CALL_NC_NN();
                        return;
                    }
                case 0xD5:
                    {
                        PUSH_RR(ref regs.D, ref regs.E);
                        return;
                    }
                case 0xD6:
                    {
                        SUB_M2((ushort)(pc + 1));
                        return;
                    }
                case 0xD7:
                    {
                        RST_N(0x10);
                        return;
                    }
                case 0xD8:
                    {
                        RET_C();
                        return;
                    }
                case 0xD9:
                    {
                        RETI();
                        return;
                    }
                case 0xDA:
                    {
                        JP_C_NN();
                        return;
                    }
                case 0xDC:
                    {
                        CALL_C_NN();
                        return;
                    }
                case 0xDE:
                    {
                        SBC_M2((ushort)(pc + 1));
                        return;
                    }
                case 0xDF:
                    {
                        RST_N(0x18);
                        return;
                    }
                case 0xE0:
                    {
                        LD_R_MR(ref regs.A);
                        return;
                    }
                case 0xE1:
                    {
                        POP_RR(ref regs.H, ref regs.L);
                        return;
                    }
                case 0xE2:
                    {
                        LD_R_MR(ref regs.A, ref regs.C);
                        return;
                    }
                case 0xE5:
                    {
                        PUSH_RR(ref regs.H, ref regs.L);
                        return;
                    }
                case 0xE6:
                    {
                        AND_M2((ushort)(pc + 1));
                        return;
                    }
                case 0xE7:
                    {
                        RST_N(0x20);
                        return;
                    }
                case 0xE8:
                    {
                        ADD_SP_N();
                        return;
                    }
                case 0xE9:
                    {
                        JP_HL();
                        return;
                    }
                case 0xEA:
                    {
                        LD_R_NN(ref regs.A);
                        return;
                    }
                case 0xEE:
                    {
                        XOR_M2((ushort)(pc + 1));
                        return;
                    }
                case 0xEF:
                    {
                        RST_N(0x28);
                        return;
                    }
                case 0xF0:
                    {
                        LD_MR_R(ref regs.A);
                        return;
                    }
                case 0xF1:
                    {
                        POP_RR(ref regs.A, ref regs.F);
                        return;
                    }
                case 0xF2:
                    {
                        LD_MR_R(ref regs.C, ref regs.A);
                        return;
                    }
                case 0xF3:
                    {
                        DI();
                        return;
                    }
                case 0xF5:
                    {
                        PUSH_RR(ref regs.A, ref regs.F);
                        return;
                    }
                case 0xF6:
                    {
                        OR_M2((ushort)(pc + 1));
                        return;
                    }
                case 0xF7:
                    {
                        RST_N(0x30);
                        return;
                    }
                case 0xF8:
                    {
                        LD_RR_RR(ref sp, ref regs.HL);
                        return;
                    }
                case 0xF9:
                    {
                        LD_RR_RR(ref regs.HL, ref sp);
                        return;
                    }
                case 0xFA:
                    {
                        LD_NN_R(ref regs.A);
                        return;
                    }
                case 0xFB:
                    {
                        EI();
                        return;
                    }
                case 0xFE:
                    {
                        CP_M2((ushort)(pc + 1));
                        return;
                    }
                case 0xFF:
                    {
                        RST_N(0x38);
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

            switch (opcode2)
            {
                case 0x00:
                    {
                        RLC_R(ref regs.B);
                        return;
                    }
                case 0x01:
                    {
                        RLC_R(ref regs.C);
                        return;
                    }
                case 0x02:
                    {
                        RLC_R(ref regs.D);
                        return;
                    }
                case 0x03:
                    {
                        RLC_R(ref regs.E);
                        return;
                    }
                case 0x04:
                    {
                        RLC_R(ref regs.H);
                        return;
                    }
                case 0x05:
                    {
                        RLC_R(ref regs.L);
                        return;
                    }
                case 0x06:
                    {
                        RLC_M(regs.HL);
                        return;
                    }
                case 0x07:
                    {
                        RLC_R(ref regs.A);
                        return;
                    }
                case 0x08:
                    {
                        RRC_R(ref regs.B);
                        return;
                    }
                case 0x09:
                    {
                        RRC_R(ref regs.C);
                        return;
                    }
                case 0x0A:
                    {
                        RRC_R(ref regs.D);
                        return;
                    }
                case 0x0B:
                    {
                        RRC_R(ref regs.E);
                        return;
                    }
                case 0x0C:
                    {
                        RRC_R(ref regs.H);
                        return;
                    }
                case 0x0D:
                    {
                        RRC_R(ref regs.L);
                        return;
                    }
                case 0x0E:
                    {
                        RRC_M(regs.HL);
                        return;
                    }
                case 0x0F:
                    {
                        RRC_R(ref regs.A);
                        return;
                    }
                case 0x10:
                    {
                        RL_R(ref regs.B);
                        return;
                    }
                case 0x11:
                    {
                        RL_R(ref regs.C);
                        return;
                    }
                case 0x12:
                    {
                        RL_R(ref regs.D);
                        return;
                    }
                case 0x13:
                    {
                        RL_R(ref regs.E);
                        return;
                    }
                case 0x14:
                    {
                        RL_R(ref regs.H);
                        return;
                    }
                case 0x15:
                    {
                        RL_R(ref regs.L);
                        return;
                    }
                case 0x16:
                    {
                        RL_M(regs.HL);
                        return;
                    }
                case 0x17:
                    {
                        RL_R(ref regs.A);
                        return;
                    }
                case 0x18:
                    {
                        RR_R(ref regs.B);
                        return;
                    }
                case 0x19:
                    {
                        RR_R(ref regs.C);
                        return;
                    }
                case 0x1A:
                    {
                        RR_R(ref regs.D);
                        return;
                    }
                case 0x1B:
                    {
                        RR_R(ref regs.E);
                        return;
                    }
                case 0x1C:
                    {
                        RR_R(ref regs.H);
                        return;
                    }
                case 0x1D:
                    {
                        RR_R(ref regs.L);
                        return;
                    }
                case 0x1E:
                    {
                        RR_M(regs.HL);
                        return;
                    }
                case 0x1F:
                    {
                        RR_R(ref regs.A);
                        return;
                    }
                case 0x20:
                    {
                        SLA_R(ref regs.B);
                        return;
                    }
                case 0x21:
                    {
                        SLA_R(ref regs.C);
                        return;
                    }
                case 0x22:
                    {
                        SLA_R(ref regs.D);
                        return;
                    }
                case 0x23:
                    {
                        SLA_R(ref regs.E);
                        return;
                    }
                case 0x24:
                    {
                        SLA_R(ref regs.H);
                        return;
                    }
                case 0x25:
                    {
                        SLA_R(ref regs.L);
                        return;
                    }
                case 0x26:
                    {
                        SLA_M(regs.HL);
                        return;
                    }
                case 0x27:
                    {
                        SLA_R(ref regs.A);
                        return;
                    }
                case 0x28:
                    {
                        SRA_R(ref regs.B);
                        return;
                    }
                case 0x29:
                    {
                        SRA_R(ref regs.C);
                        return;
                    }
                case 0x2A:
                    {
                        SRA_R(ref regs.D);
                        return;
                    }
                case 0x2B:
                    {
                        SRA_R(ref regs.E);
                        return;
                    }
                case 0x2C:
                    {
                        SRA_R(ref regs.H);
                        return;
                    }
                case 0x2D:
                    {
                        SRA_R(ref regs.L);
                        return;
                    }
                case 0x2E:
                    {
                        SRA_M(regs.HL);
                        return;
                    }
                case 0x2F:
                    {
                        SRA_R(ref regs.A);
                        return;
                    }
                case 0x30:
                    {
                        SWAP_R(ref regs.B);
                        return;
                    }
                case 0x31:
                    {
                        SWAP_R(ref regs.C);
                        return;
                    }
                case 0x32:
                    {
                        SWAP_R(ref regs.D);
                        return;
                    }
                case 0x33:
                    {
                        SWAP_R(ref regs.E);
                        return;
                    }
                case 0x34:
                    {
                        SWAP_R(ref regs.H);
                        return;
                    }
                case 0x35:
                    {
                        SWAP_R(ref regs.L);
                        return;
                    }
                case 0x36:
                    {
                        SWAP_M(regs.HL);
                        return;
                    }
                case 0x37:
                    {
                        SWAP_R(ref regs.A);
                        return;
                    }
                case 0x38:
                    {
                        SRL_R(ref regs.B);
                        return;
                    }
                case 0x39:
                    {
                        SRL_R(ref regs.C);
                        return;
                    }
                case 0x3A:
                    {
                        SRL_R(ref regs.D);
                        return;
                    }
                case 0x3B:
                    {
                        SRL_R(ref regs.E);
                        return;
                    }
                case 0x3C:
                    {
                        SRL_R(ref regs.H);
                        return;
                    }
                case 0x3D:
                    {
                        SRL_R(ref regs.L);
                        return;
                    }
                case 0x3E:
                    {
                        SRL_M(regs.HL);
                        return;
                    }
                case 0x3F:
                    {
                        SRL_R(ref regs.A);
                        return;
                    }
                case 0x40:
                    {
                        BIT_B_R(0, ref regs.B);
                        return;
                    }
                case 0x41:
                    {
                        BIT_B_R(0, ref regs.C);
                        return;
                    }
                case 0x42:
                    {
                        BIT_B_R(0, ref regs.D);
                        return;
                    }
                case 0x43:
                    {
                        BIT_B_R(0, ref regs.E);
                        return;
                    }
                case 0x44:
                    {
                        BIT_B_R(0, ref regs.H);
                        return;
                    }
                case 0x45:
                    {
                        BIT_B_R(0, ref regs.L);
                        return;
                    }
                case 0x46:
                    {
                        BIT_B_M(0, regs.HL);
                        return;
                    }
                case 0x47:
                    {
                        BIT_B_R(0, ref regs.A);
                        return;
                    }
                case 0x48:
                    {
                        BIT_B_R(1, ref regs.B);
                        return;
                    }
                case 0x49:
                    {
                        BIT_B_R(1, ref regs.C);
                        return;
                    }
                case 0x4A:
                    {
                        BIT_B_R(1, ref regs.D);
                        return;
                    }
                case 0x4B:
                    {
                        BIT_B_R(1, ref regs.E);
                        return;
                    }
                case 0x4C:
                    {
                        BIT_B_R(1, ref regs.H);
                        return;
                    }
                case 0x4D:
                    {
                        BIT_B_R(1, ref regs.L);
                        return;
                    }
                case 0x4E:
                    {
                        BIT_B_M(1, regs.HL);
                        return;
                    }
                case 0x4F:
                    {
                        BIT_B_R(1, ref regs.A);
                        return;
                    }
                case 0x50:
                    {
                        BIT_B_R(2, ref regs.B);
                        return;
                    }
                case 0x51:
                    {
                        BIT_B_R(2, ref regs.C);
                        return;
                    }
                case 0x52:
                    {
                        BIT_B_R(2, ref regs.D);
                        return;
                    }
                case 0x53:
                    {
                        BIT_B_R(2, ref regs.E);
                        return;
                    }
                case 0x54:
                    {
                        BIT_B_R(2, ref regs.H);
                        return;
                    }
                case 0x55:
                    {
                        BIT_B_R(2, ref regs.L);
                        return;
                    }
                case 0x56:
                    {
                        BIT_B_M(2, regs.HL);
                        return;
                    }
                case 0x57:
                    {
                        BIT_B_R(2, ref regs.A);
                        return;
                    }
                case 0x58:
                    {
                        BIT_B_R(3, ref regs.B);
                        return;
                    }
                case 0x59:
                    {
                        BIT_B_R(3, ref regs.C);
                        return;
                    }
                case 0x5A:
                    {
                        BIT_B_R(3, ref regs.D);
                        return;
                    }
                case 0x5B:
                    {
                        BIT_B_R(3, ref regs.E);
                        return;
                    }
                case 0x5C:
                    {
                        BIT_B_R(3, ref regs.H);
                        return;
                    }
                case 0x5D:
                    {
                        BIT_B_R(3, ref regs.L);
                        return;
                    }
                case 0x5E:
                    {
                        BIT_B_M(3, regs.HL);
                        return;
                    }
                case 0x5F:
                    {
                        BIT_B_R(3, ref regs.A);
                        return;
                    }
                case 0x60:
                    {
                        BIT_B_R(4, ref regs.B);
                        return;
                    }
                case 0x61:
                    {
                        BIT_B_R(4, ref regs.C);
                        return;
                    }
                case 0x62:
                    {
                        BIT_B_R(4, ref regs.D);
                        return;
                    }
                case 0x63:
                    {
                        BIT_B_R(4, ref regs.E);
                        return;
                    }
                case 0x64:
                    {
                        BIT_B_R(4, ref regs.H);
                        return;
                    }
                case 0x65:
                    {
                        BIT_B_R(4, ref regs.L);
                        return;
                    }
                case 0x66:
                    {
                        BIT_B_M(4, regs.HL);
                        return;
                    }
                case 0x67:
                    {
                        BIT_B_R(4, ref regs.A);
                        return;
                    }
                case 0x68:
                    {
                        BIT_B_R(5, ref regs.B);
                        return;
                    }
                case 0x69:
                    {
                        BIT_B_R(5, ref regs.C);
                        return;
                    }
                case 0x6A:
                    {
                        BIT_B_R(5, ref regs.D);
                        return;
                    }
                case 0x6B:
                    {
                        BIT_B_R(5, ref regs.E);
                        return;
                    }
                case 0x6C:
                    {
                        BIT_B_R(5, ref regs.H);
                        return;
                    }
                case 0x6D:
                    {
                        BIT_B_R(5, ref regs.L);
                        return;
                    }
                case 0x6E:
                    {
                        BIT_B_M(5, regs.HL);
                        return;
                    }
                case 0x6F:
                    {
                        BIT_B_R(5, ref regs.A);
                        return;
                    }
                case 0x70:
                    {
                        BIT_B_R(6, ref regs.B);
                        return;
                    }
                case 0x71:
                    {
                        BIT_B_R(6, ref regs.C);
                        return;
                    }
                case 0x72:
                    {
                        BIT_B_R(6, ref regs.D);
                        return;
                    }
                case 0x73:
                    {
                        BIT_B_R(6, ref regs.E);
                        return;
                    }
                case 0x74:
                    {
                        BIT_B_R(6, ref regs.H);
                        return;
                    }
                case 0x75:
                    {
                        BIT_B_R(6, ref regs.L);
                        return;
                    }
                case 0x76:
                    {
                        BIT_B_M(6, regs.HL);
                        return;
                    }
                case 0x77:
                    {
                        BIT_B_R(6, ref regs.A);
                        return;
                    }
                case 0x78:
                    {
                        BIT_B_R(7, ref regs.B);
                        return;
                    }
                case 0x79:
                    {
                        BIT_B_R(7, ref regs.C);
                        return;
                    }
                case 0x7A:
                    {
                        BIT_B_R(7, ref regs.D);
                        return;
                    }
                case 0x7B:
                    {
                        BIT_B_R(7, ref regs.E);
                        return;
                    }
                case 0x7C:
                    {
                        BIT_B_R(7, ref regs.H);
                        return;
                    }
                case 0x7D:
                    {
                        BIT_B_R(7, ref regs.L);
                        return;
                    }
                case 0x7E:
                    {
                        BIT_B_M(7, regs.HL);
                        return;
                    }
                case 0x7F:
                    {
                        BIT_B_R(7, ref regs.A);
                        return;
                    }
                case 0x80:
                    {
                        RES_B_R(0, ref regs.B);
                        return;
                    }
                case 0x81:
                    {
                        RES_B_R(0, ref regs.C);
                        return;
                    }
                case 0x82:
                    {
                        RES_B_R(0, ref regs.D);
                        return;
                    }
                case 0x83:
                    {
                        RES_B_R(0, ref regs.E);
                        return;
                    }
                case 0x84:
                    {
                        RES_B_R(0, ref regs.H);
                        return;
                    }
                case 0x85:
                    {
                        RES_B_R(0, ref regs.L);
                        return;
                    }
                case 0x86:
                    {

                        RES_B_M(0, regs.HL);
                        return;
                    }
                case 0x87:
                    {
                        RES_B_R(0, ref regs.A);
                        return;
                    }
                case 0x88:
                    {
                        RES_B_R(1, ref regs.B);
                        return;
                    }
                case 0x89:
                    {
                        RES_B_R(1, ref regs.C);
                        return;
                    }
                case 0x8A:
                    {
                        RES_B_R(1, ref regs.D);
                        return;
                    }
                case 0x8B:
                    {
                        RES_B_R(1, ref regs.E);
                        return;
                    }
                case 0x8C:
                    {
                        RES_B_R(1, ref regs.H);
                        return;
                    }
                case 0x8D:
                    {
                        RES_B_R(1, ref regs.L);
                        return;
                    }
                case 0x8E:
                    {
                        RES_B_M(1, regs.HL);
                        return;
                    }
                case 0x8F:
                    {
                        RES_B_R(1, ref regs.A);
                        return;
                    }
                case 0x90:
                    {
                        RES_B_R(2, ref regs.B);
                        return;
                    }
                case 0x91:
                    {
                        RES_B_R(2, ref regs.C);
                        return;
                    }
                case 0x92:
                    {
                        RES_B_R(2, ref regs.D);
                        return;
                    }
                case 0x93:
                    {
                        RES_B_R(2, ref regs.E);
                        return;
                    }
                case 0x94:
                    {
                        RES_B_R(2, ref regs.H);
                        return;
                    }
                case 0x95:
                    {
                        RES_B_R(2, ref regs.L);
                        return;
                    }
                case 0x96:
                    {
                        RES_B_M(2, regs.HL);
                        return;
                    }
                case 0x97:
                    {
                        RES_B_R(2, ref regs.A);
                        return;
                    }
                case 0x98:
                    {
                        RES_B_R(3, ref regs.B);
                        return;
                    }
                case 0x99:
                    {
                        RES_B_R(3, ref regs.C);
                        return;
                    }
                case 0x9A:
                    {
                        RES_B_R(3, ref regs.D);
                        return;
                    }
                case 0x9B:
                    {
                        RES_B_R(3, ref regs.E);
                        return;
                    }
                case 0x9C:
                    {
                        RES_B_R(3, ref regs.H);
                        return;
                    }
                case 0x9D:
                    {
                        RES_B_R(3, ref regs.L);
                        return;
                    }
                case 0x9E:
                    {
                        RES_B_M(3, regs.HL);
                        return;
                    }
                case 0x9F:
                    {
                        RES_B_R(3, ref regs.A);
                        return;
                    }
                case 0xA0:
                    {
                        RES_B_R(4, ref regs.B);
                        return;
                    }
                case 0xA1:
                    {
                        RES_B_R(4, ref regs.C);
                        return;
                    }
                case 0xA2:
                    {
                        RES_B_R(4, ref regs.D);
                        return;
                    }
                case 0xA3:
                    {
                        RES_B_R(4, ref regs.E);
                        return;
                    }
                case 0xA4:
                    {
                        RES_B_R(4, ref regs.H);
                        return;
                    }
                case 0xA5:
                    {
                        RES_B_R(4, ref regs.L);
                        return;
                    }
                case 0xA6:
                    {
                        RES_B_M(4, regs.HL);
                        return;
                    }
                case 0xA7:
                    {
                        RES_B_R(4, ref regs.A);
                        return;
                    }
                case 0xA8:
                    {
                        RES_B_R(5, ref regs.B);
                        return;
                    }
                case 0xA9:
                    {
                        RES_B_R(5, ref regs.C);
                        return;
                    }
                case 0xAA:
                    {
                        RES_B_R(5, ref regs.D);
                        return;
                    }
                case 0xAB:
                    {
                        RES_B_R(5, ref regs.E);
                        return;
                    }
                case 0xAC:
                    {
                        RES_B_R(5, ref regs.H);
                        return;
                    }
                case 0xAD:
                    {
                        RES_B_R(5, ref regs.L);
                        return;
                    }
                case 0xAE:
                    {
                        RES_B_M(5, regs.HL);
                        return;
                    }
                case 0xAF:
                    {
                        RES_B_R(5, ref regs.A);
                        return;
                    }
                case 0xB0:
                    {
                        RES_B_R(6, ref regs.B);
                        return;
                    }
                case 0xB1:
                    {
                        RES_B_R(6, ref regs.C);
                        return;
                    }
                case 0xB2:
                    {
                        RES_B_R(6, ref regs.D);
                        return;
                    }
                case 0xB3:
                    {
                        RES_B_R(6, ref regs.E);
                        return;
                    }
                case 0xB4:
                    {
                        RES_B_R(6, ref regs.H);
                        return;
                    }
                case 0xB5:
                    {
                        RES_B_R(6, ref regs.L);
                        return;
                    }
                case 0xB6:
                    {
                        RES_B_M(6, regs.HL);
                        return;
                    }
                case 0xB7:
                    {
                        RES_B_R(6, ref regs.A);
                        return;
                    }
                case 0xB8:
                    {
                        RES_B_R(7, ref regs.B);
                        return;
                    }
                case 0xB9:
                    {
                        RES_B_R(7, ref regs.C);
                        return;
                    }
                case 0xBA:
                    {
                        RES_B_R(7, ref regs.D);
                        return;
                    }
                case 0xBB:
                    {
                        RES_B_R(7, ref regs.E);
                        return;
                    }
                case 0xBC:
                    {
                        RES_B_R(7, ref regs.H);
                        return;
                    }
                case 0xBD:
                    {
                        RES_B_R(7, ref regs.L);
                        return;
                    }
                case 0xBE:
                    {
                        RES_B_M(7, regs.HL);
                        return;
                    }
                case 0xBF:
                    {
                        RES_B_R(7, ref regs.A);
                        return;
                    }
                case 0xC0:
                    {
                        SET_B_R(0, ref regs.B);
                        return;
                    }
                case 0xC1:
                    {
                        SET_B_R(0, ref regs.C);
                        return;
                    }
                case 0xC2:
                    {
                        SET_B_R(0, ref regs.D);
                        return;
                    }
                case 0xC3:
                    {
                        SET_B_R(0, ref regs.E);
                        return;
                    }
                case 0xC4:
                    {
                        SET_B_R(0, ref regs.H);
                        return;
                    }
                case 0xC5:
                    {
                        SET_B_R(0, ref regs.L);
                        return;
                    }
                case 0xC6:
                    {
                        SET_B_M(0, regs.HL);
                        return;
                    }
                case 0xC7:
                    {
                        SET_B_R(0, ref regs.A);
                        return;
                    }
                case 0xC8:
                    {
                        SET_B_R(1, ref regs.B);
                        return;
                    }
                case 0xC9:
                    {
                        SET_B_R(1, ref regs.C);
                        return;
                    }
                case 0xCA:
                    {
                        SET_B_R(1, ref regs.D);
                        return;
                    }
                case 0xCB:
                    {
                        SET_B_R(1, ref regs.E);
                        return;
                    }
                case 0xCC:
                    {
                        SET_B_R(1, ref regs.H);
                        return;
                    }
                case 0xCD:
                    {
                        SET_B_R(1, ref regs.L);
                        return;
                    }
                case 0xCE:
                    {
                        SET_B_M(1, regs.HL);
                        return;
                    }
                case 0xCF:
                    {
                        SET_B_R(1, ref regs.A);
                        return;
                    }
                case 0xD0:
                    {
                        SET_B_R(2, ref regs.B);
                        return;
                    }
                case 0xD1:
                    {
                        SET_B_R(2, ref regs.C);
                        return;
                    }
                case 0xD2:
                    {
                        SET_B_R(2, ref regs.D);
                        return;
                    }
                case 0xD3:
                    {
                        SET_B_R(2, ref regs.E);
                        return;
                    }
                case 0xD4:
                    {
                        SET_B_R(2, ref regs.H);
                        return;
                    }
                case 0xD5:
                    {
                        SET_B_R(2, ref regs.L);
                        return;
                    }
                case 0xD6:
                    {
                        SET_B_M(2, regs.HL);
                        return;
                    }
                case 0xD7:
                    {
                        SET_B_R(2, ref regs.A);
                        return;
                    }
                case 0xD8:
                    {
                        SET_B_R(3, ref regs.B);
                        return;
                    }
                case 0xD9:
                    {
                        SET_B_R(3, ref regs.C);
                        return;
                    }
                case 0xDA:
                    {
                        SET_B_R(3, ref regs.D);
                        return;
                    }
                case 0xDB:
                    {
                        SET_B_R(3, ref regs.E);
                        return;
                    }
                case 0xDC:
                    {
                        SET_B_R(3, ref regs.H);
                        return;
                    }
                case 0xDD:
                    {
                        SET_B_R(3, ref regs.L);
                        return;
                    }
                case 0xDE:
                    {
                        SET_B_M(3, regs.HL);
                        return;
                    }
                case 0xDF:
                    {
                        SET_B_R(3, ref regs.A);
                        return;
                    }
                case 0xE0:
                    {
                        SET_B_R(4, ref regs.B);
                        return;
                    }
                case 0xE1:
                    {
                        SET_B_R(4, ref regs.C);
                        return;
                    }
                case 0xE2:
                    {
                        SET_B_R(4, ref regs.D);
                        return;
                    }
                case 0xE3:
                    {
                        SET_B_R(4, ref regs.E);
                        return;
                    }
                case 0xE4:
                    {
                        SET_B_R(4, ref regs.H);
                        return;
                    }
                case 0xE5:
                    {
                        SET_B_R(4, ref regs.L);
                        return;
                    }
                case 0xE6:
                    {
                        SET_B_M(4, regs.HL);
                        return;
                    }
                case 0xE7:
                    {
                        SET_B_R(4, ref regs.A);
                        return;
                    }
                case 0xE8:
                    {
                        SET_B_R(5, ref regs.B);
                        return;
                    }
                case 0xE9:
                    {
                        SET_B_R(5, ref regs.C);
                        return;
                    }
                case 0xEA:
                    {
                        SET_B_R(5, ref regs.D);
                        return;
                    }
                case 0xEB:
                    {
                        SET_B_R(5, ref regs.E);
                        return;
                    }
                case 0xEC:
                    {
                        SET_B_R(5, ref regs.H);
                        return;
                    }
                case 0xED:
                    {
                        SET_B_R(5, ref regs.L);
                        return;
                    }
                case 0xEE:
                    {
                        SET_B_M(5, regs.HL);
                        return;
                    }
                case 0xEF:
                    {
                        SET_B_R(5, ref regs.A);
                        return;
                    }
                case 0xF0:
                    {
                        SET_B_R(6, ref regs.B);
                        return;
                    }
                case 0xF1:
                    {
                        SET_B_R(6, ref regs.C);
                        return;
                    }
                case 0xF2:
                    {
                        SET_B_R(6, ref regs.D);
                        return;
                    }
                case 0xF3:
                    {
                        SET_B_R(6, ref regs.E);
                        return;
                    }
                case 0xF4:
                    {
                        SET_B_R(6, ref regs.H);
                        return;
                    }
                case 0xF5:
                    {
                        SET_B_R(6, ref regs.L);
                        return;
                    }
                case 0xF6:
                    {
                        SET_B_M(6, regs.HL);
                        return;
                    }
                case 0xF7:
                    {
                        SET_B_R(6, ref regs.A);
                        return;
                    }
                case 0xF8:
                    {
                        SET_B_R(7, ref regs.B);
                        return;
                    }
                case 0xF9:
                    {
                        SET_B_R(7, ref regs.C);
                        return;
                    }
                case 0xFA:
                    {
                        SET_B_R(7, ref regs.D);
                        return;
                    }
                case 0xFB:
                    {
                        SET_B_R(7, ref regs.E);
                        return;
                    }
                case 0xFC:
                    {
                        SET_B_R(7, ref regs.H);
                        return;
                    }
                case 0xFD:
                    {
                        SET_B_R(7, ref regs.L);
                        return;
                    }
                case 0xFE:
                    {
                        SET_B_M(7, regs.HL);
                        return;
                    }
                case 0xFF:
                    {
                        SET_B_R(7, ref regs.A);
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

            switch (opcode2)
            {
                case 0x00:
                    {
                        STOP();
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
            _memory.write_byte(sp, reg1);
            _memory.write_byte((ushort)(sp - 1), reg2);
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
            reg1 = _memory.read_byte((ushort)(sp + 2));
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
            auxiliary_carry_flag_set((byte)((regs.A + reg)));
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
            auxiliary_carry_flag_set((byte)((regs.A + temp)));
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
            auxiliary_carry_flag_set((byte)((regs.A + temp)));
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

            regs.A += (byte)(reg + (regs.F >> 4 & 0x1));

            auxiliary_carry_flag_set((byte)(temp1 + reg + (regs.F >> 4 & 0x1)));
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

            regs.A += (byte)(temp2 + (regs.F >> 4 & 0x1));

            auxiliary_carry_flag_set((byte)(temp1 + temp2 + (regs.F >> 4 & 0x1)));
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

            regs.A += (byte)(temp2 + (regs.F >> 4 & 0x1));

            auxiliary_carry_flag_set((byte)(temp1 + temp2 + (regs.F >> 4 & 0x1)));
            normal_carry_flag_set((ushort)(temp1 + temp2 + (regs.F >> 4 & 0x1)));
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

            auxiliary_carry_flag_set((byte)(regs.A + temp1));
            sub_carry_flag_set((ushort)(regs.A + temp1));

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

            auxiliary_carry_flag_set((byte)(regs.A + temp1));
            sub_carry_flag_set((ushort)(regs.A + temp1));

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

            auxiliary_carry_flag_set((byte)(regs.A + temp1));
            sub_carry_flag_set((ushort)(regs.A + temp1));

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

            auxiliary_carry_flag_set((byte)(regs.A + temp1));
            sub_carry_flag_set((ushort)(regs.A + temp1));

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

            auxiliary_carry_flag_set((byte)(regs.A + temp1));
            sub_carry_flag_set((ushort)(regs.A + temp1));

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

            auxiliary_carry_flag_set((byte)(regs.A + temp1));
            sub_carry_flag_set((ushort)(regs.A + temp1));

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

            auxiliary_carry_flag_set((byte)(regs.A + temp1));
            sub_carry_flag_set((ushort)(regs.A + temp1));
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

            auxiliary_carry_flag_set((byte)(regs.A + temp1));
            sub_carry_flag_set((ushort)(regs.A + temp1));
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
            byte temp1 = (byte)(-_memory.read_byte(addr));
            byte temp2 = (byte)(regs.A + temp1);

            auxiliary_carry_flag_set((byte)(regs.A + temp1));
            sub_carry_flag_set((ushort)(regs.A + temp1));
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
            auxiliary_carry_flag_set((ushort)((regs.HL + reg)));
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
            _memory.write_byte((ushort)(sp), (byte)(pc >> 8 & 0xFF));
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
                _memory.write_byte((ushort)(sp), (byte)(pc >> 8 & 0xFF));
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
                _memory.write_byte((ushort)(sp), (byte)(pc >> 8 & 0xFF));
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
                _memory.write_byte((ushort)(sp), (byte)(pc >> 8 & 0xFF));
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
                _memory.write_byte((ushort)(sp), (byte)(pc >> 8 & 0xFF));
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
                _memory.write_byte((ushort)(sp), (byte)(pc >> 8 & 0xFF));
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
            pc = (ushort)((_memory.read_byte((ushort)(sp + 2)) << 8) | (_memory.read_byte((ushort)(sp + 1))));
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
                pc = (ushort)((_memory.read_byte((ushort)(sp + 2)) << 8) | (_memory.read_byte((ushort)(sp + 1))));
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
                pc = (ushort)((_memory.read_byte((ushort)(sp + 2)) << 8) | (_memory.read_byte((ushort)(sp + 1))));
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
                pc = (ushort)((_memory.read_byte((ushort)(sp + 2)) << 8) | (_memory.read_byte((ushort)(sp + 1))));
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
                pc = (ushort)((_memory.read_byte((ushort)(sp + 2)) << 8) | (_memory.read_byte((ushort)(sp + 1))));
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
            pc = (ushort)((_memory.read_byte((ushort)(sp + 2)) << 8) | (_memory.read_byte((ushort)(sp + 1))));
            sp += 2;
            IME = true;
            next_opcode = _memory.read_byte(pc);
        }
        #endregion
    }
}
