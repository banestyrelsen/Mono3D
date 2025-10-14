using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Mono3D;

public class Game1 : Game
{
    // Properties
    private GraphicsDeviceManager _graphics;
    private GraphicsDevice _device;
    private Effect _effect;
    private VertexPositionColor[] _vertices;
    private int[] _indices;
    private Matrix _viewMatrix;
    private Matrix _projectionMatrix;
    private float _angle = 0f;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        _graphics.PreferredBackBufferWidth = 500;
        _graphics.PreferredBackBufferHeight = 500;
        _graphics.IsFullScreen = false;
        _graphics.ApplyChanges();
        Window.Title = "Riemer's MonoGame Tutorials -- 3D Series 1";

        base.Initialize();
    }

    private void SetUpVertices()
    {
        _vertices = new VertexPositionColor[5]
        {
            new VertexPositionColor() {Position = new Vector3(0f, 0f, 0f), Color = Color.White},
            new VertexPositionColor() {Position = new Vector3(5f, 0f, 0f), Color = Color.White},
            new VertexPositionColor() {Position = new Vector3(10f, 0f, 0f), Color = Color.White},
            new VertexPositionColor() {Position = new Vector3(5f, 0f, -5f), Color = Color.White},
            new VertexPositionColor() {Position = new Vector3(10f, 0f, -5f), Color = Color.White}
        };
    }

    private void SetUpIndices()
    {
        _indices = new int[6];

        _indices[0] = 3;
        _indices[1] = 1;
        _indices[2] = 0;
        _indices[3] = 4;
        _indices[4] = 2;
        _indices[5] = 1;
    }    
    
    private void SetUpCamera()
    {
        _viewMatrix = Matrix.CreateLookAt(new Vector3(0, 50, 0), new Vector3(0, 0, 0), new Vector3(0, 0, -1));
        _projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, _device.Viewport.AspectRatio, 1.0f, 300.0f);
    }    
    
    protected override void LoadContent()
    {
        _device = _graphics.GraphicsDevice;

        _effect = Content.Load<Effect>("effects");        
        SetUpVertices();
        SetUpIndices();
        SetUpCamera();
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        _angle += 0.05f;

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
#if DEBUG
        // Culling can greatly improve performance and the number of triangles to be drawn.
        // However, when designing an application, it is better to turn culling off by putting these lines of code in the beginning of your Draw method:
        RasterizerState rs = new RasterizerState();
        rs.CullMode = CullMode.None;
        rs.FillMode = FillMode.WireFrame;
        _device.RasterizerState = rs;
#endif        
        _device.Clear(Color.DarkSlateBlue);


        _effect.CurrentTechnique = _effect.Techniques["ColoredNoShading"];
        _effect.Parameters["xView"].SetValue(_viewMatrix);
        _effect.Parameters["xProjection"].SetValue(_projectionMatrix);
        // Matrix worldMatrix = Matrix.CreateRotationY(3 * _angle);
        // Matrix worldMatrix = Matrix.CreateTranslation(-20.0f/3.0f, -10.0f / 3.0f, 0) * Matrix.CreateRotationY(_angle);
        Vector3 rotAxis = new Vector3(3*_angle, _angle, 2*_angle);
        rotAxis.Normalize();
        Matrix worldMatrix = Matrix.Identity;
        _effect.Parameters["xWorld"].SetValue(worldMatrix);
        foreach (EffectPass pass in _effect.CurrentTechnique.Passes)
        {
            pass.Apply();
            _device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, _vertices, 0, _vertices.Length, _indices, 0, _indices.Length / 3, VertexPositionColor.VertexDeclaration);
        }

        base.Draw(gameTime);
    }
}