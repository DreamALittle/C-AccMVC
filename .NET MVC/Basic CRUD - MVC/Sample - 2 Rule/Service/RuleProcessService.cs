using Acctrue.CMC.Interface.Code;
using Acctrue.CMC.Model.Code;
using Acctrue.Library.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acctrue.CMC.Service.Code
{

    public class RuleProcessService : BizBase,IRuleProcess
    {
        /// <summary>
        /// 添加码规则处理信息
        /// </summary>
        /// <param name="info">码规则处理信息</param>
        /// <returns></returns>
        public void Add(RuleProcess info)
        {
            dbContext.Insert(info);
        }

        /// <summary>
        /// 获取码规则处理信息
        /// </summary>
        /// <param name="codeRuleId">码规则Id</param>
        /// <param name="featureTag">特征标签</param>
        /// <returns></returns>
        public RuleProcess GetRuleProcess(int codeRuleId, string featureTag)
        {
            Condition where = Condition.Empty & CK.K["CodeRuleId"].Eq(codeRuleId) & CK.K["FeatureTag"].Eq(featureTag);
            RuleProcess rule = dbContext.From<RuleProcess>().Where(where).Select().FirstOrDefault();
            return rule;
        }

        /// <summary>
        /// 修改码规则处理信息
        /// </summary>
        /// <param name="info">码规则处理信息</param>
        /// <returns></returns>
        public void Update(RuleProcess info)
        {
            Acctrue.Library.Data.SqlEntry.KeyValueCollection keys = new Acctrue.Library.Data.SqlEntry.KeyValueCollection();
            keys.Add(new Acctrue.Library.Data.SqlEntry.KeyValue("LastCode", info.LastCode));
            dbContext.Update<RuleProcess>(keys, CK.K["CodeRuleId"].Eq(info.CodeRuleId)& CK.K["FeatureTag"].Eq(info.FeatureTag));
        }
    }
}
