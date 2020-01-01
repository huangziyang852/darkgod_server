//网络服务
using PENet;
using PEProtocol;
using System.Collections.Generic;

public class MsgPack
{
    public ServerSession session;
    public GameMsg msg;

    public MsgPack(ServerSession session, GameMsg msg)
    {
        //包括message和session
        this.session = session;
        this.msg = msg;
    }
}

class NetSvc
{
    private static NetSvc instance = null;

    public static NetSvc Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new NetSvc();
            }
            return instance;
        }
    }
    //线程锁
    public static readonly string obj = "lock";
    private Queue<MsgPack> msgPackQue = new Queue<MsgPack>();

    public void Init()
    {
        PESocket<ServerSession, GameMsg> server = new PESocket<ServerSession, GameMsg>();
        server.StartAsServer(SrvCfg.srvIP, SrvCfg.srvPort);

        PECommon.Log("NetSvc Init Done.");
    }
    //PESocket为异步多线程，需要注意线程安全
    //一个服务器对应多个客户端，所以需要消息队列
    public void AddMsgQue(ServerSession session,GameMsg msg)
    {
        lock (obj)
        {
            msgPackQue.Enqueue(new MsgPack(session,msg));
        }
    }

    public void Update()
    {
        //检测是否有包,取出数据
        if (msgPackQue.Count > 0)
        {
            PECommon.Log("PackCount:" + msgPackQue.Count);
            lock (obj)
            {
                MsgPack pack = msgPackQue.Dequeue();
                HandOut(pack);
            }
        }
    }
    //分发接收到的请求数据
    public void HandOut(MsgPack pack)
    {
        switch ((CMD)pack.msg.cmd)
        {
            case CMD.ReqLogin:
                LoginSys.Instance.ReqLogin(pack);
                break;
            case CMD.ReqRename:
                LoginSys.Instance.ReqRename(pack);
                break;
            case CMD.ReqGuide:
                GuideSys.Instance.ReqGuide(pack);
                break;
            case CMD.ReqStrong:
                StrongSys.Instance.ReqStrong(pack);
                break;
            case CMD.SndChat:
                ChatSys.Instance.SndChat(pack);
                break;
            case CMD.ReqBuy:
                BuySys.Instance.ReqBuy(pack);
                break;
            case CMD.ReqTakeTaskReward:
                TaskSys.Instance.ReqTakeTaskReward(pack);
                break;
            case CMD.ReqFBFight:
                FubenSys.Instance.ReqFBFight(pack);
                break;
            case CMD.ReqFBFightEnd:
                FubenSys.Instance.ReqFBFightEnd(pack);
                break;
        }
    }


}

