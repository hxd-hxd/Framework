
namespace Framework
{
    /// <summary>
    /// µ¥Àý»ùÀà
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Singleton<T> where T : new()
    {
        static T inst;
        public static T Instance
        {
            get
            {
                if (inst == null)
                    lock (inst)
                        if (inst == null)
                        {
                            inst = new T();
                        }
                return inst;
            }
            protected set { inst = value; }
        }

    }
}