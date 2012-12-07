using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Microsoft.Xna.Framework;

namespace TDGameLibrary.Resources
{
    public abstract class ContentAutoLoader<T> : DrawableGameComponent
    {
        public ContentAutoLoader(Game game)
            : base(game)
        {
        }

        protected override void LoadContent()
        {
            FieldInfo[] fieldInfos = this.GetType().GetFields(BindingFlags.Public | BindingFlags.Static);

            foreach (FieldInfo fi in fieldInfos)
            {
                Game.Content.Load<T>(fi.GetValue(null).ToString());
            }

            base.LoadContent();
        }
    }
}
