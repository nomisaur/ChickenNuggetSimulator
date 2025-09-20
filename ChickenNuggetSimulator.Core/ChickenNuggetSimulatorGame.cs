using System.Collections.Generic;
using System.Globalization;
using ChickenNuggetSimulator.Core.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using State;

namespace ChickenNuggetSimulator.Core;

/// <summary>
/// The main class for the game, responsible for managing game components, settings,
/// and platform-specific configurations.
/// </summary>
public class CNS : Game
{
    public GraphicsDeviceManager Graphics { get; private set; }
    public new GraphicsDevice GraphicsDevice { get; private set; }
    public SpriteBatch SpriteBatch { get; private set; }
    public new ContentManager Content { get; private set; }
    public Screen Screen;
    public SaveSystem saveSystem;
    public Input input;
    public bool firstFrame = true;

    public Chicken chicken;
    public Nuggets nuggets;

    public SpriteFont Font;

    private Texture2D background;

    /// <summary>
    /// Initializes a new instance of the game. Configures platform-specific settings,
    /// initializes services like settings and leaderboard managers, and sets up the
    /// screen manager for screen transitions.
    /// </summary>
    public CNS()
    {
        // Create a new graphics device manager.
        Graphics = new GraphicsDeviceManager(this);

        Screen = new Screen(this, targetWidth: 1080, targetHeight: 1920, letterboxed: false);

        Screen.SetupScreen(desktopWidth: 540, desktopHeight: 960);

        Content = base.Content;

        Content.RootDirectory = "Content";

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

        Screen.UpdateScreenScaleMatrix();

        input = new Input(this);
    }

    /// <summary>
    /// Loads game content, such as textures and particle systems.
    /// </summary>
    protected override void LoadContent()
    {
        base.LoadContent();
        chicken = new Chicken(this)
        {
            Position = new Vector2(Screen.Width * 0.5f, Screen.Height * 0.5f),
        };
        nuggets = new Nuggets(this);
        background = Content.Load<Texture2D>("Assets/background");
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

        input.Update();

        chicken.Update();

        if (firstFrame)
        {
            firstFrame = false;
            Screen.UpdateScreenScaleMatrix();
        }

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
            transformMatrix: Screen.screenScaleMatrix,
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

        chicken.Draw();
        input.Draw();

        SpriteBatch.DrawString(
            Font, // font
            $"Nuggets: {nuggets.Count}", // text
            new Vector2(Screen.Insets.Left + 20, Screen.Insets.Top + 20), // position
            Color.Black // color
        );

        // end
        SpriteBatch.End();
        base.Draw(gameTime);
    }
}
