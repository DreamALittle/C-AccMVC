using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acctrue.CMC.Model.Role
{
    public class ViewCheckedPermission
    {
        //原始字段
        #region
        public int PermissionID { get; set; }
        public string PermissionName { get; set; }
        public int? PermissionParent { get; set; }
        #endregion

        //选中字段
        #region
        public Boolean lay_is_checked { get; set; }
        #endregion
    }
}
