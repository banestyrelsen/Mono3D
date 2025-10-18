using System;
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
        private int[] _buildingHeights = new int[] { 0, 2, 2, 6, 5, 4 };
        private Model _xwingModel;
        private Vector3 _lightDirection = new Vector3(3, -2, 5);
        private Vector3 _xwingPosition = new Vector3(8, 1, -3);
        private Quaternion _xwingRotation = Quaternion.Identity;
        private float _gameSpeed = 1.0f;
        
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
            _lightDirection.Normalize();

            base.Initialize();
        }

        private void SetUpCamera()
        {
            _viewMatrix = Matrix.CreateLookAt(new Vector3(20, 13, -5), new Vector3(8, 0, -7), new Vector3(0, 1, 0));
            _projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, _device.Viewport.AspectRatio, 0.2f, 500.0f);
        }

        private void SetUpVertices()
        {
            int differentBuildings = _buildingHeights.Length - 1;
            float imagesInTexture = 1 + differentBuildings * 2;
            
            int cityWidth = _floorPlan.GetLength(0);
            int cityLength = _floorPlan.GetLength(1);

            List<VertexPositionNormalTexture> verticesList = new List<VertexPositionNormalTexture> ();
            for (int x = 0; x < cityWidth; x++)
            {
                for (int z = 0; z < cityLength; z++)
                {
                    int currentBuilding = _floorPlan[x, z];

                    //floor or ceiling
                    verticesList.Add(new VertexPositionNormalTexture(
                        new Vector3(x, _buildingHeights[currentBuilding], -z), new Vector3(0, 1, 0),
                        new Vector2(currentBuilding * 2 / imagesInTexture, 1)));
                    verticesList.Add(new VertexPositionNormalTexture(
                        new Vector3(x, _buildingHeights[currentBuilding], -z - 1), new Vector3(0, 1, 0),
                        new Vector2((currentBuilding * 2) / imagesInTexture, 0)));
                    verticesList.Add(new VertexPositionNormalTexture(
                        new Vector3(x + 1, _buildingHeights[currentBuilding], -z), new Vector3(0, 1, 0),
                        new Vector2((currentBuilding * 2 + 1) / imagesInTexture, 1)));

                    verticesList.Add(new VertexPositionNormalTexture(
                        new Vector3(x, _buildingHeights[currentBuilding], -z - 1), new Vector3(0, 1, 0),
                        new Vector2((currentBuilding * 2) / imagesInTexture, 0)));
                    verticesList.Add(new VertexPositionNormalTexture(
                        new Vector3(x + 1, _buildingHeights[currentBuilding], -z - 1), new Vector3(0, 1, 0),
                        new Vector2((currentBuilding * 2 + 1) / imagesInTexture, 0)));
                    verticesList.Add(new VertexPositionNormalTexture(
                        new Vector3(x + 1, _buildingHeights[currentBuilding], -z), new Vector3(0, 1, 0),
                        new Vector2((currentBuilding * 2 + 1) / imagesInTexture, 1)));
                    
                    if (currentBuilding != 0)
                    {
                        //front wall
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, 0, -z - 1), new Vector3(0, 0, -1), new Vector2((currentBuilding * 2) / imagesInTexture, 1)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, _buildingHeights[currentBuilding], -z - 1), new Vector3(0, 0, -1), new Vector2((currentBuilding * 2 - 1) / imagesInTexture, 0)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, 0, -z - 1), new Vector3(0, 0, -1), new Vector2((currentBuilding * 2 - 1) / imagesInTexture, 1)));

                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, _buildingHeights[currentBuilding], -z - 1), new Vector3(0, 0, -1), new Vector2((currentBuilding * 2 - 1) / imagesInTexture, 0)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, 0, -z - 1), new Vector3(0, 0, -1), new Vector2((currentBuilding * 2) / imagesInTexture, 1)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, _buildingHeights[currentBuilding], -z - 1), new Vector3(0, 0, -1), new Vector2((currentBuilding * 2) / imagesInTexture, 0)));

                        //back wall
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, 0, -z), new Vector3(0, 0, 1), new Vector2((currentBuilding * 2) / imagesInTexture, 1)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, 0, -z), new Vector3(0, 0, 1), new Vector2((currentBuilding * 2 - 1) / imagesInTexture, 1)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, _buildingHeights[currentBuilding], -z), new Vector3(0, 0, 1), new Vector2((currentBuilding * 2 - 1) / imagesInTexture, 0)));

                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, _buildingHeights[currentBuilding], -z), new Vector3(0, 0, 1), new Vector2((currentBuilding * 2 - 1) / imagesInTexture, 0)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, _buildingHeights[currentBuilding], -z), new Vector3(0, 0, 1), new Vector2((currentBuilding * 2) / imagesInTexture, 0)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, 0, -z), new Vector3(0, 0, 1), new Vector2((currentBuilding * 2) / imagesInTexture, 1)));

                        //left wall
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, 0, -z), new Vector3(-1, 0, 0), new Vector2((currentBuilding * 2) / imagesInTexture, 1)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, 0, -z - 1), new Vector3(-1, 0, 0), new Vector2((currentBuilding * 2 - 1) / imagesInTexture, 1)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, _buildingHeights[currentBuilding], -z - 1), new Vector3(-1, 0, 0), new Vector2((currentBuilding * 2 - 1) / imagesInTexture, 0)));

                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, _buildingHeights[currentBuilding], -z - 1), new Vector3(-1, 0, 0), new Vector2((currentBuilding * 2 - 1) / imagesInTexture, 0)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, _buildingHeights[currentBuilding], -z), new Vector3(-1, 0, 0), new Vector2((currentBuilding * 2) / imagesInTexture, 0)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x, 0, -z), new Vector3(-1, 0, 0), new Vector2((currentBuilding * 2) / imagesInTexture, 1)));

                        //right wall
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, 0, -z), new Vector3(1, 0, 0), new Vector2((currentBuilding * 2) / imagesInTexture, 1)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, _buildingHeights[currentBuilding], -z - 1), new Vector3(1, 0, 0), new Vector2((currentBuilding * 2 - 1) / imagesInTexture, 0)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, 0, -z - 1), new Vector3(1, 0, 0), new Vector2((currentBuilding * 2 - 1) / imagesInTexture, 1)));

                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, _buildingHeights[currentBuilding], -z - 1), new Vector3(1, 0, 0), new Vector2((currentBuilding * 2 - 1) / imagesInTexture, 0)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, 0, -z), new Vector3(1, 0, 0), new Vector2((currentBuilding * 2) / imagesInTexture, 1)));
                        verticesList.Add(new VertexPositionNormalTexture(new Vector3(x + 1, _buildingHeights[currentBuilding], -z), new Vector3(1, 0, 0), new Vector2((currentBuilding * 2) / imagesInTexture, 0)));
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
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
                {1,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {1,0,0,1,1,0,0,0,1,1,0,0,1,0,1},
                {1,0,0,1,1,0,0,0,1,0,0,0,1,0,1},
                {1,0,0,0,1,1,0,1,1,0,0,0,0,0,1},
                {1,0,0,0,0,0,0,0,0,0,0,1,0,0,1},
                {1,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {1,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {1,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {1,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {1,0,1,1,0,0,0,1,0,0,0,0,0,0,1},
                {1,0,1,0,0,0,0,0,0,0,0,0,0,0,1},
                {1,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {1,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {1,0,0,0,0,1,0,0,0,0,0,0,0,0,1},
                {1,0,0,0,0,1,0,0,0,1,0,0,0,0,1},
                {1,0,1,0,0,0,0,0,0,1,0,0,0,0,1},
                {1,0,1,1,0,0,0,0,1,1,0,0,0,1,1},
                {1,0,0,0,0,0,0,0,1,1,0,0,0,1,1},
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
            };
            
            Random random = new Random();
            int differentBuildings = _buildingHeights.Length - 1;
            for (int x = 0; x < _floorPlan.GetLength(0); x++)
            {
                for (int y = 0; y < _floorPlan.GetLength(1); y++)
                {
                    if (_floorPlan[x, y] == 1)
                    {
                        _floorPlan[x, y] = random.Next(differentBuildings) + 1;
                    }
                }
            }
        }
        
        private Model LoadModel(string assetName)
        {
            Model newModel = Content.Load<Model>(assetName);
            foreach (ModelMesh mesh in newModel.Meshes)
            {
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    meshPart.Effect = _effect.Clone();
                }
            }
            return newModel;
        }
        
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            _device = _graphics.GraphicsDevice;
            _effect = Content.Load<Effect>("effects");
            _sceneryTexture = Content.Load<Texture2D>("texturemap");
            _xwingModel = LoadModel("xwing");
            
            SetUpCamera();
            SetUpVertices();
        }

        private void MoveForward(ref Vector3 position, Quaternion rotationQuat, float speed)
        {
            Vector3 addVector = Vector3.Transform(new Vector3(0, 0, -1), rotationQuat);
            position += addVector * speed;
        }
        
        private void UpdateCamera()
        {
            Vector3 cameraPosition = new Vector3(0, 0.1f, 0.6f);
            cameraPosition = Vector3.Transform(cameraPosition, Matrix.CreateFromQuaternion(_xwingRotation));
            cameraPosition += _xwingPosition;
            Vector3 cameraUpDirection = new Vector3(0, 1, 0);
            cameraUpDirection = Vector3.Transform(cameraUpDirection, Matrix.CreateFromQuaternion(_xwingRotation));
            _viewMatrix = Matrix.CreateLookAt(cameraPosition, _xwingPosition, cameraUpDirection);
            _projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, _device.Viewport.AspectRatio, 0.2f, 500.0f);
        }
        
        private void ProcessKeyboard(GameTime gameTime)
        {
            float leftRightRotation = 0;

            float turningSpeed = (float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000.0f;
            turningSpeed *= 1.6f * _gameSpeed;

            KeyboardState keys = Keyboard.GetState();

            if (keys.IsKeyDown(Keys.A))
            {
                leftRightRotation += turningSpeed;
            }
            if (keys.IsKeyDown(Keys.D))
            {
                leftRightRotation -= turningSpeed;
            }
            float upDownRotation = 0;
            if (keys.IsKeyDown(Keys.S))
            {
                upDownRotation += turningSpeed;
            }
            if (keys.IsKeyDown(Keys.W))
            {
                upDownRotation -= turningSpeed;
            }
            Quaternion additionalRotation = Quaternion.CreateFromAxisAngle(new Vector3(0, 0, -1), leftRightRotation) * Quaternion.CreateFromAxisAngle(new Vector3(1, 0, 0), upDownRotation);
            _xwingRotation *= additionalRotation;
        }
        
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || 
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            UpdateCamera();
            ProcessKeyboard(gameTime);
            float moveSpeed = gameTime.ElapsedGameTime.Milliseconds / 500.0f * _gameSpeed;
            MoveForward(ref _xwingPosition, _xwingRotation, moveSpeed);
            
            base.Update(gameTime);
        }

        private void DrawCity()
        {
            _effect.CurrentTechnique = _effect.Techniques["Textured"];
            _effect.Parameters["xWorld"].SetValue(Matrix.Identity);
            _effect.Parameters["xView"].SetValue(_viewMatrix);
            _effect.Parameters["xProjection"].SetValue(_projectionMatrix);
            _effect.Parameters["xTexture"].SetValue(_sceneryTexture);
            _effect.Parameters["xEnableLighting"].SetValue(true);
            _effect.Parameters["xLightDirection"].SetValue(_lightDirection);
            _effect.Parameters["xAmbient"].SetValue(0.5f);
            
            foreach (EffectPass pass in _effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                _device.SetVertexBuffer(_cityVertexBuffer);
                _device.DrawPrimitives(PrimitiveType.TriangleList, 0, _cityVertexBuffer.VertexCount/3);
            }
        }
        
        private void DrawModel()
        {
            Matrix worldMatrix = Matrix.CreateScale(0.0005f, 0.0005f, 0.0005f) *
                                 Matrix.CreateRotationY(MathHelper.Pi) *
                                 Matrix.CreateFromQuaternion(_xwingRotation) *
                                 Matrix.CreateTranslation(_xwingPosition);

            Matrix[] xwingTransforms = new Matrix[_xwingModel.Bones.Count];
            _xwingModel.CopyAbsoluteBoneTransformsTo(xwingTransforms);

            foreach (ModelMesh mesh in _xwingModel.Meshes)
            {
                foreach (Effect currentEffect in mesh.Effects)
                {
                    currentEffect.CurrentTechnique = currentEffect.Techniques["Colored"];
                    currentEffect.Parameters["xWorld"].SetValue(xwingTransforms[mesh.ParentBone.Index] * worldMatrix);
                    currentEffect.Parameters["xView"].SetValue(_viewMatrix);
                    currentEffect.Parameters["xProjection"].SetValue(_projectionMatrix);
                    currentEffect.Parameters["xEnableLighting"].SetValue(true);
                    currentEffect.Parameters["xLightDirection"].SetValue(_lightDirection);          
                    currentEffect.Parameters["xAmbient"].SetValue(0.5f);
                }
                mesh.Draw();
            }
        }
        
        protected override void Draw(GameTime gameTime)
        {
            _device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.DarkSlateBlue, 1.0f, 0);

            // TODO: Add your drawing code here
            DrawCity();
            DrawModel();
            
            base.Draw(gameTime);
        }
}