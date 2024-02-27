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

        private Vector2 _targetCameraPosition;
        private float _cameraSpeed = 500.0f; // Adjust the speed as needed

        private void MoveCamera(GameTime gameTime)
        {
            var movementDirection = GetMovementDirection();
            _targetCameraPosition += _cameraSpeed * movementDirection * (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Interpolate the current camera position towards the target position for smoother movement
            _cameraPosition = Vector2.Lerp(_cameraPosition, _targetCameraPosition, 0.1f);
            //_cameraPosition = new Vector2((int)_cameraPosition.X, (int)_cameraPosition.Y);
        }

        //private void MoveCamera(GameTime gameTime)
        //{
        //    var speed = 300;
        //    var seconds = gameTime.GetElapsedSeconds();
        //    var movementDirection = GetMovementDirection();
        //    _cameraPosition += speed * movementDirection * seconds;
        //    //_cameraPosition = new Vector2((int)_cameraPosition.X, (int)_cameraPosition.Y);
        //}

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            //_graphics.PreferMultiSampling = true;
            _graphics.SynchronizeWithVerticalRetrace = true;
            _graphics.PreferHalfPixelOffset = true;
            

            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.PreferredBackBufferHeight = 1080;
       

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        // called after constructor befor game loop => non graphical setup
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here


            //var viewportadapter = new BoxingViewportAdapter(Window, GraphicsDevice, 10*1280, 10*960);
            var viewportadapter = new WindowViewportAdapter(Window, GraphicsDevice);
            //var viewportadapter = new ScalingViewportAdapter(GraphicsDevice, 10 * 1280, 10 * 960);
            _camera = new OrthographicCamera(viewportadapter);

            //_camera.ZoomIn((float)0);
            base.Initialize();
        }

        protected override void LoadContent()
        {
  
            _tiledMap = Content.Load<TiledMap>("samplemap");
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

            _camera.LookAt(_cameraPosition);

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

            // _tiledMapRenderer.Draw();
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, 
                SamplerState.PointClamp, null, null, null);
            
            _tiledMapRenderer.Draw();

            _spriteBatch.Draw(ballTexture, _playerPosition, Color.White);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}