using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TDGameLibrary.Ai.Behavioral;
using TDGameLibrary.Input;
using Microsoft.Xna.Framework;
using TDGameLibrary.Ai;

namespace TDGameLibrary.Behaviors
{
    public class InvokeAction : SequenceBehaviorTree
    {
        public InvokeAction(BehaviorTree parent, Func<bool> action)
            : base(parent)
        {
            Action = action;
        }

        private Func<bool> Action;

        public override BehaviorState GetState()
        {
            if(Action.Invoke())
                return BehaviorState.Succeeded;
            else
                return BehaviorState.Failed;
        }
    }
}
