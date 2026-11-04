using System;
using Przegladarka3D.Core.Localization;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using static System.Net.Mime.MediaTypeNames;
using System.IO;
using System.Text;

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
        VertexPositionColor[] szescianBermudzki;
        VertexBuffer vertexBuffer;
        short[] indeksyBermudzkie;        
        IndexBuffer indexbuffer;

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
        public void STLReader(){
            string file = "";
            
            if (File.Exists(file))
            {
                using (BinaryReader br = new BinaryReader(File.Open(file,FileMode.Open), Encoding.ASCII))
                {
                    br.ReadBytes(80);                    // Naglowek(syf)
                    UInt32 ntriangle = br.ReadUInt32();
                    Console.WriteLine("Liczba trojkatow:"+ ntriangle);
                    for(int i = 0; i < ntriangle; i++){
                        br.ReadBytes(12);
                        byte[] xraw = br.ReadBytes(12);
                        float[] xpos = {BitConverter.ToSingle(xraw,0),
                                        BitConverter.ToSingle(xraw,4),
                                        BitConverter.ToSingle(xraw,8)
                                       };                        
                        byte[] yraw = br.ReadBytes(12);
                        float[] ypos = {BitConverter.ToSingle(yraw,0),
                                        BitConverter.ToSingle(yraw,4),
                                        BitConverter.ToSingle(yraw,8)
                                       };
                        byte[] zraw = br.ReadBytes(12);
                        float[] zpos = {BitConverter.ToSingle(zraw,0),
                                        BitConverter.ToSingle(zraw,4),
                                        BitConverter.ToSingle(zraw,8)
                                       };
                    br.ReadBytes(2); // Atrybuty ?
                    Console.WriteLine("xpos:"+xpos[0]+" "+xpos[1]+" "+xpos[2]);
                    Console.WriteLine("ypos:"+ypos[0]+" "+ypos[1]+" "+ypos[2]);
                    Console.WriteLine("zpos:"+zpos[0]+" "+zpos[1]+" "+zpos[2]);
                    } 
                }

            }
        }


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

            szescianBermudzki= new VertexPositionColor[9];            
            szescianBermudzki[0] = new VertexPositionColor(new Vector3(-10,10,10), Color.Red);
            szescianBermudzki[1] = new VertexPositionColor(new Vector3(10,10,10), Color.Red);
            szescianBermudzki[2] = new VertexPositionColor(new Vector3(10,-10,10), Color.Blue);
            szescianBermudzki[3] = new VertexPositionColor(new Vector3(-10,-10,10), Color.Blue);
            szescianBermudzki[4] = new VertexPositionColor(new Vector3(-10,10,-10), Color.Green);
            szescianBermudzki[5] = new VertexPositionColor(new Vector3(10,10,-10), Color.Green);
            szescianBermudzki[6] = new VertexPositionColor(new Vector3(10,-10,-10), Color.Green);
            szescianBermudzki[7] = new VertexPositionColor(new Vector3(-10,-10,-10), Color.Green);

            vertexBuffer = new VertexBuffer(GraphicsDevice, typeof(VertexPositionColor),szescianBermudzki.Length, BufferUsage.WriteOnly);
            vertexBuffer.SetData<VertexPositionColor>(szescianBermudzki);
            
            indeksyBermudzkie = new short[]{
                0,1,2, //front
                0,3,2,

                4,5,6, // back
                4,7,6,
                
                0,1,4, // top
                1,4,5,

                1,5,6, // right
                1,2,6,

                3,2,6, 
                3,7,6, // bottom       

                0,4,7, // left
                0,3,7,
            };

            indexbuffer = new IndexBuffer(GraphicsDevice,IndexElementSize.SixteenBits,indeksyBermudzkie.Length, BufferUsage.WriteOnly);
            indexbuffer.SetData<short>(indeksyBermudzkie,0,indeksyBermudzkie.Length);            
            STLReader();
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
            Matrix obrot1 = Matrix.CreateRotationX(MathHelper.ToRadians(0.6f));
            camPosition = Vector3.Transform(camPosition, obrot);
            camPosition = Vector3.Transform(camPosition, obrot1);

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
            rasterizerState1.FillMode = FillMode.WireFrame;
            GraphicsDevice.RasterizerState = rasterizerState1;
            GraphicsDevice.SetVertexBuffer(vertexBuffer);
            GraphicsDevice.Indices = indexbuffer;

            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
            pass.Apply();
            GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList,0,0,12);
            }
         base.Draw(gameTime);
        }
    }
}
