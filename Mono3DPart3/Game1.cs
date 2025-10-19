using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Mono3DPart3;

public class Game1 : Game
{
         GraphicsDeviceManager graphics;
         GraphicsDevice device;
 
         Effect effect;
         Matrix viewMatrix;
         Matrix projectionMatrix;
         VertexBuffer vertexBuffer;
         Vector3 cameraPos;
 
         public Game1()
         {
             graphics = new GraphicsDeviceManager(this);
             Content.RootDirectory = "Content";
         }
 
         protected override void Initialize()
         {
             graphics.PreferredBackBufferWidth = 500;
             graphics.PreferredBackBufferHeight = 500;
             graphics.IsFullScreen = false;
             graphics.ApplyChanges();
             Window.Title = "Riemer's XNA Tutorials -- Series 3";
 
             base.Initialize();
         }
 
         protected override void LoadContent()
         {
             device = GraphicsDevice;
 

            effect = Content.Load<Effect> ("effects"); SetUpVertices();            SetUpCamera();
        }

        private void SetUpVertices()
        {
            VertexPositionColor[] vertices = new VertexPositionColor[3];

            vertices[0] = new VertexPositionColor(new Vector3(-2, 2, 0), Color.Red);
            vertices[1] = new VertexPositionColor(new Vector3(2, -2, -2), Color.Green);
            vertices[2] = new VertexPositionColor(new Vector3(0, 0, 2), Color.Yellow);

            vertexBuffer = new VertexBuffer(device, VertexPositionColor.VertexDeclaration, vertices.Length, BufferUsage.WriteOnly);
            vertexBuffer.SetData(vertices);
        }

        private void SetUpCamera()
        {
            cameraPos = new Vector3(0, 5, 6);
            viewMatrix = Matrix.CreateLookAt(cameraPos, new Vector3(0, 0, 1), new Vector3(0, 1, 0));
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, device.Viewport.AspectRatio, 1.0f, 200.0f);
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.DarkSlateBlue, 1.0f, 0);

            effect.CurrentTechnique = effect.Techniques["ColoredNoShading"];
            effect.Parameters["xView"].SetValue(viewMatrix);
            effect.Parameters["xProjection"].SetValue(projectionMatrix);
            effect.Parameters["xWorld"].SetValue(Matrix.Identity);

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                device.SetVertexBuffer(vertexBuffer);
                device.DrawPrimitives(PrimitiveType.TriangleList, 0, 1);
            }

            base.Draw(gameTime);
        }
}