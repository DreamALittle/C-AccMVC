using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acctrue.CMC.Model.Role
{
    public class ViewPermission
    {
        //原始字段
        #region
        public int PermissionID { get; set; }
        public string PermissionName { get; set; }
        public int PermissionStatus { get; set; }
        public int? PermissionParent { get; set; }
        public string URL { get; set; }
        public int SeqNO { get; set; }
        #endregion

        //页面自带字段
        #region
        public Boolean lay_is_open { get; set; }
        public Boolean lay_is_show { get; set; }
        public Boolean lay_che_disabled { get; set; }
        public int lay_level { get; set; }
        public int lay_table_index { get; set; }

        //子树
        public List<ViewPermission> children { get; set; }
        #endregion
    }
}
