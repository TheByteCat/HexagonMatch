﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HexagonMatch.Scenes
{
    public class Scene
    {
        protected SpriteBatch spriteBatch;
        protected MainGame game;

        public Screen Screen { get; set; }
        public InputState Input { get; set; }

        public Scene(MainGame game, SpriteBatch spriteBatch)
        {
            this.spriteBatch = spriteBatch;
            this.game = game;
        }
        public virtual void Initialize() { }
        public virtual void Update(GameTime gameTime) { }
        public virtual void Draw(GameTime gameTime) { }
        public virtual void Close() { }
    }
}
