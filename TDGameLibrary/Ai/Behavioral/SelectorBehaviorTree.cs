using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TDGameLibrary.Ai.Behavioral
{
    /// <summary>
    /// Acts as OR gate.
    /// </summary>
    public class SelectorBehaviorTree : BehaviorTree
    {
        public SelectorBehaviorTree(BehaviorTree parent)
            : base(parent)
        {
        }


        public override BehaviorState GetState()
        {
            BehaviorState childBehaviorState;

            foreach (BehaviorTree child in Children)
            {
                childBehaviorState = child.GetState();

                if (childBehaviorState == BehaviorState.Failed)         // If child behavior fails, go to next child
                {
                    RunningChild = null;
                    continue;
                }
                else if (childBehaviorState == BehaviorState.Running)   // If child behavior is running, then I am also running
                {
                    RunningChild = child;
                    CurrentState = BehaviorState.Running;
                    return CurrentState;
                }
                else if (childBehaviorState == BehaviorState.Succeeded) // If child behavior already ran, then I succeed
                {
                    RunningChild = null;
                    CurrentState = BehaviorState.Succeeded;
                    return CurrentState;
                }
            }

            RunningChild = null;
            CurrentState = BehaviorState.Failed;                         // If all children fail, I fail
            return CurrentState;
        }

    }
}
