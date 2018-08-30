using Acctrue.CMC.Web.Controllers.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Acctrue.CMC.Web.Controllers.UI
{
    [LoginFilter]
    public class CodeRuleController : Controller
    {
        // GET: System
        public ActionResult CodeRuleList()
        {
            return View();
        }

        public ActionResult CodeRuleEdit()
        {
            return View();
        }
        public ActionResult CodeRuleSegConfig()
        {
            return View();
        }
    }
}