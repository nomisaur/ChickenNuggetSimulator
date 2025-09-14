using System;
using System.Collections.Generic;
using System.Globalization;
using ChickenNuggetSimulator.Core.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using static System.Net.Mime.MediaTypeNames;

namespace ChickenNuggetSimulator.Core;

/// <summary>
/// The main class for the game, responsible for managing game components, settings,
/// and platform-specific configurations.
/// </summary>
public class ChickenNuggetSimulatorGame : Setup
{
    private Texture2D chicken;
    private Texture2D background;

    /// <summary>
    /// Indicates if the game is running on a mobile platform.
    /// </summary>
    public readonly static bool IsMobile = OperatingSystem.IsAndroid() || OperatingSystem.IsIOS();

    /// <summary>
    /// Indicates if the game is running on a desktop platform.
    /// </summary>
    public readonly static bool IsDesktop =
        OperatingSystem.IsMacOS() || OperatingSystem.IsLinux() || OperatingSystem.IsWindows();

    /// <summary>
    /// Initializes a new instance of the game. Configures platform-specific settings,
    /// initializes services like settings and leaderboard managers, and sets up the
    /// screen manager for screen transitions.
    /// </summary>
    public ChickenNuggetSimulatorGame()
        : base("Chicken Nugget Simulator", false, DisplayOrientation.Portrait) { }

    /// <summary>
    /// Initializes the game, including setting up localization and adding the
    /// initial screens to the ScreenManager.
    /// </summary>
    protected override void Initialize()
    {
        base.Initialize();

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
    }

    /// <summary>
    /// Loads game content, such as textures and particle systems.
    /// </summary>
    protected override void LoadContent()
    {
        base.LoadContent();
        chicken = Content.Load<Texture2D>("Assets/Chicken/chicken");
        background = Content.Load<Texture2D>("Assets/Chicken/coop");
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
        GraphicsDevice.Viewport = Viewport;
        SpriteBatch.Begin(
            transformMatrix: screenScaleMatrix,
            samplerState: SamplerState.LinearClamp
        );

        // Draw here
        SpriteBatch.Draw(
            background,
            Vector2.Zero,
            new Rectangle(200, 200, 540, 960),
            Color.White,
            0.0f,
            Vector2.Zero,
            4.0f,
            SpriteEffects.None,
            0.0f
        );

        SpriteBatch.Draw(
            chicken, // texture
            new Vector2( // position
                (Width * 0.5f) - (chicken.Width * 0.5f * 0.5f),
                (Height * 0.5f) - (chicken.Height * 0.5f * 0.5f)
            ),
            null, // sourceRectangle
            Color.White, // color
            0.0f, // rotation
            Vector2.Zero, // origin
            0.5f, // scale
            SpriteEffects.None, // effects
            0.0f
        );

        // end
        SpriteBatch.End();
        // GraphicsDevice.SetRenderTarget(null);
        // SpriteBatch.Begin(samplerState: SamplerState.PointClamp);
        // SpriteBatch.Draw(RenderTarget, RenderDestination, Color.White);
        // SpriteBatch.End();
        base.Draw(gameTime);
    }
}
