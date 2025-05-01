using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.Core.Network
{
    /// <summary>
    /// 字节缓存，用于构建、解析网络数据包
    /// </summary>
    public partial class ByteBuffer : IDisposable
    {
        // 字节缓存区
        private byte[] _buffer;
        /// <summary>
        /// 读取索引，表示即将要读取的
        /// </summary>
        private int _readIndex;
        /// <summary>
        /// 写入索引，表示即将要写入的
        /// </summary>
        private int _writeIndex;
        // 读取索引标记
        private int _markReadIndex;
        // 写入索引标记
        private int _markWriteIndex;
        // 缓存区大
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

        /// <summary>是否可读索引</summary>
        private bool IsReadableIndex(int index)
        {
            return index < _writeIndex;
        }

        /// <summary>根据长度，确定大于此长度的最近2次方数，如长度为11，则返回16</summary>
        /// <returns>2次方数</returns>
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

        /// <summary>确定内部字节缓存数组的大小</summary>
        public void FixSizeAndReset(int futureLen)
        {
            FixSizeAndReset(_buffer.Length, futureLen);
        }
        /// <summary>确定内部字节缓存数组的大小</summary>
        private int FixSizeAndReset(int currLen, int futureLen)
        {
            if (futureLen > currLen)
            {
                //以原大小的2次方数的两倍确定内部字节缓存区大小
                int size = FixLength(currLen) * 2;
                if (futureLen > size)
                {
                    //以将来的大小的2次方的两倍确定内部字节缓存区大小
                    size = FixLength(futureLen) * 2;
                }
                byte[] newbuf = new byte[size];
                Array.Copy(_buffer, 0, newbuf, 0, currLen);
                _buffer = newbuf;
                _capacity = newbuf.Length;
            }
            return futureLen;
        }

        /// <summary>翻转字节，将低位（小端）字节转换为高位（大端）字节，反之亦然，网络传输统一使用大端</summary>
        /// <returns>翻转后的原数组</returns>
        private byte[] filp(byte[] vs)
        {
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(vs);
            }
            return vs;
        }


        #region 写入
        /// <summary>
        /// 获取写入缓冲区数据段，会改变内部读取索引
        /// <para><paramref name="count"/>：数据段长度</para>
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
        /// 获取读取缓冲区数据段，会改变内部读取索引
        /// <para><paramref name="count"/>：数据段长度</para>
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
        /// 获取缓冲区数据段
        /// <para><paramref name="index"/>：开始索引</para>
        /// <para><paramref name="count"/>：数据段长度</para>
        /// </summary>
        /// <returns></returns>
        public ArraySegment<byte> GetArraySegment(int index, int count)
        {
            var r = new ArraySegment<byte>(_buffer, index, count);
            return r;
        }

        /// <summary>将 bytes 字节数组从 startIndex 开始的 length 字节写入到此缓存区</summary>
        public void Write(byte[] bytes, int startIndex, int length)
        {
            //if (bytes == null || bytes.Length == 0) throw new ArgumentNullException("不可写入空数据");
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
        /// <summary>将 bytes 字节数组从 startIndex 开始的 length 字节写入到此缓存区</summary>
        public void Write(ArraySegment<byte> bytes, int startIndex, int length)
        {
            //if (bytes == null || bytes.Length == 0) throw new ArgumentNullException("不可写入空数据");
            if (bytes == null || bytes.Count == 0) return;

            int offset = length - startIndex;// 这是接下来要写入的数据长度
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

        /// <summary>将 bytes 字节数组从 0 索引开始的 length 字节写入到此缓存区</summary>
        public void Write(byte[] bytes, int length)
        {
            Write(bytes, 0, length);
        }
        /// <summary>将 bytes 字节数组从 0 索引开始的 length 字节写入到此缓存区</summary>
        public void Write(ArraySegment<byte> bytes, int length)
        {
            Write(bytes, 0, length);
        }
        /// <summary>将bytes字节数组写入到此缓存区</summary>
        public void Write(byte[] bytes)
        {
            Write(bytes, 0, bytes.Length);
        }
        /// <summary>将bytes字节数组写入到此缓存区</summary>
        public void Write(ArraySegment<byte> bytes)
        {
            Write(bytes, 0, bytes.Count);
        }

        /// <summary>写入 <paramref name="buffer"/> 的有效缓冲区
        /// <para><paramref name="writeAll"/>：true 写入 <paramref name="buffer"/> 的所有数据</para>
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

        /// <summary> 写入<see cref="short"/> </summary>
        public void Write(short v)
        {
            Write(filp(BitConverter.GetBytes(v)));
        }
        /// <summary> 写入<see cref="ushort"/> </summary>
        public void Write(ushort v)
        {
            Write(filp(BitConverter.GetBytes(v)));
        }

        /// <summary> 写入<see cref="int"/> </summary>
        public void Write(int v)
        {
            Write(filp(BitConverter.GetBytes(v)));
        }
        /// <summary> 写入<see cref="uint"/> </summary>
        public void Write(uint v)
        {
            Write(filp(BitConverter.GetBytes(v)));
        }

        /// <summary> 写入<see cref="long"/> </summary>
        public void Write(long v)
        {
            Write(filp(BitConverter.GetBytes(v)));
        }
        /// <summary> 写入<see cref="ulong"/> </summary>
        public void Write(ulong v)
        {
            Write(filp(BitConverter.GetBytes(v)));
        }

        /// <summary> 写入<see cref="float"/> </summary>
        public void Write(float v)
        {
            Write(filp(BitConverter.GetBytes(v)));
        }

        /// <summary> 写入<see cref="double"/> </summary>
        public void Write(double v)
        {
            Write(filp(BitConverter.GetBytes(v)));
        }

        /// <summary> 写入<see cref="bool"/> </summary>
        public void Write(bool v)
        {
            Write(filp(BitConverter.GetBytes(v)));
        }

        /// <summary> 写入<see cref="byte"/> </summary>
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

        /// <summary> 写入<see cref="char"/> </summary>
        public void Write(char v)
        {
            Write(filp(BitConverter.GetBytes(v)));
        }

        /// <summary> 写入<see cref="string"/> 
        /// <para>注意：在写入 <see cref="string"/> 之前会自动写入一个代表其长度的 <see cref="int"/></para>
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
                // 先写入头部以确定字符串长度
                Write(bs.Length);
                Write(bs);
            }
        } 
        #endregion


        #region 读取
        /// <summary> 读取 <see cref="byte[]"/> 
        /// <paramref name="isOffset"/>：是否偏移读取索引
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
        /// <summary> 读取 <see cref="byte[]"/> 
        /// <paramref name="isOffset"/>：是否偏移读取索引
        /// </summary>
        [Obsolete("由于会翻转原始数据，此方法不能使用")]
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
        /// <summary> 读取 <see cref="byte[]"/> 
        /// <paramref name="isOffset"/>：是否偏移读取索引
        /// </summary>
        [Obsolete("由于会翻转原始数据，此方法不能使用")]
        public ReadOnlySpan<byte> ReadToReadOnlySpan(int len, bool isOffset = true)
        {
            return ReadToSpan(len, isOffset);
        }

        /// <summary> 从读取索引位置开始读取数据到<paramref name="distBytes"/>中，并将其填满 </summary>
        public void ReadBytes(byte[] distBytes)
        {
            ReadBytes(distBytes, 0, distBytes.Length);
        }
        /// <summary> 从读取索引位置开始读取数据到<paramref name="distBytes"/>中，并将其填满 </summary>
        public void ReadBytes(ArraySegment<byte> distBytes)
        {
            ReadBytes(distBytes, 0, distBytes.Count);
        }
        /// <summary> 从读取索引位置开始读取<paramref name="len"/>长度的数据到<paramref name="distBytes"/>中 </summary>
        public void ReadBytes(byte[] distBytes, int distIndex, int len)
        {
            int size = distIndex + len;
            if (GetReadableBytesLength() < size) new Exception("ByteBuffer：缓冲区有效数据不足");
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
        /// <summary> 从读取索引位置开始读取 <paramref name="len"/> 长度的数据到 <paramref name="distBytes"/> 中 </summary>
        public void ReadBytes(ArraySegment<byte> distBytes, int distIndex, int len)
        {
            int size = distIndex + len;
            if (GetReadableBytesLength() < size) new Exception("ByteBuffer：缓冲区有效数据不足");
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

        /// <summary> 读取<see cref="byte"/> </summary>
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

        /// <summary> 读取<see cref="short"/> 
        /// <paramref name="isOffset"/>：是否偏移读取索引
        /// </summary>
        public short ReadShort(bool isOffset = true)
        {
            return BitConverter.ToInt16(filp(Read(2, isOffset)));
        }
        /// <summary> 读取<see cref="ushort"/> 
        /// <paramref name="isOffset"/>：是否偏移读取索引
        /// </summary>
        public ushort ReadUShort(bool isOffset = true)
        {
            return BitConverter.ToUInt16(filp(Read(2, isOffset)));
        }

        /// <summary> 读取<see cref="int"/> 
        /// <paramref name="isOffset"/>：是否偏移读取索引
        /// </summary>
        public int ReadInt(bool isOffset = true)
        {
            return BitConverter.ToInt32(filp(Read(4, isOffset)));
        }
        /// <summary> 读取<see cref="uint"/> 
        /// <paramref name="isOffset"/>：是否偏移读取索引
        /// </summary>
        public uint ReadUInt(bool isOffset = true)
        {
            return BitConverter.ToUInt32(filp(Read(4, isOffset)));
        }

        /// <summary> 读取<see cref="int"/> 
        /// <paramref name="isOffset"/>：是否偏移读取索引
        /// </summary>
        public long ReadLong(bool isOffset = true)
        {
            return BitConverter.ToInt64(filp(Read(8, isOffset)));
        }
        /// <summary> 读取<see cref="uint"/> 
        /// <paramref name="isOffset"/>：是否偏移读取索引
        /// </summary>
        public ulong ReadULong(bool isOffset = true)
        {
            return BitConverter.ToUInt64(filp(Read(8, isOffset)));
        }

        /// <summary> 读取<see cref="float"/> 
        /// <paramref name="isOffset"/>：是否偏移读取索引
        /// </summary>
        public float ReadFloat(bool isOffset = true)
        {
            return BitConverter.ToSingle(filp(Read(4, isOffset)));
        }

        /// <summary> 读取<see cref="double"/> 
        /// <paramref name="isOffset"/>：是否偏移读取索引
        /// </summary>
        public double ReadDouble(bool isOffset = true)
        {
            return BitConverter.ToDouble(filp(Read(8, isOffset)));
        }

        /// <summary> 读取<see cref="bool"/> 
        /// <paramref name="isOffset"/>：是否偏移读取索引
        /// </summary>
        public bool ReadBool(bool isOffset = true)
        {
            return BitConverter.ToBoolean(filp(Read(2, isOffset)));
        }

        /// <summary> 读取<see cref="char"/> </summary>
        public char ReadChar(bool isOffset = true)
        {
            return BitConverter.ToChar(filp(Read(2, isOffset)));
        }

        /// <summary> 读取<see cref="string"/> 
        /// <para><paramref name="useHeader"/>：true 首先解析出一个 int 作为字符串长度，根据该长度解析出字符串，false 将剩余的可读字节全部解析为 字符串</para>
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
        /// <summary> 读取<see cref="string"/> </para>
        /// </summary>
        public string ReadString(int length)
        {
            if (length <= 0) return null;
            //var bs = Read(len);
            var bs = Read(length);
            return Encoding.UTF8.GetString(bs);
        } 
        #endregion

        /// <summary>获取写入的字节数组</summary>
        public byte[] ToArray()
        {
            byte[] bytes = new byte[_writeIndex];
            Array.Copy(_buffer, 0, bytes, 0, bytes.Length);
            return bytes;
        }
        /// <summary>获取写入的字节数组段</summary>
        public ArraySegment<byte> ToArraySegment()
        {
            var vs = new ArraySegment<byte>(_buffer, 0, _writeIndex);
            return vs;
        }

        /// <summary>获取剩余字节数组</summary>
        public byte[] ToSurplusArray()
        {
            byte[] bytes = new byte[GetReadableBytesLength()];
            Array.Copy(_buffer, _readIndex, bytes, 0, bytes.Length);
            return bytes;
        }
        /// <summary>获取剩余写入的字节数组段</summary>
        public ArraySegment<byte> ToSurplusArraySegment()
        {
            var vs = new ArraySegment<byte>(_buffer, _readIndex, GetReadableBytesLength());
            return vs;
        }

        /// <summary>读取剩余写入的字节数组，会改变索引</summary>
        public byte[] ReadSurplusArray()
        {
            byte[] bytes = new byte[GetReadableBytesLength()];
            Array.Copy(_buffer, _readIndex, bytes, 0, bytes.Length);
            _readIndex = _writeIndex;
            return bytes;
        }
        /// <summary>读取剩余写入的字节数组段，会改变索引</summary>
        public ArraySegment<byte> ReadSurplusArraySegment()
        {
            var vs = new ArraySegment<byte>(_buffer, _readIndex, GetReadableBytesLength());
            _readIndex = _writeIndex;
            return vs;
        }


        /// <summary>剩余可读的有效字节数</summary>
        public int GetReadableBytesLength()
        {
            return _writeIndex - _readIndex;
        }

        /// <summary>获取缓存区容量大小</summary>
        public int GetCapacity()
        {
            return _capacity;
        }

        /// <summary>设置开始读取的索引</summary>
        public void SetReadIndex(int index)
        {
            if (index < 0) return;
            _readIndex = index;
        }
        /// <summary>设置开始写入的索引</summary>
        public void SetWriteIndex(int index)
        {
            if (index < 0) return;
            _writeIndex = index;
        }

        /// <summary>标记读取的索引位置</summary>
        public void MarkReadIndex()
        {
            _markReadIndex = _readIndex;
        }

        /// <summary>标记写入的索引位置</summary>
        public void MarkWriteIndex()
        {
            _markWriteIndex = _writeIndex;
        }

        /// <summary>将读取的索引位置重置为标记的位置</summary>
        public void ResetReadIndex()
        {
            _readIndex = _markReadIndex;
        }

        /// <summary>将写入的索引位置重置为标记的位置</summary>
        public void ResetWriteIndex()
        {
            _writeIndex = _markWriteIndex;
        }

        /// <summary>剩余可写的有效字节数</summary>
        public int GetWriteableBytesLength()
        {
            return _capacity - _writeIndex;
        }
        
        /// <summary>清除已读字节并重建缓存区</summary>
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

        /// <summary>清除</summary>
        public void Clear()
        {
            // 没有写任何数据
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
                    // 释放托管状态(托管对象)
                    _buffer = null;
                }

                // 释放未托管的资源(未托管的对象)并重写终结器
                // 将大型字段设置为 null
                disposedValue = true;
            }
        }

        // // 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~ByteBuffer()
        // {
        //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}