using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Framework.Core.Network
{
    /// <summary>
    /// �ֽڻ��棬���ڹ����������������ݰ�
    /// </summary>
    public class ByteBuffer
    {
        // �ֽڻ�����
        private byte[] _buffer;
        // ��ȡ����
        private int _readIndex;
        // д������
        private int _writeIndex;
        // ��ȡ�������
        private int _markReadIndex;
        // д���������
        private int _markWriteIndex;
        // ��������
        private int _capacity;

        public ByteBuffer(int capacity)
        {
            _capacity = capacity;
            _buffer = new byte[capacity];
        }

        public ByteBuffer(byte[] buffer)
        {
            _buffer = buffer;
            _capacity = _buffer.Length;
        }

        /// <summary>���ݳ��ȣ�ȷ�����ڴ˳��ȵ����2�η������糤��Ϊ11���򷵻�16</summary>
        /// <returns>2�η���</returns>
        private int FixLength(int length)
        {
            int n = 0;
            int b = 2;
            while (b < length)
            {
                b = 2 << n;
                n++;
            }
            return b;
        }

        /// <summary>ȷ���ڲ��ֽڻ�������Ĵ�С</summary>
        /// <returns></returns>
        private int FixSizeAndReset(int currLen, int futureLen)
        {
            if (futureLen > currLen)
            {
                //��ԭ��С��2�η���������ȷ���ڲ��ֽڻ�������С
                int size = FixLength(currLen) * 2;
                if (futureLen > size)
                {
                    //�Խ����Ĵ�С��2�η�������ȷ���ڲ��ֽڻ�������С
                    size = FixLength(futureLen) * 2;
                }
                byte[] newbuf = new byte[size];
                Array.Copy(_buffer, 0, newbuf, 0, currLen);
                _buffer = newbuf;
                _capacity = newbuf.Length;
            }
            return futureLen;
        }

        /// <summary>��ת�ֽڣ�����λ��С�ˣ��ֽ�ת��Ϊ��λ����ˣ��ֽڣ���֮��Ȼ�����紫��ͳһʹ�ô��</summary>
        /// <returns>��ת���ԭ����</returns>
        private byte[] filp(byte[] vs)
        {
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(vs);
            }
            return vs;
        }

        /// <summary>��bytes�ֽ������startIndex��ʼ��length�ֽ�д�뵽�˻�����</summary>
        public void Write(byte[] bytes, int startIndex, int length)
        {
            lock (this)
            {
                int offset = length - startIndex;
                if (offset <= 0) return;
                int total = offset + _writeIndex;
                int len = _buffer.Length;
                FixSizeAndReset(len, total);
                for (int i = _writeIndex, j = startIndex; i < total; i++, j++)
                {
                    _buffer[i] = bytes[j];
                }
                _writeIndex = total;
            }
        }

        /// <summary>��bytes�ֽ������0������ʼ��length�ֽ�д�뵽�˻�����</summary>
        public void Write(byte[] bytes, int length)
        {
            Write(bytes, 0, length);
        }
        /// <summary>��bytes�ֽ�����д�뵽�˻�����</summary>
        public void Write(byte[] bytes)
        {
            Write(bytes, 0, bytes.Length);
        }

        /// <summary>д��<see cref="ByteBuffer"/>����Ч������</summary>
        public void Write(ByteBuffer buffer)
        {
            if (buffer == null) return;
            if (buffer.ReadableBytes() <= 0) return;
            Write(buffer.ToArray());
        }

        /// <summary> д��<see cref="short"/> </summary>
        public void Write(short v)
        {
            Write(filp(BitConverter.GetBytes(v)));
        }
        /// <summary> д��<see cref="ushort"/> </summary>
        public void Write(ushort v)
        {
            Write(filp(BitConverter.GetBytes(v)));
        }

        /// <summary> д��<see cref="int"/> </summary>
        public void Write(int v)
        {
            Write(filp(BitConverter.GetBytes(v)));
        }
        /// <summary> д��<see cref="uint"/> </summary>
        public void Write(uint v)
        {
            Write(filp(BitConverter.GetBytes(v)));
        }

        /// <summary> д��<see cref="long"/> </summary>
        public void Write(long v)
        {
            Write(filp(BitConverter.GetBytes(v)));
        }
        /// <summary> д��<see cref="ulong"/> </summary>
        public void Write(ulong v)
        {
            Write(filp(BitConverter.GetBytes(v)));
        }

        /// <summary> д��<see cref="float"/> </summary>
        public void Write(float v)
        {
            Write(filp(BitConverter.GetBytes(v)));
        }

        /// <summary> д��<see cref="double"/> </summary>
        public void Write(double v)
        {
            Write(filp(BitConverter.GetBytes(v)));
        }

        /// <summary> д��<see cref="bool"/> </summary>
        public void Write(bool v)
        {
            Write(filp(BitConverter.GetBytes(v)));
        }

        /// <summary> д��<see cref="byte"/> </summary>
        public void Write(byte v)
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

        /// <summary> д��<see cref="char"/> </summary>
        public void Write(char v)
        {
            Write(filp(BitConverter.GetBytes(v)));
        }

        /// <summary> д��<see cref="string"/> </summary>
        public void Write(string v)
        {
            if (v == null)
            {
                Write(-1);
            }
            else
            {
                var bs = Encoding.UTF8.GetBytes(v);
                // ��д��ͷ����ȷ���ַ�������
                Write(bs.Length);
                Write(bs);
            }
        }


        /// <summary> ��ȡ<see cref="byte[]"/> </summary>
        private byte[] Read(int len)
        {
            lock (this)
            {
                byte[] bs = new byte[len];
                Array.Copy(_buffer, _readIndex, bs, 0, len);
                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(bs);
                }

                _readIndex += len;
                return bs;
            }
        }

        /// <summary> �Ӷ�ȡ����λ�ÿ�ʼ��ȡ���ݵ�<paramref name="distBytes"/>�У����������� </summary>
        public void ReadBytes(byte[] distBytes)
        {
            int size = distBytes.Length;

            ReadBytes(distBytes, 0, size);
        }
        /// <summary> �Ӷ�ȡ����λ�ÿ�ʼ��ȡ<paramref name="len"/>���ȵ����ݵ�<paramref name="distBytes"/>�� </summary>
        public void ReadBytes(byte[] distBytes, int distIndex, int len)
        {
            int size = distIndex + len;
            //if (ReadableBytes() < size) new Exception("BitBuffer����������Ч���ݲ���");
            for (int i = distIndex; i < size; i++)
            {
                distBytes[i] = ReadByte();
            }
        }

        /// <summary> ��ȡ<see cref="byte"/> </summary>
        public byte ReadByte()
        {
            lock (this)
            {
                if (_readIndex >= _buffer.Length) return byte.MinValue;

                byte b = _buffer[_readIndex];
                _readIndex += 1;
                return b;
            }
        }

        /// <summary> ��ȡ<see cref="short"/> </summary>
        public short ReadShort()
        {
            return BitConverter.ToInt16(Read(2), 0);
        }
        /// <summary> ��ȡ<see cref="ushort"/> </summary>
        public ushort ReadUShort()
        {
            return BitConverter.ToUInt16(Read(2), 0);
        }

        /// <summary> ��ȡ<see cref="int"/> </summary>
        public int ReadInt()
        {
            return BitConverter.ToInt32(Read(4), 0);
        }
        /// <summary> ��ȡ<see cref="uint"/> </summary>
        public uint ReadUInt()
        {
            return BitConverter.ToUInt32(Read(4), 0);
        }

        /// <summary> ��ȡ<see cref="int"/> </summary>
        public long ReadLong()
        {
            return BitConverter.ToInt64(Read(8), 0);
        }
        /// <summary> ��ȡ<see cref="uint"/> </summary>
        public ulong ReadULong()
        {
            return BitConverter.ToUInt64(Read(8), 0);
        }

        /// <summary> ��ȡ<see cref="float"/> </summary>
        public float ReadFloat()
        {
            return BitConverter.ToSingle(Read(4), 0);
        }

        /// <summary> ��ȡ<see cref="double"/> </summary>
        public double ReadDouble()
        {
            return BitConverter.ToDouble(Read(8), 0);
        }

        /// <summary> ��ȡ<see cref="bool"/> </summary>
        public bool ReadBool()
        {
            return BitConverter.ToBoolean(Read(2), 0);
        }

        /// <summary> ��ȡ<see cref="char"/> </summary>
        public char ReadChar()
        {
            return BitConverter.ToChar(Read(2), 0);
        }

        /// <summary> ��ȡ<see cref="string"/> </summary>
        public string ReadString()
        {
            int len = ReadInt();
            if (len < 0) return null;
            var bs = Read(len);
            return Encoding.UTF8.GetString(bs);
        }

        /// <summary>����Ѷ��ֽڲ��ؽ�������</summary>
        public void DiscardReadBytes()
        {
            lock (this)
            {
                if (_readIndex <= 0) return;
                int len = _buffer.Length - _readIndex;
                byte[] newbuf = new byte[len];
                Array.Copy(_buffer, _readIndex, newbuf, 0, len);
                _buffer = newbuf;
                _writeIndex -= _readIndex;
                _markReadIndex -= _readIndex;
                if (_markReadIndex < 0)
                {
                    _markReadIndex = _readIndex;
                }
                _markWriteIndex -= _readIndex;
                if (_markWriteIndex < 0 || _markWriteIndex < _readIndex || _markWriteIndex < _markReadIndex)
                {
                    _markWriteIndex = _writeIndex;
                }
                _readIndex = 0;
            }
        }

        /// <summary>���</summary>
        public void Clear()
        {
            _buffer = new byte[_buffer.Length];
            _readIndex = 0;
            _markReadIndex = 0;
            _writeIndex = 0;
            _markWriteIndex = 0;
        }

        /// <summary>���ÿ�ʼ��ȡ������</summary>
        public void SetReadIndex(int index)
        {
            if (index < 0) return;
            _readIndex = index;
        }

        /// <summary>��Ƕ�ȡ������λ��</summary>
        public void MarkReadIndex()
        {
            _markReadIndex = _readIndex;
        }

        /// <summary>���д�������λ��</summary>
        public void MarkWriteIndex()
        {
            _markWriteIndex = _writeIndex;
        }

        /// <summary>����ȡ������λ������Ϊ��ǵ�λ��</summary>
        public void ResetReadIndex()
        {
            _readIndex = _markReadIndex;
        }

        /// <summary>��д�������λ������Ϊ��ǵ�λ��</summary>
        public void ResetWriteIndex()
        {
            _writeIndex = _markWriteIndex;
        }

        /// <summary>ʣ��ɶ�����Ч�ֽ���</summary>
        public int ReadableBytes()
        {
            return _writeIndex - _readIndex;
        }

        /// <summary>��ȡ�ɶ����ֽ�����</summary>
        public byte[] ToArray()
        {
            byte[] bytes = new byte[_writeIndex];
            Array.Copy(_buffer, 0, bytes, 0, bytes.Length);
            return bytes;
        }

        /// <summary>��ȡ��������С</summary>
        public int GetCapacity()
        {
            return _capacity;
        }

    }
}