using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Acctrue.CMC.Web.WebDBInstall.DBViewModel
{
    public class InstallInfo
    {
        public string AppVersion { get; set; }
        public string DBVersion { get; set; }
        //DB Setting
        public string DBDrive { get; set; }
        public bool DBCreateNew { get; set; }
        public string DBServer { get; set; }
        public string DBUserID { get; set; }
        public string DBPassWord { get; set; }
        public string DBName { get; set; }
        //Mongo
        public bool MongoSetting { get; set; }
        public string MongoServer { get; set; }
        public string MongoUserName { get; set; }
        public string MongoPassword { get; set; }
        public string MongoDBName { get; set; }
        //Login
        public string LoginUser { get; set; }
        public string LoginPassword { get; set; }
    }
}