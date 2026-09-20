using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Bastion.Presentation.Utils;
using Bastion.Resources;

namespace Bastion.Presentation;

public sealed class BastionGame : Game
{
    private readonly GraphicsDeviceManager _graphics;
    private readonly InputState _input = new();

    private SpriteBatch? _batch;
    private ShapeRenderer? _shapes;
    private Canvas? _canvas;
    private Navigator? _navigator;

    public BastionGame()
    {
        _graphics = new GraphicsDeviceManager(this)
        {
            PreferredBackBufferWidth = Theme.WindowWidth,
            PreferredBackBufferHeight = Theme.WindowHeight,
            SynchronizeWithVerticalRetrace = true
        };

        // Set before the first catalog lookup, or the interface starts in the
        // operating system language, which may be neither supported one.
        Language.Apply(Language.Default);

        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        Window.Title = TextCatalog.WindowTitle;
        Window.AllowUserResizing = false;
    }

    protected override void Initialize()
    {
        Window.TextInput += OnTextInput;
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _batch = new SpriteBatch(GraphicsDevice);
        _shapes = new ShapeRenderer(GraphicsDevice, _batch);

        _canvas = new Canvas
        {
            Shapes = _shapes,
            Text = new TextRenderer(_batch),
            Fonts = new FontSet
            {
                Regular = Content.Load<SpriteFont>("Regular"),
                Bold = Content.Load<SpriteFont>("Bold"),
                Title = Content.Load<SpriteFont>("Title")
            }
        };

        _navigator = new Navigator();
        _navigator.Start(ScreenId.Login);
    }

    protected override void Update(GameTime gameTime)
    {
        _input.Update(gameTime);
        _navigator?.Update(_input);
        _input.ClearText();

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Theme.Background);

        if (_batch is not null && _canvas is not null && _navigator is not null)
        {
            _batch.Begin(samplerState: SamplerState.LinearClamp);
            _navigator.Draw(_canvas);
            _batch.End();
        }

        base.Draw(gameTime);
    }

    protected override void UnloadContent()
    {
        _shapes?.Dispose();
        _batch?.Dispose();

        base.UnloadContent();
    }

    // DesktopGL delivers characters already resolved by the system, with accents
    // and keyboard layout applied.
    private void OnTextInput(object? sender, TextInputEventArgs e)
    {
        _input.AddCharacter(e.Character);
    }
}
