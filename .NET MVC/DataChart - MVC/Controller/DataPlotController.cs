using Acctrue.CMC.Factory.Report;
using Acctrue.CMC.Model.Report;
using Acctrue.CMC.Web.Controllers.Request;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using Acctrue.CMC.Factory.Search;
using Acctrue.CMC.Model.Code;
using Acctrue.CMC.Model.ViewModel;

namespace Acctrue.CMC.Web.Controllers
{
    public class DataPlotController : ValuesController
    {
        /// <summary>
        /// 获取申请统计
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public CMCResponse GetApplyCount(TbRequest req)
        {
            List<int> arr = new List<int>();
            arr = SearchFactory.Instance.GetApplyApproveAndDeny(false);
            this.Response.data = arr;
            return this.Response;
        }
        /// <summary>
        /// 获取激活统计
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public CMCResponse GetActiveCount(TbRequest req)
        {
            List<int> arr = new List<int>();
            arr = SearchFactory.Instance.GetActivePlot(false);
            this.Response.data = arr;
            return this.Response;
        }
        /// <summary>
        /// 获取十分钟之内下载统计
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public CMCResponse GetDownloadTenMinCount(TbRequest req)
        {
            int arr = 0;
            arr = SearchFactory.Instance.GetDownloadTenMinCount(false);
            this.Response.data = arr;
            return this.Response;
        }
        /// <summary>
        /// 获取下载统计
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public CMCResponse GetTotalDownload(TbRequest req)
        {
            List<int> arr = new List<int>();
            arr = SearchFactory.Instance.GetTotalDownload(false);
            this.Response.data = arr;
            return this.Response;
        }
        /// <summary>
        /// 获取申请监控详细信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public CMCResponse CodeApplyPlot(Dictionary<string, object> dic)
        {
            List<CodeApply> arr = new List<CodeApply>();
            arr = SearchFactory.Instance.GetApplyPlot(dic);
            Response.data = arr;
            Response.count = arr.Count;
            return Response;
        }
        /// <summary>
        /// 获取上传监控详细信息
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        [HttpPost]
        public CMCResponse CodeActivePlot(Dictionary<string, object> dic)
        {
            List<CodeActive> arr = new List<CodeActive>();
            arr = SearchFactory.Instance.GetActivePlot(dic);
            Response.data = arr;
            Response.count = arr.Count;
            return Response;
        }
        /// <summary>
        /// 10分钟内下载统计详细信息
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        [HttpPost]
        public CMCResponse GetTenMinDownload(Dictionary<string, object> dic)
        {
            List<TenMinModel> arr = new List<TenMinModel>();
            arr = SearchFactory.Instance.GetTenMinDownload(dic);
            Response.data = arr;
            Response.count = arr.Count;
            return Response;
        }
        /// <summary>
        /// 获取下载分类统计
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        [HttpPost]
        public CMCResponse GetDownloadType(Dictionary<string, object> dic)
        {
            List<DownloadType> arr = new List<DownloadType>();
            arr = SearchFactory.Instance.GetDownloadType(dic);
            Response.data = arr;
            Response.count = arr.Count;
            return Response;
        }
    }
}
