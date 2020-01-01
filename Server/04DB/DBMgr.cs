//数据库管理层
using MySql.Data.MySqlClient;
using PEProtocol;
using System;

class DBMgr
{
    private static DBMgr instance = null;

    public static DBMgr Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new DBMgr();
            }
            return instance;
        }
    }

    private MySqlConnection conn;

    public void Init()
    {
        //部署配置
        //conn = new MySqlConnection("server=localhost;User Id =admin;password=admin;Database=darkgod;Charset = utf8");
        //开发配置
        conn = new MySqlConnection("server=localhost;User Id =root;password=root;Database=darkgod;Charset = utf8");
        conn.Open();  //打开链接
        PECommon.Log("DBMgr Init Done");
    }
    //查询玩家数据
    public PlayerData QueryPlayerData(string acct,string pass)
    {
        bool isNew = true;
        PlayerData playerData = null;
        MySqlDataReader reader = null; 
        //操作数据库的过程需要捕捉异常
        try
        {
            MySqlCommand cmd = new MySqlCommand("select * from account where acct = @acct", conn);
            cmd.Parameters.AddWithValue("acct", acct);
            reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                isNew = false;
                string _pass = reader.GetString("pass");
                if (_pass.Equals(pass))
                {
                    //密码正确，返回玩家数据
                    playerData = new PlayerData
                    {
                        id = reader.GetInt32("id"),
                        name = reader.GetString("name"),
                        lv = reader.GetInt32("level"),
                        exp = reader.GetInt32("exp"),
                        Power = reader.GetInt32("power"),
                        coin = reader.GetInt32("coin"),
                        diamond = reader.GetInt32("diamond"),
                        crystal = reader.GetInt32("crystal"),

                        hp = reader.GetInt32("hp"),
                        ad = reader.GetInt32("ad"),
                        ap = reader.GetInt32("ap"),
                        addef = reader.GetInt32("addef"),
                        apdef = reader.GetInt32("apdef"),
                        dodge = reader.GetInt32("dodge"),
                        pierce = reader.GetInt32("pierce"),
                        critical = reader.GetInt32("critical"),

                        guideid = reader.GetInt32("guideid"),
                        time = reader.GetInt64("time"),
                        fuben = reader.GetInt32("fuben"),
                        //TODO 增加新字段

                    };
                    #region Strong 强化升级
                    //数据示意 1#2#2#3#4#5#
                    string[] strongStrArr = reader.GetString("strong").Split('#');
                    //分割保存
                    int[] _strongArr = new int[6];
                    for(int i = 0; i < strongStrArr.Length; i++)
                    {
                        if (strongStrArr[i] == "")
                        {
                            continue;
                        }
                        if(int.TryParse(strongStrArr[i],out int starLv))
                        {
                            _strongArr[i] = starLv;
                        }
                        else
                        {
                            PECommon.Log("Parse Strong Data Error", LogType.Error);
                        }
                    }
                    playerData.strongArr = _strongArr;
                    #endregion
                    #region 任务奖励
                    //数据示意1|1|0#2|1|5#3|1|5#4|1|5#5|1|5#6|1|5#
                    string[] taskStrArr = reader.GetString("task").Split('#');
                    playerData.taskArr = new string[6];
                    for(int i = 0; i < taskStrArr.Length; i++)
                    {
                        if(taskStrArr[i] == "")
                        {
                            continue;
                        }//安全校验
                        else if (taskStrArr[i].Length >= 5)
                        {
                            playerData.taskArr[i] = taskStrArr[i];
                        }
                        else
                        {
                            throw new Exception("DataError");
                        }
                    }
                    #endregion
                    //TODO
                }
            }
        }
        catch(Exception e)
        {
            PECommon.Log("Query PlayerDta By Acct&Pass Error:" + e, LogType.Error);
        }
        finally
        {
            if(reader != null)
            {
                reader.Close();
            }
            if (isNew)
            {
                //不存在账号数据，创建账号并返回
                playerData = new PlayerData
                {
                    id = -1,
                    name = "",
                    lv = 1,
                    exp = 0,
                    Power = 150,
                    coin = 5000,
                    diamond = 500,
                    crystal = 500,

                    hp = 2000,
                    ad = 275,
                    ap = 265,
                    addef = 67,
                    apdef = 43,
                    dodge = 7,
                    pierce = 5,
                    critical = 2,

                    guideid = 1001,
                    strongArr = new int[6],
                    time = TimerSvc.Instance.GetNowTime(),
                    taskArr = new string[6],
                    fuben = 10001,
                    //TODO增加数据
                };
                //返回主键ID
                //数据示意1|1|0#2|1|0#3|1|0#4|1|0#5|1|0#6|1|0#
                //初始化任务奖励数据
                for (int i = 0;i<playerData.taskArr.Length;i++)
                {
                    playerData.taskArr[i] = (i + 1) + "|0|0";
                }
                playerData.id = InsertNewAcctData(acct, pass, playerData);
            }
        }
        return playerData;
    }
    //插入玩家数据
    private int InsertNewAcctData(string acct,string pass,PlayerData pd)
    {
        int id = -1;
        try
        {
            MySqlCommand cmd = new MySqlCommand(
                "insert into account set acct=@acct,pass=@pass,name=@name,level=@level,exp=@exp,power=@power,coin=@coin,diamond=@diamond,crystal=@crystal,"+
                "hp=@hp,ad=@ad,ap=@ap,addef=@addef,apdef=@apdef,dodge=@dodge,pierce=@pierce,critical=@critical,guideid=@guideid,strong=@strong,time=@time,"+
                "task=@task,fuben=@fuben",conn);
            cmd.Parameters.AddWithValue("acct", acct);
            cmd.Parameters.AddWithValue("pass", pass);
            cmd.Parameters.AddWithValue("name", pd.name);
            cmd.Parameters.AddWithValue("level", pd.lv);
            cmd.Parameters.AddWithValue("exp", pd.exp);
            cmd.Parameters.AddWithValue("power", pd.Power);
            cmd.Parameters.AddWithValue("coin", pd.coin);
            cmd.Parameters.AddWithValue("diamond", pd.diamond);
            cmd.Parameters.AddWithValue("crystal", pd.crystal);

            cmd.Parameters.AddWithValue("hp", pd.hp);
            cmd.Parameters.AddWithValue("ad", pd.ad);
            cmd.Parameters.AddWithValue("ap", pd.ap);
            cmd.Parameters.AddWithValue("addef", pd.addef);
            cmd.Parameters.AddWithValue("apdef", pd.apdef);
            cmd.Parameters.AddWithValue("dodge", pd.dodge);
            cmd.Parameters.AddWithValue("pierce", pd.pierce);
            cmd.Parameters.AddWithValue("critical", pd.critical);

            cmd.Parameters.AddWithValue("guideid", pd.guideid);
            //强化信息
            string strongInfo = "";
            for(int i = 0; i < pd.strongArr.Length; i++)
            {
                strongInfo += pd.strongArr[i];
                strongInfo += "#";
            }
            cmd.Parameters.AddWithValue("strong", strongInfo);
            cmd.Parameters.AddWithValue("time", pd.time);
            //任务奖励
            //数据示意1|1|0#2|1|0#3|1|0#4|1|0#5|1|0#6|1|0#
            string taskInfo = "";
            for(int i = 0; i < pd.taskArr.Length; i++)
            {
                taskInfo += pd.taskArr[i];
                taskInfo += "#";
            }
            cmd.Parameters.AddWithValue("task", pd.taskArr);
            cmd.Parameters.AddWithValue("fuben", pd.fuben);
            //TODO添加数据
            cmd.ExecuteNonQuery();
            id = (int)cmd.LastInsertedId;//取得主键的值
        }
        catch (Exception e)
        {
            PECommon.Log("Insert PlayerDta Error:" + e, LogType.Error);
        }
        return id;
    }
    //查询玩家姓名
    public bool QueryNameData(string name)
    {
        bool exist = false;
        MySqlDataReader reader = null;
        try
        {
            MySqlCommand cmd = new MySqlCommand("select * from account where name =@name",conn);
            cmd.Parameters.AddWithValue("name", name);
            reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                exist = true;
            }
        }
        catch(Exception e)
        {
            PECommon.Log("Query Name State Error:"+e,LogType.Error);
        }
        finally
        {
            if(reader != null){
                reader.Close();
            }
        }

        return exist;
    }
    //更新玩家数据
    public bool UpdatePlayerData(int id,PlayerData playerData)
    {
        try
        {
            MySqlCommand cmd = new MySqlCommand(
                "update account set name=@name,level=@level,exp=@exp,power=@power,coin=@coin,diamond=@diamond,crystal=@crystal," +
                "hp=@hp,ad=@ad,ap=@ap,addef=@addef,apdef=@apdef,dodge=@dodge,pierce=@pierce,critical=@critical," +
                "guideid=@guideid,strong=@strong,time=@time,task=@task,fuben=@fuben where id=@id", conn
                );
            cmd.Parameters.AddWithValue("id", id);
            cmd.Parameters.AddWithValue("name", playerData.name);
            cmd.Parameters.AddWithValue("level", playerData.lv);
            cmd.Parameters.AddWithValue("exp", playerData.exp);
            cmd.Parameters.AddWithValue("power", playerData.Power);
            cmd.Parameters.AddWithValue("coin", playerData.coin);
            cmd.Parameters.AddWithValue("diamond", playerData.diamond);
            cmd.Parameters.AddWithValue("crystal", playerData.crystal);

            cmd.Parameters.AddWithValue("hp", playerData.hp);
            cmd.Parameters.AddWithValue("ad", playerData.ad);
            cmd.Parameters.AddWithValue("ap", playerData.ap);
            cmd.Parameters.AddWithValue("addef", playerData.addef);
            cmd.Parameters.AddWithValue("apdef", playerData.apdef);
            cmd.Parameters.AddWithValue("dodge", playerData.dodge);
            cmd.Parameters.AddWithValue("pierce", playerData.pierce);
            cmd.Parameters.AddWithValue("critical", playerData.critical);

            cmd.Parameters.AddWithValue("guideid", playerData.guideid);
            //强化信息
            string strongInfo = "";
            for (int i = 0; i < playerData.strongArr.Length; i++)
            {
                strongInfo += playerData.strongArr[i];
                strongInfo += "#";
            }
            cmd.Parameters.AddWithValue("strong", strongInfo);
            cmd.Parameters.AddWithValue("time", playerData.time);
            string taskInfo = "";
            for(int i = 0; i < playerData.taskArr.Length; i++)
            {
                taskInfo += playerData.taskArr[i];
                taskInfo += "#";
            }
            cmd.Parameters.AddWithValue("task",taskInfo);
            cmd.Parameters.AddWithValue("fuben", playerData.fuben);
            //增加数据

            cmd.ExecuteNonQuery();  //执行语句
        }
        catch(Exception e)
        {
            PECommon.Log("Updata PlayerData Error:"+e,LogType.Error);
            return false;
        }

        return true;
    }

}

