using System;
using System.Collections.Generic;
using System.Globalization;
using ChickenNuggetSimulator.Core.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using MonoGame.Extended;
using State;

namespace ChickenNuggetSimulator.Core;

public static class SafeArea
{
    // Set this from platform projects
    public static Func<(int Left, int Top, int Right, int Bottom)> GetInsets;
}

public class Insets
{
    public int Left;
    public int Top;
    public int Right;
    public int Bottom;
}

/// <summary>
/// The main class for the game, responsible for managing game components, settings,
/// and platform-specific configurations.
/// </summary>
public class ChickenNuggetSimulatorGame : Game
{
    // The desired resolution.
    public readonly int Width = 1080;
    public readonly int Height = 1920;

    public Insets Insets = new Insets
    {
        Left = 0,
        Top = 0,
        Right = 0,
        Bottom = 0,
    };

    private bool letterboxed = false;

    public static Matrix screenScaleMatrix;
    public static Matrix inverseScreenScaleMatrix;

    private bool isResizing = false;
    public SaveSystem saveSystem;

    /// <summary>
    /// Indicates if the game is running on a mobile platform.
    /// </summary>
    public readonly static bool IsMobile = OperatingSystem.IsAndroid() || OperatingSystem.IsIOS();

    /// <summary>
    /// Indicates if the game is running on a desktop platform.
    /// </summary>
    public readonly static bool IsDesktop =
        OperatingSystem.IsMacOS() || OperatingSystem.IsLinux() || OperatingSystem.IsWindows();
    internal static ChickenNuggetSimulatorGame s_instance;

    /// <summary>
    /// Gets a reference to the Setup instance.
    /// </summary>
    public static ChickenNuggetSimulatorGame Instance => s_instance;

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
    public Chicken chicken;
    public Nuggets nuggets;

    public SpriteFont Font;

    private Texture2D background;

    public bool firstFrame = true;

    public Input input;

    /// <summary>
    /// Initializes a new instance of the game. Configures platform-specific settings,
    /// initializes services like settings and leaderboard managers, and sets up the
    /// screen manager for screen transitions.
    /// </summary>
    public ChickenNuggetSimulatorGame()
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
        if (IsDesktop)
        {
            Graphics.PreferredBackBufferWidth = 540;
            Graphics.PreferredBackBufferHeight = 960;
        }
#if ANDROID
        Graphics.IsFullScreen = true;
        Graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
        Graphics.PreferredBackBufferHeight = GraphicsAdapter
            .DefaultAdapter
            .CurrentDisplayMode
            .Height;
#endif
        Graphics.SupportedOrientations = DisplayOrientation.Portrait;
        Graphics.IsFullScreen = false;
        Window.AllowUserResizing = true;

        Window.ClientSizeChanged += OnClientSizeChanged;

        // Apply the graphic presentation changes.

        Services.AddService(typeof(GraphicsDeviceManager), Graphics);
        Graphics.ApplyChanges();

        // Set the window title.
        Window.Title = "Chicken Nugget Simulator";

        // Set the core's content manager to a reference of the base Game's
        // content manager.
        Content = base.Content;

        // Set the root directory for content.
        Content.RootDirectory = "Content";

        // Mouse is visible by default.
        IsMouseVisible = true;

        saveSystem = new SaveSystem(this);

        Deactivated += (_, __) => saveSystem.Save();
    }

    /// <summary>
    /// Initializes the game, including setting up localization and adding the
    /// initial screens to the ScreenManager.
    /// </summary>
    protected override void Initialize()
    {
        base.Initialize();

        // Set the core's graphics device to a reference of the base Game's
        // graphics device.
        GraphicsDevice = base.GraphicsDevice;
        UpdateScreenScaleMatrix();

        // Create the sprite batch instance.
        SpriteBatch = new SpriteBatch(GraphicsDevice);

        // Load supported languages and set the default language.
        List<CultureInfo> cultures = LocalizationManager.GetSupportedCultures();
        var languages = new List<CultureInfo>();
        for (int i = 0; i < cultures.Count; i++)
        {
            languages.Add(cultures[i]);
        }

        // TODO You should load this from a settings file or similar,
        // based on what the user or operating system selected.
        var selectedLanguage = LocalizationManager.DEFAULT_CULTURE_CODE;
        LocalizationManager.SetCulture(selectedLanguage);

        input = new Input();
    }

    /// <summary>
    /// Loads game content, such as textures and particle systems.
    /// </summary>
    protected override void LoadContent()
    {
        base.LoadContent();
        chicken = new Chicken(this) { Position = new Vector2(Width * 0.5f, Height * 0.5f) };
        nuggets = new Nuggets(this);
        background = Utils.MakeTexture("Assets/background");
        Font = Content.Load<SpriteFont>("Fonts/Hud");
    }

    /// <summary>
    /// Updates the game's logic, called once per frame.
    /// </summary>
    /// <param name="gameTime">
    /// Provides a snapshot of timing values used for game updates.
    /// </param>
    protected override void Update(GameTime gameTime)
    {
        // Exit the game if the Back button (GamePad) or Escape key (Keyboard) is pressed.
        if (
            GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
            || Keyboard.GetState().IsKeyDown(Keys.Escape)
        )
            Exit();

        // TODO: Add your update logic here
        input.Update(this);

        chicken.Update();

        if (firstFrame)
        {
            firstFrame = false;
            UpdateScreenScaleMatrix();
        }

        // Console.WriteLine($"clicking? {input.JustTouched} {input.IsTouching}");

        base.Update(gameTime);
    }

    /// <summary>
    /// Draws the game's graphics, called once per frame.
    /// </summary>
    /// <param name="gameTime">
    /// Provides a snapshot of timing values used for rendering.
    /// </param>
    protected override void Draw(GameTime gameTime)
    {
        // setup
        GraphicsDevice.Clear(Color.Green);
        SpriteBatch.Begin(
            transformMatrix: screenScaleMatrix,
            samplerState: SamplerState.LinearClamp
        );

        SpriteBatch.Draw(
            texture: background,
            position: Vector2.Zero,
            sourceRectangle: null,
            color: Color.White,
            rotation: 0.0f,
            origin: new Vector2(100, 50),
            scale: 1.0f,
            effects: SpriteEffects.None,
            layerDepth: 0.0f
        );

        Utils.DrawChicken(chicken);
        input.Draw(this);

        SpriteBatch.DrawString(
            Font, // font
            $"Nuggets: {nuggets.Count}", // text
            new Vector2(Insets.Left + 20, Insets.Top + 20), // position
            Color.Black // color
        );

        // end
        SpriteBatch.End();
        base.Draw(gameTime);
    }

    private void OnClientSizeChanged(object sender, EventArgs e)
    {
        if (
            Window.ClientBounds.Width > 0
            && Window.ClientBounds.Height > 0
            && GraphicsDevice != null
        )
        {
            if (!isResizing)
            {
                isResizing = true;
                UpdateScreenScaleMatrix();
                isResizing = false;
            }
        }
    }

    public void UpdateScreenScaleMatrix()
    {
        if (OperatingSystem.IsAndroid())
        {
            Graphics.PreferredBackBufferWidth = GraphicsAdapter
                .DefaultAdapter
                .CurrentDisplayMode
                .Width;
            Graphics.PreferredBackBufferHeight = GraphicsAdapter
                .DefaultAdapter
                .CurrentDisplayMode
                .Height;
            Graphics.ApplyChanges();
        }
        float screenWidth = GraphicsDevice.PresentationParameters.BackBufferWidth;
        float screenHeight = GraphicsDevice.PresentationParameters.BackBufferHeight;

        float scaleX = screenWidth / Width; // how much we’d scale to match width
        float scaleY = screenHeight / Height; // how much we’d scale to match height

        // contain = Min, cover = Max
        float scale = letterboxed ? MathF.Min(scaleX, scaleY) : MathF.Max(scaleX, scaleY);

        int virtualX = (int)MathF.Round((screenWidth - Width * scale) * 0.5f);
        int virtualY = (int)MathF.Round((screenHeight - Height * scale) * 0.5f);

        // Uniform scale matrix
        screenScaleMatrix =
            Matrix.CreateScale(scale) * Matrix.CreateTranslation(virtualX, virtualY, 0);
        inverseScreenScaleMatrix = Matrix.Invert(screenScaleMatrix);

        TouchPanel.DisplayWidth = GraphicsDevice.PresentationParameters.BackBufferWidth;
        TouchPanel.DisplayHeight = GraphicsDevice.PresentationParameters.BackBufferHeight;
        TouchPanel.DisplayOrientation = GraphicsDevice.PresentationParameters.DisplayOrientation;

        (int left, int top, int right, int bottom) = IsMobile ? SafeArea.GetInsets() : (0, 0, 0, 0);

        int viewW = (int)MathF.Round(Width * scale);
        int viewH = (int)MathF.Round(Height * scale);
        // Usable screen bounds after OS insets (still in *screen* pixels)
        int usableRight = (int)screenWidth - right;
        int usableBottom = (int)screenHeight - bottom;

        // Convert to *virtual* distances (this naturally includes cover-cropping)
        float inv = 1f / scale;

        int padLeft = Math.Max(0, (int)MathF.Ceiling((left - virtualX) * inv));
        int padTop = Math.Max(0, (int)MathF.Ceiling((top - virtualY) * inv));
        int padRight = Math.Max(0, (int)MathF.Ceiling((virtualX + viewW - usableRight) * inv));
        int padBottom = Math.Max(0, (int)MathF.Ceiling((virtualY + viewH - usableBottom) * inv));

        // Safety clamp so pads never exceed your virtual size
        padLeft = Math.Min(padLeft, Width);
        padTop = Math.Min(padTop, Height);
        padRight = Math.Min(padRight, Width);
        padBottom = Math.Min(padBottom, Height);

        // ← Final distances you want (in *virtual* pixels)
        Insets = new Insets
        {
            Left = padLeft,
            Top = padTop,
            Right = padRight,
            Bottom = padBottom,
        };
    }
}
