using System;
using System.Collections.Generic;
using System.Globalization;
using ChickenNuggetSimulator.Core.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using State;
using static System.Net.Mime.MediaTypeNames;

namespace ChickenNuggetSimulator.Core;

/// <summary>
/// The main class for the game, responsible for managing game components, settings,
/// and platform-specific configurations.
/// </summary>
public class ChickenNuggetSimulatorGame : Setup
{
    public Chicken chicken;
    public Nuggets nuggets = new Nuggets();

    public SpriteFont Font;

    private Texture2D background;

    public Input input;

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

        input = new Input();
    }

    /// <summary>
    /// Loads game content, such as textures and particle systems.
    /// </summary>
    protected override void LoadContent()
    {
        base.LoadContent();
        chicken = new Chicken(this) { Position = new Vector2(Width * 0.5f, Height * 0.5f) };
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
        input.Update();

        chicken.Update();

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
        input.Draw();

        // end
        SpriteBatch.End();

        SpriteBatch.Begin(samplerState: SamplerState.LinearClamp);
        SpriteBatch.DrawString(
            Font, // font
            $"Nuggets: {nuggets.Count}", // text
            new Vector2(
                GraphicsDevice.Viewport.TitleSafeArea.X,
                GraphicsDevice.Viewport.TitleSafeArea.Y
            ), // position
            Color.Black // color
        );
        SpriteBatch.End();
        base.Draw(gameTime);
    }
}
