#region File Description
//-----------------------------------------------------------------------------
// ScreenManager.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using System.IO;
using System.IO.IsolatedStorage;
#endregion

namespace HexagonMatch
{
    public class Screen : DrawableGameComponent
    {
        #region Fields

        SpriteBatch spriteBatch;

        private int virtualWidth;
        private int virtualHeight;
        private bool updateMatrix = true;
        private Matrix scaleMatrix = Matrix.Identity;
        private GraphicsDeviceManager DeviceManager = null;
        private Point realScreenSize;

        #endregion

        #region Properties


        /// <summary>
        /// A default SpriteBatch shared by all the screens. This saves
        /// each screen having to bother creating their own local instance.
        /// </summary>
        public SpriteBatch SpriteBatch
        {
            get { return spriteBatch; }
        }

        public Viewport Viewport
        {
            get
            {
                return new Viewport(0, 0, virtualWidth, virtualHeight);
            }
        }

        public Vector2 InputTranslate
        {
            get
            {
                return new Vector2(GraphicsDevice.Viewport.X, GraphicsDevice.Viewport.Y);
            }
        }

        public Matrix Scale
        {
            get
            {
                if (updateMatrix)
                {
                    CreateScaleMatrix();
                    updateMatrix = false;
                }
                return scaleMatrix;
            }
        }

        public Matrix InputScale
        {
            get
            {
                return Matrix.Invert(Scale);
            }
        }


        #endregion


        /// <summary>
        /// Constructs a new screen manager component.
        /// </summary>
        public Screen(Game game, int virtualWidth, int virtualHeight)
            : base(game)
        {
            // set the Virtual environment up
            this.virtualHeight = virtualHeight;
            this.virtualWidth = virtualWidth;
            this.DeviceManager = (GraphicsDeviceManager)game.Services.GetService(typeof(IGraphicsDeviceManager));
            realScreenSize = DeviceManager.PreferredBackBufferWidth < DeviceManager.PreferredBackBufferHeight ?
                new Point(DeviceManager.PreferredBackBufferWidth, DeviceManager.PreferredBackBufferHeight) :
                new Point(DeviceManager.PreferredBackBufferHeight, DeviceManager.PreferredBackBufferWidth);
        }
        /// <summary>
        /// Load your graphics content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected void CreateScaleMatrix()
        {
            scaleMatrix = Matrix.CreateScale(
                           (float)GraphicsDevice.Viewport.Width / virtualWidth,
                           (float)GraphicsDevice.Viewport.Width / virtualWidth,
                           1f);
        }

        protected void FullViewport()
        {
            Viewport vp = new Viewport();
            vp.X = vp.Y = 0;
            vp.Width = realScreenSize.X;
            vp.Height = realScreenSize.Y;
            GraphicsDevice.Viewport = vp;
        }

        protected float GetVirtualAspectRatio()
        {
            return (float)virtualWidth / (float)virtualHeight;
        }

        protected void ResetViewport()
        {
            float targetAspectRatio = GetVirtualAspectRatio();
            // figure out the largest area that fits in this resolution at the desired aspect ratio
            int width = realScreenSize.X;
            int height = (int)(width / targetAspectRatio + .5f);
            bool changed = false;

            if (height > realScreenSize.Y)
            {
                height = realScreenSize.Y;
                // PillarBox
                width = (int)(height * targetAspectRatio + .5f);
                changed = true;
            }

            // set up the new viewport centered in the backbuffer
            Viewport viewport = new Viewport();

            viewport.X = (realScreenSize.X / 2) - (width / 2);
            viewport.Y = (realScreenSize.Y / 2) - (height / 2);
            viewport.Width = width;
            viewport.Height = height;
            viewport.MinDepth = 0;
            viewport.MaxDepth = 1;

            if (changed)
            {
                updateMatrix = true;
            }

            DeviceManager.GraphicsDevice.Viewport = viewport;
        }

        public void BeginDraw()
        {
            // Start by reseting viewport to (0,0,1,1)
            FullViewport();
            // Clear to Black
            GraphicsDevice.Clear(Color.Black);
            // Calculate Proper Viewport according to Aspect Ratio
            ResetViewport();
            // and clear that
            // This way we are gonna have black bars if aspect ratio requires it and
            // the clear color on the rest
            GraphicsDevice.Clear(Color.Black);
        }

    }
}