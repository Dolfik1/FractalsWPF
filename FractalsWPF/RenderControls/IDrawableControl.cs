using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace FractalsWPF.RenderControls
{
    public interface IDrawableControl
    {
        void Initialize(GraphicsDevice graphicsDevice, IServiceProvider serviceProvider);
        void Draw(GraphicsDevice graphicsDevice);
    }
}
