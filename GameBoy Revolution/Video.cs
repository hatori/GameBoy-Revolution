using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using SlimDX;
using SlimDX.Direct3D9;

namespace GameBoy_Revolution
{
    public class Video
    {
        #region Variables
        private Form form1_reference;

        private Form screen;
        private CancelEventHandler closing;
        private bool fatal_error;
        private bool close_canceled;
        private bool deviceLost;
        private bool critical_failure;

        private Device device;
        private PresentParameters pp;
        private Direct3D d3d;
        private Texture texture;
        private VertexBuffer vb;
        private Result result;
        private CustomVertex.TransformedTextured[] customvertex;
        private DataStream vertex_data;
        private DataRectangle texture_data;
        private uint[] data_copy;
        private int[] colors;
        private int height;
        private int width;
        #endregion

        #region vertex class
        public class CustomVertex
        {
            [StructLayout(LayoutKind.Sequential)]
            public struct TransformedTextured
            {
                public Vector4 Position;
                public Vector2 TextureCoordinates;
                public static VertexFormat Format
                {
                    get { return VertexFormat.PositionRhw | VertexFormat.Texture1; }
                }
                public TransformedTextured(Vector4 pos, Vector2 texpos)
                {
                    Position = pos;
                    TextureCoordinates = texpos;
                }
            }
        }
        #endregion

        #region constructor
        public Video(Form _form1_reference)
        {
            form1_reference = _form1_reference;
            critical_failure = false;

            Load_Window_Size();

            if (!initialize_directx())
            {
                MessageBox.Show("problem with directx initialization");
                fatal_error = true;
                return;
            }

            try
            {
                texture = new Texture(device, 256, 256, 1, Usage.Dynamic, Format.A8R8G8B8, Pool.Default);
                data_copy = new uint[256 * 256];
            }
            catch (Direct3D9Exception e)
            {
                MessageBox.Show("Gameboy Revolution failed to create rendering surface. \nPlease report this error. \n\nDirectX Error:\n" + e.ToString(), "Error!", MessageBoxButtons.OK);
                fatal_error = true;
                return;
            }

            setup_screen();
            setup_colors();
        }
        #endregion

        #region load window size
        private void Load_Window_Size()
        {
            //string size = ((Form1)form1_reference).main_settings.read_config_setting("Game", "Window_Resolution");
            string size = "160 x 144";

            string[] sizes = size.Split(new string[] { " x " }, StringSplitOptions.RemoveEmptyEntries);
            width = int.Parse(sizes[0]);
            height = int.Parse(sizes[1]);
        }
        #endregion

        #region initialize directx
        private bool initialize_directx()
        {
            d3d = new Direct3D();
            if (pp == null)
            {
                pp = new PresentParameters();
                pp.SwapEffect = SwapEffect.Discard;
                pp.Windowed = true;
                pp.PresentFlags = PresentFlags.LockableBackBuffer;
                pp.PresentationInterval = PresentInterval.Immediate;
                pp.BackBufferCount = 1;
                pp.BackBufferHeight = height;
                pp.BackBufferWidth = width;
                pp.BackBufferFormat = Format.A8R8G8B8;
            }

            initialize_directx_window();

            if (screen.IsHandleCreated == true)
            {
                try
                {
                    device = new Device(d3d, 0, DeviceType.Hardware, screen.Handle, CreateFlags.HardwareVertexProcessing, pp);
                    return true;
                }
                catch
                {
                    try
                    {
                        device = new Device(d3d, 0, DeviceType.Hardware, screen.Handle, CreateFlags.SoftwareVertexProcessing, pp);
                    }
                    catch
                    {
                        try
                        {
                            device = new Device(d3d, 0, DeviceType.Reference, screen.Handle, CreateFlags.SoftwareVertexProcessing, pp);
                        }
                        catch (Direct3D9Exception e)
                        {
                            MessageBox.Show(e.ToString(), "Error!", MessageBoxButtons.OK);
                            critical_failure = true;
                            return false;
                        }
                        return true;
                    }
                    return true;
                }
            }
            else
            {
                MessageBox.Show("Directx window could not be drawn", "Error!", MessageBoxButtons.OK);
                return false;
            }
        }
        #endregion

        #region initialize_directx_window
        private void initialize_directx_window()
        {
            screen = new Form();
            screen.MinimizeBox = false;
            screen.MaximizeBox = false;
            screen.Text = form1_reference.Text;
            screen.Name = "Directx Screen";
            screen.ClientSize = new System.Drawing.Size(width, height);
            //screen.WindowState = FormWindowState.Minimized;
            //screen.ShowInTaskbar = false;
            //this.Screen_visibility = false;
            screen.FormBorderStyle = FormBorderStyle.FixedSingle;
            screen.StartPosition = FormStartPosition.CenterScreen;
            screen.FormClosing += new FormClosingEventHandler(screen_FormClosing);
            this.closing += new CancelEventHandler(screen_FormClosing);
            screen.KeyDown += new KeyEventHandler(screen_KeyDown);
            Configuration.AddResultWatch(ResultCode.DeviceLost, ResultWatchFlags.AlwaysIgnore);
            Configuration.AddResultWatch(ResultCode.DeviceNotReset, ResultWatchFlags.AlwaysIgnore);
            screen.Show();
        }
        #endregion

        #region uninit_directx
        public void uninit_directx()
        {
            if (critical_failure == false)
            {
                if (texture != null)
                {
                    texture.Dispose();
                    texture = null;
                }

                if (vb != null)
                {
                    vb.Dispose();
                    vb = null;
                }

                if (d3d != null)
                {
                    d3d.Dispose();
                    d3d = null;
                }

                if (device != null)
                {
                    device.Dispose();
                    device = null;
                }
            }
        }
        #endregion

        #region reinit_directx
        public void reinit_directx()
        {
            if (critical_failure == false)
            {
                if (texture != null)
                {
                    texture.Dispose();
                    texture = null;
                }

                if (vb != null)
                {
                    vb.Dispose();
                    vb = null;
                }

                if (d3d != null)
                {
                    d3d.Dispose();
                    d3d = null;
                }

                if (device != null)
                {
                    device.Dispose();
                    device = null;
                }

                if (screen != null)
                {
                    screen.Dispose();
                    screen = null;
                }

                Load_Window_Size();

                if (!initialize_directx())
                {
                    MessageBox.Show("problem with directx initialization");
                    fatal_error = true;
                    return;
                }

                try
                {
                    texture = new Texture(device, 256, 256, 0, Usage.None, Format.A8B8G8R8, Pool.Managed);
                }
                catch (Direct3D9Exception e)
                {
                    MessageBox.Show("Gameboy Revolution failed to create rendering surface. \nPlease report this error. \n\nDirectX Error:\n" + e.ToString(), "Error!", MessageBoxButtons.OK);
                    fatal_error = true;
                    return;
                }

                setup_screen();
            }
        }
        #endregion

        #region screen keydown event
        void screen_KeyDown(object sender, KeyEventArgs e)
        {
            if (((Form)sender).Name == screen.Name)
            {
                if (e.KeyCode == Keys.Escape)
                {
                    Window_state = FormWindowState.Minimized;
                    Taskbar_visibility = false;
                    Screen_visibility = false;
                    ((Form1)(form1_reference)).Window_state = FormWindowState.Normal;
                    ((Form1)(form1_reference)).Taskbar_visibility = true;
                    ((Form1)(form1_reference)).Screen_visibility = true;
                    ((Form1)(form1_reference)).subitem_Pause_CPU();
                }
            }
        }
        #endregion

        #region screen closing event
        private void screen_FormClosing(object sender, CancelEventArgs e)
        {
            if (e.Cancel != true && ((Form)sender).Name == screen.Name)
            {
                e.Cancel = true;
            }
        }
        #endregion

        #region setup screen
        private void setup_screen()
        {
            customvertex = new CustomVertex.TransformedTextured[6];
            vb = new VertexBuffer(device, Marshal.SizeOf(typeof(CustomVertex.TransformedTextured)) * 6, Usage.WriteOnly, CustomVertex.TransformedTextured.Format, Pool.Default);

            customvertex[0] = new CustomVertex.TransformedTextured(new Vector4(0.0f, 0.0f, 0.0f, 1.0f), new Vector2(0.0f, 0.0f));
            customvertex[1] = new CustomVertex.TransformedTextured(new Vector4(160.0f, 0.0f, 0.0f, 1.0f), new Vector2(0.625f, 0.0f));
            customvertex[2] = new CustomVertex.TransformedTextured(new Vector4(0.0f, 144.0f, 0.0f, 1.0f), new Vector2(0.0f, 0.5625f));
            customvertex[3] = new CustomVertex.TransformedTextured(new Vector4(160.0f, 0.0f, 0.0f, 1.0f), new Vector2(0.625f, 0.0f));
            customvertex[4] = new CustomVertex.TransformedTextured(new Vector4(160.0f, 144.0f, 0.0f, 1.0f), new Vector2(0.625f, 0.5625f));
            customvertex[5] = new CustomVertex.TransformedTextured(new Vector4(0.0f, 144.0f, 0.0f, 1.0f), new Vector2(0.0f, 0.5625f));

            vertex_data = vb.Lock(0, Marshal.SizeOf(typeof(CustomVertex.TransformedTextured)) * 6, LockFlags.Discard);
            vertex_data.WriteRange(customvertex);
            vb.Unlock();
        }
        #endregion

        #region setup colors
        private void setup_colors()
        {
            colors = new int[4];

            try
            {
                colors[0] = int.Parse(((Form1)form1_reference).main_settings.read_config_setting("Video", "color0"));
                colors[1] = int.Parse(((Form1)form1_reference).main_settings.read_config_setting("Video", "color1"));
                colors[2] = int.Parse(((Form1)form1_reference).main_settings.read_config_setting("Video", "color2"));
                colors[3] = int.Parse(((Form1)form1_reference).main_settings.read_config_setting("Video", "color3"));
            }
            catch
            {
                colors[0] = Color.White.ToArgb();
                colors[1] = Color.LightGray.ToArgb();
                colors[2] = Color.DarkGray.ToArgb();
                colors[3] = Color.Black.ToArgb();
            }
        }
        #endregion

        #region check device
        void check_device()
        {
            result = device.TestCooperativeLevel();

            if (result == ResultCode.DeviceLost)
            {
                deviceLost = true;
            }
            else if (result == ResultCode.DeviceNotReset)
            {
                texture.Dispose();
                texture = null;
                vb.Dispose();
                vb = null;
                device.Reset(pp);
                deviceLost = false;
                texture = new Texture(device, 256, 256, 1, Usage.Dynamic, Format.A8R8G8B8, Pool.Default);
            }
            else
            {
                deviceLost = false;
            }
        }
        #endregion

        #region set pixel
        public void set_pixel(int x, int y, uint color)
        {
            data_copy[(y * 256) + x] = color;
        }
        #endregion

        #region clear screen
        public void clear_screen()
        {
            for (int y = 0; y < 256; y++)
            {
                for (int x = 0; x < 256; )
                {
                    data_copy[(y << 8) | x] = 0x00000000;
                    ++x;
                }
            }

            texture_data = texture.LockRectangle(0, LockFlags.Discard | LockFlags.DoNotWait);
            texture_data.Data.WriteRange<uint>(data_copy);
            texture.UnlockRectangle(0);
        }
        #endregion

        #region renderer
        public void Renderer()
        {
            check_device();

            if (deviceLost == true)
            {
                return;
            }

            device.Clear(ClearFlags.Target, Color.CornflowerBlue, 1.0f, 0);
            device.BeginScene();

            device.SetTexture(0, texture);
            device.VertexFormat = CustomVertex.TransformedTextured.Format;
            device.SetStreamSource(0, vb, 0, Marshal.SizeOf(typeof(CustomVertex.TransformedTextured)));
            device.DrawPrimitives(PrimitiveType.TriangleList, 0, 2);
            device.SetTexture(0, null);

            device.EndScene();
            try
            {
                device.Present();
            }
            catch
            {
                MessageBox.Show("An error has been thrown by the directx renderer");
            }
        }
        #endregion

        #region retrieve event status
        public bool Closing_event
        {
            get
            {
                return close_canceled;
            }

            set
            {
                close_canceled = value;
            }
        }
        #endregion

        #region retrieve video status
        public bool Fatal_error
        {
            get
            {
                return fatal_error;
            }

            set
            {
                fatal_error = value;
            }
        }
        #endregion

        #region retrieve screen status
        public FormWindowState Window_state
        {
            get
            {
                return screen.WindowState;
            }

            set
            {
                screen.WindowState = value;
            }
        }
        #endregion

        #region retrieve taskbar visibility
        public bool Taskbar_visibility
        {
            get
            {
                return screen.ShowInTaskbar;
            }

            set
            {
                screen.ShowInTaskbar = value;
            }
        }
        #endregion

        #region retrieve screen visibility
        public bool Screen_visibility
        {
            get
            {
                return screen.Visible;
            }

            set
            {
                screen.Visible = value;
            }
        }
        #endregion

        #region retrieve screen handle
        public IntPtr Screen_Handle
        {
            get
            {
                return screen.Handle;
            }
        }
        #endregion

        #region retrieve colors
        public int[] Colors
        {
            get
            {
                return colors;
            }
        }
        #endregion

        #region directx window
        public Form Directx_Window
        {
            get
            {
                return screen;
            }
        }
        #endregion

        #region lock texture
        public void lock_texture()
        {
            texture_data = texture.LockRectangle(0, LockFlags.Discard | LockFlags.DoNotWait);
        }
        #endregion

        #region unlock texture
        public void unlock_texture()
        {
            texture.UnlockRectangle(0);
        }
        #endregion
    }
}
