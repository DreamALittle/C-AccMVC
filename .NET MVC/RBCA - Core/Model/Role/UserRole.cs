using Acctrue.Library.Data.Definition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Acctrue.CMC.Model.Role
{
    [DbTable("Role")]
    [Serializable()]
    [DataContract()]
    public class UserRole
    {
        [DataMember()]
        [DbKey]
        //角色ID
        public int RoleID { get; set; }
        //角色名
        [DataMember()]
        public string RoleName { get; set; }
        //角色状态
        [DataMember()]
        public int RoleStatus { get; set; }
        //创建者
        [DataMember()]
        public string Creator { get; set; }
        [DataMember()]
        public int CreatorID { get; set; }
        //所有者
        [DataMember()]
        public string Owner { get; set; }
        [DataMember()]
        public int OwnerID { get; set; }
        //创建时间
        [DataMember()]
        public DateTime CreatedTime { get; set; }
    }
}
