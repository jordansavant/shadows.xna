using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using TDGameLibrary.Ui;
using Microsoft.Xna.Framework;

namespace TDGameLibrary.Components
{
    public class UiComponentManager : TdglComponent
    {
        public UiComponentManager(SpriteBatch spriteBatch)
        {
            UiContainers = new List<UiContainer>();
            UiSpriteBatch = spriteBatch;
        }

        private SpriteBatch UiSpriteBatch;
        public int UiDrawIndex { get; private set; }
        public List<UiContainer> UiContainers { get; private set; }

        public bool IsUiSelected { get; set; }
        public bool IsUiFocused { get; set; }

        public override void Update(GameTime gameTime)
        {
            IsUiSelected = false;
            IsUiFocused = false;

            foreach (UiContainer uiContainer in UiContainers)
            {
                uiContainer.Update(gameTime);
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            UiSpriteBatch.Begin(
             SpriteSortMode.Immediate,
             BlendState.NonPremultiplied,
             SamplerState.AnisotropicClamp,
             null,
             null,
             null,
             Matrix.Identity);

            foreach (UiContainer uiContainer in UiContainers)
            {
                uiContainer.Draw(gameTime, UiSpriteBatch);
            }

            UiSpriteBatch.End();
        }

        public void AddUiContainer(UiContainer uiContainer)
        {
            UiContainers.Add(uiContainer);

            // also subscribe to their selected/focused events
            uiContainer.SubscribeFunctionToControlSelectEvents(UiSelected);
            uiContainer.SubscribeFunctionToControlFocusEvents(UiFocused);
        }

        public void UiSelected(object sender, EventArgs e)
        {
            IsUiSelected = true;
        }

        public void UiFocused(object sender, EventArgs e)
        {
            IsUiFocused = true;
        }
    }
}
