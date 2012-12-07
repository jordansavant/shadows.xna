using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TDGameLibrary.Ai.Behavioral
{
    public abstract class Behavior : SequenceBehaviorTree
    {
        public Behavior(BehaviorTree parent)
            : base(parent)
        {
        }

        public T SafeExec<T>(Func<T> func)
        {
            if (func != null)
            {
                return func.Invoke();
            }
            return default(T);
        }

        public void SafeExec<T>(Action<T> action, T args)
        {
            if (action != null)
            {
                action.Invoke(args);
            }
        }


    }

    //public abstract class Behavior<T> : SequenceBehaviorTree
    //    where T : Behavior<T>
    //{
    //    public Behavior(BehaviorTree parent, Action<T> updater)
    //        : base(parent)
    //    {
    //        Updater = updater;
    //    }

    //    private Action<T> _updater;
    //    public Action<T> Updater
    //    {
    //        get
    //        {
    //            return _updater;
    //        }
    //        set
    //        {
    //            if (value == null)
    //            {
    //                throw new ArgumentNullException("Updater");
    //            }
    //            _updater = value;
    //        }

    //    }

    //    public override sealed BehaviorState GetState()
    //    {
    //        Updater.Invoke((T)this);
    //        return Update();
    //    }

    //    protected abstract BehaviorState Update();
    //}
}
