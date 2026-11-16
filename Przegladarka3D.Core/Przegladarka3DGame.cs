using System;
using Przegladarka3D.Core.Localization;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Przegladarka3D.Core.Scene;
using Przegladarka3D.Core.Gui;

namespace Przegladarka3D.Core
{
    public class Przegladarka3DGame : Game
    {
        // globalnie do testu jak to dziala
        GraphicsDeviceManager graphicsDeviceManager;
        SceneManager _scenemgr;
        InputManager _inputmgr;
        Entity bryla;

        public Przegladarka3DGame()
        {

            graphicsDeviceManager = new GraphicsDeviceManager(this);
            Services.AddService(typeof(GraphicsDeviceManager), graphicsDeviceManager);
            Content.RootDirectory = "Content";
            // Configure screen orientations.
            graphicsDeviceManager.SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;
            _inputmgr = new();
        }

        /// <summary>
        /// Initializes the game, including setting up localization and adding the 
        /// initial screens to the ScreenManager.
        /// </summary>
        protected override void Initialize()
        {
            this.Window.AllowUserResizing = true;
            _scenemgr = new(graphicsDeviceManager.GraphicsDevice);

            List<CultureInfo> cultures = LocalizationManager.GetSupportedCultures();
            var languages = new List<CultureInfo>();
            for (int i = 0; i < cultures.Count; i++)
            {
                languages.Add(cultures[i]);
            }

            var selectedLanguage = LocalizationManager.DEFAULT_CULTURE_CODE;
            LocalizationManager.SetCulture(selectedLanguage);
            
            _scenemgr.Init();
            base.Initialize();
        }

        /// <summary>
        /// Loads game content, such as textures and particle systems.
        /// </summary>
        protected override void LoadContent()
        {
            bryla = new("", _scenemgr._konwerter);
            _scenemgr.Entitytest.Add(bryla);
            base.LoadContent();
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
                || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
             // TODO: Add your update logic here
            _inputmgr.Update(gameTime);
            _scenemgr.Update(_inputmgr);
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
        _scenemgr.Draw();
         base.Draw(gameTime);
        }
    }
}
