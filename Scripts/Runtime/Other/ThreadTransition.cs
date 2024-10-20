using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 线程过渡
/// <para>ps：用于解决在 unity 中，其他线程无法调用主线程的问题</para>
/// </summary>
public class ThreadTransition : MonoBehaviour
{
    //是否已经初始化
    static bool isInitialized;
    private static ThreadTransition _ins;

    public static ThreadTransition Instance
    {
        get
        {
            Initialize();
            return _ins;
        }
    }

    //初始化
    public static void Initialize()
    {
        if (!isInitialized)
        {
            if (!Application.isPlaying)
                return;

            isInitialized = true;
            _ins = new GameObject("[Thread Transition]").AddComponent<ThreadTransition>();

            DontDestroyOnLoad(_ins.gameObject);
        }
    }

    void Awake()
    {
        _ins = this;
        isInitialized = true;
    }

    List<Action> listAction = new List<Action>();
    //全部执行列表（有延迟）
    List<DelayedQueueItem> listDelayedActions = new List<DelayedQueueItem>();
    //全部执行列表（无延迟）
    List<NoDelayedQueueItem> listNoDelayActions = new List<NoDelayedQueueItem>();

    List<Action> currentActions = new List<Action>();
    //当前执行的有延时函数链
    List<DelayedQueueItem> currentDelayed = new List<DelayedQueueItem>();
    //当前执行的无延时函数链
    List<NoDelayedQueueItem> currentNoDelayed = new List<NoDelayedQueueItem>();

    /// <summary>
    /// 加入到主线程执行队列（无延迟）
    /// </summary>
    /// <param name="taction"></param>
    public static void QueueOnMainThread(Action taction)
    {
        lock (Instance)
        {
            if (!Instance)
            {
                Debug.LogError("线程过渡组件 未初始化");
                return;
            }

            if (!Instance.gameObject.activeInHierarchy || !Instance.isActiveAndEnabled) return;
        }

        lock (Instance.listAction)
        {
            Instance.listAction.Add(taction);
        }
    }

    /// <summary>加入到主线程执行队列（无延迟）</summary>
    public static void QueueOnMainThread(Action<object> taction, object param)
    {
        QueueOnMainThread(taction, param, 0f);
    }

    /// <summary>加入到主线程执行队列（有延迟）</summary>
    public static void QueueOnMainThread(Action<object> action, object param, float time)
    {
        lock (Instance)
        {
            if (!Instance)
            {
                Debug.LogError("线程过渡组件 未初始化");
                return;
            }

            if (!Instance.gameObject.activeInHierarchy || !Instance.isActiveAndEnabled) return;
        }

        if (time != 0)
        {
            lock (Instance.listDelayedActions)
            {
                Instance.listDelayedActions.Add(new DelayedQueueItem
                { time = Time.time + time, action = action, param = param });
            }
        }
        else
        {
            lock (Instance.listNoDelayActions)
            {
                Instance.listNoDelayActions.Add(new NoDelayedQueueItem { action = action, param = param });
            }
        }
    }

    void Update()
    {
        if (listAction.Count > 0)
        {
            lock (listAction)
            {
                currentActions.Clear();
                currentActions.AddRange(listAction);
                listAction.Clear();
            }

            for (int i = 0; i < currentActions.Count; i++)
            {
                currentActions[i]?.Invoke();
            }
        }

        if (listNoDelayActions.Count > 0)
        {
            lock (listNoDelayActions)
            {
                currentNoDelayed.Clear();
                currentNoDelayed.AddRange(listNoDelayActions);
                listNoDelayActions.Clear();
            }

            for (int i = 0; i < currentNoDelayed.Count; i++)
            {
                currentNoDelayed[i].action(currentNoDelayed[i].param);
            }
        }

        if (listDelayedActions.Count > 0)
        {
            lock (listDelayedActions)
            {
                currentDelayed.Clear();
                currentDelayed.AddRange(listDelayedActions.Where(d => Time.time >= d.time));
                for (int i = 0; i < currentDelayed.Count; i++)
                {
                    listDelayedActions.Remove(currentDelayed[i]);
                }
            }

            for (int i = 0; i < currentDelayed.Count; i++)
            {
                currentDelayed[i].action(currentDelayed[i].param);
            }
        }
    }

    //单个执行单元（无延迟）
    struct NoDelayedQueueItem
    {
        public Action<object> action;
        public object param;
    }

    //单个执行单元（有延迟）
    struct DelayedQueueItem
    {
        public Action<object> action;
        public object param;
        public float time;
    }

}