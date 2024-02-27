using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;

using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;
using System;
using MonoGame.Extended.Sprites;

namespace ProjectBanana
{
    public class Game1 : Game
    {
        
        // used for drawing on the screen
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private OrthographicCamera _camera;

        private Vector2 _cameraPosition;

        TiledMap _tiledMap;
        TiledMapRenderer _tiledMapRenderer;


        // playable character
        private Texture2D ballTexture;
        private Vector2 _playerPosition;

        private Vector2 GetMovementDirection()
        {
            var movementDirection = Vector2.Zero;
            var state = Keyboard.GetState();
            if (state.IsKeyDown(Keys.S))
            {
                movementDirection += Vector2.UnitY;
            }
            if (state.IsKeyDown(Keys.W))
            {
                movementDirection -= Vector2.UnitY;
            }
            if (state.IsKeyDown(Keys.A))
            {
                movementDirection -= Vector2.UnitX;
            }
            if (state.IsKeyDown(Keys.D))
            {
                movementDirection += Vector2.UnitX;
            }

            // Can't normalize the zero vector so test for it before normalizing
            if (movementDirection != Vector2.Zero)
            {
                movementDirection.Normalize();
            }

            return movementDirection;
        }


        bool _isFullscreen = false;
        bool _isBorderless = false;
        int _width = 0;
        int _height = 0;
        GameWindow _window;

        public void ToggleFullscreen()
        {
            bool oldIsFullscreen = _isFullscreen;

            if (_isBorderless)
            {
                _isBorderless = false;
            }
            else
            {
                _isFullscreen = !_isFullscreen;
            }

            ApplyFullscreenChange(oldIsFullscreen);
        }
        public void ToggleBorderless()
        {
            bool oldIsFullscreen = _isFullscreen;

            _isBorderless = !_isBorderless;
            _isFullscreen = _isBorderless;

            ApplyFullscreenChange(oldIsFullscreen);
        }

        private void ApplyFullscreenChange(bool oldIsFullscreen)
        {
            if (_isFullscreen)
            {
                if (oldIsFullscreen)
                {
                    ApplyHardwareMode();
                }
                else
                {
                    SetFullscreen();
                }
            }
            else
            {
                UnsetFullscreen();
            }
        }
        private void ApplyHardwareMode()
        {
            _graphics.HardwareModeSwitch = !_isBorderless;
            _graphics.ApplyChanges();
        }
        private void SetFullscreen()
        {
            _width = Window.ClientBounds.Width;
            _height = Window.ClientBounds.Height;

            _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            _graphics.HardwareModeSwitch = !_isBorderless;

            _graphics.IsFullScreen = true;
            _graphics.ApplyChanges();
        }
        private void UnsetFullscreen()
        {
            _graphics.PreferredBackBufferWidth = _width;
            _graphics.PreferredBackBufferHeight = _height;
            _graphics.IsFullScreen = false;
            _graphics.ApplyChanges();
        }

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            //_graphics.PreferMultiSampling = true;
            //_graphics.SynchronizeWithVerticalRetrace = true;
            //_graphics.PreferHalfPixelOffset = true;
            //_graphics.ToggleFullScreen();
            //_graphics.PreferredBackBufferWidth = 1920;
            //_graphics.PreferredBackBufferHeight = 1080;
            //_graphics.ApplyChanges();
    
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        // called after constructor befor game loop => non graphical setup
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            ToggleFullscreen();

            base.Initialize();
        }

        protected override void LoadContent()
        {
  
            _tiledMap = Content.Load<TiledMap>("TestingMap");
            _tiledMapRenderer = new TiledMapRenderer(GraphicsDevice, _tiledMap);
            _spriteBatch = new SpriteBatch(GraphicsDevice);


            ballTexture = Content.Load<Texture2D>("ball");
            _playerPosition = new Vector2(0, 0);
        }

        // regular Updates of game => check for collisions, input, audio etc.
        protected override void Update(GameTime gameTime)
        {

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _tiledMapRenderer.Update(gameTime);

            // removed for now
            MoveCharacter(gameTime);
            //MoveCamera(gameTime);
            //var corrected = new Vector2(_cameraPosition.X - (_cameraPosition.X % 8),
            //    _cameraPosition.Y - (_cameraPosition.Y % 8));

            //_camera.LookAt(_cameraPosition);

            base.Update(gameTime);
        }

        private void MoveCharacter(GameTime gameTime)
        {
            var deltaSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            var walkSpeed = deltaSeconds * 500;
            var keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.W) || keyboardState.IsKeyDown(Keys.Up))
            {
                _playerPosition.Y -= walkSpeed;
            }
            if (keyboardState.IsKeyDown(Keys.S) || keyboardState.IsKeyDown(Keys.Down))
            {
                _playerPosition.Y += walkSpeed;
            }
            if (keyboardState.IsKeyDown(Keys.A) || keyboardState.IsKeyDown(Keys.Left))
            {
                _playerPosition.X -= walkSpeed;
            }

            if (keyboardState.IsKeyDown(Keys.D) || keyboardState.IsKeyDown(Keys.Right))
            {
                _playerPosition.X += walkSpeed;
            }
        }

        // takes game state current and draws game entities  
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);


            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, 
                SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, 
                null, null);

            //_tiledMapRenderer.Draw();
            Matrix transformMatrix = Matrix.CreateScale(GraphicsDevice.Viewport.Width / (float)_tiledMap.WidthInPixels, GraphicsDevice.Viewport.Height / (float)_tiledMap.HeightInPixels, 1f);

            _tiledMapRenderer.Draw(transformMatrix);
            _spriteBatch.Draw(ballTexture, _playerPosition, Color.White);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}