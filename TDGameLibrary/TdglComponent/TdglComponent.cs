using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TDGameLibrary.Components
{
    public abstract class TdglComponent
    {
        public TdglComponent()
        {
        }

        public int DrawOrder;
        public bool IsPauseIgnored;
        public bool IsLoaded;

        /// <summary>
        /// This method runs every step until IsLoaded = true
        /// </summary>
        /// <param name="gameTime"></param>
        public virtual void LoadWait(GameTime gameTime)
        {
            IsLoaded = true;
        }

        /// <summary>
        /// This method runs every step once ALL COMPONENTS are loaded
        /// </summary>
        /// <param name="gameTime"></param>
        public virtual void Update(GameTime gameTime)
        {
        }

        /// <summary>
        /// This method runs every step once IsLoaded = true
        /// </summary>
        /// <param name="gameTime"></param>
        public virtual void Draw(GameTime gameTime)
        {
        }

        public virtual void GameOver()
        {
        }

        public virtual void Destruct()
        {
        }

    }
}
