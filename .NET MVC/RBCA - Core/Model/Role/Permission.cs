using Acctrue.Library.Data.Definition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Acctrue.CMC.Model.Role
{
    [DbTable("Permission")]
    [Serializable()]
    [DataContract()]
    public class Permission
    {
        //权限ID
        [DataMember()]
        [DbKey(IsDbGenerate = true)]
        public int PermissionID { get; set; }
        //权限名称
        [DataMember()]
        public string PermissionName { get; set; }
        //权限状态
        [DataMember()]
        public int PermissionStatus { get; set; }
        //权限父节点
        [DataMember()]
        public int? PermissionParent { get; set; }
        //对应网址
        [DataMember()]
        [AllowNull]
        public string URL { get; set; }
        //排序
        [DataMember()]
        public int SeqNO { get; set; }
    }
}
