using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;

namespace HexagonMatch.GUI
{
    class Layout
    {
        protected LinkedList<Widget> widgets;
        protected LinkedList<Layout> layouts;
        private Rectangle area;
        protected Layout parent;
        protected Layout important = null;// show under all layout and stops their updates
        protected bool closable = false;// important layout will be close if touch ouf of it

        internal Layout Parent
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
        public Rectangle Area
        {
            get
            {
                return area;
            }

            set
            {
                area = value;
            }
        }

        public Layout(Rectangle area, Layout parent = null)
        {
            this.area = area;
            this.parent = parent;
            widgets = new LinkedList<Widget>();
            layouts = new LinkedList<Layout>();
        }

        public void AddWidget(Widget widget)
        {
            widgets.AddLast(widget);
            widget.Parent = this;
            //widget.TouchArea = new Rectangle(area.Location + widget.TouchArea.Location, widget.TouchArea.Size);
        }
        public void DeleteWidget(Widget widget)
        {
            widgets.Remove(widget);
            widget.Parent = null;
        }
        public void AddLayout(Layout layout)
        {
            layouts.AddFirst(layout);
            layout.Parent = this;
            layout.Area.Offset(area.Location);
        }
        public void DeleteLayout(Layout layout)
        {
            if (important == layout)
            {
                important = null;
                layout.Parent = null;
            }
            else
            {
                layouts.Remove(layout);
                layout.Parent = null;
            }
        }
        public void ShowLayout(Layout layout, bool closable = false)
        {
            important = layout;
            important.Parent = this;
            this.closable = closable;
        }
        public void CloseImportant()
        {
            important.Parent = null;
            important = null;
            closable = false;
        }

        public virtual void Update(GameTime gameTime, TouchCollection state)
        {
            if (important == null)
            {
                var curr = layouts.First;
                while (curr != null)
                {
                    curr.Value.Update(gameTime, state);
                    curr = curr.Next;
                }
                foreach (Widget w in widgets)
                    w.Update(gameTime, state);
            }
            else
            {
                if (state.Count == 0 || important.Area.Contains(state[0].Position))
                {
                    important.Update(gameTime, state);
                }
                else if (closable)
                {
                    CloseImportant();
                }
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            var curr = layouts.Last;
            while (curr != null)
            {
                curr.Value.Draw(spriteBatch);
                curr = curr.Previous;
            }
            foreach (Widget w in widgets)
                w.Draw(spriteBatch);
            if (important != null)
                important.Draw(spriteBatch);
        }

    }
}