using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TDGameLibrary.Ai.Behavioral
{
    /// <summary>
    /// Acts as AND gate with short-circuit (default) or without short-circuit (IsPrioritySelector=false).
    /// </summary>
    public class SequenceBehaviorTree : BehaviorTree
    {
        public SequenceBehaviorTree(BehaviorTree parent)
            : base(parent)
        {
            IsPrioritySelector = true;
        }

        protected bool IsPrioritySelector;


        public override BehaviorState GetState()
        {
            BehaviorState childBehaviorState;

            foreach (BehaviorTree child in Children)
            {
                if (RunningChild != null && child != RunningChild && IsPrioritySelector)      // Jump to running child if we have one
                {
                    continue;
                }

                childBehaviorState = child.GetState();

                if (childBehaviorState == BehaviorState.Failed)         // If child behavior fails, I fail
                {
                    RunningChild = null;
                    CurrentState = BehaviorState.Failed;
                    return CurrentState;
                }
                else if (childBehaviorState == BehaviorState.Running)   // If child behavior is running, then I am also running
                {
                    RunningChild = child;
                    CurrentState = BehaviorState.Running;
                    return CurrentState;
                }
                else if (childBehaviorState == BehaviorState.Succeeded) // If child behavior already ran, then go to next child to run
                {
                    RunningChild = null;
                    continue;
                }
            }

            RunningChild = null;
            CurrentState = BehaviorState.Succeeded;                         // If no children, or children all ready, I am ready
            return CurrentState;
        }

    }
}
