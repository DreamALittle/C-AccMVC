using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.IO;
using System.Text;
using Acctrue.CMC.Configuration;
using Acctrue.CMC.Model.ServerSideModel;
namespace Acctrue.CMC.Service.DBInstall
{
    public class DBScriptExecutor
    {
        public DBObject DB { get; set; }
        
        public DBScriptExecutor(string conn)
        {
            this.DB = new DBObject(conn);
        }

        public string ExecuteSqlScript(string Drive,List<SqlGroup> scriptsList,string DBName)
        {
            string res = "";
            switch (Drive)
            {
                case "SqlServer2005":
                    res = ExecuteSqlServerScript(scriptsList, DBName);
                    break;
                case "MySql":
                    res = ExecuteMySqlScript(scriptsList, DBName);
                    break;
                case "Oracle":
                    res = ExecuteOracleScript(scriptsList, DBName);
                    break;
            }
            return res;
        }

        public string ExecuteSqlServerScript(List<SqlGroup> scriptsList,string DBName)
        {
            DBObject masterDbObject = DB.NewMasterDBObject();
            using (IDbConnection connection = masterDbObject.DbConnection)
            {
                connection.Open();
                using (IDbCommand cmd = connection.CreateCommand())
                {
                    string dbPath = string.Empty;
                    foreach (var group in scriptsList)
                    {
                        foreach (string script in group.Statements)
                        {
                            string cmdText = null;
                            try
                            {
                                string s = script.Trim().ToLower();
                                if (s.Contains("use[{0}]") || s.Contains("use [{0}]"))
                                {
                                    //获取默认的数据库存放的物理路径
                                    cmd.CommandText = "select physical_name from sys.master_files where database_id=db_id('" + DBName + "')";
                                    object v = cmd.ExecuteScalar();
                                    if (v == null)
                                        throw new Exception(DBName + " DbName not exists");
                                    string path = v.ToString();
                                    dbPath = path.Substring(0, path.LastIndexOf('\\')).TrimEnd('\\');
                                }
                                try
                                {
                                    if (!s.Contains("insert into languages"))
                                        cmdText = string.Format(script, DBName, dbPath);
                                    else
                                        cmdText = script;
                                }
                                catch
                                {
                                    cmdText = script;
                                }
                                if (!string.IsNullOrEmpty(cmdText))
                                {
                                    cmd.CommandText = cmdText;
                                    cmd.CommandTimeout = 300;
                                    cmd.ExecuteNonQuery();
                                }
                            }
                            catch (Exception err)
                            {
                                return "执行" + cmdText + " \r\n 出错：" + err.Message + "\r\n";
                            }
                        }
                    }
                }
            }
            return "执行脚本成功";
        }

        public string ExecuteMySqlScript(List<SqlGroup> scriptsList, string DBName)
        {
            DBObject masterDbObject = DB.NewMasterDBObject();
            using (IDbConnection connection = masterDbObject.DbConnection)
            {
                connection.Open();
                using (IDbCommand cmd = connection.CreateCommand())
                {
                    string dbPath = string.Empty;
                    foreach (var group in scriptsList)
                    {
                        foreach (string script in group.Statements)
                        {
                            string cmdText = null;
                            try
                            {
                                string s = script.Trim().ToLower();
                                if (s.Contains("use{0}") || s.Contains("use {0}"))
                                {
                                    //获取默认的数据库存放的物理路径
                                    cmd.CommandText = "SELECT * FROM information_schema.SCHEMATA where SCHEMA_NAME='" + DBName + "'";
                                    object v = cmd.ExecuteScalar();
                                    if (v == null)
                                        throw new Exception(DBName + " DbName not exists");
                                }
                                try
                                {
                                    if (!s.Contains("insert into languages"))
                                        cmdText = string.Format(s, DBName);
                                    else
                                        cmdText = s;
                                }
                                catch
                                {
                                    cmdText = s;
                                }
                                if (!string.IsNullOrEmpty(cmdText))
                                {
                                    cmd.CommandText = cmdText;
                                    cmd.CommandTimeout = 30000;
                                    cmd.ExecuteNonQuery();
                                }
                            }
                            catch (Exception err)
                            {
                                return "执行" + cmdText + " \r\n 出错：" + err.Message + "\r\n";
                            }
                        }
                    }
                }
            }
            return "执行脚本成功";
        }

        public string ExecuteOracleScript(List<SqlGroup> scriptsList, string DBName)
        {
            var sb = new StringBuilder();
            using (var connection = DB.DbConnection)
            {
                connection.Open();
                using (IDbCommand cmd = connection.CreateCommand())
                {
                    //获取默认表空间存放的物理路径
                    string dbPath = string.Empty;
                    cmd.CommandText = @"select file_name from dba_data_files x, dba_tablespaces y, dba_users z
                            where x.tablespace_name=y.tablespace_name and y.tablespace_name=z.default_tablespace and username='" + DB.UserId.ToUpper() + "'";
                    object v = cmd.ExecuteScalar();
                    if (v == null)
                        throw new Exception(DB.UserId + " UserId not exists");
                    string path = v.ToString();
                    dbPath = path.Substring(0, path.LastIndexOf('\\')).TrimEnd('\\');

                    List<List<string>> list = new List<List<string>>();
                    List<string> sbList = new List<string>();
                    foreach (var group in scriptsList)
                    {
                        foreach (string script in group.Statements)
                        {
                            string cmdText = null;
                            try
                            {
                                if (script.Trim().ToLower().StartsWith("declare") || script.Trim().ToUpper().Contains("TRIGGER"))
                                    cmdText = string.Format(script, DB.UserId, dbPath);
                                else if (!script.Trim().ToLower().Contains("insert into languages"))
                                    cmdText = string.Format(script.Substring(0, script.Length - 1), DB.UserId, dbPath);
                                else
                                    cmdText = script.Substring(0, script.Length - 1);
                            }
                            catch
                            {
                                if (script.Trim().ToLower().StartsWith("declare"))
                                    cmdText = script;
                                else
                                    cmdText = script.TrimEnd(';');
                            }
                            if (string.IsNullOrEmpty(cmdText))
                                continue;

                            string tmp = cmdText.ToUpper().TrimStart();
                            if (!tmp.StartsWith("INSERT") && !tmp.StartsWith("UPDATE") && !tmp.StartsWith("DELETE"))
                            {
                                List<string> tList = new List<string>();
                                tList.Add(cmdText);
                                list.Add(tList);
                            }
                            else
                            {
                                if (sbList.Count <= 1000)
                                    sbList.Add(cmdText);
                                if (sbList.Count >= 1000)
                                {
                                    list.Add(sbList);
                                    sbList = new List<string>();
                                }
                            }
                        }
                        if (sbList.Count > 0)
                        {
                            list.Add(sbList);
                            sbList = new List<string>();
                        }
                    }

                    foreach (List<string> cmdTextList in list)
                    {
                        using (var tran = connection.BeginTransaction())
                        {
                            cmd.Transaction = tran;
                            foreach (string cmdText in cmdTextList)
                            {
                                try
                                {
                                    cmd.CommandText = cmdText;
                                    cmd.CommandTimeout = 300;
                                    cmd.ExecuteNonQuery();
                                }
                                catch (Exception err)
                                {
                                    //return "执行" + cmdText + " \r\n 出错：" + err.Message + "\r\n";

                                    return "执行" + cmdText + "出错!" + err.Message;
                                }
                            }
                            tran.Commit();
                        }
                    }
                }
            }
            return "执行脚本成功";
        }
    }
}
