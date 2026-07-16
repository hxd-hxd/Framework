namespace Framework.InputTool
{
    /// <summary>输入开关</summary>
    public interface IInputSwitch
    {
        /// <summary>是否启用输入</summary>
        bool isEnableInput { get; set; }

        /// <summary>是否是一个输入</summary>
        bool isInput { get; }

    }
}
