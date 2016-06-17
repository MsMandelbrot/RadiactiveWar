using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace _3D_Game
{
   
    public static class Global
    {
        public static GraphicsDevice Graphics;

        public static int ScreenWidth { get { return Graphics.Viewport.Width; } }
        public static int ScreenHeight { get { return Graphics.Viewport.Height; } }
    }
}
