using Acctrue.CMC.Interface.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Acctrue.CMC.Model.Users;
using Acctrue.Library.Data;
using Acctrue.CMC.Model.Code;
using Newtonsoft.Json;

namespace Acctrue.CMC.Service.Code
{
    public partial class CodeRuleService : BizBase,ICodeRuleService
    {
        public List<CodeRule> GetAllCodeRules()
        {
            Condition where = Condition.Empty & CK.K["RuleStatus"].Gt(0);
            List<CodeRule> ruleList = dbContext.From<CodeRule>().Where(where).Select().ToList();
            ruleList.Sort((x, y) => { return x.CodeRuleId.CompareTo(y.CodeRuleId); });
            return ruleList;
        }
        /// <summary>
        /// 获取查询申请记录
        /// </summary>
        /// <param name="applyId">申请记录Id</param>
        /// <returns></returns>

        public CodeRule GetCodeRule(int codeRuleId)
        {
            Condition where = Condition.Empty & CK.K["CodeRuleId"].Eq(codeRuleId);
            CodeRule rule = dbContext.From<CodeRule>().Where(where).Select().FirstOrDefault();
            return rule;
        }

        public int Add(CodeRule info)
        {
            info.CreatedTime = info.ModifiedTime = DateTime.Now;
            dbContext.Insert(info);
            return info.CodeRuleId;
        }
        /// <summary>
        /// 查找码规则
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public List<CodeRule> SearchCodeRuleConfig(dynamic obj, out int count)
        {
            Dictionary<string, object> dic = JsonConvert.DeserializeObject<Dictionary<string, object>>((string)obj["paramObj"]);

            Condition where = Condition.Empty;
            if (dic.Keys.Contains("CodeRuleId")) 
            {
                if (dic["CodeRuleId"] != null && (string)dic["CodeRuleId"] != "")
                {
                    where &= CK.K["CodeRuleId"].Eq(int.Parse((string)dic["CodeRuleId"]));
                }
            }
            if (dic.Keys.Contains("RuleName"))
            {
                if (dic["RuleName"] != null && (string)dic["RuleName"] != "")
                {
                    where |= CK.K["CodeRuleName"].Eq((string)dic["RuleName"]);
                }
            }
            if (dic.Keys.Contains("Creater"))
            {
                if (dic["Creater"] != null && (string)dic["Creater"] != "")
                {
                    where |= CK.K["Creator"].Eq((string)dic["Creater"]);
                }
            }
            if (dic.Keys.Contains("RuleDescription"))
            {
                if (dic["RuleDescription"] != null && (string)dic["RuleDescription"] != "")
                {
                    where |= CK.K["Description"].MiddleLike((string)dic["RuleDescription"]);
                }
            }
            int page = obj.page;
            int limit = obj.limit;
            var recordCount = dbContext.From<CodeRule>().Where(where).GetCount();
            count = (int)recordCount;

            if (page > 0)
            {
                List<CodeRule> RList = dbContext.From<CodeRule>().Where(where).Select().Skip((page - 1) * limit).Take(limit).ToList();
                RList.Sort((x, y) => { return DateTime.Compare(y.CreatedTime, x.CreatedTime);});
                return RList;
            }
            else
            {
                List<CodeRule> RList = dbContext.From<CodeRule>().Where(where).Select().ToList();
                RList.Sort((x, y) => { return DateTime.Compare(y.CreatedTime, x.CreatedTime); });
                return RList;
            }


        }

        public CodeRule Get(int codeRuleId)
        {
            CodeRule[] datas = dbContext.From<CodeRule>().Where(CK.K["CodeRuleId"].Eq(codeRuleId)).Select().ToArray();
            if (datas.Length > 0)
                return datas[0];
            return null;
        }

        public void Update(CodeRule info)
        {
            Acctrue.Library.Data.SqlEntry.KeyValueCollection keys = new Acctrue.Library.Data.SqlEntry.KeyValueCollection();
            info.ModifiedTime = DateTime.Now;
            info.Modifier = "test";
            keys.Add(new Acctrue.Library.Data.SqlEntry.KeyValue("CodeRuleName", info.CodeRuleName));
            keys.Add(new Acctrue.Library.Data.SqlEntry.KeyValue("RuleStatus", info.RuleStatus));
            keys.Add(new Acctrue.Library.Data.SqlEntry.KeyValue("CodeLength", info.CodeLength));
            keys.Add(new Acctrue.Library.Data.SqlEntry.KeyValue("CodeVersions", info.CodeVersions != null ? info.CodeVersions : ""));
            keys.Add(new Acctrue.Library.Data.SqlEntry.KeyValue("CodeArguments", info.CodeArguments != null ? info.CodeArguments : ""));
            keys.Add(new Acctrue.Library.Data.SqlEntry.KeyValue("CodeExtensions", info.CodeExtensions != null ? info.CodeExtensions : ""));
            keys.Add(new Acctrue.Library.Data.SqlEntry.KeyValue("CodeRuleType", info.CodeRuleType));
            keys.Add(new Acctrue.Library.Data.SqlEntry.KeyValue("Description", info.Description));
            keys.Add(new Acctrue.Library.Data.SqlEntry.KeyValue("IsMaskCode", info.IsMaskCode));
            keys.Add(new Acctrue.Library.Data.SqlEntry.KeyValue("IsParseCode", info.IsParseCode));
            keys.Add(new Acctrue.Library.Data.SqlEntry.KeyValue("ModifiedTime", info.ModifiedTime));
            keys.Add(new Acctrue.Library.Data.SqlEntry.KeyValue("Modifier", info.Modifier));
            if (info.CodeRuleId == 0)
            {
                info.CreatedTime = DateTime.Now;
                info.Creator = "test";
                dbContext.Insert<CodeRule>(info);
            }
            else
            {
                dbContext.Update<CodeRule>(keys, CK.K["CodeRuleId"].Eq(info.CodeRuleId));
            }
        }


        public CodeRuleSeg[] GetCodeRuleSegConfig(int CodeRuleId)
        {
            CodeRuleSeg[] datas = dbContext.From<CodeRuleSeg>().Where(CK.K["CodeRuleId"].Eq(CodeRuleId)).Select().ToArray();
            return datas;
        }
    }
}
