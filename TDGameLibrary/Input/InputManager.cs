using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;

namespace TDGameLibrary.Input
{
    public class InputManager : GameComponent
    {
        public InputManager(Game game) : base(game)
        {            
            ButtonBinds = new Dictionary<Enum, IButtonBind>();
            CursorBinds = new Dictionary<Enum, ICursorSensor>();
            VectorBinds = new Dictionary<Enum, IVectorBind>();
            
            MouseCursor = new MouseCursor();
            Instance = this;
            Game = game;
        }

        protected static InputManager Instance { get; set; }
        protected static Dictionary<Enum, IButtonBind> ButtonBinds { get; set; }
        protected static Dictionary<Enum, ICursorSensor> CursorBinds { get; set; }
        protected static Dictionary<Enum, IVectorBind> VectorBinds { get; set; }
        public static ICursorSensor MouseCursor { get; protected set; }
        protected static Game Game;
        public static bool IsInputSilenced
        {
            get
            {
                return false;
                //return (Guide.IsVisible || !Game.IsActive);
            }
        }

        public KeyboardState KeyboardLastState = Keyboard.GetState();
        public KeyboardState KeyboardCurrentState = Keyboard.GetState();
        public override void Update(GameTime gameTime)
        {
            KeyboardLastState = KeyboardCurrentState;
            KeyboardCurrentState = Keyboard.GetState();

            foreach (IButtonBind button in ButtonBinds.Values)
            {
                button.Update();
            }

            foreach (ICursorSensor cursor in CursorBinds.Values)
            {
                cursor.Update();
            }

            foreach (IVectorBind vectorControl in VectorBinds.Values)
            {
                vectorControl.Update();
            }

            base.Update(gameTime);
        }

        public static bool IsButtonDown(Keys key)
        {
            return Instance.KeyboardCurrentState.IsKeyDown(key);
        }

        public static bool IsButtonPressed(Keys key)
        {
            return Instance.KeyboardCurrentState.IsKeyDown(key) && Instance.KeyboardLastState.IsKeyUp(key);
        }

        public static bool IsButtonReleased(Keys key)
        {
            return Instance.KeyboardCurrentState.IsKeyUp(key) && Instance.KeyboardLastState.IsKeyDown(key);
        }

        public static bool IsButtonDown(Enum button)
        {
            if (IsInputSilenced)
                return false;

            return ButtonBinds.ContainsKey(button) && ButtonBinds[button].IsDown;
        }

        public static bool IsButtonPressed(Enum button)
        {
            if (IsInputSilenced)
                return false;

            return ButtonBinds.ContainsKey(button) && ButtonBinds[button].IsPressed;
        }

        public static bool IsButtonReleased(Enum button)
        {
            if (IsInputSilenced)
                return false;

            return ButtonBinds.ContainsKey(button) && ButtonBinds[button].IsReleased;
        }

        public static bool IsCursorHovering(Enum cursor, Rectangle rectangle)
        {
            if (IsInputSilenced)
                return false;

            return CursorBinds.ContainsKey(cursor) && CursorBinds[cursor].IsCursorHovering(rectangle);
        }

        public static bool IsCursorHovering(Enum cursor, Vector2 position, float radius)
        {
            if (IsInputSilenced)
                return false;

            return CursorBinds.ContainsKey(cursor) && CursorBinds[cursor].IsCursorHovering(position, radius);
        }

        public static bool IsCursorMoving(Enum cursor)
        {
            if (IsInputSilenced)
                return false;

            return CursorBinds.ContainsKey(cursor) && CursorBinds[cursor].IsCursorMoving();
        }

        private static void PrepareButtonBind(Enum button)
        {
            if (!ButtonBinds.ContainsKey(button))
            {
                ButtonBinds.Add(button, new ButtonBind());
            }
        }

        private static void PrepareVectorBind(Enum vectorControl)
        {
            if (!VectorBinds.ContainsKey(vectorControl))
            {
                VectorBinds.Add(vectorControl, new VectorBind());
            }
        }

        public static void BindButton(Enum button, Keys keyboardButton)
        {
            PrepareButtonBind(button);
            ButtonBinds[button].Buttons.Add(new KeyboardDeviceButton(keyboardButton));
        }

        public static void BindButton(Enum button, MouseButton mouseButton)
        {
            PrepareButtonBind(button);
            ButtonBinds[button].Buttons.Add(new MouseDeviceButton(mouseButton));
        }

        public static void BindButton(Enum button, MouseGesture mouseGesture)
        {
            PrepareButtonBind(button);
            ButtonBinds[button].Buttons.Add(new MouseGestureButton(mouseGesture));
        }

        public static void BindButton(Enum button, Buttons gamePadButton)
        {
            PrepareButtonBind(button);
            ButtonBinds[button].Buttons.Add(new GamePadDeviceButton(gamePadButton));
        }


        //Cursors can be mapped to many enums, but each distinct enum can only map to one cursor.
        private static void BindCursor(Enum cursorId, ICursorSensor cursor)
        {
            if (!CursorBinds.ContainsKey(cursorId))
            {
                CursorBinds.Add(cursorId, cursor);
            }
            else
            {
                CursorBinds[cursorId] = cursor;
            }
        }

        public static void BindCursorToMouse(Enum cursor)
        {
            BindCursor(cursor, MouseCursor);
        }


        public static void BindVectorControl(Enum vectorControl, XboxThumbsticks xboxThumbstick)
        {
            PrepareVectorBind(vectorControl);
            VectorBinds[vectorControl].DirectionalDevices.Add(new GamePadVectorBind(xboxThumbstick));
        }

        public static void BindVectorControlToMouseCursor(Enum vectorControl, Func<Vector2> getComparisonPoint, int deadZoneRadius)
        {
            PrepareVectorBind(vectorControl);
            VectorBinds[vectorControl].DirectionalDevices.Add(new MouseVectorBind(getComparisonPoint, deadZoneRadius));
        }

        public static void BindVectorControlToButtons(Enum vectorControl, Keys keyLeft, Keys keyRight, Keys keyUp, Keys keyDown)
        {
            PrepareVectorBind(vectorControl);
            VectorBinds[vectorControl].DirectionalDevices.Add(new KeyboardVectorBind(keyLeft, keyRight, keyUp, keyDown));
        }

        public static Vector2 GetCurrentControlVector(Enum vectorControl)
        {
            return VectorBinds.ContainsKey(vectorControl) ? VectorBinds[vectorControl].GetCurrentVector() : Vector2.Zero;
        }
    }
}
