using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Mono3D;



public class Game1 : Game
{
    public struct VertexPositionColorNormal
    {
        public Vector3 Position;
        public Color Color;
        public Vector3 Normal;

        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(sizeof(float) * 3, VertexElementFormat.Color, VertexElementUsage.Color, 0),
            new VertexElement(sizeof(float) * 3 + 4, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0)
        );
    }
    
    // Properties
    private GraphicsDeviceManager _graphics;
    private GraphicsDevice _device;
    private Effect _effect;
    private VertexPositionColorNormal[] _vertices;
    private short[] _indices;
    private Matrix _viewMatrix;
    private Matrix _projectionMatrix;
    private float _angle = 0f;
    private int _terrainWidth = 4;
    private int _terrainHeight = 3;
    private float[,] _heightData;
    private VertexBuffer _myVertexBuffer;
    private IndexBuffer _myIndexBuffer;
    
    // Toggles
    private bool _isEnableWireFrame = false;
    private bool _isEnableLighting = true;

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
        
        _vertices = new VertexPositionColorNormal[_terrainWidth * _terrainHeight];
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
                    _vertices[x + y * _terrainWidth].Color = Color.Green;
                }
                else
                {
                    _vertices[x + y * _terrainWidth].Color = Color.Green;
                }
            }
        }
    }

    private void SetUpIndices()
    {
        _indices = new short[(_terrainWidth - 1) * (_terrainHeight - 1) * 6];
        int counter = 0;
        for (short y = 0; y < _terrainHeight - 1; y++)
        {
            for (short x = 0; x < _terrainWidth - 1; x++)
            {
                short lowerLeft = (short)(x + y * _terrainWidth);
                short lowerRight = (short) ((x + 1) + y * _terrainWidth);
                short topLeft = (short)(x + (y + 1) * _terrainWidth);
                short topRight = (short)((x + 1) + (y + 1) * _terrainWidth);

                _indices[counter++] = topLeft;
                _indices[counter++] = lowerRight;
                _indices[counter++] = lowerLeft;
                
                _indices[counter++] = topLeft;
                _indices[counter++] = topRight;
                _indices[counter++] = lowerRight;
            }
        }
    }    
    
    private void CopyToBuffers()
    {
        _myVertexBuffer = new VertexBuffer(_device, VertexPositionColorNormal.VertexDeclaration, _vertices.Length, BufferUsage.WriteOnly);
        _myVertexBuffer.SetData(_vertices);

        _myIndexBuffer = new IndexBuffer(_device, typeof(short), _indices.Length, BufferUsage.WriteOnly);
        _myIndexBuffer.SetData(_indices);
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
        CalculateNormals();
        CopyToBuffers();
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
#if DEBUG
        if (keyState.IsKeyDown(Keys.G) && !previousKeyboardState.IsKeyDown(Keys.G))
        {
            _isEnableWireFrame = !_isEnableWireFrame;
        }
        if (keyState.IsKeyDown(Keys.L) && !previousKeyboardState.IsKeyDown(Keys.L))
        {
            _isEnableLighting = !_isEnableLighting;
        }
#endif
        previousKeyboardState = keyState;
        base.Update(gameTime);
    }

    private void CalculateNormals()
    {
        for (int i = 0; i < _vertices.Length; i++)
        {
            _vertices[i].Normal = Vector3.Zero;
        }
        for (int i = 0; i < _indices.Length / 3; i++)
        {
            int index1 = _indices[i * 3];
            int index2 = _indices[i * 3 + 1];
            int index3 = _indices[i * 3 + 2];

            Vector3 side1 = _vertices[index1].Position - _vertices[index3].Position;
            Vector3 side2 = _vertices[index1].Position - _vertices[index2].Position;
            Vector3 normal = Vector3.Cross(side1, side2);

            _vertices[index1].Normal += normal;
            _vertices[index2].Normal += normal;
            _vertices[index3].Normal += normal;
        }
        for (int i = 0; i < _vertices.Length; i++)
        {
            _vertices[i].Normal.Normalize();
        }
    }
    
    protected override void Draw(GameTime gameTime)
    {
#if DEBUG
        // Culling can greatly improve performance and the number of triangles to be drawn.
        // However, when designing an application, it is better to turn culling off by putting these lines of code in the beginning of your Draw method:
        RasterizerState rs = new RasterizerState();
        rs.CullMode = CullMode.None;
        rs.FillMode = _isEnableWireFrame ? FillMode.WireFrame : FillMode.Solid; // .Solid . WireFrame
        _device.RasterizerState = rs;
#endif        
        _device.Clear(ClearOptions.Target|ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);


        Matrix worldMatrix = Matrix.CreateTranslation(-_terrainWidth / 2.0f, 0, _terrainHeight / 2.0f) * Matrix.CreateRotationY(_angle);
        _effect.CurrentTechnique = _effect.Techniques["Colored"];
        _effect.Parameters["xView"].SetValue(_viewMatrix);
        _effect.Parameters["xProjection"].SetValue(_projectionMatrix);
        _effect.Parameters["xWorld"].SetValue(worldMatrix);
        // Matrix worldMatrix = Matrix.CreateRotationY(3 * _angle);
        // Matrix worldMatrix = Matrix.CreateTranslation(-20.0f/3.0f, -10.0f / 3.0f, 0) * Matrix.CreateRotationY(_angle);
        Vector3 rotAxis = new Vector3(3*_angle, _angle, 2*_angle);
        rotAxis.Normalize();

        Vector3 lightDirection = new Vector3(1.0f, -1.0f, -1.0f);
        lightDirection.Normalize();
        _effect.Parameters["xLightDirection"].SetValue(lightDirection);
        _effect.Parameters["xAmbient"].SetValue(0.1f);
        _effect.Parameters["xEnableLighting"].SetValue(true);


        foreach (EffectPass pass in _effect.CurrentTechnique.Passes)
        {
            pass.Apply();
            // _device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, _vertices, 0, _vertices.Length, _indices, 0, _indices.Length / 3, VertexPositionColorNormal.VertexDeclaration);
            _device.Indices = _myIndexBuffer;
            _device.SetVertexBuffer(_myVertexBuffer);
            _device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, _vertices.Length, 0, _indices.Length / 3);

        }

        base.Draw(gameTime);
    }
}