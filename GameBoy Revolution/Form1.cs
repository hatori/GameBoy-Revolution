//#define SPEED
using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Ionic.Zip;

namespace GameBoy_Revolution
{
    public partial class Form1 : Form
    {
        #region Variables
        private Video video;
        private Memory memory;
        //private Sound sound;
        private CPU cpu;
        //private Input input;
        private Timer time;
        private Debugger debugger;
        private bool paused;
        private ZipFile resources;
        private bool critical_failure = false;
        private Settings settings;

#if SPEED
        //DateTime time_now;
        //DateTime time_elasped;
        //TimeSpan time_span;
        [DllImport("kernel32.dll")]
        extern static short QueryPerformanceCounter(ref long x);
        [DllImport("kernel32.dll")]
        extern static short QueryPerformanceFrequency(ref long x);

        long ctr1 = 0, ctr2 = 0, freq = 0;
#endif
        #endregion

        #region constructor
        public Form1()
        {
            try
            {
                Assembly.Load("SlimDX, Version=4.0.11.43, Culture=neutral, PublicKeyToken=b1b0c32fd1ffe4f9");
            }
            catch
            {
                critical_failure = true;
                MessageBox.Show("An attempt to load SlimDX has failed, please download and install SlimDX to use\nthis emulator. If error persists after install, please contact author.", "Error!", MessageBoxButtons.OK);
            }

            if (!File.Exists("Ionic.Zip.dll"))
            {
                MessageBox.Show("Ionic.Zip.dll is missing, GameBoy Revolution can not continue to load properly and will now crash. \n\nPlease replace Ionic.Zip.dll before further use.", "Warning!", MessageBoxButtons.OK);
            }

            InitializeComponent();
            startUp();
        }
        #endregion

        #region startup
        private void startUp()
        {
            /*if (File.Exists("Space Invaders Resources.zip"))
            {
                resources = new ZipFile("Space Invaders Resources.zip");
            }*/
            application_title("GameBoy Revolution");
            application_resize(300, 26);
            application_menubar();
            //application_wallpaper("invaders.jpg");
            application_misc();
            //settings = new Settings();
        }
        #endregion

        #region misc
        public void application_misc()
        {
            try
            {
                this.StartPosition = FormStartPosition.CenterScreen;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region resize
        public void application_resize(int size_x, int size_y)
        {
            try
            {
                this.ClientSize = new Size(size_x, size_y);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region title
        public void application_title(string title)
        {
            try
            {
                this.Text = title;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region wallpaper
        public void application_wallpaper(string file_name)
        {
            try
            {
                FileInfo image_info = new FileInfo(file_name);
                Bitmap wallpaper = new Bitmap(image_info.FullName);
                PictureBox image = new PictureBox();
                int width, height;

                if (wallpaper.Width >= this.ClientSize.Width)
                {
                    width = wallpaper.Width;
                }
                else
                {
                    width = this.ClientSize.Width;
                }

                if (wallpaper.Height >= this.ClientSize.Height)
                {
                    height = wallpaper.Height;
                }
                else
                {
                    height = this.ClientSize.Width;
                }

                image.Location = new Point(0, (this.ClientSize.Height));
                image.ClientSize = new Size(width, height);
                image.Image = wallpaper;

                application_resize(width, height + this.ClientSize.Height);

                this.Controls.Add(image);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region menubar
        void application_menubar()
        {
            string[] file = new string[] { "Open Rom", "Close" };
            string[] cpu = new string[] { "Start CPU", "Pause CPU", "Resume CPU", "Restart CPU", "Stop CPU" };
            string[] debug = new string[] { "Debugger" };
            //string[] config = new string[] { "Settings", "Color Settings", "Key Bindings" };

            MenuStrip menu = new MenuStrip();
            menu.Name = "App_Menu";
            application_menubar_item(menu, "File", file);
            application_menubar_item(menu, "CPU", cpu);
            application_menubar_item(menu, "Debug", debug);
            //application_menubar_item(menu, "Config", config);
            this.Controls.Add(menu);
        }
        #endregion

        #region menubar item
        void application_menubar_item(MenuStrip menu, string text, string[] subitems)
        {
            ToolStripMenuItem item = new ToolStripMenuItem(text);
            for (int i = 0; i < subitems.Length; i++)
            {
                application_menubar_item_subitem(item, subitems[i]);
            }
            menu.Items.Add(item);
        }
        #endregion

        #region menubar subitem
        void application_menubar_item_subitem(ToolStripMenuItem item, string text)
        {
            ToolStripMenuItem subitem = new ToolStripMenuItem(text);
            subitem.Name = "subitem_" + text.Replace(' ', '_');
            subitem.Click += new EventHandler(subitem_Click);

            item.DropDownItems.Add(subitem);
        }
        #endregion

        #region menu click event
        void subitem_Click(object sender, EventArgs e)
        {
            try
            {
                call_function(((ToolStripMenuItem)sender).Name);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region menu call function
        void call_function(string func_name)
        {
            try
            {
                Type type = this.GetType();
                MethodInfo mi = type.GetMethod(func_name);
                mi.Invoke(this, null);
            }
            catch
            {
                MessageBox.Show("Function: " + func_name + " could not be located, please contact author for assistance.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region timer event
        void time_Tick(object sender, EventArgs e)
        {
#if SPEED
            //time_now = DateTime.Now;
            if (QueryPerformanceCounter(ref ctr1) != 0)	// Begin timing.
            {
#endif
            cpu.execute();
            //video.Renderer();

#if SPEED
                //time_elasped = DateTime.Now;
                //time_span = time_elasped - time_now;
                //application_title("Space Invaders Revolution " + time_span.TotalMilliseconds + " ms to render frame");
                QueryPerformanceCounter(ref ctr2);
                QueryPerformanceFrequency(ref freq);
                application_title("Space Invaders Revolution " + ((ctr2 - ctr1) * (1.0 / freq)) + " s to render frame");
            }
            else
            {
                application_title("Space Invaders Revolution");
            }
#endif
        }
        #endregion

        #region form closing
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Critical_failure == false)
            {
                /*if (video != null)
                {
                    video.uninit_directx();
                }

                if (sound != null)
                {
                    sound.uninit_directsound();
                }

                if (input != null)
                {
                    input.Uninitialize_Keyboard();
                }*/
            }
        }
        #endregion

        #region Window State
        public FormWindowState Window_state
        {
            get
            {
                return this.WindowState;
            }

            set
            {
                this.WindowState = value;
            }
        }
        #endregion

        #region retrieve taskbar visibility
        public bool Taskbar_visibility
        {
            get
            {
                return this.ShowInTaskbar;
            }

            set
            {
                this.ShowInTaskbar = value;
            }
        }
        #endregion

        #region retrieve screen visibility
        public bool Screen_visibility
        {
            get
            {
                return this.Visible;
            }

            set
            {
                this.Visible = value;
            }
        }
        #endregion

        #region retrieve resources
        public ZipFile retrieve_resources
        {
            get
            {
                return this.resources;
            }

            set
            {
                this.resources = value;
            }
        }
        #endregion

        #region retrieve critical status
        public bool Critical_failure
        {
            get
            {
                return critical_failure;
            }
        }
        #endregion

        #region retrieve settings
        public Settings main_settings
        {
            get
            {
                return settings;
            }
            set
            {
                settings = value;
            }
        }
        #endregion

        /*#region retrieve input
        public Input main_input
        {
            get
            {
                return input;
            }
            set
            {
                input = value;
            }
        }
        #endregion*/

        #region paused state
        public bool Paused_state
        {
            get
            {
                return paused;
            }

            set
            {
                paused = value;
            }
        }
        #endregion

        #region close
        public void subitem_Close()
        {
            try
            {
                this.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region open rom main
        public void subitem_Open_Rom()
        {
            if (memory == null)
            {
                memory = new Memory();
            }

            memory.open_rom();
        }
        #endregion

        #region start cpu
        public void subitem_Start_CPU()
        {
            if (Critical_failure == false)
            {
                try
                {
                    if (video != null && time != null)
                    {
                        if (video.Fatal_error)
                        {
                            MessageBox.Show("The Space Invaders encountered an error loading, \nthis option is not available right now.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            return;
                        }

                        if (time.Enabled || Paused_state == true)
                        {
                            MessageBox.Show("CPU is either currently running or is currently paused, \nthis operation can not continue.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            return;
                        }
                    }

                    Window_state = FormWindowState.Minimized;
                    Taskbar_visibility = false;
                    Screen_visibility = false;

                    if (video == null)
                    {
                        video = new Video(this);
                        if (memory == null)
                        {
                            memory = new Memory();
                        }
                        //input = new Input(this, video.Directx_Window.Handle);
                        //sound = new Sound(this, video);
                        cpu = new CPU(this, ref memory, ref video/*, ref sound, ref input*/);
                    }
                    else
                    {
                        video.reinit_directx();
                        //sound.reinit_directsound();
                        //input.Reinitialize_Keyboard(video.Directx_Window.Handle);
                        if (cpu == null)
                        {
                            cpu = new CPU(this, ref memory, ref video/*, ref sound, ref input*/);
                        }
                    }

                    if (time == null)
                    {
                        time = new Timer();
                        time.Interval = 15;
                        time.Tick += new EventHandler(time_Tick);
                    }

                    time.Start();
                    video.Directx_Window.Activate();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("A critical error has been detected in the emulator, This menu has been disabled. Please contact author for assistance.", "Error!", MessageBoxButtons.OK);
            }
        }
        #endregion

        #region stop cpu
        public void subitem_Stop_CPU()
        {
            if (Critical_failure == false)
            {
                try
                {
                    if (video.Fatal_error)
                    {
                        MessageBox.Show("Gameboy Revolution encountered an error loading, \nthis option is not available right now.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }

                    if (!time.Enabled && paused == false)
                    {
                        MessageBox.Show("CPU can not be stopped if it has not been started yet, \nthis operation can not continue.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    else
                    {
                        if (time.Enabled)
                        {
                            time.Stop();
                        }

                        if (Paused_state == true)
                        {
                            Paused_state = false;
                        }

                        video.uninit_directx();
                        //sound.uninit_directsound();
                        //input.Uninitialize_Keyboard();

                        //if (cpu != null)
                        //{
                            //cpu = null;
                        //}
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("A critical error has been detected in the emulator, This menu has been disabled. Please contact author for assistance.", "Error!", MessageBoxButtons.OK);
            }
        }
        #endregion

        #region pause cpu
        public void subitem_Pause_CPU()
        {
            if (Critical_failure == false)
            {
                try
                {
                    if (video.Fatal_error)
                    {
                        MessageBox.Show("Gameboy Revolution encountered an error loading, \nthis option is not available right now.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }

                    if (time.Enabled && Paused_state == false)
                    {
                        time.Stop();
                        Paused_state = true;
                        this.Activate();
                    }
                    else
                    {
                        MessageBox.Show("CPU is either currently not running or is currently paused, \nthis operation can not continue.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("A critical error has been detected in the emulator, This menu has been disabled. Please contact author for assistance.", "Error!", MessageBoxButtons.OK);
            }
        }
        #endregion

        #region resume cpu
        public void subitem_Resume_CPU()
        {
            if (Critical_failure == false)
            {
                try
                {
                    if (video.Fatal_error)
                    {
                        MessageBox.Show("Gameboy Revolution encountered an error loading, \nthis option is not available right now.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }

                    if (!time.Enabled && paused == true)
                    {
                        Window_state = FormWindowState.Minimized;
                        Taskbar_visibility = false;
                        Screen_visibility = false;
                        video.Window_state = FormWindowState.Normal;
                        video.Taskbar_visibility = true;
                        video.Screen_visibility = true;
                        video.reinit_directx();
                        //sound.reinit_directsound();
                        //input.Reinitialize_Keyboard(video.Directx_Window.Handle);
                        time.Start();
                        video.Directx_Window.Activate();
                        paused = false;
                    }
                    else
                    {
                        MessageBox.Show("CPU is either currently running or has not been started yet, \nthis operation can not continue.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("A critical error has been detected in the emulator, This menu has been disabled. Please contact author for assistance.", "Error!", MessageBoxButtons.OK);
            }
        }
        #endregion

        #region restart cpu
        public void subitem_Restart_CPU()
        {
            if (Critical_failure == false)
            {
                try
                {
                    if (video.Fatal_error)
                    {
                        MessageBox.Show("Gameboy Revolution encountered an error loading, \nthis option is not available right now.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }

                    if (!time.Enabled && paused == false)
                    {
                        MessageBox.Show("CPU can not be restarted if it has not been started yet, \nthis operation can not continue.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    else
                    {
                        if (time.Enabled)
                        {
                            time.Stop();
                        }

                        if (paused == true)
                        {
                            paused = false;
                        }

                        Window_state = FormWindowState.Minimized;
                        Taskbar_visibility = false;
                        Screen_visibility = false;

                        video.reinit_directx();
                        //sound.reinit_directsound();
                        //input.Reinitialize_Keyboard(video.Directx_Window.Handle);

                        //if (cpu != null)
                        //{
                            //cpu = null;
                        //}

                        //cpu = new Cpu(this, ref memory, ref video, ref sound, ref input);
                        //memory.reinit_memory();

                        video.clear_screen();
                        video.Renderer();

                        time.Start();
                        video.Directx_Window.Activate();
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("A critical error has been detected in the emulator, This menu has been disabled. Please contact author for assistance.", "Error!", MessageBoxButtons.OK);
            }
        }
        #endregion

        #region settings
        public void subitem_Settings()
        {
            try
            {
                if (time != null)
                {
                    if ((time.Enabled) || (paused))
                    {
                        MessageBox.Show("The cpu is either paused or running, \nall settings are read only.", "Error!", MessageBoxButtons.OK);
                    }
                }

                //if (form2 == null)
                //{
                    //form2 = new Form2(this, "settings");
                    //form2.Show();
                //}
                //else
                //{
                    //if (form2.IsDisposed == true)
                    //{
                        //form2 = new Form2(this, "settings");
                        //form2.Show();
                    //}
                    //else
                    //{
                        //MessageBox.Show("Please close the current settings menu before opening another.", "Warning", MessageBoxButtons.OK);
                    //}
                //}
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region key bindings
        public void subitem_Key_Bindings()
        {
            try
            {
                if (time != null)
                {
                    if ((time.Enabled) || (paused))
                    {
                        MessageBox.Show("The cpu is either paused or running, \nall settings are read only.", "Error!", MessageBoxButtons.OK);
                    }
                }

                //if (form2 == null)
                //{
                    //form2 = new Form2(this, "key_bindings");
                    //form2.Show();
                //}
                //else
                //{
                    //if (form2.IsDisposed == true)
                    //{
                        //form2 = new Form2(this, "key_bindings");
                        //form2.Show();
                    //}
                    //else
                    //{
                        //MessageBox.Show("Please close the current settings menu before opening another.", "Warning", MessageBoxButtons.OK);
                    //}
                //}
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region color settings
        public void subitem_Color_Settings()
        {
            try
            {
                if (time != null)
                {
                    if ((time.Enabled) || (paused))
                    {
                        MessageBox.Show("The cpu is either paused or running, \nall settings are read only.", "Error!", MessageBoxButtons.OK);
                    }
                }

                //if (form2 == null)
                //{
                    //form2 = new Form2(this, "color");
                    //form2.Show();
                //}
                //else
                //{
                    //if (form2.IsDisposed == true)
                    //{
                        //form2 = new Form2(this, "color");
                        //form2.Show();
                    //}
                    //else
                    //{
                        //MessageBox.Show("Please close the current settings menu before opening another.", "Warning", MessageBoxButtons.OK);
                    //}
                //}
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region debugger
        public void subitem_Debugger()
        {
            try
            {
                if (video == null)
                {
                    video = new Video(this);
                    if (memory == null)
                    {
                        memory = new Memory();
                    }
                    //input = new Input(this, video.Directx_Window.Handle);
                    //sound = new Sound(this, video);
                    cpu = new CPU(this, ref memory, ref video/*, ref sound, ref input*/);
                }
                else
                {
                    video.reinit_directx();
                    //sound.reinit_directsound();
                    //input.Reinitialize_Keyboard(video.Directx_Window.Handle);
                    if (cpu == null)
                    {
                        cpu = new CPU(this, ref memory, ref video/*, ref sound, ref input*/);
                    }
                }

                if (debugger == null)
                {
                    debugger = new Debugger(cpu);
                    debugger.Show();
                }
                else
                {
                    if (debugger.IsDisposed == true)
                    {
                        debugger = new Debugger(cpu);
                        debugger.Show();
                    }
                    else
                    {
                        MessageBox.Show("Please close the current debugger menu before opening another.", "Warning", MessageBoxButtons.OK);
                    }
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
