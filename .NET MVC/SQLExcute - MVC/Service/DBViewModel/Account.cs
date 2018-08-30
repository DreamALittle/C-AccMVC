using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Acctrue.CMC.Web.WebDBInstall.DBViewModel
{
    public class Account
    {
        public Account(string Name ,string PWD)
        {
            UserName = Name;
            PassWord = PWD;
        }

        public string UserName { get; set; }
        public string PassWord { get; set; }
    }
}