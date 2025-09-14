using System;
using System.Collections.Generic;
using System.Globalization;
using ChickenNuggetSimulator.Core.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using static System.Net.Mime.MediaTypeNames;

namespace ChickenNuggetSimulator.Core
{
    /// <summary>
    /// The main class for the game, responsible for managing game components, settings,
    /// and platform-specific configurations.
    /// </summary>
    public class ChickenNuggetSimulatorGame : Setup
    {
        private Texture2D chicken;

        /// <summary>
        /// Indicates if the game is running on a mobile platform.
        /// </summary>
        public readonly static bool IsMobile =
            OperatingSystem.IsAndroid() || OperatingSystem.IsIOS();

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
            : base("Chicken Nugget Simulator", 360, 640, false, DisplayOrientation.Portrait) { }

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
            chicken = Content.Load<Texture2D>("Assets/Chicken/chichen");
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
            // Clears the screen with the MonoGame orange color before drawing.
            GraphicsDevice.Clear(Color.Green);

            // Begin the sprite batch to prepare for rendering.
            SpriteBatch.Begin();

            // Draw the chicken texture
            SpriteBatch.Draw(chicken, Vector2.Zero, Color.White);

            // Always end the sprite batch when finished.
            SpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
