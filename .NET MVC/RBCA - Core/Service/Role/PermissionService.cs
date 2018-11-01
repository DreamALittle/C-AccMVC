using Acctrue.CMC.Interface.Role;
using Acctrue.CMC.Model.Role;
using Acctrue.CMC.Model.Users;
using Acctrue.Library.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Acctrue.CMC.Service.Users;
using System.Web;

namespace Acctrue.CMC.Service.Role
{
    public class PermissionService: BizBase, IPermissionInterface
    {
        public Dictionary<string, object> GetAllPermissions()
        {
            Condition where = Condition.Empty;

            List<Permission> infos = dbContext.From<Permission>().Where(where).Select().ToList();

            //此处有待确认是否按照序列分级排序返回！
            //List<Permission> LevelOne = infos.Where(c => c.PermissionParent == null).OrderBy(c => c.SeqNO).ToList();

            //foreach (Permission item in LevelOne)
            //{
            //    List<Permission> values = infos.Where(c => c.PermissionParent == item.PermissionID).OrderBy(c => c.SeqNO).ToList();
            //    dic.Add(item.PermissionName, values);
            //}


            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("msg","");
            dic.Add("code",0);
            dic.Add("data", infos);
            dic.Add("count",infos.Count);
            dic.Add("is",true);
            dic.Add("tip", "操作成功！");
            //string data = JsonConvert.SerializeObject(dic);
            return dic;
        }

        public string Update(Permission p)
        {

            if (p.PermissionID == 10086)
            {
                Permission newPermission = new Permission();
                newPermission.PermissionName = p.PermissionName;
                newPermission.PermissionStatus = 1;
                newPermission.PermissionParent = p.PermissionParent;
                newPermission.URL = p.URL;
                newPermission.SeqNO = p.SeqNO;

                try
                {
                    dbContext.Insert<Permission>(newPermission);
                    return "OK";
                }
                catch (Exception err)
                {
                    return err.Message;
                }
            }

            try
            {
                dbContext.Update<Permission>(p);
                return "OK";
            }
            catch(Exception e)
            {
                return e.Message;
            }
        }

        List<Permission> permissions = new List<Permission>();
        public string RemovePermission(ViewPermission vp)
        {
            SwitchModel(vp);
            try
            {
                foreach (Permission p in permissions)
                {
                    dbContext.Delete(p);
                }

                return "OK";
            }
            catch (Exception err)
            {
                return err.Message;
            }
        }
       
        private void SwitchModel(ViewPermission vp)
        {
            //将当前节点信息添加到记录中
            Permission p = new Permission();
            p.PermissionID = vp.PermissionID;
            p.PermissionName = vp.PermissionName;
            p.PermissionParent = vp.PermissionParent;
            p.PermissionStatus = vp.PermissionStatus;
            p.SeqNO = vp.SeqNO;
            p.URL = vp.URL;
            permissions.Add(p);

            if (vp.children.Count <= 0)
            {                
                return;
            }
            else
            {
                foreach (ViewPermission subVP in vp.children)
                {
                    SwitchModel(subVP);
                }              
            }
        }

        public Dictionary<string, object> GetUserPermissions(string UserName,int RoleID)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            if (UserName == "root")
            {
                Condition where = Condition.Empty;
                Permission[] infos = dbContext.From<Permission>().Where(where).Select().ToArray();

                #region
                //根据角色ID获取角色已有权限打钩
                where = CK.K["RoleID"].Eq(RoleID);
                List<RoleWithPermission> RPS = dbContext.From<RoleWithPermission>().Where(where).Select().ToList();
                int[] pID = new int[RPS.Count];
                for (int i = 0; i < RPS.Count; i++)
                {
                    pID[i] = RPS[i].PermissionID;
                }
                ViewCheckedPermission[] permissionsChecked = new ViewCheckedPermission[infos.Length];
                for (int index = 0;index < infos.Length;index++)
                {
                    ViewCheckedPermission vp = new ViewCheckedPermission();
                    vp.PermissionID = infos[index].PermissionID;
                    vp.PermissionName = infos[index].PermissionName;
                    vp.PermissionParent = infos[index].PermissionParent;

                    if (pID.Contains(vp.PermissionID))
                    {
                        vp.lay_is_checked = true;
                    }

                    permissionsChecked[index] = vp;
                }
                #endregion
                dic.Add("msg", "");
                dic.Add("code", 0);
                dic.Add("data", permissionsChecked);
                dic.Add("count", permissionsChecked.Length);
                dic.Add("is", true);
                dic.Add("tip", "操作成功！");
            }
            else
            {
                //找到用户ID
                UserService uservice = new UserService();
                UserInfo user = uservice.SearchUser(UserName);
                //查找用户所有角色ID
                Condition where = Condition.Empty;
                where = CK.K["UserID"].Eq(user.UserId);
                List<UserWithRole> UR = dbContext.From<UserWithRole>().Where(where).Select().ToList();
                int[] URID = new int[UR.Count] ;
                for (int index = 0; index < UR.Count; index++)
                {
                    URID[index] = UR[index].RoleID; //用户所有角色ID
                }
                //返回所有角色拥有权限ID
                List<RoleWithPermission> RP = new List<RoleWithPermission>();
                if (URID.Length > 0)
                {
                    for (int index = 0; index < URID.Length; index++)
                    {
                        where = CK.K["RoleId"].Eq(URID[index]);
                        RP.AddRange(dbContext.From<RoleWithPermission>().Where(where).Select().ToList());
                    }
                }
                List<int> PID = new List<int>();
                foreach (RoleWithPermission r in RP)
                {
                    if (PID.Contains(r.PermissionID))
                    {
                        continue;
                    }
                    else
                    {
                        PID.Add(r.PermissionID); //所有权限的ID
                    }
                }
                //根据权限ID获取所有权限内容
                Permission[] infos = new Permission[PID.Count];
                if (PID.Count > 0)
                {
                    for (int index = 0; index < PID.Count; index++)
                    {
                        where = CK.K["PermissionID"].Eq(PID[index]);
                        infos[index]=dbContext.From<Permission>().Where(where).Select().ToArray()[0];
                    }               
                }

                #region
                //根据角色ID获取角色已有权限打钩
                where = Condition.Empty;
                where = CK.K["RoleID"].Eq(RoleID);
                List<RoleWithPermission> RPS = dbContext.From<RoleWithPermission>().Where(where).Select().ToList();
                int[] pID = new int[RPS.Count];
                for (int i = 0; i < RPS.Count; i++)
                {
                    pID[i] = RPS[i].PermissionID;
                }
                ViewCheckedPermission[] permissionsChecked = new ViewCheckedPermission[infos.Length];
                for (int index = 0; index < infos.Length; index++)
                {
                    ViewCheckedPermission vp = new ViewCheckedPermission();
                    vp.PermissionID = infos[index].PermissionID;
                    vp.PermissionName = infos[index].PermissionName;
                    vp.PermissionParent = infos[index].PermissionParent;

                    if (pID.Contains(vp.PermissionID))
                    {
                        vp.lay_is_checked = true;
                    }

                    permissionsChecked[index] = vp;
                }
                #endregion
                dic.Add("msg", "");
                dic.Add("code", 0);
                dic.Add("data", permissionsChecked);
                dic.Add("count", permissionsChecked.Length);
                dic.Add("is", true);
                dic.Add("tip", "操作成功！");
            }
            return dic;
        }

        public List<ViewButtons> CheckButtonList(string UserName,string ParentURL)
        {
            #region 获取当前权限子Button对应ID
            Condition where = Condition.Empty;
            where = CK.K["URL"].MiddleLike(ParentURL);
            Permission currentVisitPermisiion = dbContext.From<Permission>().Where(where).Select().FirstOrDefault();
            where = CK.K["PermissionParent"].Eq(currentVisitPermisiion.PermissionID);
            List<Permission> currentViewButtons = dbContext.From<Permission>().Where(where).Select().ToList();
            #endregion

            #region 获取用户所有权限ID 
            //找到用户ID
            UserService uservice = new UserService();
            UserInfo user = uservice.SearchUser(UserName);
            //查找用户所有角色ID
            where = CK.K["UserID"].Eq(user.UserId);
            List<UserWithRole> UR = dbContext.From<UserWithRole>().Where(where).Select().ToList();
            //返回所有角色拥有权限ID
            List<RoleWithPermission> permissionIDs = new List<RoleWithPermission>();
            if (UR.Count > 0)
            {
                for (int index = 0; index < UR.Count; index++)
                {
                    where = CK.K["RoleId"].Eq(UR[index].RoleID);
                    permissionIDs.AddRange(dbContext.From<RoleWithPermission>().Where(where).Select().ToList());
                }
            }
            List<int> PID = new List<int>();
            foreach (RoleWithPermission r in permissionIDs)
            {
                if (PID.Contains(r.PermissionID))
                {
                    continue;
                }
                else
                {
                    PID.Add(r.PermissionID); //所有权限的ID
                }
            }
            #endregion

            #region 对比ID是否包含权限，返回T/F
            //PID 拥有的
            //currentViewButtons 当前页需要的
            List<ViewButtons> VB = new List<ViewButtons>();
            foreach (Permission p in currentViewButtons)
            {
                if (PID.Contains(p.PermissionID))
                {
                    ViewButtons v = new ViewButtons();
                    v.ButtonName = p.PermissionName;
                    v.Show = true;
                    VB.Add(v);
                }
            }
            #endregion

            return VB;
        }

        public bool DoubleCheckPermission(string UName, string PermissionUrl)
        {
            Condition where = Condition.Empty;
            where = CK.K["URL"].MiddleLike(PermissionUrl).Or(CK.K["URL"].LeftLike(PermissionUrl)).Or(CK.K["URL"].RightLike(PermissionUrl));
            Permission currentVisitPermisiion = dbContext.From<Permission>().Where(where).Select().FirstOrDefault();

            //找到用户ID
            UserService uservice = new UserService();
            UserInfo user = uservice.SearchUser(UName);
            //查找用户所有角色ID
            where = CK.K["UserID"].Eq(user.UserId);
            List<UserWithRole> UR = dbContext.From<UserWithRole>().Where(where).Select().ToList();
            //返回所有角色拥有权限ID
            List<RoleWithPermission> permissionIDs = new List<RoleWithPermission>();
            if (UR.Count > 0)
            {
                for (int index = 0; index < UR.Count; index++)
                {
                    where = CK.K["RoleId"].Eq(UR[index].RoleID);
                    permissionIDs.AddRange(dbContext.From<RoleWithPermission>().Where(where).Select().ToList());
                }
            }
           
            foreach (RoleWithPermission r in permissionIDs)
            {
                if (r.PermissionID == currentVisitPermisiion.PermissionID)
                {
                    return false;
                }
            }

            return true;
        }

    }
}
