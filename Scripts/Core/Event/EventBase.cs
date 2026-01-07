
using System;

namespace Framework.Event
{
    /// <summary>
    /// 事件基类 - 无参
    /// <para>注意：不要缓存事件消息实例，因为发送完毕之后会被对象池回收</para>
    /// </summary>
    public abstract class EventBase<T> : ITypePoolObject, IEventMessage
        where T : EventBase<T>
    {
        /// <summary>
        /// 发送者
        /// </summary>
        public object sender;

        public static void Send()
        {
            var msg = TypePool.root.Get<T>();
            EventCenter.SendType(msg);
            TypePool.root.Return(msg);
        }

        public static void Send(object sender)
        {
            var msg = TypePool.root.Get<T>();
            msg.sender = sender;
            EventCenter.SendType(msg);
            TypePool.root.Return(msg);
        }

        public static void AddListener(Action<T> listener)
        {
            EventCenter.AddListener(listener);
        }

        public static void RemoveListener(Action<T> listener)
        {
            EventCenter.RemoveListener(listener);
        }

        public virtual void Clear()
        {
            sender = null;
        }
    }

    /// <summary>
    /// 事件基类 - 1 个参数
    /// <para>注意：不要缓存事件消息实例，因为发送完毕之后会被对象池回收</para>
    /// </summary>
    public abstract class EventBase<T, P1> : EventBase<EventBase<T, P1>>
        where T : EventBase<T, P1>
    {
        public P1 p1;

        public static void Send(P1 p1)
        {
            Send(p1, null);
        }

        public static void Send(P1 p1, object sender)
        {
            var msg = TypePool.root.Get<T>();
            msg.sender = sender;
            msg.p1 = p1;
            EventCenter.SendType(msg);
            TypePool.root.Return(msg);
        }

        public override void Clear()
        {
            base.Clear();
            p1 = default;
        }
    }

    /// <summary>
    /// 事件基类 - 2 个参数
    /// <para>注意：不要缓存事件消息实例，因为发送完毕之后会被对象池回收</para>
    /// </summary>
    public abstract class EventBase<T, P1, P2> : EventBase<EventBase<T, P1, P2>>
        where T : EventBase<T, P1, P2>
    {
        public P1 p1;
        public P2 p2;

        public static void Send(P1 p1, P2 p2)
        {
            Send(p1, p2, null);
        }

        public static void Send(P1 p1, P2 p2, object sender)
        {
            var msg = TypePool.root.Get<T>();
            msg.sender = sender;
            msg.p1 = p1;
            msg.p2 = p2;
            EventCenter.SendType(msg);
            TypePool.root.Return(msg);
        }

        public override void Clear()
        {
            base.Clear();
            p1 = default;
            p2 = default;
        }
    }

    /// <summary>
    /// 事件基类 - 3 个参数
    /// <para>注意：不要缓存事件消息实例，因为发送完毕之后会被对象池回收</para>
    /// </summary>
    public abstract class EventBase<T, P1, P2, P3> : EventBase<EventBase<T, P1, P2, P3>>
        where T : EventBase<T, P1, P2, P3>
    {
        public P1 p1;
        public P2 p2;
        public P3 p3;

        public static void Send(P1 p1, P2 p2, P3 p3)
        {
            Send(p1, p2, p3, null);
        }

        public static void Send(P1 p1, P2 p2, P3 p3, object sender)
        {
            var msg = TypePool.root.Get<T>();
            msg.sender = sender;
            msg.p1 = p1;
            msg.p2 = p2;
            msg.p3 = p3;
            EventCenter.SendType(msg);
            TypePool.root.Return(msg);
        }

        public override void Clear()
        {
            base.Clear();
            p1 = default;
            p2 = default;
            p3 = default;
        }
    }

    /// <summary>
    /// 事件基类 - 4 个参数
    /// <para>注意：不要缓存事件消息实例，因为发送完毕之后会被对象池回收</para>
    /// </summary>
    public abstract class EventBase<T, P1, P2, P3, P4> : EventBase<EventBase<T, P1, P2, P3, P4>>
        where T : EventBase<T, P1, P2, P3, P4>
    {
        public P1 p1;
        public P2 p2;
        public P3 p3;
        public P4 p4;

        public static void Send(P1 p1, P2 p2, P3 p3, P4 p4)
        {
            Send(p1, p2, p3, p4, null);
        }

        public static void Send(P1 p1, P2 p2, P3 p3, P4 p4, object sender)
        {
            var msg = TypePool.root.Get<T>();
            msg.sender = sender;
            msg.p1 = p1;
            msg.p2 = p2;
            msg.p3 = p3;
            msg.p4 = p4;
            EventCenter.SendType(msg);
            TypePool.root.Return(msg);
        }

        public override void Clear()
        {
            base.Clear();
            p1 = default;
            p2 = default;
            p3 = default;
            p4 = default;
        }
    }

}
