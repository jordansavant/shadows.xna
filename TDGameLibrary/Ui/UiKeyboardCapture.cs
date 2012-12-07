using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.GamerServices;

namespace TDGameLibrary.Ui
{
    public class UiKeyboardCapture
    {
        public UiKeyboardCapture(int maxKeys, string title, string description, string defaultText)
        {
            MaxKeys = maxKeys;
            Title = title;
            Description = description;
            DefaultText = defaultText;
        }

        // Keyboard Variables
        public int MaxKeys;
        private string Text = "";
        private KeyboardState LastKeyboardState;
        private KeyboardState CurrentKeyboardState;
        private Keys[] KeysToCheck = new Keys[]
        {
            Keys.NumPad0, Keys.NumPad1, Keys.NumPad2,
            Keys.NumPad3, Keys.NumPad4, Keys.NumPad5,
            Keys.NumPad6, Keys.NumPad7, Keys.NumPad8,
            Keys.NumPad9,
            Keys.D0, Keys.D1, Keys.D2, Keys.D3, Keys.D4,
            Keys.D5, Keys.D6, Keys.D7, Keys.D8, Keys.D9,
            Keys.A, Keys.B, Keys.C, Keys.D, Keys.E,
            Keys.F, Keys.G, Keys.H, Keys.I, Keys.J,
            Keys.K, Keys.L, Keys.M, Keys.N, Keys.O,
            Keys.P, Keys.Q, Keys.R, Keys.S, Keys.T,
            Keys.U, Keys.V, Keys.W, Keys.X, Keys.Y,
            Keys.Z,
            Keys.Back, Keys.Space
        };


        // XBOX Variables
        private IAsyncResult VirtualKeyboard = null;
        public string Title;
        public string Description;
        public string DefaultText;
        public EventHandler<EventArgs> OnVirtualKeyboardClose;

        public void Update(GameTime gameTime)
        {
#if WINDOWS
            UpdateViaKeyboard(gameTime);
#elif XBOX
            UpdateViaXboxVirtualKeyboard(gameTime);
#endif
        }


        private void UpdateViaXboxVirtualKeyboard(GameTime gameTime)
        {
            if (VirtualKeyboard == null)
            {
                VirtualKeyboard = Guide.BeginShowKeyboardInput(GameEnvironment.PlayerIndex, Title, Description, DefaultText, GetVirtualKeyboardResult, null);
            }
        }

        private void GetVirtualKeyboardResult(IAsyncResult result)
        {
            Text = Guide.EndShowKeyboardInput(result);

            if (OnVirtualKeyboardClose != null)
            {
                OnVirtualKeyboardClose.Invoke(this, EventArgs.Empty);
            }

            VirtualKeyboard = null;
        }



        private void UpdateViaKeyboard(GameTime gameTime)
        {
            CurrentKeyboardState = Keyboard.GetState();

            foreach (Keys key in KeysToCheck)
            {
                if (CheckKey(key))
                {
                    AddKeyToText(key);
                    break;
                }
            }

            LastKeyboardState = CurrentKeyboardState;
        }

        public string GetText()
        {
            return Text;
        }

        public void Clear()
        {
            Text = "";
        }

        private void AddKeyToText(Keys key)
        {
            string newChar = "";

            if (Text.Length >= MaxKeys && key != Keys.Back)
                return;

            switch (key)
            {
                case Keys.A: newChar += "a"; break;
                case Keys.B: newChar += "b"; break;
                case Keys.C: newChar += "c"; break;
                case Keys.D: newChar += "d"; break;
                case Keys.E: newChar += "e"; break;
                case Keys.F: newChar += "f"; break;
                case Keys.G: newChar += "g"; break;
                case Keys.H: newChar += "h"; break;
                case Keys.I: newChar += "i"; break;
                case Keys.J: newChar += "j"; break;
                case Keys.K: newChar += "k"; break;
                case Keys.L: newChar += "l"; break;
                case Keys.M: newChar += "m"; break;
                case Keys.N: newChar += "n"; break;
                case Keys.O: newChar += "o"; break;
                case Keys.P: newChar += "p"; break;
                case Keys.Q: newChar += "q"; break;
                case Keys.R: newChar += "r"; break;
                case Keys.S: newChar += "s"; break;
                case Keys.T: newChar += "t"; break;
                case Keys.U: newChar += "u"; break;
                case Keys.V: newChar += "v"; break;
                case Keys.W: newChar += "w"; break;
                case Keys.X: newChar += "x"; break;
                case Keys.Y: newChar += "y"; break;
                case Keys.Z: newChar += "z"; break;
                case Keys.Space: newChar += " "; break;
                case Keys.Back:
                    if (Text.Length != 0)
                    {
                        Text = Text.Remove(Text.Length - 1);
                    }
                    return;
            }

            if (CurrentKeyboardState.IsKeyDown(Keys.RightShift) || CurrentKeyboardState.IsKeyDown(Keys.LeftShift))
            {
                newChar = newChar.ToUpper();
            }

            Text += newChar;
        }

        private bool CheckKey(Keys theKey)
        {
            return LastKeyboardState.IsKeyDown(theKey) && CurrentKeyboardState.IsKeyUp(theKey);
        }
    }
}
