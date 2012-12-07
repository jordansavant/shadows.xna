using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;

namespace TDGameLibrary.Components
{
    public class WorldManager : TdglComponent
    {
        public WorldManager(World world)
        {
            World = world;
        }

        public World World { get; private set; }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            World.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f);
        }

        /// <summary>
        /// Gets the position of the screen in the world. (Pixels)
        /// This provides support for a screen camera that can pan across the world, etc.
        /// </summary>
        public virtual Vector2 ScreenPositionInWorld
        {
            get
            {
                return Vector2.Zero;
            }
        }


        Rectangle _screenRectangle;
        public Rectangle ScreenRectangle
        {
            get
            {
                _screenRectangle.X = (int)ScreenPositionInWorld.X;
                _screenRectangle.Y = (int)ScreenPositionInWorld.Y;
                _screenRectangle.Width = GameEnvironment.ScreenRectangle.Width;
                _screenRectangle.Height = GameEnvironment.ScreenRectangle.Height;

                return _screenRectangle;
            }
        }

    }
}
