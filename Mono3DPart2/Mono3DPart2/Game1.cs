using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Mono3DPart2;

public class Game1 : Game
{
        //Properties
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private GraphicsDevice _device;
        private Effect _effect;

        private Matrix _viewMatrix;
        private Matrix _projectionMatrix;
        private Texture2D _sceneryTexture;
        private int[,] _floorPlan;

        private VertexBuffer _cityVertexBuffer;
        
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            _graphics.PreferredBackBufferWidth = 500;
            _graphics.PreferredBackBufferHeight = 500;
            _graphics.IsFullScreen = false;
            _graphics.ApplyChanges();
            Window.Title = "Riemer's MonoGame Tutorials -- 3D Series 2";

            LoadFloorPlan();
            
            base.Initialize();
        }

        private void SetUpCamera()
        {
            _viewMatrix = Matrix.CreateLookAt(new Vector3(3, 5, 2), new Vector3(2, 0, -1), new Vector3(0, 1, 0));
            _projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, _device.Viewport.AspectRatio, 0.2f, 500.0f);
        }

        private void SetUpVertices()
        {
            int cityWidth = _floorPlan.GetLength(0);
            int cityLength = _floorPlan.GetLength(1);

            List<VertexPositionNormalTexture> verticesList = new List<VertexPositionNormalTexture> ();
            for (int x = 0; x < cityWidth; x++)
            {
                for (int z = 0; z < cityLength; z++)
                {
                    int imagesInTexture = 11;
                    if (_floorPlan[x, z] == 0)
                    {
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, 0, -z), new Vector3(0, 1, 0), new Vector2(0, 1)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, 0, -z - 1), new Vector3(0, 1, 0), new Vector2(0, 0)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, 0, -z), new Vector3(0, 1, 0), new Vector2(1.0f / imagesInTexture, 1)));

                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, 0, -z - 1), new Vector3(0, 1, 0), new Vector2(0, 0)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, 0, -z-1), new Vector3(0, 1, 0), new Vector2(1.0f / imagesInTexture, 0)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, 0, -z), new Vector3(0, 1, 0), new Vector2(1.0f / imagesInTexture, 1)));
                    }
                }
            }

            _cityVertexBuffer = new VertexBuffer(_device, VertexPositionNormalTexture.VertexDeclaration, verticesList.Count, BufferUsage.WriteOnly);

            _cityVertexBuffer.SetData<VertexPositionNormalTexture> (verticesList.ToArray());     
        }
        
        private void LoadFloorPlan()
        {
            _floorPlan = new int[,]
            {
                {0,0,0},
                {0,1,0},
                {0,0,0},
            };
        }
        
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            _device = _graphics.GraphicsDevice;
            _effect = Content.Load<Effect>("effects");
            _sceneryTexture = Content.Load<Texture2D>("texturemap");
            
            SetUpCamera();
            SetUpVertices();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || 
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        private void DrawCity()
        {
            _effect.CurrentTechnique = _effect.Techniques["Textured"];
            _effect.Parameters["xWorld"].SetValue(Matrix.Identity);
            _effect.Parameters["xView"].SetValue(_viewMatrix);
            _effect.Parameters["xProjection"].SetValue(_projectionMatrix);
            _effect.Parameters["xTexture"].SetValue(_sceneryTexture);

            foreach (EffectPass pass in _effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                _device.SetVertexBuffer(_cityVertexBuffer);
                _device.DrawPrimitives(PrimitiveType.TriangleList, 0, _cityVertexBuffer.VertexCount/3);
            }
        }
        
        protected override void Draw(GameTime gameTime)
        {
            _device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.DarkSlateBlue, 1.0f, 0);

            // TODO: Add your drawing code here
            DrawCity();
            
            base.Draw(gameTime);
        }
}