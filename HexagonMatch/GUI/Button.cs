﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;

namespace HexagonMatch.GUI
{
    class Button : Widget
    {
        string text;
        Texture2D texture;
        bool pressed = false;
        SpriteFont font;       
        bool enable = true;

        public Texture2D Texture
        {
            get
            {
                return texture;
            }

            set
            {
                texture = value;
            }
        }

        public delegate void clickDelegate();
        public event clickDelegate Click;

        public void On()
        {
            enable = true;
        }

        public void Off()
        {
            enable = false;
        }

        public Button(Vector2 position, Texture2D texture = null, string text = "", SpriteFont font = null) : base(new Rectangle((int)position.X, (int)position.Y, texture.Bounds.Width, texture.Bounds.Height), position)
        {
            this.texture = texture;
            this.text = text;
            this.font = font;
        }

        public override void Update(GameTime gameTime, TouchCollection state)
        {            
            if (enable)
            {
                if (state.Count == 1 )
                {                    
                    pressed = touchArea.Contains(state[0].Position);
                }
                if (state.Count == 0)
                {
                    if (pressed && Click != null)
                        Click();
                    pressed = false;                  
                }
            }
            base.Update(gameTime, state);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (enable)
            {
                spriteBatch.Draw(texture, position + Parent.Area.Location.ToVector2(), color : Color.White);
                if (text != "")
                {
                    Vector2 size = font.MeasureString(text);
                    spriteBatch.DrawString(font, text, new Vector2((int)(touchArea.X + touchArea.Width / 2 - size.X / 2), (int)(touchArea.Y + touchArea.Height / 2 - size.Y / 2)), Color.White);
                }
            }
            base.Draw(spriteBatch);
        }
    }
}
