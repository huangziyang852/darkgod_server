//登录业务模块

using PEProtocol;

class LoginSys
{
    private static LoginSys instance = null;

    public static LoginSys Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new LoginSys();
            }
            return instance;
        }
    }

    private CacheSvc cacheSvc = null;

    private TimerSvc timerSvc = null;

    public void Init()
    {
        cacheSvc = CacheSvc.Instance;
        timerSvc = TimerSvc.Instance;
        PECommon.Log("LoginSys Init Done.");

    }

    public void ReqLogin(MsgPack pack)
    {
        ReqLogin data = pack.msg.reqLogin;
        GameMsg msg = new GameMsg
        {
            cmd = (int)CMD.RspLogin,
            
        };
        //判断当前帐号是否上线
        if (cacheSvc.IsAcctOnline(data.acct))
        {
            //已上线返回一个错误信息
            msg.err = (int)ErrorCode.AcctIsOnline;
        }
        else
        {
            //未上线判断当前账号是否存在,此处检测缓存中是否存在玩家数据
            PlayerData pd = cacheSvc.GetPlayerData(data.acct, data.pass);
            if (pd == null)
            {
                //存在，检测密码
                msg.err = (int)ErrorCode.WrongPass;
            }
            else
            {
                //计算离线体力增长
                int power = pd.Power;
                long now = timerSvc.GetNowTime();
                long milliseconds = now - pd.time;
                int addpower = (int)(milliseconds / (1000 *60* PECommon.PowerAddSpace))*PECommon.PowerAddCount;
                if(addpower > 0)
                {
                    int powerMax = PECommon.GetPowerLimit(pd.lv);
                    if (pd.Power < powerMax)
                    {
                        pd.Power += addpower;
                        if(pd.Power > powerMax)
                        {
                            pd.Power = powerMax;
                        }
                    }
                }
                if(power != pd.Power)
                {
                    cacheSvc.UpdataPlayerData(pd.id, pd);
                }

                //返回玩家数据
                msg.rspLogin = new RspLogin
                {
                    playerData = pd
                };

                //缓存玩家信息
                cacheSvc.AcctOnline(data.acct, pack.session,pd);
            }
        }
        //回应客户端
        pack.session.SendMsg(msg);
    }

    public void ReqRename(MsgPack pack)
    {
        ReqRename data = pack.msg.reqRename;
        GameMsg msg = new GameMsg
        {
            cmd = (int)CMD.RspRename   //回应改名请求
        };

        if (cacheSvc.IsNameExist(data.name))
        {
            //名字是否已存在
            //存在：返回错误码
            msg.err = (int)ErrorCode.NameIsExist;
        }
        else
        {
            //不存在：更新缓存以及数据库再返回给客户端,一开始玩家姓名为空所以要更新缓存
            PlayerData playerData = cacheSvc.GetPlayerDataBySession(pack.session);
            playerData.name = data.name;

            if (!cacheSvc.UpdataPlayerData(playerData.id, playerData))
            {
                msg.err = (int)ErrorCode.UpdateDBError;
            }
            else
            {
                //响应请求并返回
                msg.rspRename = new RspRename
                {
                    name = data.name
                };
            }
        }
        pack.session.SendMsg(msg);
    }

    public void ClearOfflineData(ServerSession session)
    {
        //写入下线时间
        PlayerData pd = cacheSvc.GetPlayerDataBySession(session);
        if (pd != null)
        {
            pd.time = timerSvc.GetNowTime();
            if (!cacheSvc.UpdataPlayerData(pd.id, pd))
            {
                PECommon.Log("Update offline time error",LogType.Error);
            }
        }
        cacheSvc.AcctOffLine(session);
    }
}
