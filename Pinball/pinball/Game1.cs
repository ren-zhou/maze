using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using pinball.Physics;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace pinball
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Texture2D _ballTexture;
        private Texture2D _flipperTexture;

        private const float RESTANGLE = 20;
        private float _leftAngle = MathHelper.ToRadians(RESTANGLE);
        private float _rightAngle = MathHelper.ToRadians(180 - RESTANGLE);
        private float _rotSpeed = MathHelper.ToRadians(30);
        private Level _level;

        private KeyboardState _prevKeyState;

        private List<Particle> _particles = new List<Particle>();

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferMultiSampling = false;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            _prevKeyState = Keyboard.GetState();
            _level = new Level(GraphicsDevice);
            Window.Title = "Collision";
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            _level.LoadContent(Content);
            _ballTexture = Content.Load<Texture2D>("ball");
            _flipperTexture = Content.Load<Texture2D>("flipper");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            //ProcessInput(); 
            _level.Update((float)1 / 30);


            base.Update(gameTime);
        }

        private void ProcessInput()
        {
            KeyboardState keystate = Keyboard.GetState();
            if (keystate.IsKeyDown(Keys.Z))
            {
                MoveLeft(1);
            }
            else
            {
                MoveLeft(-1);
            }
            if (keystate.IsKeyDown(Keys.M))
            {
                MoveRight(1);
            }
            else
            {
                MoveRight(-1);
            }
            _prevKeyState = keystate;
        }

        private bool IsKeyDownEdge(Keys key, KeyboardState keystate)
        {
            return keystate.IsKeyDown(key) && _prevKeyState.IsKeyUp(key);
        }

        private void Fire()
        {
            _particles.Add(new Particle(new Vector2(50, 50), new Vector2(100, 0), new Vector2(0, 0), 0.999f, 100, _ballTexture));
        }

        private void MoveLeft(int up)
        {
            _leftAngle = MathHelper.Clamp(_leftAngle - up*_rotSpeed, -(float)Math.PI / 2, MathHelper.ToRadians(RESTANGLE));
        }

        private void MoveRight(int up)
        {
            _rightAngle = MathHelper.Clamp(_rightAngle + up*_rotSpeed, MathHelper.ToRadians(180 - RESTANGLE), 3*(float)Math.PI / 2);
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            _spriteBatch.Begin();
            _level.Draw(_spriteBatch);
            //_spriteBatch.Draw(_ballTexture, new Rectangle(0, 0, 64, 64), Color.White);
            //_spriteBatch.Draw(_flipperTexture, new Rectangle(100, 100, 128, 31), null, Color.White, _leftAngle, new Vector2(12, 16), SpriteEffects.None, 1);
            //_spriteBatch.Draw(_flipperTexture, new Rectangle(400, 100, 128, 31), null, Color.White, _rightAngle, new Vector2(12, 16), SpriteEffects.None, 1);
            //DrawParticles();
            _spriteBatch.End();


            base.Draw(gameTime);
        }
    }
}