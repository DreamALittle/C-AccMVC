using Acctrue.CMC.Model.Code;
using Acctrue.CMC.Model;
using Acctrue.CMC.Service;
using Acctrue.CMC.Factory.Code;
using Acctrue.CMC.Factory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Acctrue.CMC.CodeBuild;
using System.IO;
using System.Threading;

namespace Acctrue.CMC.CodeService
{
    public class ApplyProcesser
    {
        /// <summary>
        /// 码申请信息
        /// </summary>
        private CodeApply codeApply;
        /// <summary>
        /// 码申请使用的码规则
        /// </summary>
        private List<CodeRule> codeRules = new List<CodeRule>();
        /// <summary>
        /// 码规则实例集合
        /// </summary>
        private List<SegRuler> segRulers =  new List<SegRuler>();
        /// <summary>
        /// 包装比例（各个码规则的申请比例）
        /// </summary>
        private int[] outerPackageRatios;
        /// <summary>
        /// 各码规则处理状态
        /// </summary>
        private Dictionary<SegRuler, ApplyRuleProcess> applyRuleProcessDict = new Dictionary<SegRuler, ApplyRuleProcess>();
        /// <summary>
        /// 各码规则特征标签
        /// </summary>
        private Dictionary<SegRuler, string> ruleFeatureTags = new Dictionary<SegRuler, string>();
        /// <summary>
        /// 批量操作数据大小
        /// </summary>
        private static readonly int BatchSize = 10000;
        //private int singleRatioAmount;//一个比率单位的数量
        //private Dictionary<CodeRule, List<CodeRuleSeg>> codeRuleSegDict = new Dictionary<CodeRule, List<CodeRuleSeg>>();//码规则包含的码类配置信息
        public ApplyProcesser(CodeApply codeApply)
        {
            this.codeApply = codeApply;
        }
        /// <summary>
        /// 载入数据信息
        /// </summary>
        public void LoadData()
        {
            //清理旧信息
            codeRules.Clear();
            segRulers.Clear();
            applyRuleProcessDict.Clear();
            ruleFeatureTags.Clear();
            //codeRuleSegDict.Clear();
            
            
            int[] ruleIds;
            
            try
            {
                ruleIds = Array.ConvertAll<string, int>(codeApply.CodeRulesIDs.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries), s => int.Parse(s));
                #region 计算各码规则生成数量
                outerPackageRatios = Array.ConvertAll<string, int>(codeApply.OuterPackage.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries), s => int.Parse(s));
                if (outerPackageRatios.Length == 0)//默认未配置则为1比例
                {
                    outerPackageRatios = new int[] { 1};
                }
                if (outerPackageRatios.Length < ruleIds.Length)
                {
                    throw new Exception("码申请信息设置的包装比例分组小于码规则分组!");
                }
                //singleRatioAmount = GetSingleRatioAmount(this.outerPackageRatios, this.codeApply.ApplyAmount);
                #endregion
                string featureTag = string.Empty;
                List<ApplyRuleProcess> allApplyRuleProcess = ApplyRuleProcessFactory.Instance.GetCodeRuleByApplyId(this.codeApply.ApplyId);
                for (int i = 0; i < ruleIds.Length; i++)//载入码规则及码规则处理状态
                {
                    CodeRule rule = CodeRuleFactory.Instance.GetCodeRule(ruleIds[i]);//码规则详细信息
                    if(rule == null)
                    {
                        throw new Exception($"码申请中使用的码规则不存在，码规则Id:{ruleIds[i]}");
                    }
                    codeRules.Add(rule);//码规则数据模型集合
                    //codeRuleSegDict.Add(rule, CodeRuleSegFactory.Instance.GetByCodeRuleId(rule.CodeRuleId));
                    SegRuler ruler = new SegRuler();
                    //码段信息
                    List<CodeRuleSeg> segs = CodeRuleSegFactory.Instance.GetByCodeRuleId(rule.CodeRuleId);

                    FormatSegs(this.codeApply, segs, out featureTag);//根据码申请信息格式化码段参数
                    ruler.Initialization(rule, segs);//初始化码规则类
                    segRulers.Add(ruler);//码规则类集合
                    applyRuleProcessDict.Add(ruler, allApplyRuleProcess[i]);//码申请下码规则处理信息数据模型集合
                    ruleFeatureTags.Add(ruler, featureTag);//码申请下码规则当前特征标签
                }
                
            }
            catch(Exception ex)
            {
                throw new Exception($"码申请处理模块载入码规则出现格式异常！{ex.Message}", ex);
            }

        }
        /// <summary>
        /// 根据码申请信息格式化码段配置项
        /// </summary>
        /// <param name="codeApply">码申请</param>
        /// <param name="segs">码段配置集合</param>
        /// <param name="featureTag">特征标签</param>
        private void FormatSegs(CodeApply codeApply, List<CodeRuleSeg> segs, out string featureTag)
        {
            featureTag = string.Empty;
            string inputFlag = string.Empty;
            foreach (CodeRuleSeg seg in segs)
            {
                if (seg.ClassArgs == null )
                {
                    seg.ClassArgs = "";
                }
                inputFlag = "${" + Enum.GetName(typeof(UserInputFlags), UserInputFlags.CorpCode) + "}";
                if (seg.ClassArgs.Contains(inputFlag))
                {
                    seg.ClassArgs = seg.ClassArgs.Replace(inputFlag, codeApply.CorpCode);
                    featureTag += codeApply.CorpCode;
                }

                inputFlag = "${" + Enum.GetName(typeof(UserInputFlags), UserInputFlags.ProductCode) + "}";
                if (seg.ClassArgs.Contains(inputFlag))
                {
                    seg.ClassArgs = seg.ClassArgs.Replace(inputFlag, codeApply.ProductCode);
                    featureTag += codeApply.ProductCode;
                }

                inputFlag = "${" + Enum.GetName(typeof(UserInputFlags), UserInputFlags.WorklineCode) + "}";
                if (seg.ClassArgs.Contains(inputFlag))
                {
                    seg.ClassArgs = seg.ClassArgs.Replace(inputFlag, codeApply.ProduceWorkLine);
                    featureTag += codeApply.ProduceWorkLine;
                }

                inputFlag = "${" + Enum.GetName(typeof(UserInputFlags), UserInputFlags.OtherCode) + "}";
                if (seg.ClassArgs.Contains(inputFlag))
                {
                    seg.ClassArgs = seg.ClassArgs.Replace(inputFlag, codeApply.UseSubCorpCode);
                    featureTag += codeApply.UseSubCorpCode;
                }
            }
        }

        /// <summary>
        /// 申请生成码
        /// </summary>
        public void ApplyGenerate()
        {
            string applyKey = string.Empty;
            for (int i = 0; i < this.segRulers.Count; i++)
            {
                if (applyRuleProcessDict[this.segRulers[i]].Status == ApplyRuleStatus.None)
                {
                    applyKey = this.segRulers[i].ApplyGenerate(this.codeApply.ApplyAmount * this.outerPackageRatios[i]);
                    applyRuleProcessDict[this.segRulers[i]].AttributeInfo = applyKey;
                    applyRuleProcessDict[this.segRulers[i]].Status = ApplyRuleStatus.Applied;
                    ApplyRuleProcessFactory.Instance.Update(applyRuleProcessDict[this.segRulers[i]]);
                }
            }
        }

        /// <summary>
        /// 验证申请完成可下载
        /// </summary>
        public bool GenerateCompleted()
        {
            bool allRulerCompleted = true;
            for (int i = 0; i < this.segRulers.Count; i++)
            {
                if (applyRuleProcessDict[this.segRulers[i]].Status == ApplyRuleStatus.Applied)
                {
                    if (this.segRulers[i].GenerateCompleted(applyRuleProcessDict[this.segRulers[i]].AttributeInfo))
                    {
                        applyRuleProcessDict[this.segRulers[i]].Status = ApplyRuleStatus.Generated;
                        ApplyRuleProcessFactory.Instance.Update(applyRuleProcessDict[this.segRulers[i]]);
                    }
                    else
                    {
                        allRulerCompleted = false;
                    }
                }
            }
            return allRulerCompleted;
        }

        /// <summary>
        /// 存储各码规则生成码
        /// </summary>
        public void CodesStore()
        {
            List<List<string>> rulersResult = new List<List<string>>();
            List<string> allCode = new List<string>();
            Dictionary<string, RuleProcess> ruleLastCode = new Dictionary<string, RuleProcess>();
            if (applyRuleProcessDict.Values.Where(s => s.Status < ApplyRuleStatus.Generated).Count() > 0)
            {
                throw new Exception($"码申请Id:{this.codeApply.ApplyId}存储码信息过程中，发现中各码规则未完成码申请。");
            }

            //    if (applyRuleProcessDict.Values.Where(s => s.Status != ApplyRuleStatus.Generated).Count() > 0)
            //{
            //    if (applyRuleProcessDict.Values.Where(s => s.Status == ApplyRuleStatus.Stored).Count() > 0)
            //    { }
            //    else
            //    {
            //        throw new Exception($"码申请Id:{this.codeApply.ApplyId}存储码信息过程中，发现中各码规则未完成码申请。");
            //    }

            //}
            if (applyRuleProcessDict.Values.Where(s => s.Status == ApplyRuleStatus.Generated).Count() > 0)
            {
                int outterFileCount = CodeDownloadFactory.Instance.GetOutterCodeFileCount(this.codeApply.ApplyId);//当前申请已存在的外码文件数量
                for (int i = 0; i < this.segRulers.Count; i++)
                {
                    if (applyRuleProcessDict[this.segRulers[i]].Status == ApplyRuleStatus.Generated)
                    {
                        byte[] originalCodeData;
                        #region 加载码规则最后码信息（序列码段序列位置）
                        RuleProcess ruleProcess = null;
                        if (ruleLastCode.ContainsKey(this.codeRules[i].CodeRuleId + "|" + this.ruleFeatureTags[this.segRulers[i]]))
                        {
                            ruleProcess = ruleLastCode[this.codeRules[i].CodeRuleId + "|" + this.ruleFeatureTags[this.segRulers[i]]];
                        }
                        else
                        {
                            ruleProcess = RuleProcessFactory.Instance.GetRuleProcess(this.codeRules[i].CodeRuleId, this.ruleFeatureTags[this.segRulers[i]]);
                            if (ruleProcess == null)
                            {
                                ruleProcess = new RuleProcess() { CodeRuleId = this.codeRules[i].CodeRuleId, FeatureTag = this.ruleFeatureTags[this.segRulers[i]], LastCode = string.Empty };
                                RuleProcessFactory.Instance.Add(ruleProcess);
                            }
                            ruleLastCode.Add(this.codeRules[i].CodeRuleId + "|" + this.ruleFeatureTags[this.segRulers[i]], ruleProcess);
                        }
                        if (ruleProcess.LastCode != null && ruleProcess.LastCode != "")
                        {
                            this.segRulers[i].LastCode = ruleProcess.LastCode;
                        }
                        #endregion
                        rulersResult.Add(this.segRulers[i].GetCodes(applyRuleProcessDict[this.segRulers[i]].AttributeInfo, this.codeApply.ApplyAmount * this.outerPackageRatios[i], out originalCodeData));
                        ruleProcess.LastCode = rulersResult[i][rulersResult[i].Count - 1];

                        if (originalCodeData != null && i >= outterFileCount)//判断有原始码数据且之前没有保存（内部码无原始码数据）
                        {
                            OutterCodeFileRecord outterFile = new OutterCodeFileRecord();
                            outterFile.ApplyId = codeApply.ApplyId;
                            outterFile.Content = originalCodeData;
                            CodeDownloadFactory.Instance.AddOutterCodeFiledRecord(outterFile);
                        }
                    }
                }
                if (this.segRulers.Count > 1)
                {
                    for (int i = 0; i < this.codeApply.ApplyAmount; i++)
                    {
                        for (int j = 0; j < this.segRulers.Count; j++)
                        {
                            for (int k = 0; k < this.outerPackageRatios[j]; k++)
                            {
                                allCode.Add(rulersResult[j][i * this.outerPackageRatios[j] + k]);
                            }
                        }
                    }
                }
                else
                {
                    allCode = rulersResult[0];
                }

                Dictionary<string, string> header = new Dictionary<string, string>();
                header.Add("ApplyId", this.codeApply.ApplyId.ToString());
                header.Add("CorpCode", this.codeApply.CorpCode);
                header.Add("UseCorpCode", this.codeApply.UseCorpCode);
                header.Add("Amount", allCode.Count.ToString());

                CodeRule LastRule = codeRules[codeRules.Count - 1];
                if (LastRule.CodeExtensions != "" || LastRule.CodeExtensions != null)
                {
                    header.Add("UrlFormat", LastRule.CodeExtensions.Split('{')[0]);
                }

                string codeFileName = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
                try
                {
                    using (StreamWriter sw = new StreamWriter(codeFileName, false, Encoding.UTF8))
                    {

                        sw.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(header));
                        foreach (string code in allCode)
                        {
                            //单行添加URL
                            if (LastRule.CodeArguments == "ADD")
                            {
                                sw.WriteLine(string.Format(LastRule.CodeExtensions, code));
                            }
                            else
                            {
                                sw.WriteLine(code);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    try
                    {
                        File.Delete(codeFileName);
                    }
                    catch { }
                    throw new Exception($"码申请Id:{ this.codeApply.ApplyId }存储码信息过程中，发生成存储临时码文件异常:{ex.Message}", ex);
                }
                finally
                {

                }
                string zipFileName = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));

                InnerCodeFileRecord record = new InnerCodeFileRecord();
                record.ApplyId = codeApply.ApplyId;

                try
                {
                    Tools.ZipFile(codeFileName, zipFileName);


                    using (FileStream fs = new FileStream(zipFileName, FileMode.Open))
                    {
                        byte[] contentBytes = new byte[fs.Length];
                        fs.Read(contentBytes, 0, (int)fs.Length);
                        record.Content = contentBytes;
                        //record.Content = Convert.ToBase64String(contentBytes);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"码申请Id:{ this.codeApply.ApplyId }存储码信息过程中，发生压缩码文件异常:{ex.Message}", ex);
                }
                finally
                {
                    try
                    {
                        File.Delete(codeFileName);
                        File.Delete(zipFileName);
                    }
                    catch { }
                }
                CodeDownloadFactory.Instance.AddInnerCodeFiledRecord(record);

                //更新申请码规则状态
                for (int i = 0; i < this.segRulers.Count; i++)
                {
                    if (applyRuleProcessDict[this.segRulers[i]].Status == ApplyRuleStatus.Generated)
                    {
                        applyRuleProcessDict[this.segRulers[i]].FirstCode = rulersResult[i][0];
                        applyRuleProcessDict[this.segRulers[i]].EndCode = rulersResult[i][rulersResult[i].Count - 1];
                        applyRuleProcessDict[this.segRulers[i]].Status = ApplyRuleStatus.Stored;
                        ApplyRuleProcessFactory.Instance.Update(applyRuleProcessDict[this.segRulers[i]]);
                    }
                }

                //更新码规则最后码状态信息
                foreach (RuleProcess process in ruleLastCode.Values)
                {
                    RuleProcessFactory.Instance.Update(process);
                }
            }
            
        }



        ///// <summary>
        ///// 生成码信息
        ///// </summary>
        //public void Generate()
        //{
        //    List<List<string>> rulersResult = new List<List<string>>();
        //    List<string> allCode = new List<string>();
        //    for(int i=0;i<this.segRulers.Count;i++)
        //    {
        //        rulersResult.Add(this.segRulers[i].Generate(this.codeApply.ApplyAmount * this.outerPackageRatios[i]));
        //    }
        //    if (this.segRulers.Count > 1)
        //    {
        //        for (int i = 0; i < this.codeApply.ApplyAmount; i++)
        //        {
        //            for (int j = 0; j < this.segRulers.Count; j++)
        //            {
        //                for (int k = 0; k < this.outerPackageRatios[j]; k++)
        //                {
        //                    allCode.Add(rulersResult[j][i * this.outerPackageRatios[j] + k]);
        //                }
        //            }
        //        }
        //    }
        //    else
        //    {
        //        allCode = rulersResult[0];
        //    }
        //    ////#region测试代码
        //    //Dictionary<string, object> extentDictionary = new Dictionary<string, object>();
        //    //extentDictionary.Add("ExtentName","TEST");
        //    //extentDictionary.Add("ExtentType", "TestType");
        //    //List<Acctrue.CMC.Model.Report.ActivityCodes> activityCodes = allCode.Select(s => new Acctrue.CMC.Model.Report.ActivityCodes() { Code=s, Extent=extentDictionary }).ToList();
        //    //string activeStr =  Newtonsoft.Json.JsonConvert.SerializeObject(activityCodes);
        //    //CodeActiveUploadFactory.Instance.InsertNewActive(new Model.Systems.AppSettingInfo() { AppID="1", AppSettingID=1, AppStatus=0, CorpCode="1", CorpName="TEST", CreatedDate=DateTime.Now, SubCorpCode="01" }, new Model.Request.UploadActiveRequest() { ApplyId=33, ActiveName="激活", Memo="", ProductCode="01", WorkLine="01", ProductName="TEST", File= Tools.SerializeZipCode(activeStr) });
        //    ////#endregion
        //    InnerCodeFileRecord record = new InnerCodeFileRecord();
        //    record.ApplyId = codeApply.ApplyId;
        //    record.Content = Tools.SerializeZipCode(allCode);
        //    CodeDownloadFactory.Instance.AddInnerCodeFiledRecord(record);

        //    //更新码申请记录状态完成
        //    codeApply.ApplyStatus = ApplyStatus.Completed;
        //    codeApply.ProcessType = ProcessType.Completed;
        //    CodeApplyFactory.Instance.Update(codeApply);
        //}

        /// <summary>
        /// 将生成后的码信息逐个存储于mongodb
        /// </summary>
        ///<param name="ct">工作取消令牌</param>
        public void SaveBySingleCode(CancellationToken ct)
        {
            InnerCodeFileRecord record = CodeDownloadFactory.Instance.GetInnerCodeFileRecord(codeApply.ApplyId);

            //处理码格式
            List<string> codes = new List<string>();
            List<string> originCodes = Tools.DeserializeZipCode(Convert.ToBase64String(record.Content));
            foreach (string oriCode in originCodes)
            {
                codes.Add(oriCode.Split(new char[] { '/'}).LastOrDefault());
            }
            
            List<ApplyCodes> applyCodes = new List<ApplyCodes>();

            ApplyRuleProcess applyRuleProcess = ApplyRuleProcessFactory.Instance.GetCodeRuleByApplyIdRuleId(this.codeApply.ApplyId, 0);
            if (applyRuleProcess == null)
            {
                applyRuleProcess = new ApplyRuleProcess() { ApplyId=this.codeApply.ApplyId, CodeRuleId=0, FirstCode=codes[1], EndCode=codes[codes.Count-1], Status= ApplyRuleStatus.None, AttributeInfo="0" };
                ApplyRuleProcessFactory.Instance.Add(applyRuleProcess);
            }
            int lastLineIndex = 0;
            int.TryParse(applyRuleProcess.AttributeInfo, out lastLineIndex);
            if (lastLineIndex<codes.Count-1)
            {
                for (int i = lastLineIndex; i < codes.Count-1; i++)
                {
                    applyCodes.Clear();
                    for (int j = 0; j < BatchSize && i < codes.Count-1; j++, i++)
                    {
                        applyCodes.Add(new ApplyCodes() { AppId = codeApply.ApplyAppId, ApplyId = codeApply.ApplyId, ApplyType = codeApply.ApplyType, Code = codes[i+1], CorpCode = codeApply.CorpCode, CorpName = codeApply.CorpName, CreateDate = codeApply.AuditDate == null ? DateTime.Now : (DateTime)codeApply.AuditDate, ProduceWorkline = codeApply.ProduceWorkLine, ProductCode = codeApply.ProductCode, ProductName = codeApply.ProductName, SubCorpCode = codeApply.ApplySubCorpCode, UseCorpCode = codeApply.UseCorpCode, UseCorpName = codeApply.UseCorpName, UseSubCorpCode = codeApply.UseSubCorpCode });
                    };
                    
                    
                    ApplyCodesFactory.Instance.BatchAdd(applyCodes);
                    applyRuleProcess.AttributeInfo = i.ToString();
                    ApplyRuleProcessFactory.Instance.Update(applyRuleProcess);
                    i--;
                }
            }

            //更新状态
            applyRuleProcess.Status = ApplyRuleStatus.Completed;
            ApplyRuleProcessFactory.Instance.Update(applyRuleProcess);
            for (int i = 0; i < this.segRulers.Count; i++)
            {
                if (applyRuleProcessDict[this.segRulers[i]].Status == ApplyRuleStatus.Stored)
                {
                    applyRuleProcessDict[this.segRulers[i]].Status = ApplyRuleStatus.Completed;
                    ApplyRuleProcessFactory.Instance.Update(applyRuleProcessDict[this.segRulers[i]]);
                }
            }
        }

        ///// <summary>
        ///// 得到每个单位比例的码数量
        ///// </summary>
        ///// <param name="packageRatios">包装比例</param>
        ///// <param name="applyAmount">码申请数量</param>
        ///// <returns></returns>
        //private int GetSingleRatioAmount(int[] packageRatios, int applyAmount)
        //{
        //    int amount = 0;
        //    int minRatio = 0;
        //    foreach(int ratioValue in packageRatios)
        //    {
        //        if (minRatio == 0 || ratioValue < minRatio)
        //        {
        //            minRatio = ratioValue;
        //        }
        //    }
        //    if (applyAmount % minRatio == 0)
        //    {
        //        amount = applyAmount / minRatio;
        //    }
        //    else
        //    {
        //        throw new Exception("码申请数量不能被包装比例设置整除");
        //    }

        //    return amount;
        //}
    }
}
