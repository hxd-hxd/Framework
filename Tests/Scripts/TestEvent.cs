using System;
using System.Collections;
using System.Collections.Generic;
using Framework.Event;
using UnityEngine;

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
            Debug.Log("------------------------- ������� -------------------------");

            /* 
            ��� ͨ�� ��Ϣ�������������ж�����صķ���������Ҫָ��������Ӧ��ί�����͡�
            ��� ר�� ��Ϣ������������û�����صķ���������Ҫָ�����Ͳ������ͣ���ȻҲ����ָ��������Ӧ��ί�����͡�

            */

            /// ������ָ�� id ������� Type Ϊ id

            EventCenter.AddListener<MsgNot>((Action)HandleEvent);// �޲���Ϣ��ֻʹ����Ϣ������Ϊ id
            EventCenter.AddListener<MsgNot>(HandleEventNotMsg);

            EventCenter.AddListener((Action<Msg1>)HandleEvent);
            EventCenter.AddListener<Msg1>(HandleMsg1);

            EventCenter.AddListener((Action<Msg2>)HandleEvent);
            EventCenter.AddListener<Msg2>(HandleMsg2);
            // �����ظ����
            EventCenter.AddListener((Action<Msg2>)HandleEvent);
            EventCenter.AddListener<Msg2>(HandleMsg2);

            /// ����ָ�� id

            // ��������д��ѡ��һ��ע��ͬһ���������ᱻ�ظ����
            EventCenter.AddListener("ValueType", (Action<ValueType>)HandleEvent);
            EventCenter.AddListener<ValueType>("ValueType", HandleEvent);

            EventCenter.AddListener<ValueType, ValueType>("ValueType2", HandleEvent);
            EventCenter.AddListener<object, object>("object2", HandleEvent);
            EventCenter.AddListener<object, object>("string2", HandleEvent);

            EventCenter.AddListener<Msg1, Msg2>("Msg1_Msg2", HandleEvent);
            EventCenter.AddListener<string, Msg1, Msg2>("Msg1_Msg2", HandleMsg1_2);

            EventCenter.AddListener<Msg2, Msg1>("Msg2_Msg1", HandleEvent);
            EventCenter.AddListener<string, Msg2, Msg1>("Msg2_Msg1", HandleMsg2_1);

            EventCenter.AddListener<string, Msg1, Msg2, string>("Msg1_Msg2_Msg3", HandleEvent);


            Debug.Log("------------------------- ������Ϣ -------------------------");
            EventCenter.Send<MsgNot>();
            EventCenter.SendType(new Msg1());
            EventCenter.SendType(new Msg2());
            EventCenter.Send("Msg1_Msg2", new Msg1(), new Msg2());
            EventCenter.Send("Msg2_Msg1", new Msg2(), new Msg1());

            EventCenter.Send("ValueType", 1);
            EventCenter.Send("ValueType", 0.5f);
            EventCenter.Send("ValueType", DateTime.Now);
            EventCenter.Send("ValueType2", 1, 2f);
            EventCenter.Send("ValueType2", 0.5f, -0.5f);

            EventCenter.Send("ValueType2", new DateTime().AddDays(3), DateTime.Now);
            EventCenter.Send("object2", new DateTime().AddDays(3), DateTime.Now);

            EventCenter.Send("string2", "���", DateTime.Now.ToString());
            // ���������ʾ
            //EventCenter.Send("string2", new DateTime().AddDays(3), DateTime.Now);

            //Log.Error("------------------------- ����δ��������Ϣ -------------------------");
            //EventCenter.Send("Msg1_Msg1", new Msg1(), new Msg1());


            Debug.Log("------------------------- �Ƴ����� -------------------------");
            EventCenter.RemoveListener((Action<Msg1>)HandleEvent);
            EventCenter.RemoveListener<Msg1>(HandleMsg1);

            EventCenter.RemoveListener((Action<Msg2>)HandleEvent);
            EventCenter.RemoveListener<Msg2>(HandleMsg2);

            Debug.Log("------------------------- ������Ϣ -------------------------");
            EventCenter.SendType(new Msg1());
            EventCenter.SendType(new Msg2());
            EventCenter.Send("Msg1_Msg2", new Msg1(), new Msg2());
            EventCenter.Send("Msg2_Msg1", new Msg2(), new Msg1());


            //Log.Error("------------------------- ����δ��������Ϣ -------------------------");
            //EventCenter.Send("Msg1_Msg1", new Msg1(), new Msg1());

            ////Log.Error("------------------------- ���Ͳ���ǩ������ȷ����Ϣ -------------------------");
            //// �����Ϣ���շ��Ƿ��ͷ��ĸ��࣬������������յ���Ϣ
            //// ��֮�����쳣��ArgumentException: Object of type 'Framework.Test.TestEvent+Msg2' cannot be converted to type 'Framework.Test.TestEvent+Msg1'.
            ////EventCenter.Send("Msg1_Msg2", new Msg2(), new Msg1());


            Send20();

            SendEventGroup();

            Debug.Log("------------------------- ����������� -------------------------");
            EventCenter.Clear();
            //EventCenter.ClearAll();
            Debug.Log("------------------------- ������Ϣ -------------------------");
            EventCenter.Send("ValueType", 1);
            EventCenter.Send("ValueType", 0.5f);
            EventCenter.Send("ValueType", DateTime.Now);
            EventCenter.Send("ValueType2", 1, 2f);
            EventCenter.Send("ValueType2", 0.5f, -0.5f);

            EventCenter.Send("ValueType2", new DateTime().AddDays(3), DateTime.Now);
            EventCenter.Send("object2", new DateTime().AddDays(3), DateTime.Now);

            EventCenter.Send("string2", "���", DateTime.Now.ToString());
        }

        private void Send20()
        {
            Debug.Log("------------------------- �����������Ϣ -------------------------");

            // ���Ͳ�����������֧�� 16 �������� 16 ����������Ҫ�Զ���ί��
            EventCenter.AddListener("object 20", (CustomAction<object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object>)HandleEvent);
            EventCenter.AddListener("object 20", (CustomAction<object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object>)HandleEvent20);

            Debug.Log("------------------------- ������Ϣ -------------------------");
            //var msgs = new object[20] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 };
            var msgs = TypePool.root.GetArrayE<object>(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20);
            EventCenter.Send("object 20", msgs);

            Debug.Log("------------------------- �Ƴ����� -------------------------");
            EventCenter.RemoveListener("object 20", (CustomAction<object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object>)HandleEvent);
            Debug.Log("------------------------- ������Ϣ -------------------------");
            EventCenter.Send("object 20", msgs);

            TypePool.root.Return(msgs);
        }

        private void SendEventGroup()
        {
            Debug.Log("------------------------- �¼������Ϣ -------------------------");
            _eventGroup.AddListener("eg", HandleEG);
            _eventGroup.AddListener<string, Msg1>("eg_1", HandleEG_1);
            _eventGroup.AddListener<string, string>("eg_2", HandleEG_2);

            Debug.Log("------------------------- ������Ϣ -------------------------");
            EventCenter.Send("eg");
            EventCenter.Send("eg_1", new Msg1());
            EventCenter.Send("eg_2", "�¼�����Ϣ");

            Debug.Log("------------------------- ������� -------------------------");
            _eventGroup.Clear();

            Debug.Log("------------------------- ������Ϣ -------------------------");
            EventCenter.Send("eg");
            EventCenter.Send("eg_1", new Msg1());
            EventCenter.Send("eg_2", "�¼�����Ϣ");

        }

        public void HandleEvent(IEventMessage msg)
        {
            Debug.Log($"ͨ�� ��Ϣ��IEventMessage�� ����{msg}");
        }
        public void HandleEvent(IEventMessage msg1, IEventMessage msg2)
        {
            Debug.Log($"ͨ�� ��Ϣ��IEventMessage��IEventMessage�� ����{msg1}��{msg2}");
        }
        //public void HandleEvent(Msg1 msg1, Msg2 msg2)
        //{
        //    Debug.Log($"ר�� ��Ϣ��Msg1��Msg2�� ����{msg1}��{msg2}");
        //}

        public void HandleEvent()
        {
            Debug.Log($"ͨ�� ��Ϣ���޲Σ� ����");
        }
        public void HandleEventNotMsg()
        {
            Debug.Log($"ר�� ��Ϣ���޲Σ� ����");
        }
        public void HandleEvent(object msg)
        {
            Debug.Log($"ͨ�� ��Ϣ��object�� ����{msg}");
        }
        public void HandleEvent(object msg1, object msg2)
        {
            Debug.Log($"ͨ�� ��Ϣ��object��object�� ����{msg1}��{msg2}");
        }
        public void HandleEvent(object msg1, object msg2, object msg3)
        {
            Debug.Log($"ͨ�� ��Ϣ��object��object�� ����{msg1}��{msg2}��{msg3}");
        }
        public void HandleEvent(string msg1, string msg2)
        {
            Debug.Log($"ר�� ��Ϣ��string��string�� ����{msg1}��{msg2}");
        }

        public void HandleEvent(ValueType msg)
        {
            Debug.Log($"ͨ�� ��Ϣ��ValueTuple�� ����{msg}");
        }
        public void HandleEvent(ValueType msg1, ValueType msg2)
        {
            Debug.Log($"ͨ�� ��Ϣ��ValueTuple��ValueTuple�� ����{msg1}��{msg2}");
        }

        // �� 16 ����������Ϣ�����Զ���ί������
        public void HandleEvent(
            object msg1, object msg2, object msg3, object msg4, object msg5, object msg6, object msg7, object msg8, object msg9, object msg10,
            object msg11, object msg12, object msg13, object msg14, object msg15, object msg16, object msg17, object msg18, object msg19, object msg20)
        {
            Debug.Log($"ͨ�� ��Ϣ��object 20�� ����{msg1}��{msg2}��{msg3}��{msg4}��{msg5}��{msg6}��{msg7}��{msg8}��{msg9}��{msg10}��{msg11}��{msg12}��{msg13}��{msg14}��{msg15}��{msg16}��{msg17}��{msg18}��{msg19}��{msg20}");
        }
        public void HandleEvent20(
            object msg1, object msg2, object msg3, object msg4, object msg5, object msg6, object msg7, object msg8, object msg9, object msg10,
            object msg11, object msg12, object msg13, object msg14, object msg15, object msg16, object msg17, object msg18, object msg19, object msg20)
        {
            Debug.Log($"ר�� ��Ϣ��object 20�� ����{msg1}��{msg2}��{msg3}��{msg4}��{msg5}��{msg6}��{msg7}��{msg8}��{msg9}��{msg10}��{msg11}��{msg12}��{msg13}��{msg14}��{msg15}��{msg16}��{msg17}��{msg18}��{msg19}��{msg20}");
        }
        /// <summary>
        /// �Զ�������ί��
        /// </summary>
        public delegate void CustomAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20>(T1 ags1, T2 ags2, T3 ags3, T4 ags4, T5 ags5, T6 ags6, T7 ags7, T8 ags8, T9 ags9, T10 ags10, T11 ags11, T12 ags12, T13 ags13, T14 ags14, T15 ags15, T16 ags16, T17 ags17, T18 ags18, T19 ags19, T20 ags20);

        public void HandleMsg1(Msg1 msg)
        {
            Debug.Log($"ר�� ��Ϣ��Msg1�� ����{msg}");
        }
        public void HandleMsg2(Msg2 msg)
        {
            Debug.Log($"ר�� ��Ϣ��Msg2�� ����{msg}");
        }
        public void HandleMsg1_2(Msg1 msg1, Msg2 msg2)
        {
            Debug.Log($"ר�� ��Ϣ��Msg1��Msg2�� ����{msg1}��{msg2}");
        }
        public void HandleMsg2_1(Msg2 msg2, Msg1 msg1)
        {
            Debug.Log($"ר�� ��Ϣ��Msg2��Msg1�� ����{msg2}��{msg1}");
        }

        public void HandleEG()
        {
            Debug.Log($"�� ��Ϣ���޲Σ� ����");
        }
        public void HandleEG_1(Msg1 msg1)
        {
            Debug.Log($"�� ��Ϣ��Msg1�� ����{msg1}");
        }
        public void HandleEG_2(string msg1)
        {
            Debug.Log($"�� ��Ϣ��string�� ����{msg1}");
        }

        public void Send()
        {

        }

        class TestEventString
        {
            public TestEventString()
            {
                // �����÷�������ʹ��ϵͳ������Ϊ�¼� id
                EventCenter.AddListener<string>(Event);

                // �����ƶϳ�
                EventCenter.AddListener("0", Event);

                // �в������޷��ƶϳ��������ͣ�ֻ���Լ�ָ�����Ͳ���
                EventCenter.AddListener("1", (Action<int>)Event1);
                EventCenter.AddListener<string, int>("1", Event1);

                EventCenter.AddListener("2", (Action<int, string>)Event2);
                EventCenter.AddListener<string, int, string>("2", Event2);

                EventCenter.Send("0");
                EventCenter.Send("1", 6);
            }

            void Event()
            {

            }
            void Event1(int a1)
            {

            }
            void Event2(int a1, string a2)
            {

            }
        }
        class TestEventInt
        {
            public TestEventInt()
            {
                // �����÷�������ʹ��ϵͳ������Ϊ�¼� id
                EventCenter.AddListener<int>(Event);

                // �����ƶϳ�
                EventCenter.AddListener(0, Event);

                // �в������޷��ƶϳ��������ͣ�ֻ���Լ�ָ�����Ͳ���
                EventCenter.AddListener(1, (Action<int>)Event1);
                EventCenter.AddListener<int, int>(1, Event1);

                EventCenter.AddListener(2, (Action<int, string>)Event2);
                EventCenter.AddListener<int, int, string>(2, Event2);
            }

            void Event()
            {

            }
            void Event1(int a1)
            {

            }
            void Event2(int a1, string a2)
            {

            }
        }
        class TestEventEnum
        {
            public TestEventEnum()
            {
                // �����÷���ͬһ id �¼���ǩ������һ��
                EventCenter.AddListener<MsgType>(Event);
                EventCenter.AddListener<MsgType>(Event1);
                EventCenter.AddListener<Type>(typeof(MsgType), (Action<string>)Event);

                // �����ƶϳ�
                EventCenter.AddListener(MsgType.None, Event);

                // �в������޷��ƶϳ��������ͣ�ֻ���Լ�ָ�����Ͳ���
                EventCenter.AddListener(MsgType.A, (Action<int>)Event1);
                EventCenter.AddListener<MsgType, int>(MsgType.A, Event1);

                EventCenter.AddListener(MsgType.B, (Action<int, string>)Event2);
                EventCenter.AddListener<MsgType, int, string>(MsgType.B, Event2);

                EventCenter.Send(MsgType.A);
            }

            void Event()
            {

            }
            void Event(string v)
            {

            }
            void Event1(int a1)
            {

            }
            void Event1(MsgType a1)
            {

            }
            void Event2(int a1, string a2)
            {

            }

            enum MsgType
            {
                None = 0,
                A,
                B,
                C,
            }
        }
        class TestEventMsg
        {
            public TestEventMsg()
            {
                // ��ȷ�÷���ʹ���Զ���������Ϊ�¼� id
                EventCenter.AddListener<MsgID>(Event);
                // �����÷���ͬһ id �¼���ǩ������һ��
                EventCenter.AddListener<MsgID>(Event1);

                // �����÷���ǩ����һ��
                EventCenter.Send<MsgID>();


                /// �Ƽ��÷�
                // �������÷���һ����
                // ��������֧������������ԣ��ɽ���������Ϊ����
                EventCenter.AddListener<Msg>(Event1);
                EventCenter.AddListener<Type, Msg>(typeof(Msg), Event1);

                // �������÷���һ���ģ����в�ͬǩ����ͬ������ʱ����ʾָ��
                EventCenter.AddListener<Msg>((Action<IEventMessage>)Event1);
                EventCenter.AddListener<Type, IEventMessage>(typeof(Msg), Event1);

                // 
                EventCenter.AddListener((Action<IEventMessage>)Event1);
                EventCenter.AddListener<Type, Msg>(typeof(IEventMessage), Event1);
                EventCenter.AddListener<Type, IEventMessage>(typeof(IEventMessage), Event1);

                // ��ȷ�÷�
                EventCenter.SendType(new Msg());
                EventCenter.Send<Msg, Msg1>(new Msg1());
                EventCenter.SendType(new Msg1());
                EventCenter.Send<IEventMessage, Msg>(new Msg());
                // �����÷���ǩ����һ��
                EventCenter.Send<Msg>();
                EventCenter.Send<IEventMessage>();

            }

            void Event()
            {

            }
            void Event1(MsgID a1)
            {

            }
            void Event1(IEventMessage a1)
            {
                Debug.Log($"Event1��IEventMessage����{a1}");
            }
            void Event1(Msg a1)
            {
                Debug.Log($"Event1��Msg����{a1}");
            }
            void Event2(Msg a1, Msg a2)
            {

            }

            public class MsgID : IEventMessage
            {
                
            }
            public class Msg : IEventMessage
            {
                public int id = 0;

                
            }
            public class Msg1 : IEventMessage
            {
                public int id = 0;

                
            }
        }
    }
}
