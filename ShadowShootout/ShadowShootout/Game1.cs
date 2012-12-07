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
        }

        private int PreferredWidth = 1920;
        private int PreferredHeight = 1080;
        private bool FullScreen = false;
        private GraphicsDeviceManager GraphicsDeviceManager;
        private FrameRateCounter FrameRateCounter;
        private Player Player;
        private KryptonEngine KryptonEngine;
        private Texture2D LightTexture;
        private Texture2D Ground;

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

            LightTexture = LightTextureBuilder.CreatePointLight(this.GraphicsDevice, 1024);
            Light2D light = new Light2D()
            {
                Texture = LightTexture,
                Range = 120,
                Color = Color.LightBlue,
                Intensity = .7f,
                Angle = MathHelper.TwoPi
            };

            Player = new Player(light, new Vector2(100, 100));
            KryptonEngine.Lights.Add(Player.Light);

            for (int i = 0; i < 80; i++)
            {
                switch(GameEnvironment.Random.Next(2))
                {
                    case 0:
                        CircleBlockade cblockade = new CircleBlockade(
                            5 + GameEnvironment.Random.NextFloat() * 10, 
                            new Vector2(GameEnvironment.Random.NextFloat() * GameEnvironment.ScreenRectangle.Width, GameEnvironment.Random.NextFloat() * GameEnvironment.ScreenRectangle.Height));
                        KryptonEngine.Hulls.Add(cblockade.Hull);
                        break;
                    case 1:
                        RectangleBlockade rblockade = new RectangleBlockade(
                            5 + GameEnvironment.Random.NextFloat() * 10, 
                            5 + GameEnvironment.Random.NextFloat() * 10, 
                            new Vector2(GameEnvironment.Random.NextFloat() * GameEnvironment.ScreenRectangle.Width, GameEnvironment.Random.NextFloat() * GameEnvironment.ScreenRectangle.Height));
                        KryptonEngine.Hulls.Add(rblockade.Hull);
                        break;
                }

            }
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

            FrameRateCounter.Update(gameTime);

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
            KryptonEngine.AmbientColor = Color.Black;
            KryptonEngine.LightMapPrepare();

            // Make sure we clear the backbuffer *after* Krypton is done pre-rendering
            this.GraphicsDevice.Clear(Color.White);

            GamePlayManager.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);
            GamePlayManager.SpriteBatch.Draw(Ground, GameEnvironment.ScreenRectangle, Color.White);
            GamePlayManager.SpriteBatch.End();

            // ----- DRAW STUFF HERE ----- //
            // By drawing here, you ensure that your scene is properly lit by krypton.
            // Drawing after KryptonEngine.Draw will cause you objects to be drawn on top of the lightmap (can be useful, fyi)
            // ----- DRAW STUFF HERE ----- //

            // Draw krypton (This can be omited if krypton is in the Component list. It will simply draw krypton when base.Draw is called
            KryptonEngine.Draw(gameTime);

            // Draw the shadow hulls as-is (no shadow stretching) in pure white on top of the shadows
            // You can omit this line if you want to see what the light-map looks like :)
            //DebugDraw();

            base.Draw(gameTime);

            FrameRateCounter.Draw(gameTime);
        }







        private void DebugDraw()
        {
            KryptonEngine.RenderHelper.Effect.CurrentTechnique = KryptonEngine.RenderHelper.Effect.Techniques["DebugDraw"];
            GraphicsDevice.RasterizerState = new RasterizerState()
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
            InputManager.BindVectorControlToButtons(VectorControl.Movement, Keys.A, Keys.D, Keys.W, Keys.S);
            InputManager.BindCursorToMouse(Cursor.Player1);

#elif XBOX
            InputManager.BindVectorControl(VectorControl.Movement, XboxThumbsticks.Left);
            InputManager.BindVectorControl(VectorControl.MenuNavigation, XboxThumbsticks.Left);
#endif
        }
    }
}
