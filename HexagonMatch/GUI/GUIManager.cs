using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;

namespace HexagonMatch.GUI
{
    class GUIManager
    {
        Layout mainLayout;
        SpriteBatch spriteBatch;
        //Vector2 scale;

        internal Layout MainLayout
        {
            get
            {
                return mainLayout;
            }

            set
            {
                mainLayout = value;
            }
        }

        public GUIManager(SpriteBatch spriteBatch)
        {
            this.spriteBatch = spriteBatch;
            mainLayout = new Layout(new Rectangle(0, 0, MainGame.NormalWidth, MainGame.NormalHeight));
        }

        public void Update(GameTime gameTime, TouchCollection state)
        {
            mainLayout.Update(gameTime, state);
        }

        public void Draw(GameTime gameTime)
        {
            mainLayout.Draw(spriteBatch);
        }
    }
}
