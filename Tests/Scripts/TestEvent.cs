using System;
using System.Collections;
using System.Collections.Generic;
using Framework.Event;
using UnityEngine;
using UnityEngine.UI;
using static Framework.Test.TestEvent;

namespace Framework.Test
{
    public class TestEvent : MonoBehaviour
    {
        public class MsgNot : IEventMessage
        {

        }
        public class Msg1 : IEventMessage
        {

        }
        public class Msg2 : IEventMessage
        {

        }

        EventGroup _eventGroup = new EventGroup();

        private void Start()
        {
            Debug.Log("------------------------- 添加侦听 -------------------------");

            /* 
            添加 通用 消息处理函数（就是有多个重载的方法），需要指定函数对应的委托类型。
            添加 专用 消息处理函数（就是没有重载的方法），需要指定泛型参数类型，当然也可以指定函数对应的委托类型。

            */

            /// 不主动指定 id ，则会以 Type.GetHashCode() 为 id

            EventCenter.AddListener<MsgNot>((Action)HandleEvent);// 无参消息，只使用消息类型作为 id
            EventCenter.AddListener<MsgNot>(HandleEventNotMsg);

            EventCenter.AddListener((Action<Msg1>)HandleEvent);
            EventCenter.AddListener<Msg1>(HandleMsg1);

            EventCenter.AddListener((Action<Msg2>)HandleEvent);
            EventCenter.AddListener<Msg2>(HandleMsg2);
            // 不可重复添加
            EventCenter.AddListener((Action<Msg2>)HandleEvent);
            EventCenter.AddListener<Msg2>(HandleMsg2);

            /// 主动指定 id

            // 以下两种写法选其一
            //EventCenter.AddListener("ValueType", (Action<ValueType>)HandleEvent);
            EventCenter.AddListener<ValueType>("ValueType", HandleEvent);

            EventCenter.AddListener<ValueType, ValueType>("ValueType2", HandleEvent);
            EventCenter.AddListener<object, object>("object2", HandleEvent);
            EventCenter.AddListener<string, string>("string2", HandleEvent);

            EventCenter.AddListener<Msg1, Msg2>("Msg1_Msg2", HandleEvent);
            EventCenter.AddListener<Msg1, Msg2>("Msg1_Msg2", HandleMsg1_2);

            EventCenter.AddListener<Msg2, Msg1>("Msg2_Msg1", HandleEvent);
            EventCenter.AddListener<Msg2, Msg1>("Msg2_Msg1", HandleMsg2_1);

            EventCenter.AddListener<Msg1, Msg2, string>("Msg1_Msg2_Msg3", HandleEvent);


            Debug.Log("------------------------- 发送消息 -------------------------");
            EventCenter.Send<MsgNot>();
            EventCenter.Send(new Msg1());
            EventCenter.Send(new Msg2());
            EventCenter.Send("Msg1_Msg2", new Msg1(), new Msg2());
            EventCenter.Send("Msg2_Msg1", new Msg2(), new Msg1());

            EventCenter.Send("ValueType", 1);
            EventCenter.Send("ValueType", 0.5f);
            EventCenter.Send("ValueType", DateTime.Now);
            EventCenter.Send("ValueType2", 1, 2f);
            EventCenter.Send("ValueType2", 0.5f, -0.5f);

            EventCenter.Send("ValueType2", new DateTime().AddDays(3), DateTime.Now);
            EventCenter.Send("object2", new DateTime().AddDays(3), DateTime.Now);

            EventCenter.Send("string2", "你好", DateTime.Now.ToString());
            // 错误参数演示
            //EventCenter.Send("string2", new DateTime().AddDays(3), DateTime.Now);

            //Log.Error("------------------------- 发送未侦听的消息 -------------------------");
            //EventCenter.Send("Msg1_Msg1", new Msg1(), new Msg1());


            Debug.Log("------------------------- 移除侦听 -------------------------");
            EventCenter.RemoveListener((Action<Msg1>)HandleEvent);
            EventCenter.RemoveListener<Msg1>(HandleMsg1);

            EventCenter.RemoveListener((Action<Msg2>)HandleEvent);
            EventCenter.RemoveListener<Msg2>(HandleMsg2);

            Debug.Log("------------------------- 发送消息 -------------------------");
            EventCenter.Send(new Msg1());
            EventCenter.Send(new Msg2());
            EventCenter.Send("Msg1_Msg2", new Msg1(), new Msg2());
            EventCenter.Send("Msg2_Msg1", new Msg2(), new Msg1());


            //Log.Error("------------------------- 发送未侦听的消息 -------------------------");
            //EventCenter.Send("Msg1_Msg1", new Msg1(), new Msg1());

            ////Log.Error("------------------------- 发送参数签名不正确的消息 -------------------------");
            //// 如果消息接收方是发送方的父类，则可以正常接收到消息
            //// 反之引发异常：ArgumentException: Object of type 'Framework.Test.TestEvent+Msg2' cannot be converted to type 'Framework.Test.TestEvent+Msg1'.
            ////EventCenter.Send("Msg1_Msg2", new Msg2(), new Msg1());


            Send20();

            SendEventGroup();
        }

        private void Send20()
        {
            Debug.Log("------------------------- 超多参数的消息 -------------------------");

            // 泛型参数类型至多支持 16 个，多余 16 各参数的需要自定义委托
            EventCenter.AddListener("object 20", (CustomAction<object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object>)HandleEvent);
            EventCenter.AddListener("object 20", (CustomAction<object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object>)HandleEvent20);

            Debug.Log("------------------------- 发送消息 -------------------------");
            //var msgs = new object[20] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 };
            var msgs = TypePool.root.GetArrayE<object>(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20);
            EventCenter.Send("object 20", msgs);

            Debug.Log("------------------------- 移除侦听 -------------------------");
            EventCenter.RemoveListener("object 20", (CustomAction<object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object>)HandleEvent);
            Debug.Log("------------------------- 发送消息 -------------------------");
            EventCenter.Send("object 20", msgs);

            TypePool.root.Return(msgs);
        }

        private void SendEventGroup()
        {
            Debug.Log("------------------------- 事件组的消息 -------------------------");
            _eventGroup.AddListener("eg", HandleEG);
            _eventGroup.AddListener<Msg1>("eg_1", HandleEG_1);
            _eventGroup.AddListener<string>("eg_2", HandleEG_2);

            Debug.Log("------------------------- 发送消息 -------------------------");
            EventCenter.Send("eg");
            EventCenter.Send("eg_1", new Msg1());
            EventCenter.Send("eg_2", "事件组消息");

            Debug.Log("------------------------- 移除侦听 -------------------------");
            _eventGroup.Clear();

            Debug.Log("------------------------- 发送消息 -------------------------");
            EventCenter.Send("eg");
            EventCenter.Send("eg_1", new Msg1());
            EventCenter.Send("eg_2", "事件组消息");

        }

        public void HandleEvent(IEventMessage msg)
        {
            Debug.Log($"通用 消息（IEventMessage） 处理：{msg}");
        }
        public void HandleEvent(IEventMessage msg1, IEventMessage msg2)
        {
            Debug.Log($"通用 消息（IEventMessage、IEventMessage） 处理：{msg1}，{msg2}");
        }
        //public void HandleEvent(Msg1 msg1, Msg2 msg2)
        //{
        //    Debug.Log($"专用 消息（Msg1、Msg2） 处理：{msg1}，{msg2}");
        //}

        public void HandleEvent()
        {
            Debug.Log($"通用 消息（无参） 处理");
        }
        public void HandleEventNotMsg()
        {
            Debug.Log($"专用 消息（无参） 处理");
        }
        public void HandleEvent(object msg)
        {
            Debug.Log($"通用 消息（object） 处理：{msg}");
        }
        public void HandleEvent(object msg1, object msg2)
        {
            Debug.Log($"通用 消息（object、object） 处理：{msg1}，{msg2}");
        }
        public void HandleEvent(object msg1, object msg2, object msg3)
        {
            Debug.Log($"通用 消息（object、object） 处理：{msg1}，{msg2}，{msg3}");
        }
        public void HandleEvent(string msg1, string msg2)
        {
            Debug.Log($"专用 消息（string、string） 处理：{msg1}，{msg2}");
        }

        public void HandleEvent(ValueType msg)
        {
            Debug.Log($"通用 消息（ValueTuple） 处理：{msg}");
        }
        public void HandleEvent(ValueType msg1, ValueType msg2)
        {
            Debug.Log($"通用 消息（ValueTuple、ValueTuple） 处理：{msg1}，{msg2}");
        }

        // 超 16 个参数的消息，需自定义委托类型
        public void HandleEvent(
            object msg1, object msg2, object msg3, object msg4, object msg5, object msg6, object msg7, object msg8, object msg9, object msg10,
            object msg11, object msg12, object msg13, object msg14, object msg15, object msg16, object msg17, object msg18, object msg19, object msg20)
        {
            Debug.Log($"通用 消息（object 20） 处理：{msg1}，{msg2}，{msg3}，{msg4}，{msg5}，{msg6}，{msg7}，{msg8}，{msg9}，{msg10}，{msg11}，{msg12}，{msg13}，{msg14}，{msg15}，{msg16}，{msg17}，{msg18}，{msg19}，{msg20}");
        }
        public void HandleEvent20(
            object msg1, object msg2, object msg3, object msg4, object msg5, object msg6, object msg7, object msg8, object msg9, object msg10,
            object msg11, object msg12, object msg13, object msg14, object msg15, object msg16, object msg17, object msg18, object msg19, object msg20)
        {
            Debug.Log($"专用 消息（object 20） 处理：{msg1}，{msg2}，{msg3}，{msg4}，{msg5}，{msg6}，{msg7}，{msg8}，{msg9}，{msg10}，{msg11}，{msg12}，{msg13}，{msg14}，{msg15}，{msg16}，{msg17}，{msg18}，{msg19}，{msg20}");
        }
        /// <summary>
        /// 自定义多参数委托
        /// </summary>
        public delegate void CustomAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20>(T1 ags1, T2 ags2, T3 ags3, T4 ags4, T5 ags5, T6 ags6, T7 ags7, T8 ags8, T9 ags9, T10 ags10, T11 ags11, T12 ags12, T13 ags13, T14 ags14, T15 ags15, T16 ags16, T17 ags17, T18 ags18, T19 ags19, T20 ags20);

        public void HandleMsg1(Msg1 msg)
        {
            Debug.Log($"专用 消息（Msg1） 处理：{msg}");
        }
        public void HandleMsg2(Msg2 msg)
        {
            Debug.Log($"专用 消息（Msg2） 处理：{msg}");
        }
        public void HandleMsg1_2(Msg1 msg1, Msg2 msg2)
        {
            Debug.Log($"专用 消息（Msg1、Msg2） 处理：{msg1}，{msg2}");
        }
        public void HandleMsg2_1(Msg2 msg2, Msg1 msg1)
        {
            Debug.Log($"专用 消息（Msg2、Msg1） 处理：{msg2}，{msg1}");
        }

        public void HandleEG()
        {
            Debug.Log($"组 消息（无参） 处理");
        }
        public void HandleEG_1(Msg1 msg1)
        {
            Debug.Log($"组 消息（Msg1） 处理：{msg1}");
        }
        public void HandleEG_2(string msg1)
        {
            Debug.Log($"组 消息（string） 处理：{msg1}");
        }

        public void Send()
        {

        }
    }
}
