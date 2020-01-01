//强化系统


using PEProtocol;

class StrongSys
{
    private static StrongSys instance = null;
    public static StrongSys Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new StrongSys();
            }
            return instance;
        }
    }

    private CacheSvc cacheSvc = null;

    public void Init()
    {
        cacheSvc = CacheSvc.Instance;
        PECommon.Log("StrongSys Init Done");
    }

    public void ReqStrong(MsgPack pack)
    {
        ReqStrong data = pack.msg.reqStrong;

        GameMsg msg = new GameMsg
        {
            cmd = (int)CMD.RspStrong
        };

         PlayerData pd= cacheSvc.GetPlayerDataBySession(pack.session);
        int curtStartLv = pd.strongArr[data.pos];
        //获取下一个级别的数据
        StrongCfg nextSd = CfgSvc.Instance.GetStrongCfg(data.pos, curtStartLv + 1);
        //服务端需要二次校验
        //条件判断
        if (pd.lv < nextSd.minlv)
        {
            msg.err = (int)ErrorCode.LackLevel;
        }
        if (pd.coin<nextSd.coin)
        {
            msg.err = (int)ErrorCode.LackCoin;
        }
        if (pd.crystal < nextSd.crystal)
        {
            msg.err = (int)ErrorCode.LackCrystal;
        }
        else
        {
            //更新任务
            TaskSys.Instance.CalcTaskPrgs(pd, 3);
            //资源扣除
            pd.coin -= nextSd.coin;
            pd.crystal -= nextSd.crystal;

            pd.strongArr[data.pos] += 1;

            //增加属性
            pd.hp += nextSd.addhp;
            pd.ad += nextSd.addhurt;
            pd.ap += nextSd.addhurt;
            pd.addef += nextSd.adddef;
            pd.apdef += nextSd.adddef;
        }
        //更新数据库
        if (!cacheSvc.UpdataPlayerData(pd.id, pd))
        {
            msg.err = (int)ErrorCode.UpdateDBError;
        }
        else
        {
            //返回数据
            msg.rspStrong = new RspStrong
            {
                coin = pd.coin,
                crystal = pd.crystal,
                hp = pd.hp,
                ad = pd.ad,
                ap = pd.ap,
                addef = pd.addef,
                apdef = pd.apdef,
                strongArr = pd.strongArr
            };
        }
        //回应客户端
        pack.session.SendMsg(msg);

    }
}

