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
    Model carModel;
    Texture2D[] carTextures;
    
    public Game1()
    {
        graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
    }

    protected override void Initialize()
    {
        graphics.PreferredBackBufferWidth = 1200;
        graphics.PreferredBackBufferHeight = 800;
        graphics.IsFullScreen = false;
        graphics.ApplyChanges();
        Window.Title = "Riemer's XNA Tutorials -- Series 3";

        base.Initialize();
    }

    private Model LoadModel(string assetName, out Texture2D[] textures)
    {

        Model newModel = Content.Load<Model> (assetName);
        textures = new Texture2D[7];
        int i = 0;
        foreach (ModelMesh mesh in newModel.Meshes)
        foreach (BasicEffect currentEffect in mesh.Effects)
            textures[i++] = currentEffect.Texture;

        foreach (ModelMesh mesh in newModel.Meshes)
        foreach (ModelMeshPart meshPart in mesh.MeshParts)
            meshPart.Effect = effect.Clone();

        return newModel;
    }
    
    protected override void LoadContent()
    {
        device = GraphicsDevice;


        effect = Content.Load<Effect>("v_effects");
        streetTexture = Content.Load<Texture2D> ("streettexture");
        SetUpVertices();
        SetUpCamera();
        carModel = LoadModel("ferrari", out carTextures);
    }

    private void SetUpVertices()
    {
        MyOwnVertexFormat[] vertices = new MyOwnVertexFormat[12];

        vertices[0] = new MyOwnVertexFormat(new Vector3(-20, 0, 10), new Vector2(-0.25f, 25.0f));
        vertices[1] = new MyOwnVertexFormat(new Vector3(-20, 0, -100), new Vector2(-0.25f, 0.0f));
        vertices[2] = new MyOwnVertexFormat(new Vector3(2, 0, 10), new Vector2(0.25f, 25.0f));
        vertices[3] = new MyOwnVertexFormat(new Vector3(2, 0, -100), new Vector2(0.25f, 0.0f));
        vertices[4] = new MyOwnVertexFormat(new Vector3(2, 1, 10), new Vector2(0.375f, 25.0f));
        vertices[5] = new MyOwnVertexFormat(new Vector3(2, 1, -100), new Vector2(0.375f, 0.0f));
        vertices[6] = new MyOwnVertexFormat(new Vector3(3, 1, 10), new Vector2(0.5f, 25.0f));
        vertices[7] = new MyOwnVertexFormat(new Vector3(3, 1, -100), new Vector2(0.5f, 0.0f));
        vertices[8] = new MyOwnVertexFormat(new Vector3(13, 1, 10), new Vector2(0.75f, 25.0f));
        vertices[9] = new MyOwnVertexFormat(new Vector3(13, 1, -100), new Vector2(0.75f, 0.0f));
        vertices[10] = new MyOwnVertexFormat(new Vector3(13, 21, 10), new Vector2(1.25f, 25.0f));
        vertices[11] = new MyOwnVertexFormat(new Vector3(13, 21, -100), new Vector2(1.25f, 0.0f));

        vertexBuffer = new VertexBuffer(device, MyOwnVertexFormat.VertexDeclaration, vertices.Length,BufferUsage.WriteOnly);
        vertexBuffer.SetData(vertices);
    }

    private void SetUpCamera()
    {
        cameraPos = new Vector3(-25, 13, 18);
        viewMatrix = Matrix.CreateLookAt(cameraPos, new Vector3(0, 2, -12), new Vector3(0, 1, 0));
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

    private void DrawModel(Model model, Texture2D[] textures, Matrix wMatrix, string technique)
    {            
        Matrix[] modelTransforms = new Matrix[model.Bones.Count];
        model.CopyAbsoluteBoneTransformsTo(modelTransforms);
        int i = 0;
        foreach (ModelMesh mesh in model.Meshes)
        {
            foreach (Effect currentEffect in mesh.Effects)
            {
                Matrix worldMatrix = modelTransforms[mesh.ParentBone.Index] * wMatrix;
                currentEffect.CurrentTechnique = currentEffect.Techniques[technique];                                            currentEffect.Parameters["xWorldViewProjection"].SetValue(worldMatrix* viewMatrix*projectionMatrix);
                currentEffect.Parameters["xTexture"].SetValue(textures[i++]);
            }
            mesh.Draw();
        }
    }
    
    protected override void Draw(GameTime gameTime)
    {
        device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.DarkSlateBlue, 1.0f, 0);

        effect.CurrentTechnique = effect.Techniques["Simplest"];
        effect.Parameters["xWorldViewProjection"].SetValue(Matrix.Identity * viewMatrix * projectionMatrix);
        effect.Parameters["xTexture"].SetValue(streetTexture);

        foreach (EffectPass pass in effect.CurrentTechnique.Passes)
        {
            pass.Apply();

            device.SetVertexBuffer(vertexBuffer);
            device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 10);
            
            Matrix car1Matrix = Matrix.CreateScale(4f) * Matrix.CreateRotationY(MathHelper.Pi) * Matrix.CreateTranslation(-3, 0, -15);
            DrawModel(carModel, carTextures, car1Matrix, "Simplest");
 
            Matrix car2Matrix = Matrix.CreateScale(4f) * Matrix.CreateRotationY(MathHelper.Pi * 5.0f / 8.0f) * Matrix.CreateTranslation(-28, 0, -1.9f);
            DrawModel(carModel, carTextures, car2Matrix, "Simplest");
        }

        base.Draw(gameTime);
    }
}