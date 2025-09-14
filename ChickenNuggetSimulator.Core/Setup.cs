using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

public class Setup : Game
{
    // The desired resolution.
    public readonly int Width = 1080;
    public readonly int Height = 1920;

    // The rendered resolution.
    public int vWidth = 1080;
    public int vHeight = 1920;

    private bool letterboxed = false;

    public Matrix screenScaleMatrix;
    public Viewport Viewport;

    private bool isResizing = false;
    internal static Setup s_instance;

    /// <summary>
    /// Gets a reference to the Setup instance.
    /// </summary>
    public static Setup Instance => s_instance;

    /// <summary>
    /// Gets the graphics device manager to control the presentation of graphics.
    /// </summary>
    public static GraphicsDeviceManager Graphics { get; private set; }

    /// <summary>
    /// Gets the graphics device used to create graphical resources and perform primitive rendering.
    /// </summary>
    public static new GraphicsDevice GraphicsDevice { get; private set; }

    /// <summary>
    /// Gets the sprite batch used for all 2D rendering.
    /// </summary>
    public static SpriteBatch SpriteBatch { get; private set; }

    /// <summary>
    /// Gets the content manager used to load global assets.
    /// </summary>
    public static new ContentManager Content { get; private set; }

    /// <summary>
    /// Creates a new Core instance.
    /// </summary>
    /// <param name="title">The title to display in the title bar of the game window.</param>
    /// <param name="width">The initial width, in pixels, of the game window.</param>
    /// <param name="height">The initial height, in pixels, of the game window.</param>
    /// <param name="fullScreen">Indicates if the game should start in fullscreen mode.</param>
    /// <param name="orientation">The initial orientation of the game window.</param>
    public Setup(string title, bool fullScreen, DisplayOrientation orientation)
    {
        // Ensure that multiple cores are not created.
        if (s_instance != null)
        {
            throw new InvalidOperationException($"Only a single Setup instance can be created");
        }

        // Store reference to engine for global member access.
        s_instance = this;

        // Create a new graphics device manager.
        Graphics = new GraphicsDeviceManager(this);

        // Set the graphics defaults.
        Graphics.PreferredBackBufferWidth = 540;
        Graphics.PreferredBackBufferHeight = 960;
        // Graphics.PreferredBackBufferWidth = Width;
        // Graphics.PreferredBackBufferHeight = Height;
        Graphics.SupportedOrientations = orientation;
        Graphics.IsFullScreen = fullScreen;
        Window.AllowUserResizing = true;

        Window.ClientSizeChanged += OnClientSizeChanged;

        // Apply the graphic presentation changes.

        Services.AddService(typeof(GraphicsDeviceManager), Graphics);
        Graphics.ApplyChanges();

        // Set the window title.
        Window.Title = title;

        // Set the core's content manager to a reference of the base Game's
        // content manager.
        Content = base.Content;

        // Set the root directory for content.
        Content.RootDirectory = "Content";

        // Mouse is visible by default.
        IsMouseVisible = true;
    }

    protected override void LoadContent()
    {
        base.LoadContent();
    }

    protected override void Initialize()
    {
        base.Initialize();

        // Set the core's graphics device to a reference of the base Game's
        // graphics device.
        GraphicsDevice = base.GraphicsDevice;
        UpdateScreenScaleMatrix();

        // Create the sprite batch instance.
        SpriteBatch = new SpriteBatch(GraphicsDevice);
    }

    private void OnClientSizeChanged(object sender, EventArgs e)
    {
        if (Window.ClientBounds.Width > 0 && Window.ClientBounds.Height > 0)
        {
            if (!isResizing)
            {
                isResizing = true;
                UpdateScreenScaleMatrix();
                isResizing = false;
            }
        }
    }

    private void UpdateScreenScaleMatrix()
    {
        // size of actual screen
        float screenWidth = GraphicsDevice.PresentationParameters.BackBufferWidth;
        float screenHeight = GraphicsDevice.PresentationParameters.BackBufferHeight;

        // calculate virtual resolution based on aspect ratio of actual screen
        if (
            letterboxed
                ? screenWidth / Width > screenHeight / Height
                : screenWidth / Width < screenHeight / Height
        )
        {
            float aspect = screenHeight / Height;
            vWidth = (int)(aspect * Width);
            vHeight = (int)screenHeight;
        }
        else
        {
            float aspect = screenWidth / Width;
            vWidth = (int)screenWidth;
            vHeight = (int)(aspect * Height);
        }

        screenScaleMatrix = Matrix.CreateScale(vWidth / (float)Width);

        Viewport = new Viewport
        {
            X = (int)(screenWidth / 2 - vWidth / 2),
            Y = (int)(screenHeight / 2 - vHeight / 2),
            Width = vWidth,
            Height = vHeight,
            MinDepth = 0,
            MaxDepth = 1,
        };
    }
}
