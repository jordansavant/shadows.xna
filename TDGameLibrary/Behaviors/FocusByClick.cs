using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TDGameLibrary.Ai.Behavioral;
using TDGameLibrary.Input;
using Microsoft.Xna.Framework;
using TDGameLibrary.Ai;
using TDGameLibrary.Mobs;

namespace TDGameLibrary.Behaviors
{
    public class FocusByClick : SequenceBehaviorTree
    {
        public FocusByClick(BehaviorTree parent, Mob mob, Rectangle clickRegion, Enum button, Enum cursor)
            : base(parent)
        {
            Mob = mob;
            ClickRegion = clickRegion;
            Button = button;
            Cursor = cursor;
        }

        private Enum Button { get; set; }
        private Enum Cursor { get; set; }
        private Mob Mob { get; set; }
        private Rectangle ClickRegion { get; set; }

        public override BehaviorState GetState()
        {
            //if (InputManager.IsButtonPressed(Button))
            //{
            //    Mob.IsFocused = InputManager.IsCursorHovering(Cursor, ClickRegion);
            //}

            //if (Mob.IsFocused)
            //    return BehaviorState.Succeeded;

            return BehaviorState.Failed;
        }
    }
}
