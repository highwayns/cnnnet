using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using CnnNetLib;

namespace CnnNet4
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class GameCnnNet : Microsoft.Xna.Framework.Game
    {
        #region Fields

        private const int WIDTH = 800;
        private const int HEIGHT = 600;

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private Texture2D _neuron;
        private Texture2D _background;
        private int[] _backgroundData;

        private CnnNet _cnnNet;

        #endregion

        #region Instance

        public GameCnnNet()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = WIDTH;
            graphics.PreferredBackBufferHeight = HEIGHT;
            graphics.ApplyChanges();
            
            Content.RootDirectory = "Content";
        }

        #endregion

        #region Methods

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

            _cnnNet = new CnnNet(WIDTH, HEIGHT, 0.001, 10);
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
            _neuron = Content.Load<Texture2D>("neuron");

            _background = new Texture2D(GraphicsDevice, WIDTH, HEIGHT);
            _backgroundData = Enumerable.Repeat<int>(-1, _background.Width * _background.Height).ToArray();
            _background.SetData<int>(_backgroundData);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
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
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

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
            spriteBatch.Begin();

            spriteBatch.Draw(_background, new Rectangle(0, 0, WIDTH, HEIGHT), Color.White);

            int[,] tableNeurons = _cnnNet.GetTableNeurons();

            for (int y = 0; y < HEIGHT; y++)
            {
                for (int x = 0; x < WIDTH; x++)
                {
                    if (tableNeurons[y, x] != 0)
                    {
                        spriteBatch.Draw(_neuron, new Vector2(x, y), Color.White);
                    }
                }
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        #endregion
    }
}