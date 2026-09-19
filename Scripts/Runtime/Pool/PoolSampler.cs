using System;
using System.Collections.Generic;

namespace Framework.ObjectPool
{
    /// <summary>池采样器</summary>
    [Serializable]
    public class PoolSampler
    {
        /// <summary>采样方式</summary>
        public PoolSampleMode _sampleMode = PoolSampleMode.Min;

        /// <summary>最小位置</summary>
        public Data _minPos = new()
        {
            pos = int.MaxValue,
        };

        /// <summary>存在采样</summary>
        public bool _hasSample;

        /// <summary>空池计数</summary>
        public int _nullPoolCount;

        /// <summary>开始采样时间</summary>
        public DateTime _startTime = DateTime.UtcNow;

        /// <summary>最新采样时间</summary>
        public DateTime _lastTime = DateTime.UtcNow;

        /// <summary>池中对象每次采样时的数据</summary>
        public List<Data> _datas = new List<Data>();

        /// <summary>池中对象每次采样相同时的数量统计</summary>
        public Dictionary<int, Data> _dataCount = new Dictionary<int, Data>();

        /// <summary>采样总时间</summary>
        public TimeSpan sampleTime => _lastTime - _startTime;

        /// <summary>采样
        /// <para><paramref name="pos"/>：池对象数</para>
        /// </summary>
        public void Sample(int pos)
        {
            _lastTime = DateTime.UtcNow;

            var curData = new Data
            {
                time = _lastTime,
                pos = pos
            };

            if (_minPos.pos > pos || !_hasSample) _minPos = curData;
            if (_sampleMode == PoolSampleMode.Full)
            {
                _datas.Add(curData);
            }

            if (_dataCount.TryGetValue(pos, out var count))
            {
                // 更新
                count.time = _lastTime;
                count.pos += 1;
                _dataCount[pos] = count;
            }
            else
            {
                // 添加
                _dataCount[pos] = new Data
                {
                    time = _lastTime,
                    pos = 1
                };
            }

            _hasSample = true;
        }

        /// <summary>根据当前采样数据判断是否空池</summary>
        public bool IsNullPool()
        {
            // 只统计到一种数据，并且最低是 0 
            bool isNuulPool = _dataCount.Count == 1 && _minPos.pos == 0;
            return isNuulPool;
        }

        /// <summary>清理采样数据</summary>
        public void ClearSample()
        {
            // 注意空池计数不在这里清除，因为其可能跨多个清理周期
            _minPos.pos = int.MaxValue;
            _minPos.time = default;
            _datas.Clear();
            _dataCount.Clear();

            _hasSample = false;
            _lastTime = _startTime = DateTime.UtcNow;
        }

        /// <summary>清理所有数据</summary>
        public void Clear()
        {
            ClearSample();
            _nullPoolCount = 0;
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
