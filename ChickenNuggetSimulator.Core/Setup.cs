using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

public class Setup : Game
{
    private int vHieght = 1920;
    private int vWidth = 1080;

    public Rectangle RenderDestination;

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

    public static RenderTarget2D RenderTarget { get; private set; }

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
        Graphics.PreferredBackBufferWidth = 480;
        Graphics.PreferredBackBufferHeight = 800;
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
        RenderTarget = new RenderTarget2D(GraphicsDevice, vWidth, vHieght);
        CalculateRenderDestination();

        // Create the sprite batch instance.
        SpriteBatch = new SpriteBatch(GraphicsDevice);
    }

    private void OnClientSizeChanged(object sender, EventArgs e)
    {
        if (
            !isResizing
            && GraphicsDevice != null
            && Window.ClientBounds.Width > 0
            && Window.ClientBounds.Height > 0
        )
        {
            isResizing = true;
            CalculateRenderDestination();
            isResizing = false;
        }
    }

    private void CalculateRenderDestination()
    {
        Point size = GraphicsDevice.Viewport.Bounds.Size;
        float scaleX = (float)size.X / vWidth;
        float scaleY = (float)size.Y / vHieght;
        float scale = Math.Max(scaleX, scaleY);

        RenderDestination.Width = (int)(vWidth * scale);
        RenderDestination.Height = (int)(vHieght * scale);
        RenderDestination.X = (size.X - RenderDestination.Width) / 2;
        RenderDestination.Y = (size.Y - RenderDestination.Height) / 2;
    }
}
