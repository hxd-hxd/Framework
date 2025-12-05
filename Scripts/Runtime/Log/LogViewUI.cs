using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.LogSystem
{
    /// <summary>
    /// 日志操作视图 ui 
    /// </summary>
    public class LogViewUI : UIBase
    {
        [SerializeField]
        private GameObject _logInfoItemUITemplate;
        [SerializeField]
        private Transform _logInfoItemUIParent;
        private GameObjectPool _logInfoItemUIPool;
        private LinkedList<LogInfoItemUI> _logInfoItemUIs;// 所有显示的日志项
        private LogInfoItemUI _selectLogInfoItemUI;// 当前选择的 log ui
        [SerializeField]
        private int _logInfoItemUIShowMaxNum = 200;// 允许日志显示的最大数量
        private Dictionary<LogShowType, LogShowTypeNum> _logShowTypeNumDic;

        [Space]
        [SerializeField]
        private Text _conditionText;
        [SerializeField]
        private Text _stackTraceText;

        [Space]
        [SerializeField]
        private Button _closeBtn;
        [SerializeField]
        private Button _clearLogBtn, _copySelectedLogBtn, _copyAllLogBtn;
        [SerializeField]
        private Toggle _infoToggle, _warningToggle, _errorToggle, _excptionToggle;
        [SerializeField]
        private Text _logNumText;// 显示日志数量

        [SerializeField]
        private LogShowType _logShowType = LogShowType.All;

        /// <summary>
        /// 关闭时触发此事件
        /// </summary>
        public Action closeClickEvent;

        public GameObject logInfoItemUITemplate
        {
            get => _logInfoItemUITemplate;
            set
            {
                _logInfoItemUITemplate = value;
                _logInfoItemUIPool.template = value;
            }
        }

        protected override void Awake()
        {
            base.Awake();

            _logInfoItemUIPool ??= new GameObjectPool(1, _logInfoItemUITemplate);
            //_logInfoItemUIs ??= new List<LogInfoItemUI>(_logInfoItemUIShowMaxNum);
            _logInfoItemUIs ??= new LinkedList<LogInfoItemUI>();
            _logShowTypeNumDic ??= new Dictionary<LogShowType, LogShowTypeNum>();

            // 根据ui的设置初始化日志显示类型
            //if (_infoToggle) SetLogShowType(_infoToggle.isOn, LogShowType.Info);
            //if (_warningToggle) SetLogShowType(_warningToggle.isOn, LogShowType.Warning);
            //if (_errorToggle) SetLogShowType(_errorToggle.isOn, LogShowType.Error);
            //if (_excptionToggle) SetLogShowType(_excptionToggle.isOn, LogShowType.Exception);

        }

        protected override void Start()
        {
            base.Start();

            _logInfoItemUIPool.returnParent = _logInfoItemUIParent;
            _logInfoItemUIPool.PreCreateInstanceAsync(100);

            AddEvent(_closeBtn, () =>
            {
                Enable(false);
                closeClickEvent?.Invoke();
            });
            AddEvent(_clearLogBtn, () =>
            {
                // 清除
                ClearLog();
            });
            AddEvent(_copySelectedLogBtn, () =>
            {
                // 复制日志
                //_selectLogInfoItemUI .0
                if (_selectLogInfoItemUI)
                {
                    //GUIUtility.systemCopyBuffer = _selectLogInfoItemUI.logInfo.condition;
                    GUIUtility.systemCopyBuffer = _selectLogInfoItemUI.logInfo.ToText();
                }
            });
            AddEvent(_copyAllLogBtn, () =>
            {
                // 复制日志
                GUIUtility.systemCopyBuffer = LogInfo.ToFileFormatTextAll();
            });

            // 注册日志显示类型控制事件
            Init(_infoToggle, LogShowType.Info);
            Init(_warningToggle, LogShowType.Warning);
            Init(_errorToggle, LogShowType.Error);
            Init(_excptionToggle, LogShowType.Exception);

        }

        private void Init(Toggle toggle, LogShowType type)
        {
            if (toggle)
            {
                // 根据日志查看类型初始化ui
                if (toggle) toggle.isOn = _logShowType.HasFlag(type);
                // 添加查看日志显示数量
                var numText = toggle.transform.FindOf<Text>("Num");
                if (!_logShowTypeNumDic.TryGetValue(type, out var showNum))
                {
                    _logShowTypeNumDic[type] = showNum = new LogShowTypeNum();
                }
                showNum.text = numText;
                // 注册事件
                toggle.onValueChanged.AddListener((isOn) =>
                {
                    SetLogShowType(isOn, type);
                });
            }
        }

        private void Update()
        {
            if (_logNumText)
            {
                //_logNumText.text = $"{_logInfoItemUIs.Count}/{_logInfoItemUIShowMaxNum}";
                _logNumText.text = $"显示数量 {_logInfoItemUIs.Count}/{_logInfoItemUIShowMaxNum}，总量 {LogInfo.logInfoCount}";
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            // 更新日志显示
            UpdateLogUI();

            LogInfo.logMessageReceived += OnHandleLog;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            LogInfo.logMessageReceived -= OnHandleLog;
        }

        // 接收处理日志
        //private void OnHandleLog(string condition, string stackTrace, LogType type)
        public void OnHandleLog(LogInfo info)
        {
            AddLogUI(info);
        }

        /// <summary>
        /// 刷新日志 ui
        /// </summary>
        public void RefreshLogUI()
        {
            //for (int i = 0; i < _logInfoItemUIs.Count; i++)
            //{
            //    var _ui = _logInfoItemUIs[i];
            //    int n = i + 1;
            //    SetLogUIFills(_ui, n);
            //}
            int n = 0;
            foreach (var _ui in _logInfoItemUIs)
            {
                SetLogUIFills(_ui, n++);
            }
        }
        // 设置填充区分色块
        void SetLogUIFills(LogInfoItemUI _ui, int n)
        {
            // 双数打开背景
            if (_ui)
            {
                _ui.isOpenBG = n % 2 == 0;
            }
        }

        /// <summary>
        /// 更新日志 ui
        /// </summary>
        public void UpdateLogUI()
        {
            ClearLog();

            // 取出限制数量的 日志
            // 计算开始索引
            int startIndex = LogInfo.logInfos.Count - _logInfoItemUIShowMaxNum;
            startIndex = Mathf.Max(startIndex, 0);
            for (int i = startIndex; i < LogInfo.logInfos.Count; i++)
            {
                var info = LogInfo.logInfos[i];
                AddLogUI(info);
            }

            //// 更新显示类型
            //ShowLogType();
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="info"></param>
        public void AddLogUI(LogInfo info)
        {
            var _ui = _logInfoItemUIPool.Get(_logInfoItemUIParent).GetComponent<LogInfoItemUI>();
            if (_ui != null)
            {
                //_ui.transform.SetParent(_logInfoItemUIParent);
                _ui.transform.SetAsLastSibling();
                if (!_ui.gameObject.activeSelf)
                    _ui.gameObject.SetActive(true);
                _ui.SetLogInfo(info);
                _ui.clickEvent = () =>
                {
                    // 设置点击事件
                    SelectLogItemUI(_ui);
                };
                _logInfoItemUIs.AddLast(_ui);

                // 区分色块
                SetLogUIFills(_ui, _logInfoItemUIs.Count);

                ShowLogItem(_ui, _logShowType, info.logType);

                AddLogShowTypeNum(_ui.logInfo.logType, 1);

                // 检查是否超过最大数量
                while (_logInfoItemUIs.Count > _logInfoItemUIShowMaxNum && _logInfoItemUIs.Count > 0)
                {
                    RemoveLogUI();// 移除最老的
                }
            }
        }
        // 移除
        //public void RemoveLogUI(int index)
        //{
        //    var _ui = _logInfoItemUIs[index];
        //    _logInfoItemUIs.RemoveAt(index);
        //    if (_ui != null)
        //    {
        //        ReturnPool(_ui);
        //    }
        //}
        public void RemoveLogUI()
        {
            var _ui = _logInfoItemUIs.First.Value;
            _logInfoItemUIs.RemoveFirst();
            if (_ui != null)
            {
                ReturnPool(_ui);
                AddLogShowTypeNum(_ui.logInfo.logType, -1);
            }
        }

        // 选中
        public void SelectLogItemUI(LogInfoItemUI _ui)
        {
            if (_ui == _selectLogInfoItemUI)
            {
                return;
            }
            if (_ui == null)
            {
                if (!_selectLogInfoItemUI)
                {
                    _selectLogInfoItemUI.isSelected = false;
                }
            }
            else
            {
                foreach (var item in _logInfoItemUIs)
                {
                    item.isSelected = false;
                }
                _ui.isSelected = true;

                // 显示 log 详情
                ShowLogInfoText(_ui.logInfo);
            }

            _selectLogInfoItemUI = _ui;
        }

        // 显示某一条日志信息文本
        public void ShowLogInfoText(LogInfo info)
        {
            if (info == null)
            {
                ExtendUtility.SetText(_conditionText, null);
                ExtendUtility.SetText(_stackTraceText, null);
            }
            else
            {
                ExtendUtility.SetText(_conditionText, info.condition);
                ExtendUtility.SetText(_stackTraceText, info.stackTrace);
            }
        }

        // 显示不同类型的日志
        public void ShowLogType()
        {
            ShowLogType(_logShowType);
        }
        // 显示不同类型的日志
        public void ShowLogType(LogShowType type)
        {
            foreach (var item in _logInfoItemUIs)
            {
                // 显示或隐藏 普通 日志信息
                ShowLogItem(item, type, LogType.Log);
                // 显示或隐藏 警告 日志信息
                ShowLogItem(item, type, LogType.Warning);
                // 显示或隐藏 错误 日志信息
                ShowLogItem(item, type, LogType.Error);
                // 显示或隐藏 异常 日志信息
                ShowLogItem(item, type, LogType.Exception);
                // 显示或隐藏 断言 日志信息
                ShowLogItem(item, type, LogType.Assert);
            }
        }
        // 控制指定日志项类型的显示或隐藏
        private void ShowLogItem(LogInfoItemUI item, LogShowType baseLogShowType, LogType logType)
        {
            if (item.logInfo.logType == logType)
            {
                var targetLogShowType = ToLogShowType(logType);
                bool show = baseLogShowType.HasFlag(targetLogShowType);
                if (!item.gameObject.activeSelf && show)
                    item.gameObject.SetActive(show);
                else
                if (item.gameObject.activeSelf && !show)
                    item.gameObject.SetActive(show);
            }
        }
        private LogShowType ToLogShowType(LogType type)
        {
            switch (type)
            {
                case LogType.Log:
                    return LogShowType.Info;
                case LogType.Warning:
                    return LogShowType.Warning;
                case LogType.Assert:// 断言算作错误
                case LogType.Error:
                    return LogShowType.Error;
                case LogType.Exception:
                    return LogShowType.Exception;
                default:
                    return LogShowType.All;
            }
        }
        //private LogType ToLogType(LogShowType showType)
        //{
        //    switch (showType)
        //    {
        //        case LogShowType.Info:
        //            return LogType.Log;
        //        case LogShowType.Warning:
        //            return LogType.Warning;
        //        case LogShowType.Error:
        //            return LogType.Error;
        //        case LogShowType.Exception:
        //            return LogType.Exception;
        //        case LogShowType.None: 
        //        case LogShowType.All:
        //        default:
        //            return (LogType)(-1);
        //    }
        //}

        private void AddLogShowTypeNum(LogType type, int num)
        {
            AddLogShowTypeNum(ToLogShowType(type), num);
        }
        private void AddLogShowTypeNum(LogShowType type, int num)
        {
            if (!_logShowTypeNumDic.TryGetValue(type, out var showNum))
            {
                _logShowTypeNumDic[type] = showNum = new LogShowTypeNum();
            }

            showNum.AddNum(num);
        }

        // 设置是否显示对应类型的日志
        private void SetLogShowType(bool isOn, LogShowType type, bool refresh = true)
        {
            if (isOn) _logShowType |= type;
            else _logShowType ^= type;

            if (refresh) ShowLogType();
        }

        // 返回对象池
        private void ReturnPool(LogInfoItemUI _ui)
        {
            _logInfoItemUIPool.Return(_ui.gameObject);
        }

        public void ClearLog()
        {
            foreach (var item in _logInfoItemUIs)
            {
                ReturnPool(item);
            }
            _logInfoItemUIs.Clear();

            ShowLogInfoText(null);
        }


        private class LogShowTypeNum
        {
            private Text _text;
            private int num;

            public LogShowTypeNum() { }
            public LogShowTypeNum(Text text)
            {
                this.text = text;
            }

            public Text text
            {
                get => _text;
                set
                {
                    _text = value;
                    ShowNum();
                }
            }

            public void AddNum(int num)
            {
                this.num += num;
                ShowNum();
            }

            public void ShowNum()
            {
                if (_text) _text.text = num.ToString();
            }
        }
    }

    /// <summary>
    /// 日志显示类型
    /// </summary>
    [Flags]
    public enum LogShowType : byte
    {
        None = 0,
        Info = 1,
        Warning = 1 << 1,
        Error = 1 << 2,
        Exception = 1 << 3,

        All = byte.MaxValue
    }
}