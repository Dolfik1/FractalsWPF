using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Xna.Framework.Graphics;
using OpenTK;
using Microsoft.Xna.Framework;
using OpenTK.Graphics.OpenGL;
using FractalsWPF.RenderControls;

namespace FractalsWPF
{
    public class GraphicsDeviceControl : GLControl
    {
        public GraphicsDeviceControl()
        {
            _designMode = DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime;
        }
        
        public GraphicsDevice GraphicsDevice
        {
            get { return _deviceService.GraphicsDevice; }
        }

        private GraphicsDeviceService _deviceService;
        public GraphicsDeviceService GraphicsDeviceService
        {
            get { return _deviceService; }
        }

        private ServiceContainer _serviceContainer = new ServiceContainer();
        public ServiceContainer Services
        {
            get { return _serviceContainer; }
        }

        private IDrawableControl _drawControl;
        private Timer _timer = new Timer();

        public void Initialise(IDrawableControl drawControl)
        {
            if (!DesignMode)
            {
                _deviceService = GraphicsDeviceService.AddRef(Handle, ClientSize.Width, ClientSize.Height);
                _serviceContainer.AddService<IGraphicsDeviceService>(_deviceService);
                _drawControl = drawControl;

                if (_drawControl != null)
                    _drawControl.Initialize(GraphicsDevice, _serviceContainer);

                _timer.Interval = 4;
                _timer.Tick += new EventHandler(Timer_Tick);
                _timer.Start();

                // Hook the idle event to constantly redraw our animation.
                Application.Idle += delegate { Invalidate(); };
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {            
            Invalidate();
        }
        
        protected override void Dispose (bool disposing)
        {
            if (_deviceService != null)
            {
                try 
                {
                    _deviceService.Release();
                }
                catch
                {
                }

                _deviceService = null;
            }

            base.Dispose(disposing);
        }

        private bool _designMode;
        protected new bool DesignMode
        {
            get 
            { 
                return _designMode; 
            }
        }

        protected override void OnPaint (PaintEventArgs e)
        {
            string beginDrawError = BeginDraw();

            if (string.IsNullOrEmpty(beginDrawError))
            {
                if (_drawControl != null)
                    _drawControl.Draw(GraphicsDevice);

                EndDraw();
            }
            else
            {
                PaintUsingSystemDrawing(e.Graphics, beginDrawError);
            }
        }

        private string BeginDraw ()
        {
            if (_deviceService == null) 
            {
                return string.Format("{0}\n\n{1}", Text, GetType());
            }

            string deviceResetError = HandleDeviceReset();

            if (!string.IsNullOrEmpty(deviceResetError)) 
            {
                return deviceResetError;
            }

            var control = GLControl.FromHandle(_deviceService.GraphicsDevice.PresentationParameters.DeviceWindowHandle) as GLControl;
            if (control != null) 
            {
                control.Context.MakeCurrent(WindowInfo);
                _deviceService.GraphicsDevice.PresentationParameters.BackBufferHeight = ClientSize.Height;
                _deviceService.GraphicsDevice.PresentationParameters.BackBufferWidth = ClientSize.Width;
            }

            var viewport = new Viewport();

            viewport.X = 0;
            viewport.Y = 0;

            viewport.Width = ClientSize.Width;
            viewport.Height = ClientSize.Height;

            viewport.MinDepth = 0;
            viewport.MaxDepth = 1;

            if (GraphicsDevice.Viewport.Equals(viewport) == false)
                GraphicsDevice.Viewport = viewport;

            return null;
        }
        
        private void EndDraw ()
        {
            try 
            {
                SwapBuffers();
            }
            catch 
            {
            }
        }

        private string HandleDeviceReset ()
        {
            bool needsReset = false;

            switch (GraphicsDevice.GraphicsDeviceStatus) 
            {
                case GraphicsDeviceStatus.Lost:
                    return "Graphics device lost";

                case GraphicsDeviceStatus.NotReset:
                    needsReset = true;
                    break;

                default:
                    PresentationParameters pp = GraphicsDevice.PresentationParameters;
                    needsReset = (ClientSize.Width > pp.BackBufferWidth) || (ClientSize.Height > pp.BackBufferHeight);
                    break;
            }

            if (needsReset) 
            {
                try
                {
                    _deviceService.ResetDevice(ClientSize.Width, ClientSize.Height);
                }
                catch (Exception e) 
                {
                    return "Graphics device reset failed\n\n" + e;
                }
            }

            return null;
        }

        protected virtual void PaintUsingSystemDrawing (Graphics graphics, string text)
        {
            graphics.Clear(System.Drawing.Color.Black);

            using (Brush brush = new SolidBrush(System.Drawing.Color.White)) 
            {
                using (StringFormat format = new StringFormat())
                {
                    format.Alignment = StringAlignment.Center;
                    format.LineAlignment = StringAlignment.Center;

                    graphics.DrawString(text, Font, brush, ClientRectangle, format);
                }
            }
        }

        protected override void OnPaintBackground (PaintEventArgs pevent)
        {
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // GraphicsDeviceControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Name = "GraphicsDeviceControl";
            this.ResumeLayout(false);

        }
    }
}