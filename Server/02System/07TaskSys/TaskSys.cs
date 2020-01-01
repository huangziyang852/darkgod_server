
//任务奖励系统

using PEProtocol;

class TaskSys
{
    private static TaskSys instance = null;
    public static TaskSys Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new TaskSys();
            }
            return instance;
        }
    }
    private CacheSvc cacheSvc = null;
    private CfgSvc cfgSvc = null;

    public void Init()
    {
        cacheSvc = CacheSvc.Instance;
        PECommon.Log("TaskSys Init Done");
        cfgSvc = CfgSvc.Instance;
        PECommon.Log("CfgSvc Init Done");
    }

    public void ReqTakeTaskReward(MsgPack pack)
    {
        ReqTakeTaskReward data = pack.msg.reqTakeTaskReward;

        GameMsg msg = new GameMsg
        {
            cmd = (int)CMD.RspTakeTaskReward
        };

        PlayerData pd = cacheSvc.GetPlayerDataBySession(pack.session);

        TaskRewardCfg trc = cfgSvc.GetTaskRewardCfg(data.rid);
        TaskRewardData trd = CalcTaskRewardData(pd, data.rid);
        //任务已完成，且没有领取奖励
        if(trd.prgs == trc.count && !trd.taked)
        {
            pd.coin += trc.coin;
            PECommon.CalcExp(pd,trc.exp);
            trd.taked = true;
            //更新任务进度数据
            CalcTaskArr(pd, trd);
            //更新到数据库
            if (!cacheSvc.UpdataPlayerData(pd.id, pd))
            {
                msg.err = (int)ErrorCode.UpdateDBError;
            }
            else
            {
                //回应客户端
                RspTakeTaskReward rspTakeTaskReward = new RspTakeTaskReward
                {
                    coin = pd.coin,
                    lv = pd.lv,
                    exp = pd.exp,
                    taskArr = pd.taskArr
                };
                msg.RspTakeTaskReward = rspTakeTaskReward;
            }

        }
        else
        {
            msg.err = (int)ErrorCode.ClientDataError;
        }
        pack.session.SendMsg(msg);
    }

    //转换数据
    public TaskRewardData CalcTaskRewardData(PlayerData pd,int rid)
    {
        TaskRewardData trd = null;
        for(int i = 0; i < pd.taskArr.Length; i++)
        {
            string[] taskinfo = pd.taskArr[i].Split('|');
            //1|0|0
            if (int.Parse(taskinfo[0]) == rid)
            {
                trd = new TaskRewardData
                {
                    ID = int.Parse(taskinfo[0]),
                    prgs = int.Parse(taskinfo[1]),
                    //判断是否完成
                    taked = taskinfo[2].Equals("1")
                };
                break;
            }
        }
        return trd;
    }
    //转换数据
    public void CalcTaskArr(PlayerData pd,TaskRewardData trd)
    {
        string result = trd.ID + "|" + trd.prgs + "|" + (trd.taked ? 1 : 0);
        int index = -1;
        for(int i = 0; i < pd.taskArr.Length; i++)
        {
            string[] taskinfo = pd.taskArr[i].Split('|');
            if (int.Parse(taskinfo[0]) == trd.ID)
            {
                index = i;
                break;
            }
        }

        pd.taskArr[index] = result;
    }
    //检测任务进度
    public void CalcTaskPrgs(PlayerData pd,int tid)
    {
        TaskRewardData trd = CalcTaskRewardData(pd, tid);
        TaskRewardCfg trc = cfgSvc.GetTaskRewardCfg(tid);

        if (trd.prgs < trc.count)
        {
            trd.prgs += 1;
            //更新任务进度
            CalcTaskArr(pd, trd);

            ServerSession session = cacheSvc.GetOnlineServersession(pd.id);
            if (session != null)
            {
                session.SendMsg(new GameMsg
                {
                    cmd = (int)CMD.PshTaskPrgs,
                    pshTaskPrgs = new PshTaskPrgs
                    {
                        taskArr = pd.taskArr
                    }
                });
            }
        }
    }


    //检测任务进度
    public PshTaskPrgs GetTaskPrgs(PlayerData pd, int tid)
    {
        TaskRewardData trd = CalcTaskRewardData(pd, tid);
        TaskRewardCfg trc = cfgSvc.GetTaskRewardCfg(tid);

        if (trd.prgs < trc.count)
        {
            trd.prgs += 1;
            //更新任务进度
            CalcTaskArr(pd, trd);

            return new PshTaskPrgs
            {
                taskArr = pd.taskArr
            };
        }
        else
        {
            return null;
        }
    }

}

