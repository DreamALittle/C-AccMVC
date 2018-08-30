using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Acctrue.CMC.Configuration;
using Acctrue.CMC.Web.WebDBInstall.Config;

namespace Acctrue.CMC.Web.WebDBInstall.Setup
{
    public class DBSetup
    {
        public DBObject Initialize()
        {
            var archiver = DatabaseArchiver.Create();
            DBObject db = archiver.Read();
            return db ;
        }
    }
}