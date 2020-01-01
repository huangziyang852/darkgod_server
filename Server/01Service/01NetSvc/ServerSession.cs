//网络会话连接

using PENet;
using PEProtocol;

public class ServerSession : PESession<GameMsg>
{
    //每一个对话对应一个ID
    public int sessionID = 0;

    protected override void OnConnected()
    {
        sessionID = ServerRoot.Instance.GetSessionID();
        PECommon.Log("Client Connection,SessionID:"+sessionID+".");
        
    }

    protected override void OnReciveMsg(GameMsg msg)
    {
        PECommon.Log("SessionID:" + sessionID+"Recive Package CMD:" +((CMD)msg.cmd).ToString());//注意类型转换
        //将消息传到消息队列,需要附带会话的session
        NetSvc.Instance.AddMsgQue(this,msg);
        
    }

    protected override void OnDisConnected()
    {
        LoginSys.Instance.ClearOfflineData(this);
        PECommon.Log("SessionID:"+sessionID+"Client DisConnect");
    }
}

