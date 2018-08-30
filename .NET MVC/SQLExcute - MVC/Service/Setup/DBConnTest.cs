using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Acctrue.CMC.Web.WebDBInstall.Setup
{
    public class DBConnTest
    {


        public string MongoTest()
        {
            string connectionString = "mongodb://";
            string dbServer = this["DbServer"] as string;
            string dbName = this["DbName"] as string;
            string userId = this["UserName"] as string;
            string password = this["Password"] as string;

            //分割
            string[] dbServers = dbServer.Split('|');
            bool success = true;
            string errIP = string.Empty;
            string connection = connectionString;
            foreach (string item in dbServers)
            {
                string connstr = string.Empty;
                if (!userId.IsNullOrEmpty() && !password.IsNullOrEmpty())
                {
                    connection = connection + userId + ":" + password + "@" + item + "|";
                    connstr = connectionString + userId + ":" + password + "@" + item + "/" + dbName;
                }
                else
                {
                    connection = connection + item + "|";
                    connstr = connectionString + item + "/" + dbName;
                }
                Mongo mongo = new Mongo(connstr);
                try
                {
                    // 打开连接
                    mongo.Connect();
                    // 切换到指定的数据库
                    IMongoDatabase database = mongo.GetDatabase(dbName);
                    if (database.Javascript.Contains("异常"))
                    {
                    }

                }
                catch (Exception e)
                {
                    errIP = item;
                    success = false;
                }
                finally
                {
                    mongo.Dispose();
                }
            }
            if (success)
            {
                var appDomainConfigFile = HttpContext.Current.Server.MapPath("~/") + "Web.config";
                Acctrue.TTS.Util.Config config = new Acctrue.TTS.Util.Config(appDomainConfigFile);
                config.SetValue(ConfigInfo.MongoDBConnection, connection.Remove(connection.Length - 1) + "/" + dbName, false);
                config.SetValue(ConfigInfo.MongoDBHosts, dbServer, false);
                config.SaveConfig();
                WriteJson(new { Success = "1", Message = "MongoDB数据库连接成功" });
                return;
            }
            else
            {
                WriteJson(new { Success = "0", Message = "MongoDB" + errIP + "连接失败，请检查配置是否正确" });
                return;
            }
        }
    }
}