using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Framework.Core.Network
{
    /// <summary>
    /// 字节缓存，用于构建、解析网络数据包
    /// </summary>
    public class ByteBuffer
    {
        // 字节缓存区
        private byte[] _buffer;
        // 读取索引
        private int _readIndex;
        // 写入索引
        private int _writeIndex;
        // 读取索引标记
        private int _markReadIndex;
        // 写入索引标记
        private int _markWriteIndex;
        // 缓存区大
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
        /// <returns></returns>
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

        /// <summary>将bytes字节数组从startIndex开始的length字节写入到此缓存区</summary>
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

        /// <summary>将bytes字节数组从0索引开始的length字节写入到此缓存区</summary>
        public void Write(byte[] bytes, int length)
        {
            Write(bytes, 0, length);
        }
        /// <summary>将bytes字节数组写入到此缓存区</summary>
        public void Write(byte[] bytes)
        {
            Write(bytes, 0, bytes.Length);
        }

        /// <summary>写入<see cref="ByteBuffer"/>的有效缓冲区</summary>
        public void Write(ByteBuffer buffer)
        {
            if (buffer == null) return;
            if (buffer.ReadableBytes() <= 0) return;
            Write(buffer.ToArray());
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

        /// <summary> 写入<see cref="string"/> </summary>
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


        /// <summary> 读取<see cref="byte[]"/> </summary>
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

        /// <summary> 从读取索引位置开始读取数据到<paramref name="distBytes"/>中，并将其填满 </summary>
        public void ReadBytes(byte[] distBytes)
        {
            int size = distBytes.Length;

            ReadBytes(distBytes, 0, size);
        }
        /// <summary> 从读取索引位置开始读取<paramref name="len"/>长度的数据到<paramref name="distBytes"/>中 </summary>
        public void ReadBytes(byte[] distBytes, int distIndex, int len)
        {
            int size = distIndex + len;
            //if (ReadableBytes() < size) new Exception("BitBuffer：缓冲区有效数据不足");
            for (int i = distIndex; i < size; i++)
            {
                distBytes[i] = ReadByte();
            }
        }

        /// <summary> 读取<see cref="byte"/> </summary>
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

        /// <summary> 读取<see cref="short"/> </summary>
        public short ReadShort()
        {
            return BitConverter.ToInt16(Read(2), 0);
        }
        /// <summary> 读取<see cref="ushort"/> </summary>
        public ushort ReadUShort()
        {
            return BitConverter.ToUInt16(Read(2), 0);
        }

        /// <summary> 读取<see cref="int"/> </summary>
        public int ReadInt()
        {
            return BitConverter.ToInt32(Read(4), 0);
        }
        /// <summary> 读取<see cref="uint"/> </summary>
        public uint ReadUInt()
        {
            return BitConverter.ToUInt32(Read(4), 0);
        }

        /// <summary> 读取<see cref="int"/> </summary>
        public long ReadLong()
        {
            return BitConverter.ToInt64(Read(8), 0);
        }
        /// <summary> 读取<see cref="uint"/> </summary>
        public ulong ReadULong()
        {
            return BitConverter.ToUInt64(Read(8), 0);
        }

        /// <summary> 读取<see cref="float"/> </summary>
        public float ReadFloat()
        {
            return BitConverter.ToSingle(Read(4), 0);
        }

        /// <summary> 读取<see cref="double"/> </summary>
        public double ReadDouble()
        {
            return BitConverter.ToDouble(Read(8), 0);
        }

        /// <summary> 读取<see cref="bool"/> </summary>
        public bool ReadBool()
        {
            return BitConverter.ToBoolean(Read(2), 0);
        }

        /// <summary> 读取<see cref="char"/> </summary>
        public char ReadChar()
        {
            return BitConverter.ToChar(Read(2), 0);
        }

        /// <summary> 读取<see cref="string"/> </summary>
        public string ReadString()
        {
            int len = ReadInt();
            if (len < 0) return null;
            var bs = Read(len);
            return Encoding.UTF8.GetString(bs);
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
            _buffer = new byte[_buffer.Length];
            _readIndex = 0;
            _markReadIndex = 0;
            _writeIndex = 0;
            _markWriteIndex = 0;
        }

        /// <summary>设置开始读取的索引</summary>
        public void SetReadIndex(int index)
        {
            if (index < 0) return;
            _readIndex = index;
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

        /// <summary>剩余可读的有效字节数</summary>
        public int ReadableBytes()
        {
            return _writeIndex - _readIndex;
        }

        /// <summary>获取可读的字节数组</summary>
        public byte[] ToArray()
        {
            byte[] bytes = new byte[_writeIndex];
            Array.Copy(_buffer, 0, bytes, 0, bytes.Length);
            return bytes;
        }

        /// <summary>获取缓存区大小</summary>
        public int GetCapacity()
        {
            return _capacity;
        }

    }
}