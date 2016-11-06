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
        List<Widget> widgets;
        SpriteBatch spriteBatch;
        Vector2 scale;

        public GUIManager(Game game, SpriteBatch spriteBatch, Vector2 scale ,int capacity = 10) : base(game)
        {
            widgets = new List<Widget>(capacity);
            this.spriteBatch = spriteBatch;
            this.scale = scale;
        }

        public void AddWidget(Widget widget)
        {
            widgets.Add(widget);
        }

        public override void Update(GameTime gameTime)
        {
            TouchCollection state = TouchPanel.GetState();   
            foreach (Widget w in widgets)
                w.Update(gameTime, state);
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            foreach (Widget w in widgets)
                w.Draw(spriteBatch, scale);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
