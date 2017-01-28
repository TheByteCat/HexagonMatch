using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using HexagonMatch.Scenes;

namespace HexagonMatch
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class MainGame : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;        
        SpriteFont baseFont;        
        Point currentScreenSize;
        public const int NormalWidth = 1080, NormalHeight = 1920;
        SceneManager sceneManager;

        public Point CurrentScreenSize
        {
            get
            {
                return new Point(NormalWidth, NormalHeight);
            }
        }
        public SpriteFont BaseFont
        {
            get
            {
                return baseFont;
            }
        }
        public Scene CurrentScene { get { return SceneManager.CurrentScene; } }

        internal SceneManager SceneManager
        {
            get
            {
                return sceneManager;
            }

            private set
            {
                sceneManager = value;
            }
        }

        public MainGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            var metric = new Android.Util.DisplayMetrics();
            Activity.WindowManager.DefaultDisplay.GetMetrics(metric);
            graphics.IsFullScreen = true;
            
            currentScreenSize = metric.WidthPixels < metric.HeightPixels ? new Point(metric.WidthPixels, metric.HeightPixels) : new Point(metric.HeightPixels, metric.WidthPixels);
            graphics.PreferredBackBufferWidth = currentScreenSize.X;
            graphics.PreferredBackBufferHeight = currentScreenSize.Y;
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
            baseFont = Content.Load<SpriteFont>("baseFont");

            SceneManager = new SceneManager(this, spriteBatch, new Point(NormalWidth, NormalHeight), SceneTitle.Level);
            FPSCounterComponent fps = new FPSCounterComponent(this, spriteBatch, baseFont);
            Components.Add(SceneManager);
            Components.Add(fps);                      
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
            base.Draw(gameTime);
        }
    }
}
