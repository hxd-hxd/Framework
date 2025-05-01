using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.Core.Network
{
    /// <summary>
    /// �ֽڻ��棬���ڹ����������������ݰ�
    /// </summary>
    public partial class ByteBuffer : IDisposable
    {
        // �ֽڻ�����
        private byte[] _buffer;
        /// <summary>
        /// ��ȡ��������ʾ����Ҫ��ȡ��
        /// </summary>
        private int _readIndex;
        /// <summary>
        /// д����������ʾ����Ҫд���
        /// </summary>
        private int _writeIndex;
        // ��ȡ�������
        private int _markReadIndex;
        // д���������
        private int _markWriteIndex;
        // ��������
        private int _capacity;

        private bool disposedValue;

        //public int writeIndex
        //{
        //    get => _writeIndex;
        //    protected set
        //    {
        //        _writeIndex = value < 0 ? 0 : value;
        //    }
        //}

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

        /// <summary>�Ƿ�ɶ�����</summary>
        private bool IsReadableIndex(int index)
        {
            return index < _writeIndex;
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
        public void FixSizeAndReset(int futureLen)
        {
            FixSizeAndReset(_buffer.Length, futureLen);
        }
        /// <summary>ȷ���ڲ��ֽڻ�������Ĵ�С</summary>
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


        #region д��
        /// <summary>
        /// ��ȡд�뻺�������ݶΣ���ı��ڲ���ȡ����
        /// <para><paramref name="count"/>�����ݶγ���</para>
        /// </summary>
        /// <returns></returns>
        public ArraySegment<byte> GetWriteArraySegment(int count)
        {
            FixSizeAndReset(_writeIndex + count);
            int offset = _writeIndex;
            var r = new ArraySegment<byte>(_buffer, offset, count);
            _writeIndex += count;
            return r;
        }

        /// <summary>
        /// ��ȡ��ȡ���������ݶΣ���ı��ڲ���ȡ����
        /// <para><paramref name="count"/>�����ݶγ���</para>
        /// </summary>
        /// <returns></returns>
        public ArraySegment<byte> GetReadArraySegment(int count)
        {
            int offset = _readIndex;
            var r = new ArraySegment<byte>(_buffer, offset, count);
            _readIndex += count;
            return r;
        }

        /// <summary>
        /// ��ȡ���������ݶ�
        /// <para><paramref name="index"/>����ʼ����</para>
        /// <para><paramref name="count"/>�����ݶγ���</para>
        /// </summary>
        /// <returns></returns>
        public ArraySegment<byte> GetArraySegment(int index, int count)
        {
            var r = new ArraySegment<byte>(_buffer, index, count);
            return r;
        }

        /// <summary>�� bytes �ֽ������ startIndex ��ʼ�� length �ֽ�д�뵽�˻�����</summary>
        public void Write(byte[] bytes, int startIndex, int length)
        {
            //if (bytes == null || bytes.Length == 0) throw new ArgumentNullException("����д�������");
            if (bytes == null || bytes.Length == 0) return;

            int offset = length - startIndex;
            if (offset <= 0) return;
            lock (this)
            {
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
        /// <summary>�� bytes �ֽ������ startIndex ��ʼ�� length �ֽ�д�뵽�˻�����</summary>
        public void Write(ArraySegment<byte> bytes, int startIndex, int length)
        {
            //if (bytes == null || bytes.Length == 0) throw new ArgumentNullException("����д�������");
            if (bytes == null || bytes.Count == 0) return;

            int offset = length - startIndex;// ���ǽ�����Ҫд������ݳ���
            if (offset <= 0) return;
            lock (this)
            {
                int total = offset + _writeIndex;
                int len = _buffer.Length;
                FixSizeAndReset(len, total);
                for (int i = _writeIndex, j = startIndex; i < total; i++, j++)
                {
                    _buffer[i] = (bytes as IList<byte>)[j];
                }
                _writeIndex = total;
            }
        }

        /// <summary>�� bytes �ֽ������ 0 ������ʼ�� length �ֽ�д�뵽�˻�����</summary>
        public void Write(byte[] bytes, int length)
        {
            Write(bytes, 0, length);
        }
        /// <summary>�� bytes �ֽ������ 0 ������ʼ�� length �ֽ�д�뵽�˻�����</summary>
        public void Write(ArraySegment<byte> bytes, int length)
        {
            Write(bytes, 0, length);
        }
        /// <summary>��bytes�ֽ�����д�뵽�˻�����</summary>
        public void Write(byte[] bytes)
        {
            Write(bytes, 0, bytes.Length);
        }
        /// <summary>��bytes�ֽ�����д�뵽�˻�����</summary>
        public void Write(ArraySegment<byte> bytes)
        {
            Write(bytes, 0, bytes.Count);
        }

        /// <summary>д�� <paramref name="buffer"/> ����Ч������
        /// <para><paramref name="writeAll"/>��true д�� <paramref name="buffer"/> ����������</para>
        /// </summary>
        public void Write(ByteBuffer buffer, bool writeAll = false)
        {
            if (buffer == null) return;
            //if (!writeAll && buffer.GetReadableBytesLength() <= 0) return;

            if (writeAll)
            {
                Write(buffer.ToArraySegment());
            }
            else
            {
                if (buffer.GetReadableBytesLength() <= 0) return;

                Write(buffer.ReadSurplusArraySegment());
            }
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

        /// <summary> д��<see cref="string"/> 
        /// <para>ע�⣺��д�� <see cref="string"/> ֮ǰ���Զ�д��һ�������䳤�ȵ� <see cref="int"/></para>
        /// </summary>
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
        #endregion


        #region ��ȡ
        /// <summary> ��ȡ <see cref="byte[]"/> 
        /// <paramref name="isOffset"/>���Ƿ�ƫ�ƶ�ȡ����
        /// </summary>
        private byte[] Read(int len, bool isOffset = true)
        {
            lock (this)
            {
                //byte[] bs = new byte[len];
                //Array.Copy(_buffer, _readIndex, bs, 0, len);
                ////if (BitConverter.IsLittleEndian)
                ////{
                ////    Array.Reverse(bs);
                ////}

                var bs = Read(_readIndex, len);
                if (isOffset)
                    _readIndex += len;
                return bs;
            }
        }
        /// <summary> ��ȡ <see cref="byte[]"/> 
        /// <paramref name="isOffset"/>���Ƿ�ƫ�ƶ�ȡ����
        /// </summary>
        [Obsolete("���ڻᷭתԭʼ���ݣ��˷�������ʹ��")]
        public Span<byte> ReadToSpan(int len, bool isOffset = true)
        {
            lock (this)
            {
                var bs = new Span<byte>(_buffer, _readIndex, len);
                if (BitConverter.IsLittleEndian)
                {
                    bs.Reverse();
                }

                if (isOffset)
                    _readIndex += len;
                return bs;
            }
        }
        /// <summary> ��ȡ <see cref="byte[]"/> 
        /// <paramref name="isOffset"/>���Ƿ�ƫ�ƶ�ȡ����
        /// </summary>
        [Obsolete("���ڻᷭתԭʼ���ݣ��˷�������ʹ��")]
        public ReadOnlySpan<byte> ReadToReadOnlySpan(int len, bool isOffset = true)
        {
            return ReadToSpan(len, isOffset);
        }

        /// <summary> �Ӷ�ȡ����λ�ÿ�ʼ��ȡ���ݵ�<paramref name="distBytes"/>�У����������� </summary>
        public void ReadBytes(byte[] distBytes)
        {
            ReadBytes(distBytes, 0, distBytes.Length);
        }
        /// <summary> �Ӷ�ȡ����λ�ÿ�ʼ��ȡ���ݵ�<paramref name="distBytes"/>�У����������� </summary>
        public void ReadBytes(ArraySegment<byte> distBytes)
        {
            ReadBytes(distBytes, 0, distBytes.Count);
        }
        /// <summary> �Ӷ�ȡ����λ�ÿ�ʼ��ȡ<paramref name="len"/>���ȵ����ݵ�<paramref name="distBytes"/>�� </summary>
        public void ReadBytes(byte[] distBytes, int distIndex, int len)
        {
            int size = distIndex + len;
            if (GetReadableBytesLength() < size) new Exception("ByteBuffer����������Ч���ݲ���");
            //for (int i = distIndex; i < size; i++)
            //{
            //    distBytes[i] = ReadByte();
            //}
            lock (this)
            {
                for (int i = distIndex; i < size; i++)
                {
                    byte b = _buffer[_readIndex];
                    distBytes[i] = b;
                    _readIndex += 1;
                }
            }
        }
        /// <summary> �Ӷ�ȡ����λ�ÿ�ʼ��ȡ <paramref name="len"/> ���ȵ����ݵ� <paramref name="distBytes"/> �� </summary>
        public void ReadBytes(ArraySegment<byte> distBytes, int distIndex, int len)
        {
            int size = distIndex + len;
            if (GetReadableBytesLength() < size) new Exception("ByteBuffer����������Ч���ݲ���");
            //for (int i = distIndex; i < size; i++)
            //{
            //    distBytes[i] = ReadByte();
            //}
            lock (this)
            {
                for (int i = distIndex; i < size; i++)
                {
                    byte b = _buffer[_readIndex];
                    //distBytes[i] = b;
                    (distBytes as IList<byte>)[i] = b;
                    _readIndex += 1;
                }
            }
        }

        /// <summary> ��ȡ<see cref="byte"/> </summary>
        public byte ReadByte()
        {
            lock (this)
            {
                //if (_readIndex >= _buffer.Length) return byte.MinValue;
                if (GetReadableBytesLength() <= 0) return default;

                byte b = _buffer[_readIndex];
                _readIndex += 1;
                return b;
            }
        }

        /// <summary> ��ȡ<see cref="short"/> 
        /// <paramref name="isOffset"/>���Ƿ�ƫ�ƶ�ȡ����
        /// </summary>
        public short ReadShort(bool isOffset = true)
        {
            return BitConverter.ToInt16(filp(Read(2, isOffset)));
        }
        /// <summary> ��ȡ<see cref="ushort"/> 
        /// <paramref name="isOffset"/>���Ƿ�ƫ�ƶ�ȡ����
        /// </summary>
        public ushort ReadUShort(bool isOffset = true)
        {
            return BitConverter.ToUInt16(filp(Read(2, isOffset)));
        }

        /// <summary> ��ȡ<see cref="int"/> 
        /// <paramref name="isOffset"/>���Ƿ�ƫ�ƶ�ȡ����
        /// </summary>
        public int ReadInt(bool isOffset = true)
        {
            return BitConverter.ToInt32(filp(Read(4, isOffset)));
        }
        /// <summary> ��ȡ<see cref="uint"/> 
        /// <paramref name="isOffset"/>���Ƿ�ƫ�ƶ�ȡ����
        /// </summary>
        public uint ReadUInt(bool isOffset = true)
        {
            return BitConverter.ToUInt32(filp(Read(4, isOffset)));
        }

        /// <summary> ��ȡ<see cref="int"/> 
        /// <paramref name="isOffset"/>���Ƿ�ƫ�ƶ�ȡ����
        /// </summary>
        public long ReadLong(bool isOffset = true)
        {
            return BitConverter.ToInt64(filp(Read(8, isOffset)));
        }
        /// <summary> ��ȡ<see cref="uint"/> 
        /// <paramref name="isOffset"/>���Ƿ�ƫ�ƶ�ȡ����
        /// </summary>
        public ulong ReadULong(bool isOffset = true)
        {
            return BitConverter.ToUInt64(filp(Read(8, isOffset)));
        }

        /// <summary> ��ȡ<see cref="float"/> 
        /// <paramref name="isOffset"/>���Ƿ�ƫ�ƶ�ȡ����
        /// </summary>
        public float ReadFloat(bool isOffset = true)
        {
            return BitConverter.ToSingle(filp(Read(4, isOffset)));
        }

        /// <summary> ��ȡ<see cref="double"/> 
        /// <paramref name="isOffset"/>���Ƿ�ƫ�ƶ�ȡ����
        /// </summary>
        public double ReadDouble(bool isOffset = true)
        {
            return BitConverter.ToDouble(filp(Read(8, isOffset)));
        }

        /// <summary> ��ȡ<see cref="bool"/> 
        /// <paramref name="isOffset"/>���Ƿ�ƫ�ƶ�ȡ����
        /// </summary>
        public bool ReadBool(bool isOffset = true)
        {
            return BitConverter.ToBoolean(filp(Read(2, isOffset)));
        }

        /// <summary> ��ȡ<see cref="char"/> </summary>
        public char ReadChar(bool isOffset = true)
        {
            return BitConverter.ToChar(filp(Read(2, isOffset)));
        }

        /// <summary> ��ȡ<see cref="string"/> 
        /// <para><paramref name="useHeader"/>��true ���Ƚ�����һ�� int ��Ϊ�ַ������ȣ����ݸó��Ƚ������ַ�����false ��ʣ��Ŀɶ��ֽ�ȫ������Ϊ �ַ���</para>
        /// </summary>
        public string ReadString(bool useHeader = true)
        {
            int length = 0;
            if (useHeader)
            {
                length = ReadInt();
            }
            else
            {
                length = GetReadableBytesLength();
            }
            return ReadString(length);
        }
        /// <summary> ��ȡ<see cref="string"/> </para>
        /// </summary>
        public string ReadString(int length)
        {
            if (length <= 0) return null;
            //var bs = Read(len);
            var bs = Read(length);
            return Encoding.UTF8.GetString(bs);
        } 
        #endregion

        /// <summary>��ȡд����ֽ�����</summary>
        public byte[] ToArray()
        {
            byte[] bytes = new byte[_writeIndex];
            Array.Copy(_buffer, 0, bytes, 0, bytes.Length);
            return bytes;
        }
        /// <summary>��ȡд����ֽ������</summary>
        public ArraySegment<byte> ToArraySegment()
        {
            var vs = new ArraySegment<byte>(_buffer, 0, _writeIndex);
            return vs;
        }

        /// <summary>��ȡʣ���ֽ�����</summary>
        public byte[] ToSurplusArray()
        {
            byte[] bytes = new byte[GetReadableBytesLength()];
            Array.Copy(_buffer, _readIndex, bytes, 0, bytes.Length);
            return bytes;
        }
        /// <summary>��ȡʣ��д����ֽ������</summary>
        public ArraySegment<byte> ToSurplusArraySegment()
        {
            var vs = new ArraySegment<byte>(_buffer, _readIndex, GetReadableBytesLength());
            return vs;
        }

        /// <summary>��ȡʣ��д����ֽ����飬��ı�����</summary>
        public byte[] ReadSurplusArray()
        {
            byte[] bytes = new byte[GetReadableBytesLength()];
            Array.Copy(_buffer, _readIndex, bytes, 0, bytes.Length);
            _readIndex = _writeIndex;
            return bytes;
        }
        /// <summary>��ȡʣ��д����ֽ�����Σ���ı�����</summary>
        public ArraySegment<byte> ReadSurplusArraySegment()
        {
            var vs = new ArraySegment<byte>(_buffer, _readIndex, GetReadableBytesLength());
            _readIndex = _writeIndex;
            return vs;
        }


        /// <summary>ʣ��ɶ�����Ч�ֽ���</summary>
        public int GetReadableBytesLength()
        {
            return _writeIndex - _readIndex;
        }

        /// <summary>��ȡ������������С</summary>
        public int GetCapacity()
        {
            return _capacity;
        }

        /// <summary>���ÿ�ʼ��ȡ������</summary>
        public void SetReadIndex(int index)
        {
            if (index < 0) return;
            _readIndex = index;
        }
        /// <summary>���ÿ�ʼд�������</summary>
        public void SetWriteIndex(int index)
        {
            if (index < 0) return;
            _writeIndex = index;
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

        /// <summary>ʣ���д����Ч�ֽ���</summary>
        public int GetWriteableBytesLength()
        {
            return _capacity - _writeIndex;
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
            // û��д�κ�����
            if (_writeIndex <= 0) return;

            //_buffer = new byte[_buffer.Length];
            for (int i = 0; i < _writeIndex; i++)
            {
                _buffer[i] = byte.MinValue;
            }
            _readIndex = 0;
            _markReadIndex = 0;
            _writeIndex = 0;
            _markWriteIndex = 0;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // �ͷ��й�״̬(�йܶ���)
                    _buffer = null;
                }

                // �ͷ�δ�йܵ���Դ(δ�йܵĶ���)����д�ս���
                // �������ֶ�����Ϊ null
                disposedValue = true;
            }
        }

        // // ������Dispose(bool disposing)��ӵ�������ͷ�δ�й���Դ�Ĵ���ʱ������ս���
        // ~ByteBuffer()
        // {
        //     // ��Ҫ���Ĵ˴��롣�뽫���������롰Dispose(bool disposing)��������
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // ��Ҫ���Ĵ˴��롣�뽫���������롰Dispose(bool disposing)��������
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}