using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using FractalsWPF.RenderControls;

namespace FractalsWPF
{
    public interface IMonoGameTarget
    {
        void LoadContent();
        void Update(float deltaTime);
        void Draw();
    }

    public class DrawManager : IDrawableControl
    {
        public DrawManager(IMonoGameTarget target)
        {
            _target = target;
        }

        private IMonoGameTarget _target;

        public GraphicsDevice GraphicsDevice { get; private set; }
        public ContentManager ContentManager { get; private set; }

        private Stopwatch _stopwatch;

        public void Initialize(GraphicsDevice graphicsDevice, IServiceProvider serviceProvider)
        {
            GraphicsDevice = graphicsDevice;
            ContentManager = new ContentManager(serviceProvider, @"Content");
            _target.LoadContent();

            _stopwatch = Stopwatch.StartNew();
        }

        public void Draw(GraphicsDevice graphicsDevice)
        {
            var deltaTime = (float)_stopwatch.Elapsed.TotalSeconds;

            //if (HasViewportSizeChanged())
            //    ResetCamera(ViewportWidth, ViewportHeight);

            _target.Update(deltaTime);
            _target.Draw();

            _stopwatch.Restart();
        }

        private Point _previousViewportSize = new Point();
        private bool HasViewportSizeChanged()
        {
            var width = GraphicsDevice.Viewport.Width;
            var height = GraphicsDevice.Viewport.Height;
            var hasChanged = width != _previousViewportSize.X || height != _previousViewportSize.Y;

            if (hasChanged)
            {
                _previousViewportSize = new Point(width, height);
                return true;
            }

            return false;
        }
    }
}
