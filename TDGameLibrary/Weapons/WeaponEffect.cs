using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TDGameLibrary.Components;

namespace TDGameLibrary.Weapons
{
    public class WeaponEffect
    {
        public WeaponEffect(WeaponManager manager)
        {
            WeaponManager = manager;
            WeaponManager.RegisterWeaponEffect(this);
            IsComplete = false;
        }

        public WeaponManager WeaponManager;
        public bool IsComplete;

        public virtual void Update(GameTime gameTime)
        {
            if (IsComplete)
            {
                Destruct();
            }
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
        }


        public virtual void Destruct()
        {
            WeaponManager.UnregisterWeaponEffect(this);
        }
    }
}
