using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Acctrue.CMC.Interface.DBInstall;
using MongoDB.Driver;
using Acctrue.CMC.Model.ServerSideModel;
using MongoDB.Bson;
using Acctrue.CMC.Configuration;
using System.Web;
using System.IO;
using System.Data.SqlClient;
using Acctrue.CMC.Service.Configs;

namespace Acctrue.CMC.Service.DBInstall
{
    public class DBInstallService : IDBInstall
    {
        //数据库测试连接
        public DBConnTestResultModel DBConnTest(DBConnTestModel dbModel)
        {
            DBConnTestResultModel returnRes = new DBConnTestResultModel();

            DBObject db = null;

            if (dbModel.DBDrive == "SqlServer2005" || dbModel.DBDrive == "MySql")
            {
                if (dbModel.DBCreateNew == true)
                {
                    //尝试新建数据库
                    string str = "Data Source=" + dbModel.DBServer + ";" + "Initial Catalog = master;" + "User id=" + dbModel.DBUserID + ";" + "Password=" + dbModel.DBPassWord;
                    SqlConnection Conn = new SqlConnection(str);
                    using (SqlCommand Comm = Conn.CreateCommand())
                    {
                        Conn.Open();
                        Comm.CommandText = "CREATE  DATABASE " + dbModel.DBName;
                        try
                        {
                            Comm.ExecuteNonQuery();
                        }
                        catch 
                        {
                            returnRes.res = "尝试建立新数据库失败，请检查配置是否正确";
                            return returnRes;
                        }
                        //测试成功则可以返回
                        db = CreateDBObject(dbModel.DBDrive, dbModel.DBServer, dbModel.DBName, dbModel.DBUserID, dbModel.DBPassWord);
                        returnRes.db = db;
                        returnRes.res = "数据库连接成功";
                        return returnRes;
                    }
                }                
            }

            db = CreateDBObject(dbModel.DBDrive, dbModel.DBServer, dbModel.DBName, dbModel.DBUserID, dbModel.DBPassWord);

            if (db.DbDrive != DBDrive.Oracle)
            {
                if (dbModel.DBCreateNew)
                {
                    if (db.TestConnection(false))
                    {
                        returnRes.res = "数据库已经创建";
                    }
                    if (!db.NewMasterDBObject().TestConnection(false))
                    {
                        returnRes.res = "连接失败，请检查配置是否正确(或检查是否勾选“新建数据库”)";
                    }
                    returnRes.res = "数据库连接成功"; 
                }
            }
            if (!db.TestConnection(false))
            {
                returnRes.res = "连接失败，请检查配置是否正确(或检查是否勾选“新建数据库”)";
            }
            else
            {
                returnRes.res = "数据库连接成功";
            }

            returnRes.db = db;
            return returnRes;
        }
        //MongoDB测试连接
        public string MongoDBConnTest(MongoDBConnTestModel mongoPost)
        {

            string connectionString = "mongodb://";
            string dbServer = mongoPost.SeverIP;
            string dbName = mongoPost.MongoDBName;

            //同理，要区分是否添加用户名、密码
            string connection = "";
            if (mongoPost.User == "无需填写" || mongoPost.Password == "无需填写")
            {
                connection = connectionString + dbServer + "/" + dbName;
            }
            else
            {
                string userId = mongoPost.User;
                string password = mongoPost.Password;
                connection = connectionString + userId + ":" + password + "@" + dbServer + "/" + dbName;
            }
            
            MongoClient Client = new MongoClient(connection);
           
            IMongoDatabase db = Client.GetDatabase(dbName);

            string res = "";
            try {
                
                db.CreateCollection("InstallTest");
                db.DropCollection("InstallTest");
                res = "MongoDB连接成功";
            }
            catch(Exception exception)
            {
                string err = exception.Message;
                res = "Mongo连接失败，请检查配置信息,若无误请联系DBA";
            }

            return res +"|"+connection;
        }
        //数据库脚本执行
        public string SqlScriptExcute(DBinstallModel installModel,string rootPath,string conn)
        {
            string res = "";
            string filePath = "";
            if (installModel.DBDrive == "Oracle")
            {
                //防止用户错误操作
                installModel.DBCreateNew = false;
            }

            //获取文件路径
            List<FileInfo> files = new List<FileInfo>();
            if (installModel.DBCreateNew || Convert.ToDouble(installModel.DBNewestVersion) == 1.0 )
            {
                filePath = rootPath + "Create";
                files.AddRange(LoadScriptFiles(filePath));
            }
            else
            {
                while(Convert.ToSingle(installModel.DBCurrentVersion) < Convert.ToSingle(installModel.DBNewestVersion))
                {
                    installModel.DBCurrentVersion = Convert.ToString(Convert.ToDouble(installModel.DBCurrentVersion) + 0.01);
                    filePath = rootPath + "Update/V-" + installModel.DBCurrentVersion;
                    files.AddRange(LoadScriptFiles(filePath));                            
                }
            }

            //读取路径下所有文件
            try
            {
                DBScriptExecutor excutor = new DBScriptExecutor(conn);
                List<SqlGroup> scriptsList = new List<SqlGroup>();

                //读取SQL内容
                foreach (FileInfo scriptFile in files)
                {
                    scriptsList.Add(new SqlGroup(ReadStatements(scriptFile.FullName, installModel.DBDrive)));
                }

                //执行SQL脚本
                res = excutor.ExecuteSqlScript(installModel.DBDrive, scriptsList, installModel.DBName);

                //更新数据库config表中DBVersion信息
                if (res == "执行脚本成功")
                {
                    string updateVersion = installModel.DBCurrentVersion;
                    ConfigService ser = new ConfigService();
                    res = ser.UpdateDBVersion("DBVersion", updateVersion);
                }
                return res;
            }
            catch (Exception ex)
            {
                return res = ex.Message;
            }
        }

        private List<FileInfo> LoadScriptFiles(string path)
        {
            DirectoryInfo dir = new DirectoryInfo(path);
            FileInfo[] files = dir.GetFiles("*.sql");
            return files.ToList();
        }

        private DBObject CreateDBObject(string Drive, string Server, string Name, string UserId, string Password)
        {
            DBObject db = null;
            
            switch (Drive)
            {
                case "SqlServer2005":
                    db = new DBObject(DBDrive.SqlServer2005);
                    break;
                case "Oracle":
                    db = new DBObject(DBDrive.Oracle);
                    break;
                case "MySql":
                    db = new DBObject(DBDrive.MySql);
                    break;
            }

            db.DbServer = Server;
            db.DbName = Name;
            db.UserId = UserId;
            db.Password = Password;
            return db;
        }

        private List<string> ReadStatements(string fileName,string Drive)
        {
            var list = new List<string>();
            var sb = new StringBuilder();
            var databasePattern = new System.Text.RegularExpressions.Regex("(CREATE|DROP).* DATABASE.*");
            using (var reader = new StreamReader(fileName, Encoding.UTF8))
            {
                string text = reader.ReadLine();
                while (text != null)
                {
                    if (!text.Trim().StartsWith("--")
                        && !text.Trim().StartsWith("/*")
                        && !text.Trim().StartsWith("*")
                        && !text.Trim().StartsWith("*/"))
                    {
                        switch (Drive)
                        {
                            case "SqlServer2005":
                                if (text.Trim().ToUpper() != "GO")
                                    sb.Append(text + " ");
                                else
                                {
                                    var sql = sb.ToString().Replace("\r", " ").Replace("\n", "");
                                    list.Add(sql);
                                    sb.Remove(0, sb.Length);
                                }
                                break;
                            case "Oracle":
                                if (!text.Trim().EndsWith(";"))
                                {
                                    if (text.Trim().EndsWith("~"))
                                        text = text.Trim().TrimEnd('~') + "; ";
                                    sb.Append(" " + text);
                                }
                                else
                                {
                                    sb.Append(" " + text.Trim().TrimEnd(';') + ";");
                                    var sql = sb.ToString().Replace("\r", " ").Replace("\n", "");
                                    list.Add(sql);
                                    sb.Remove(0, sb.Length);
                                }
                                break;
                            case "MySql":

                                if (text.Trim().ToUpper() != "GO")
                                    sb.Append(" " + text);
                                else
                                {
                                    sb.Append(" " + text.Trim());
                                    var sql = sb.ToString().Replace("\r", " ").Replace("\n", "").Replace("GO", "");
                                    if (sql.Contains("delimiter $$"))
                                    {
                                        StringBuilder ssb = new StringBuilder();
                                        ssb.Append(sql.Replace("delimiter $$", "")).Replace("end $$", "end;");
                                        sql = ssb.ToString();
                                    }
                                    list.Add(sql);
                                    sb.Remove(0, sb.Length);
                                }
                                break;
                        }
                    }
                    text = reader.ReadLine();
                }
            }
            return list;
        }
    }
}
