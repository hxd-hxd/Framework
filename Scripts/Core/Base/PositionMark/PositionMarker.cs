using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Core
{
    // 用于处理位标法采样
    /// <summary>
    /// 位标器
    /// </summary>
    public class PositionMarker
    {
        protected float _clearTime = 10 * 60;// 清理周期，默认10分钟
        protected float _sampleTime = 1;// 采样时间

        protected int _clearCount;// 采样次数

        protected float _clearTimeer;// 清理周期计时器
        protected float _sampleTimeer;// 采样时间计时器

        public event Action onSample;
        public event Action onClear;

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="elapseTime">流逝的时间</param>
        /// <param name="realElapseTime">真实流失的时间</param>
        public virtual void Update(float elapseTime, float realElapseTime)
        {
            _clearTimeer += realElapseTime;
            _sampleTimeer += realElapseTime;

            if (_sampleTimeer >= _sampleTime)
            {
                _sampleTimeer = 0;
                Sample();
            }

            if (_clearTimeer >= _clearTime)
            {
                _clearTimeer = 0;
                Clear();
            }
        }

        /// <summary>
        /// 采样
        /// </summary>
        public virtual void Sample()
        {

        }

        /// <summary>
        /// 清理
        /// </summary>
        public virtual void Clear()
        {

        }


    }
}
