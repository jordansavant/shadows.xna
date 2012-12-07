using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using TDGameLibrary.Screens;

namespace TDGameLibrary.XnaComponents
{
    public class ScreenManager : DrawableGameComponent
    {
        public ScreenManager(Game game)
            : base(game)
        {
            GameScreens = new Stack<GameScreen>();
        }

        private Stack<GameScreen> GameScreens;
        public event EventHandler OnStateChange;

        public GameScreen CurrentScreen
        {
            get { return GameScreens.Peek(); }
        }

        public void PreviousScreen()
        {
            if (GameScreens.Count > 0)
            {
                RemoveScreen();

                if (GameScreens.Count == 0)
                {
                    Game.Exit();
                }

                if (OnStateChange != null)
                {
                    OnStateChange(this, EventArgs.Empty);
                }
            }
        }

        public void NextScreen(GameScreen nextScreen)
        {

            AddScreen(nextScreen); // add the state

            // Call event handler if subscribed to
            if (OnStateChange != null)
                OnStateChange(this, null); // if yes call the state change event
        }

        // Removes all other states/screens from the stack and starts at the new state
        public void RestartAtScreen(GameScreen newScreen)
        {
            while (GameScreens.Count > 0)
            {
                RemoveScreen();
            }

            AddScreen(newScreen);

            if (OnStateChange != null)
            {
                OnStateChange(this, EventArgs.Empty);
            }
        }

        private void AddScreen(GameScreen newState)
        {
            GameScreens.Push(newState);
            newState.LoadContent();
            OnStateChange += newState.StateChange;
        }

        private void RemoveScreen()
        {
            GameScreen screen = GameScreens.Peek();
            OnStateChange -= screen.StateChange;
            screen.Destruct();
            GameScreens.Pop();
        }


        public override void Update(GameTime gameTime)
        {
            if (GameScreens.Count > 0)
            {
                GameScreens.Peek().Update(gameTime);
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (GameScreens.Count > 0)
            {
                GameScreens.Peek().Draw(gameTime);
            }

            base.Draw(gameTime);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (GameScreen gs in GameScreens)
                {
                    gs.Destruct();
                }
            }
            base.Dispose(disposing);
        }

    }
}
