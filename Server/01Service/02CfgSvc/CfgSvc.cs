
//配置数据服务
using System.Xml;
using System.Collections.Generic;
using System;

public class CfgSvc
{
    private static CfgSvc instance = null;
    public static CfgSvc Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new CfgSvc();
            }
            return instance;
        }
    }

    public void Init()
    {
        InitGuideCfg();
        InitStrongCfg();
        InitTaskRewardCfg();
        InitMapCfg();
        PECommon.Log("CfgSvc Init Done.");
    }

    #region 自动引导配置
    private Dictionary<int, GuideCfg> guideDic = new Dictionary<int, GuideCfg>();
    private void InitGuideCfg()
    {            
        //读取配置文件
        XmlDocument doc = new XmlDocument();
        //TODO
        //开发配置
        doc.Load(@"C:\huangziyang\study\Unity\Project\DarkGod\Assets\Resources\ResCfgs\guide.xml");
        //部署配置
        //doc.Load(@"C:\ResCfgs\guide.xml");
        XmlNodeList nodeList = doc.SelectSingleNode("root").ChildNodes;

        for (int i = 0; i < nodeList.Count; i++)
        {
            XmlElement ele = nodeList[i] as XmlElement;

            if (ele.GetAttributeNode("ID") == null)
            {
                continue;
            }
            int ID = Convert.ToInt32(ele.GetAttributeNode("ID").InnerText);

            GuideCfg mc = new GuideCfg
            {
                ID = ID
            };
            foreach (XmlElement e in nodeList[i].ChildNodes)
            {
                switch (e.Name)
                {
                    case "coin":
                        mc.coin = int.Parse(e.InnerText);
                        break;
                    case "exp":
                        mc.exp = int.Parse(e.InnerText);
                        break;
                }
            }
            guideDic.Add(ID, mc);
        }
        PECommon.Log("GuideCfg Init Done.");
    }
    public GuideCfg GetGuideCfg(int id)
    {
        GuideCfg agc = null;
        if (guideDic.TryGetValue(id, out agc))
        {
            return agc;
        }
        return null;
    }
    #endregion

    #region 任务奖励配置
    private Dictionary<int, TaskRewardCfg> taskRewardDic = new Dictionary<int, TaskRewardCfg>();
    private void InitTaskRewardCfg()
    {
        //读取配置文件
        XmlDocument doc = new XmlDocument();
        //TODO
        //开发配置
        doc.Load(@"C:\huangziyang\study\Unity\Project\DarkGod\Assets\Resources\ResCfgs\taskreward.xml");
        //部署配置
        //doc.Load(@"C:\ResCfgs\taskreward.xml");
        XmlNodeList nodeList = doc.SelectSingleNode("root").ChildNodes;

        for (int i = 0; i < nodeList.Count; i++)
        {
            XmlElement ele = nodeList[i] as XmlElement;

            if (ele.GetAttributeNode("ID") == null)
            {
                continue;
            }
            int ID = Convert.ToInt32(ele.GetAttributeNode("ID").InnerText);

            TaskRewardCfg trc = new TaskRewardCfg
            {
                ID = ID
            };
            foreach (XmlElement e in nodeList[i].ChildNodes)
            {
                switch (e.Name)
                {
                    case "count":
                        trc.count = int.Parse(e.InnerText);
                        break;
                    case "exp":
                        trc.exp = int.Parse(e.InnerText);
                        break;
                    case "coin":
                        trc.coin = int.Parse(e.InnerText);
                        break;
                }
            }
            taskRewardDic.Add(ID, trc);
        }
        PECommon.Log("TaskRewardCfg Init Done.");
    }
    public TaskRewardCfg GetTaskRewardCfg(int id)
    {
        TaskRewardCfg trc = null;
        if (taskRewardDic.TryGetValue(id, out trc))
        {
            return trc;
        }
        return null;
    }
    #endregion

    #region 地图配置
    private Dictionary<int, MapCfg> mapDic = new Dictionary<int, MapCfg>();
    private void InitMapCfg()
    {
        //读取配置文件
        XmlDocument doc = new XmlDocument();
        //TODO
        //开发配置
        doc.Load(@"C:\huangziyang\study\Unity\Project\DarkGod\Assets\Resources\ResCfgs\map.xml");
        //部署配置
        //doc.Load(@"C:\ResCfgs\taskreward.xml");
        XmlNodeList nodeList = doc.SelectSingleNode("root").ChildNodes;

        for (int i = 0; i < nodeList.Count; i++)
        {
            XmlElement ele = nodeList[i] as XmlElement;

            if (ele.GetAttributeNode("ID") == null)
            {
                continue;
            }
            int ID = Convert.ToInt32(ele.GetAttributeNode("ID").InnerText);

            MapCfg mc = new MapCfg
            {
                ID = ID
            };
            foreach (XmlElement e in nodeList[i].ChildNodes)
            {
                switch (e.Name)
                {
                    case "power":
                        mc.power = int.Parse(e.InnerText);
                        break;
                    case "coin":
                        mc.coin = int.Parse(e.InnerText);
                        break;
                    case "exp":
                        mc.exp = int.Parse(e.InnerText);
                        break;
                    case "crystal":
                        mc.crystal = int.Parse(e.InnerText);
                        break;
                }
            }
            mapDic.Add(ID, mc);
        }
        PECommon.Log("MapCfg Init Done.");
    }
    public MapCfg GetMapCfg(int id)
    {
        MapCfg mc = null;
        if (mapDic.TryGetValue(id, out mc))
        {
            return mc;
        }
        return null;
    }
    #endregion

    #region 强化升级配置
    //字典以位置分成6个字典
    private Dictionary<int, Dictionary<int, StrongCfg>> strongDic = new Dictionary<int, Dictionary<int, StrongCfg>>();
    private void InitStrongCfg()
    {
        XmlDocument doc = new XmlDocument();
        //TODO
        //开发配置
        doc.Load(@"C:\huangziyang\study\Unity\Project\DarkGod\Assets\Resources\ResCfgs\strong.xml");
        //部署配置
        //doc.Load(@"C:\ResCfgs\strong.xml");
        XmlNodeList nodeList = doc.SelectSingleNode("root").ChildNodes;

        for (int i = 0; i < nodeList.Count; i++)
        {
            XmlElement ele = nodeList[i] as XmlElement;

            if (ele.GetAttributeNode("ID") == null)
            {
                continue;
            }
            int ID = Convert.ToInt32(ele.GetAttributeNode("ID").InnerText);

            StrongCfg sd = new StrongCfg
            {
                ID = ID
            };
            foreach (XmlElement e in nodeList[i].ChildNodes)
            {
                int val = int.Parse(e.InnerText);
                switch (e.Name)
                {
                    case "pos":
                        sd.pos = val;
                        break;
                    case "starlv":
                        sd.startlv = val;
                        break;
                    case "addhp":
                        sd.addhp = val;
                        break;
                    case "addhurt":
                        sd.addhurt = val;
                        break;
                    case "adddef":
                        sd.adddef = val;
                        break;
                    case "minlv":
                        sd.minlv = val;
                        break;
                    case "coin":
                        sd.coin = val;
                        break;
                    case "crystal":
                        sd.crystal = val;
                        break;
                }
            }

            Dictionary<int, StrongCfg> dic = null;
            if (strongDic.TryGetValue(sd.pos, out dic))
            {
                dic.Add(sd.startlv, sd);
            }
            else
            {
                dic = new Dictionary<int, StrongCfg>();
                dic.Add(sd.startlv, sd);

                strongDic.Add(sd.pos, dic);
            }
        }
        PECommon.Log("StrongCfg Init Done.");
    }
    //根据部位获取数据
    public StrongCfg GetStrongCfg(int pos, int startlv)
    {
        StrongCfg sd = null;
        Dictionary<int, StrongCfg> dic = null;
        if (strongDic.TryGetValue(pos, out dic))
        {
            if (dic.ContainsKey(startlv))
            {
                sd = dic[startlv];
            }
        }
        return sd;
    }
    #endregion

}

//数据类

public class MapCfg : BaseData<MapCfg>
{
    public int power;
    public int coin;
    public int exp;
    public int crystal;
}
//强化
public class StrongCfg : BaseData<StrongCfg>
{
    public int pos;//装备位置
    public int startlv;//星级
    public int addhp;
    public int addhurt;
    public int adddef;
    public int minlv; //最低需求等级
    public int coin;
    public int crystal;
}
//任务奖励
public class TaskRewardCfg : BaseData<TaskRewardCfg>
{
    public int count;
    public int exp;
    public int coin;
}

public class TaskRewardData : BaseData<TaskRewardData>
{
    public int prgs;  //进度
    public bool taked; //是否领取
}

//引导
public class GuideCfg : BaseData<GuideCfg>
{
    public int coin;
    public int exp;
}

public class BaseData<T>
{
    public int ID;
}

