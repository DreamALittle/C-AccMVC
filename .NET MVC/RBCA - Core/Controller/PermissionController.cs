using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Acctrue.CMC.Factory.Role;
using Acctrue.CMC.Web.Controllers.Request;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Acctrue.CMC.Model.Role;
using Acctrue.CMC.Web.Controllers.WebApi;

namespace Acctrue.CMC.Web.Controllers
{
    public class PermissionController : ValuesController
    {
        [HttpGet]
        public Dictionary<string, object> GetPermission()
        {
            Dictionary<string, object> res = PermissionFactory.Instance.GetAllPermissions();
            return res;
        }

        [HttpPost]
        public string SaveChange(TbRequest req)
        {
            Permission P = JsonConvert.DeserializeObject<Permission>(req.Data);
            string res = PermissionFactory.Instance.Update(P);
            return res;   
        }

        [HttpPost]
        public string DeletePermission(TbRequest req)
        {
            ViewPermission VP= JsonConvert.DeserializeObject<ViewPermission>(req.Data);
            string res = PermissionFactory.Instance.RemovePermission(VP);
            return res;
        }

        [HttpGet]
        public Dictionary<string, object> GetUserPermission()
        {
            string userName = HttpContext.Current.Request.Params["Name"];
            int RoleID =Convert.ToInt32( HttpContext.Current.Request.Params["RoleID"]);
            Dictionary<string, object> res = PermissionFactory.Instance.GetUserPermissions(userName,RoleID);
            return res;
        }

        [HttpPost]
        public string SavePermission(TbRequest request)
        {
            if (ApiHelper.UserPermissionCheck())
            {
                return "当前登录用户无权限访问该功能";
            }

            List<ViewPermission> VPS = new List<ViewPermission>();
            JArray jarr = JArray.Parse(request.Data);
            foreach (JObject js in jarr)
            {
                VPS.Add(js.ToObject<ViewPermission>());
            }

            int RoleID = request.Page;

            string res = RoleFactory.Instance.AddRolePermission(RoleID,VPS);

            return res;
        }

        [HttpGet]
        public Dictionary<string, object> GetRoleUser(TbRequest request)
        {
            string userName = HttpContext.Current.Request.Params["Name"];
            int RoleID = Convert.ToInt32(HttpContext.Current.Request.Params["RoleID"]);
            Dictionary<string, object> res = RoleFactory.Instance.GetRoleUser(userName,RoleID);
            return res;
        }

        [HttpPost]
        public string SaveUsers(TbRequest request)
        {
            if (ApiHelper.UserPermissionCheck())
            {
                return "当前登录用户无权限访问该功能";
            }

            List<ViewUser> VU = new List<ViewUser>();
            JArray jarr = JArray.Parse(request.Data);
            foreach (JObject js in jarr)
            {
                VU.Add(js.ToObject<ViewUser>());
            }

            int RoleID = request.Page;

            string res = RoleFactory.Instance.AddRoleUser(RoleID, VU);

            return res;
        }
    }
}
