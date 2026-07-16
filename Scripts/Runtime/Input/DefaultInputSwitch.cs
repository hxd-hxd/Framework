namespace Framework.InputTool
{
    /// <summary>默认输入开关</summary>
    public class DefaultInputSwitch : IInputSwitch
    {
        /// <summary>一个空开关，可用于占位</summary>
        public static DefaultInputSwitch empty { get; } = new DefaultInputSwitch();

        /// <summary>是否启用输入</summary>
        public bool isEnableInput { get; set; }

        /// <summary>是否是一个输入</summary>
        public bool isInput { get; }

    }
}
