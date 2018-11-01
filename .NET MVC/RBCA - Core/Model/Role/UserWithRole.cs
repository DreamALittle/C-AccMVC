using Acctrue.Library.Data.Definition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Acctrue.CMC.Model.Role
{
    [DbTable("UserRole")]
    [Serializable()]
    [DataContract()]
    public class UserWithRole
    {
        [DataMember()]
        [DbKey(IsDbGenerate = true)]
        public int ID { get; set; }
        [DataMember()]
        public int UserID { get; set; }
        [DataMember()]
        public int RoleID { get; set; }
    }
}
