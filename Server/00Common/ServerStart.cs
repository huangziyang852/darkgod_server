//服务器入口



using System.Threading;

class ServerStart
{
    static void Main(string[] args)
    {
        ServerRoot.Instance.Init();

        while (true)
        {
            ServerRoot.Instance.Update();
            //降低服务器运行的帧率
            Thread.Sleep(20);
        }
    }
}

