using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.LogSystem
{
    /// <summary>
    /// 日志信息 ui 项
    /// </summary>
    public class LogInfoItemUI : MonoBehaviour, ITypePoolObject
    {
        [SerializeField]
        private Image _bg, _selectUI;
        [SerializeField]
        private Text _textUI;
        [SerializeField]
        private Button _btnUI;

        [SerializeField]
        private LogNumUI _logNumUI;

        [SerializeField]
        private LogInfo _logInfo;

        /// <summary>
        /// 点击事件
        /// </summary>
        public Action clickEvent;

        /// <summary>
        /// 是否打开背景
        /// </summary>
        public bool isOpenBG
        {
            get { return ExtendUtility.GetActive(_bg); }
            set { ExtendUtility.SetActive(_bg, value); }
        }
        /// <summary>
        /// 选中状态
        /// </summary>
        public bool isSelected
        {
            get { return ExtendUtility.GetActive(_selectUI); }
            set { ExtendUtility.SetActive(_selectUI, value); }
        }
        /// <summary>
        /// 文本
        /// </summary>
        public string text
        {
            get { return ExtendUtility.GetText(_textUI); }
            set { ExtendUtility.SetText(_textUI, value); }
        }

        public LogNumUI logNumUI => _logNumUI;

        /// <summary>
        /// 数量
        /// </summary>
        public int num
        {
            get { return _logNumUI.num; }
            set
            {
                _logNumUI.num = value;
            }
        }

        /// <summary>
        /// 数量文本
        /// </summary>
        public string numText
        {
            get { return _logNumUI.numText; }
            set { _logNumUI.numText = value; }
        }

        /// <summary>
        /// 数量文本
        /// </summary>
        public bool showNum
        {
            get { return _logNumUI.show; }
            set { _logNumUI.show = value; }
        }

        public LogInfo logInfo
        {
            get { return _logInfo; }
            set { SetLogInfo(_logInfo); }
        }

        void Start()
        {
            if (_btnUI)
            {
                _btnUI.onClick.AddListener(() =>
                {
                    clickEvent?.Invoke();
                });
            }
        }

        public void SetLogInfo(LogInfo info)
        {
            _logInfo = info;
            if (info != null)
            {
                text = info.ToConditionText();
                SetTextColor();

                var sb = TypePool.root.Get<StringBuilder>();
                // 时间
                sb.Append("[").Append(info.time.ToHourBehindTimeText()).Append("]");
                // 添加类型标识
                sb.Append("[").Append(info.logType).Append("]");
                name = sb.ToString();
                TypePool.root.Return(sb);
            }
        }

        public void SetTextColor()
        {
            if (_textUI && _logInfo != null)
            {
                _textUI.color = _logInfo.logColor;
            }
        }

        /// <summary>检查指定日志信息是否可堆叠到本项</summary>
        public bool CanStack(LogInfo info)
        {
            if (info == null) return false;

            bool canStack = false;
            if (_logInfo != info)
            {
                // 除时间外，其他内容完全一样
                canStack = _logInfo.condition == info.condition
                    && _logInfo.stackTrace == info.stackTrace
                    && _logInfo.logType == info.logType;
            }
            return canStack;
        }

        /// <summary>尝试将指定日志堆叠到本项</summary>
        public bool TryStack(LogInfo info)
        {
            bool canStack = CanStack(info);
            if (canStack)
            {
                SetLogInfo(info);
                // 将其数量加1
                num++;
            }
            return canStack;
        }

        void ITypePoolObject.Clear()
        {
            isOpenBG = false;
            isSelected = false;
            text = null;
            clickEvent = null;
            _logNumUI?.Clear();
            SetLogInfo(null);
        }

        [Serializable]
        public class LogNumUI
        {
            public GameObject _root;
            public Text _numText;
            public int _num;

            public int num
            {
                get
                {
                    return _num;
                }
                set
                {
                    _num = value;
                    numText = _num.ToString();
                }
            }

            public string numText
            {
                get
                {
                    if (_numText) return _numText.text;
                    return null;
                }
                set
                {
                    if (_numText) _numText.text = value;
                }
            }

            public bool show
            {
                get { return _root && _root.activeSelf; }
                set { if (_root) _root.SetActive(value); }
            }

            public void Clear()
            {
                _num = 0;
                numText = null;
                show = false;
            }
        }
    }
}