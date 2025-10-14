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
        _vertices = new VertexPositionColor[3];

        _vertices[0].Position = new Vector3(-0.5f, -0.5f, 0f);
        _vertices[0].Color = Color.Red;
        _vertices[1].Position = new Vector3(0, 0.5f, 0f);
        _vertices[1].Color = Color.Green;
        _vertices[2].Position = new Vector3(0.5f, -0.5f, 0f);
        _vertices[2].Color = Color.Yellow;
    }    
    
    protected override void LoadContent()
    {
        _device = _graphics.GraphicsDevice;

        _effect = Content.Load<Effect>("effects");        
        SetUpVertices();
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();



        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        _device.Clear(Color.DarkSlateBlue);


        _effect.CurrentTechnique = _effect.Techniques["Pretransformed"];
        foreach (EffectPass pass in _effect.CurrentTechnique.Passes)
        {
            pass.Apply();
            _device.DrawUserPrimitives(PrimitiveType.TriangleList, _vertices, 0, 1, VertexPositionColor.VertexDeclaration);
        }

        base.Draw(gameTime);
    }
}