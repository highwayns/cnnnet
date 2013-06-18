#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using cnnnet.Lib;
using System.Linq;
#endregion

namespace cnnnet.Viewer
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class CnnNetGame : Game
    {
        #region Fields

        private readonly FormNetworkControl _formNetworkControl;

#pragma warning disable 169
        private const int ColorIndexRed = 0;
        private const int ColorIndexGreen = 1;
        private const int ColorIndexBlue = 2;
#pragma warning restore 169

        private const int Width = 800;
        private const int Height = 600;

        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Texture2D _neuronIdle;
        private Texture2D _neuronActive;

        private Texture2D _neuronInputIdle;
        private Texture2D _neuronInputActive;

        private Texture2D _background;
        private Texture2D _blank;

        private byte[] _backgroundData;
        private readonly CnnNet _cnnNet;

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

            IsMouseVisible = true;
            IsFixedTimeStep = false;

            _formNetworkControl.Show();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // use this.Content to load your game content here
            _neuronIdle = Content.Load<Texture2D>("neuronIdle");
            _neuronActive = Content.Load<Texture2D>("neuronActive");
            _neuronInputIdle = Content.Load<Texture2D>("neuronInputIdle");
            _neuronInputActive = Content.Load<Texture2D>("neuronInputActive");

            _background = new Texture2D(GraphicsDevice, Width, Height);
            _backgroundData = Enumerable.Repeat<byte>(0, _background.Width * _background.Height * 4).ToArray();
            _background.SetData(_backgroundData);
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
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
            GraphicsDevice.Clear(Color.Black);
            GraphicsDevice.Textures[0] = null;

            var neuronDesirabilityMap = _cnnNet.NeuronDesirabilityMap ?? new double[0, 0];
            var neuronUndesirabilityMap = _cnnNet.NeuronUndesirabilityMap ?? new double[0, 0];

            var tableNeurons = _cnnNet.Neurons ?? new NeuronCompute[0];

            _backgroundData = Enumerable.Repeat<byte>(0, _background.Width * _background.Height * 4).ToArray();

            // Drawing code here
            _spriteBatch.Begin();

            #region Background

            if (_formNetworkControl.dsNeuronDesirabilityMap.Checked)
            {
                UpdateBackground(neuronDesirabilityMap, ColorIndexGreen);
            }

            if (_formNetworkControl.dsNeuronUndesirabilityMap.Checked)
            {
                UpdateBackground(neuronUndesirabilityMap, ColorIndexRed);
            }

            #endregion

            _background.SetData(_backgroundData);
            _spriteBatch.Draw(_background, new Rectangle(0, 0, Width, Height), Color.White);

            #region Neurons

            UpdateNeurons(tableNeurons);

            #endregion

            var mouseState = Mouse.GetState();

            _spriteBatch.Draw(_neuronIdle,
                new Vector2(mouseState.X - _neuronIdle.Width / 2, mouseState.Y - _neuronIdle.Height / 2),
                Color.White);

            _formNetworkControl.Text = string.Format("X = {0} Y = {1}", mouseState.X, mouseState.Y);

            _spriteBatch.End();

            base.Draw(gameTime);
            System.Windows.Forms.Application.DoEvents();
        }

        

        private void UpdateBackground(double[,] values, int colorIndex)
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    int index = y * Width * 4 + x * 4;

                    _backgroundData[index + colorIndex] = (byte)(values[y, x] * 255);
                }
            }
        }

        private void UpdateNeurons(IEnumerable<NeuronBase> neurons)
        {
            Texture2D circle = CreateCircle(_cnnNet.NeuronDesirabilityInfluenceRange);

            foreach (NeuronBase neuron in neurons)
            {
                var inputNeuron = neuron as NeuronInput;
                var isInputNeuron = inputNeuron != null;

                var neuronTexture = neuron.IsActive
                                      ? isInputNeuron ? _neuronInputActive : _neuronActive
                                      : isInputNeuron ? _neuronInputIdle : _neuronIdle;

                _spriteBatch.Draw(neuronTexture, new Vector2
                    (neuron.PosX - neuronTexture.Width / 2,
                    neuron.PosY - neuronTexture.Height / 2), Color.White);

                if (neuron.HasReachedFinalPosition
                    && _formNetworkControl.dsDisplayNeuronDesirabilityRange.Checked)
                {
                    _spriteBatch.Draw(circle, new Vector2(neuron.PosX - _cnnNet.NeuronDesirabilityInfluenceRange, neuron.PosY - _cnnNet.NeuronDesirabilityInfluenceRange), Color.Red);
                }

                if (isInputNeuron)
                {
                    for (int i = 1; i < inputNeuron.AxonWaypoints.Count; i++)
                    {
                        var startPos = new Vector2(inputNeuron.AxonWaypoints[i - 1].Item2, inputNeuron.AxonWaypoints[i - 1].Item1);
                        var endPos = new Vector2(inputNeuron.AxonWaypoints[i].Item2, inputNeuron.AxonWaypoints[i].Item1);

                        _blank = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
                        _blank.SetData(new[] { Color.White });
                        DrawLine(_spriteBatch, _blank, 1, Color.White, startPos, endPos);
                    }

                    var axonLastWayPoint = inputNeuron.AxonWaypoints.LastOrDefault();

                    var axonLastWaypointCircle = CreateCircle(_cnnNet.AxonHigherUndesirabilitySearchPlainRange);

                    _spriteBatch.Draw(axonLastWaypointCircle, new Vector2(axonLastWayPoint.Item2 - _cnnNet.AxonHigherUndesirabilitySearchPlainRange, axonLastWayPoint.Item1 - _cnnNet.AxonHigherUndesirabilitySearchPlainRange), Color.Red);
                }
            }
        }

        private Texture2D CreateCircle(int radius)
        {
            int outerRadius = radius * 2 + 2; // So circle doesn't go out of bounds
            var texture = new Texture2D(GraphicsDevice, outerRadius, outerRadius);

            var data = new Color[outerRadius * outerRadius];

            // Colour the entire texture transparent first.
            for (int i = 0; i < data.Length; i++)
                data[i] = Color.Transparent;

            // Work out the minimum step necessary using trigonometry + sine approximation.
            double angleStep = 1f / radius;

            for (double angle = 0; angle < Math.PI * 2; angle += angleStep)
            {
                // Use the parametric definition of a circle: http://en.wikipedia.org/wiki/Circle#Cartesian_coordinates
                var x = (int)Math.Round(radius + radius * Math.Cos(angle));
                var y = (int)Math.Round(radius + radius * Math.Sin(angle));

                data[y * outerRadius + x + 1] = Color.White;
            }

            texture.SetData(data);
            return texture;
        }

        private void DrawLine(SpriteBatch batch, Texture2D blank,
              float width, Color color, Vector2 point1, Vector2 point2)
        {
            var angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
            var length = Vector2.Distance(point1, point2);

            batch.Draw(blank, point1, null, color,
                       angle, Vector2.Zero, new Vector2(length, width),
                       SpriteEffects.None, 0);
        }

        #endregion

        #region Instance

        public CnnNetGame()
            : base()
        {
            _graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = Width,
                PreferredBackBufferHeight = Height
            };
            _graphics.ApplyChanges();
            Content.RootDirectory = "Content";

            _blank = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            _blank.SetData(new[] { Color.White });

            _formNetworkControl = new FormNetworkControl();
            _cnnNet = new CnnNet(Width, Height);
            _formNetworkControl.CnnNet = _cnnNet;
        }

        #endregion
    }
}
