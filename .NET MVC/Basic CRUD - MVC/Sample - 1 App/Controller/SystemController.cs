using Acctrue.CMC.Factory.Log;
using Acctrue.CMC.Factory.Systems;
using Acctrue.CMC.Factory.Users;
using Acctrue.CMC.Interface.Common;
using Acctrue.CMC.Model.Systems;
using Acctrue.CMC.Model.Users;
using Acctrue.CMC.Web.Controllers.Request;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Web.Http;
using MyResponse = Acctrue.CMC.Web.Controllers.Response;

namespace Acctrue.CMC.Web.Controllers
{
    public class SystemController : ValuesController,IBaseInterface
    {

        // POST api/values
        [HttpPost]
        [CMC(InterfaceName = "添加应用程序", Open = false)]
        public CMCResponse Add(CMCRequest req)
        {
            string msg = "";
            UserInfo user = UserFactory.Instance.SearchUser(this.UserName);
            bool isAdmin = false;
            if (user.UserType == UserType.Administrator)
                isAdmin = true;
            try
            {
                SystemFactory.Instance.Add(req.Data, isAdmin);
            }
            catch(Exception ex)
            {
                if (ex.Message.Split(':')[0] == "ORA-00001")
                {
                    throw new Exception("请不要插入重复的 AppID!");
                }
                else if (ex.Message.Contains( "违反了 PRIMARY KEY 约束"))
                {
                    throw new Exception("请不要插入重复的 AppID!");
                }
                else
                {
                    throw new Exception("新增APP失败" + ex.Message);
                }
            }
            Response.message = msg;
            Log("{\"LogMenu\":\"系统管理\",\"LogAction\":\"添加应用程序\"}", this.UserName, this.IP);
            return Response;
        }
        //更新应用
        public CMCResponse Update(CMCRequest req) {
            AppSettingInfo info = JsonConvert.DeserializeObject<AppSettingInfo>(req.Data);
            SystemFactory.Instance.Update(info);
            Log("{\"LogMenu\":\"系统管理\",\"LogAction\":\"修改应用程序["+ info .AppSettingID+ "]\"}", this.UserName, this.IP);
            return Response;
        }

        /// <summary>
        /// 获取应用配置列表
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        [CMC(InterfaceName = "获取应用程序接口", Open = false)]
        public HttpResponseMessage SearchSystemAppSettings(dynamic obj)
        {
            int count = 0;
            AppSettingInfo[] appInfos = SystemFactory.Instance.SearchSystemAppSettings(obj, out count);
            string respon = "{ \"code\":0,\"msg\":1,\"count\":" + count + ",\"data\":" + JsonConvert.SerializeObject(appInfos).ToString() + "}";
            HttpResponseMessage result = new HttpResponseMessage { Content = new StringContent(respon, Encoding.GetEncoding("UTF-8"), "application/json") };
            return result;
        }

        public CMCResponse SearchSystemAppSetting(dynamic obj)
        {
            AppSettingInfo appInfo = SystemFactory.Instance.GetUseAppConfig(Convert.ToInt32(obj));
            Response.data = appInfo;
            return Response;
        }

        [HttpPost]
        public CMCResponse AddAppInterFaceInfos(List<AppInterFaceInfo> dic)
        {
            SystemFactory.Instance.AddAppInterFaceInfos(dic);
            return Response;
        }
        /// <summary>
        /// 获取已经配置接口信息
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        public CMCResponse SearchAppInterFaceInfos(Dictionary<string, object> dic)
        {
            //已经配置的接口信息
            AppInterFaceInfo[] appInfos = SystemFactory.Instance.SearchAppInterFaceInfos(dic);
            //所有接口信息

            Response.data = appInfos;
            return Response;
        }

        [HttpPost]
        public CMCResponse SearchInterFaces(Dictionary<string, object> requestdic)
        {
            List<Dictionary<string, string>> methods = new List<Dictionary<string, string>>();
            List<string> addedInterface = new List<string>();
            try
            {
                //已经配置的接口信息
                foreach (AppInterFaceInfo info in SystemFactory.Instance.SearchAppInterFaceInfos(requestdic))
                {
                    addedInterface.Add(info.InterfaceFunctionName);
                }
                //所有接口信息
                Assembly assembly = Assembly.Load("Acctrue.CMC.Web");
                Type[] types = assembly.GetTypes().Where(c=>c.FullName.Contains("Acctrue.CMC.Web.Controllers")).ToArray();
                foreach (Type type in types)
                {
                    if (!typeof(IBaseInterface).IsAssignableFrom(type))
                        continue;
                    MethodInfo[] meths = type.GetMethods();
                    foreach (MethodInfo meth in meths)
                    {
                        CMCAttribute att = meth.GetCustomAttribute(typeof(CMCAttribute)) as CMCAttribute;
                        if (att == null)
                            continue;
                        if (meth.CustomAttributes.Count() != 0 && att.Open)
                        {
                            if (att.Open)
                            {
                                //构建参数
                                Dictionary<string, string> dic = new Dictionary<string, string>();
                                dic.Add("namespace", type.Namespace);
                                dic.Add("interfacefunctionname", att.InterfaceName);
                                dic.Add("functionname", type.Name.Replace("Controller", "") + "/" + meth.Name);
                                //判断是否已经包含
                                if (addedInterface.Contains(att.InterfaceName))
                                {
                                    dic.Add("LAY_CHECKED", "Added");
                                }
                                else
                                {
                                    dic.Add("LAY_CHECKED", "None");
                                }
                                methods.Add(dic);
                            }
                        }
                    }
                }
            }
            catch(Exception)
            {

            }
            Response.data = methods;
            return Response;
        }

        [HttpPost]
        public CMCResponse RemoveAppInterface(Dictionary<string, object> dic) {
            SystemFactory.Instance.RemoveAppInterface(dic);
            return Response;
        }
        public CMCResponse RemoveAppSetting(Dictionary<string, object> dic) {
            SystemFactory.Instance.RemoveAppSetting(dic);
            Log("{\"LogMenu\":\"系统管理\",\"LogAction\":\"删除应用程序\"}", this.UserName, this.IP);
            return Response;
        }

        [HttpPost]
        public CMCResponse SearchLogs(TbRequest req)
        {
            Dictionary<string, object> dic = JsonConvert.DeserializeObject<Dictionary<string, object>>(req.Data);
            int count = 0;
            Response.data = LogFactory.Instance.SearchLog(dic ,req.Page,req.Limit,out count);
            Response.count = count;
            return Response;
        }


    }
}
