//聊天业务系统

using PEProtocol;
using System.Collections.Generic;

class ChatSys
{
    private static ChatSys instance = null;
    public static ChatSys Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new ChatSys();
            }
            return instance;
        }
    }
    private CacheSvc cacheSvc = null;

    public void Init()
    {
        cacheSvc = CacheSvc.Instance;
        PECommon.Log("ChatSys Init Done");
    }

    public void SndChat(MsgPack pack)
    {
        SndChat data = pack.msg.sndChat;
        PlayerData pd = cacheSvc.GetPlayerDataBySession(pack.session);
        //任务进度更新
        TaskSys.Instance.CalcTaskPrgs(pd,6);

        GameMsg msg = new GameMsg
        {
            cmd = (int)CMD.PshChat,
            pshChat = new PshChat
            {
                name = pd.name,
                chat = data.chat
            }
        };

        //广播所有在线客户端
        List<ServerSession> list = cacheSvc.GetOnlineServerSessions();
        //将所有消息序列化
        byte[] bytes = PENet.PETool.PackNetMsg(msg);
        for(int i = 0; i < list.Count; i++)
        {
            //先将多条数据序列化后再发送
            list[i].SendMsg(bytes);
        }
    }
}

