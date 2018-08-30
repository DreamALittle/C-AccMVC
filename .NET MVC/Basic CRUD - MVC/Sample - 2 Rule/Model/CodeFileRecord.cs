using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Acctrue.Library.Data.Definition;
using System.Runtime.Serialization;

namespace Acctrue.CMC.Model.Code
{
    [DbTable("CodeFiledRecord")]
    [DataContract()]
    [Serializable()]
    public partial class CodeFileRecord
    {
        [DataMember()]
        [DbKey]
        public int FileRecordId { get; set; }
        [DataMember()]
        public int ApplyId { get; set; }
        [DataMember()]
        [AllowNull]
        public byte[] Content { get; set; }
    }
}
