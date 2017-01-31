using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HexagonMatch.Scenes
{
    enum SceneTitle
    {
        Menu = 0, LevelSelectMenu, Level
    }

    class SceneManager : DrawableGameComponent
    {
        Scene currentScene;
        SceneTitle sceneTitle;
        Scene[] scenes;

        internal Scene CurrentScene
        {
            get
            {
                return currentScene;
            }

            set
            {
                currentScene = value;
            }
        }

        public Screen Screen { get; private set;}

        public InputState Input { get; private set; }

        public delegate void sceneChangeDelegate(SceneTitle prevScene, SceneTitle newScene);
        public event sceneChangeDelegate SceneChanged;

        public SceneManager(MainGame game, SpriteBatch spriteBatch, Point virtualScreenSize, SceneTitle startScene) : base(game)
        {
            Screen = new Screen(game, virtualScreenSize.X, virtualScreenSize.Y);
            Input = new InputState();
            Input.Screen = Screen;

            scenes = new Scene[Enum.GetNames(typeof(SceneTitle)).Length];
            scenes[(int)SceneTitle.Menu] = new MenuScene(game, spriteBatch);
            scenes[(int)SceneTitle.LevelSelectMenu] = new LevelSelectScene(game, spriteBatch);
            scenes[(int)SceneTitle.Level] = new LevelScene(game, spriteBatch);
            sceneTitle = startScene;
            currentScene = scenes[(int)startScene];
            //currentScene.Initialize();           

            currentScene.Screen = Screen;
            currentScene.Input = Input;
        }

        public void SceneChange(SceneTitle newScene)
        {
            if (newScene != sceneTitle)
            {
                currentScene.Close();
                SceneChanged?.Invoke(sceneTitle, newScene);
                sceneTitle = newScene;
                currentScene = scenes[(int)newScene];
                currentScene.Screen = Screen;
                currentScene.Input = Input;
                currentScene.Initialize();
            }
        }

        public override void Update(GameTime gameTime)
        {
            Input.Update();
            currentScene.Update(gameTime);
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Screen.BeginDraw();
            currentScene.Draw(gameTime);
            base.Draw(gameTime);
        }
    }
}
