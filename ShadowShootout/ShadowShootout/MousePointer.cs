using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TDGameLibrary.Input;
using TDGameLibrary;

namespace ShadowShootout.Input
{
    // Defines how the mouse will look and update for the game
    public class MousePointer : DrawableGameComponent
    {
        public Texture2D mouseTexture;
        public Color mouseTint;
        private Vector2 mousePosition;
        private MouseState mouseState;
        private MouseState lastMouseState;

        public MousePointer(Game game)
            : base(game)
        {
            DrawOrder = GameEnvironment.MousePointerDrawOrder;
        }


        protected override void LoadContent()
        {
            mouseTexture = Game.Content.Load<Texture2D>(@"Mouse");
            this.mouseState = Mouse.GetState();
            this.mousePosition.X = this.mouseState.X;
            this.mousePosition.Y = this.mouseState.Y;

            base.LoadContent();
        }

        

        public override void Update(GameTime gameTime)
        {
            this.lastMouseState = this.mouseState;
            this.mouseState = Mouse.GetState();
            this.mousePosition.X = this.mouseState.X;
            this.mousePosition.Y = this.mouseState.Y;
            this.mouseTint = Color.White;

            // Restrict the Mouse so that it stays inside the current display
            if (this.mousePosition.X < 0)
                this.mousePosition.X = 0;
            if (this.mousePosition.X > Game.Window.ClientBounds.Width)
                this.mousePosition.X = Game.Window.ClientBounds.Width;
            if (this.mousePosition.Y < 0)
                this.mousePosition.Y = 0;
            if (this.mousePosition.Y > Game.Window.ClientBounds.Height)
                this.mousePosition.Y = Game.Window.ClientBounds.Height;

            base.Update(gameTime);
        }


        public override void Draw(GameTime gameTime)
        {
            GamePlayManager.SpriteBatch.Begin();

            GamePlayManager.SpriteBatch.Draw(this.mouseTexture, this.mousePosition, this.mouseTint);

            GamePlayManager.SpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
