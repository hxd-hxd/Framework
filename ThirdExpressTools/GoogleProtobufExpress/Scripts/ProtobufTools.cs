using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Framework.Core;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;

using Type = System.Type;

namespace Framework.GoogleProtobufExpress
{
    /// <summary>
    /// �������л��ͷ����л� proto Э������
    /// </summary>
    public class ProtobufTools
    {
        static Dictionary<Type, IMessage> _msgDic = new Dictionary<Type, IMessage>();
        static Any _tempAny = new Any();

        public static Any GetAny(IMessage msg)
        {
            Any r = new Any();
            r.TypeUrl = msg.GetType().FullName;
            r.Value = msg.ToByteString();
            return r;
        }

        /// <summary>���л�</summary>
        /// <returns></returns>
        public static byte[] Serialize(IMessage msg)
        {
            if (msg == null) throw new ArgumentNullException("Serialize��bytes");
            var r = msg.ToByteArray();
            return r;
        }

        /// <summary>�� <paramref name="msg"/> ��װΪ <see cref="Any"/> �����л�</summary>
        /// <returns></returns>
        public static byte[] SerializeByAny(IMessage msg)
        {
            if (msg == null) throw new ArgumentNullException("SerializeByAny��msg");

            _tempAny.TypeUrl = msg.GetType().FullName;
            _tempAny.Value = msg.ToByteString();
            var r = _tempAny.ToByteArray();
            return r;
        }

        /// <summary>�����л� <see cref="Any"/>
        /// <para><paramref name="bytes"/>��<see cref="Any"/> ���л�������</para>
        /// </summary>
        /// <returns><see cref="Any.Value"/> ������������</returns>
        public static IMessage DeserializeByAny(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0) throw new ArgumentNullException("SerializeByAny��bytes");

            Any any = Deserialize<Any>(bytes);
            var r = Deserialize(any);
            return r;
        }
        /// <summary>�����л� <see cref="Any"/>
        /// <para><paramref name="bytes"/>��<see cref="Any"/> ���л�������</para>
        /// </summary>
        /// <returns><see cref="Any.Value"/> ������������</returns>
        public static IMessage DeserializeByAny(ReadOnlySpan<byte> bytes)
        {
            if (bytes == null || bytes.Length == 0) throw new ArgumentNullException("SerializeByAny��bytes");

            Any any = Deserialize<Any>(bytes);
            var r = Deserialize(any);
            return r;
        }

        /// <summary>�����л� <paramref name="any"/> �а�װ���� </summary>
        /// <returns></returns>
        public static IMessage Deserialize(Any any)
        {
            if (any == null) throw new ArgumentNullException("Deserialize��any");

            Type type = Type.GetType(any.TypeUrl);

            IMessage r = Deserialize(type, any.Value.ToByteArray());

            if (r == null)
            {
                var assems = AppDomain.CurrentDomain.GetAssemblies();
                foreach (var item in assems)
                {
                    type = item.GetType(any.TypeUrl);
                    if (type == null)
                    {
                        Log.Info($"δ�ڳ��� \"{item.FullName}\" ���ҵ����� \"{any.TypeUrl}\" ");
                        continue;
                    }

                    try
                    {
                        r = Deserialize(type, any.Value.ToByteArray());

                        if (r == null) continue;
                    }
                    catch (Exception)
                    {
                        continue;
                    }

                    if (r != null)
                        Log.Info($"�ڳ��� \"{item.FullName}\" ���ҵ����� \"{any.TypeUrl}\" ");
                }
            }

            return r;
        }

        /// <summary>�����л�</summary>
        /// <returns></returns>
        public static T Deserialize<T>(byte[] bytes) where T : IMessage, new()
        {
            T r = (T)Deserialize(typeof(T), bytes);
            return r;
        }
        /// <summary>�����л�</summary>
        /// <returns></returns>
        public static T Deserialize<T>(ReadOnlySpan<byte> bytes) where T : IMessage, new()
        {
            T r = (T)Deserialize(typeof(T), bytes);
            return r;
        }

        /// <summary>�����л�</summary>
        /// <returns></returns>
        public static IMessage Deserialize(string typeName, byte[] bytes)
        {
            Type type = Type.GetType(typeName);
            return Deserialize(type, bytes);
        }
        /// <summary>�����л�</summary>
        /// <returns></returns>
        public static IMessage Deserialize(string typeName, ReadOnlySpan<byte> bytes)
        {
            Type type = Type.GetType(typeName);
            return Deserialize(type, bytes);
        }

        /// <summary>�����л�</summary>
        /// <returns></returns>
        public static IMessage Deserialize(Type type, byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0) throw new ArgumentNullException("Deserialize��bytes");
            if (type == null) throw new ArgumentNullException("Deserialize��type");

            IMessage? msgDeserializer = null;
            _msgDic.TryGetValue(type, out msgDeserializer);
            if (msgDeserializer == null)
            {
                msgDeserializer = Activator.CreateInstance(type) as IMessage;
                _msgDic[type] = msgDeserializer;
            }

            if (msgDeserializer == null) throw new Exception($"Deserialize���޷��ҵ����� \"{type.FullName}\" �����л���");

            var r = msgDeserializer.Descriptor.Parser.ParseFrom(bytes);
            return r;
        }
        /// <summary>�����л�</summary>
        /// <returns></returns>
        public static IMessage Deserialize(Type type, ReadOnlySpan<byte> bytes)
        {
            if (bytes == null) throw new ArgumentNullException("Deserialize��bytes");
            if (type == null) throw new ArgumentNullException("Deserialize��type");

            IMessage? msgDeserializer = null;
            _msgDic.TryGetValue(type, out msgDeserializer);
            if (msgDeserializer == null)
            {
                msgDeserializer = Activator.CreateInstance(type) as IMessage;
                _msgDic[type] = msgDeserializer;
            }

            if (msgDeserializer == null) throw new Exception($"Deserialize���޷��ҵ����� \"{type.FullName}\" �����л���");

            var r = msgDeserializer.Descriptor.Parser.ParseFrom(bytes);
            return r;
        }
    }
}