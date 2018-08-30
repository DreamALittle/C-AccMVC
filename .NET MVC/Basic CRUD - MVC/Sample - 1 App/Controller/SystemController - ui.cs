using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Acctrue.CMC.Web.Controllers.Filter;
namespace Acctrue.CMC.Web.Controllers.UI
{
    [LoginFilter]
    public class SystemController : Controller
    {
        // GET: System

        public ActionResult AppSettingList()
        {
            return View();
        }

        public ActionResult AppEdit()
        {
            return View();
        }

        public ActionResult AppInterfaceEdit()
        {
            return View();
        }

        public ActionResult AppInterfaceList()
        {
            return View();
        }

        public ActionResult AppSettingEdit()
        {
            return View();
        }

        public ActionResult SystemLogList()
        {
            return View();
        }
    }
}