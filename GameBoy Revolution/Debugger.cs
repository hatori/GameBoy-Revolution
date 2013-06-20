using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace GameBoy_Revolution
{
    public partial class Debugger : Form
    {
        #region variables
        CPU _cpu;
        #endregion

        #region constructor
        public Debugger(CPU cpu)
        {
            InitializeComponent();

            _cpu = cpu;
            InitDebugger();
            UpdateDebugger();
        }
        #endregion

        #region Update Debugger
        public void UpdateDebugger()
        {
            txtPrevOpcode.Text = txtCurrOpcode.Text;
            txtCurrOpcode.Text = Decode_Opcode(_cpu.retrieve_next_opcode);
            txtIE.Text = _cpu.retrieve_memory.Retrieve_IE.ToString("X2");
            txtIF.Text = _cpu.retrieve_memory.Retrieve_IF.ToString("X2");
            chkZ.Checked = _cpu.check_zero_flag();
            chkN.Checked = _cpu.check_negative_flag();
            chkH.Checked = _cpu.check_auxiliary_flag();
            chkC.Checked = _cpu.check_carry_flag();
            chkIMEEnabled.Checked = _cpu.retrieve_IME;
            txtCurrCycles.Text = _cpu.retrieve_Cycles.ToString();
            txtAF.Text = _cpu.retrieve_cpu_status.AF.ToString("X4");
            txtBC.Text = _cpu.retrieve_cpu_status.BC.ToString("X4");
            txtDE.Text = _cpu.retrieve_cpu_status.DE.ToString("X4");
            txtHL.Text = _cpu.retrieve_cpu_status.HL.ToString("X4");
            txtPC.Text = _cpu.retrieve_pc.ToString("X4");
            txtSP.Text = _cpu.retrieve_sp.ToString("X4");
            UpdateMemory();
        }
        #endregion

        #region update Tile Memory
        public void updateTileMem()
        {
            string tempStore = "";

            for (ushort i = 0x8000; i < 0x97FF;)
            {
                for (int j = 0x0; j < 0x10; j++)
                {
                    if (j == 0x0)
                    {
                        tempStore += i.ToString("X4") + " : " + _cpu.retrieve_memory.read_byte(i).ToString("X2") + "  ";
                    }
                    else
                    {
                        tempStore += _cpu.retrieve_memory.read_byte(i).ToString("X2") + "  ";
                    }
                    i++;
                }
                tempStore = tempStore + "\r\n";
            }
            txtMemory.Text = tempStore;
        }
        #endregion

        #region update Video Memory
        public void updateVidMem()
        {
            string tempStore = "";

            for (ushort i = 0x9800; i < 0x9FFF; )
            {
                for (int j = 0x0; j < 0x10; j++)
                {
                    if (j == 0x0)
                    {
                        tempStore += i.ToString("X4") + " : " + _cpu.retrieve_memory.read_byte(i).ToString("X2") + "  ";
                    }
                    else
                    {
                        tempStore += _cpu.retrieve_memory.read_byte(i).ToString("X2") + "  ";
                    }
                    i++;
                }
                tempStore = tempStore + "\r\n";
            }
            txtMemory.Text = tempStore;
        }
        #endregion

        #region update Ram Bank Memory
        public void updateRamBankMem()
        {
            string tempStore = "";

            for (ushort i = 0xA000; i < 0xBFFF; )
            {
                for (int j = 0x0; j < 0x10; j++)
                {
                    if (j == 0x0)
                    {
                        tempStore += i.ToString("X4") + " : " + _cpu.retrieve_memory.read_byte(i).ToString("X2") + "  ";
                    }
                    else
                    {
                        tempStore += _cpu.retrieve_memory.read_byte(i).ToString("X2") + "  ";
                    }
                    i++;
                }
                tempStore = tempStore + "\r\n";
            }
            txtMemory.Text = tempStore;
        }
        #endregion

        #region update Internal Ram Memory
        public void updateInternalRamMem()
        {
            string tempStore = "";

            for (ushort i = 0xC000; i < 0xDFFF; )
            {
                for (int j = 0x0; j < 0x10; j++)
                {
                    if (j == 0x0)
                    {
                        tempStore += i.ToString("X4") + " : " + _cpu.retrieve_memory.read_byte(i).ToString("X2") + "  ";
                    }
                    else
                    {
                        tempStore += _cpu.retrieve_memory.read_byte(i).ToString("X2") + "  ";
                    }
                    i++;
                }
                tempStore = tempStore + "\r\n";
            }
            txtMemory.Text = tempStore;
        }
        #endregion

        #region update System Memory
        public void updateSystemMem()
        {
            string tempStore = "";

            for (uint i = 0xFE00; i < 0x10000; )
            {
                for (int j = 0x0; j < 0x10; j++)
                {
                    if (j == 0x0)
                    {
                        tempStore += i.ToString("X4") + " : " + _cpu.retrieve_memory.read_byte((ushort)i).ToString("X2") + "  ";
                    }
                    else
                    {
                        tempStore += _cpu.retrieve_memory.read_byte((ushort)i).ToString("X2") + "  ";
                    }
                    i++;
                }
                tempStore = tempStore + "\r\n";
            }
            txtMemory.Text = tempStore;
        }
        #endregion

        #region Init Debugger
        /// <summary>
        /// This initializes the debugger display.
        /// </summary>
        public void InitDebugger()
        {
            InitMemoryTypes();
            txtCurrOpcode.Text = "0x00";
            txtIE.Text = "0x00";
            txtIF.Text = "0x00";
            chkZ.Checked = _cpu.check_zero_flag();
            chkN.Checked = _cpu.check_negative_flag();
            chkH.Checked = _cpu.check_auxiliary_flag();
            chkC.Checked = _cpu.check_carry_flag();
            chkIMEEnabled.Checked = _cpu.retrieve_IME;
            txtAF.Text = "0x0000";
            txtBC.Text = "0x0000";
            txtCurrCycles.Text = "0";
            txtDE.Text = "0x0000";
            txtHL.Text = "0x0000";
            txtPC.Text = "0x0000";
            txtPrevOpcode.Text = "0x00";
            txtSP.Text = "0x0000";
        }
        #endregion

        #region Init Memory Types
        /// <summary>
        /// This initializes the memory types dropdown.
        /// </summary>
        public void InitMemoryTypes()
        {
            string[] types = new string[] { "", "Tile", "Video","Ram Bank", "Internal Ram", "System" };
            cmbMemoryType.Items.AddRange(types);
        }
        #endregion

        #region form close
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        #region run
        private void btnRun_Click(object sender, EventArgs e)
        {
            if (txtBreakpoint.Text.Length > 0)
            {
                _cpu.breakpoint(Convert.ToUInt16(txtBreakpoint.Text, 16));
                UpdateDebugger();
            }
            else
            {
                _cpu.execute();
            }
        }
        #endregion

        #region step
        private void btnStep_Click(object sender, EventArgs e)
        {
            _cpu.step();
            UpdateDebugger();
        }
        #endregion

        #region decode opcode
        private string Decode_Opcode(byte opcode)
        {
            switch (opcode)
            {
                case 0x00:
                    {
                        return opcode.ToString("X2") + " NOP";
                    }
                case 0x01:
                    {
                        return opcode.ToString("X2") + " " + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 1)).ToString("X2") + " " + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 2)).ToString("X2") + " LD BC, " + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 2)).ToString("X2") + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 1)).ToString("X2");
                    }
                case 0x02:
                    {
                        return opcode.ToString("X2") + " LD (BC), A";
                    }
                case 0x03:
                    {
                        return opcode.ToString("X2") + " INC BC";
                    }
                case 0x04:
                    {
                        return opcode.ToString("X2") + " INC B";
                    }
                case 0x05:
                    {
                        return opcode.ToString("X2") + " DEC B";
                    }
                case 0x06:
                    {
                        return opcode.ToString("X2") + " " + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 1)).ToString("X2") + " LD B, " + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 1)).ToString("X2");
                    }
                case 0x07:
                    {
                        return opcode.ToString("X2") + " RLCA";
                    }
                case 0x08:
                    {
                        return opcode.ToString("X2") + " " + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 1)).ToString("X2") + " " + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 2)).ToString("X2") + " LD (" + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 2)).ToString("X2") + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 1)).ToString("X2") + "), SP";
                    }
                case 0x09:
                    {
                        return opcode.ToString("X2") + " ADD HL, BC";
                    }
                case 0x0A:
                    {
                        return opcode.ToString("X2") + " LD A, (BC)";
                    }
                case 0x0B:
                    {
                        return opcode.ToString("X2") + " DEC BC";
                    }
                case 0x0C:
                    {
                        return opcode.ToString("X2") + " INC C";
                    }
                case 0x0D:
                    {
                        return opcode.ToString("X2") + " DEC C";
                    }
                case 0x0E:
                    {
                        return opcode.ToString("X2") + " " + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 1)).ToString("X2") + " LD C, " + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 1)).ToString("X2");
                    }
                case 0x0F:
                    {
                        return opcode.ToString("X2") + " RRCA";
                    }
                case 0x10:
                    {
                        return Decode_Opcode_10(_cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 1)));
                    }
                case 0x11:
                    {
                        return opcode.ToString("X2") + " " + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 1)).ToString("X2") + " " + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 2)).ToString("X2") + " LD DE, " + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 2)).ToString("X2") + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 1)).ToString("X2");
                    }
                case 0x12:
                    {
                        return opcode.ToString("X2") + " LD (DE), A";
                    }
                case 0x13:
                    {
                        return opcode.ToString("X2") + " INC DE";
                    }
                case 0x14:
                    {
                        return opcode.ToString("X2") + " INC D";
                    }
                case 0x15:
                    {
                        return opcode.ToString("X2") + " DEC D";
                    }
                case 0x16:
                    {
                        return opcode.ToString("X2") + " " + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 1)).ToString("X2") + " LD D, " + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 1)).ToString("X2");
                    }
                case 0x17:
                    {
                        return opcode.ToString("X2") + " RLA";
                    }
                case 0x18:
                    {
                        return opcode.ToString("X2") + " " + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 1)).ToString("X2") + " JR " + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 1)).ToString("X2");
                    }
                case 0x19:
                    {
                        return opcode.ToString("X2") + " ADD HL, DE";
                    }
                case 0x1A:
                    {
                        return opcode.ToString("X2") + " LD A, (DE)";
                    }
                case 0x1B:
                    {
                        return opcode.ToString("X2") + " DEC DE";
                    }
                case 0x1C:
                    {
                        return opcode.ToString("X2") + " INC E";
                    }
                case 0x1D:
                    {
                        return opcode.ToString("X2") + " DEC E";
                    }
                case 0x1E:
                    {
                        return opcode.ToString("X2") + " " + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 1)).ToString("X2") + " LD E, " + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 1)).ToString("X2");
                    }
                case 0x1F:
                    {
                        return opcode.ToString("X2") + " RRA";
                    }
                case 0x20:
                    {
                        return opcode.ToString("X2") + " " + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 1)).ToString("X2") + " JR NZ, " + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 1)).ToString("X2");
                    }
                case 0x21:
                    {
                        return opcode.ToString("X2") + " " + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 1)).ToString("X2") + " " + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 2)).ToString("X2") + " LD HL, " + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 2)).ToString("X2") + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 1)).ToString("X2");
                    }
                case 0x22:
                    {
                        return opcode.ToString("X2") + " LD (HLI), A";
                    }
                case 0x23:
                    {
                        return opcode.ToString("X2") + " INC HL";
                    }
                case 0x24:
                    {
                        return opcode.ToString("X2") + " INC H";
                    }
                case 0x25:
                    {
                        return opcode.ToString("X2") + " DEC H";
                    }
                case 0x26:
                    {
                        return opcode.ToString("X2") + " " + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 1)).ToString("X2") + " LD H, " + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 1)).ToString("X2");
                    }
                case 0x27:
                    {
                        return opcode.ToString("X2") + " DAA";
                    }
                case 0x28:
                    {
                        return opcode.ToString("X2") + " " + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 1)).ToString("X2") + " JR Z, " + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 1)).ToString("X2");
                    }
                case 0x29:
                    {
                        return opcode.ToString("X2") + " ADD HL, HL";
                    }
                case 0x2A:
                    {
                        return opcode.ToString("X2") + " LD A, (HLI)";
                    }
                case 0x2B:
                    {
                        return opcode.ToString("X2") + " DEC HL";
                    }
                case 0x2C:
                    {
                        return opcode.ToString("X2") + " INC L";
                    }
                case 0x2D:
                    {
                        return opcode.ToString("X2") + " DEC L";
                    }
                case 0x2E:
                    {
                        return opcode.ToString("X2") + " " + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 1)).ToString("X2") + " LD L, " + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 1)).ToString("X2");
                    }
                case 0x2F:
                    {
                        return opcode.ToString("X2") + " CPL";
                    }
                case 0x30:
                    {
                        return opcode.ToString("X2") + " " + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 1)).ToString("X2") + " JR NC, " + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 1)).ToString("X2");
                    }
                case 0x31:
                    {
                        return opcode.ToString("X2") + " " + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 1)).ToString("X2") + " " + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 2)).ToString("X2") + " LD SP, " + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 2)).ToString("X2") + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 1)).ToString("X2");
                    }
                case 0x32:
                    {
                        return opcode.ToString("X2") + " LD (HLD), A";
                    }
                case 0x33:
                    {
                        return opcode.ToString("X2") + " INC SP";
                    }
                case 0x34:
                    {
                        return opcode.ToString("X2") + " INC (HL)";
                    }
                case 0x35:
                    {
                        return opcode.ToString("X2") + " DEC (HL)";
                    }
                case 0x36:
                    {
                        return opcode.ToString("X2") + " " + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 1)).ToString("X2") + " LD (HL), " + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 1)).ToString("X2");
                    }
                case 0x37:
                    {
                        return opcode.ToString("X2") + " SCF";
                    }
                case 0x38:
                    {
                        return opcode.ToString("X2") + " " + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 1)).ToString("X2") + " JR C, " + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 1)).ToString("X2");
                    }
                case 0x39:
                    {
                        return opcode.ToString("X2") + " ADD HL, SP";
                    }
                case 0x3A:
                    {
                        return opcode.ToString("X2") + " LD A, (HLD)";
                    }
                case 0x3B:
                    {
                        return opcode.ToString("X2") + " DEC SP";
                    }
                case 0x3C:
                    {
                        return opcode.ToString("X2") + " INC A";
                    }
                case 0x3D:
                    {
                        return opcode.ToString("X2") + " DEC A";
                    }
                case 0x3E:
                    {
                        return opcode.ToString("X2") + " " + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 1)).ToString("X2") + " LD A, " + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 1)).ToString("X2");
                    }
                case 0x3F:
                    {
                        return opcode.ToString("X2") + " CCF";
                    }
                case 0x40:
                    {
                        return opcode.ToString("X2") + " LD B, B";
                    }
                case 0x41:
                    {
                        return opcode.ToString("X2") + " LD B, C";
                    }
                case 0x42:
                    {
                        return opcode.ToString("X2") + " LD B, D";
                    }
                case 0x43:
                    {
                        return opcode.ToString("X2") + " LD B, E";
                    }
                case 0x44:
                    {
                        return opcode.ToString("X2") + " LD B, H";
                    }
                case 0x45:
                    {
                        return opcode.ToString("X2") + " LD B, L";
                    }
                case 0x46:
                    {
                        return opcode.ToString("X2") + " LD B, (HL)";
                    }
                case 0x47:
                    {
                        return opcode.ToString("X2") + " LD B, A";
                    }
                case 0x48:
                    {
                        return opcode.ToString("X2") + " LD C, B";
                    }
                case 0x49:
                    {
                        return opcode.ToString("X2") + " LD C, C";
                    }
                case 0x4A:
                    {
                        return opcode.ToString("X2") + " LD C, D";
                    }
                case 0x4B:
                    {
                        return opcode.ToString("X2") + " LD C, E";
                    }
                case 0x4C:
                    {
                        return opcode.ToString("X2") + " LD C, H";
                    }
                case 0x4D:
                    {
                        return opcode.ToString("X2") + " LD C, L";
                    }
                case 0x4E:
                    {
                        return opcode.ToString("X2") + " LD C, (HL)";
                    }
                case 0x4F:
                    {
                        return opcode.ToString("X2") + " LD C, A";
                    }
                case 0x50:
                    {
                        return opcode.ToString("X2") + " LD D, B";
                    }
                case 0x51:
                    {
                        return opcode.ToString("X2") + " LD D, C";
                    }
                case 0x52:
                    {
                        return opcode.ToString("X2") + " LD D, D";
                    }
                case 0x53:
                    {
                        return opcode.ToString("X2") + " LD D, E";
                    }
                case 0x54:
                    {
                        return opcode.ToString("X2") + " LD D, H";
                    }
                case 0x55:
                    {
                        return opcode.ToString("X2") + " LD D, L";
                    }
                case 0x56:
                    {
                        return opcode.ToString("X2") + " LD D, (HL)";
                    }
                case 0x57:
                    {
                        return opcode.ToString("X2") + " LD D, A";
                    }
                case 0x58:
                    {
                        return opcode.ToString("X2") + " LD E, B";
                    }
                case 0x59:
                    {
                        return opcode.ToString("X2") + " LD E, C";
                    }
                case 0x5A:
                    {
                        return opcode.ToString("X2") + " LD E, D";
                    }
                case 0x5B:
                    {
                        return opcode.ToString("X2") + " LD E, E";
                    }
                case 0x5C:
                    {
                        return opcode.ToString("X2") + " LD E, H";
                    }
                case 0x5D:
                    {
                        return opcode.ToString("X2") + " LD E, L";
                    }
                case 0x5E:
                    {
                        return opcode.ToString("X2") + " LD E, (HL)";
                    }
                case 0x5F:
                    {
                        return opcode.ToString("X2") + " LD E, A";
                    }
                case 0x60:
                    {
                        return opcode.ToString("X2") + " LD H, B";
                    }
                case 0x61:
                    {
                        return opcode.ToString("X2") + " LD H, C";
                    }
                case 0x62:
                    {
                        return opcode.ToString("X2") + " LD H, D";
                    }
                case 0x63:
                    {
                        return opcode.ToString("X2") + " LD H, E";
                    }
                case 0x64:
                    {
                        return opcode.ToString("X2") + " LD H, H";
                    }
                case 0x65:
                    {
                        return opcode.ToString("X2") + " LD H, L";
                    }
                case 0x66:
                    {
                        return opcode.ToString("X2") + " LD H, (HL)";
                    }
                case 0x67:
                    {
                        return opcode.ToString("X2") + " LD H, A";
                    }
                case 0x68:
                    {
                        return opcode.ToString("X2") + " LD L, B";
                    }
                case 0x69:
                    {
                        return opcode.ToString("X2") + " LD L, C";
                    }
                case 0x6A:
                    {
                        return opcode.ToString("X2") + " LD L, D";
                    }
                case 0x6B:
                    {
                        return opcode.ToString("X2") + " LD L, E";
                    }
                case 0x6C:
                    {
                        return opcode.ToString("X2") + " LD L, H";
                    }
                case 0x6D:
                    {
                        return opcode.ToString("X2") + " LD L, L";
                    }
                case 0x6E:
                    {
                        return opcode.ToString("X2") + " LD L, (HL)";
                    }
                case 0x6F:
                    {
                        return opcode.ToString("X2") + " LD L, A";
                    }
                case 0x70:
                    {
                        return opcode.ToString("X2") + " LD (HL), B";
                    }
                case 0x71:
                    {
                        return opcode.ToString("X2") + " LD (HL), C";
                    }
                case 0x72:
                    {
                        return opcode.ToString("X2") + " LD (HL), D";
                    }
                case 0x73:
                    {
                        return opcode.ToString("X2") + " LD (HL), E";
                    }
                case 0x74:
                    {
                        return opcode.ToString("X2") + " LD (HL), H";
                    }
                case 0x75:
                    {
                        return opcode.ToString("X2") + " LD (HL), L";
                    }
                case 0x76:
                    {
                        return opcode.ToString("X2") + " HALT";
                    }
                case 0x77:
                    {
                        return opcode.ToString("X2") + " LD (HL), A";
                    }
                case 0x78:
                    {
                        return opcode.ToString("X2") + " LD A, B";
                    }
                case 0x79:
                    {
                        return opcode.ToString("X2") + " LD A, C";
                    }
                case 0x7A:
                    {
                        return opcode.ToString("X2") + " LD A, D";
                    }
                case 0x7B:
                    {
                        return opcode.ToString("X2") + " LD A, E";
                    }
                case 0x7C:
                    {
                        return opcode.ToString("X2") + " LD A, H";
                    }
                case 0x7D:
                    {
                        return opcode.ToString("X2") + " LD A, L";
                    }
                case 0x7E:
                    {
                        return opcode.ToString("X2") + " LD A, (HL)";
                    }
                case 0x7F:
                    {
                        return opcode.ToString("X2") + " LD A, A";
                    }
                case 0x80:
                    {
                        return opcode.ToString("X2") + " ADD B";
                    }
                case 0x81:
                    {
                        return opcode.ToString("X2") + " ADD C";
                    }
                case 0x82:
                    {
                        return opcode.ToString("X2") + " ADD D";
                    }
                case 0x83:
                    {
                        return opcode.ToString("X2") + " ADD  E";
                    }
                case 0x84:
                    {
                        return opcode.ToString("X2") + " ADD H";
                    }
                case 0x85:
                    {
                        return opcode.ToString("X2") + " ADD L";
                    }
                case 0x86:
                    {
                        return opcode.ToString("X2") + " ADD (HL)";
                    }
                case 0x87:
                    {
                        return opcode.ToString("X2") + " ADD A";
                    }
                case 0x88:
                    {
                        return opcode.ToString("X2") + " ADC B";
                    }
                case 0x89:
                    {
                        return opcode.ToString("X2") + " ADC C";
                    }
                case 0x8A:
                    {
                        return opcode.ToString("X2") + " ADC D";
                    }
                case 0x8B:
                    {
                        return opcode.ToString("X2") + " ADC E";
                    }
                case 0x8C:
                    {
                        return opcode.ToString("X2") + " ADC H";
                    }
                case 0x8D:
                    {
                        return opcode.ToString("X2") + " ADC L";
                    }
                case 0x8E:
                    {
                        return opcode.ToString("X2") + " ADC (HL)";
                    }
                case 0x8F:
                    {
                        return opcode.ToString("X2") + " ADC A";
                    }
                case 0x90:
                    {
                        return opcode.ToString("X2") + " SUB B";
                    }
                case 0x91:
                    {
                        return opcode.ToString("X2") + " SUB C";
                    }
                case 0x92:
                    {
                        return opcode.ToString("X2") + " SUB D";
                    }
                case 0x93:
                    {
                        return opcode.ToString("X2") + " SUB E";
                    }
                case 0x94:
                    {
                        return opcode.ToString("X2") + " SUB H";
                    }
                case 0x95:
                    {
                        return opcode.ToString("X2") + " SUB L";
                    }
                case 0x96:
                    {
                        return opcode.ToString("X2") + " SUB (HL)";
                    }
                case 0x97:
                    {
                        return opcode.ToString("X2") + " SUB A";
                    }
                case 0x98:
                    {
                        return opcode.ToString("X2") + " SBC B";
                    }
                case 0x99:
                    {
                        return opcode.ToString("X2") + " SBC C";
                    }
                case 0x9A:
                    {
                        return opcode.ToString("X2") + " SBC D";
                    }
                case 0x9B:
                    {
                        return opcode.ToString("X2") + " SBC E";
                    }
                case 0x9C:
                    {
                        return opcode.ToString("X2") + " SBC H";
                    }
                case 0x9D:
                    {
                        return opcode.ToString("X2") + " SBC L";
                    }
                case 0x9E:
                    {
                        return opcode.ToString("X2") + " SBC (HL)";
                    }
                case 0x9F:
                    {
                        return opcode.ToString("X2") + " SBC A";
                    }
                case 0xA0:
                    {
                        return opcode.ToString("X2") + " AND B";
                    }
                case 0xA1:
                    {
                        return opcode.ToString("X2") + " AND C";
                    }
                case 0xA2:
                    {
                        return opcode.ToString("X2") + " AND D";
                    }
                case 0xA3:
                    {
                        return opcode.ToString("X2") + " AND E";
                    }
                case 0xA4:
                    {
                        return opcode.ToString("X2") + " AND H";
                    }
                case 0xA5:
                    {
                        return opcode.ToString("X2") + " AND L";
                    }
                case 0xA6:
                    {
                        return opcode.ToString("X2") + " AND (HL)";
                    }
                case 0xA7:
                    {
                        return opcode.ToString("X2") + " AND A";
                    }
                case 0xA8:
                    {
                        return opcode.ToString("X2") + " XOR B";
                    }
                case 0xA9:
                    {
                        return opcode.ToString("X2") + " XOR C";
                    }
                case 0xAA:
                    {
                        return opcode.ToString("X2") + " XOR D";
                    }
                case 0xAB:
                    {
                        return opcode.ToString("X2") + " XOR E";
                    }
                case 0xAC:
                    {
                        return opcode.ToString("X2") + " XOR H";
                    }
                case 0xAD:
                    {
                        return opcode.ToString("X2") + " XOR L";
                    }
                case 0xAE:
                    {
                        return opcode.ToString("X2") + " XOR (HL)";
                    }
                case 0xAF:
                    {
                        return opcode.ToString("X2") + " XOR A";
                    }
                case 0xB0:
                    {
                        return opcode.ToString("X2") + " OR B";
                    }
                case 0xB1:
                    {
                        return opcode.ToString("X2") + " OR C";
                    }
                case 0xB2:
                    {
                        return opcode.ToString("X2") + " OR D";
                    }
                case 0xB3:
                    {
                        return opcode.ToString("X2") + " OR E";
                    }
                case 0xB4:
                    {
                        return opcode.ToString("X2") + " OR H";
                    }
                case 0xB5:
                    {
                        return opcode.ToString("X2") + " OR L";
                    }
                case 0xB6:
                    {
                        return opcode.ToString("X2") + " OR (HL)";
                    }
                case 0xB7:
                    {
                        return opcode.ToString("X2") + " OR A";
                    }
                case 0xB8:
                    {
                        return opcode.ToString("X2") + " CPL B";
                    }
                case 0xB9:
                    {
                        return opcode.ToString("X2") + " CPL C";
                    }
                case 0xBA:
                    {
                        return opcode.ToString("X2") + " CPL D";
                    }
                case 0xBB:
                    {
                        return opcode.ToString("X2") + " CPL E";
                    }
                case 0xBC:
                    {
                        return opcode.ToString("X2") + " CPL H";
                    }
                case 0xBD:
                    {
                        return opcode.ToString("X2") + " CPL L";
                    }
                case 0xBE:
                    {
                        return opcode.ToString("X2") + " CPL (HL)";
                    }
                case 0xBF:
                    {
                        return opcode.ToString("X2") + " CPL A";
                    }
                case 0xC0:
                    {
                        return opcode.ToString("X2") + " RET NZ";
                    }
                case 0xC1:
                    {
                        return opcode.ToString("X2") + " POP BC";
                    }
                case 0xC2:
                    {
                        return opcode.ToString("X2") + " " + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 1)).ToString("X2") + " " + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 2)).ToString("X2") + " JP NZ, " + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 2)).ToString("X2") + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 1)).ToString("X2");
                    }
                case 0xC3:
                    {
                        return opcode.ToString("X2") + " " + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 1)).ToString("X2") + " " + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 2)).ToString("X2") + " JP " + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 2)).ToString("X2") + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 1)).ToString("X2");
                    }
                case 0xC4:
                    {
                        return opcode.ToString("X2") + " " + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 1)).ToString("X2") + " " + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 2)).ToString("X2") + " CALL NZ, " + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 2)).ToString("X2") + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 1)).ToString("X2");
                    }
                case 0xC5:
                    {
                        return opcode.ToString("X2") + " PUSH BC";
                    }
                case 0xC6:
                    {
                        return opcode.ToString("X2") + " " + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 1)).ToString("X2") + " ADD " + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 1)).ToString("X2");
                    }
                case 0xC7:
                    {
                        return opcode.ToString("X2") + " RST 0x0000";
                    }
                case 0xC8:
                    {
                        return opcode.ToString("X2") + " RET Z";
                    }
                case 0xC9:
                    {
                        return opcode.ToString("X2") + " RET";
                    }
                case 0xCA:
                    {
                        return opcode.ToString("X2") + " " + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 1)).ToString("X2") + " " + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 2)).ToString("X2") + " JP Z, " + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 2)).ToString("X2") + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 1)).ToString("X2");
                    }
                case 0xCB:
                    {
                        return opcode.ToString("X2") + " " + Decode_Opcode_CB(_cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 1)));
                    }
                case 0xCC:
                    {
                        return opcode.ToString("X2") + " " + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 1)).ToString("X2") + " " + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 2)).ToString("X2") + " CALL Z, " + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 2)).ToString("X2") + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 1)).ToString("X2");
                    }
                case 0xCD:
                    {
                        return opcode.ToString("X2") + " " + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 1)).ToString("X2") + " " + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 2)).ToString("X2") + " CALL " + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 2)).ToString("X2") + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 1)).ToString("X2");
                    }
                case 0xCE:
                    {
                        return opcode.ToString("X2") + " " + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 1)).ToString("X2") + " ADC " + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 1)).ToString("X2");
                    }
                case 0xCF:
                    {
                        return opcode.ToString("X2") + " RST 0x0008";
                    }
                case 0xD0:
                    {
                        return opcode.ToString("X2") + " RET NC";
                    }
                case 0xD1:
                    {
                        return opcode.ToString("X2") + " POP DE";
                    }
                case 0xD2:
                    {
                        return opcode.ToString("X2") + " " + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 1)).ToString("X2") + " " + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 2)).ToString("X2") + " JP NC, " + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 2)).ToString("X2") + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 1)).ToString("X2");
                    }
                case 0xD4:
                    {
                        return opcode.ToString("X2") + " " + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 1)).ToString("X2") + " " + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 2)).ToString("X2") + " CALL NC, " + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 2)).ToString("X2") + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 1)).ToString("X2");
                    }
                case 0xD5:
                    {
                        return opcode.ToString("X2") + " PUSH DE";
                    }
                case 0xD6:
                    {
                        return opcode.ToString("X2") + " " + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 1)).ToString("X2") + " SUB " + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 1)).ToString("X2");
                    }
                case 0xD7:
                    {
                        return opcode.ToString("X2") + " RST 0x0010";
                    }
                case 0xD8:
                    {
                        return opcode.ToString("X2") + " RET C";
                    }
                case 0xD9:
                    {
                        return opcode.ToString("X2") + " RETI";
                    }
                case 0xDA:
                    {
                        return opcode.ToString("X2") + " " + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 1)).ToString("X2") + " " + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 2)).ToString("X2") + " JP C, " + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 2)).ToString("X2") + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 1)).ToString("X2");
                    }
                case 0xDC:
                    {
                        return opcode.ToString("X2") + " " + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 1)).ToString("X2") + " " + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 2)).ToString("X2") + " CALL C, " + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 2)).ToString("X2") + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 1)).ToString("X2");
                    }
                case 0xDE:
                    {
                        return opcode.ToString("X2") + " " + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 1)).ToString("X2") + " SBC " + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 1)).ToString("X2");
                    }
                case 0xDF:
                    {
                        return opcode.ToString("X2") + " RST 0x0018";
                    }
                case 0xE0:
                    {
                        return opcode.ToString("X2") + " " + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 1)).ToString("X2") + " LD (" + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 1)).ToString("X2") + "), A";
                    }
                case 0xE1:
                    {
                        return opcode.ToString("X2") + " POP HL";
                    }
                case 0xE2:
                    {
                        return opcode.ToString("X2") + " LD (C), A";
                    }
                case 0xE5:
                    {
                        return opcode.ToString("X2") + " PUSH HL";
                    }
                case 0xE6:
                    {
                        return opcode.ToString("X2") + " " + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 1)).ToString("X2") + " AND " + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 1)).ToString("X2");
                    }
                case 0xE7:
                    {
                        return opcode.ToString("X2") + " RST 0x0020";
                    }
                case 0xE8:
                    {
                        return opcode.ToString("X2") + " " + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 1)).ToString("X2") + " ADD sp, " + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 1)).ToString("X2");
                    }
                case 0xE9:
                    {
                        return opcode.ToString("X2") + " JP (HL)";
                    }
                case 0xEA:
                    {
                        return opcode.ToString("X2") + " " + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 1)).ToString("X2") + " " + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 2)).ToString("X2") + " LD (" + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 2)).ToString("X2") + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 1)).ToString("X2") + "), A";
                    }
                case 0xEE:
                    {
                        return opcode.ToString("X2") + " " + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 1)).ToString("X2") + " XOR " + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 1)).ToString("X2");
                    }
                case 0xEF:
                    {
                        return opcode.ToString("X2") + " RST 0x0028";
                    }
                case 0xF0:
                    {
                        return opcode.ToString("X2") + " " + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 1)).ToString("X2") + " LD A, (" + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 1)).ToString("X2") + ")";
                    }
                case 0xF1:
                    {
                        return opcode.ToString("X2") + " POP AF";
                    }
                case 0xF2:
                    {
                        return opcode.ToString("X2") + " LD A, (C)";
                    }
                case 0xF3:
                    {
                        return opcode.ToString("X2") + " DI";
                    }
                case 0xF5:
                    {
                        return opcode.ToString("X2") + " PUSH AF";
                    }
                case 0xF6:
                    {
                        return opcode.ToString("X2") + " " + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 1)).ToString("X2") + " OR " + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 1)).ToString("X2");
                    }
                case 0xF7:
                    {
                        return opcode.ToString("X2") + " RST 0x0030";
                    }
                case 0xF8:
                    {
                        return opcode.ToString("X2") + " LD HL, SP";
                    }
                case 0xF9:
                    {
                        return opcode.ToString("X2") + " LD SP, HL";
                    }
                case 0xFA:
                    {
                        return opcode.ToString("X2") + " " + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 1)).ToString("X2") + " " + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 2)).ToString("X2") + " LD A, (" + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 2)).ToString("X2") + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 1)).ToString("X2") + ")";
                    }
                case 0xFB:
                    {
                        return opcode.ToString("X2") + " EI";
                    }
                case 0xFE:
                    {
                        return opcode.ToString("X2") + " " + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 1)).ToString("X2") + " CP " + _cpu.retrieve_memory.read_byte((ushort)(_cpu.retrieve_pc + 1)).ToString("X2");
                    }
                case 0xFF:
                    {
                        return opcode.ToString("X2") + " RST 0x0038";
                    }
                default:
                    {
                        return opcode.ToString("X2") + " UNKNOWN";
                    }
            }
        }
        #endregion

        #region decode opcode CB
        private string Decode_Opcode_CB(byte opcode2)
        {
            switch (opcode2)
            {
                case 0x00:
                    {
                        return opcode2.ToString("X2") + " RLC B";
                    }
                case 0x01:
                    {
                        return opcode2.ToString("X2") + " RLC C";
                    }
                case 0x02:
                    {
                        return opcode2.ToString("X2") + " RLC D";
                    }
                case 0x03:
                    {
                        return opcode2.ToString("X2") + " RLC E";
                    }
                case 0x04:
                    {
                        return opcode2.ToString("X2") + " RLC H";
                    }
                case 0x05:
                    {
                        return opcode2.ToString("X2") + " RLC L";
                    }
                case 0x06:
                    {
                        return opcode2.ToString("X2") + " RLC (HL)";
                    }
                case 0x07:
                    {
                        return opcode2.ToString("X2") + " RLC A";
                    }
                case 0x08:
                    {
                        return opcode2.ToString("X2") + " RRC B";
                    }
                case 0x09:
                    {
                        return opcode2.ToString("X2") + " RRC C";
                    }
                case 0x0A:
                    {
                        return opcode2.ToString("X2") + " RRC D";
                    }
                case 0x0B:
                    {
                        return opcode2.ToString("X2") + " RRC E";
                    }
                case 0x0C:
                    {
                        return opcode2.ToString("X2") + " RRC H";
                    }
                case 0x0D:
                    {
                        return opcode2.ToString("X2") + " RRC L";
                    }
                case 0x0E:
                    {
                        return opcode2.ToString("X2") + " RRC (HL)";
                    }
                case 0x0F:
                    {
                        return opcode2.ToString("X2") + " RRC A";
                    }
                case 0x10:
                    {
                        return opcode2.ToString("X2") + " RL B";
                    }
                case 0x11:
                    {
                        return opcode2.ToString("X2") + " RL C";
                    }
                case 0x12:
                    {
                        return opcode2.ToString("X2") + " RL D";
                    }
                case 0x13:
                    {
                        return opcode2.ToString("X2") + " RL E";
                    }
                case 0x14:
                    {
                        return opcode2.ToString("X2") + " RL H";
                    }
                case 0x15:
                    {
                        return opcode2.ToString("X2") + " RL L";
                    }
                case 0x16:
                    {
                        return opcode2.ToString("X2") + " RL (HL)";
                    }
                case 0x17:
                    {
                        return opcode2.ToString("X2") + " RL A";
                    }
                case 0x18:
                    {
                        return opcode2.ToString("X2") + " RR B";
                    }
                case 0x19:
                    {
                        return opcode2.ToString("X2") + " RR C";
                    }
                case 0x1A:
                    {
                        return opcode2.ToString("X2") + " RR D";
                    }
                case 0x1B:
                    {
                        return opcode2.ToString("X2") + " RR E";
                    }
                case 0x1C:
                    {
                        return opcode2.ToString("X2") + " RR H";
                    }
                case 0x1D:
                    {
                        return opcode2.ToString("X2") + " RR L";
                    }
                case 0x1E:
                    {
                        return opcode2.ToString("X2") + " RR (HL)";
                    }
                case 0x1F:
                    {
                        return opcode2.ToString("X2") + " RR A";
                    }
                case 0x20:
                    {
                        return opcode2.ToString("X2") + " SLA B";
                    }
                case 0x21:
                    {
                        return opcode2.ToString("X2") + " SLA C";
                    }
                case 0x22:
                    {
                        return opcode2.ToString("X2") + " SLA D";
                    }
                case 0x23:
                    {
                        return opcode2.ToString("X2") + " SLA E";
                    }
                case 0x24:
                    {
                        return opcode2.ToString("X2") + " SLA H";
                    }
                case 0x25:
                    {
                        return opcode2.ToString("X2") + " SLA L";
                    }
                case 0x26:
                    {
                        return opcode2.ToString("X2") + " SLA (HL)";
                    }
                case 0x27:
                    {
                        return opcode2.ToString("X2") + " SLA A";
                    }
                case 0x28:
                    {
                        return opcode2.ToString("X2") + " SRA B";
                    }
                case 0x29:
                    {
                        return opcode2.ToString("X2") + " SRA C";
                    }
                case 0x2A:
                    {
                        return opcode2.ToString("X2") + " SRA D";
                    }
                case 0x2B:
                    {
                        return opcode2.ToString("X2") + " SRA E";
                    }
                case 0x2C:
                    {
                        return opcode2.ToString("X2") + " SRA H";
                    }
                case 0x2D:
                    {
                        return opcode2.ToString("X2") + " SRA L";
                    }
                case 0x2E:
                    {
                        return opcode2.ToString("X2") + " SRA (HL)";
                    }
                case 0x2F:
                    {
                        return opcode2.ToString("X2") + " SRA A";
                    }
                case 0x30:
                    {
                        return opcode2.ToString("X2") + " SWAP B";
                    }
                case 0x31:
                    {
                        return opcode2.ToString("X2") + " SWAP C";
                    }
                case 0x32:
                    {
                        return opcode2.ToString("X2") + " SWAP D";
                    }
                case 0x33:
                    {
                        return opcode2.ToString("X2") + " SWAP E";
                    }
                case 0x34:
                    {
                        return opcode2.ToString("X2") + " SWAP H";
                    }
                case 0x35:
                    {
                        return opcode2.ToString("X2") + " SWAP L";
                    }
                case 0x36:
                    {
                        return opcode2.ToString("X2") + " SWAP (HL)";
                    }
                case 0x37:
                    {
                        return opcode2.ToString("X2") + " SWAP A";
                    }
                case 0x38:
                    {
                        return opcode2.ToString("X2") + " SRL B";
                    }
                case 0x39:
                    {
                        return opcode2.ToString("X2") + " SRL C";
                    }
                case 0x3A:
                    {
                        return opcode2.ToString("X2") + " SRL D";
                    }
                case 0x3B:
                    {
                        return opcode2.ToString("X2") + " SRL E";
                    }
                case 0x3C:
                    {
                        return opcode2.ToString("X2") + " SRL H";
                    }
                case 0x3D:
                    {
                        return opcode2.ToString("X2") + " SRL L";
                    }
                case 0x3E:
                    {
                        return opcode2.ToString("X2") + " SRL (HL)";
                    }
                case 0x3F:
                    {
                        return opcode2.ToString("X2") + " SRL A";
                    }
                case 0x40:
                    {
                        return opcode2.ToString("X2") + " BIT 0, B";
                    }
                case 0x41:
                    {
                        return opcode2.ToString("X2") + " BIT 0, C";
                    }
                case 0x42:
                    {
                        return opcode2.ToString("X2") + " BIT 0, D";
                    }
                case 0x43:
                    {
                        return opcode2.ToString("X2") + " BIT 0, E";
                    }
                case 0x44:
                    {
                        return opcode2.ToString("X2") + " BIT 0, H";
                    }
                case 0x45:
                    {
                        return opcode2.ToString("X2") + " BIT 0, L";
                    }
                case 0x46:
                    {
                        return opcode2.ToString("X2") + " BIT 0, (HL)";
                    }
                case 0x47:
                    {
                        return opcode2.ToString("X2") + " BIT 0, A";
                    }
                case 0x48:
                    {
                        return opcode2.ToString("X2") + " BIT 1, B";
                    }
                case 0x49:
                    {
                        return opcode2.ToString("X2") + " BIT 1, C";
                    }
                case 0x4A:
                    {
                        return opcode2.ToString("X2") + " BIT 1, D";
                    }
                case 0x4B:
                    {
                        return opcode2.ToString("X2") + " BIT 1, E";
                    }
                case 0x4C:
                    {
                        return opcode2.ToString("X2") + " BIT 1, H";
                    }
                case 0x4D:
                    {
                        return opcode2.ToString("X2") + " BIT 1, L";
                    }
                case 0x4E:
                    {
                        return opcode2.ToString("X2") + " BIT 1, (HL)";
                    }
                case 0x4F:
                    {
                        return opcode2.ToString("X2") + " BIT 1, A";
                    }
                case 0x50:
                    {
                        return opcode2.ToString("X2") + " BIT 2, B";
                    }
                case 0x51:
                    {
                        return opcode2.ToString("X2") + " BIT 2, C";
                    }
                case 0x52:
                    {
                        return opcode2.ToString("X2") + " BIT 2, D";
                    }
                case 0x53:
                    {
                        return opcode2.ToString("X2") + " BIT 2, E";
                    }
                case 0x54:
                    {
                        return opcode2.ToString("X2") + " BIT 2, H";
                    }
                case 0x55:
                    {
                        return opcode2.ToString("X2") + " BIT 2, L";
                    }
                case 0x56:
                    {
                        return opcode2.ToString("X2") + " BIT 2, (HL)";
                    }
                case 0x57:
                    {
                        return opcode2.ToString("X2") + " BIT 2, A";
                    }
                case 0x58:
                    {
                        return opcode2.ToString("X2") + " BIT 3, B";
                    }
                case 0x59:
                    {
                        return opcode2.ToString("X2") + " BIT 3, C";
                    }
                case 0x5A:
                    {
                        return opcode2.ToString("X2") + " BIT 3, D";
                    }
                case 0x5B:
                    {
                        return opcode2.ToString("X2") + " BIT 3, E";
                    }
                case 0x5C:
                    {
                        return opcode2.ToString("X2") + " BIT 3, H";
                    }
                case 0x5D:
                    {
                        return opcode2.ToString("X2") + " BIT 3, L";
                    }
                case 0x5E:
                    {
                        return opcode2.ToString("X2") + " BIT 3, (HL)";
                    }
                case 0x5F:
                    {
                        return opcode2.ToString("X2") + " BIT 3, A";
                    }
                case 0x60:
                    {
                        return opcode2.ToString("X2") + " BIT 4, B";
                    }
                case 0x61:
                    {
                        return opcode2.ToString("X2") + " BIT 4, C";
                    }
                case 0x62:
                    {
                        return opcode2.ToString("X2") + " BIT 4, D";
                    }
                case 0x63:
                    {
                        return opcode2.ToString("X2") + " BIT 4, E";
                    }
                case 0x64:
                    {
                        return opcode2.ToString("X2") + " BIT 4, H";
                    }
                case 0x65:
                    {
                        return opcode2.ToString("X2") + " BIT 4, L";
                    }
                case 0x66:
                    {
                        return opcode2.ToString("X2") + " BIT 4, (HL)";
                    }
                case 0x67:
                    {
                        return opcode2.ToString("X2") + " BIT 4, A";
                    }
                case 0x68:
                    {
                        return opcode2.ToString("X2") + " BIT 5, B";
                    }
                case 0x69:
                    {
                        return opcode2.ToString("X2") + " BIT 5, C";
                    }
                case 0x6A:
                    {
                        return opcode2.ToString("X2") + " BIT 5, D";
                    }
                case 0x6B:
                    {
                        return opcode2.ToString("X2") + " BIT 5, E";
                    }
                case 0x6C:
                    {
                        return opcode2.ToString("X2") + " BIT 5, H";
                    }
                case 0x6D:
                    {
                        return opcode2.ToString("X2") + " BIT 5, L";
                    }
                case 0x6E:
                    {
                        return opcode2.ToString("X2") + " BIT 5, (HL)";
                    }
                case 0x6F:
                    {
                        return opcode2.ToString("X2") + " BIT 5, A";
                    }
                case 0x70:
                    {
                        return opcode2.ToString("X2") + " BIT 6, B";
                    }
                case 0x71:
                    {
                        return opcode2.ToString("X2") + " BIT 6, C";
                    }
                case 0x72:
                    {
                        return opcode2.ToString("X2") + " BIT 6, D";
                    }
                case 0x73:
                    {
                        return opcode2.ToString("X2") + " BIT 6, E";
                    }
                case 0x74:
                    {
                        return opcode2.ToString("X2") + " BIT 6, H";
                    }
                case 0x75:
                    {
                        return opcode2.ToString("X2") + " BIT 6, L";
                    }
                case 0x76:
                    {
                        return opcode2.ToString("X2") + " BIT 6, (HL)";
                    }
                case 0x77:
                    {
                        return opcode2.ToString("X2") + " BIT 6, A";
                    }
                case 0x78:
                    {
                        return opcode2.ToString("X2") + " BIT 7, B";
                    }
                case 0x79:
                    {
                        return opcode2.ToString("X2") + " BIT 7, C";
                    }
                case 0x7A:
                    {
                        return opcode2.ToString("X2") + " BIT 7, D";
                    }
                case 0x7B:
                    {
                        return opcode2.ToString("X2") + " BIT 7, E";
                    }
                case 0x7C:
                    {
                        return opcode2.ToString("X2") + " BIT 7, H";
                    }
                case 0x7D:
                    {
                        return opcode2.ToString("X2") + " BIT 7, L";
                    }
                case 0x7E:
                    {
                        return opcode2.ToString("X2") + " BIT 7, (HL)";
                    }
                case 0x7F:
                    {
                        return opcode2.ToString("X2") + " BIT 7, A";
                    }
                case 0x80:
                    {
                        return opcode2.ToString("X2") + " RES 0, B";
                    }
                case 0x81:
                    {
                        return opcode2.ToString("X2") + " RES 0, C";
                    }
                case 0x82:
                    {
                        return opcode2.ToString("X2") + " RES 0, D";
                    }
                case 0x83:
                    {
                        return opcode2.ToString("X2") + " RES 0, E";
                    }
                case 0x84:
                    {
                        return opcode2.ToString("X2") + " RES 0, H";
                    }
                case 0x85:
                    {
                        return opcode2.ToString("X2") + " RES 0, L";
                    }
                case 0x86:
                    {

                        return opcode2.ToString("X2") + " RES 0, (HL)";
                    }
                case 0x87:
                    {
                        return opcode2.ToString("X2") + " RES 0, A";
                    }
                case 0x88:
                    {
                        return opcode2.ToString("X2") + " RES 1, B";
                    }
                case 0x89:
                    {
                        return opcode2.ToString("X2") + " RES 1, C";
                    }
                case 0x8A:
                    {
                        return opcode2.ToString("X2") + " RES 1, D";
                    }
                case 0x8B:
                    {
                        return opcode2.ToString("X2") + " RES 1, E";
                    }
                case 0x8C:
                    {
                        return opcode2.ToString("X2") + " RES 1, H";
                    }
                case 0x8D:
                    {
                        return opcode2.ToString("X2") + " RES 1, L";
                    }
                case 0x8E:
                    {
                        return opcode2.ToString("X2") + " RES 1, (HL)";
                    }
                case 0x8F:
                    {
                        return opcode2.ToString("X2") + " RES 1, A";
                    }
                case 0x90:
                    {
                        return opcode2.ToString("X2") + " RES 2, B";
                    }
                case 0x91:
                    {
                        return opcode2.ToString("X2") + " RES 2, C";
                    }
                case 0x92:
                    {
                        return opcode2.ToString("X2") + " RES 2, D";
                    }
                case 0x93:
                    {
                        return opcode2.ToString("X2") + " RES 2, E";
                    }
                case 0x94:
                    {
                        return opcode2.ToString("X2") + " RES 2, H";
                    }
                case 0x95:
                    {
                        return opcode2.ToString("X2") + " RES 2, L";
                    }
                case 0x96:
                    {
                        return opcode2.ToString("X2") + " RES 2, (HL)";
                    }
                case 0x97:
                    {
                        return opcode2.ToString("X2") + " RES 2, A";
                    }
                case 0x98:
                    {
                        return opcode2.ToString("X2") + " RES 3, B";
                    }
                case 0x99:
                    {
                        return opcode2.ToString("X2") + " RES 3, C";
                    }
                case 0x9A:
                    {
                        return opcode2.ToString("X2") + " RES 3, D";
                    }
                case 0x9B:
                    {
                        return opcode2.ToString("X2") + " RES 3, E";
                    }
                case 0x9C:
                    {
                        return opcode2.ToString("X2") + " RES 3, H";
                    }
                case 0x9D:
                    {
                        return opcode2.ToString("X2") + " RES 3, L";
                    }
                case 0x9E:
                    {
                        return opcode2.ToString("X2") + " RES 3, (HL)";
                    }
                case 0x9F:
                    {
                        return opcode2.ToString("X2") + " RES 3, A";
                    }
                case 0xA0:
                    {
                        return opcode2.ToString("X2") + " RES 4, B";
                    }
                case 0xA1:
                    {
                        return opcode2.ToString("X2") + " RES 4, C";
                    }
                case 0xA2:
                    {
                        return opcode2.ToString("X2") + " RES 4, D";
                    }
                case 0xA3:
                    {
                        return opcode2.ToString("X2") + " RES 4, E";
                    }
                case 0xA4:
                    {
                        return opcode2.ToString("X2") + " RES 4, H";
                    }
                case 0xA5:
                    {
                        return opcode2.ToString("X2") + " RES 4, L";
                    }
                case 0xA6:
                    {
                        return opcode2.ToString("X2") + " RES 4, (HL)";
                    }
                case 0xA7:
                    {
                        return opcode2.ToString("X2") + " RES 4, A";
                    }
                case 0xA8:
                    {
                        return opcode2.ToString("X2") + " RES 5, B";
                    }
                case 0xA9:
                    {
                        return opcode2.ToString("X2") + " RES 5, C";
                    }
                case 0xAA:
                    {
                        return opcode2.ToString("X2") + " RES 5, D";
                    }
                case 0xAB:
                    {
                        return opcode2.ToString("X2") + " RES 5, E";
                    }
                case 0xAC:
                    {
                        return opcode2.ToString("X2") + " RES 5, H";
                    }
                case 0xAD:
                    {
                        return opcode2.ToString("X2") + " RES 5, L";
                    }
                case 0xAE:
                    {
                        return opcode2.ToString("X2") + " RES 5, (HL)";
                    }
                case 0xAF:
                    {
                        return opcode2.ToString("X2") + " RES 5, A";
                    }
                case 0xB0:
                    {
                        return opcode2.ToString("X2") + " RES 6, B";
                    }
                case 0xB1:
                    {
                        return opcode2.ToString("X2") + " RES 6, C";
                    }
                case 0xB2:
                    {
                        return opcode2.ToString("X2") + " RES 6, D";
                    }
                case 0xB3:
                    {
                        return opcode2.ToString("X2") + " RES 6, E";
                    }
                case 0xB4:
                    {
                        return opcode2.ToString("X2") + " RES 6, H";
                    }
                case 0xB5:
                    {
                        return opcode2.ToString("X2") + " RES 6, L";
                    }
                case 0xB6:
                    {
                        return opcode2.ToString("X2") + " RES 6, (HL)";
                    }
                case 0xB7:
                    {
                        return opcode2.ToString("X2") + " RES 6, A";
                    }
                case 0xB8:
                    {
                        return opcode2.ToString("X2") + " RES 7, B";
                    }
                case 0xB9:
                    {
                        return opcode2.ToString("X2") + " RES 7, C";
                    }
                case 0xBA:
                    {
                        return opcode2.ToString("X2") + " RES 7, D";
                    }
                case 0xBB:
                    {
                        return opcode2.ToString("X2") + " RES 7, E";
                    }
                case 0xBC:
                    {
                        return opcode2.ToString("X2") + " RES 7, H";
                    }
                case 0xBD:
                    {
                        return opcode2.ToString("X2") + " RES 7, L";
                    }
                case 0xBE:
                    {
                        return opcode2.ToString("X2") + " RES 7, (HL)";
                    }
                case 0xBF:
                    {
                        return opcode2.ToString("X2") + " RES 7, A";
                    }
                case 0xC0:
                    {
                        return opcode2.ToString("X2") + " SET 0, B";
                    }
                case 0xC1:
                    {
                        return opcode2.ToString("X2") + " SET 0, C";
                    }
                case 0xC2:
                    {
                        return opcode2.ToString("X2") + " SET 0, D";
                    }
                case 0xC3:
                    {
                        return opcode2.ToString("X2") + " SET 0, E";
                    }
                case 0xC4:
                    {
                        return opcode2.ToString("X2") + " SET 0, H";
                    }
                case 0xC5:
                    {
                        return opcode2.ToString("X2") + " SET 0, L";
                    }
                case 0xC6:
                    {
                        return opcode2.ToString("X2") + " SET 0, (HL)";
                    }
                case 0xC7:
                    {
                        return opcode2.ToString("X2") + " SET 0, A";
                    }
                case 0xC8:
                    {
                        return opcode2.ToString("X2") + " SET 1, B";
                    }
                case 0xC9:
                    {
                        return opcode2.ToString("X2") + " SET 1, C";
                    }
                case 0xCA:
                    {
                        return opcode2.ToString("X2") + " SET 1, D";
                    }
                case 0xCB:
                    {
                        return opcode2.ToString("X2") + " SET 1, E";
                    }
                case 0xCC:
                    {
                        return opcode2.ToString("X2") + " SET 1, H";
                    }
                case 0xCD:
                    {
                        return opcode2.ToString("X2") + " SET 1, L";
                    }
                case 0xCE:
                    {
                        return opcode2.ToString("X2") + " SET 1, (HL)";
                    }
                case 0xCF:
                    {
                        return opcode2.ToString("X2") + " SET 1, A";
                    }
                case 0xD0:
                    {
                        return opcode2.ToString("X2") + " SET 2, B";
                    }
                case 0xD1:
                    {
                        return opcode2.ToString("X2") + " SET 2, C";
                    }
                case 0xD2:
                    {
                        return opcode2.ToString("X2") + " SET 2, D";
                    }
                case 0xD3:
                    {
                        return opcode2.ToString("X2") + " SET 2, E";
                    }
                case 0xD4:
                    {
                        return opcode2.ToString("X2") + " SET 2, H";
                    }
                case 0xD5:
                    {
                        return opcode2.ToString("X2") + " SET 2, L";
                    }
                case 0xD6:
                    {
                        return opcode2.ToString("X2") + " SET 2, (HL)";
                    }
                case 0xD7:
                    {
                        return opcode2.ToString("X2") + " SET 2, A";
                    }
                case 0xD8:
                    {
                        return opcode2.ToString("X2") + " SET 3, B";
                    }
                case 0xD9:
                    {
                        return opcode2.ToString("X2") + " SET 3, C";
                    }
                case 0xDA:
                    {
                        return opcode2.ToString("X2") + " SET 3, D";
                    }
                case 0xDB:
                    {
                        return opcode2.ToString("X2") + " SET 3, E";
                    }
                case 0xDC:
                    {
                        return opcode2.ToString("X2") + " SET 3, H";
                    }
                case 0xDD:
                    {
                        return opcode2.ToString("X2") + " SET 3, L";
                    }
                case 0xDE:
                    {
                        return opcode2.ToString("X2") + " SET 3, (HL)";
                    }
                case 0xDF:
                    {
                        return opcode2.ToString("X2") + " SET 3, A";
                    }
                case 0xE0:
                    {
                        return opcode2.ToString("X2") + " SET 4, B";
                    }
                case 0xE1:
                    {
                        return opcode2.ToString("X2") + " SET 4, C";
                    }
                case 0xE2:
                    {
                        return opcode2.ToString("X2") + " SET 4, D";
                    }
                case 0xE3:
                    {
                        return opcode2.ToString("X2") + " SET 4, E";
                    }
                case 0xE4:
                    {
                        return opcode2.ToString("X2") + " SET 4, H";
                    }
                case 0xE5:
                    {
                        return opcode2.ToString("X2") + " SET 4, L";
                    }
                case 0xE6:
                    {
                        return opcode2.ToString("X2") + " SET 4, (HL)";
                    }
                case 0xE7:
                    {
                        return opcode2.ToString("X2") + " SET 4, A";
                    }
                case 0xE8:
                    {
                        return opcode2.ToString("X2") + " SET 5, B";
                    }
                case 0xE9:
                    {
                        return opcode2.ToString("X2") + " SET 5, C";
                    }
                case 0xEA:
                    {
                        return opcode2.ToString("X2") + " SET 5, D";
                    }
                case 0xEB:
                    {
                        return opcode2.ToString("X2") + " SET 5, E";
                    }
                case 0xEC:
                    {
                        return opcode2.ToString("X2") + " SET 5, H";
                    }
                case 0xED:
                    {
                        return opcode2.ToString("X2") + " SET 5, L";
                    }
                case 0xEE:
                    {
                        return opcode2.ToString("X2") + " SET 5, (HL)";
                    }
                case 0xEF:
                    {
                        return opcode2.ToString("X2") + " SET 5, A";
                    }
                case 0xF0:
                    {
                        return opcode2.ToString("X2") + " SET 6, B";
                    }
                case 0xF1:
                    {
                        return opcode2.ToString("X2") + " SET 6, C";
                    }
                case 0xF2:
                    {
                        return opcode2.ToString("X2") + " SET 6, D";
                    }
                case 0xF3:
                    {
                        return opcode2.ToString("X2") + " SET 6, E";
                    }
                case 0xF4:
                    {
                        return opcode2.ToString("X2") + " SET 6, H";
                    }
                case 0xF5:
                    {
                        return opcode2.ToString("X2") + " SET 6, L";
                    }
                case 0xF6:
                    {
                        return opcode2.ToString("X2") + " SET 6, (HL)";
                    }
                case 0xF7:
                    {
                        return opcode2.ToString("X2") + " SET 6, A";
                    }
                case 0xF8:
                    {
                        return opcode2.ToString("X2") + " SET 7, B";
                    }
                case 0xF9:
                    {
                        return opcode2.ToString("X2") + " SET 7, C";
                    }
                case 0xFA:
                    {
                        return opcode2.ToString("X2") + " SET 7, D";
                    }
                case 0xFB:
                    {
                        return opcode2.ToString("X2") + " SET 7, E";
                    }
                case 0xFC:
                    {
                        return opcode2.ToString("X2") + " SET 7, H";
                    }
                case 0xFD:
                    {
                        return opcode2.ToString("X2") + " SET 7, L";
                    }
                case 0xFE:
                    {
                        return opcode2.ToString("X2") + " SET 7, (HL)";
                    }
                case 0xFF:
                    {
                        return opcode2.ToString("X2") + " SET 7, A";
                    }
                default:
                    {
                        return opcode2.ToString("X2") + " UNKNOWN";
                    }
            }
        }
        #endregion

        #region decode opcode 10
        private string Decode_Opcode_10(byte opcode2)
        {
            switch (opcode2)
            {
                case 0x00:
                    {
                        return opcode2.ToString("X2") + " STOP";
                    }
                default:
                    {
                        return opcode2.ToString("X2") + " UNKNOWN";
                    }
            }
        }
        #endregion

        #region Memory Type Selector
        private void cmbMemoryType_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cmbMemoryType.Text)
            {
                case "Tile":
                    {
                        lblMemory.Text = cmbMemoryType.Text + " Memory";
                        updateTileMem();
                        return;
                    }
                case "Video":
                    {
                        lblMemory.Text = cmbMemoryType.Text + " Memory";
                        updateVidMem();
                        return;
                    }
                case "Ram Bank":
                    {
                        lblMemory.Text = cmbMemoryType.Text + " Memory";
                        updateRamBankMem();
                        return;
                    }
                case "Internal Ram":
                    {
                        lblMemory.Text = cmbMemoryType.Text + " Memory";
                        updateInternalRamMem();
                        return;
                    }
                case "System":
                    {
                        lblMemory.Text = cmbMemoryType.Text + " Memory";
                        updateSystemMem();
                        return;
                    }
            }
        }
        #endregion

        #region Update Memory
        private void UpdateMemory()
        {
            switch (cmbMemoryType.Text)
            {
                case "Tile":
                    {
                        updateTileMem();
                        return;
                    }
                case "Video":
                    {
                        updateVidMem();
                        return;
                    }
                case "Ram Bank":
                    {
                        updateRamBankMem();
                        return;
                    }
                case "Internal Ram":
                    {
                        updateInternalRamMem();
                        return;
                    }
                case "System":
                    {
                        updateSystemMem();
                        return;
                    }
            }
        }
        #endregion
    }
}
