using Acctrue.CMC.Factory.Log;
using Acctrue.CMC.Factory.Menu;
using Acctrue.CMC.Web.Controllers.Request;
using Acctrue.CMC.Web.Controllers.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web;
using System.Web.Http;

namespace Acctrue.CMC.Web.Controllers
{
    public class ValuesController : ApiController
    {
        private string name;
        public string UserName
        {
            get
            {
                if (name == null)
                {
                    name = HttpContext.Current.Session["UserName"] !=null? (string)HttpContext.Current.Session["UserName"] : "Unknown";
                }
                return name;
            }
            set
            { name = value; }
        }

        private string ip;

        public string IP
        {
            get
            {
                if (ip == null)
                {
                    ip = ApiHelper.GetIPAddress().IsNullOrEmpty() ? "Unknown":ApiHelper.GetIPAddress();
                }
                return ip;
            }
            set
            {
                ip = value;
            }
            
         }
        private CMCResponse response { get; set; }
        public CMCResponse Response {
            get
            {
                if (response == null)
                {
                    response = new CMCResponse();
                }
                return response;
            }
            set
            {
                response = value;
            }
        }

        public T DicToObject<T>(Dictionary<string, object> dic) where T : new()
        {
            var md = new T();
            //CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            //TextInfo textInfo = cultureInfo.TextInfo;
            foreach (var d in dic)
            {
                //var filed = textInfo.ToTitleCase(d.Key);
                try
                {
                    var value = d.Value;
                    if (md.GetType().GetProperty(d.Key).PropertyType.GenericTypeArguments.Length > 0)
                    {
                        Type ty = md.GetType().GetProperty(d.Key).PropertyType.GenericTypeArguments[0];
                        if (ty == typeof(DateTime))
                        {
                            md.GetType().GetProperty(d.Key).SetValue(md, Convert.ToDateTime(value));
                            continue;
                        }
                    }
                    if (md.GetType().GetProperty(d.Key).PropertyType == typeof(Int32) || md.GetType().GetProperty(d.Key).PropertyType.IsEnum)
                    {
                        md.GetType().GetProperty(d.Key).SetValue(md, Convert.ToInt32(value));
                        continue;
                    }
                    md.GetType().GetProperty(d.Key).SetValue(md, value);

                }
                catch (Exception ex)
                {

                }
            }
            return md;
        }

        [HttpPost]
        public void Log(dynamic obj) {
            this.Log((string)obj.data, UserName,IP);
        }

        [HttpPost]
        public CMCResponse Menus()
        {
            Response.data = MenuFactory.Instance.SearchMenu();
            return Response;
        }

        protected void Log(string data,string userName ,string ip) {
            LogFactory.Instance.Add(data, userName,ip);
        }
    }

    public class CMCResponse
    {
        /// <summary>
        /// 数据体
        /// </summary>
        public object data { get; set; }
        /// <summary>
        /// 错误消息
        /// </summary>
        public string message { get; set; }
        /// <summary>
        /// 状态 0：成功 1：失败
        /// </summary>
        public int status { get; set; }
        public int count { get; set; }
    }

   
}
