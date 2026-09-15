using System;
using System.Collections.Generic;
using Framework.Runtime;
using UnityEngine;

namespace Framework.ObjectPool
{
    /// <summary><see cref="GameObjectPool"/> 池位标器</summary>
    [Serializable]
    public class GOPoolPosMarker
    {
        private GameObjectPool _pool;

        [SerializeField]
        private PositionMarker _marker = new PositionMarker();

        private Dictionary<GameObject, PoolPosMarker> _poolMarkers = new Dictionary<GameObject, PoolPosMarker>();

        // 专用位标器信息配置
        private Dictionary<GameObject, PositionMarkerInfo> _markerInfos = new Dictionary<GameObject, PositionMarkerInfo>();

        public GOPoolPosMarker()
        {
            Init();
        }

        public GOPoolPosMarker(GameObjectPool pool)
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
                    _marker.RemoveListeners();
                }
                _marker = value;
                Init();
            }
        }

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
        }

        /// <summary>添加专用位标器</summary>
        public void AddMarker(GameObject template, PositionMarkerInfo info)
        {
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
                marker.sampler.Clear();
            }
        }

        // 添加位标器
        private void UpdateMarkerAdd()
        {
            foreach (var item in _markerInfos)
            {
                var template = item.Key;
                var info = item.Value;
                AddAndCreateMarker(template, info);
            }
        }

        // 添加池
        private void UpdatePoolAdd()
        {
            // 扫描所有池并添加

            // 通用池
            foreach (var kvp in _pool.pool)
            {
                var template = kvp.Key;
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
            1、没有对应的类型
            2、空池暂留，TODO：一定时间后将空池一并清理掉
             */

            var tempGOs = InternalTypePool.root.GetList<GameObject>();

            // 通用池
            foreach (var item in _poolMarkers)
            {
                var template = item.Key;
                if (!_pool.pool.ContainsKey(template))
                {
                    tempGOs.Add(template);
                }
            }
            foreach (var template in tempGOs)
            {
                Destroy(_poolMarkers[template]);
                _poolMarkers.Remove(template);
            }

            InternalTypePool.root.Return(tempGOs);
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
                    var count = _pool.GetFreeCount(template);
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
                    var sampler = item.Value.sampler;
                    if (!sampler._hasSample) continue;
                    var pos = sampler._minPos.pos;
                    _pool.Remove(template, pos);
                    sampler.Clear();
                }
            }

            UpdatePoolRemove();
        }

        /// <summary>尝试添加并创建专用位标器</summary>
        private void AddAndCreateMarker(GameObject template, PositionMarkerInfo info)
        {
            if (_pool == null) return;

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
                    posMarker.sampler.Clear();

                    InternalTypePool.root.TryGet(out marker);
                    posMarker.marker = marker;

                    // 绑定事件
                    marker.onSample.AddListener((_) =>
                    {
                        // 对池类型对象采样
                        var count = _pool.GetFreeCount(template);
                        posMarker.sampler.Sample(count);
                        posMarker.marker?.overrideInfo?.onSampleGO?.Invoke(template);
                    });
                    marker.onClear.AddListener((_) =>
                    {
                        var sampler = posMarker.sampler;
                        if (!sampler._hasSample) return;
                        var pos = sampler._minPos.pos;
                        _pool.Remove(template, pos);
                        sampler.Clear();
                        posMarker.marker?.overrideInfo?.onClearGO?.Invoke(template);
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

        private PoolPosMarker Create()
        {
            return InternalTypePool.root.Get<PoolPosMarker>();
        }

        private void Destroy(PoolPosMarker marker)
        {
            InternalTypePool.root.Return(marker);
        }
    }
}
