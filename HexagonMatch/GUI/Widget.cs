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
    class Widget
    {
        protected Rectangle touchArea;
        protected Vector2 position;
        private Layout parent;

        public Widget(Rectangle touchArea, Vector2 position, Widget parent = null)
        {
            this.touchArea = touchArea;
            this.position = position;
        }

        public virtual void Update(GameTime gameTime, TouchCollection touchs) { }
        public virtual void Draw(SpriteBatch spriteBatch, Vector2 scale) { }

        public Rectangle TouchArea
        {
            get
            {
                return touchArea;
            }

            set
            {
                touchArea = value;
            }
        }
        public Layout Parent
        {
            get
            {
                return parent;
            }

            set
            {
                parent = value;
            }
        }
    }
}
