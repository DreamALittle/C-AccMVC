using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ReportEFCore.Models;
using Microsoft.EntityFrameworkCore;
using ReportEFCore.ReportViewModel;
using System.Data;
using System.Data.SqlClient;
using Stimulsoft.Report;
using Stimulsoft.Report.Mvc;
using Stimulsoft.Report.Web;
using Microsoft.AspNetCore.Routing;
using System.IO;
using ReportEFCore.SearchModel;
using Newtonsoft.Json;

namespace ReportEFCore.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult About()
        {
            return View();
        }
        public IActionResult AboutReport()
        {
            //从数据库获取数据源 Linq 
            HaiwoContext context = new HaiwoContext();
            List<Corps> corpList = context.Corps.FromSql("select * from Corps where AreaName <> ''").ToList();

            List<SelectCorp> sList = new List<SelectCorp>();
            foreach (Corps c in corpList)
            {
                SelectCorp s = new SelectCorp();
                s.AreaName = c.AreaName;
                s.CorpName = c.CorpName;
                s.CorpStatus = c.CorpStatus;
                sList.Add(s);
            }

            StiReport report = new StiReport();
            report.Load(StiNetCoreHelper.MapPath(this, "Report/TemplateFilterReport.mrt"));
            report.RegBusinessObject("CorpList", sList);

            return StiNetCoreViewer.GetReportResult(this, report);
        }

        private static CrossSearch searchCondition;
        public IActionResult Cross(CrossSearch para)
        {
            if (para != null)
            {
                searchCondition = para;
            }
            return View();
        }

        /// <summary>
        /// 区别点：显示置顶数据绑定
        /// 注意：application/x-www-form-urlencode换形式 否则报错415
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult CrossPartial([FromBody]CrossSearch para)
        {
            if (para != null)
            {
                searchCondition = para;
            }
            return PartialView();
        }
        /// <summary>
        /// 1.获取交叉数据绑定后台视图模型
        /// -- 基本数据从Ticket表中获取
        /// -- 再依据外键进行相应数据列匹配和选取（根据FORM请求进行数据筛选后再绑定）
        /// -- 取出所有数据后绑定报表文件展示
        /// </summary>
        /// <returns></returns>
        public IActionResult CrossReport()
        {
            TDContext context = new TDContext();

            List<Ticket> tickets = new List<Ticket>();

            if (searchCondition.DateFrom != null && searchCondition.DateFrom.ToString() != "" && searchCondition.DateTo.ToString() != "" && searchCondition.DateTo != null)
            {
                 tickets = context.Ticket.Where(t => t.TicketDate < searchCondition.DateTo && t.TicketDate > searchCondition.DateFrom).ToList();
            }
           else
            {
                 tickets = context.Ticket.FromSql("select * from Ticket").ToList();
            }

            List<TicketView> tList = new List<TicketView>();
            foreach(Ticket tic in tickets)
            {
                TicketView tView = new TicketView();
                # region 产品过滤
                Product product = new Product();
                if (searchCondition.Product != null && searchCondition.Product !="")
                {
                    product = context.Product.Where(p => p.ProductID == tic.ProductID && p.ProductName ==searchCondition.Product).FirstOrDefault();
                    if (product == null) continue; //问题 - 没有要不要显示？
                }
                else
                {
                    product = context.Product.Where(p => p.ProductID == tic.ProductID).FirstOrDefault();
                }
                tView.Product = product;
                #endregion

                #region 区域过滤 办事处过滤
                Corp issuer = context.Corp.Where(c => c.CorpID == tic.IssuerID).FirstOrDefault();
                if (issuer.CorpLevel != "办事处")
                {
                    //父企业
                    Corp parentCorp = context.Corp.Where(c => c.CorpID == issuer.ParentCorpID).FirstOrDefault();
                    if (searchCondition.Office != null && searchCondition.Office != "" && searchCondition.Office != parentCorp.CorpName) continue;
                    //区域信息
                    Area areaInfo = context.Area.Where(a => a.AreaId == parentCorp.AreaID).FirstOrDefault();
                    if (searchCondition.Area != null && searchCondition.Area != "" && searchCondition.Area != areaInfo.AreaName) continue;
                    //组合信息
                    IssuerArea i = new IssuerArea();
                    i.IssuerID = issuer.CorpID;
                    i.IssuerParentArea = areaInfo.AreaName;
                    i.IssuerParentName = parentCorp.CorpName;
                    tView.IssuerArea = i;
                }
                else
                {
                    //无父企业直接获得
                    IssuerArea i = new IssuerArea();
                    i.IssuerID = issuer.CorpID;
                    i.IssuerParentArea = issuer.AreaName;
                    if (searchCondition.Area != null && searchCondition.Area != "" && searchCondition.Area != issuer.AreaName) continue;
                    i.IssuerParentName = issuer.CorpName;
                    if (searchCondition.Office != null && searchCondition.Office != "" && searchCondition.Office != issuer.CorpName) continue;
                    tView.IssuerArea = i;
                }
                #endregion

                #region 补全
                var ParentType = typeof(Ticket);
                var Properties = ParentType.GetProperties();
                foreach (var Propertie in Properties)
                {
                    //循环遍历属性
                    if (Propertie.CanRead && Propertie.CanWrite)
                    {
                        //进行属性拷贝
                        Propertie.SetValue(tView, Propertie.GetValue(tic, null), null);
                    }
                }
                #endregion

                tList.Add(tView);
            }

            StiReport report = new StiReport();
            report.Load(StiNetCoreHelper.MapPath(this, "Report/OfficeReturnReport.mrt"));
            report.RegBusinessObject("Ticket", tList);

            return StiNetCoreViewer.GetReportResult(this, report);
        }

        public IActionResult UserVi()
        {
            var context = HttpContext.Request;
            return View();
        }
        public IActionResult UserReport()
        {
            //SQl取数据
            HaiwoContext context = new HaiwoContext();
            List<User> userList = context.Users.FromSql("select * from Users where UserName = 'dd'").ToList();
            List<UserViewModels> userLIst = new List<UserViewModels>();
            foreach (var c in userList)
            {
                UserViewModels s = new UserViewModels();
                s.UserID = c.UserId;
                s.UserName = c.UserName;
                s.CreateDate = c.CreatedTime;
                userLIst.Add(s);
            }

            //建报表
            StiReport report = new StiReport();
            report.Load(StiNetCoreHelper.MapPath(this, "Report/UserReport.mrt"));
            report.RegBusinessObject("User", userLIst);

            return StiNetCoreViewer.GetReportResult(this, report);
        }
        public IActionResult Chart()
        {
            return View();
        }
        public IActionResult ChartReport()
        {
            HaiwoContext context = new HaiwoContext();
            List<Corps> corpList = context.Corps.FromSql("select * from Corps where AreaName <> ''").ToList();
            List<SelectCorp> sList = new List<SelectCorp>();
            foreach (Corps c in corpList)
            {
                SelectCorp s = new SelectCorp();
                s.AreaName = c.AreaName;
                s.CorpName = c.CorpName;
                s.CorpStatus = c.CorpStatus;
                sList.Add(s);
            }

            var corps = corpList.GroupBy(corp => corp.AreaName);
            List<AreaStatistics> aList = new List<AreaStatistics>();
            foreach (var corp in corps)
            {
                AreaStatistics ars = new AreaStatistics();
                ars.Total = corp.Count();
                ars.AreaName = corp.First().AreaName;
                aList.Add(ars);
            }

            StiReport report = new StiReport();
            report.Load(StiNetCoreHelper.MapPath(this, "Report/DrillDownReportOnly.mrt"));
            report.RegBusinessObject("CorpList", aList);
            report.RegBusinessObject("InfoList", sList);

            return StiNetCoreViewer.GetReportResult(this, report);
        }
        public IActionResult ViewerEvent()
        {
            //点击下一页调用               
            return StiNetCoreViewer.ViewerEventResult(this);
        }
        public IActionResult ViewerInteraction()
        {
            StiRequestParams requestParams = StiNetCoreViewer.GetRequestParams(this);

            RouteValueDictionary routeValues =StiNetCoreViewer.GetRouteValues(this);

            //点击返回下一级报表
            switch (requestParams.Action)
            {
                case StiAction.DrillDown:
                    break;
            }
            return StiNetCoreViewer.InteractionResult(this);
        }
        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}































//var corps = context.Corps.Where(corp => corp.AreaName != null).GroupBy(corp => corp.AreaName);
//List<AreaStatistics> aList = new List<AreaStatistics>();
//foreach (var corp in corps)
//{
//    AreaStatistics ars = new AreaStatistics();
//    ars.Total = corp.Count();
//    ars.AreaName = corp.First().AreaName;
//    aList.Add(ars);
//}