using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TDGameLibrary.Ui.Modifiers
{
    public class UiControlModifier
    {
        public UiControlModifier(UiControl control)
        {
            Control = control;
        }

        public UiControl Control { get; private set; }
        public event EventHandler OnFinishedEffect;

        public virtual void Update(GameTime gameTime)
        {
        }

        protected void FinishEffect()
        {
            if (OnFinishedEffect != null)
            {
                OnFinishedEffect.Invoke(this, EventArgs.Empty);
            }
        }

    }
}
