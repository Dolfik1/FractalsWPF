using System;
using System.Threading;
using Microsoft.Xna.Framework.Graphics;

namespace FractalsWPF.RenderControls
{
    public class GraphicsDeviceService : IGraphicsDeviceService
    {
        private static readonly GraphicsDeviceService _instance = new GraphicsDeviceService();
        private static int _refCount;

        private GraphicsDevice _device;

        public GraphicsDevice GraphicsDevice
        {
            get 
            { 
                return _device; 
            }
        }

        public event EventHandler<EventArgs> DeviceCreated;
        public event EventHandler<EventArgs> DeviceDisposing;
        public event EventHandler<EventArgs> DeviceReset = (s, e) => { };
        public event EventHandler<EventArgs> DeviceResetting = (s, e) => { };

        protected GraphicsDeviceService ()
        {
        }

        public static GraphicsDeviceService AddRef (IntPtr windowHandle, int width, int height)
        {
            if (Interlocked.Increment(ref _refCount) == 1) 
            {
                _instance.CreateDevice(windowHandle, width, height);
            }

            return _instance;
        }

        public void Release ()
        {
            Release(true);
        }

        protected void Release (bool disposing)
        {
            if (Interlocked.Decrement(ref _refCount) == 0) 
            {
                if (disposing) 
                {
                    if (DeviceDisposing != null) 
                    {
                        DeviceDisposing(this, EventArgs.Empty);
                    }

                    _device.Dispose();
                }

                _device = null;
            }
        }

        protected void CreateDevice (IntPtr windowHandle, int width, int height)
        {
            var adapter = GraphicsAdapter.DefaultAdapter;

            _device = new GraphicsDevice(adapter, GraphicsProfile.Reach, new PresentationParameters());
            _device.PresentationParameters.DeviceWindowHandle = windowHandle;
            _device.PresentationParameters.BackBufferWidth = Math.Max(width, 1);
            _device.PresentationParameters.BackBufferHeight = Math.Max(height, 1);

            if (DeviceCreated != null) 
            {
                DeviceCreated(this, EventArgs.Empty);
            }
        }

        public void ResetDevice (int width, int height)
        { 
        }
    }
}