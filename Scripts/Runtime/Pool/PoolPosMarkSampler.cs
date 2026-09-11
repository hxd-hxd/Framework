using System;
using System.Collections.Generic;

namespace Framework.ObjectPool
{
    /// <summary>池位标采样器</summary>
    [Serializable]
    public class PoolPosMarkSampler
    {
        /// <summary>采样方式</summary>
        public PoolPosMarkSampleMode _sampleMode = PoolPosMarkSampleMode.Min;

        /// <summary>最小位置</summary>
        public Data _minPos = new()
        {
            pos = int.MaxValue,
        };

        /// <summary>存在采样</summary>
        public bool _hasSample;

        /// <summary>池中对象每次采样时的数据</summary>
        public List<Data> _datas = new List<Data>();

        /// <summary>池中对象每次采样相同时的数量统计</summary>
        public Dictionary<int, Data> _dataCount = new Dictionary<int, Data>();

        /// <summary>采样
        /// <para><paramref name="pos"/>：池对象数</para>
        /// </summary>
        public void Sample(int pos)
        {
            var curData = new Data
            {
                time = DateTime.UtcNow,
                pos = pos
            };

            if (_minPos.pos > pos || !_hasSample) _minPos = curData;
            if (_sampleMode == PoolPosMarkSampleMode.Full)
            {
                _datas.Add(curData);
            }

            if (_dataCount.TryGetValue(pos, out var count))
            {
                // 更新
                count.time = DateTime.UtcNow;
                count.pos += 1;
                _dataCount[pos] = count;
            }
            else
            {
                // 添加
                _dataCount[pos] = new Data
                {
                    time = DateTime.UtcNow,
                    pos = 1
                };
            }

            _hasSample = true;
        }

        /// <summary>清理采样数据</summary>
        public void Clear()
        {
            _minPos = new() { pos = int.MaxValue };
            _datas.Clear();
            _dataCount.Clear();

            _hasSample = false;
        }

        /// <summary>采样数据</summary>
        public struct Data
        {
            /// <summary>时间</summary>
            public DateTime time;
            /// <summary>位置</summary>
            public int pos;
        }
    }
}
