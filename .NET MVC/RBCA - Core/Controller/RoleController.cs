using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Acctrue.CMC.Factory.Role;
using Newtonsoft.Json;
using System.Text;
using Acctrue.CMC.Web.Controllers.Request;
using Acctrue.CMC.Web.Controllers.WebApi;

namespace Acctrue.CMC.Web.Controllers
{
    public class RoleController : ValuesController
    {
        /// <summary>
        /// 获取系统所有角色
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public CMCResponse GetRoles(TbRequest req)
        {
            int count = 0;
            Dictionary<string, object> dic = JsonConvert.DeserializeObject<Dictionary<string, object>>(req.Data);
            dic.Add("Page",req.Page);
            dic.Add("Limit",req.Limit);
            Response.data = RoleFactory.Instance.GetAllRoles(dic, out count);
            Response.count = count;
            return Response;
        }
        /// <summary>
        /// 添加新角色
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        [HttpPost]
        public string AddNewRole(Dictionary<string, object> dic)
        {
            if (ApiHelper.UserPermissionCheck())
            {
                return "当前登录用户无权限访问该功能";
            }

            string res = RoleFactory.Instance.AddRole(dic);
            return res;
        }

        [HttpPost]
        public string DisableRole(TbRequest req)
        {
            if (ApiHelper.UserPermissionCheck())
            {
                return "当前登录用户无权限访问该功能";
            }

            int RoleID = Convert.ToInt32(req.Data);
            string res = RoleFactory.Instance.DisableRole(RoleID);
            return res;
        }
    }
}
