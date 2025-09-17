using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

public class Setup : Game
{
    // The desired resolution.
    public readonly int Width = 1080;
    public readonly int Height = 1920;

    private bool letterboxed = false;

    public static Matrix screenScaleMatrix;
    public static Matrix inverseScreenScaleMatrix;
    public Viewport Viewport;

    private bool isResizing = false;

    /// <summary>
    /// Indicates if the game is running on a mobile platform.
    /// </summary>
    public readonly static bool IsMobile = OperatingSystem.IsAndroid() || OperatingSystem.IsIOS();

    /// <summary>
    /// Indicates if the game is running on a desktop platform.
    /// </summary>
    public readonly static bool IsDesktop =
        OperatingSystem.IsMacOS() || OperatingSystem.IsLinux() || OperatingSystem.IsWindows();
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

    void UpdateScreenScaleMatrix()
    {
        float screenWidth = GraphicsDevice.PresentationParameters.BackBufferWidth;
        float screenHeight = GraphicsDevice.PresentationParameters.BackBufferHeight;

        float scaleX = screenWidth / Width; // how much we’d scale to match width
        float scaleY = screenHeight / Height; // how much we’d scale to match height

        // contain = Min, cover = Max
        float scale = letterboxed ? MathF.Min(scaleX, scaleY) : MathF.Max(scaleX, scaleY);

        // Destination size in screen pixels (before rounding)
        float destinationWidth = Width * scale;
        float destinationHeight = Height * scale;

        // Centered destination rect (careful rounding)
        int virtualWidth = (int)MathF.Round(destinationWidth);
        int virtualHeight = (int)MathF.Round(destinationHeight);
        int virtualX = (int)MathF.Round((screenWidth - destinationWidth) * 0.5f);
        int virtualY = (int)MathF.Round((screenHeight - destinationHeight) * 0.5f);

        // Uniform scale matrix
        screenScaleMatrix = Matrix.CreateScale(scale);
        inverseScreenScaleMatrix = Matrix.Invert(screenScaleMatrix);
        // Viewport for centering/letterbox or full cover
        Viewport = new Viewport
        {
            X = virtualX,
            Y = virtualY,
            Width = virtualWidth,
            Height = virtualHeight,
            MinDepth = 0,
            MaxDepth = 1,
        };
    }
}
