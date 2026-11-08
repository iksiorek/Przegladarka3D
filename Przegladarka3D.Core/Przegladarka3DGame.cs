using System;
using Przegladarka3D.Core.Localization;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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
        VertexBuffer vertexBuffer;
        int[] indeksyBermudzkie;        
        IndexBuffer indexbuffer;

        public struct VertexPositionColorNormal
        {
            public Vector3 Position;
            public Color Color;
            public Vector3 Normal;
            // specjalna klasa do definicji ulozenia w pamieci VRAM GPU 
            public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
            (
                new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                new VertexElement(sizeof(float) * 3, VertexElementFormat.Color, VertexElementUsage.Color, 0),
                new VertexElement(sizeof(float) * 3 + 4, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0)
            );
        }
        VertexPositionColorNormal[] brylka;

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
        // TO DO: zoptymalizowac ilosc wierzcholkow
        public (Vector3[],int[],Vector3[]) STLReader(){
            string file = "";
            Vector3[] pos;
            UInt32 ntriangle;
            int[] indicies;
            if (File.Exists(file))
            {
                using (BinaryReader br = new BinaryReader(File.Open(file,FileMode.Open), Encoding.ASCII))
                {
                    br.ReadBytes(80);                    // Naglowek(syf)
                    ntriangle = br.ReadUInt32();
                    pos = new Vector3[ntriangle*3];
                    indicies = new int[ntriangle*3];
                    Vector3[] normals = new Vector3[ntriangle*3];
                    //Console.WriteLine("Liczba trojkatow:"+ ntriangle);
                    for(int i = 0; i < ntriangle; i++){
                        byte[] normalraw = br.ReadBytes(12); // normal do swiatla
                        float[] normal = {BitConverter.ToSingle(normalraw,0),
                                                                BitConverter.ToSingle(normalraw,4),
                                                                BitConverter.ToSingle(normalraw,8)
                                                               };
                        byte[] vertex1raw = br.ReadBytes(12);
                        float[] vertex1 = {BitConverter.ToSingle(vertex1raw,0),
                                        BitConverter.ToSingle(vertex1raw,4),
                                        BitConverter.ToSingle(vertex1raw,8)
                                       };
                        pos[i*3]= new Vector3(vertex1[0],vertex1[1],vertex1[2]);
                        indicies[i*3] = i*3;
                        normals[i*3] = new Vector3(normal[0],normal[1],normal[2]);

                        byte[] vertex2raw = br.ReadBytes(12);
                        float[] vertex2 = {BitConverter.ToSingle(vertex2raw,0),
                                        BitConverter.ToSingle(vertex2raw,4),
                                        BitConverter.ToSingle(vertex2raw,8)
                                       };
                        pos[i*3+1]= new Vector3(vertex2[0],vertex2[1],vertex2[2]);
                        indicies[i*3+1] = i*3+1;
                        normals[i*3+1] = new Vector3(normal[0],normal[1],normal[2]);

                        byte[] vertex3raw = br.ReadBytes(12);
                        float[] vertex3 = {BitConverter.ToSingle(vertex3raw,0),
                                        BitConverter.ToSingle(vertex3raw,4),
                                        BitConverter.ToSingle(vertex3raw,8)
                                       };
                        pos[i*3+2]= new Vector3(vertex3[0],vertex3[1],vertex3[2]);
                        indicies[i*3+2] = i*3+2;
                        normals[i*3+2] = new Vector3(normal[0],normal[1],normal[2]);

                    br.ReadBytes(2); // Atrybuty ?
                    } 
                    return (pos, indicies, normals);
                }

            }
            return (null,null,null);
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
            

            camTarget = new Vector3(-100,200f,0f);
            camPosition = new Vector3(0f,0f,300f);

            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f),GraphicsDevice.DisplayMode.AspectRatio,1f,1000f);

            viewMatrix = Matrix.CreateLookAt(camPosition,camTarget,Vector3.Up);
            worldMatrix = Matrix.CreateWorld(camTarget, Vector3.Forward, Vector3.Up);

            // shader monogame ? albo cos podobnego
            basicEffect = new BasicEffect(GraphicsDevice);
            basicEffect.Alpha = 1.0f;
            basicEffect.VertexColorEnabled = true;

            var model = STLReader();
            brylka = new VertexPositionColorNormal[model.Item1.Length];
            for(int i = 0; i < model.Item1.Length; i++){
                brylka[i].Color = Color.Aqua;
                brylka[i].Position = model.Item1[i];
            }
            for(int i = 0; i <model.Item3.Length;i++){

             brylka[i].Normal = model.Item3[i];
            }


            for(int i = 0; i < brylka.Length;i++){
                brylka[i].Normal.Normalize();
            }

            vertexBuffer = new VertexBuffer(GraphicsDevice, VertexPositionColorNormal.VertexDeclaration ,brylka.Length, BufferUsage.WriteOnly);
            vertexBuffer.SetData(brylka);
            
            indeksyBermudzkie = model.Item2;
            indexbuffer = new IndexBuffer(GraphicsDevice,IndexElementSize.ThirtyTwoBits,indeksyBermudzkie.Length, BufferUsage.WriteOnly);
            indexbuffer.SetData<int>(indeksyBermudzkie,0,indeksyBermudzkie.Length);            
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
            rasterizerState1.FillMode = FillMode.Solid;
            GraphicsDevice.RasterizerState = rasterizerState1;
            GraphicsDevice.SetVertexBuffer(vertexBuffer);
            GraphicsDevice.Indices = indexbuffer;

            Vector3 lightDirection = camPosition;
            lightDirection.Normalize();
            basicEffect.LightingEnabled = true;
            basicEffect.PreferPerPixelLighting = true;
            basicEffect.DirectionalLight0.Enabled = true;
            basicEffect.DirectionalLight0.Direction = lightDirection;
            basicEffect.DirectionalLight0.DiffuseColor = Color.White.ToVector3();
            basicEffect.AmbientLightColor = new Vector3(0.1f);

            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
            pass.Apply();
            GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList,0,0,(indeksyBermudzkie.Length/3));
            }
         base.Draw(gameTime);
        }
    }
}
