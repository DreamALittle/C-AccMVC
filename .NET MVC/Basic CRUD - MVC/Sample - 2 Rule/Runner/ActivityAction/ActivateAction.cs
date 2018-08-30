using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Acctrue.CMC.Model.Code;
using Acctrue.CMC.Factory.Code;
using Acctrue.CMC.CodeBuild;
using System.Reflection;
using Acctrue.CMC.Model.Report;

namespace Acctrue.CMC.CodeService.ActivityAction
{
    /// <summary>
    /// 激活活动处理类
    /// </summary>
    public class ActivateAction : IActivityAction
    {
        /// <summary>
        /// 接口对应活动名称
        /// </summary>
        public string ActivityName { get{ return "激活"; } }

        /// <summary>
        /// 活动处理方法
        /// </summary>
        /// <param name="codeActive">码活动信息对象</param>
        /// <param name="activityCodes">码活动信息集合</param>
        public void ActionProcess<T>(CodeActive codeActive, List<T> activityCodes)where T: ActivityCodes
        {
            CodeApply codeApply = CodeApplyFactory.Instance.GetApply(codeActive.ApplyId);
            if (codeApply.ApplyType == 2)//外部平台码
            {
                string[] tempStrings = codeApply.CodeRulesIDs.Split(new char[] { '|' });
                int ruleSegId = 0;
                if (tempStrings.Length > 0 && int.TryParse(tempStrings[0], out ruleSegId))
                {
                    List<CodeRuleSeg> ruleSegs = CodeRuleSegFactory.Instance.GetByCodeRuleId(ruleSegId);
                    if (ruleSegs.Count > 0)
                    {
                        Type type = Assembly.Load(new AssemblyName("Acctrue.CMC.CodeBuild")).GetType(ruleSegs[0].ClassName);

                        IOtherFlatformSeg seg = (Activator.CreateInstance(type) as IOtherFlatformSeg);
                        seg.Initialize(Newtonsoft.Json.JsonConvert.DeserializeObject<List<Acctrue.CMC.Model.Code.ParameterInfo>>(ruleSegs[0].ClassArgs));
                        string mess = string.Empty;
                        if (seg.EcodeActivate(activityCodes.Select(s => s.Code).ToList(), codeActive, out mess))
                        {
                            
                        }
                        else
                        {
                            throw new Exception($"码激活任务Id:{codeActive.CodeActivityId}进行外部平台激活同步失败：{mess}");
                        }
                    }
                }
            }
        }
    }
}
