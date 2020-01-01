//客户端服务器公用类

using PENet;
using PEProtocol;

public enum LogType
{
    Log =0,
    Warn =1,
    Error =2,
    Info =3
}

public class PECommon
{
    public static void Log(string msg ="",LogType tp = LogType.Log)
    {
        LogLevel lv = (LogLevel)tp;
        PETool.LogMsg(msg, lv);
    }

    public static int GetFightByProps(PlayerData pd)
    {
        return pd.lv * 100 + pd.ad + pd.ap + pd.addef + pd.apdef;
    }

    public static int GetPowerLimit(int lv)
    {
        return ((lv - 1) / 10 )* 150 + 150;
    }

    public static int GetExpUpValByLv(int lv)
    {
        return 100 * lv * lv;
    }
    //体力增加的间隙：单位分钟
    public const int PowerAddSpace = 5;
    //体力增加的数值
    public const int PowerAddCount = 2;

    //升级计算
    public static void CalcExp(PlayerData pd, int addExp)
    {
        int curtLv = pd.lv;
        int curtExp = pd.exp;
        int addRestExp = addExp;
        while (true)
        {
            int upNeedExp = PECommon.GetExpUpValByLv(curtLv) - curtExp;
            if (addRestExp >= upNeedExp)
            {
                //2.足够升一次级
                curtLv += 1;
                curtExp = 0;
                addRestExp -= upNeedExp;
            }
            else
            {
                //1.经验不够升级
                pd.lv = curtLv;
                pd.exp = curtExp + addRestExp;
                break;
            }
        }
    }
}

