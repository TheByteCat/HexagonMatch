using System.Collections.Generic;
using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using HexagonMatch.GUI;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;

namespace HexagonMatch
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;        
        SpriteFont baseFont, scoreFont;        
        Point currentScreenSize;
        byte maxValue = 5;
        Grid grid;
        GUIManager gui;
        Texture2D fonTexture;//background texture
        Random rand;
        const int NormalWidth = 1080, NormalHeight = 1980;
        Vector2 scale;
        public static Color[] colors = { Color.LightSkyBlue, Color.SeaGreen, Color.White, Color.MediumTurquoise };

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            var metric = new Android.Util.DisplayMetrics();
            Activity.WindowManager.DefaultDisplay.GetMetrics(metric);
            graphics.IsFullScreen = true;
            graphics.PreferredBackBufferWidth = metric.WidthPixels;
            graphics.PreferredBackBufferHeight = metric.HeightPixels;
            currentScreenSize = metric.WidthPixels < metric.HeightPixels ? new Point(metric.WidthPixels, metric.HeightPixels) : new Point(metric.HeightPixels, metric.WidthPixels);
            scale =new Vector2((float)currentScreenSize.X / NormalWidth, (float)currentScreenSize.Y / NormalHeight);
            //graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;
            graphics.SupportedOrientations = DisplayOrientation.Portrait | DisplayOrientation.PortraitDown;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            rand = new Random();
            //Color[] colors = {Color.Blue, Color.SeaGreen, Color.White, Color.MediumTurquoise};
            //Initialize grid
            Vector2 center = currentScreenSize.ToVector2() / 2.0f;            
            int mapRadius = 3;
            short hexSize = (short)((currentScreenSize.X / (2 * (mapRadius + 1) + mapRadius))) ;
            grid = new Grid(hexSize, center, maxValue, scale, mapRadius);
            Hexagon.OwnerGrid = grid;            
            for (int q = -mapRadius; q <= mapRadius; q++)
            {
                int r1 = MathHelper.Max(-mapRadius, -q - mapRadius);
                int r2 = MathHelper.Min(mapRadius, -q + mapRadius);
                for (int r = r1; r <= r2; r++)
                {
                    grid.AddHexagon(new Hexagon(new Hex(q, r, -q - r), (byte)(rand.Next(maxValue) + 1), grid.HexagonSize, colors[rand.Next(colors.Length)]));
                }
            }
                                                            
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            Hexagon.Texture = Content.Load<Texture2D>("white_hexagon");
            baseFont = Content.Load<SpriteFont>("baseFont");
            scoreFont = Content.Load<SpriteFont>("score_font");
            fonTexture = Content.Load<Texture2D>("background_fhd");
            Texture2D backBtnTexture = Content.Load<Texture2D>("restart_ico");
            //GUI Initialization
            Button restartBtn = new Button(new Vector2(5, 10), backBtnTexture);
            restartBtn.Click += () =>
            {
                grid.Score = 0;
                foreach (Hexagon h in grid.HexMap)
                {                    
                    if (h != null)
                    {
                        h.Value = (byte)(rand.Next(maxValue) + 1);
                        h.BaseColor = colors[rand.Next(colors.Length)];
                    }
                }
            };
            gui = new GUIManager(this, spriteBatch, scale);
            gui.AddWidget(restartBtn);
            Components.Add(gui);            
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }        

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                Exit();

            // TODO: Add your update logic here            
            TouchCollection touch = TouchPanel.GetState();
            grid.Update(gameTime, touch);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            
            // TODO: Add your drawing code here
            spriteBatch.Begin();            
            spriteBatch.Draw(fonTexture, Vector2.Zero, color: Color.White, scale:scale);//Draw background            
            string scoreStr = "Score: " + grid.Score.ToString();
            Vector2 scorePosition = new Vector2(currentScreenSize.X - scoreFont.MeasureString(scoreStr).X * scale.X - 10, 5);            
            spriteBatch.DrawString(scoreFont, scoreStr, scorePosition, Color.White, 0, Vector2.Zero, scale.X, SpriteEffects.None, 0);//Draw score            
            spriteBatch.End();
            base.Draw(gameTime);
            grid.Draw(spriteBatch, baseFont);//Draw hexagon grid
        }
    }
}
