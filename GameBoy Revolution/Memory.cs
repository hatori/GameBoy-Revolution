using System;
using System.IO;
using System.Windows.Forms;

namespace GameBoy_Revolution
{
    public class Memory
    {
        #region variables
        private byte[] memory;
        private byte[,] ram_banks;
        private byte[,] rom_banks;
        private FileStream read;

        private byte keyBits;
        private bool keyRow0;
        private bool keyRow1;

        private string rom_title;
        private byte current_rom_bank_0;
        private byte current_rom_bank_1;
        private byte mbc1_memory_mode;
        private bool ram_enabled;

        private string rom_name;
        private byte game_type;
        private ushort license_code;
        private byte device_mode;
        private byte cart_type;
        private byte rom_size;
        private byte ram_size;
        private byte destination_code;
        private byte old_license_code;
        private byte mask_rom_version;

        private byte P1;
        private byte SB;
        private byte SC;
        private byte DIV;
        private byte TIMA;
        private byte TMA;
        private byte TAC;
        private byte IF;
        private byte LCDC;
        private byte STAT;
        private byte SCY;
        private byte SCX;
        private byte LY;
        private byte LYC;
        private byte DMA;
        private byte BGP;
        private byte OBP0;
        private byte OBP1;
        private byte WY;
        private byte WX;
        private byte NR10;
        private byte NR11;
        private byte NR12;
        private byte NR13;
        private byte NR14;
        private byte NR21;
        private byte NR22;
        private byte NR23;
        private byte NR24;
        private byte NR30;
        private byte NR31;
        private byte NR32;
        private byte NR33;
        private byte NR34;
        private byte NR41;
        private byte NR42;
        private byte NR43;
        private byte NR44;
        private byte NR50;
        private byte NR51;
        private byte NR52;
        private byte IE;
        #endregion

        #region Retrieve P1
        public byte Retrieve_P1
        {
            get
            {
                return P1;
            }
            set
            {
                P1 = value;
            }
        }
        #endregion

        #region Retrieve SB
        public byte Retrieve_SB
        {
            get
            {
                return SB;
            }
            set
            {
                SB = value;
            }
        }
        #endregion

        #region Retrieve SC
        public byte Retrieve_SC
        {
            get
            {
                return SC;
            }
            set
            {
                SC = value;
            }
        }
        #endregion

        #region Retrieve DIV
        public byte Retrieve_DIV
        {
            get
            {
                return DIV;
            }
            set
            {
                DIV = value;
            }
        }
        #endregion

        #region Retrieve TIMA
        public byte Retrieve_TIMA
        {
            get
            {
                return TIMA;
            }
            set
            {
                TIMA = value;
            }
        }
        #endregion

        #region Retrieve TMA
        public byte Retrieve_TMA
        {
            get
            {
                return TMA;
            }
            set
            {
                TMA = value;
            }
        }
        #endregion

        #region Retrieve TAC
        public byte Retrieve_TAC
        {
            get
            {
                return TAC;
            }
            set
            {
                TAC = value;
            }
        }
        #endregion

        #region Retrieve IF
        public byte Retrieve_IF
        {
            get
            {
                return IF;
            }
            set
            {
                IF = value;
            }
        }
        #endregion

        #region Retrieve LCDC
        public byte Retrieve_LCDC
        {
            get
            {
                return LCDC;
            }
            set
            {
                LCDC = value;
            }
        }
        #endregion

        #region Retrieve STAT
        public byte Retrieve_STAT
        {
            get
            {
                return STAT;
            }
            set
            {
                STAT = value;
            }
        }
        #endregion

        #region Retrieve SCY
        public byte Retrieve_SCY
        {
            get
            {
                return SCY;
            }
            set
            {
                SCY = value;
            }
        }
        #endregion

        #region Retrieve SCX
        public byte Retrieve_SCX
        {
            get
            {
                return SCX;
            }
            set
            {
                SCX = value;
            }
        }
        #endregion

        #region Retrieve LY
        public byte Retrieve_LY
        {
            get
            {
                return LY;
            }
            set
            {
                LY = value;
            }
        }
        #endregion

        #region Retrieve LYC
        public byte Retrieve_LYC
        {
            get
            {
                return LYC;
            }
            set
            {
                LYC = value;
            }
        }
        #endregion

        #region Retrieve DMA
        public byte Retrieve_DMA
        {
            get
            {
                return DMA;
            }
            set
            {
                DMA = value;
            }
        }
        #endregion

        #region Retrieve BGP
        public byte Retrieve_BGP
        {
            get
            {
                return BGP;
            }
            set
            {
                BGP = value;
            }
        }
        #endregion

        #region Retrieve OBP0
        public byte Retrieve_OBP0
        {
            get
            {
                return OBP0;
            }
            set
            {
                OBP0 = value;
            }
        }
        #endregion

        #region Retrieve OBP1
        public byte Retrieve_OBP1
        {
            get
            {
                return OBP1;
            }
            set
            {
                OBP1 = value;
            }
        }
        #endregion

        #region Retrieve WY
        public byte Retrieve_WY
        {
            get
            {
                return WY;
            }
            set
            {
                WY = value;
            }
        }
        #endregion

        #region Retrieve WX
        public byte Retrieve_WX
        {
            get
            {
                return WX;
            }
            set
            {
                WX = value;
            }
        }
        #endregion

        #region Retrieve NR10
        public byte Retrieve_NR10
        {
            get
            {
                return NR10;
            }
            set
            {
                NR10 = value;
            }
        }
        #endregion

        #region Retrieve NR11
        public byte Retrieve_NR11
        {
            get
            {
                return NR11;
            }
            set
            {
                NR11 = value;
            }
        }
        #endregion

        #region Retrieve NR12
        public byte Retrieve_NR12
        {
            get
            {
                return NR12;
            }
            set
            {
                NR12 = value;
            }
        }
        #endregion

        #region Retrieve NR13
        public byte Retrieve_NR13
        {
            get
            {
                return NR13;
            }
            set
            {
                NR13 = value;
            }
        }
        #endregion

        #region Retrieve NR14
        public byte Retrieve_NR14
        {
            get
            {
                return NR14;
            }
            set
            {
                NR14 = value;
            }
        }
        #endregion

        #region Retrieve NR21
        public byte Retrieve_NR21
        {
            get
            {
                return NR21;
            }
            set
            {
                NR21 = value;
            }
        }
        #endregion

        #region Retrieve NR22
        public byte Retrieve_NR22
        {
            get
            {
                return NR22;
            }
            set
            {
                NR22 = value;
            }
        }
        #endregion

        #region Retrieve NR23
        public byte Retrieve_NR23
        {
            get
            {
                return NR23;
            }
            set
            {
                NR23 = value;
            }
        }
        #endregion

        #region Retrieve NR24
        public byte Retrieve_NR24
        {
            get
            {
                return NR24;
            }
            set
            {
                NR24 = value;
            }
        }
        #endregion

        #region Retrieve NR30
        public byte Retrieve_NR30
        {
            get
            {
                return NR30;
            }
            set
            {
                NR30 = value;
            }
        }
        #endregion

        #region Retrieve NR31
        public byte Retrieve_NR31
        {
            get
            {
                return NR31;
            }
            set
            {
                NR31 = value;
            }
        }
        #endregion

        #region Retrieve NR32
        public byte Retrieve_NR32
        {
            get
            {
                return NR32;
            }
            set
            {
                NR32 = value;
            }
        }
        #endregion

        #region Retrieve NR33
        public byte Retrieve_NR33
        {
            get
            {
                return NR33;
            }
            set
            {
                NR33 = value;
            }
        }
        #endregion

        #region Retrieve NR34
        public byte Retrieve_NR34
        {
            get
            {
                return NR34;
            }
            set
            {
                NR34 = value;
            }
        }
        #endregion

        #region Retrieve NR41
        public byte Retrieve_NR41
        {
            get
            {
                return NR41;
            }
            set
            {
                NR41 = value;
            }
        }
        #endregion

        #region Retrieve NR42
        public byte Retrieve_NR42
        {
            get
            {
                return NR42;
            }
            set
            {
                NR42 = value;
            }
        }
        #endregion

        #region Retrieve NR43
        public byte Retrieve_NR43
        {
            get
            {
                return NR43;
            }
            set
            {
                NR43 = value;
            }
        }
        #endregion

        #region Retrieve NR44
        public byte Retrieve_NR44
        {
            get
            {
                return NR44;
            }
            set
            {
                NR44 = value;
            }
        }
        #endregion

        #region Retrieve NR50
        public byte Retrieve_NR50
        {
            get
            {
                return NR50;
            }
            set
            {
                NR50 = value;
            }
        }
        #endregion

        #region Retrieve NR51
        public byte Retrieve_NR51
        {
            get
            {
                return NR51;
            }
            set
            {
                NR51 = value;
            }
        }
        #endregion

        #region Retrieve NR52
        public byte Retrieve_NR52
        {
            get
            {
                return NR52;
            }
            set
            {
                NR52 = value;
            }
        }
        #endregion

        #region Retrieve IE
        public byte Retrieve_IE
        {
            get
            {
                return IE;
            }
            set
            {
                IE = value;
            }
        }
        #endregion

        #region Retrieve KeyBits
        public byte Retrieve_KeyBits
        {
            get
            {
                return keyBits;
            }
            set
            {
                keyBits = value;
            }
        }
        #endregion

        #region constructor
        public Memory()
        {
            memory = new byte[0x10000];
            //ram_banks = new byte[0x10, 0x2000];
            //rom_banks = new byte[0x60, 0x4000];

            Initialize_Memory();
            startup();
        }
        #endregion

        #region startup
        private void startup()
        {
            rom_title = null;
            current_rom_bank_0 = 0;
            current_rom_bank_1 = 0;
            keyBits = 0xFF;
            keyRow0 = false;
            keyRow1 = false;
        }
        #endregion

        #region set rom data
        private void set_rom_data()
        {
            for (int i = 0; i < 0xF; i++)
            {
                rom_name += (char)(read_byte((ushort)(0x134 + i)));
            }

            game_type = read_byte(0x143);
            license_code = (ushort)(read_byte(0x144) | read_byte(0x145));
            device_mode = read_byte(0x146);
            cart_type = read_byte(0x147);
            rom_size = read_byte(0x148);
            ram_size = read_byte(0x149);
            destination_code = read_byte(0x14A);
            old_license_code = read_byte(0x14B);
            mask_rom_version = read_byte(0x14C);
        }
        #endregion

        #region setup rom settings
        private void setup_rom_settings()
        {
            ram_enabled = false;

            switch (cart_type)
            {
                case 0x0:
                    {
                        current_rom_bank_1 = 0x1;
                        break;
                    }
                case 0x1:
                    {
                        current_rom_bank_1 = 0x1;
                        mbc1_memory_mode = 0x0;
                        break;
                    }
            }
        }
        #endregion

        #region initialize memory
        public void Initialize_Memory()
        {
            for (int i = 0; i < memory.Length; i++)
            {
                memory[i] = 0;
            }

            /*for (int i = 0; i < 0x10; i++)
            {
                for (int k = 0; k < 0x2000; k++)
                {
                    ram_banks[i, k] = 0;
                }
            }

            for (int i = 0; i < 0x60; i++)
            {
                for (int k = 0; k < 0x4000; k++)
                {
                    rom_banks[i, k] = 0;
                }
            }*/
        }
        #endregion

        #region initialize rom
        public void Initialize_Rom(byte size)
        {
            rom_banks = new byte[size, 0x4000];
            for (int i = 0; i < size; i++)
            {
                for (int k = 0; k < 0x4000; k++)
                {
                    rom_banks[i, k] = 0;
                }
            }
        }
        #endregion

        #region initialize ram
        public void Initialize_Ram(byte size)
        {
            ram_banks = new byte[size, 0x2000];
            for (int i = 0; i < size; i++)
            {
                for (int k = 0; k < 0x2000; k++)
                {
                    ram_banks[i, k] = 0;
                }
            }
        }
        #endregion

        #region handle dma
        private void HandleDMA(byte source)
        {
            ushort source_addr = (ushort)(source << 8);
            for (int i = 0; i < 0xA0; i++)
            {
                write_byte((ushort)(0xFE00 + i), read_byte((ushort)(source_addr + i)));
            }
        }
        #endregion

        #region read byte
        public byte read_byte(ushort address, bool raw_access = false)
        {
            if (address >= 0x0 && address < 0x4000)
            {
                return rom_banks[current_rom_bank_0, address];
            }

            if (address >= 0x4000 && address < 0x8000)
            {
                return rom_banks[current_rom_bank_1, address ^ 0x4000];
            }

            if (address >= 0x8000 && address < memory.Length)
            {
                if(raw_access == true)
                    return memory[address];

                if (address == 0xFF00)
                {
                    if (keyRow0)
                    {
                        P1 = (byte)(0x10 | (keyBits & 0xF));
                    }
                    else if (keyRow1)
                    {
                        P1 = (byte)(0x20 | (keyBits >> 4 & 0xF));
                    }

                    return P1;
                }

                if (address == 0xFF01)
                    return SB;

                if (address == 0xFF02)
                    return SC;

                if (address == 0xFF04)
                    return DIV;

                if (address == 0xFF05)
                    return TIMA;

                if (address == 0xFF06)
                    return TMA;

                if (address == 0xFF07)
                    return TAC;

                if (address == 0xFF0F)
                    return IF;

                if (address == 0xFFFF)
                    return IE;

                if (address == 0xFF40)
                    return LCDC;

                if (address == 0xFF41)
                    return STAT;

                if (address == 0xFF42)
                    return SCY;

                if (address == 0xFF43)
                    return SCX;

                if (address == 0xFF44)
                    return LY;

                if (address == 0xFF45)
                    return LYC;

                if (address == 0xFF46)
                    return 0x0;

                if (address == 0xFF47)
                    return 0x0;

                if (address == 0xFF48)
                    return 0x0;

                if (address == 0xFF49)
                    return 0x0;

                if (address == 0xFF4A)
                    return WY;

                if (address == 0xFF4B)
                    return WX;

                if (address == 0xFF10)
                    return NR10;

                if (address == 0xFF11)
                    return NR11;

                if (address == 0xFF12)
                    return NR12;

                if (address == 0xFF13)
                    return 0x0;

                if (address == 0xFF14)
                    return 0x0;

                if (address == 0xFF16)
                    return NR21;

                if (address == 0xFF17)
                    return NR22;

                if (address == 0xFF18)
                    return 0x0;

                if (address == 0xFF19)
                    return 0x0;

                if (address == 0xFF1A)
                    return NR30;

                if (address == 0xFF1B)
                    return NR31;

                if (address == 0xFF1C)
                    return NR32;

                if (address == 0xFF1D)
                    return 0x0;

                if (address == 0xFF1E)
                    return 0x0;

                if (address == 0xFF20)
                    return NR41;

                if (address == 0xFF21)
                    return NR42;

                if (address == 0xFF22)
                    return NR43;

                if (address == 0xFF23)
                    return NR44;

                if (address == 0xFF24)
                    return NR50;

                if (address == 0xFF25)
                    return NR51;

                if (address == 0xFF26)
                    return NR52;

                return memory[address];
            }
            else
            {
                return 0x0;
            }
        }
        #endregion

        #region read ushort
        public ushort read_ushort(ushort address)
        {
            if (address >= 0x0 && address < 0x3FFF)
            {
                return (ushort)((rom_banks[current_rom_bank_0, address + 1] << 8) | rom_banks[current_rom_bank_0, address]);
            }

            if (address >= 0x4000 && address < 0x7FFF)
            {
                return (ushort)((rom_banks[current_rom_bank_1, (address + 1) ^ 0x4000] << 8) | rom_banks[current_rom_bank_1, address ^ 0x4000]);
            }

            if (address >= 0x8000 && address < (memory.Length - 1))
            {
                return (ushort)((memory[(ushort)(address + 1)] << 8) | memory[(ushort)(address)]);
            }
            else
            {
                return 0x0;
            }
        }
        #endregion

        #region write byte
        public void write_byte(ushort address, byte data)
        {
            if (address >= 0x8000 && address < memory.Length)
            {
                if (address == 0xFF00)
                {
                    if ((data & 0x20) == 0x20)
                    {
                        keyRow0 = true;
                        keyRow1 = false;
                    }
                    if ((data & 0x10) == 0x10)
                    {
                        keyRow1 = true;
                        keyRow0 = false;
                    }
                    if ((data & 0x30) == 0x30)
                    {
                        keyRow1 = false;
                        keyRow0 = false;
                    }
                }
                else if (address == 0xFF01)
                    SB = data;
                else if (address == 0xFF02)
                    SC = data;
                else if (address == 0xFF04)
                    DIV = 0x0;
                else if (address == 0xFF05)
                    TIMA = data;
                else if (address == 0xFF06)
                    TMA = data;
                else if (address == 0xFF07)
                    TAC = data;
                else if (address == 0xFF0F)
                    IF = data;
                else if (address == 0xFFFF)
                    IE = data;
                else if (address == 0xFF40)
                    LCDC = data;
                else if (address == 0xFF41)
                    STAT = data;
                else if (address == 0xFF42)
                    SCY = data;
                else if (address == 0xFF43)
                    SCX = data;
                else if (address == 0xFF44)
                    LY = 0;
                else if (address == 0xFF45)
                    LYC = data;
                else if (address == 0xFF46)
                {
                    DMA = data;
                    HandleDMA(DMA);
                }
                else if (address == 0xFF47)
                    BGP = data;
                else if (address == 0xFF48)
                    OBP0 = data;
                else if (address == 0xFF49)
                    OBP1 = data;
                else if (address == 0xFF4A)
                    WY = data;
                else if (address == 0xFF4B)
                    WX = data;
                else if (address == 0xFF10)
                    NR10 = data;
                else if (address == 0xFF11)
                    NR11 = data;
                else if (address == 0xFF12)
                    NR12 = data;
                else if (address == 0xFF13)
                    NR13 = data;
                else if (address == 0xFF14)
                    NR14 = data;
                else if (address == 0xFF16)
                    NR21 = data;
                else if (address == 0xFF17)
                    NR22 = data;
                else if (address == 0xFF18)
                    NR23 = data;
                else if (address == 0xFF19)
                    NR24 = data;
                else if (address == 0xFF1A)
                    NR30 = data;
                else if (address == 0xFF1B)
                    NR31 = data;
                else if (address == 0xFF1C)
                    NR32 = data;
                else if (address == 0xFF1D)
                    NR33 = data;
                else if (address == 0xFF1E)
                    NR34 = data;
                else if (address == 0xFF20)
                    NR41 = data;
                else if (address == 0xFF21)
                    NR42 = data;
                else if (address == 0xFF22)
                    NR43 = data;
                else if (address == 0xFF23)
                    NR44 = data;
                else if (address == 0xFF24)
                    NR50 = data;
                else if (address == 0xFF25)
                    NR51 = data;
                else if (address == 0xFF26)
                    NR52 = data;
                else if (address >= 0xA000 && address < 0xC000)
                {
                    if (ram_enabled)
                        memory[address] = data;
                }
                else if (address >= 0xC000 && address < 0xE000)
                    memory[address] = data;
                else if (address >= 0xE000 && address < 0xFE00)
                {
                    memory[address] = data;
                    memory[address - 0x2000] = data;
                }
                else if (address >= 0xFEA0 && address < 0xFF00)
                {
                }
                else if (address >= 0xFF4C && address < 0xFF80)
                {
                }
                else
                    memory[address] = data;
            }
            else
            {
                if(address >= 0x6000 && address <= 0x7FFF)
                {
                    mbc1_memory_mode = (byte)(data & 0x1);
                }
                else if (address >= 0x2000 && address <= 0x3FFF)
                {
                    if ((data & 0x1F) > 0)
                        current_rom_bank_1 = (byte)(data & 0x1F);
                }
            }
        }
        #endregion

        #region write ushort
        public void write_ushort(ushort address, ushort data)
        {
            if (address >= 0x8000 && address < (memory.Length - 1))
            {
                memory[(ushort)(address)] = (byte)(data & 0xFF);
                memory[(ushort)(address + 1)] = (byte)((data >> 8) & 0xFF);
            }
        }
        #endregion

        #region retrieve rom title
        public string Rom_Title
        {
            get
            {
                return rom_title;
            }
        }
        #endregion

        #region open rom
        public void open_rom()
        {
            try
            {
                OpenFileDialog open = new OpenFileDialog();
                open.AddExtension = true;
                open.AutoUpgradeEnabled = true;
                open.CheckFileExists = true;
                open.CheckPathExists = true;
                open.DefaultExt = ".gb";
                open.Multiselect = false;
                open.RestoreDirectory = true;
                open.ShowReadOnly = false;
                open.Title = "Open Gameboy rom...";
                open.Filter = "Gameboy Roms (*.gb) | *.gb";
                DialogResult result = open.ShowDialog();
                if (result == DialogResult.OK)
                {
                    rom_title = open.SafeFileName.Substring(0, open.SafeFileName.Length - 3);
                    read = new FileStream(open.FileName, FileMode.Open, FileAccess.Read);

                    byte[] content = File.ReadAllBytes(open.FileName);
                    byte pages = (byte)(content.Length / 0x4000);
                    Initialize_Rom(pages);

                    for (int i = 0x0; i < pages; i++)
                    {
                        for (int k = 0; k < 0x4000; k++)
                        {
                            rom_banks[i, k] = content[(i * 0x4000) + k];
                        }
                    }

                    set_rom_data();
                    setup_rom_settings();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion
    }
}
