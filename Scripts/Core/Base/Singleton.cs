
namespace Framework
{
    /// <summary>
    /// 单例基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Singleton<T> where T : new()
    {
        private static readonly object _lock = new object();

        static T inst;
        public static T Instance
        {
            get
            {
                if (inst == null)
                    lock (_lock)
                        if (inst == null)
                        {
                            inst = new T();
                        }
                return inst;
            }
            protected set { inst = value; }
        }

        public static T Ins => Instance;
    }
}