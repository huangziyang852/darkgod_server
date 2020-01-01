//时间服务类


using System;
using System.Collections.Generic;

class TimerSvc
{
    class TaskPack
    {
        public int tid;
        public Action<int> cb;
        public TaskPack(int tid, Action<int> cb)
        {
            this.tid = tid;
            this.cb = cb;

        }
    }

    private static TimerSvc instance = null;

    public static TimerSvc Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new TimerSvc();
            }
            return instance;
        }
    }
    PETimer pt = null;
    //任务包队列
    Queue<TaskPack> tpQue = new Queue<TaskPack>();
    private static readonly string tpQuelock = "tpQueLock";

    public void Init()
    {
        //100毫秒调用一次
        pt = new PETimer(100);
        tpQue.Clear();
        //日志输出
        pt.SetLog((string info) =>
        {
            PECommon.Log(info);
        });

        pt.SetHandle((Action<int> cb,int tid)=>
        {
            if(cb!= null)
            {
                lock (tpQuelock)
                {
                    tpQue.Enqueue(new TaskPack(tid, cb));
                }
            }
        });
        PECommon.Log("Init TimerSvc...");
    }


    public void Update()
    {
        if(tpQue.Count > 0)
        {
            TaskPack tp = null;
            lock (tpQuelock)
            {
                tp = tpQue.Dequeue();
            }

            if(tp != null)
            {
                tp.cb(tp.tid);
            }
        }
    }

    public int AddTimeTask(Action<int> callback, double delay, PETimeUnit timeUnit = PETimeUnit.Millisecond, int count = 1)
    {
        return pt.AddTimeTask(callback, delay, timeUnit, count);
    }

    public long GetNowTime()
    {
        return (long)pt.GetMillisecondsTime();
    }
}

