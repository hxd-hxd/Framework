using System;
using System.Collections;
using System.Collections.Generic;
using Framework.Event;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.Test
{
    public class TestEvent : MonoBehaviour
    {
        public class Msg1 : IEventMessage
        {

        }
        public class Msg2 : IEventMessage
        {

        }

        private void Start()
        {
            Debug.Log("------------------------- ������� -------------------------");

            /// ������ָ�� id ������� Type.GetHashCode() Ϊ id
            EventCenter.AddListener<Msg1>(HandleEvent);
            EventCenter.AddListener<Msg1>(HandleMsg1);

            EventCenter.AddListener<Msg2>(HandleEvent);
            EventCenter.AddListener<Msg2>(HandleMsg2);
            // �����ظ����
            EventCenter.AddListener<Msg2>(HandleEvent);
            EventCenter.AddListener<Msg2>(HandleMsg2);

            /// ����ָ�� id

            // ��������д��ѡ��һ
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

            Debug.Log("------------------------- ������Ϣ -------------------------");
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

            EventCenter.Send("string2", "���", DateTime.Now.ToString());
            // ���������ʾ
            //EventCenter.Send("string2", new DateTime().AddDays(3), DateTime.Now);

            //Log.Error("------------------------- ����δ��������Ϣ -------------------------");
            //EventCenter.Send("Msg1_Msg1", new Msg1(), new Msg1());


            Debug.Log("------------------------- �Ƴ����� -------------------------");
            EventCenter.RemoveListener<Msg1>(HandleEvent);
            EventCenter.RemoveListener<Msg1>(HandleMsg1);

            EventCenter.RemoveListener<Msg2>(HandleEvent);
            EventCenter.RemoveListener<Msg2>(HandleMsg2);

            Debug.Log("------------------------- ������Ϣ -------------------------");
            EventCenter.Send(new Msg1());
            EventCenter.Send(new Msg2());
            EventCenter.Send("Msg1_Msg2", new Msg1(), new Msg2());
            EventCenter.Send("Msg2_Msg1", new Msg2(), new Msg1());


            //Log.Error("------------------------- ����δ��������Ϣ -------------------------");
            //EventCenter.Send("Msg1_Msg1", new Msg1(), new Msg1());

            ////Log.Error("------------------------- ���Ͳ���ǩ������ȷ����Ϣ -------------------------");
            //// �����Ϣ���շ��Ƿ��ͷ��ĸ��࣬������������յ���Ϣ
            //// ��֮�����쳣��ArgumentException: Object of type 'Framework.Test.TestEvent+Msg2' cannot be converted to type 'Framework.Test.TestEvent+Msg1'.
            ////EventCenter.Send("Msg1_Msg2", new Msg2(), new Msg1());

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
            Debug.Log($"ͨ�� ��Ϣ��object 20�� ����{msg1}��{msg2}��{msg3}��{msg4}��{msg5}��{msg6}��{msg7}��{msg8}��{msg9}��{msg10}��{msg11}��{msg12}��{msg13}��{msg14}��{msg15}��{msg16}��{msg17}��{msg18}��{msg19}��{msg20}");
        }

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



        public void Send()
        {

        }
    }
}
