using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RRClon.GUI
{
    class GUILayer
    {
        protected List<Widget> elements;
        protected SpriteBatch spriteBatch;
        protected Rectangle area;
        protected bool enable = true;
        
        public GUILayer(Game game, SpriteBatch spriteBatch, Rectangle area, int capacity = 10)
        {
            elements = new List<Widget>(capacity);
            this.spriteBatch = spriteBatch;
            this.area = area;
        }

        public bool IsEnable
        {
            get
            {
                return enable;
            }
        }
        public Vector2 Position
        {
            get { return new Vector2(area.X, area.Y); }
        }

        public void AddWidget(Widget widget)
        {
            widget.OwnerLayer = this;
            elements.Add(widget);
        }
        public void Enable()
        {
            enable = true;
        }
        public void Disable()
        {
            enable = false;
        }

        public virtual void Update(GameTime gameTime, MouseState mState)
        {
            if (enable && area.Contains(mState.Position))
            {
                foreach (Widget w in elements)
                    w.Update(gameTime, mState);
            }
        }

        public virtual void Draw()
        {
            if (enable)
            {
                foreach (Widget w in elements)
                    w.Draw(spriteBatch);
            }
        }
    }
}
