using System;
using Przegladarka3D.Core.Localization;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using static System.Net.Mime.MediaTypeNames;

namespace Przegladarka3D.Core
{
    /// <summary>
    /// The main class for the game, responsible for managing game components, settings, 
    /// and platform-specific configurations.
    /// </summary>
    public class Przegladarka3DGame : Game
    {
        // Resources for drawing.
        // globalnie do testu jak to dziala
        public GraphicsDeviceManager graphicsDeviceManager;
        Vector3 camTarget;
        Vector3 camPosition;
        Matrix projectionMatrix; //Focal Plane tzw
        Matrix viewMatrix; // faktyczna "pozycja" kamery
        Matrix worldMatrix;
        BasicEffect basicEffect; //chuj wie do czego ustawiasz alpha i wlasciwosici renderowania ale po co ? Basic effect = upo shader
        VertexPositionColor[] trojkatBermudzki;
        VertexBuffer vertexBuffer;
        


        /// <summary>
        /// Indicates if the game is running on a mobile platform.
        /// </summary>
        public readonly static bool IsMobile = OperatingSystem.IsAndroid() || OperatingSystem.IsIOS();

        /// <summary>
        /// Indicates if the game is running on a desktop platform.
        /// </summary>
        public readonly static bool IsDesktop = OperatingSystem.IsMacOS() || OperatingSystem.IsLinux() || OperatingSystem.IsWindows();

        /// <summary>
        /// Initializes a new instance of the game. Configures platform-specific settings, 
        /// initializes services like settings and leaderboard managers, and sets up the 
        /// screen manager for screen transitions.
        /// </summary>
        public Przegladarka3DGame()
        {
            graphicsDeviceManager = new GraphicsDeviceManager(this);

            // Share GraphicsDeviceManager as a service.
            Services.AddService(typeof(GraphicsDeviceManager), graphicsDeviceManager);

            Content.RootDirectory = "Content";

            // Configure screen orientations.
            graphicsDeviceManager.SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;
        }

        /// <summary>
        /// Initializes the game, including setting up localization and adding the 
        /// initial screens to the ScreenManager.
        /// </summary>
        protected override void Initialize()
        {
            
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
            

            camTarget = new Vector3(0f,0f,0f);
            camPosition = new Vector3(0f,0f,100f);

            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f),GraphicsDevice.DisplayMode.AspectRatio,1f,1000f);

            viewMatrix = Matrix.CreateLookAt(camPosition,camTarget,Vector3.Up);
            worldMatrix = Matrix.CreateWorld(camTarget, Vector3.Forward, Vector3.Up);

            // shader monogame ? albo cos podobnego
            basicEffect = new BasicEffect(GraphicsDevice);
            basicEffect.Alpha = 1.0f;
            basicEffect.VertexColorEnabled = true;
            basicEffect.LightingEnabled = false;

            trojkatBermudzki = new VertexPositionColor[3];            
            trojkatBermudzki[0] = new VertexPositionColor(new Vector3(0,10,0), Color.Red);
            trojkatBermudzki[1] = new VertexPositionColor(new Vector3(20,-10,0), Color.Green);
            trojkatBermudzki[2] = new VertexPositionColor(new Vector3(-20,-10,0), Color.Blue);

            vertexBuffer = new VertexBuffer(GraphicsDevice, typeof(VertexPositionColor),3, BufferUsage.WriteOnly);
            vertexBuffer.SetData<VertexPositionColor>(trojkatBermudzki);
            base.Initialize();
        }

        /// <summary>
        /// Loads game content, such as textures and particle systems.
        /// </summary>
        protected override void LoadContent()
        {
            
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
            Matrix obrot = Matrix.CreateRotationY(MathHelper.ToRadians(0.5f));
            camPosition = Vector3.Transform(camPosition, obrot);

            viewMatrix = Matrix.CreateLookAt(camPosition,camTarget, Vector3.Up);
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
            basicEffect.Projection = projectionMatrix;
            basicEffect.View = viewMatrix;
            basicEffect.World = worldMatrix;
            
            GraphicsDevice.Clear(Color.White);
            
            RasterizerState rasterizerState1 = new RasterizerState();
            rasterizerState1.CullMode = CullMode.None;
            GraphicsDevice.RasterizerState = rasterizerState1;
            GraphicsDevice.SetVertexBuffer(vertexBuffer);

            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
            pass.Apply();
            GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList,0,3);
            }
         base.Draw(gameTime);
        }
    }
}
