
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

        /// <summary>清除所有监听</summary>
        public static void ClearListener()
        {
            EventCenter.Clear<T>();
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
    public abstract class EventBase<T, P1> : ITypePoolObject, IEventMessage
        where T : EventBase<T, P1>
    {
        /// <summary>
        /// 发送者
        /// </summary>
        public object sender;

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

        public static void AddListener(Action<T> listener)
        {
            EventCenter.AddListener(listener);
        }

        public static void RemoveListener(Action<T> listener)
        {
            EventCenter.RemoveListener(listener);
        }

        /// <summary>清除所有监听</summary>
        public static void ClearListener()
        {
            EventCenter.Clear<T>();
        }

        public virtual void Clear()
        {
            sender = null;
            p1 = default;
        }
    }

    /// <summary>
    /// 事件基类 - 2 个参数
    /// <para>注意：不要缓存事件消息实例，因为发送完毕之后会被对象池回收</para>
    /// </summary>
    public abstract class EventBase<T, P1, P2> : ITypePoolObject, IEventMessage
        where T : EventBase<T, P1, P2>
    {
        /// <summary>
        /// 发送者
        /// </summary>
        public object sender;

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

        public static void AddListener(Action<T> listener)
        {
            EventCenter.AddListener(listener);
        }

        public static void RemoveListener(Action<T> listener)
        {
            EventCenter.RemoveListener(listener);
        }

        /// <summary>清除所有监听</summary>
        public static void ClearListener()
        {
            EventCenter.Clear<T>();
        }

        public virtual void Clear()
        {
            sender = null;
            p1 = default;
            p2 = default;
        }
    }

    /// <summary>
    /// 事件基类 - 3 个参数
    /// <para>注意：不要缓存事件消息实例，因为发送完毕之后会被对象池回收</para>
    /// </summary>
    public abstract class EventBase<T, P1, P2, P3> : ITypePoolObject, IEventMessage
        where T : EventBase<T, P1, P2, P3>
    {
        /// <summary>
        /// 发送者
        /// </summary>
        public object sender;

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

        public static void AddListener(Action<T> listener)
        {
            EventCenter.AddListener(listener);
        }

        public static void RemoveListener(Action<T> listener)
        {
            EventCenter.RemoveListener(listener);
        }

        /// <summary>清除所有监听</summary>
        public static void ClearListener()
        {
            EventCenter.Clear<T>();
        }

        public virtual void Clear()
        {
            sender = null;
            p1 = default;
            p2 = default;
            p3 = default;
        }
    }

    /// <summary>
    /// 事件基类 - 4 个参数
    /// <para>注意：不要缓存事件消息实例，因为发送完毕之后会被对象池回收</para>
    /// </summary>
    public abstract class EventBase<T, P1, P2, P3, P4> : ITypePoolObject, IEventMessage
        where T : EventBase<T, P1, P2, P3, P4>
    {
        /// <summary>
        /// 发送者
        /// </summary>
        public object sender;

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

        public static void AddListener(Action<T> listener)
        {
            EventCenter.AddListener(listener);
        }

        public static void RemoveListener(Action<T> listener)
        {
            EventCenter.RemoveListener(listener);
        }

        /// <summary>清除所有监听</summary>
        public static void ClearListener()
        {
            EventCenter.Clear<T>();
        }

        public virtual void Clear()
        {
            sender = null;
            p1 = default;
            p2 = default;
            p3 = default;
            p4 = default;
        }
    }

    /// <summary>
    /// 事件基类 - 5 个参数
    /// <para>注意：不要缓存事件消息实例，因为发送完毕之后会被对象池回收</para>
    /// </summary>
    public abstract class EventBase<T, P1, P2, P3, P4, P5> : ITypePoolObject, IEventMessage
        where T : EventBase<T, P1, P2, P3, P4, P5>
    {
        /// <summary>
        /// 发送者
        /// </summary>
        public object sender;

        public P1 p1;
        public P2 p2;
        public P3 p3;
        public P4 p4;
        public P5 p5;

        public static void Send(P1 p1, P2 p2, P3 p3, P4 p4, P5 p5)
        {
            Send(p1, p2, p3, p4, p5, null);
        }

        public static void Send(P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, object sender)
        {
            var msg = TypePool.root.Get<T>();
            msg.sender = sender;
            msg.p1 = p1;
            msg.p2 = p2;
            msg.p3 = p3;
            msg.p4 = p4;
            msg.p5 = p5;
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

        /// <summary>清除所有监听</summary>
        public static void ClearListener()
        {
            EventCenter.Clear<T>();
        }

        public virtual void Clear()
        {
            sender = null;
            p1 = default;
            p2 = default;
            p3 = default;
            p4 = default;
            p5 = default;
        }
    }

    /// <summary>
    /// 事件基类 - 6 个参数
    /// <para>注意：不要缓存事件消息实例，因为发送完毕之后会被对象池回收</para>
    /// </summary>
    public abstract class EventBase<T, P1, P2, P3, P4, P5, P6> : ITypePoolObject, IEventMessage
        where T : EventBase<T, P1, P2, P3, P4, P5, P6>
    {
        /// <summary>
        /// 发送者
        /// </summary>
        public object sender;

        public P1 p1;
        public P2 p2;
        public P3 p3;
        public P4 p4;
        public P5 p5;
        public P6 p6;

        public static void Send(P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6)
        {
            Send(p1, p2, p3, p4, p5, p6, null);
        }

        public static void Send(P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, object sender)
        {
            var msg = TypePool.root.Get<T>();
            msg.sender = sender;
            msg.p1 = p1;
            msg.p2 = p2;
            msg.p3 = p3;
            msg.p4 = p4;
            msg.p5 = p5;
            msg.p6 = p6;
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

        /// <summary>清除所有监听</summary>
        public static void ClearListener()
        {
            EventCenter.Clear<T>();
        }

        public virtual void Clear()
        {
            sender = null;
            p1 = default;
            p2 = default;
            p3 = default;
            p4 = default;
            p5 = default;
            p6 = default;
        }
    }

    /// <summary>
    /// 事件基类 - 7 个参数
    /// <para>注意：不要缓存事件消息实例，因为发送完毕之后会被对象池回收</para>
    /// </summary>
    public abstract class EventBase<T, P1, P2, P3, P4, P5, P6, P7> : ITypePoolObject, IEventMessage
        where T : EventBase<T, P1, P2, P3, P4, P5, P6, P7>
    {
        /// <summary>
        /// 发送者
        /// </summary>
        public object sender;

        public P1 p1;
        public P2 p2;
        public P3 p3;
        public P4 p4;
        public P5 p5;
        public P6 p6;
        public P7 p7;

        public static void Send(P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7)
        {
            Send(p1, p2, p3, p4, p5, p6, p7, null);
        }

        public static void Send(P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, object sender)
        {
            var msg = TypePool.root.Get<T>();
            msg.sender = sender;
            msg.p1 = p1;
            msg.p2 = p2;
            msg.p3 = p3;
            msg.p4 = p4;
            msg.p5 = p5;
            msg.p6 = p6;
            msg.p7 = p7;
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

        /// <summary>清除所有监听</summary>
        public static void ClearListener()
        {
            EventCenter.Clear<T>();
        }

        public virtual void Clear()
        {
            sender = null;
            p1 = default;
            p2 = default;
            p3 = default;
            p4 = default;
            p5 = default;
            p6 = default;
            p7 = default;
        }
    }

    /// <summary>
    /// 事件基类 - 8 个参数
    /// <para>注意：不要缓存事件消息实例，因为发送完毕之后会被对象池回收</para>
    /// </summary>
    public abstract class EventBase<T, P1, P2, P3, P4, P5, P6, P7, P8> : ITypePoolObject, IEventMessage
        where T : EventBase<T, P1, P2, P3, P4, P5, P6, P7, P8>
    {
        /// <summary>
        /// 发送者
        /// </summary>
        public object sender;

        public P1 p1;
        public P2 p2;
        public P3 p3;
        public P4 p4;
        public P5 p5;
        public P6 p6;
        public P7 p7;
        public P8 p8;

        public static void Send(P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8)
        {
            Send(p1, p2, p3, p4, p5, p6, p7, p8, null);
        }

        public static void Send(P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, object sender)
        {
            var msg = TypePool.root.Get<T>();
            msg.sender = sender;
            msg.p1 = p1;
            msg.p2 = p2;
            msg.p3 = p3;
            msg.p4 = p4;
            msg.p5 = p5;
            msg.p6 = p6;
            msg.p7 = p7;
            msg.p8 = p8;
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

        /// <summary>清除所有监听</summary>
        public static void ClearListener()
        {
            EventCenter.Clear<T>();
        }

        public virtual void Clear()
        {
            sender = null;
            p1 = default;
            p2 = default;
            p3 = default;
            p4 = default;
            p5 = default;
            p6 = default;
            p7 = default;
            p8 = default;
        }
    }

    /// <summary>
    /// 事件基类 - 9 个参数
    /// <para>注意：不要缓存事件消息实例，因为发送完毕之后会被对象池回收</para>
    /// </summary>
    public abstract class EventBase<T, P1, P2, P3, P4, P5, P6, P7, P8, P9> : ITypePoolObject, IEventMessage
        where T : EventBase<T, P1, P2, P3, P4, P5, P6, P7, P8, P9>
    {
        /// <summary>
        /// 发送者
        /// </summary>
        public object sender;

        public P1 p1;
        public P2 p2;
        public P3 p3;
        public P4 p4;
        public P5 p5;
        public P6 p6;
        public P7 p7;
        public P8 p8;
        public P9 p9;

        public static void Send(P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9)
        {
            Send(p1, p2, p3, p4, p5, p6, p7, p8, p9, null);
        }

        public static void Send(P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, object sender)
        {
            var msg = TypePool.root.Get<T>();
            msg.sender = sender;
            msg.p1 = p1;
            msg.p2 = p2;
            msg.p3 = p3;
            msg.p4 = p4;
            msg.p5 = p5;
            msg.p6 = p6;
            msg.p7 = p7;
            msg.p8 = p8;
            msg.p9 = p9;
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

        /// <summary>清除所有监听</summary>
        public static void ClearListener()
        {
            EventCenter.Clear<T>();
        }

        public virtual void Clear()
        {
            sender = null;
            p1 = default;
            p2 = default;
            p3 = default;
            p4 = default;
            p5 = default;
            p6 = default;
            p7 = default;
            p8 = default;
            p9 = default;
        }
    }

    /// <summary>
    /// 事件基类 - 10 个参数
    /// <para>注意：不要缓存事件消息实例，因为发送完毕之后会被对象池回收</para>
    /// </summary>
    public abstract class EventBase<T, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10> : ITypePoolObject, IEventMessage
        where T : EventBase<T, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>
    {
        /// <summary>
        /// 发送者
        /// </summary>
        public object sender;

        public P1 p1;
        public P2 p2;
        public P3 p3;
        public P4 p4;
        public P5 p5;
        public P6 p6;
        public P7 p7;
        public P8 p8;
        public P9 p9;
        public P10 p10;

        public static void Send(P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10)
        {
            Send(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, null);
        }

        public static void Send(P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10, object sender)
        {
            var msg = TypePool.root.Get<T>();
            msg.sender = sender;
            msg.p1 = p1;
            msg.p2 = p2;
            msg.p3 = p3;
            msg.p4 = p4;
            msg.p5 = p5;
            msg.p6 = p6;
            msg.p7 = p7;
            msg.p8 = p8;
            msg.p9 = p9;
            msg.p10 = p10;
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

        /// <summary>清除所有监听</summary>
        public static void ClearListener()
        {
            EventCenter.Clear<T>();
        }

        public virtual void Clear()
        {
            sender = null;
            p1 = default;
            p2 = default;
            p3 = default;
            p4 = default;
            p5 = default;
            p6 = default;
            p7 = default;
            p8 = default;
            p9 = default;
            p10 = default;
        }
    }

    /// <summary>
    /// 事件基类 - 11 个参数
    /// <para>注意：不要缓存事件消息实例，因为发送完毕之后会被对象池回收</para>
    /// </summary>
    public abstract class EventBase<T, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11> : ITypePoolObject, IEventMessage
        where T : EventBase<T, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>
    {
        /// <summary>
        /// 发送者
        /// </summary>
        public object sender;

        public P1 p1;
        public P2 p2;
        public P3 p3;
        public P4 p4;
        public P5 p5;
        public P6 p6;
        public P7 p7;
        public P8 p8;
        public P9 p9;
        public P10 p10;
        public P11 p11;

        public static void Send(P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10, P11 p11)
        {
            Send(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, null);
        }

        public static void Send(P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10, P11 p11, object sender)
        {
            var msg = TypePool.root.Get<T>();
            msg.sender = sender;
            msg.p1 = p1;
            msg.p2 = p2;
            msg.p3 = p3;
            msg.p4 = p4;
            msg.p5 = p5;
            msg.p6 = p6;
            msg.p7 = p7;
            msg.p8 = p8;
            msg.p9 = p9;
            msg.p10 = p10;
            msg.p11 = p11;
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

        /// <summary>清除所有监听</summary>
        public static void ClearListener()
        {
            EventCenter.Clear<T>();
        }

        public virtual void Clear()
        {
            sender = null;
            p1 = default;
            p2 = default;
            p3 = default;
            p4 = default;
            p5 = default;
            p6 = default;
            p7 = default;
            p8 = default;
            p9 = default;
            p10 = default;
            p11 = default;
        }
    }

    /// <summary>
    /// 事件基类 - 12 个参数
    /// <para>注意：不要缓存事件消息实例，因为发送完毕之后会被对象池回收</para>
    /// </summary>
    public abstract class EventBase<T, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12> : ITypePoolObject, IEventMessage
        where T : EventBase<T, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12>
    {
        /// <summary>
        /// 发送者
        /// </summary>
        public object sender;

        public P1 p1;
        public P2 p2;
        public P3 p3;
        public P4 p4;
        public P5 p5;
        public P6 p6;
        public P7 p7;
        public P8 p8;
        public P9 p9;
        public P10 p10;
        public P11 p11;
        public P12 p12;

        public static void Send(P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10, P11 p11, P12 p12)
        {
            Send(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, null);
        }

        public static void Send(P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10, P11 p11, P12 p12, object sender)
        {
            var msg = TypePool.root.Get<T>();
            msg.sender = sender;
            msg.p1 = p1;
            msg.p2 = p2;
            msg.p3 = p3;
            msg.p4 = p4;
            msg.p5 = p5;
            msg.p6 = p6;
            msg.p7 = p7;
            msg.p8 = p8;
            msg.p9 = p9;
            msg.p10 = p10;
            msg.p11 = p11;
            msg.p12 = p12;
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

        /// <summary>清除所有监听</summary>
        public static void ClearListener()
        {
            EventCenter.Clear<T>();
        }

        public virtual void Clear()
        {
            sender = null;
            p1 = default;
            p2 = default;
            p3 = default;
            p4 = default;
            p5 = default;
            p6 = default;
            p7 = default;
            p8 = default;
            p9 = default;
            p10 = default;
            p11 = default;
            p12 = default;
        }
    }

    /// <summary>
    /// 事件基类 - 13 个参数
    /// <para>注意：不要缓存事件消息实例，因为发送完毕之后会被对象池回收</para>
    /// </summary>
    public abstract class EventBase<T, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13> : ITypePoolObject, IEventMessage
        where T : EventBase<T, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13>
    {
        /// <summary>
        /// 发送者
        /// </summary>
        public object sender;

        public P1 p1;
        public P2 p2;
        public P3 p3;
        public P4 p4;
        public P5 p5;
        public P6 p6;
        public P7 p7;
        public P8 p8;
        public P9 p9;
        public P10 p10;
        public P11 p11;
        public P12 p12;
        public P13 p13;

        public static void Send(P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10, P11 p11, P12 p12, P13 p13)
        {
            Send(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, null);
        }

        public static void Send(P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10, P11 p11, P12 p12, P13 p13, object sender)
        {
            var msg = TypePool.root.Get<T>();
            msg.sender = sender;
            msg.p1 = p1;
            msg.p2 = p2;
            msg.p3 = p3;
            msg.p4 = p4;
            msg.p5 = p5;
            msg.p6 = p6;
            msg.p7 = p7;
            msg.p8 = p8;
            msg.p9 = p9;
            msg.p10 = p10;
            msg.p11 = p11;
            msg.p12 = p12;
            msg.p13 = p13;
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

        /// <summary>清除所有监听</summary>
        public static void ClearListener()
        {
            EventCenter.Clear<T>();
        }

        public virtual void Clear()
        {
            sender = null;
            p1 = default;
            p2 = default;
            p3 = default;
            p4 = default;
            p5 = default;
            p6 = default;
            p7 = default;
            p8 = default;
            p9 = default;
            p10 = default;
            p11 = default;
            p12 = default;
            p13 = default;
        }
    }

    /// <summary>
    /// 事件基类 - 14 个参数
    /// <para>注意：不要缓存事件消息实例，因为发送完毕之后会被对象池回收</para>
    /// </summary>
    public abstract class EventBase<T, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, P14> : ITypePoolObject, IEventMessage
        where T : EventBase<T, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, P14>
    {
        /// <summary>
        /// 发送者
        /// </summary>
        public object sender;

        public P1 p1;
        public P2 p2;
        public P3 p3;
        public P4 p4;
        public P5 p5;
        public P6 p6;
        public P7 p7;
        public P8 p8;
        public P9 p9;
        public P10 p10;
        public P11 p11;
        public P12 p12;
        public P13 p13;
        public P14 p14;

        public static void Send(P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10, P11 p11, P12 p12, P13 p13, P14 p14)
        {
            Send(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, null);
        }

        public static void Send(P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10, P11 p11, P12 p12, P13 p13, P14 p14, object sender)
        {
            var msg = TypePool.root.Get<T>();
            msg.sender = sender;
            msg.p1 = p1;
            msg.p2 = p2;
            msg.p3 = p3;
            msg.p4 = p4;
            msg.p5 = p5;
            msg.p6 = p6;
            msg.p7 = p7;
            msg.p8 = p8;
            msg.p9 = p9;
            msg.p10 = p10;
            msg.p11 = p11;
            msg.p12 = p12;
            msg.p13 = p13;
            msg.p14 = p14;
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

        /// <summary>清除所有监听</summary>
        public static void ClearListener()
        {
            EventCenter.Clear<T>();
        }

        public virtual void Clear()
        {
            sender = null;
            p1 = default;
            p2 = default;
            p3 = default;
            p4 = default;
            p5 = default;
            p6 = default;
            p7 = default;
            p8 = default;
            p9 = default;
            p10 = default;
            p11 = default;
            p12 = default;
            p13 = default;
            p14 = default;
        }
    }

    /// <summary>
    /// 事件基类 - 15 个参数
    /// <para>注意：不要缓存事件消息实例，因为发送完毕之后会被对象池回收</para>
    /// </summary>
    public abstract class EventBase<T, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, P14, P15> : ITypePoolObject, IEventMessage
        where T : EventBase<T, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, P14, P15>
    {
        /// <summary>
        /// 发送者
        /// </summary>
        public object sender;

        public P1 p1;
        public P2 p2;
        public P3 p3;
        public P4 p4;
        public P5 p5;
        public P6 p6;
        public P7 p7;
        public P8 p8;
        public P9 p9;
        public P10 p10;
        public P11 p11;
        public P12 p12;
        public P13 p13;
        public P14 p14;
        public P15 p15;

        public static void Send(P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10, P11 p11, P12 p12, P13 p13, P14 p14, P15 p15)
        {
            Send(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15, null);
        }

        public static void Send(P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10, P11 p11, P12 p12, P13 p13, P14 p14, P15 p15, object sender)
        {
            var msg = TypePool.root.Get<T>();
            msg.sender = sender;
            msg.p1 = p1;
            msg.p2 = p2;
            msg.p3 = p3;
            msg.p4 = p4;
            msg.p5 = p5;
            msg.p6 = p6;
            msg.p7 = p7;
            msg.p8 = p8;
            msg.p9 = p9;
            msg.p10 = p10;
            msg.p11 = p11;
            msg.p12 = p12;
            msg.p13 = p13;
            msg.p14 = p14;
            msg.p15 = p15;
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

        /// <summary>清除所有监听</summary>
        public static void ClearListener()
        {
            EventCenter.Clear<T>();
        }

        public virtual void Clear()
        {
            sender = null;
            p1 = default;
            p2 = default;
            p3 = default;
            p4 = default;
            p5 = default;
            p6 = default;
            p7 = default;
            p8 = default;
            p9 = default;
            p10 = default;
            p11 = default;
            p12 = default;
            p13 = default;
            p14 = default;
            p15 = default;
        }
    }

    /// <summary>
    /// 事件基类 - 16 个参数
    /// <para>注意：不要缓存事件消息实例，因为发送完毕之后会被对象池回收</para>
    /// </summary>
    public abstract class EventBase<T, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, P14, P15, P16> : ITypePoolObject, IEventMessage
        where T : EventBase<T, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, P14, P15, P16>
    {
        /// <summary>
        /// 发送者
        /// </summary>
        public object sender;

        public P1 p1;
        public P2 p2;
        public P3 p3;
        public P4 p4;
        public P5 p5;
        public P6 p6;
        public P7 p7;
        public P8 p8;
        public P9 p9;
        public P10 p10;
        public P11 p11;
        public P12 p12;
        public P13 p13;
        public P14 p14;
        public P15 p15;
        public P16 p16;

        public static void Send(P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10, P11 p11, P12 p12, P13 p13, P14 p14, P15 p15, P16 p16)
        {
            Send(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15, p16, null);
        }

        public static void Send(P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10, P11 p11, P12 p12, P13 p13, P14 p14, P15 p15, P16 p16, object sender)
        {
            var msg = TypePool.root.Get<T>();
            msg.sender = sender;
            msg.p1 = p1;
            msg.p2 = p2;
            msg.p3 = p3;
            msg.p4 = p4;
            msg.p5 = p5;
            msg.p6 = p6;
            msg.p7 = p7;
            msg.p8 = p8;
            msg.p9 = p9;
            msg.p10 = p10;
            msg.p11 = p11;
            msg.p12 = p12;
            msg.p13 = p13;
            msg.p14 = p14;
            msg.p15 = p15;
            msg.p16 = p16;
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

        /// <summary>清除所有监听</summary>
        public static void ClearListener()
        {
            EventCenter.Clear<T>();
        }

        public virtual void Clear()
        {
            sender = null;
            p1 = default;
            p2 = default;
            p3 = default;
            p4 = default;
            p5 = default;
            p6 = default;
            p7 = default;
            p8 = default;
            p9 = default;
            p10 = default;
            p11 = default;
            p12 = default;
            p13 = default;
            p14 = default;
            p15 = default;
            p16 = default;
        }
    }

}
