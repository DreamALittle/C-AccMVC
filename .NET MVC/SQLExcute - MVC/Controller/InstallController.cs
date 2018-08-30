using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Acctrue.CMC.Web.WebDBInstall.DBViewModel;
using Acctrue.CMC.Web.WebDBInstall.Config;
using Acctrue.CMC.Web.WebDBInstall.Setup;
using System.Net;
using System.Web.Routing;
using Acctrue.CMC.Configuration;
using System.Configuration;

namespace Acctrue.CMC.Web.Controllers.UI
{
    public class InstallController : Controller
    {
        public ActionResult DBInstall()
        {
            HttpCookie cookie = System.Web.HttpContext.Current.Request.Cookies.Get("IFDBSignIn");
            if (cookie == null)
            {
                return RedirectToAction("DBInstallLogin");
            }
            else
            {
                InstallInfo info = new InstallInfo();
                string appVersion = ConfigReader.GetAppSetting();
                info.AppVersion = appVersion;

                try
                {
                    DBSetup setup = new DBSetup();
                    DBObject db = setup.Initialize();
                   
                    info.DBVersion = VersionReader.DatabaseVersion(db).ToString(); 
                    info.DBDrive = db.DbDrive.ToString();
                    info.DBCreateNew = false;
                    info.DBServer = db.DbServer;
                    info.DBUserID = db.UserId;
                    info.DBPassWord = db.Password;
                    info.DBName = db.DbName;
                }

                catch
                {
                    info.DBVersion = "1.0";
                    info.DBDrive = "SqlServer2005";
                    info.DBCreateNew = false;
                    info.DBServer = "";
                    info.DBUserID = "";
                    info.DBPassWord = "";
                    info.DBName = "";
                }
                
                
                //Mongo
                string mongoStr = "";
                try
                {
                    mongoStr = ConfigurationManager.ConnectionStrings["MongoDB"].ConnectionString;
                }
                catch{}

                if (mongoStr == "")
                {
                    info.MongoSetting = false;
                }
                else
                {
                    info.MongoSetting = true;
                    //mongodb://10.210.160.98:20001/test
                    //mongodb://libin1:123@192.168.20.113:27017/test
                    //要区分@ 和没有@两种情况
                    string[] mongoArr =new string[] { };
                    if (mongoStr.Contains('@'))
                    {
                        mongoArr = mongoStr.Split(new char[] { '/', ':', '@' });
                        info.MongoUserName = mongoArr[3];
                        info.MongoPassword = mongoArr[4];
                    }
                    else
                    {
                        mongoArr = mongoStr.Split(new char[] { '/', ':' });
                        info.MongoUserName = "无需填写";
                        info.MongoPassword = "无需填写";
                    }
                    info.MongoServer = mongoArr[mongoArr.Length - 3] + ":" + mongoArr[mongoArr.Length - 2];
                    info.MongoDBName = mongoArr[mongoArr.Length - 1];
                }

                Account configAccout = ConfigReader.GetAccount();
                info.LoginUser = configAccout.UserName;
                info.LoginPassword = configAccout.PassWord;

                return View(info);
            }     
        }
        public ActionResult DBInstallLogin()
        {
            return View();
        }
        public ActionResult DBInstallCheckAccount()
        {
            string UserName =System.Web.HttpContext.Current.Request.Params["UserName"];
            string PassWord = System.Web.HttpContext.Current.Request.Params["PassWord"];

            Account configAccout = ConfigReader.GetAccount();
            if(UserName == configAccout.UserName && PassWord == configAccout.PassWord)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Accepted);
            }
            return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
        }
    }
}