using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using HexagonMatch.GUI;

namespace HexagonMatch
{
    class LevelManagerLayout : Layout
    {
        private Texture2D backgraund;
        private LevelManager levelManager;
        private SpriteFont font;

        public LevelManagerLayout(Rectangle area, Texture2D backgraund, LevelManager levelManager, SpriteFont font, Layout parent = null) : base(area, parent)
        {
            this.backgraund = backgraund;
            this.levelManager = levelManager;
            this.font = font;
        }        

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(backgraund, Area , Color.White);
            for (int i = 0; i < levelManager.Conditions.Count; i++)
            {
                LevelCondition c = levelManager.Conditions[i];
                string s = c.Progress.ToString() + "/" + c.Info.RequiredAmount.ToString() + " " + (c.Complete ? "+" : "");
                Vector2 size = font.MeasureString(s);
                spriteBatch.Draw(
                    Grid.Elements,
                    destinationRectangle: new Rectangle(new Point(10, (int)(size.Y + 5) * i) + Area.Location, new Point((int)size.Y)),
                    sourceRectangle: Grid.ElementSource(c.Info.Element), 
                    color: Color.White);
                spriteBatch.DrawString(font, s, new Vector2(10 + size.Y, (int)(size.Y + 5) * i) + Area.Location.ToVector2(),  Color.Black);
            }
            Vector2 stepSize = font.MeasureString(levelManager.Steps.ToString());
            Vector2 stepsPos = new Vector2(Area.Width / 2 - stepSize.X, Area.Height - stepSize.Y);
            spriteBatch.DrawString(font, levelManager.Steps.ToString(), Area.Location.ToVector2() + stepsPos, Color.Black);
            base.Draw(spriteBatch);
        }
    }
}