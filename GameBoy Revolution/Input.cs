using System;
using SlimDX;
using SlimDX.DirectInput;

namespace GameBoy_Revolution
{
    public class Input
    {
        #region variables
        private System.Windows.Forms.Form _form1_reference;

        private DirectInput dinput;
        private Keyboard keyb;
        private KeyboardState key_pressed;
        private Key[] keys;
        private bool critical_failure;
        private Key[] key_list;
        #endregion

        #region Constructor
        public Input(System.Windows.Forms.Form form1_reference, IntPtr window_handle)
        {
            _form1_reference = form1_reference;
            critical_failure = false;

            populate_key_list();
            Initialize_Keyboard(window_handle);
        }
        #endregion

        #region initialize keyboard
        private void Initialize_Keyboard(IntPtr window_handle)
        {
            try
            {
                dinput = new DirectInput();
                keyb = new Keyboard(dinput);
                keyb.SetCooperativeLevel(window_handle, CooperativeLevel.Background | CooperativeLevel.Nonexclusive);
                keyb.Acquire();
                setup_keys();
            }
            catch
            {
                System.Windows.Forms.MessageBox.Show("A failure has been detected during DirectInput initialization, please contact the author for assistance.", "Error!", System.Windows.Forms.MessageBoxButtons.OK);
                critical_failure = true;
            }
        }
        #endregion

        #region uninitialize keyboard
        public void Uninitialize_Keyboard()
        {
            if (critical_failure == false)
            {
                if (dinput != null)
                {
                    dinput.Dispose();
                    dinput = null;
                }

                if (keyb != null)
                {
                    keyb.Dispose();
                    keyb = null;
                }
            }
        }
        #endregion

        #region reinitialize keyboard
        public void Reinitialize_Keyboard(IntPtr window_handle)
        {
            if (critical_failure == false)
            {
                try
                {
                    Uninitialize_Keyboard();
                    dinput = new DirectInput();
                    keyb = new Keyboard(dinput);
                    keyb.SetCooperativeLevel(window_handle, CooperativeLevel.Background | CooperativeLevel.Nonexclusive);
                    keyb.Acquire();
                    setup_keys();
                }
                catch (Exception e)
                {
                    System.Windows.Forms.MessageBox.Show(e.ToString(), "Error!", System.Windows.Forms.MessageBoxButtons.OK);
                    return;
                }
            }
        }
        #endregion

        #region populate key list
        private void populate_key_list()
        {
            key_list = new Key[145];

            key_list[0] = Key.A;
            key_list[1] = Key.AbntC1;
            key_list[2] = Key.AbntC2;
            key_list[3] = Key.Apostrophe;
            key_list[4] = Key.Applications;
            key_list[5] = Key.AT;
            key_list[6] = Key.AX;
            key_list[7] = Key.B;
            key_list[8] = Key.Backslash;
            key_list[9] = Key.Backspace;
            key_list[10] = Key.C;
            key_list[11] = Key.Calculator;
            key_list[12] = Key.CapsLock;
            key_list[13] = Key.Colon;
            key_list[14] = Key.Comma;
            key_list[15] = Key.Convert;
            key_list[16] = Key.D;
            key_list[17] = Key.D0;
            key_list[18] = Key.D1;
            key_list[19] = Key.D2;
            key_list[20] = Key.D3;
            key_list[21] = Key.D4;
            key_list[22] = Key.D5;
            key_list[23] = Key.D6;
            key_list[24] = Key.D7;
            key_list[25] = Key.D8;
            key_list[26] = Key.D9;
            key_list[27] = Key.Delete;
            key_list[28] = Key.DownArrow;
            key_list[29] = Key.E;
            key_list[30] = Key.End;
            key_list[31] = Key.Equals;
            key_list[32] = Key.Escape;
            key_list[33] = Key.F;
            key_list[34] = Key.F1;
            key_list[35] = Key.F10;
            key_list[36] = Key.F11;
            key_list[37] = Key.F12;
            key_list[38] = Key.F13;
            key_list[39] = Key.F14;
            key_list[40] = Key.F15;
            key_list[41] = Key.F2;
            key_list[42] = Key.F3;
            key_list[43] = Key.F4;
            key_list[44] = Key.F5;
            key_list[45] = Key.F6;
            key_list[46] = Key.F7;
            key_list[47] = Key.F8;
            key_list[48] = Key.F9;
            key_list[49] = Key.G;
            key_list[50] = Key.Grave;
            key_list[51] = Key.H;
            key_list[52] = Key.Home;
            key_list[53] = Key.I;
            key_list[54] = Key.Insert;
            key_list[55] = Key.J;
            key_list[56] = Key.K;
            key_list[57] = Key.Kana;
            key_list[58] = Key.Kanji;
            key_list[59] = Key.L;
            key_list[60] = Key.LeftAlt;
            key_list[61] = Key.LeftArrow;
            key_list[62] = Key.LeftBracket;
            key_list[63] = Key.LeftControl;
            key_list[64] = Key.LeftShift;
            key_list[65] = Key.LeftWindowsKey;
            key_list[66] = Key.M;
            key_list[67] = Key.Mail;
            key_list[68] = Key.MediaSelect;
            key_list[69] = Key.MediaStop;
            key_list[70] = Key.Minus;
            key_list[71] = Key.Mute;
            key_list[72] = Key.MyComputer;
            key_list[73] = Key.N;
            key_list[74] = Key.NextTrack;
            key_list[75] = Key.NoConvert;
            key_list[76] = Key.NumberLock;
            key_list[77] = Key.NumberPad0;
            key_list[78] = Key.NumberPad1;
            key_list[79] = Key.NumberPad2;
            key_list[80] = Key.NumberPad3;
            key_list[81] = Key.NumberPad4;
            key_list[82] = Key.NumberPad5;
            key_list[83] = Key.NumberPad6;
            key_list[84] = Key.NumberPad7;
            key_list[85] = Key.NumberPad8;
            key_list[86] = Key.NumberPad9;
            key_list[87] = Key.NumberPadComma;
            key_list[88] = Key.NumberPadEnter;
            key_list[89] = Key.NumberPadEquals;
            key_list[90] = Key.NumberPadMinus;
            key_list[91] = Key.NumberPadPeriod;
            key_list[92] = Key.NumberPadPlus;
            key_list[93] = Key.NumberPadSlash;
            key_list[94] = Key.NumberPadStar;
            key_list[95] = Key.O;
            key_list[96] = Key.Oem102;
            key_list[97] = Key.P;
            key_list[98] = Key.PageDown;
            key_list[99] = Key.PageUp;
            key_list[100] = Key.Pause;
            key_list[101] = Key.Period;
            key_list[102] = Key.PlayPause;
            key_list[103] = Key.Power;
            key_list[104] = Key.PreviousTrack;
            key_list[105] = Key.PrintScreen;
            key_list[106] = Key.Q;
            key_list[107] = Key.R;
            key_list[108] = Key.Return;
            key_list[109] = Key.RightAlt;
            key_list[110] = Key.RightArrow;
            key_list[111] = Key.RightBracket;
            key_list[112] = Key.RightControl;
            key_list[113] = Key.RightShift;
            key_list[114] = Key.RightWindowsKey;
            key_list[115] = Key.S;
            key_list[116] = Key.ScrollLock;
            key_list[117] = Key.Semicolon;
            key_list[118] = Key.Slash;
            key_list[119] = Key.Sleep;
            key_list[120] = Key.Space;
            key_list[121] = Key.Stop;
            key_list[122] = Key.T;
            key_list[123] = Key.Tab;
            key_list[124] = Key.U;
            key_list[125] = Key.Underline;
            key_list[126] = Key.Unknown;
            key_list[127] = Key.Unlabeled;
            key_list[128] = Key.UpArrow;
            key_list[129] = Key.V;
            key_list[130] = Key.VolumeDown;
            key_list[131] = Key.VolumeUp;
            key_list[132] = Key.W;
            key_list[133] = Key.Wake;
            key_list[134] = Key.WebBack;
            key_list[135] = Key.WebFavorites;
            key_list[136] = Key.WebForward;
            key_list[137] = Key.WebHome;
            key_list[138] = Key.WebRefresh;
            key_list[139] = Key.WebSearch;
            key_list[140] = Key.WebStop;
            key_list[141] = Key.X;
            key_list[142] = Key.Y;
            key_list[143] = Key.Yen;
            key_list[144] = Key.Z;
        }
        #endregion

        #region retrieve key type
        private Key retrieve_key_type(string key)
        {
            for (int i = 0; i < key_list.Length; i++)
            {
                if (key_list[i].ToString() == key)
                {
                    return key_list[i];
                }
            }

            return Key.A;
        }
        #endregion

        #region setup keys
        private void setup_keys()
        {
            keys = new Key[8];

            try
            {
                keys[0] = retrieve_key_type(((Form1)_form1_reference).main_settings.read_config_setting("Input", "key0"));
                keys[1] = retrieve_key_type(((Form1)_form1_reference).main_settings.read_config_setting("Input", "key1")); ;
                keys[2] = retrieve_key_type(((Form1)_form1_reference).main_settings.read_config_setting("Input", "key2")); ;
                keys[3] = retrieve_key_type(((Form1)_form1_reference).main_settings.read_config_setting("Input", "key3")); ;
                keys[4] = retrieve_key_type(((Form1)_form1_reference).main_settings.read_config_setting("Input", "key4")); ;
                keys[5] = retrieve_key_type(((Form1)_form1_reference).main_settings.read_config_setting("Input", "key5")); ;
                keys[6] = retrieve_key_type(((Form1)_form1_reference).main_settings.read_config_setting("Input", "key6")); ;
                keys[7] = retrieve_key_type(((Form1)_form1_reference).main_settings.read_config_setting("Input", "key7")); ;
            }
            catch
            {
                keys[0] = Key.RightArrow;
                keys[1] = Key.LeftArrow;
                keys[2] = Key.UpArrow;
                keys[3] = Key.DownArrow;
                keys[4] = Key.J;
                keys[5] = Key.K;
                keys[6] = Key.G;
                keys[7] = Key.H;
            }
        }
        #endregion

        #region retrieve keyboardstate
        public KeyboardState retrieve_keyboardstate()
        {
            return keyb.GetCurrentState();
        }
        #endregion

        #region retrieve keys
        public Key[] Keys
        {
            get
            {
                return keys;
            }
        }
        #endregion

        #region keyboardstate
        public KeyboardState Key_Pressed
        {
            get
            {
                return key_pressed;
            }

            set
            {
                key_pressed = value;
            }
        }
        #endregion
    }
}
