using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using TDGameLibrary;
using TDGameLibrary.XnaComponents;
using FarseerTools;
using FarseerPhysics;
using TDGameLibrary.Input;
using ShadowShootout.Input;
using TDGameLibrary.Utility;
using TDGameLibrary.Ui;
using TDGameLibrary.Mobs;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using TDGameLibrary.Animation;
using Krypton;
using Krypton.Lights;
using FarseerPhysics.Common;
using FarseerPhysics.Common.PolygonManipulation;
using FarseerPhysics.Common.Decomposition;
using System.Diagnostics;

namespace ShadowShootout
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        
        public Game1()
        {
            GraphicsDeviceManager = new GraphicsDeviceManager(this);
            GraphicsDeviceManager.PreferredBackBufferWidth = PreferredWidth;
            GraphicsDeviceManager.PreferredBackBufferHeight = PreferredHeight;
            GraphicsDeviceManager.IsFullScreen = FullScreen;

            Content.RootDirectory = "Content";

            GameEnvironment.StartServices(this, new ScreenManager(this), null, GraphicsDeviceManager);
            GameEnvironment.ScreenRectangle = new Rectangle(0, 0, PreferredWidth, PreferredHeight);

            KryptonEngine = new KryptonEngine(this, "KryptonEffect");

            Components.Add(new MousePointer(this));
        }

        private int PreferredWidth = 1920;
        private int PreferredHeight = 1080;
        private bool FullScreen = false;
        private GraphicsDeviceManager GraphicsDeviceManager;
        private FrameRateCounter FrameRateCounter;
        private Player Player;
        private KryptonEngine KryptonEngine;
        private Texture2D Ground;
        private Texture2D MazeA, MazeB;
        private bool IsKrypton = true;

        public static Texture2D LightTexture;

        protected override void Initialize()
        {
            GamePlayManager.SpriteBatch = new SpriteBatch(GraphicsDevice);
            KryptonEngine.Initialize();
            InitializePhysics();
            InitializeInput();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            FrameRateCounter = new FrameRateCounter(this, Content.Load<SpriteFont>(@"Fonts\Visitor43"), GamePlayManager.SpriteBatch);
            GamePlayManager.StartServices();
            Components.Add(GamePlayManager.GamePlayComponentManager);
            GamePlayManager.UiManager.AddUiContainer(new UiContainer(GameEnvironment.ScreenRectangle, new UiControlManager()));

            Ground = Content.Load<Texture2D>(@"Concrete");
            MazeA = Content.Load<Texture2D>(@"MazeA");
            MazeB = Content.Load<Texture2D>(@"MazeB");
            LightTexture = LightTextureBuilder.CreatePointLight(this.GraphicsDevice, 1024);
            //CreateLights(LightTexture, 20);
            //CreateHulls(50);
            _compoundA = CreateMaze(MazeA, ref _originA);
            _compoundB = CreateMaze(MazeB, ref _originB);
            _compoundB.Position += ConvertUnits.ToSimUnits( MazeBOffset);
            
            Player = new Player(new Vector2(100, 100));
            KryptonEngine.Lights.Add(Player.Light);
        }

        private void CreateHulls(int count)
        {
            for (int i = 0; i < count; i++)
            {
                switch (0)//GameEnvironment.Random.Next(2))
                {
                    case 0:
                        if (i < 5)
                        {
                            CircleBlockade cblockade = new CircleBlockade(
                                5 + GameEnvironment.Random.NextFloat() * 10,
                                new Vector2(GameEnvironment.Random.NextFloat() * GameEnvironment.ScreenRectangle.Width, GameEnvironment.Random.NextFloat() * GameEnvironment.ScreenRectangle.Height));
                            KryptonEngine.Hulls.Add(cblockade.Hull);
                        }
                        else
                        {
                            int w = 10;
                            int h = 10; 
                            switch (GameEnvironment.Random.Next(2))
                            {
                                case 0:
                                    w = GameEnvironment.Random.Next(200, 1500);
                                    h = 10;
                                    break;
                                case 1:
                                    w = 10;
                                    h = GameEnvironment.Random.Next(20, 400);
                                    break;
                            }

                            RectangleBlockade rblockade = new RectangleBlockade(
                                w,
                                h,
                                new Vector2(GameEnvironment.Random.NextFloat() * GameEnvironment.ScreenRectangle.Width, GameEnvironment.Random.NextFloat() * GameEnvironment.ScreenRectangle.Height));
                            KryptonEngine.Hulls.Add(rblockade.Hull);
                        }
                        break;
                }

            }
        }

        private void CreateLights(Texture2D texture, int count)
        {
            return;
            // Make some random lights!
            for (int i = 0; i < count; i++)
            {
                byte r = (byte)(GameEnvironment.Random.Next(255 - 64) + 64);
                byte g = (byte)(GameEnvironment.Random.Next(255 - 64) + 64);
                byte b = (byte)(GameEnvironment.Random.Next(255 - 64) + 64);

                Light2D light = new Light2D()
                {
                    Texture = texture,
                    Range = (float)(GameEnvironment.Random.NextDouble() * 300 + 1),
                    Color = new Color(r, g, b),
                    Intensity = 1f,
                    Angle = MathHelper.TwoPi * (float)GameEnvironment.Random.NextDouble(),
                    X = (float)(GameEnvironment.Random.NextFloat() * GameEnvironment.ScreenRectangle.Width),
                    Y = (float)(GameEnvironment.Random.NextFloat() * GameEnvironment.ScreenRectangle.Height),
                };

                // Here we set the light's field of view
                if (i % 2 == 0)
                {
                    light.Fov = MathHelper.PiOver2 * (float)(GameEnvironment.Random.NextDouble() * 0.75 + 0.25);
                }

                KryptonEngine.Lights.Add(light);
            }
        }


        private Body _compoundA, _compoundB;
        private Vector2 _originA, _originB;
        private Vector2 MazeOffset = new Vector2(500, 500);
        private Vector2 MazeBOffset = new Vector2(152, -147);
        private float _scale;
        private Body CreateMaze(Texture2D Texture, ref Vector2 origin)
        {
            World World = GamePlayManager.WorldManager.World;
            World.Gravity = Vector2.Zero;

            //Create an array to hold the data from the texture
            uint[] data = new uint[Texture.Width * Texture.Height];

            //Transfer the texture data to the array
            Texture.GetData(data);

            //Find the vertices that makes up the outline of the shape in the texture
            Vertices textureVertices = PolygonTools.CreatePolygon(data, Texture.Width, false);

            //The tool return vertices as they were found in the texture.
            //We need to find the real center (centroid) of the vertices for 2 reasons:

            //1. To translate the vertices so the polygon is centered around the centroid.
            Vector2 centroid = -textureVertices.GetCentroid();
            textureVertices.Translate(ref centroid);

            //2. To draw the texture the correct place.
            origin = -centroid;

            //We simplify the vertices found in the texture.
            textureVertices = SimplifyTools.ReduceByDistance(textureVertices, 4f);

            //Since it is a concave polygon, we need to partition it into several smaller convex polygons
            /// EarclipDecomposer - 303 polygons - 60fps on XBOX - 7835 Milliseconds on XBOX
            /// BayazitDecomposer - 108 polygons - 60fps on XBOX -  222 Milliseconds on XBOX
            Stopwatch s = new Stopwatch();
            s.Start();
            List<Vertices> list = BayazitDecomposer.ConvexPartition(textureVertices);
            s.Stop();

            _scale = 1f;

            // Add to krypton
            foreach (Vertices vertices in list)
            {
                Vector2[] points = new Vector2[vertices.Count];
                int i = vertices.Count;
                foreach (Vector2 vector in vertices)
                {
                    i--;
                    points[i] = vector + MazeOffset + (Texture == MazeB ? MazeBOffset : Vector2.Zero);
                }
                ShadowHull hull = ShadowHull.CreateConvex(ref points);
                KryptonEngine.Hulls.Add(hull);
            }

            //scale the vertices from graphics space to sim space
            Vector2 vertScale = new Vector2(ConvertUnits.ToSimUnits(1)) * _scale;
            foreach (Vertices vertices in list)
            {
                vertices.Scale(ref vertScale);
            }

            //Create a single body with multiple fixtures
            Body compound = BodyFactory.CreateCompoundPolygon(World, list, 1f, BodyType.Static);
            compound.BodyType = BodyType.Static;
            compound.Position = ConvertUnits.ToSimUnits(MazeOffset);

            new Mob(GamePlayManager.MobManager, new FarseerPhysicalBody(compound))
            {
                AnimatedSprite = new AnimatedSprite(@"Blank")
            };

            return compound;
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

            Globals.CharacterComparisonPointForMouseControlVector = Player.PositionOnScreen;

            FrameRateCounter.Update(gameTime);


            if(InputManager.IsButtonPressed(Keys.O))
            {
                IsKrypton = !IsKrypton;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            //create matrices
            Vector2 Location = new Vector2(0, 0);
            float zoom = 1;
            float Rotation = 0;
            var translate = Matrix.CreateTranslation(-Location.X, -Location.Y, 0);
            //var offset = Matrix.CreateTranslation(GraphicsDevice.Adapter.CurrentDisplayMode.Width / 2, GraphicsDevice.Adapter.CurrentDisplayMode.Height / 2, 0);
            var scale = Matrix.CreateScale(zoom, zoom, 1);
            var rotation = Matrix.CreateRotationZ(Rotation);

            // transform them into one another
            Matrix matrix = translate;
            Matrix.Multiply(ref matrix, ref scale, out matrix);
            Matrix.Multiply(ref matrix, ref rotation, out matrix);
            //Matrix.Multiply(ref matrix, ref offset, out matrix);

            // Assign the matrix and pre-render the lightmap.
            // Make sure not to change the position of any lights or shadow hulls after this call, as it won't take effect till the next frame!
            KryptonEngine.Matrix = matrix;
            KryptonEngine.Bluriness = 1.5f;
            KryptonEngine.SpriteBatchCompatablityEnabled = true;
            KryptonEngine.CullMode = CullMode.CullClockwiseFace;
            KryptonEngine.AmbientColor = (IsKrypton ? Color.Black : Color.LightGray);
            KryptonEngine.LightMapPrepare();

            // Make sure we clear the backbuffer *after* Krypton is done pre-rendering
            this.GraphicsDevice.Clear(Color.White);

            GamePlayManager.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);
            GamePlayManager.SpriteBatch.Draw(Ground, GameEnvironment.ScreenRectangle, Color.White);
            GamePlayManager.SpriteBatch.Draw(MazeA, ConvertUnits.ToDisplayUnits(_compoundA.Position),
                                           null, Color.Tomato, _compoundA.Rotation, _originA, _scale, SpriteEffects.None,
                                           0f);
            GamePlayManager.SpriteBatch.Draw(MazeB, ConvertUnits.ToDisplayUnits(_compoundB.Position),
                                           null, Color.Tomato, _compoundB.Rotation, _originB, _scale, SpriteEffects.None,
                                           0f);
            GamePlayManager.SpriteBatch.End();

            // ----- DRAW STUFF HERE ----- //
            // By drawing here, you ensure that your scene is properly lit by krypton.
            // Drawing after KryptonEngine.Draw will cause you objects to be drawn on top of the lightmap (can be useful, fyi)
            // ----- DRAW STUFF HERE ----- //


            KryptonEngine.Draw(gameTime);

            DebugDraw();

            base.Draw(gameTime);

            FrameRateCounter.Draw(gameTime);
        }



        private void DebugDraw()
        {
            KryptonEngine.RenderHelper.Effect.CurrentTechnique = KryptonEngine.RenderHelper.Effect.Techniques["DebugDraw"];
            this.GraphicsDevice.RasterizerState = new RasterizerState()
            {
                CullMode = CullMode.None,
                FillMode = FillMode.WireFrame,
            };
            if (Keyboard.GetState().IsKeyDown(Keys.H))
            {
                // Clear the helpers vertices
                KryptonEngine.RenderHelper.ShadowHullVertices.Clear();
                KryptonEngine.RenderHelper.ShadowHullIndicies.Clear();

                foreach (var hull in KryptonEngine.Hulls)
                {
                    KryptonEngine.RenderHelper.BufferAddShadowHull(hull);
                }


                foreach (var effectPass in KryptonEngine.RenderHelper.Effect.CurrentTechnique.Passes)
                {
                    effectPass.Apply();
                    KryptonEngine.RenderHelper.BufferDraw();
                }
            }

            if (Keyboard.GetState().IsKeyDown(Keys.L))
            {
                KryptonEngine.RenderHelper.ShadowHullVertices.Clear();
                KryptonEngine.RenderHelper.ShadowHullIndicies.Clear();

                foreach (Light2D light in KryptonEngine.Lights)
                {
                    KryptonEngine.RenderHelper.BufferAddBoundOutline(light.Bounds);
                }

                foreach (var effectPass in KryptonEngine.RenderHelper.Effect.CurrentTechnique.Passes)
                {
                    effectPass.Apply();
                    KryptonEngine.RenderHelper.BufferDraw();
                }
            }
        }



        private void InitializePhysics()
        {
            ConvertUnits.SetDisplayUnitToSimUnitRatio(16f); // 16 pixels equals one meter
            Settings.TOIPositionIterations = 2;
            Settings.TOIVelocityIterations = 2;
            Settings.PositionIterations = 1;
            Settings.VelocityIterations = 1;
            Settings.EnableDiagnostics = false;
        }


        public void InitializeInput()
        {
#if WINDOWS
            InputManager.BindButton(Button.ActivatePointLight, Keys.Q);
            InputManager.BindButton(Button.ActivateSpotLight, Keys.E);
            InputManager.BindVectorControlToButtons(VectorControl.Movement, Keys.A, Keys.D, Keys.W, Keys.S);
            InputManager.BindCursorToMouse(Cursor.Player1);
            InputManager.BindVectorControlToMouseCursor(VectorControl.Aiming, () => { return Globals.CharacterComparisonPointForMouseControlVector; }, 5);

#elif XBOX
            InputManager.BindButton(Button.ActivatePointLight, Buttons.LeftTrigger);
            InputManager.BindButton(Button.ActivateSpotLight, Buttons.RightTrigger);
            InputManager.BindVectorControl(VectorControl.Movement, XboxThumbsticks.Left);
            InputManager.BindVectorControl(VectorControl.Aiming, XboxThumbsticks.Right);
#endif
        }
    }
}
