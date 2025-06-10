using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.Core.Network
{
    // ����ָ�������������Ĳ���
    public partial class ByteBuffer
    {
        #region ָ������������д��

        /// <summary>��ָ���Ļ����� <paramref name="index"/> ����  <paramref name="bytes"/> �ֽ����� <paramref name="startIndex"/> ��ʼ�� <paramref name="length"/> �ֽ�д�뵽�˻�����</summary>
        public void Write(int index, byte[] bytes, int startIndex, int length)
        {
            //if (bytes == null) throw new ArgumentNullException("����д�������");
            if (bytes == null) return;

            int offset = length - startIndex;// ���ǽ�����Ҫд������ݳ���
            if (offset <= 0) return;
            int total = offset + index;
            lock (this)
            {
                int len = _buffer.Length;
                FixSizeAndReset(len, total);
                for (int i = index, j = startIndex; i < total; i++, j++)
                {
                    _buffer[i] = (bytes)[j];
                }

                if(_writeIndex < total) _writeIndex = total;// ���д������ݳ�����д����������̬����д������
            }
        }
        /// <summary>��ָ���Ļ����� <paramref name="index"/> ����  <paramref name="bytes"/> �ֽ����� <paramref name="startIndex"/> ��ʼ�� <paramref name="length"/> �ֽ�д�뵽�˻�����</summary>
        public void Write(int index, ArraySegment<byte> bytes, int startIndex, int length)
        {
            //if (bytes == null) throw new ArgumentNullException("����д�������");
            if (bytes == null) return;

            int offset = length - startIndex;// ���ǽ�����Ҫд������ݳ���
            if (offset <= 0) return;
            int total = offset + index;
            lock (this)
            {
                int len = _buffer.Length;
                FixSizeAndReset(len, total);
                for (int i = index, j = startIndex; i < total; i++, j++)
                {
                    _buffer[i] = (bytes as IList<byte>)[j];
                }

                if(_writeIndex < total) _writeIndex = total;// ���д������ݳ�����д����������̬����д������
            }
        }

        /// <summary>��ָ���Ļ����� <paramref name="index"/> ����  <paramref name="bytes"/> �ֽ����� 0 ��ʼ�� <paramref name="length"/> �ֽ�д�뵽�˻�����</summary>
        public void Write(int index, byte[] bytes, int length)
        {
            Write(index, bytes, 0, length);
        }
        /// <summary>��ָ���Ļ����� <paramref name="index"/> ����  <paramref name="bytes"/> �ֽ����� 0 ��ʼ�� <paramref name="length"/> �ֽ�д�뵽�˻�����</summary>
        public void Write(int index, ArraySegment<byte> bytes, int length)
        {
            Write(index, bytes, 0, length);
        }

        /// <summary>��ָ���Ļ����� <paramref name="index"/> ����  <paramref name="bytes"/> �ֽ����� 0 ��ʼ���ֽ�д�뵽�˻�����</summary>
        public void Write(int index, byte[] bytes)
        {
            Write(index, bytes, 0, bytes.Length);
        }
        /// <summary>��ָ���Ļ����� <paramref name="index"/> ����  <paramref name="bytes"/> �ֽ����� 0 ��ʼ���ֽ�д�뵽�˻�����</summary>
        public void Write(int index, ArraySegment<byte> bytes)
        {
            Write(index, bytes, 0, bytes.Count);
        }

        /// <summary>д�� <paramref name="buffer"/> ����Ч������
        /// <para><paramref name="writeAll"/>��true д�� <paramref name="buffer"/> ����������</para>
        /// </summary>
        public void Write(int index, ByteBuffer buffer, bool writeAll = false)
        {
            if (buffer == null) return;
            //if (!writeAll && buffer.GetReadableBytesLength() <= 0) return;

            if (writeAll)
            {
                Write(index, buffer.ToArraySegment());
            }
            else
            {
                if (buffer.GetReadableBytesLength() <= 0) return;

                Write(index, buffer.ReadSurplusArraySegment());
            }
        }

        /// <summary> ��ָ���Ļ����� <paramref name="index"/> ��д�� <see cref="short"/> </summary>
        public void Write(int index, short v)
        {
            Write(index, filp(BitConverter.GetBytes(v)));
        }
        /// <summary> ��ָ���Ļ����� <paramref name="index"/> ��д�� <see cref="ushort"/> </summary>
        public void Write(int index, ushort v)
        {
            Write(index, filp(BitConverter.GetBytes(v)));
        }

        /// <summary> ��ָ���Ļ����� <paramref name="index"/> ��д�� <see cref="int"/> </summary>
        public void Write(int index, int v)
        {
            Write(index, filp(BitConverter.GetBytes(v)));
        }
        /// <summary> ��ָ���Ļ����� <paramref name="index"/> ��д�� <see cref="uint"/> </summary>
        public void Write(int index, uint v)
        {
            Write(index, filp(BitConverter.GetBytes(v)));
        }

        /// <summary> ��ָ���Ļ����� <paramref name="index"/> ��д�� <see cref="long"/> </summary>
        public void Write(int index, long v)
        {
            Write(index, filp(BitConverter.GetBytes(v)));
        }
        /// <summary> ��ָ���Ļ����� <paramref name="index"/> ��д�� <see cref="ulong"/> </summary>
        public void Write(int index, ulong v)
        {
            Write(index, filp(BitConverter.GetBytes(v)));
        }

        /// <summary> ��ָ���Ļ����� <paramref name="index"/> ��д�� <see cref="float"/> </summary>
        public void Write(int index, float v)
        {
            Write(index, filp(BitConverter.GetBytes(v)));
        }

        /// <summary> ��ָ���Ļ����� <paramref name="index"/> ��д�� <see cref="double"/> </summary>
        public void Write(int index, double v)
        {
            Write(index, filp(BitConverter.GetBytes(v)));
        }

        /// <summary> ��ָ���Ļ����� <paramref name="index"/> ��д�� <see cref="bool"/> </summary>
        public void Write(int index, bool v)
        {
            Write(index, filp(BitConverter.GetBytes(v)));
        }

        /// <summary> ��ָ���Ļ����� <paramref name="index"/> ��д�� <see cref="byte"/> </summary>
        public void Write(int index, byte v)
        {
            lock (this)
            {
                int total = 1 + _writeIndex;
                int len = _buffer.Length;
                FixSizeAndReset(len, total);
                _buffer[_writeIndex] = v;
                _writeIndex = total;
            }
        }

        /// <summary> ��ָ���Ļ����� <paramref name="index"/> ��д�� <see cref="char"/> </summary>
        public void Write(int index, char v)
        {
            Write(index, filp(BitConverter.GetBytes(v)));
        }

        /// <summary> ��ָ���Ļ����� <paramref name="index"/> ��д�� <see cref="string"/> 
        /// <para>ע�⣺��д�� <see cref="string"/> ֮ǰ���Զ�д��һ�������䳤�ȵ� <see cref="int"/></para>
        /// </summary>
        public void Write(int index, string v)
        {
            if (v == null)
            {
                Write(index, -1);
            }
            else
            {
                var bs = Encoding.UTF8.GetBytes(v);
                // ��д��ͷ����ȷ���ַ�������
                Write(index, bs.Length);
                index += bs.Length;
                Write(index, bs);
            }
        }

        #endregion

        /// <summary>�� <paramref name="index"/> ������ʼ��ȡ <paramref name="len"/> ���ȵ� <see cref="byte"/>[]</summary>
        private byte[] Read(int index, int len)
        {
            lock (this)
            {
                // ֻ�ܶ�ȡ�Ѿ�д���
                if (index >= _writeIndex) return default;

                byte[] bs = new byte[len];
                Array.Copy(_buffer, index, bs, 0, len);
                //if (BitConverter.IsLittleEndian)
                //{
                //    Array.Reverse(bs);
                //}

                return bs;
            }
        }

        /// <summary>�� <paramref name="index"/> ������ʼ��ȡ<see cref="byte"/> </summary>
        public byte ReadByte(int index)
        {
            lock (this)
            {
                // ֻ�ܶ�ȡ�Ѿ�д���
                if (index >= _writeIndex) return default;

                byte b = _buffer[index];
                return b;
            }
        }

        /// <summary>�� <paramref name="index"/> ������ʼ��ȡ<see cref="short"/> 
        /// </summary>
        public short ReadShort(int index)
        {
            return BitConverter.ToInt16(filp(Read(index, sizeof(short))), 0);
        }
        /// <summary>�� <paramref name="index"/> ������ʼ��ȡ<see cref="ushort"/> 
        /// </summary>
        public ushort ReadUShort(int index)
        {
            return BitConverter.ToUInt16(filp(Read(index, sizeof(ushort))), 0);
        }

        /// <summary>�� <paramref name="index"/> ������ʼ��ȡ<see cref="int"/> 
        /// </summary>
        public int ReadInt(int index)
        {
            return BitConverter.ToInt32(filp(Read(index, sizeof(int))), 0);
        }
        /// <summary>�� <paramref name="index"/> ������ʼ��ȡ<see cref="uint"/> 
        /// </summary>
        public uint ReadUInt(int index)
        {
            return BitConverter.ToUInt32(filp(Read(index, sizeof(uint))), 0);
        }

        /// <summary>�� <paramref name="index"/> ������ʼ��ȡ<see cref="int"/> 
        /// </summary>
        public long ReadLong(int index)
        {
            return BitConverter.ToInt64(filp(Read(index, sizeof(long))), 0);
        }
        /// <summary>�� <paramref name="index"/> ������ʼ��ȡ<see cref="uint"/> 
        /// </summary>
        public ulong ReadULong(int index)
        {
            return BitConverter.ToUInt64(filp(Read(index, sizeof(ulong))), 0);
        }

        /// <summary>�� <paramref name="index"/> ������ʼ��ȡ<see cref="float"/> 
        /// </summary>
        public float ReadFloat(int index)
        {
            return BitConverter.ToSingle(filp(Read(index, sizeof(float))), 0);
        }

        /// <summary>�� <paramref name="index"/> ������ʼ��ȡ<see cref="double"/> 
        /// </summary>
        public double ReadDouble(int index)
        {
            return BitConverter.ToDouble(filp(Read(index, sizeof(double))), 0);
        }

        /// <summary>�� <paramref name="index"/> ������ʼ��ȡ<see cref="bool"/> 
        /// </summary>
        public bool ReadBool(int index)
        {
            return BitConverter.ToBoolean(filp(Read(index, sizeof(bool))), 0);
        }

        /// <summary>�� <paramref name="index"/> ������ʼ��ȡ<see cref="char"/> </summary>
        public char ReadChar(int index)
        {
            return BitConverter.ToChar(filp(Read(index, sizeof(char))), 0);
        }

        /// <summary>�� <paramref name="index"/> ������ʼ��ȡ <see cref="string"/> 
        /// <para><paramref name="useHeader"/>��true ���Ƚ�����һ�� int ��Ϊ�ַ������ȣ����ݸó��Ƚ������ַ�����false ��ʣ��Ŀɶ��ֽ�ȫ������Ϊ �ַ���</para>
        /// </summary>
        public string ReadString(int index, bool useHeader = true)
        {
            if (!IsReadableIndex(index)) return default;

            int length = 0;
            if (useHeader)
            {
                length = ReadInt(index);
                index += 4;
            }
            else
            {
                length = GetReadableBytesLength();
            }
            return ReadString(index, length);
        }
        /// <summary>�� <paramref name="index"/> ������ʼ��ȡ <see cref="string"/>
        /// </summary>
        public string ReadString(int index, int length)
        {
            if (length <= 0) return default;
            if (!IsReadableIndex(index)) return default;

            var bs = Read(index, length);
            return Encoding.UTF8.GetString(bs);
        }

    }
}