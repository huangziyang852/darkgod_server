//交易购买系统



using PEProtocol;

class BuySys
{
    private static BuySys instance = null;
    public static BuySys Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new BuySys();
            }
            return instance;
        }
    }
    private CacheSvc cacheSvc = null;

    public void Init()
    {
        cacheSvc = CacheSvc.Instance;
        PECommon.Log("BuySys init done");
    }

    public void ReqBuy(MsgPack pack)
    {
        ReqBuy data = pack.msg.reqBuy;
        GameMsg msg = new GameMsg
        {
            cmd = (int)CMD.RspBuy
        };

        PlayerData pd = cacheSvc.GetPlayerDataBySession(pack.session);

        if(pd.diamond < data.cost)
        {
            msg.err = (int)ErrorCode.LackDiamond;
        }
        else
        {
            pd.diamond -= data.cost;
            PshTaskPrgs pshTaskPrgs = null;
            switch (data.type)
            {
                case 0:
                    pd.Power += 100;
                    //任务进度更新
                    pshTaskPrgs= TaskSys.Instance.GetTaskPrgs(pd, 4);
                    break;
                case 1:
                    pd.coin += 1000;
                    //任务进度更新
                    pshTaskPrgs = TaskSys.Instance.GetTaskPrgs(pd, 5);
                    break;
            }

            if (!cacheSvc.UpdataPlayerData(pd.id, pd))
            {
                msg.err = (int)ErrorCode.UpdateDBError;
            }
            else
            {
                RspBuy rspBuy = new RspBuy
                {
                    type = data.type,
                    diamond = pd.diamond,
                    coin = pd.coin,
                    power = pd.Power
                };
                msg.rspBuy = rspBuy;

                //并包处理
                msg.pshTaskPrgs = pshTaskPrgs;
            }   
        }
        pack.session.SendMsg(msg);
    }      

}

