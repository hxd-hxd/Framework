using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.Core.Network
{
    // 都是指定缓冲区索引的操作
    public partial class ByteBuffer
    {
        #region 指定缓冲区索引写入

        /// <summary>从指定的缓存区 <paramref name="index"/> 处将  <paramref name="bytes"/> 字节数组 <paramref name="startIndex"/> 开始的 <paramref name="length"/> 字节写入到此缓存区</summary>
        public void Write(int index, byte[] bytes, int startIndex, int length)
        {
            //if (bytes == null) throw new ArgumentNullException("不可写入空数据");
            if (bytes == null) return;

            int offset = length - startIndex;// 这是接下来要写入的数据长度
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

                if(_writeIndex < total) _writeIndex = total;// 如果写入的数据超过了写入索引，动态调整写入索引
            }
        }
        /// <summary>从指定的缓存区 <paramref name="index"/> 处将  <paramref name="bytes"/> 字节数组 <paramref name="startIndex"/> 开始的 <paramref name="length"/> 字节写入到此缓存区</summary>
        public void Write(int index, ArraySegment<byte> bytes, int startIndex, int length)
        {
            //if (bytes == null) throw new ArgumentNullException("不可写入空数据");
            if (bytes == null) return;

            int offset = length - startIndex;// 这是接下来要写入的数据长度
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

                if(_writeIndex < total) _writeIndex = total;// 如果写入的数据超过了写入索引，动态调整写入索引
            }
        }

        /// <summary>从指定的缓存区 <paramref name="index"/> 处将  <paramref name="bytes"/> 字节数组 0 开始的 <paramref name="length"/> 字节写入到此缓存区</summary>
        public void Write(int index, byte[] bytes, int length)
        {
            Write(index, bytes, 0, length);
        }
        /// <summary>从指定的缓存区 <paramref name="index"/> 处将  <paramref name="bytes"/> 字节数组 0 开始的 <paramref name="length"/> 字节写入到此缓存区</summary>
        public void Write(int index, ArraySegment<byte> bytes, int length)
        {
            Write(index, bytes, 0, length);
        }

        /// <summary>从指定的缓存区 <paramref name="index"/> 处将  <paramref name="bytes"/> 字节数组 0 开始的字节写入到此缓存区</summary>
        public void Write(int index, byte[] bytes)
        {
            Write(index, bytes, 0, bytes.Length);
        }
        /// <summary>从指定的缓存区 <paramref name="index"/> 处将  <paramref name="bytes"/> 字节数组 0 开始的字节写入到此缓存区</summary>
        public void Write(int index, ArraySegment<byte> bytes)
        {
            Write(index, bytes, 0, bytes.Count);
        }

        /// <summary>写入 <paramref name="buffer"/> 的有效缓冲区
        /// <para><paramref name="writeAll"/>：true 写入 <paramref name="buffer"/> 的所有数据</para>
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

        /// <summary> 从指定的缓存区 <paramref name="index"/> 处写入 <see cref="short"/> </summary>
        public void Write(int index, short v)
        {
            Write(index, filp(BitConverter.GetBytes(v)));
        }
        /// <summary> 从指定的缓存区 <paramref name="index"/> 处写入 <see cref="ushort"/> </summary>
        public void Write(int index, ushort v)
        {
            Write(index, filp(BitConverter.GetBytes(v)));
        }

        /// <summary> 从指定的缓存区 <paramref name="index"/> 处写入 <see cref="int"/> </summary>
        public void Write(int index, int v)
        {
            Write(index, filp(BitConverter.GetBytes(v)));
        }
        /// <summary> 从指定的缓存区 <paramref name="index"/> 处写入 <see cref="uint"/> </summary>
        public void Write(int index, uint v)
        {
            Write(index, filp(BitConverter.GetBytes(v)));
        }

        /// <summary> 从指定的缓存区 <paramref name="index"/> 处写入 <see cref="long"/> </summary>
        public void Write(int index, long v)
        {
            Write(index, filp(BitConverter.GetBytes(v)));
        }
        /// <summary> 从指定的缓存区 <paramref name="index"/> 处写入 <see cref="ulong"/> </summary>
        public void Write(int index, ulong v)
        {
            Write(index, filp(BitConverter.GetBytes(v)));
        }

        /// <summary> 从指定的缓存区 <paramref name="index"/> 处写入 <see cref="float"/> </summary>
        public void Write(int index, float v)
        {
            Write(index, filp(BitConverter.GetBytes(v)));
        }

        /// <summary> 从指定的缓存区 <paramref name="index"/> 处写入 <see cref="double"/> </summary>
        public void Write(int index, double v)
        {
            Write(index, filp(BitConverter.GetBytes(v)));
        }

        /// <summary> 从指定的缓存区 <paramref name="index"/> 处写入 <see cref="bool"/> </summary>
        public void Write(int index, bool v)
        {
            Write(index, filp(BitConverter.GetBytes(v)));
        }

        /// <summary> 从指定的缓存区 <paramref name="index"/> 处写入 <see cref="byte"/> </summary>
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

        /// <summary> 从指定的缓存区 <paramref name="index"/> 处写入 <see cref="char"/> </summary>
        public void Write(int index, char v)
        {
            Write(index, filp(BitConverter.GetBytes(v)));
        }

        /// <summary> 从指定的缓存区 <paramref name="index"/> 处写入 <see cref="string"/> 
        /// <para>注意：在写入 <see cref="string"/> 之前会自动写入一个代表其长度的 <see cref="int"/></para>
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
                // 先写入头部以确定字符串长度
                Write(index, bs.Length);
                index += bs.Length;
                Write(index, bs);
            }
        }

        #endregion

        /// <summary>从 <paramref name="index"/> 索引开始读取 <paramref name="len"/> 长度的 <see cref="byte"/>[]</summary>
        private byte[] Read(int index, int len)
        {
            lock (this)
            {
                // 只能读取已经写入的
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

        /// <summary>从 <paramref name="index"/> 索引开始读取<see cref="byte"/> </summary>
        public byte ReadByte(int index)
        {
            lock (this)
            {
                // 只能读取已经写入的
                if (index >= _writeIndex) return default;

                byte b = _buffer[index];
                return b;
            }
        }

        /// <summary>从 <paramref name="index"/> 索引开始读取<see cref="short"/> 
        /// </summary>
        public short ReadShort(int index)
        {
            return BitConverter.ToInt16(filp(Read(index, sizeof(short))), 0);
        }
        /// <summary>从 <paramref name="index"/> 索引开始读取<see cref="ushort"/> 
        /// </summary>
        public ushort ReadUShort(int index)
        {
            return BitConverter.ToUInt16(filp(Read(index, sizeof(ushort))), 0);
        }

        /// <summary>从 <paramref name="index"/> 索引开始读取<see cref="int"/> 
        /// </summary>
        public int ReadInt(int index)
        {
            return BitConverter.ToInt32(filp(Read(index, sizeof(int))), 0);
        }
        /// <summary>从 <paramref name="index"/> 索引开始读取<see cref="uint"/> 
        /// </summary>
        public uint ReadUInt(int index)
        {
            return BitConverter.ToUInt32(filp(Read(index, sizeof(uint))), 0);
        }

        /// <summary>从 <paramref name="index"/> 索引开始读取<see cref="int"/> 
        /// </summary>
        public long ReadLong(int index)
        {
            return BitConverter.ToInt64(filp(Read(index, sizeof(long))), 0);
        }
        /// <summary>从 <paramref name="index"/> 索引开始读取<see cref="uint"/> 
        /// </summary>
        public ulong ReadULong(int index)
        {
            return BitConverter.ToUInt64(filp(Read(index, sizeof(ulong))), 0);
        }

        /// <summary>从 <paramref name="index"/> 索引开始读取<see cref="float"/> 
        /// </summary>
        public float ReadFloat(int index)
        {
            return BitConverter.ToSingle(filp(Read(index, sizeof(float))), 0);
        }

        /// <summary>从 <paramref name="index"/> 索引开始读取<see cref="double"/> 
        /// </summary>
        public double ReadDouble(int index)
        {
            return BitConverter.ToDouble(filp(Read(index, sizeof(double))), 0);
        }

        /// <summary>从 <paramref name="index"/> 索引开始读取<see cref="bool"/> 
        /// </summary>
        public bool ReadBool(int index)
        {
            return BitConverter.ToBoolean(filp(Read(index, sizeof(bool))), 0);
        }

        /// <summary>从 <paramref name="index"/> 索引开始读取<see cref="char"/> </summary>
        public char ReadChar(int index)
        {
            return BitConverter.ToChar(filp(Read(index, sizeof(char))), 0);
        }

        /// <summary>从 <paramref name="index"/> 索引开始读取 <see cref="string"/> 
        /// <para><paramref name="useHeader"/>：true 首先解析出一个 int 作为字符串长度，根据该长度解析出字符串，false 将剩余的可读字节全部解析为 字符串</para>
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
        /// <summary>从 <paramref name="index"/> 索引开始读取 <see cref="string"/>
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