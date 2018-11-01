using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Acctrue.CMC.Model.Users;

namespace Acctrue.CMC.Model.Role
{
    public class ViewUser
    {
        #region 原有字段
        public int UserId { get; set;}
        public string UserName{get;set;  }
        #endregion

        #region 视图内容
        public Boolean lay_is_open { get; set; }
        public Boolean lay_is_show { get; set; }
        public Boolean lay_che_disabled { get; set; }
        public int lay_level { get; set; }
        //子树
        public List<ViewPermission> children { get; set; }
        #endregion
    }
}
