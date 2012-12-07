using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TDGameLibrary.Ui
{
    /**
     * A control managers responsibility is delegating when to calculate control focus and selection
     * Also to apply the update and draw methods for the child controls
     */
    public class UiControlManager : List<UiControl>
    {
        public UiControlManager()
            : base()
        {
        }

        public UiControlManager(int capacity)
            : base(capacity)
        {
        }

        public UiControlManager(IEnumerable<UiControl> collection)
            : base(collection)
        {
        }


        private int SelectedControlIndex = 0;
        private UiControl SelectedControl;
        public event EventHandler OnFocus;
        public bool IsFocusPersistent = false;
        public bool IsEnabled = true;
        public bool IsVisible = true;


        // Used to update the controls and handle the input for the currently selected control
        public void Update(GameTime gameTime)
        {
            if (Count == 0 || !IsEnabled)
            {
                return;
            }

            int i = 0;
            foreach (UiControl control in this)
            {
                if (control.IsEnabled)
                {
                    control.Update(gameTime);
                }

                if (control.IsInFocus)
                {
                    control.HandleInput();
                }

                if (!IsFocusPersistent)
                {
                    // We reset all controls to no focus on each update
                    control.IsInFocus = false;
                }


                if (IsEnabled && control.IsSelectable && control.IsEnabled && IsFocused(control))
                {
                    if (OnFocus != null && control != SelectedControl)
                    {
                        // Fire FocusChanged event
                        OnFocus(control, null);
                    }


                    if (IsFocusPersistent)
                    {
                        // We reset all controls to no focus if a new control is in focus
                        foreach (UiControl c in this)
                        {
                            c.IsInFocus = false;
                        }
                    }

                    // Set the new control to be in focus
                    control.IsInFocus = true;
                    SelectedControl = control;
                    SelectedControlIndex = i;

                    break;
                }
                i++;
            }


            if (IsSelectedNext())
            {
                NextControl();
            }


            if (IsSelectedPrevious())
            {
                PreviousControl();
            }
        }

        protected virtual bool IsFocused(UiControl control)
        {
            return false;
        }

        protected virtual bool IsSelectedPrevious()
        {
            return false;
        }

        protected virtual bool IsSelectedNext()
        {
            return false;
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            if (!IsVisible)
                return;

            foreach (UiControl c in this)
            {
                if (c.IsVisible)
                    c.Draw(spriteBatch);
            }
        }


        public void NextControl()
        {
            if (Count == 0)
            {
                return;
            }

            bool haveLocatedCurrent = false;
            bool swapped = false;
            foreach (UiControl control in this)
            {
                if(control == SelectedControl)
                {
                    haveLocatedCurrent = true;
                }

                if (haveLocatedCurrent && ChangeFocus(control))
                {
                    swapped = true;
                    break;
                }
            }

            // Wrap around
            if (!swapped)
            {
                foreach (UiControl control in this)
                {
                    if (ChangeFocus(control))
                        break;
                }
            }
        }


        public void PreviousControl()
        {
            if (Count == 0)
            {
                return;
            }

            bool haveLocatedCurrent = false;
            bool swapped = false;
            foreach (UiControl control in this.Reverse<UiControl>())
            {
                if (control == SelectedControl)
                {
                    haveLocatedCurrent = true;
                }

                if (haveLocatedCurrent && ChangeFocus(control))
                {
                    swapped = true;
                    break;
                }
            }

            // Wrap around
            if (!swapped)
            {
                foreach (UiControl control in this.Reverse<UiControl>())
                {
                    if(ChangeFocus(control))
                        break;
                }
            }
        }


        public void ClearFocus()
        {
            SelectedControlIndex = 0;
            foreach (UiControl control in this)
            {
                control.IsInFocus = false;
            }
        }

        public bool ChangeFocus(UiControl control)
        {
            bool focusChanged = false;

            if (SelectedControl == null)
            {
                SelectedControl = control;
                SelectedControl.IsInFocus = true;
                focusChanged = true;
            }
            else if (control != SelectedControl && control.IsEnabled && control.IsSelectable && !control.IsInFocus)
            {
                SelectedControl.IsInFocus = false;
                SelectedControl = control;
                SelectedControl.IsInFocus = true;
                focusChanged = true;
            }

            if (focusChanged)
            {
                if (OnFocus != null)
                {
                    OnFocus(control, EventArgs.Empty);
                }
                return true;
            }
            return false;
        }
    }
}
