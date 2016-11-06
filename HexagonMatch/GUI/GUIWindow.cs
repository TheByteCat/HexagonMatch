using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RRClon.GUI
{   

    class GUIWindow : GUILayer
    {
        Texture2D background;

        public GUIWindow(Game game, SpriteBatch spriteBatch, Rectangle area, Texture2D background, int capacity = 10) : base(game, spriteBatch, area, capacity)
        {
            this.background = background;
        }

        public override void Draw()
        {
            spriteBatch.Draw(background, area, Color.White);
            base.Draw();
        }
    }
}
