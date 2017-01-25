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
    class GUIManager : DrawableGameComponent
    {
        Layout mainLayout;
        SpriteBatch spriteBatch;
        Vector2 scale;

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

        public GUIManager(MainGame game, SpriteBatch spriteBatch, Vector2 scale) : base(game)
        {
            this.spriteBatch = spriteBatch;
            this.scale = scale;
            mainLayout = new Layout(new Rectangle(Point.Zero, game.CurrentScreenSize));
        }

        public override void Update(GameTime gameTime)
        {
            TouchCollection state = TouchPanel.GetState();
            mainLayout.Update(gameTime, state);
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            mainLayout.Draw(spriteBatch, scale);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
