using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.LogSystem
{
    /// <summary>
    /// 日志浮标 ui 
    /// </summary>
    public class LogBuoyUI : UIBase
    {
        [SerializeField]
        private Button _logBtn;
        [SerializeField]
        private Text _fpsText;
        [SerializeField]
        private LogSystemUI _logSystemUI;

        /// <summary>
        /// 点击 log 按钮时触发此事件
        /// </summary>
        public Action logClickEvent;

        protected override void Start()
        {
            base.Start();

            _logBtn = transform.FindOf<Button>("LogBtn");
            _fpsText = _logBtn.transform.FindOf<Text>("Text");

            if (!_logSystemUI)
                _logSystemUI = UIManager.ExpectGetUI<LogSystemUI>();

            AddEvent(_logBtn, OnLogClick);
        }

        float tiemer = 0;
        //StringBuilder _fpsSB = new StringBuilder();
        protected virtual void Update()
        {
            if (tiemer < 0.2f)
            {
                tiemer += Time.deltaTime;
                return;
            }
            tiemer = 0;

            var _fpsSB = TypePool.root.Get<StringBuilder>();
            //_fpsSB.Clear();
            _fpsSB.Append("打开日志");
            if (_logSystemUI && _logSystemUI.fpsCounter != null)
            {
                _fpsSB.Clear();
                //fpsT = $"FPS：{_logSystemUI.fpsCounter.CurrentFps:F}";
                // 颜色
                Color color = default;
                if (LogInfo.ErrorCount > 0
                    || LogInfo.ExceptionCount > 0
                    || LogInfo.AssertCount > 0
                    )
                {
                    color = LogInfo.GetLogColor(LogType.Error);
                }
                else if (LogInfo.WarningCount > 0)
                {
                    color = LogInfo.GetLogColor(LogType.Warning);
                }
                else
                {
                    color = LogInfo.GetLogColor(LogType.Log);
                }
                //fpsT = $"<color=#{ColorUtility.ToHtmlStringRGBA(color)}>{fpsT}</color>";
                _fpsSB.AppendFormat("<color=#{0}>", ColorUtility.ToHtmlStringRGBA(color));
                _fpsSB.AppendFormat("FPS：{0}", _logSystemUI.fpsCounter.CurrentFps.ToString("f1"));
                _fpsSB.Append("</color>");
            }
            ExtendUtility.SetText(_fpsText, _fpsSB.ToString());
            TypePool.root.Return(_fpsSB);
        }

        protected virtual void OnLogClick()
        {
            Enable(false);
            logClickEvent?.Invoke();
        }
    }

}