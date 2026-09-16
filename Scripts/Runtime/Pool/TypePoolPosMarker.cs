using System;
using System.Collections.Generic;
using Framework.Runtime;
using UnityEngine;

namespace Framework.ObjectPool
{
    /// <summary><see cref="TypePool"/> 池位标器</summary>
    [Serializable]
    public class TypePoolPosMarker
    {
        private TypePool _pool;

        [SerializeField]
        private PositionMarker _marker = new PositionMarker();

        private Dictionary<Type, PoolPosMarkerItem> _poolMarkers = new Dictionary<Type, PoolPosMarkerItem>();
        private Dictionary<Type, Dictionary<int, PoolPosMarkerItem>> _arrayPoolMarkers = new Dictionary<Type, Dictionary<int, PoolPosMarkerItem>>();

        // 专用位标器信息配置
        private Dictionary<Type, PositionMarkerInfo> _markerInfos = new Dictionary<Type, PositionMarkerInfo>();
        private Dictionary<Type, Dictionary<int, PositionMarkerInfo>> _arrayMarkerInfos = new Dictionary<Type, Dictionary<int, PositionMarkerInfo>>();

        public TypePoolPosMarker()
        {
            Init();
        }

        public TypePoolPosMarker(TypePool pool)
        {
            this.pool = pool;

            Init();
        }

        /// <summary>公用位标器</summary>
        public PositionMarker marker
        {
            get => _marker;
            set
            {
                if (_marker != null && _marker != value)
                {
                    foreach (var item in _poolMarkers)
                    {
                        if (item.Value.marker == null)
                            item.Value.Reset();
                    }
                    foreach (var item in _arrayPoolMarkers)
                    {
                        foreach (var aItem in item.Value)
                        {
                            if (aItem.Value.marker == null)
                                aItem.Value.Reset();
                        }
                    }
                    _marker.RemoveListeners();
                }
                _marker = value;
                Init();
            }
        }

        public TypePool pool
        {
            get => _pool;
            set
            {
                if (_pool != null && _pool != value)
                {
                    Clear();
                }
                _pool = value;
            }
        }

        public void Init()
        {
            if (_marker != null)
            {
                _marker.onSample.RemoveListener(OnSample);
                _marker.onClear.RemoveListener(OnClear);
                _marker.onSample.AddListener(OnSample);
                _marker.onClear.AddListener(OnClear);
            }
        }

        /// <summary>更新位标器</summary>
        public void Update(float elapseTime, float realElapseTime)
        {
            if (_pool == null) return;

            UpdateMarkerAdd();

            _marker?.Update(elapseTime, realElapseTime);

            // 轮询专用位标器
            foreach (var item in _poolMarkers)
            {
                item.Value.marker?.Update(elapseTime, realElapseTime);
            }
            foreach (var item in _arrayPoolMarkers)
            {
                foreach (var aMarker in item.Value)
                {
                    aMarker.Value.marker?.Update(elapseTime, realElapseTime);
                }
            }
        }

        public void Clear()
        {
            foreach (var item in _poolMarkers)
            {
                Destroy(item.Value);
            }
            _poolMarkers.Clear();

            foreach (var item in _arrayPoolMarkers)
            {
                foreach (var aItem in item.Value)
                {
                    Destroy(aItem.Value);
                }
                InternalTypePool.root.Return(item.Value);
            }
            _arrayPoolMarkers.Clear();

            // 专用位标器信息
            _markerInfos.Clear();

            foreach (var item in _arrayMarkerInfos)
            {
                InternalTypePool.root.Return(item.Value);
            }
            _arrayMarkerInfos.Clear();
        }

        /// <summary>添加专用位标器</summary>
        public void AddMarker(Type type, PositionMarkerInfo info)
        {
            if (type.IsArray)
            {
                throw new ArrayTypeMismatchException("不支持数组类型");
            }

            _markerInfos[type] = info;

            // 立马尝试添加对应的池，并创建位标器
            AddAndCreateMarker(type, info);
        }

        /// <summary>添加数组池专用位标器</summary>
        public void AddMarker(Type type, int length, PositionMarkerInfo info)
        {
            if (!_arrayMarkerInfos.TryGetValue(type, out var aInfos))
            {
                InternalTypePool.root.TryGet(out aInfos);
                _arrayMarkerInfos[type] = aInfos;
            }
            aInfos[length] = info;

            // 立马尝试添加对应的池，并创建位标器
            AddAndCreateMarker(type, length, info);
        }

        /// <summary>移除专用位标器</summary>
        public void RemoveMarker(Type type)
        {
            // 移除信息
            _markerInfos.Remove(type);
            // 立马移除位标器
            if (_poolMarkers.TryGetValue(type, out var marker))
            {
                marker.RemoveMarker();
                marker.sampler.Clear();
            }
        }

        /// <summary>移除数组池专用位标器</summary>
        public void RemoveMarker(Type type, int length)
        {
            // 移除信息
            if (_arrayMarkerInfos.TryGetValue(type, out var tInfos))
            {
                tInfos.Remove(length);
                if (tInfos.Count <= 0)
                {
                    _arrayMarkerInfos.Remove(type);
                    InternalTypePool.root.Return(tInfos);
                }
            }
            // 立马移除位标器
            if (_arrayPoolMarkers.TryGetValue(type, out var tMarker))
                if (tMarker.TryGetValue(length, out var marker))
                {
                    marker.RemoveMarker();
                    marker.sampler.Clear();
                }
        }

        // 添加位标器
        private void UpdateMarkerAdd()
        {
            foreach (var item in _markerInfos)
            {
                var type = item.Key;
                var info = item.Value;
                AddAndCreateMarker(type, info);
            }

            foreach (var item in _arrayMarkerInfos)
            {
                var type = item.Key;
                foreach (var aInfos in item.Value)
                {
                    var length = aInfos.Key;
                    var info = aInfos.Value;
                    AddAndCreateMarker(type, length, info);
                }
            }
        }

        // 添加池
        private void UpdatePoolAdd()
        {
            // 扫描所有池并添加

            // 通用池
            foreach (var kvp in _pool.pool)
            {
                var type = kvp.Key;
                if (!_poolMarkers.ContainsKey(type))
                {
                    _poolMarkers.Add(type, Create());
                }
            }

            // 数组池
            foreach (var kvp in _pool.arrayPool.pool)
            {
                var type = kvp.Key;
                // 类型
                if (!_arrayPoolMarkers.TryGetValue(type, out var tMarker))
                {
                    tMarker = InternalTypePool.root.GetDic<int, PoolPosMarkerItem>();
                    _arrayPoolMarkers.Add(type, tMarker);
                }

                // 数量
                foreach (var aItem in kvp.Value)
                {
                    var length = aItem.Key;
                    if (!tMarker.ContainsKey(length))
                    {
                        tMarker.Add(length, Create());
                    }
                }
            }
        }

        // 回收池
        private void UpdatePoolRemove()
        {
            /* 回收条件
            1、没有对应的类型
            2、空池暂留，TODO：一定时间后将空池一并清理掉
             */

            var tempTypes = InternalTypePool.root.GetList<Type>();
            var tempInts = InternalTypePool.root.GetDic<Type, List<int>>();

            // 通用池
            foreach (var item in _poolMarkers)
            {
                var type = item.Key;
                if (!_pool.pool.ContainsKey(type))
                {
                    tempTypes.Add(type);
                }
            }
            foreach (var type in tempTypes)
            {
                Destroy(_poolMarkers[type]);
                _poolMarkers.Remove(type);
            }

            tempTypes.Clear();
            // 数组池
            foreach (var item in _arrayPoolMarkers)
            {
                var type = item.Key;
                // 类型
                if (!_pool.arrayPool.pool.TryGetValue(type, out var tPool))
                {
                    tempTypes.Add(type);
                    // 这里截断，进了类型的就不进数量
                    continue;
                }

                // 数量
                List<int> lengths = null;
                foreach (var aMarker in item.Value)
                {
                    int length = aMarker.Key;
                    if (!tPool.ContainsKey(length))
                    {
                        lengths ??= InternalTypePool.root.GetList<int>();
                        lengths.Add(length);
                    }
                }
                if (lengths != null)
                {
                    tempInts[type] = lengths;
                }
            }
            // 按类型回收
            foreach (var type in tempTypes)
            {
                var aDic = _arrayPoolMarkers[type];
                foreach (var aMarker in aDic)
                {
                    Destroy(aMarker.Value);
                }
                InternalTypePool.root.Return(aDic);
                _arrayPoolMarkers.Remove(type);
            }
            // 按数量回收
            foreach (var item in tempInts)
            {
                var type = item.Key;
                var aDic = _arrayPoolMarkers[type];
                foreach (var length in item.Value)
                {
                    Destroy(aDic[length]);
                    aDic.Remove(length);
                }
                // 如果全部被回收，则类型也没必要保留
                if (aDic.Count == 0)
                {
                    InternalTypePool.root.Return(aDic);
                    _arrayPoolMarkers.Remove(type);
                }
            }

            InternalTypePool.root.Return(tempTypes);
            foreach (var item in tempInts)
            {
                InternalTypePool.root.Return(item.Value);
            }
            InternalTypePool.root.Return(tempInts);
        }

        private void OnSample(PositionMarker marker)
        {
            if (_pool == null) return;

            UpdatePoolAdd();

            foreach (var item in _poolMarkers)
            {
                // 检查没有专用位标器的
                if (item.Value.marker == null)
                {
                    var type = item.Key;
                    // 对池类型对象采样
                    var count = _pool.GetFreeCount(type);
                    item.Value.sampler.Sample(count);
                }
            }

            foreach (var item in _arrayPoolMarkers)
            {
                foreach (var aItem in item.Value)
                {
                    // 检查没有专用位标器的
                    if (aItem.Value.marker == null)
                    {
                        var type = item.Key;
                        var length = aItem.Key;
                        // 对池类型对象采样
                        var count = _pool.arrayPool.GetFreeCount(type, length);
                        aItem.Value.sampler.Sample(count);
                    }
                }
            }
        }

        private void OnClear(PositionMarker marker)
        {
            if (_pool == null) return;

            foreach (var item in _poolMarkers)
            {
                // 检查没有专用位标器的
                if (item.Value.marker == null)
                {
                    var type = item.Key;
                    var sampler = item.Value.sampler;
                    if (!sampler._hasSample) continue;
                    var pos = sampler._minPos.pos;
                    _pool.Remove(type, pos);
                    sampler.Clear();
                }
            }

            foreach (var item in _arrayPoolMarkers)
            {
                foreach (var aItem in item.Value)
                {
                    // 检查没有专用位标器的
                    if (aItem.Value.marker == null)
                    {
                        var type = item.Key;
                        var length = aItem.Key;
                        var sampler = aItem.Value.sampler;
                        if (!sampler._hasSample) continue;
                        var pos = sampler._minPos.pos;

                        _pool.arrayPool.Remove(type, length, pos);
                        sampler.Clear();
                    }
                }
            }

            UpdatePoolRemove();
        }

        /// <summary>尝试添加并创建专用位标器</summary>
        private void AddAndCreateMarker(Type type, PositionMarkerInfo info)
        {
            if (_pool == null) return;

            // 添加对应的池
            if (_pool.pool.ContainsKey(type)
                && !_poolMarkers.ContainsKey(type))
            {
                _poolMarkers.Add(type, Create());
            }

            // 创建位标器
            CreateMarker(type, info);
        }

        /// <summary>尝试添加并创建专用位标器</summary>
        private void AddAndCreateMarker(Type type, int length, PositionMarkerInfo info)
        {
            if (_pool == null) return;

            Dictionary<int, PoolPosMarkerItem> tMarker = null;
            // 添加对应的池
            if (_pool.arrayPool.pool.TryGetValue(type, out var aPool)
                && !_arrayPoolMarkers.TryGetValue(type, out tMarker))
            {
                tMarker = InternalTypePool.root.GetDic<int, PoolPosMarkerItem>();
                _arrayPoolMarkers.Add(type, tMarker);
            }
            if (tMarker != null
                && aPool.ContainsKey(length)
                && !tMarker.ContainsKey(length))
            {
                tMarker.Add(length, Create());
            }

            // 立马创建位标器
            CreateMarker(type, length, info);
        }

        /// <summary>创建专用位标器</summary>
        private void CreateMarker(Type type, PositionMarkerInfo info)
        {
            if (_poolMarkers.TryGetValue(type, out var posMarker))
            {
                var marker = posMarker.marker;
                if (marker == null)
                {
                    posMarker.sampler.Clear();

                    InternalTypePool.root.TryGet(out marker);
                    posMarker.marker = marker;

                    // 绑定事件
                    marker.onSample.AddListener((_) =>
                    {
                        // 对池类型对象采样
                        var count = _pool.GetFreeCount(type);
                        posMarker.sampler.Sample(count);
                        posMarker.marker?.overrideInfo?.onSample?.Invoke(type);
                    });
                    marker.onClear.AddListener((_) =>
                    {
                        var sampler = posMarker.sampler;
                        if (!sampler._hasSample) return;
                        var pos = sampler._minPos.pos;
                        _pool.Remove(type, pos);
                        sampler.Clear();
                        posMarker.marker?.overrideInfo?.onClear?.Invoke(type);
                    });
                }
                else
                {
                    if (marker.overrideInfo != info)
                        posMarker.Reset();
                }

                marker.overrideInfo = info;
            }
        }

        /// <summary>创建专用位标器</summary>
        private void CreateMarker(Type type, int length, PositionMarkerInfo info)
        {
            if (_arrayPoolMarkers.TryGetValue(type, out var aMarker))
            {
                if (!aMarker.TryGetValue(length, out var posMarker)) return;
                var marker = posMarker.marker;
                if (marker == null)
                {
                    posMarker.sampler.Clear();

                    InternalTypePool.root.TryGet(out marker);
                    posMarker.marker = marker;

                    // 绑定事件
                    marker.onSample.AddListener((_) =>
                    {
                        // 对池类型对象采样
                        var count = _pool.arrayPool.GetFreeCount(type, length);
                        posMarker.sampler.Sample(count);
                        posMarker.marker?.overrideInfo?.onSampleArray?.Invoke(type, length);
                    });
                    marker.onClear.AddListener((_) =>
                    {
                        var sampler = posMarker.sampler;
                        if (!sampler._hasSample) return;
                        var pos = sampler._minPos.pos;
                        _pool.arrayPool.Remove(type, length, pos);
                        sampler.Clear();
                        posMarker.marker?.overrideInfo?.onClearArray?.Invoke(type, length);
                    });
                }
                else
                {
                    if (marker.overrideInfo != info)
                    {
                        posMarker.Reset();
                    }
                }

                marker.overrideInfo = info;
            }
        }

        private PoolPosMarkerItem Create()
        {
            return InternalTypePool.root.Get<PoolPosMarkerItem>();
        }

        private void Destroy(PoolPosMarkerItem marker)
        {
            InternalTypePool.root.Return(marker);
        }

    }
}
