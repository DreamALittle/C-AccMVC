using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Acctrue.Library.Data;
using Acctrue.CMC.Interface.Code;
using Acctrue.CMC.Model.Code;
using Newtonsoft.Json;

namespace Acctrue.CMC.Service.CodeRules
{
    public partial class CodeRuleSegService : BizBase, ICodeRuleSegService
    {
        public List<CodeRuleSeg> GetByCodeRuleId(int codeRuleId)
        {
            List<CodeRuleSeg> datas = dbContext.From<CodeRuleSeg>().Where(CK.K["CodeRuleId"].Eq(codeRuleId)).OrderBy(s => s.SegIndex).Select().ToList();
            return datas;
        }

        public void Add(CodeRuleSeg info)
        {
            dbContext.Insert(info);
        }

        public void Delete(CodeRuleSeg info)
        {
            dbContext.Delete(info);
        }

        public void Update(CodeRuleSeg info)
        {
            Acctrue.Library.Data.SqlEntry.KeyValueCollection keys = new Acctrue.Library.Data.SqlEntry.KeyValueCollection();
            keys.Add(new Acctrue.Library.Data.SqlEntry.KeyValue("ClassArgs", info.ClassArgs));
            keys.Add(new Acctrue.Library.Data.SqlEntry.KeyValue("ClassName", info.ClassName));
            keys.Add(new Acctrue.Library.Data.SqlEntry.KeyValue("CodeRuleId", info.CodeRuleId));
            keys.Add(new Acctrue.Library.Data.SqlEntry.KeyValue("OutArgs", info.OutArgs));
            keys.Add(new Acctrue.Library.Data.SqlEntry.KeyValue("SegIndex", info.SegIndex));
            keys.Add(new Acctrue.Library.Data.SqlEntry.KeyValue("SegLength", info.SegLength));
            if (info.SegId == 0)
            {
                dbContext.Insert<CodeRuleSeg>(info);
            }
            else
            {
                dbContext.Update<CodeRuleSeg>(keys, CK.K["SegId"].Eq(info.SegId));
            }
        }
    }

}
