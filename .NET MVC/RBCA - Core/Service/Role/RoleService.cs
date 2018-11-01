using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Acctrue.CMC.Interface.Role;
using Acctrue.Library.Data;
using Acctrue.CMC.Model.Role;
using Newtonsoft.Json;
using Acctrue.CMC.Model.Users;

namespace Acctrue.CMC.Service.Role
{
    public class RoleService: BizBase,IRoleInterface
    {
        /// <summary>
        /// 获取所有用户组
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public List<UserRole> GetAllRoles(Dictionary<string, object> dic, out int count)
        {
            int pageIndex = Convert.ToInt32(dic["Page"]);
            int pageCount = Convert.ToInt32(dic["Limit"]);
            string userName = (string)dic["UserName"];

            List<UserRole> roleList = new List<UserRole>();
            Condition where = Condition.Empty;
            where = where & CK.K["RoleStatus"].Eq(1);
            where = where & CK.K["Creator"].Eq(userName).Or(CK.K["Owner"].Eq(userName));
            roleList = dbContext.From<UserRole>().Where(where).Select().OrderByDescending(r=>r.RoleID).Skip((pageIndex - 1) * pageCount).Take(pageCount).ToList();
            count = roleList.Count;
            return roleList;
        }

        public string AddRole(Dictionary<string, object> dic)
        {
            UserRole role = new UserRole();

            role.RoleName = (string)dic["RoleName"];
            role.RoleStatus = Convert.ToInt32(dic["RoleStatus"]);
            role.Owner = (string)dic["Owner"];
            role.OwnerID = Convert.ToInt32(dic["OwnerID"]);
            role.Creator = (string)dic["Creator"];
            role.CreatedTime = DateTime.Now;
            //role.CreatorID = Convert.ToInt32(dic["CreatorID"]);

            try
            {
                dbContext.Insert<UserRole>(role);
                return "OK";
            }
            catch(Exception err)
            {
                return err.Message;
            }
        }

        public string AddRolePermission(int RoleID, List<ViewPermission> VPS)
        {
            try
            {
                //清空旧数据（未来可改进！取非集节省空间）
                Condition where = Condition.Empty;
                where &= CK.K["RoleID"].Eq(RoleID);
                dbContext.Delete<RoleWithPermission>(where);

                foreach (ViewPermission VP in VPS)
                {
                    RoleWithPermission RP = new RoleWithPermission();
                    RP.RoleID = RoleID;
                    RP.PermissionID = VP.PermissionID;
                    dbContext.Insert<RoleWithPermission>(RP);
                }

                return "Yes";
            }
            catch (Exception err)
            {
                return err.Message;
            }        
        }

        public Dictionary<string, object> GetRoleUser(string UserName, int RoleID)
        {
            Condition where = Condition.Empty;
            where = where & CK.K["Creator"].MiddleLike(UserName).Or(CK.K["Creator"].LeftLike(UserName)).Or(CK.K["Creator"].RightLike(UserName));
            UserInfo[] infos = dbContext.From<UserInfo>().Where(where).OrderByDescending(x=>x.UserId).Select().ToArray();

            //返回前台打勾用户
            where = Condition.Empty;
            where = CK.K["RoleID"].Eq(RoleID);
            UserWithRole[] UR = dbContext.From<UserWithRole>().Where(where).Select().ToArray();
            int[] UID = new int[UR.Length];
            for (int index = 0; index < UR.Length; index++)
            {
                UID[index] = UR[index].UserID;
            }
            RoleUserChecked[] RUC = new RoleUserChecked[infos.Length];
            for (int index = 0; index < infos.Length; index++)
            {
                RoleUserChecked r = new RoleUserChecked();
                r.UserID = infos[index].UserId;
                r.UserName = infos[index].UserName;
                if (UID.Contains(infos[index].UserId))
                {
                    r.lay_is_checked = true;
                }
                RUC[index] = r;
            }

           
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("msg", "");
            dic.Add("code", 0);
            dic.Add("data", RUC);
            dic.Add("count", RUC.Length);
            dic.Add("is", true);
            dic.Add("tip", "操作成功！");
            return dic;
        }

        public string AddRoleUser(int RoleID, List<ViewUser> VU)
        {
            try
            {
                //清空旧数据（未来可改进！取非集节省空间）
                Condition where = Condition.Empty;
                where &= CK.K["RoleID"].Eq(RoleID);
                dbContext.Delete<UserWithRole>(where);

                foreach (ViewUser VP in VU)
                {
                    UserWithRole UR = new UserWithRole();
                    UR.RoleID = RoleID;
                    UR.UserID = VP.UserId;
                    dbContext.Insert<UserWithRole>(UR);
                }

                return "Yes";
            }
            catch (Exception err)
            {
                return err.Message;
            }
        }

        public string DisableRole(int RoleID)
        {
            try
            {
                dbContext.Delete<UserRole>(CK.K["RoleID"].Eq(RoleID));
                return "Yes";
            }
            catch (Exception err)
            {
                return err.Message;
            }
        }
    }
}
