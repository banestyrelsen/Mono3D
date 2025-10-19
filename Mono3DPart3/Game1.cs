using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Mono3DPart3;

public class Game1 : Game
{
    struct MyOwnVertexFormat
    {
        private Vector3 position;
        private Vector2 texCoord;

        public MyOwnVertexFormat(Vector3 position, Vector2 texCoord)
        {
            this.position = position;
            this.texCoord = texCoord;
        }
        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(sizeof(float) * 3, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)
        );
    }

    GraphicsDeviceManager graphics;
    GraphicsDevice device;

    Effect effect;
    Matrix viewMatrix;
    Matrix projectionMatrix;
    VertexBuffer vertexBuffer;
    Vector3 cameraPos;
    Texture2D streetTexture;
    
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


        effect = Content.Load<Effect>("v_effects");
        streetTexture = Content.Load<Texture2D> ("streettexture");
        SetUpVertices();
        SetUpCamera();
    }

    private void SetUpVertices()
    {
        MyOwnVertexFormat[] vertices = new MyOwnVertexFormat[3];

        vertices[0] = new MyOwnVertexFormat(new Vector3(-2, 2, 0), new Vector2(0.0f, 0.0f));
        vertices[1] = new MyOwnVertexFormat(new Vector3(2, -2, -2), new Vector2(0.125f, 1.0f));
        vertices[2] = new MyOwnVertexFormat(new Vector3(0, 0, 2), new Vector2(0.25f, 0.0f));

        vertexBuffer = new VertexBuffer(device, MyOwnVertexFormat.VertexDeclaration, vertices.Length,BufferUsage.WriteOnly);
        vertexBuffer.SetData(vertices);
    }

    private void SetUpCamera()
    {
        cameraPos = new Vector3(0, 5, 6);
        viewMatrix = Matrix.CreateLookAt(cameraPos, new Vector3(0, 0, 1), new Vector3(0, 1, 0));
        projectionMatrix =
            Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, device.Viewport.AspectRatio, 1.0f, 200.0f);
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

        effect.CurrentTechnique = effect.Techniques["Simplest"];
        effect.Parameters["xViewProjection"].SetValue(viewMatrix*projectionMatrix);
        effect.Parameters["xTexture"].SetValue(streetTexture);

        foreach (EffectPass pass in effect.CurrentTechnique.Passes)
        {
            pass.Apply();

            device.SetVertexBuffer(vertexBuffer);
            device.DrawPrimitives(PrimitiveType.TriangleList, 0, 1);
        }

        base.Draw(gameTime);
    }
}