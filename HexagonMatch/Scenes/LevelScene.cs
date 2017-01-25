using System;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;
using HexagonMatch.GUI;

namespace HexagonMatch.Scenes
{
    enum LevelType { Score = 0, Color }


    class LevelScene : Scene
    {
        LevelType type;
        Grid grid;
        byte maxValue = 5;
        GUIManager gui;
        Random rand;
        Texture2D fonTexture;
        SpriteFont scoreFont;
        long score = 0;
        Rectangle gridArea, locationArea;
        Location location;
        Enemy enemy;
        LevelManager levelManager;
        
        private void RestartGrid()
        {
            score = 0;
            foreach (Hexagon h in grid.HexMap)
            {
                if (h != null && h.Content.Type == HexagonType.Element)
                {                   
                    h.Content.Element = (HexagonElement)(rand.Next(maxValue));
                }
            }
        }
        private void LoadContentForLevel()
        {
            Grid.Elements = game.Content.Load<Texture2D>("stones");
            grid.HexagonTexture = game.Content.Load<Texture2D>("png_hexagon");
            grid.WallTexture = game.Content.Load<Texture2D>("wall");       
            scoreFont = game.Content.Load<SpriteFont>("score_font");
            fonTexture = game.Content.Load<Texture2D>("background_fhd");
            Texture2D forestTx = game.Content.Load<Texture2D>("forest4_background");
            Texture2D goblinTx = game.Content.Load<Texture2D>("goblin");
            Texture2D backBtnTexture = game.Content.Load<Texture2D>("restart_ico");
            //
            location = new Location("Forest", forestTx);
            enemy = new Enemy("Goblin", goblinTx, 30);
            //
            //GUI Initialization
            Button restartBtn = new Button(new Vector2(game.CurrentScreenSize.X - backBtnTexture.Height, 10), backBtnTexture);
            restartBtn.Click += () =>
            {
                RestartGrid();
            };
            //LevelManagerLayout lvlInfoLayout = new LevelManagerLayout()
            gui = new GUIManager(game, spriteBatch, game.Scale);
            gui.MainLayout.AddWidget(restartBtn);
            //game.Components.Add(gui);
        }

        public LevelScene(MainGame game, SpriteBatch spriteBatch) : base(game, spriteBatch)
        {
            rand = new Random();            
            Initialize();
            LoadContentForLevel();
        }

        internal LevelType Type
        {
            get
            {
                return type;
            }
            set
            {
                type = value;
            }
        }

        public override void Initialize()
        {
            //Initialize grid
            gridArea = new Rectangle(
                game.CurrentScreenSize.X / 10,
                game.CurrentScreenSize.Y / 100 * 40,
                game.CurrentScreenSize.X - (game.CurrentScreenSize.X / 10) * 2,
                game.CurrentScreenSize.Y - ((game.CurrentScreenSize.Y / 100 * 40) + (game.CurrentScreenSize.Y / 10))
                );
            locationArea = new Rectangle(0, 0, game.CurrentScreenSize.X, game.CurrentScreenSize.Y);
            Vector2 center = new Vector2(gridArea.Width / 2.0f + gridArea.X, gridArea.Height / 2.0f + gridArea.Y);
            int mapRadius = 3;
            short hexSize = (short)((gridArea.Width / (2 * (mapRadius + 1) + mapRadius)));
            grid = new Grid(hexSize, center, maxValue, game.Scale, mapRadius);
            Hexagon.OwnerGrid = grid;
            for (int q = -mapRadius; q <= mapRadius; q++)
            {
                int r1 = MathHelper.Max(-mapRadius, -q - mapRadius);
                int r2 = MathHelper.Min(mapRadius, -q + mapRadius);
                for (int r = r1; r <= r2; r++)
                {
                    grid.AddHexagon(new Hexagon(new Hex(q, r, -q - r), new HexagonContent(HexagonType.Element, (HexagonElement)rand.Next(maxValue))));
                }
            }
            //
            grid.GetHex(new Hex(-2, 1, 1)).Content = HexagonContent.Block;
            grid.GetHex(new Hex(2, -1, -1)).Content.Frozen = true;
            grid.GetHex(new Hex(0, 0, 0)).addWall(1);
            //

            grid.CopyMapToBuffer();
            grid.NormalizeStart += Grid_NormalizeStart;

            levelManager = new LevelManager(game, grid, new LevelInfo(10, new LevelConditionInfo(HexagonElement.Brown, 10)));
            base.Initialize();
        }

        private void Grid_NormalizeStart(Grid grid)
        {
            score += grid.SelectedHex.Count * 5 * (grid.SelectedHex.Count - 2);
            enemy.Hp -= grid.SelectedHex.Count;
            if (enemy.Hp == 0)
            {
                enemy.MaxHp *= 2;
                enemy.Hp = enemy.MaxHp; 
            }
        }

        public override void Update(GameTime gameTime)
        {
            TouchCollection touch = TouchPanel.GetState();
            grid.Update(gameTime, touch);
            gui.Update(gameTime);
            base.Update(gameTime);
        }

        private void DebugDraw(GameTime gameTime)
        {
            Vector2 scorePosition = new Vector2(game.CurrentScreenSize.X - scoreFont.MeasureString(gameTime.ElapsedGameTime.TotalSeconds.ToString()).X * game.Scale.X - 10, 5);
            spriteBatch.Begin();
            spriteBatch.DrawString(scoreFont, gameTime.ElapsedGameTime.TotalSeconds.ToString(), scorePosition, Color.White, 0, Vector2.Zero, game.Scale.X, SpriteEffects.None, 0);
            spriteBatch.End();
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(fonTexture, Vector2.Zero, color: Color.White, scale: game.Scale);//Draw background 
            //Draw score           
            //string scoreStr = "Score: " + score.ToString();
            //Vector2 scorePosition = new Vector2(game.CurrentScreenSize.X - scoreFont.MeasureString(scoreStr).X * game.Scale.X - 10, 5);
            //spriteBatch.DrawString(scoreFont, scoreStr, scorePosition, Color.White, 0, Vector2.Zero, game.Scale.X, SpriteEffects.None, 0);   
            //
            //Draw location
            spriteBatch.Draw(location.Texture, locationArea, Color.White);
            //Draw enemy
            //Vector2 enemyPos = locationArea.Center.ToVector2() - (new Vector2(enemy.Texture.Width / 2, enemy.Texture.Height / 2) * game.Scale);
            Vector2 enemyPos = new Rectangle(0, 0, game.CurrentScreenSize.X, game.CurrentScreenSize.X / 16 * 9).Center.ToVector2() - (new Vector2(enemy.Texture.Width / 2, enemy.Texture.Height / 2) * game.Scale);
            spriteBatch.Draw(enemy.Texture, position: enemyPos, scale: game.Scale, color: Color.White);
            //Draw enemy hp
            string enemyHpStr = enemy.Hp.ToString() + " / " + enemy.MaxHp;
            Vector2 size = game.BaseFont.MeasureString(enemyHpStr) * game.Scale;
            //Vector2 enemyHpPos = new Vector2(locationArea.X + (locationArea.Width / 2 - size.X / 2), locationArea.Y + locationArea.Height - size.Y);
            Vector2 enemyHpPos = new Vector2(enemyPos.X + (enemy.Texture.Width * game.Scale.X / 2 - size.X / 2), enemyPos.Y + enemy.Texture.Height * game.Scale.Y + size.Y);
            spriteBatch.DrawString(game.BaseFont, enemyHpStr, enemyHpPos, Color.Maroon, 0, Vector2.Zero, game.Scale, SpriteEffects.None, 0);
            spriteBatch.End();

            grid.Draw(spriteBatch, game.BaseFont);//Draw hexagon grid
            //DebugDraw(gameTime);
            gui.Draw(gameTime);
            base.Draw(gameTime);
        }

        public override void Close()
        {
            base.Close();
        }
    }
}