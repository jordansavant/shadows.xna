using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TDGameLibrary.Ai.Behavioral
{
    public abstract class BehaviorTree
    {
        public BehaviorTree(BehaviorTree parent)
        {
            Parent = parent;
            Blackboard = (parent != null) ? new Blackboard(Parent.Blackboard) : new Blackboard(null);
            Children = new List<BehaviorTree>();
            CurrentState = BehaviorState.Ready;
            RunningChild = null;

            Name = GetType().Name;
        }

        public Blackboard Blackboard;
        public BehaviorTree Parent;
        public List<BehaviorTree> Children;
        public BehaviorTree RunningChild;
        public BehaviorState CurrentState;
        public string Name { get; private set; }

        public abstract BehaviorState GetState();

        //TODO: use generics / find a fix for this.
        //It is dangerous to set parent = null in the child constructor and then assume ChainChild will always be there to fix it.
        //What if code gets refactored so the child is no longer chained, but the line of code instantiating the child is copied
        //  and the developer forgets to change 'null' to 'self'?
        public virtual BehaviorTree ChainChild(BehaviorTree child)
        {
            child.Parent = this;
            Children.Add(child);

            return this;
        }

        public string GetPath()
        {
            return GetPath(this.Parent);
        }

        private string GetPath(BehaviorTree parent)
        {
            return parent == null ? Name : parent.GetPath() + "\\" + Name;
        }
    }

}
