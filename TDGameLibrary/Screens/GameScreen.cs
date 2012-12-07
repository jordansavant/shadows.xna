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
using TDGameLibrary.Components;
using TDGameLibrary.XnaComponents;

namespace TDGameLibrary.Screens
{
    // This class enables or disables GameComponents based upon screen events.
    // This allows for the game to not update or render screens that are not active.
    // It also preserves their state.

    public abstract partial class GameScreen
    {
        public GameScreen(Game game, ScreenManager manager)
        {
            StateManager = manager;

            Components = new List<GameComponent>();
        }

        // List of game components that belong to the screen
        public List<GameComponent> Components { get; private set; }

        protected ScreenManager StateManager { get; private set; }

        public bool Visible;
        public bool Enabled;

        public virtual void LoadContent()
        {
        }

        public virtual void Update(GameTime gameTime)
        {
            foreach (GameComponent component in Components)
            {
                if (component.Enabled)
                {
                    component.Update(gameTime);
                }
            }
        }

        public virtual void Draw(GameTime gameTime)
        {
            DrawableGameComponent drawComponent;

            foreach (GameComponent component in Components)
            {
                if (component is DrawableGameComponent)
                {
                    drawComponent = component as DrawableGameComponent;
                    if (drawComponent.Visible)
                    {
                        drawComponent.Draw(gameTime);
                    }
                }
            }
        }


        public virtual void Destruct()
        {
        }


        // Changes game states
        // All active screens subscribe to an event in the GameStateManager class
        internal protected virtual void StateChange(object sender, EventArgs e)
        {
            if (StateManager.CurrentScreen == this)
                Show();
            else
                Hide(); 
        }

        // Sets a screen enabled and visible
        // For all components, since the screen is active, make them visible or enabled for Draw and Update
        protected virtual void Show()
        {
            Visible = true;
            Enabled = true;
            foreach (GameComponent component in Components)
            {
                component.Enabled = true;
                if (component is DrawableGameComponent)
                {
                    ((DrawableGameComponent)component).Visible = true;
                }
            }
        }

        // Opposite, hide the screen GameComponents, i.e. don't update or draw them for now
        protected virtual void Hide()
        {
            Visible = false;
            Enabled = false;
            foreach (GameComponent component in Components)
            {
                component.Enabled = false;
                if (component is DrawableGameComponent)
                {
                    ((DrawableGameComponent)component).Visible = false;
                }
            }
        }

    }
}
