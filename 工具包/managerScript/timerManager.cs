using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class timerManager:MonoBehaviour{
    // Use this for initialization
    static timerManager instance;
    void Awake () {
        

    }
    public static timerManager GetInstance() {
        if (instance == null)
        {
          GameObject  go =new GameObject("timerManager");
          instance = go.AddComponent(typeof(timerManager)) as timerManager;
          DontDestroyOnLoad(go);
        }
        return instance;
    }

    public Int32 addTimer(float start, float interval,Timer.TimerCallback function, object userdata)
    {
        Timer t = new Timer(start, interval, function, userdata);
        return t.start();
    }

    public Int32 addTimer(float start, float interval, Timer.TimerCallback function)
    {
        Timer t = new Timer(start, interval, function, null);
        return t.start();
    }
    public Int32 addTimer(float start, Timer.TimerCallback function)
    {
        Timer t = new Timer(start, 0.0f, function, null);
        return t.start();
    }
	public Int32 addTimer(float start, Timer.TimerCallback function, object userdata)
	{
		Timer t = new Timer(start, 0.0f, function, userdata);
		return t.start();
	}
    public void cancelTimer(Int32 timerID)
    {
        Task.remove(timerID);
    }
    void FixedUpdate()
    {
        Task.updateAll();
    }
}
public class Timer : Task
{
	public delegate void TimerCallback(Int32 timerID, object userData);

    float _start;
    float _interval;
    TimerCallback _callback;
    object _userdata;

    float _next;

    /// <summary>
    /// 注册一个触发器
    /// </summary>
    /// <param name="start">第一次触发的时间</param>
    /// <param name="interval">每次触发的时间，如果仅想触发一次则置值小于或等于0.0f</param>
    /// <param name="function">回调函数</param>
    /// <param name="userData">用户自定义回传数据，不需要可以置为null</param>
    public Timer(float start, float interval, TimerCallback function, object userdata)
    {
        _start = start;
        _interval = interval;
        _callback = function;
        _userdata = userdata;

        _next = Time.time + _start;
    }

    /// <summary>
    /// 依赖于外部每tick执行一次的更新接口
    /// </summary>
    protected override void onUpdate()
    {
        if (_next > Time.time)
            return;

        _callback(id, _userdata);
        if (_interval > 0.0f)
        {
            // 这里不使用 d.next = t，是为了在偶尔延时下尽量保证在一定的时间内执行的回调次数是固定的
            _next += _interval;
        }
        else
        {
            stop();
        }
    }
}
public abstract class Task
{
    static Task s_begin = null;
    static Task s_last = null;
    static Int32 s_lastID = 0;
    static Dictionary<Int32, Task> s_tasks = new Dictionary<int, Task>();

    static Int32 NewID()
    {
        return ++s_lastID;
    }

    static void add(Task task)
    {
        if (s_begin == null)
        {
            s_begin = task;
            s_last = task;
            task._next = null;
            task._prev = null;
        }
        else
        {
            task._prev = s_last;
            s_last._next = task;
            task._next = null;
            s_last = task;
        }

        s_tasks.Add(task._id, task);
    }

    static void remove(Task task)
    {
        Task prev = task._prev;
        Task next = task._next;
        task._prev = null;
        task._next = null;

        if (prev == null)
        {
            if (next == null)
            {
                s_begin = null;
                s_last = null;
            }
            else
            {
                next._prev = null;
                s_begin = next;
            }
        }
        else
        {
            if (next == null)
            {
                prev._next = null;
                s_last = prev;
            }
            else
            {
                prev._next = next;
                next._prev = prev;
            }
        }

        s_tasks.Remove(task._id);
    }

    public static void remove(Int32 taskID)
    {
        if (s_tasks.ContainsKey(taskID))
            remove(s_tasks[taskID]);
    }

    public static void updateAll()
    {
        if (s_tasks.Count == 0)
            return;

        Task next = s_begin;
        while (next != null)
        {
            // 先移动到下一个任务，以应对在处理任务时存在删除任务的情况
            Task old = next;
            next = next._next;

            old.onUpdate();
        }
    }

    Task _prev;
    Task _next;
    Int32 _id;

    public Task()
    {

    }

    ~Task()
    {
        //(string.Format("Task::~Task(), task id = {0}", _id));
    }

    public Int32 id
    {
        get { return _id; }
    }

    /// <summary>
    /// 开始任务
    /// </summary>
    public Int32 start()
    {
        _id = NewID();
        add(this);
        onStart();
        return _id;
    }

    /// <summary>
    /// 结束任务
    /// </summary>
    public void stop()
    {
        remove(this);
        onStop();
    }

    /// <summary>
    /// Update is called once per frame
    /// 每一帧做一些事情
    /// </summary>
    protected virtual void onUpdate()
    {

    }

    /// <summary>
    /// 开始任务时做一些事情
    /// </summary>
    protected virtual void onStart()
    {

    }

    /// <summary>
    /// 结束任务时做一些事情
    /// </summary>
    protected virtual void onStop()
    {

    }
}

