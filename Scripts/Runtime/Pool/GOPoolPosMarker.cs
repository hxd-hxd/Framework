using System;
using System.Collections.Generic;
using Framework.Runtime;
using UnityEngine;

namespace Framework.ObjectPool
{
    /// <summary><see cref="GameObjectPool"/> 池位标器</summary>
    [Serializable]
    public sealed class GOPoolPosMarker
    {
        [Tooltip("空池保留轮数，例如：1，清理时检测到空池则保留 1 轮，下一轮如果仍为空池则销毁，否则重置")]
        [SerializeField]
        private int _nullPoolReserveNum = 1;

        private GameObjectPool _pool;

        [SerializeField]
        private PositionMarker _marker = new PositionMarker();

        private Dictionary<GameObject, PoolPosMarkerItem> _poolMarkers = new Dictionary<GameObject, PoolPosMarkerItem>();

        // 专用位标器信息配置
        private Dictionary<GameObject, PositionMarkerInfo> _markerInfos = new Dictionary<GameObject, PositionMarkerInfo>();

        // 标记需要移除
        private bool _needPoolRemove;

        public GameObjectPool pool
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
                    _marker.RemoveListeners();
                }
                _marker = value;
                Init();
            }
        }

        /// <summary>空池保留轮数</summary>
        public int nullPoolReserveNum { get => _nullPoolReserveNum; set => _nullPoolReserveNum = value; }

        public GOPoolPosMarker()
        {
            Init();
        }

        public GOPoolPosMarker(GameObjectPool pool)
        {
            this.pool = pool;

            Init();
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
                if (item.Key == null) continue;// 死键不更新
                item.Value.marker?.Update(elapseTime, realElapseTime);
            }

            if (_needPoolRemove)
            {
                UpdatePoolRemove();
                _needPoolRemove = false;
            }
        }

        public void Clear()
        {
            foreach (var item in _poolMarkers)
            {
                Destroy(item.Value);
            }
            _poolMarkers.Clear();

            // 专用位标器信息
            _markerInfos.Clear();

            _needPoolRemove = false;
        }

        /// <summary>添加专用位标器</summary>
        public void AddMarker(GameObject template, PositionMarkerInfo info)
        {
            if (template == null) return;

            _markerInfos[template] = info;

            // 立马尝试添加对应的池，并创建位标器
            AddAndCreateMarker(template, info);
        }

        /// <summary>移除专用位标器</summary>
        public void RemoveMarker(GameObject template)
        {
            // 移除信息
            _markerInfos.Remove(template);
            // 立马移除位标器
            if (_poolMarkers.TryGetValue(template, out var marker))
            {
                marker.RemoveMarker();
                marker.sampler.ClearSample();
            }
        }

        // 添加位标器
        private void UpdateMarkerAdd()
        {
            foreach (var item in _markerInfos)
            {
                var template = item.Key;
                if (template == null) continue;
                var info = item.Value;
                AddAndCreateMarker(template, info);
            }
        }

        // 添加池
        private void UpdatePoolAdd()
        {
            // 扫描所有池并添加
            foreach (var kvp in _pool.pool)
            {
                var template = kvp.Key;
                if (template == null) continue;
                if (!_poolMarkers.ContainsKey(template))
                {
                    _poolMarkers.Add(template, Create());
                }
            }
        }

        // 回收池
        private void UpdatePoolRemove()
        {
            /* 回收条件
            模板键被销毁
            没有对应的键
             */

            var tempGOs = InternalTypePool.root.GetList<GameObject>();

            // 扫描池中死键
            foreach (var item in _pool.pool)
            {
                var template = item.Key;
                if (template == null) tempGOs.Add(template);
            }

            // 扫描位标无效键
            foreach (var item in _poolMarkers)
            {
                var template = item.Key;
                if (template == null || !_pool.pool.ContainsKey(template))
                {
                    tempGOs.Add(template);
                }
            }

            foreach (var item in _markerInfos)
            {
                var template = item.Key;
                // 移除死键专用位标器
                if (template == null)
                    tempGOs.Add(template);
            }

            // 统一销毁
            foreach (var template in tempGOs)
            {
                // 销毁死键池
                _pool.Destroy(template);

                // 回收位标器
                if (_poolMarkers.TryGetValue(template, out var marker))
                {
                    Destroy(marker);
                    _poolMarkers.Remove(template);
                }

                // 移除死键专用位标器
                if (template == null) _markerInfos.Remove(template);
            }

            InternalTypePool.root.Return(tempGOs);
        }

        // 空池处理
        private void NullPoolHandle(PoolSampler sampler, out bool isNullPool, out bool isDestroy, Func<bool> handle)
        {
            isDestroy = false;
            // 记录空池
            isNullPool = sampler.IsNullPool();
            if (isNullPool)
            {
                if (sampler._nullPoolCount < nullPoolReserveNum)
                {
                    sampler._nullPoolCount += 1;
                }
                else
                {
                    isDestroy = handle();
                    sampler._nullPoolCount = 0;
                }
            }
            else
            {
                // 空池计数期间任何一次非空池都会重置
                sampler._nullPoolCount = 0;
            }
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
                    var template = item.Key;
                    // 对池类型对象采样
                    // 如果模板被销毁，特别是有多个模板被销毁，采样可能得到的都是第一个空模板的
                    // 死键不采样
                    if (template == null) continue;
                    int count = _pool.GetFreeCount(template);
                    item.Value.sampler.Sample(count);
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
                    var template = item.Key;
                    if (template == null) continue;// 死键不按采样清理
                    var sampler = item.Value.sampler;
                    if (!sampler._hasSample) continue;

                    NullPoolHandle(sampler, out var isNullPool, out _, () =>
                    {
                        if (_pool.GetFreeCount(template) > 0)
                        {
                            return false;
                        }
                        // 达到保留轮数则销毁空池
                        _pool.Destroy(template);
                        return true;
                    });

                    if (!isNullPool)
                    {
                        var pos = sampler._minPos.pos;
                        _pool.Remove(template, pos);
                    }

                    sampler.ClearSample();
                }
            }

            UpdatePoolRemove();
        }

        /// <summary>尝试添加并创建专用位标器</summary>
        private void AddAndCreateMarker(GameObject template, PositionMarkerInfo info)
        {
            if (_pool == null || template == null) return;

            // 添加对应的池
            if (_pool.pool.ContainsKey(template)
                && !_poolMarkers.ContainsKey(template))
            {
                _poolMarkers.Add(template, Create());
            }

            // 创建位标器
            CreateMarker(template, info);
        }

        /// <summary>创建专用位标器</summary>
        private void CreateMarker(GameObject template, PositionMarkerInfo info)
        {
            if (_poolMarkers.TryGetValue(template, out var posMarker))
            {
                var marker = posMarker.marker;
                if (marker == null)
                {
                    posMarker.sampler.ClearSample();

                    InternalTypePool.root.TryGet(out marker);
                    posMarker.marker = marker;

                    // 绑定事件
                    marker.onSample.AddListener((_) =>
                    {
                        // 对池类型对象采样
                        if (template == null) return;
                        var count = _pool.GetFreeCount(template);
                        posMarker.sampler.Sample(count);
                        posMarker.marker?.overrideInfo?.onSampleGO?.Invoke(template);
                    });
                    marker.onClear.AddListener((_) =>
                    {
                        if (template == null) return;
                        var sampler = posMarker.sampler;
                        if (!sampler._hasSample) return;

                        NullPoolHandle(sampler, out var isNullPool, out var isDestroy, () =>
                        {
                            if (_pool.GetFreeCount(template) > 0)
                            {
                                return false;
                            }
                            // 达到保留轮数则销毁空池
                            _pool.Destroy(template);
                            return true;
                        });

                        if (!isNullPool)
                        {
                            var pos = sampler._minPos.pos;
                            _pool.Remove(template, pos);
                        }

                        sampler.ClearSample();
                        posMarker.marker?.overrideInfo?.onClearGO?.Invoke(template);

                        if (isDestroy)
                        {
                            _needPoolRemove = true;
                        }
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
