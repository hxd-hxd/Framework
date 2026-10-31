using Framework.Event;

namespace Framework.Save
{
    /// <summary>事件 - 保存之前</summary>
    public class EventSaveBefore : EventBase<EventSaveBefore>
    {
        
    }

    /// <summary>事件 - 保存之后</summary>
    public class EventSaveAfter : EventBase<EventSaveAfter>
    {
        
    }

    /// <summary>事件 - 自动保存</summary>
    public class EventAutoSave : EventBase<EventAutoSave>
    {

    }
}
