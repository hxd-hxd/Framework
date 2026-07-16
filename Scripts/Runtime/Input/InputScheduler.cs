using System.Collections;
using System.Collections.Generic;
using Framework;

namespace Framework.InputTool
{
    /// <summary>输入调度器</summary>
    public class InputScheduler : Singleton<InputScheduler>
    {
        protected List<IInputSwitch> _inputSwitches = new List<IInputSwitch>();

        /// <summary>添加输入</summary>
        public void AddInput(IInputSwitch input)
        {
            if (ObjectUtility.IsNull(input) || _inputSwitches.Contains(input))
            {
                return;
            }

            _inputSwitches.Add(input);
            if (!ObjectUtility.IsNull(input))
            {
                input.isEnableInput = true;
            }
        }

        /// <summary>移除输入</summary>
        public void RemoveInput(IInputSwitch input)
        {
            _inputSwitches.Remove(input);
            if (!ObjectUtility.IsNull(input))
            {
                input.isEnableInput = false;
            }
        }

        //protected IInputSwitch _curInput;

        public void SetInput(IInputSwitch input)
        {
            ClearInput();

            _inputSwitches.Add(input);
            EnableInput();
        }

        public void ClearInput()
        {
            DisableInput();
            _inputSwitches.Clear();
        }

        public void DisableInput()
        {
            for (int i = 0; i < _inputSwitches.Count; i++)
            {
                var item = _inputSwitches[i];
                if (!ObjectUtility.IsNull(item))
                {
                    item.isEnableInput = false;
                }
            }
        }

        public void EnableInput()
        {
            for (int i = 0; i < _inputSwitches.Count; i++)
            {
                var item = _inputSwitches[i];
                if (!ObjectUtility.IsNull(item))
                {
                    item.isEnableInput = true;
                }
            }
        }

        /// <summary>添加顶层输入，只有这个输入开启</summary>
        public void AddInputTop(IInputSwitch input)
        {
            if (ObjectUtility.IsNull(input))
            {
                return;
            }
            if (_inputSwitches.Contains(input))
            {
                _inputSwitches.Remove(input);
            }

            DisableInput();

            _inputSwitches.Add(input);
            if (!ObjectUtility.IsNull(input))
            {
                input.isEnableInput = true;
            }
        }

        /// <summary>移除顶层输入，关闭这个输入，并开启移除后处于顶层的输入</summary>
        public void RemoveInputTop(IInputSwitch input)
        {
            RemoveInput(input);

            DisableInput();

            for (int i = _inputSwitches.Count - 1; i >= 0; i--)
            {
                var cur = _inputSwitches[i];
                if (!ObjectUtility.IsNull(cur))
                {
                    cur.isEnableInput = true;
                }
            }
        }
    }
}