using Acctrue.CMC.CodeBuild;
using Acctrue.CMC.CodeBuild.SegBuilder;
using Acctrue.CMC.Factory.Code;
using Acctrue.CMC.Model.Code;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Acctrue.CMC.CodeService
{
    public class CodeWork
    {
        /// <summary>
        /// 码活动处理工作
        /// </summary>
        /// <param name="codeActive">码活动信息</param>
        /// <param name="ct">工作取消令牌</param>
        public static void CodeActivitysWork(CodeActive codeActive, CancellationToken ct)
        {
            CodeService.logger.Log(LogLevel.Info,$"开始处理码活动Id:{codeActive.CodeActivityId}");
            ActivityProcesser activityProcesser = new ActivityProcesser(codeActive);
            activityProcesser.UpdateSingleActive(ct);
            CodeService.logger.Log(LogLevel.Info, $"完成码活动Id:{codeActive.CodeActivityId}");
        }
        /// <summary>
        /// 码申请处理申请工作
        /// </summary>
        /// <param name="codeApply">码申请信息</param>
        /// <param name="ct"></param>
        public static void ApplyGenerateWork(CodeApply codeApply, CancellationToken ct)
        {
            CodeService.logger.Log(LogLevel.Info,$"处理码申请Id:{codeApply.ApplyId},申请工作");

            #region 初始化码申请码规则处理表ApplyRuleProcess数据
            //获取码规则ID
            int[] ruleIds = Array.ConvertAll<string,int>(codeApply.CodeRulesIDs.Split(new char[] { '|' }), int.Parse);
            //获取处理程序
            List<ApplyRuleProcess> allApplyRuleProcess = ApplyRuleProcessFactory.Instance.GetCodeRuleByApplyId(codeApply.ApplyId);
            for(int i = 0; i < ruleIds.Length; i++)
            {
                if (allApplyRuleProcess.Count <= i)
                {
                    //生成APPLY - RULE - PROCESSOR
                    ApplyRuleProcess ruleProcess = new ApplyRuleProcess() { ApplyId = codeApply.ApplyId, CodeRuleId = ruleIds[i], Status = ApplyRuleStatus.None, AttributeInfo = string.Empty, EndCode = string.Empty, FirstCode = string.Empty };
                    ApplyRuleProcessFactory.Instance.Add(ruleProcess);
                }
            }
            #endregion
            ApplyProcesser applyProcess = new ApplyProcesser(codeApply);
            applyProcess.LoadData();
            applyProcess.ApplyGenerate();
            codeApply.ProcessType = ProcessType.Applied;
            CodeService.logger.Log(LogLevel.Info, $"完成码申请Id:{codeApply.ApplyId},申请工作");
        }

        /// <summary>
        /// 码申请验证申请完成
        /// </summary>
        /// <param name="codeApply">码申请信息</param>
        /// <param name="ct"></param>
        public static bool ApplyGenerateCompleted(CodeApply codeApply, CancellationToken ct)
        {
            CodeService.logger.Log(LogLevel.Info,$"处理码申请Id:{codeApply.ApplyId},验证申请完成工作");
            ApplyProcesser applyProcess = new ApplyProcesser(codeApply);
            applyProcess.LoadData();
            bool completed = applyProcess.GenerateCompleted();
            CodeService.logger.Log(LogLevel.Info, $"完成码申请Id:{codeApply.ApplyId},验证申请完成工作");
            return completed;
        }

        /// <summary>
        /// 码申请存储码结果
        /// </summary>
        /// <param name="codeApply">码申请信息</param>
        /// <param name="ct"></param>
        public static void ApplyCodesStore(CodeApply codeApply, CancellationToken ct)
        {
            CodeService.logger.Log(LogLevel.Info,$"处理码申请Id:{codeApply.ApplyId},存储码结果");
            ApplyProcesser applyProcess = new ApplyProcesser(codeApply);
            applyProcess.LoadData();
            applyProcess.CodesStore();
            CodeService.logger.Log(LogLevel.Info, $"完成码申请Id:{codeApply.ApplyId},存储码结果");
        }

        /// <summary>
        /// 码申请保存单个码信息
        /// </summary>
        /// <param name="codeApply">码申请信息</param>
        /// <param name="ct"></param>
        public static void ApplySaveBySingleCode(CodeApply codeApply, CancellationToken ct)
        {
            CodeService.logger.Log(LogLevel.Info,$"处理码申请Id:{codeApply.ApplyId},保存单个码信息");
            ApplyProcesser applyProcess = new ApplyProcesser(codeApply);
            applyProcess.LoadData();
            applyProcess.SaveBySingleCode(ct);
            CodeService.logger.Log(LogLevel.Info, $"完成码申请Id:{codeApply.ApplyId},保存单个码信息");
        }
        //public static void CodeApplysWork(CodeApply codeApply, CancellationToken ct)
        //{
        //    CodeService.logger.Log(LogLevel.Info,"开始处理码申请："+codeApply.ApplyCorpName);

        //    ApplyProcesser applyProcess = new ApplyProcesser(codeApply);
        //    applyProcess.LoadData();
        //    applyProcess.Generate();
        //    applyProcess.SaveBySingleCode();
        //    //CodeRuleSegFactory.Instance.GetByCodeRuleId()
        //    //Type type = Assembly.Load(new AssemblyName("Acctrue.CMC.CodeBuild")).GetType(codeApply.ClassName);

        //    //ICodeSeg seg = (Activator.CreateInstance(type) as ICodeSeg);


        //    #region 国家发码中心测试
        //    //OtherFlatformIOTRoot other = new OtherFlatformIOTRoot();
        //    //List<ParameterInfo> paras = other.GetArgsFormat();
        //    //paras[0].ParamenterValues = "http://219.232.123.227:8081";
        //    //paras[1].ParamenterValues = "5d4641242d59449686121b1ffa107b90";
        //    //paras[2].ParamenterValues = "EUtuaxcCtRkhRMeYEGs/ag==";
        //    //other.Initialize(paras);
        //    //string applyKey = "";
        //    //string mess = "";

        //    //List<string> ecodes;
        //    //other.EncodeApply(1000000, out applyKey, out mess);
        //    //bool isDownload = false;
        //    //for (int i = 0; i < 60; i++)
        //    //{
        //    //    if (other.ReadyDownload(applyKey))
        //    //    {
        //    //        ecodes = other.EncodeDownload(applyKey);
        //    //        isDownload = true;
        //    //        other.EcodeActivate(ecodes, out mess);
        //    //        break;
        //    //    }
        //    //    System.Threading.Thread.Sleep(i * 30000);
        //    //}
        //    //if (!isDownload)
        //    //{
        //    //    mess = "在最大码下载周期内对方平台无法提供下载!";
        //    //}
        //    #endregion
        //    //int[] codeRulsIds;

        //    //codeRulsIds = Array.ConvertAll<string, int>(codeApply.CodeRulesIDs.Split(new char[] { '|' }), s => int.Parse(s));
        //}
    }
}
