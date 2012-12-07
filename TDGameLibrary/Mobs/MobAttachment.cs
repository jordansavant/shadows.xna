using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TDGameLibrary.Components;

namespace TDGameLibrary.Mobs
{
    public abstract class MobAttachment : DrawOnlyMob
    {
        public MobAttachment(MobManager mobManager, Vector2 positionInWorld, float parallax, int worldLayer)
            : base(mobManager, positionInWorld, parallax, worldLayer)
        {
        }

        public virtual Mob Owner { get; set; }

        public override bool Collide(Mob other)
        {
            return false;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, float layerDepth)
        {
            // Do not draw unless no owner.
            if (Owner == null)
            {
                base.Draw(gameTime, spriteBatch, layerDepth);
            }
        }
    }
}
