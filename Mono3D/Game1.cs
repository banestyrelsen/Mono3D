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
    private int _terrainWidth = 4;
    private int _terrainHeight = 3;
    private float[,] _heightData;
    private bool _wireFrame = false;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        _graphics.PreferredBackBufferWidth = 1920;
        _graphics.PreferredBackBufferHeight = 1080;
        _graphics.IsFullScreen = false;
        _graphics.ApplyChanges();
        Window.Title = "Riemer's MonoGame Tutorials -- 3D Series 1";

        base.Initialize();
    }

    private void SetUpVertices()
    {
        float minHeight = float.MaxValue;
        float maxHeight = float.MinValue;
        for (int x = 0; x < _terrainWidth; x++)
        {
            for (int y = 0; y < _terrainHeight; y++)
            {
                if (_heightData[x, y] < minHeight)
                    minHeight = _heightData[x, y];
                if (_heightData[x, y] > maxHeight)
                    maxHeight = _heightData[x, y];
            }
        }
        
        _vertices = new VertexPositionColor[_terrainWidth * _terrainHeight];
        for (int x = 0; x < _terrainWidth; x++)
        {
            for (int y = 0; y < _terrainHeight; y++)
            {
                _vertices[x + y * _terrainWidth].Position = new Vector3(x, _heightData[x,y], -y);
                _vertices[x + y * _terrainWidth].Position = new Vector3(x, _heightData[x, y], -y);

                if (_heightData[x, y] < minHeight + (maxHeight - minHeight) / 4)
                {
                    _vertices[x + y * _terrainWidth].Color = Color.Blue;
                }
                else if (_heightData[x, y] < minHeight + (maxHeight - minHeight) * 2 / 4)
                {
                    _vertices[x + y * _terrainWidth].Color = Color.Green;
                }
                else if (_heightData[x, y] < minHeight + (maxHeight - minHeight) * 3 / 4)
                {
                    _vertices[x + y * _terrainWidth].Color = Color.Brown;
                }
                else
                {
                    _vertices[x + y * _terrainWidth].Color = Color.White;
                }
            }
        }
    }

    private void SetUpIndices()
    {
        _indices = new int[(_terrainWidth - 1) * (_terrainHeight - 1) * 6];
        int counter = 0;
        for (int y = 0; y < _terrainHeight - 1; y++)
        {
            for (int x = 0; x < _terrainWidth - 1; x++)
            {
                int lowerLeft = x + y * _terrainWidth;
                int lowerRight = (x + 1) + y * _terrainWidth;
                int topLeft = x + (y + 1) * _terrainWidth;
                int topRight = (x + 1) + (y + 1) * _terrainWidth;

                _indices[counter++] = topLeft;
                _indices[counter++] = lowerRight;
                _indices[counter++] = lowerLeft;
                
                _indices[counter++] = topLeft;
                _indices[counter++] = topRight;
                _indices[counter++] = lowerRight;
            }
        }
    }    
    
    private void SetUpCamera()
    {
        _viewMatrix = Matrix.CreateLookAt(new Vector3(60, 80, -80), new Vector3(0, 0, 0), new Vector3(0, 1, 0));
        _projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, _device.Viewport.AspectRatio, 1.0f, 300.0f);
    }    
    
    private void LoadHeightData(Texture2D heightMap)
    {
        _terrainWidth = heightMap.Width;
        _terrainHeight = heightMap.Height;

        Color[] heightMapColors = new Color[_terrainWidth * _terrainHeight];
        heightMap.GetData(heightMapColors);

        _heightData = new float[_terrainWidth, _terrainHeight];
        for (int x = 0; x < _terrainWidth; x++)
        {
            for (int y = 0; y < _terrainHeight; y++)
            {
                _heightData[x, y] = heightMapColors[x + y * _terrainWidth].R / 5.0f;
            }
        }
        
        // _heightData = new float[4, 3];
        // _heightData[0, 0] = 0;
        // _heightData[1, 0] = 0;
        // _heightData[2, 0] = 0;
        // _heightData[3, 0] = 0;
        //
        // _heightData[0, 1] = 0.5f;
        // _heightData[1, 1] = 0;
        // _heightData[2, 1] = -1.0f;
        // _heightData[3, 1] = 0.2f;
        //
        // _heightData[0, 2] = 1.0f;
        // _heightData[1, 2] = 1.2f;
        // _heightData[2, 2] = 0.8f;
        // _heightData[3, 2] = 0;
    }
    
    protected override void LoadContent()
    {
        _device = _graphics.GraphicsDevice;

        _effect = Content.Load<Effect>("effects");        
        SetUpCamera();

        Texture2D heightMap = Content.Load<Texture2D>("heightmap");
        LoadHeightData(heightMap);
        SetUpVertices();
        SetUpIndices();
    }


    KeyboardState previousKeyboardState = Keyboard.GetState();
    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        KeyboardState keyState = Keyboard.GetState();
        if (keyState.IsKeyDown(Keys.Q))
        {
            _angle += 0.05f;
        }
        if (keyState.IsKeyDown(Keys.E))
        {
            _angle -= 0.05f;
        }
        if (keyState.IsKeyDown(Keys.G) && !previousKeyboardState.IsKeyDown(Keys.G))
        {
            _wireFrame = !_wireFrame;
        }

        previousKeyboardState = keyState;
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
#if DEBUG
        // Culling can greatly improve performance and the number of triangles to be drawn.
        // However, when designing an application, it is better to turn culling off by putting these lines of code in the beginning of your Draw method:
        RasterizerState rs = new RasterizerState();
        rs.CullMode = CullMode.None;
        rs.FillMode = _wireFrame ? FillMode.WireFrame : FillMode.Solid; // .Solid . WireFrame
        _device.RasterizerState = rs;
#endif        
        _device.Clear(ClearOptions.Target|ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);


        _effect.CurrentTechnique = _effect.Techniques["ColoredNoShading"];
        _effect.Parameters["xView"].SetValue(_viewMatrix);
        _effect.Parameters["xProjection"].SetValue(_projectionMatrix);
        // Matrix worldMatrix = Matrix.CreateRotationY(3 * _angle);
        // Matrix worldMatrix = Matrix.CreateTranslation(-20.0f/3.0f, -10.0f / 3.0f, 0) * Matrix.CreateRotationY(_angle);
        Vector3 rotAxis = new Vector3(3*_angle, _angle, 2*_angle);
        rotAxis.Normalize();
        Matrix worldMatrix = Matrix.CreateTranslation(-_terrainWidth / 2.0f, 0, _terrainHeight / 2.0f) * Matrix.CreateRotationY(_angle);
        _effect.Parameters["xWorld"].SetValue(worldMatrix);
        foreach (EffectPass pass in _effect.CurrentTechnique.Passes)
        {
            pass.Apply();
            _device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, _vertices, 0, _vertices.Length, _indices, 0, _indices.Length / 3, VertexPositionColor.VertexDeclaration);
        }

        base.Draw(gameTime);
    }
}