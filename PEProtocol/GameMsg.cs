//网络通信协议
//req为请求 rsp为响应

using System;
using PENet;

namespace PEProtocol
{
   [Serializable]
    public class GameMsg:PEMsg
    {
        //PEMsg里必须有cmd变量标识请求类型

        public ReqLogin reqLogin;
        public RspLogin rspLogin;
        public ReqRename reqRename;
        public RspRename rspRename;

        public ReqGuide reqGuide;
        public RspGuide rspGuide;

        public ReqStrong reqStrong;
        public RspStrong rspStrong;

        public SndChat sndChat;
        public PshChat pshChat;

        public ReqBuy reqBuy;
        public RspBuy rspBuy;

        public PshPower pshPower;

        public ReqTakeTaskReward reqTakeTaskReward;
        public RspTakeTaskReward RspTakeTaskReward;

        public PshTaskPrgs pshTaskPrgs;

        public ReqFBFight reqFBFight;
        public RspFBFight rspFBFight;

        public ReqFBFightEnd reqFBFightEnd;
        public RspFBFightEnd rspFBFightEnd;
    }
    #region  登陆相关
    //登录请求
    [Serializable]
    public class ReqLogin
    {
        public string acct;
        public string pass;
    }
    //登陆回应
    [Serializable]
    public class RspLogin
    {
        public PlayerData playerData;
    }
    //此处定义玩家数据，！！！！需要返回客户端
    [Serializable]
    public class PlayerData
    {
        public int id;
        public string name;
        public int lv;
        public int exp;
        public int Power;
        public int coin;
        public int diamond;
        public int crystal;

        public int hp;
        public int ad;
        public int ap;
        public int addef;
        public int apdef;
        public int dodge;   //闪避
        public int pierce;   //穿透
        public int critical;  //暴击

        public int guideid;  //引导任务ID
        public int[] strongArr; //强化

        public long time;  //上次离线时间
        public string[] taskArr;
        public int fuben;  //副本进度
        //TODO

    }

    [Serializable]
    public class ReqRename
    {
        public string name;
    }
    [Serializable]
    public class RspRename
    {
        public string name;
    }
    #endregion

    #region 引导相关
    [Serializable]
    public class ReqGuide
    {
        public int guideid;
    }
    //关键性数据不能让客户端更改，只能发送，金币经验值的配置都在服务器端完成
    [Serializable]
    public class RspGuide
    {
        public int guideid;
        public int coin;
        public int lv;
        public int exp;

    }
    #endregion

    #region 强化相关
    [Serializable]
    public class ReqStrong
    {
        public int pos;
    }
    [Serializable]
    public class RspStrong
    {
        public int coin;
        public int crystal;
        public int hp;
        public int ad;
        public int ap;
        public int addef;
        public int apdef;
        public int[] strongArr;  //强化数据的更新
    }
    #endregion

    #region 聊天相关
    [Serializable]
    public class SndChat
    {
        public string chat;
    }
    [Serializable]
    public class PshChat
    {
        public string name;
        public string chat;
    }
    #endregion

    #region 资源交易相关
    [Serializable]
    public class ReqBuy
    {
        public int type;
        public int cost;
    }
    [Serializable]
    public class RspBuy
    {
        public int type;
        public int diamond;
        public int coin;
        public int power;
    }
    #endregion

    #region 副本战斗相关
    [Serializable]
    public class ReqFBFight
    {
        public int fbid;
    }
    [Serializable]
    public class RspFBFight
    {
        public int fbid;
        public int power;
    }

    [Serializable]
    public class ReqFBFightEnd
    {
        public bool win;
        public int fbid;
        public int resthp;
        public int costtime;
    }

    [Serializable]
    public class RspFBFightEnd
    {
        public bool win;
        public int fbid;
        public int resthp;
        public int costtime;
        //副本奖励
        public int coin;
        public int lv;
        public int exp;
        public int crystal;
        public int fuben;
    }
    #endregion

    #region 体力增长
    [Serializable]
    public class PshPower
    {
        public int power;
    }
    #endregion

    #region 任务奖励
    [Serializable]
    public class ReqTakeTaskReward
    {
        public int rid;
    }
    [Serializable]
    public class RspTakeTaskReward
    {
        //会产生变化的数据
        public int coin;
        public int lv;
        public int exp;
        public string[] taskArr;
    }
    [Serializable]
    public class PshTaskPrgs
    {
        public string[] taskArr;
    }
    #endregion
    //定义错误码
    public enum ErrorCode
    {
        None =0,   //没错误
        ServerDataError, //服务器数据异常
        UpdateDBError, //更新数据库出错
        ClientDataError,//客户端数据异常

        AcctIsOnline,   //账号已上线
        WrongPass,    //密码错误
        NameIsExist,  //名字已存在

        LackLevel, //等级不够
        LackCoin,  //缺少金币
        LackCrystal, //缺少水晶
        LackDiamond,//缺少钻石
        LackPower,//体力不足

    }

    //定义消息类型
    public enum CMD
    {
        None =0,
        //登陆相关
        ReqLogin = 101,
        RspLogin = 102,
        ReqRename =103,
        RspRename =104,


        //主城相关
        ReqGuide = 201,
        RspGuide = 202,

        //强化相关
        ReqStrong = 203,
        RspStrong = 204,

        //聊天相关
        SndChat = 205,
        PshChat = 206,

        //资源交易
        ReqBuy = 207,
        RspBuy = 208,

        //体力增长
        PshPower = 209,

        //任务奖励
        ReqTakeTaskReward=210,
        RspTakeTaskReward=211,
        //更新任务进度
        PshTaskPrgs = 212,
        //副本
        ReqFBFight = 301,
        RspFBFight = 302,

        ReqFBFightEnd = 303,
        RspFBFightEnd = 304,
    }

    public class SrvCfg
    {
        //这里设置服务器的IP和端口
        //服务器部署配置
        //public const string srvIP = "172.16.120.12";
        //客户端部署配置
        //public const string srvIP = "114.55.36.58";
        //开发配置
        public const string srvIP = "127.0.0.1";
        public const int srvPort = 17666;
    }
}
