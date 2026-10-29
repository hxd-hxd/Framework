using System;
using System.Collections.Generic;

namespace Framework.Event
{
    /// <summary>事件监听者</summary>
    internal struct EventListener
    {
        /// <summary>要执行的委托</summary>
        public Delegate listener;
        /// <summary>执行优先级</summary>
        public int priority;

        public bool Equals(EventListener listener)
        {
            return EqualityComparer<Delegate>.Default.Equals(this.listener, listener.listener) &&
                   priority == listener.priority;
        }

        public override bool Equals(object obj)
        {
            return obj is EventListener listener && Equals(listener);
        }

        public override int GetHashCode()
        {
            return System.HashCode.Combine(listener, priority);
        }

        public static bool operator ==(EventListener l, EventListener r)
        {
            return l.Equals(r);
        }
        public static bool operator !=(EventListener l, EventListener r)
        {
            return !(l == r);
        }
    }
}
