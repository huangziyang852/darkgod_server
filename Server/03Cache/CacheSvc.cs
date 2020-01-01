//缓存服务层



using PEProtocol;
using System.Collections.Generic;

class CacheSvc
{
    private static CacheSvc instance = null;

    public static CacheSvc Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new CacheSvc();
            }
            return instance;
        }
    }
    private DBMgr dbMgr;

    //缓存上线玩家数据
    private Dictionary<string, ServerSession> onLineAcctDic = new Dictionary<string, ServerSession>();
    private Dictionary<ServerSession, PlayerData> OnLineSessionDic = new Dictionary<ServerSession, PlayerData>();

    public void Init()
    {
        dbMgr = DBMgr.Instance;
        PECommon.Log("CacheSvc Init Done");
    }
    //检测玩家是否上线
    public bool IsAcctOnline(string acct)
    {
        return onLineAcctDic.ContainsKey(acct);
    }

    //根据账号密码获取玩家数据,密码错误返回null，帐号不存在创建新的账号
    public PlayerData GetPlayerData(string acct,string pass)
    {
        //创建账号
        
        return dbMgr.QueryPlayerData(acct,pass);
    }
    //账号上线，缓存玩家数据()
    public void AcctOnline(string acct,ServerSession session,PlayerData playerData)
    {
        onLineAcctDic.Add(acct, session);
        OnLineSessionDic.Add(session, playerData);
    }
    //判断名字是否存在
    public bool IsNameExist(string name)
    {
        return dbMgr.QueryNameData(name);
    }
    //获取所有在线的客户端
    public List<ServerSession> GetOnlineServerSessions()
    {
        List<ServerSession> list = new List<ServerSession>();
        foreach(var item in OnLineSessionDic)
        {
            list.Add(item.Key);
        }
        return list;
    }

    //更新缓存
    public PlayerData GetPlayerDataBySession(ServerSession session)
    {
        if(OnLineSessionDic.TryGetValue(session,out PlayerData playerData))
        {
            return playerData;
        }
        else
        {
            return null;
        }
    }

    //返回所有在线玩家的数据
    public Dictionary<ServerSession,PlayerData> GetOnlineCache()
    {
        return OnLineSessionDic;
    }

    //根据id号获取连接
    public ServerSession GetOnlineServersession(int ID)
    {
        ServerSession session = null;
        foreach(var item in OnLineSessionDic)
        {
            if(item.Value.id == ID)
            {
                session = item.Key;
                break;
            }
        }
        return session;
    }

    //更新数据库
    public bool UpdataPlayerData(int id,PlayerData playerData)
    {
        return dbMgr.UpdatePlayerData(id, playerData);
    }

    //清除下线玩家数据
    public void AcctOffLine(ServerSession session)
    {
        foreach(var item in onLineAcctDic)
        {
            if(item.Value == session)
            {
                onLineAcctDic.Remove(item.Key);
                break;
            }
        }

        bool succ = OnLineSessionDic.Remove(session);
        PECommon.Log("Offline Result:SessionID:" + session.sessionID+" "+succ);
    }
}

