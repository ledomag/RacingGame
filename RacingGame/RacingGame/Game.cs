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

using RacingGame.Graphics;

namespace RacingGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private PrimitiveBatch _primitiveBatch;

        private SpriteFont _font;
        private Dictionary<string, Texture2D> _textures = new Dictionary<string, Texture2D>();

        private const string _carValuesPath = "CarValues.txt";
        private Counter _startCounter;
        private bool _isActive = false;
        private CarValues _carValues;
        private Unit _player;
        private RacingGameAI _ai;
        private List<GameObject> _objects = new List<GameObject>();

        public Game()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = 1024;
            _graphics.PreferredBackBufferHeight = 768;
            _graphics.ApplyChanges();

            Environment.Scale = 0.7f;
            Environment.PixelNumInMeter = 15;
            Environment.FrictionForce = 10f;
            Environment.FrictionForceLMax = 30f;
            Environment.FrictionForceWMax = 30f;
            Environment.FrictionForceRotationMax = 2f;

            _carValues = new CarValues()
            {
                EnginePower = 0.3f,
                Width = 1.85f,
                Height = 2.3f,
                GearsAcceleration = new[] { -0.3f, 0.000000001f, 0.3f, 0.566f, 0.8f, 1f },
                DamperStiffness = 3,
                LSlope = 2f,
                WSlope = 3.5f,
                TurningRadius = 5.7f,
                ControlSensitivity = 1f,
                DistanceRACG = 1.25f,
                SpeedMax = 100
            };

            //Serializer.XmlSerialization(_carValues, _carValuesPath);

            base.Initialize();
        }

        private void InitializeObject()
        {
            _isActive = false;
            _startCounter = new Counter(() => { _isActive = true; _startCounter = null; }, 1000);

            _objects.Clear();

            Animation blockAnim = new Animation(_textures["Block"], 32, 55, 0);

            #region " Blocks "

            for (int i = 1; i < 12; i++)
                _objects.Add(new GameObject(blockAnim, new Position(30, i * 80 + 60)));
            for (int i = 1; i < 12; i++)
                _objects.Add(new GameObject(blockAnim, new Position(1430, i * 80 + 60)));

            for (int i = 1; i < 17; i++)
                _objects.Add(new GameObject(blockAnim, new Position(i * 80 + 40, 30)) { Angle = 90 });
            for (int i = 1; i < 17; i++)
                _objects.Add(new GameObject(blockAnim, new Position(i * 80 + 40, 1070)) { Angle = 90 });

            _objects.Add(new GameObject(blockAnim, new Position(50, 60)) { Angle = 135 });
            _objects.Add(new GameObject(blockAnim, new Position(1400, 60)) { Angle = 45 });
            _objects.Add(new GameObject(blockAnim, new Position(50, 1020)) { Angle = 45 });
            _objects.Add(new GameObject(blockAnim, new Position(1400, 1020)) { Angle = 135 });

            for (int i = 1; i < 5; i++)
                _objects.Add(new GameObject(blockAnim, new Position(400, i * 80 + 340)));
            for (int i = 1; i < 5; i++)
                _objects.Add(new GameObject(blockAnim, new Position(1070, i * 80 + 340)));

            for (int i = 1; i < 9; i++)
                _objects.Add(new GameObject(blockAnim, new Position(i * 80 + 380, 350)) { Angle = 90 });
            for (int i = 1; i < 9; i++)
                _objects.Add(new GameObject(blockAnim, new Position(i * 80 + 380, 730)) { Angle = 90 });

            #endregion

            //CarValues playerCarValues = Serializer.XmlDeserialization<CarValues>(_carValuesPath);

            _player = new Unit(new Animation(_textures["Car1"], 32, 55, 0), new Position(700, 850), _carValues.Clone()) { Angle = 90 };
            _player.CarValues.SpeedMax = 100;
            _objects.Add(_player);

            Unit unit1 = new Unit(new Animation(_textures["Car2"], 32, 55, 0), new Position(850, 950), _carValues.Clone()) { Angle = 90 };
            _objects.Add(unit1);
            unit1.CarValues.EnginePower = 0.4f;
            Unit unit2 = new Unit(new Animation(_textures["Car3"], 32, 55, 0), new Position(850, 850), _carValues.Clone()) { Angle = 90 };
            _objects.Add(unit2);
            unit2.CarValues.EnginePower = 0.2f;
            Unit unit3 = new Unit(new Animation(_textures["Car4"], 32, 55, 0), new Position(700, 950), _carValues.Clone()) { Angle = 90 };
            _objects.Add(unit3);

            _ai = new RacingGameAI() { LapCount = 5, DistanceMax = 2000, SpeedMax = 220, SpeedMiddle = 100, SpeedMin = 20 };
            _ai.Waypoints.Add(new Waypoint() { Position = new Position(1210, 860), Radius = 190, SpeedMin = 10, SpeedMax = 100 });
            _ai.Waypoints.Add(new Waypoint() { Position = new Position(1210, 220), Radius = 190, SpeedMin = 10, SpeedMax = 100 });
            _ai.Waypoints.Add(new Waypoint() { Position = new Position(260, 220), Radius = 190, SpeedMin = 10, SpeedMax = 100 });
            _ai.Waypoints.Add(new Waypoint() { Position = new Position(260, 860), Radius = 190, SpeedMin = 10, SpeedMax = 100 });

            _ai.AddPlayer(_player);
            _ai.AddUnit(unit1);
            _ai.AddUnit(unit2);
            _ai.AddUnit(unit3);
        }

        protected override void LoadContent()
        {
            _textures.Add("Ground", Content.Load<Texture2D>("Ground"));
            _textures.Add("Car1", Content.Load<Texture2D>("Car1"));
            _textures.Add("Car2", Content.Load<Texture2D>("Car2"));
            _textures.Add("Car3", Content.Load<Texture2D>("Car3"));
            _textures.Add("Car4", Content.Load<Texture2D>("Car4"));
            _textures.Add("Block", Content.Load<Texture2D>("Block"));
            _font = Content.Load<SpriteFont>("Arial");

            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _primitiveBatch = new PrimitiveBatch(GraphicsDevice);

            InitializeObject();
        }

        protected override void UnloadContent()
        {
            Content.Unload();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            uint msec = (uint)gameTime.ElapsedGameTime.Milliseconds;

            if (_startCounter != null)
                _startCounter.Update(msec);

            KeyboardState kbState = Keyboard.GetState();

            if (kbState.IsKeyDown(Keys.R))
                InitializeObject();

            if (_isActive)
            {
                if (_player != null)
                {
                    CarControl carControl = CarControl.None;

                    if (kbState.IsKeyDown(Keys.Up))
                        carControl |= CarControl.Gas;
                    if (kbState.IsKeyDown(Keys.Down))
                        carControl |= CarControl.Break;
                    if (kbState.IsKeyDown(Keys.Left))
                        carControl |= CarControl.Right;
                    if (kbState.IsKeyDown(Keys.Right))
                        carControl |= CarControl.Left;
                    //if (kbState.IsKeyDown(Keys.Z))
                    //    carControl |= CarControl.GearUp;
                    //if (kbState.IsKeyDown(Keys.X))
                    //    carControl |= CarControl.GearDown;

                    _player.Control = carControl;
                }

                _ai.Update();

                for (int i = 0; i < _objects.Count; i++)
                    if (_objects[i] is Unit)
                        ((Unit)_objects[i]).Move(msec);

                for (int i = 0; i < _objects.Count; i++)
                {
                    for (int j = 0; j < _objects.Count; j++)
                    {
                        if (i != j)
                        {
                            GameObject obj1 = _objects[i];
                            GameObject obj2 = _objects[j];

                            int x1 = (int)(obj1.Position.X + obj1.Animation.FrameWidth / 2);
                            int y1 = (int)(obj1.Position.Y + obj1.Animation.FrameHeight / 2);
                            int x2 = (int)(obj2.Position.X + obj2.Animation.FrameWidth / 2);
                            int y2 = (int)(obj2.Position.Y + obj2.Animation.FrameHeight / 2);

                            int subX = x1 - x2;
                            int subY = y1 - y2;
                            int length = (int)Math.Sqrt(Math.Pow(subX, 2) + Math.Pow(subY, 2));

                            int maxHeight = (obj1.Animation.FrameHeight > obj2.Animation.FrameHeight) ? obj1.Animation.FrameHeight : obj2.Animation.FrameHeight;

                            if (length < maxHeight)
                            {
                                if (obj1 is Unit)
                                    ((Unit)obj1).Ñollision(obj2);

                                if (obj2 is Unit)
                                    ((Unit)obj2).Ñollision(obj1);

                                break;
                            }
                        }
                    }
                }
            }

            base.Update(gameTime);
        }

        private void DrawObject(GameObject obj)
        {
            float halfWidth = obj.Animation.FrameWidth / 2;
            float halfHeight = obj.Animation.FrameHeight / 2;

            Vector2 pos = new Vector2(obj.Position.X, obj.Position.Y);
            Vector2 orign = new Vector2(halfWidth, halfHeight);
            Vector2 pos2 = pos + orign;

            _spriteBatch.Draw(
                obj.Animation.Texture,
                new Rectangle(
                    (int)((pos2.X - halfWidth) * Environment.Scale),
                    (int)((pos2.Y - halfHeight) * Environment.Scale),
                    (int)(obj.Animation.FrameWidth * Environment.Scale),
                    (int)(obj.Animation.FrameHeight * Environment.Scale)),
                obj.Animation.Frame,
                Color.White,
                -(float)obj.Angle.ToRadian(),
                orign,
                SpriteEffects.None,
                0f);

            Unit unit = obj as Unit;

            if (unit != null)
            {
                string text = String.Format("speed: {0}\r\nmax: {1}", Math.Round(unit.Speed), Math.Round(unit.CarValues.SpeedMax));
                _spriteBatch.DrawString(_font, text, new Vector2(pos.X * Environment.Scale, pos.Y * Environment.Scale), Color.White);
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            _spriteBatch.Draw(
                _textures["Ground"],
                new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight),
                Color.White);

            for (int i = 0; i < _objects.Count; i++)
                DrawObject(_objects[i]);

            _spriteBatch.End();

            _primitiveBatch.Begin();

            for (int i = 0; i < _ai.Waypoints.Count; i++)
                _primitiveBatch.DrawCircle(_ai.Waypoints[i].Position.X * Environment.Scale, _ai.Waypoints[i].Position.Y * Environment.Scale, _ai.Waypoints[i].Radius * Environment.Scale, Color.Blue);

            //for (int i = 0; i < _objects.Count; i++)
            //    _primitiveBatch.DrawCircle(_objects[i].Position.X * Environment.Scale, _objects[i].Position.Y * Environment.Scale, 55 /2 * Environment.Scale, Color.White);

            _primitiveBatch.End();

            base.Draw(gameTime);
        }
    }
}
