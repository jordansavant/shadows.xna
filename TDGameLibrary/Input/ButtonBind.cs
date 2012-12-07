using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TDGameLibrary.Input
{
    internal sealed class ButtonBind : IButtonBind
    {
        public ButtonBind()
        {
            Buttons = new List<IDeviceButton>();
        }

        public List<IDeviceButton> Buttons { get; private set; }

        public bool IsDown
        {
            get
            {
                foreach (IDeviceButton button in Buttons)
                {
                    if (button.IsDown)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public bool IsPressed
        {
            get
            {
                foreach (IDeviceButton button in Buttons)
                {
                    if (button.IsPressed)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public bool IsReleased
        {
            get
            {
                foreach (IDeviceButton button in Buttons)
                {
                    if (button.IsReleased)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public void Update()
        {
            foreach (IDeviceButton b in Buttons)
            {
                b.Update();
            }
        }

    }
}
