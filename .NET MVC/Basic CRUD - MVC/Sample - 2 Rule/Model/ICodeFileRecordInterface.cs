using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acctrue.CMC.Model.Code
{
    public interface ICodeFileRecord
    {
        int FileRecordId { get; set; }
        int ApplyId { get; set; }
        //string Content { get; set; }
    }
}
