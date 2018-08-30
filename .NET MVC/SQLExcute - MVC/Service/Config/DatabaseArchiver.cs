using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Xml;
using Acctrue.CMC.Configuration;

namespace Acctrue.CMC.Web.WebDBInstall.Config
{
    public class DatabaseArchiver :XmlArchiver
    {
        const string DB_PATH = @"//configuration/Acctrue.Library.Data.Settings/add";
        const string DB_NAME = "key";
        const string DB_CONN_STR = "value";
        XmlNodeList databaseNodes = null;

        public static DatabaseArchiver Create()
        {
            return new DatabaseArchiver(HttpContext.Current.Server.MapPath("~/Web.Config"));
        }

        /// <summary>
        /// 构造并校验
        /// </summary>
        /// <param name="configPath">配置文件物理路径</param>
        public DatabaseArchiver(string configPath) : base(configPath)
        {
            lock (_lock)
            {
                databaseNodes = document.SelectNodes(DB_PATH);
                if (databaseNodes == null || databaseNodes.Count <= 0)
                    throw new Exception(string.Format("the database config is not exists in {0}", configPath));
            }
        }

        /// <summary>
        /// 读取数据库配置信息
        /// </summary>
        /// <returns></returns>
        public DBObject Read()
        {
            lock (_lock)
            {
                var config = ReadNode(databaseNodes[0]);
                return new DBObject(config.Value);
            }
        }

        /// <summary>
        /// 保存数据库配置 
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public bool Save(DBObject db)
        {
            lock (_lock)
            {
                //for(int i=databaseNodes.Count-1;i>=0;i--)
                foreach (XmlNode dbNode in databaseNodes)
                {
                    //XmlNode dbNode = databaseNodes[i];
                    if (dbNode.ParentNode == null) continue;
                    var key = dbNode.Attributes["key"].Value;
                    var index = key.IndexOf('.');
                    var providerPrefix = "DbProviderFactory";
                    var provider = providerPrefix;
                    var providerPathFormat = "add[@key='{0}']";
                    if (index <= -1)
                        dbNode.Attributes[DB_CONN_STR].Value = db.ToString();
                    else
                    {
                        var tmp = new DBObject(db.DbDrive);
                        tmp.UserId = db.UserId;
                        tmp.DbServer = db.DbServer;
                        tmp.DbName = db.DbName;
                        tmp.Password = db.Password;
                        var keyword = key.Substring(0, index);
                        tmp.DbName = string.Format("{0}_{1}", tmp.DbName, keyword.Trim());
                        dbNode.Attributes[DB_CONN_STR].Value = tmp.ToString();
                        provider = keyword.Trim() + "." + providerPrefix;
                    }

                    var providerNodePath = string.Format(providerPathFormat, provider);
                    var providerNode = CheckAndCreateNode(dbNode.ParentNode, "add", providerNodePath);

                    if (db.DbDrive == DBDrive.Oracle)
                    {
                        CheckAndCreateAttribute(providerNode, "key", provider);
                        CheckAndCreateAttribute(providerNode, "value", "@SmartDbFactory : System.Data.OracleClient, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
                    }
                    else if (db.DbDrive == DBDrive.MySql)
                    {
                        CheckAndCreateAttribute(providerNode, "key", provider);
                        CheckAndCreateAttribute(providerNode, "value", "@SmartDbFactory : MySql.Data, Version=5.2.3.0, Culture=neutral, PublicKeyToken=c5687fc88969c44d");
                    }
                    else
                        providerNode.ParentNode.RemoveChild(providerNode);
                }

                var success = false;
                for (var i = 0; i < 10; i++)
                {
                    success = false;
                    try
                    {
                        base.Save();
                        break;
                    }
                    catch
                    {
                        System.Threading.Thread.Sleep(30);
                    }
                }
                return success;
            }
        }

        private KeyValuePair<string, string> ReadNode(XmlNode node)
        {
            lock (_lock)
            {
                return new KeyValuePair<string, string>(node.Attributes[DB_NAME].Value, node.Attributes[DB_CONN_STR].Value.Trim());
            }
        }


    }
}