using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Acctrue.CMC.Model.Code;
using Newtonsoft.Json;

namespace Acctrue.CMC.CodeBuild
{
    /// <summary>
    /// 码规则类
    /// </summary>
    public class SegRuler
    {
        private CodeRule codeRule;
        private List<CodeRuleSeg> codeRuleSegs;
        private List<ICodeSeg> codeSegList = new List<ICodeSeg>();//码规则容器

        private List<List<ParameterInfo>> allSegParameters = new List<List<ParameterInfo>>();

        private RuleSegType ruleType = RuleSegType.Local;
        private string lastCode;//当前码规则最后一次码信息
        private static int MaxRetryCount = 5;//最大重试次数
        private static readonly int RetryIntervalMillisecond = 1000;//重试间隔时间（毫秒）


        public RuleSegType RuleType { get => ruleType; set => ruleType = value; }

        //public CodeRule CodeRule { get => codeRule; }

        //public SegRuler(CodeRule rule, List<CodeRuleSeg> segs)
        //{
        //    this.codeRule = rule;
        //    this.codeRuleSegs = segs;
        //}

        public void Initialization(CodeRule rule, List<CodeRuleSeg> segs)
        {
            try
            {
                this.codeRule = rule;
                this.codeRuleSegs = segs;
                System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
                
                foreach (CodeRuleSeg seg in segs)
                {
                    Type type = assembly.GetType(seg.ClassName);
                    ICodeSeg codeSeg = (Activator.CreateInstance(type) as ICodeSeg);
                    if (seg.ClassArgs == null)
                    {
                        seg.ClassArgs = "";
                    }
                    codeSeg.Initialize(JsonConvert.DeserializeObject<List<ParameterInfo>>(seg.ClassArgs));
                    codeSegList.Add(codeSeg);
                    if(codeSeg is IOtherFlatformSeg)
                    {
                        this.ruleType = RuleSegType.OtherFlatform;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"码规则Id {rule.CodeRuleId}初始化过程失败：{ex.Message}", ex);
            }
        }

        /// <summary>
        /// 检测初始化状态
        /// </summary>
        public void CheckInitialized()
        {
            if(codeRule == null)
            {
                throw new Exception("类未进行初始化操作！");
            }
        }

        /// <summary>
        /// 载入配置信息
        /// </summary>
        public void LoadConfig(List<ParameterInfo> currentArgs)
        {
            CheckInitialized();
            if (codeSegList.Count != allSegParameters.Count)
            {
                throw new FormatException("配置信息不匹配");
            }
            string mess;
            for (int i = 0; i < codeSegList.Count; i--)
            {
                if (codeSegList[i].ValidateArgsFormat(currentArgs, out mess))
                {
                    codeSegList[i].Initialize(currentArgs);
                }
                else
                {
                    throw new FormatException($"{codeSegList[i].Description}配置信息异常:{mess}");
                }
            }
        }

        /// <summary>
        /// 获取当前配置
        /// </summary>
        public string GetCurrentConfig()
        {
            return JsonConvert.SerializeObject(allSegParameters);
        }

        /// <summary>
        /// 设置规则序列位置状态
        /// </summary>
        /// <param name="serialStr">序列状态串</param>
        public void SetSerial(string serialStr)
        {
            CheckInitialized();
            List<string> serialFlags = JsonConvert.DeserializeObject<List<string>>(serialStr);
            IEnumerable<ICodeSeg> serialSegs = codeSegList.Where(s => (s is ISerialSeg));
            if (serialSegs.Count() != serialFlags.Count)
            {
                throw new FormatException("序列状态串不匹配");
            }
            for (int i = 0; i < serialSegs.Count(); i++)
            {
                ((ISerialSeg)serialSegs.ElementAt(i)).SetLastSegValue(serialFlags[i]);
            }
        }

        /// <summary>
        /// 获取当前序列位置状态串
        /// </summary>
        public string GetCurrentSerial()
        {
            CheckInitialized();
            List<string> serialFlags = new List<string>();
            foreach (ICodeSeg seg in codeSegList)
            {
                if (seg is ISerialSeg)
                {
                    serialFlags.Add(((ISerialSeg)seg).CurrentSerialNoString);
                }
            }
            return JsonConvert.SerializeObject(serialFlags);
        }

        /// <summary>
        /// 申请生成码
        /// </summary>
        /// <param name="amount">数量</param>
        /// <returns></returns>
        public string ApplyGenerate(int amount)
        {
            CheckInitialized();
            bool succeed = false;
            string applyKey = string.Empty;
            if (this.ruleType == RuleSegType.Local)
            {
                applyKey = Guid.NewGuid().ToString("N");
            }
            else
            {
                foreach (ICodeSeg seg in codeSegList)
                {
                    if (seg is IOtherFlatformSeg)
                    {
                        string mess = string.Empty;
                        IOtherFlatformSeg otherSeg = ((IOtherFlatformSeg)seg);
                        try
                        {
                            for (int i = 0; i < MaxRetryCount; i++) {
                                try
                                {
                                    succeed = otherSeg.EncodeApply(amount, out applyKey, out mess);
                                } catch (Exception applyEx)
                                {
                                    if (i == MaxRetryCount - 1)
                                    {
                                        throw applyEx;
                                    }
                                    System.Threading.Thread.Sleep(RetryIntervalMillisecond);
                                }
                            }
                            if (succeed)
                            {
                                
                            }
                            else
                            {
                                throw new Exception($"外部平台<{otherSeg.Description}>码申请失败:{mess}");
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception($"外部平台<{otherSeg.Description}>码申请失败:{ex.Message}", ex);
                        }
                        break;
                    }
                }
            }
            return applyKey;
        }
        /// <summary>
        /// 验证码生成完成（可以下载状态）
        /// </summary>
        /// <param name="applyKey">码生成凭证</param>
        /// <returns>已完成</returns>
        public bool GenerateCompleted(string applyKey)
        {
            CheckInitialized();
            bool completed = false;
            if (this.ruleType == RuleSegType.Local)
            {
                completed = true;
            }
            else
            {
                foreach (ICodeSeg seg in codeSegList)
                {
                    if (seg is IOtherFlatformSeg)
                    {
                        IOtherFlatformSeg otherSeg = ((IOtherFlatformSeg)seg);
                        try
                        {
                            for (int i = 0; i < MaxRetryCount; i++)
                            {
                                try
                                {
                                    completed = otherSeg.ReadyDownload(applyKey);
                                }
                                catch (Exception downEx)
                                {
                                    if (i == MaxRetryCount - 1)
                                    {
                                        throw downEx;
                                    }
                                    System.Threading.Thread.Sleep(RetryIntervalMillisecond);
                                }
                            }
                            if (completed)
                            {
                                completed = true;
                                break;
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception($"外部平台<{otherSeg.Description}>码申请验证状态失败:{ex.Message}", ex);
                        }
                        break;
                    }
                }
            }
            return completed;
        }

        /// <summary>
        /// 最后一次码信息
        /// </summary>
        public string LastCode
        {
            get
            {
                return lastCode;
            }
            set
            {
                int startIndex = 0;
                for (int i=0;i<codeSegList.Count;i++)
                {
                    if (codeSegList[i] is ISerialSeg)
                    {
                        ((ISerialSeg)codeSegList[i]).SetLastSegValue(value.Substring(startIndex, codeSegList[i].Length));
                    }
                    else
                    {
                        startIndex = startIndex + codeSegList[i].Length;
                        //startIndex += this.codeRuleSegs[i].SegLength;
                    }
                }
                lastCode = value;
            }
        }

        /// <summary>
        /// 获取码信息
        /// </summary>
        /// <param name="applyKey">码申请凭证</param>
        /// <param name="amount">数量</param>
        /// <param name="originalCodeData">原始码数据(针对外部码平台)</param>
        /// <returns></returns>
        public List<string> GetCodes(string applyKey, int amount, out byte[] originalCodeData)
        {
            originalCodeData = null;
            CheckInitialized();
            List<string> results = new List<string>();
            StringBuilder codeBuilder = new StringBuilder();
            if (this.ruleType == RuleSegType.Local)
            {
                for (int i = 0; i < amount; i++)
                {
                    codeBuilder.Clear();
                    foreach (ICodeSeg seg in codeSegList)
                    {
                        if (seg is IAntiFakeSeg)
                        {
                            codeBuilder.Append(((IAntiFakeSeg)seg).Generate(codeBuilder.ToString()));
                        }
                        else if (seg is ISerialSeg)
                        {
                            codeBuilder.Append(seg.Generate());
                        }
                        else
                        {
                            codeBuilder.Append(seg.Generate());
                        }
                    }
                    results.Add(codeBuilder.ToString());
                }
            }
            else
            {
                foreach (ICodeSeg seg in codeSegList)
                {
                    if (seg is IOtherFlatformSeg)
                    {
                        IOtherFlatformSeg otherSeg = ((IOtherFlatformSeg)seg);
                        try
                        {
                            bool isDownload = false;
                            for (int i = 0; i < MaxRetryCount; i++)
                            {
                                if (otherSeg.ReadyDownload(applyKey))
                                {
                                    try
                                    {
                                        results = otherSeg.EncodeDownload(applyKey, out originalCodeData);
                                    }
                                    catch (Exception downEx)
                                    {
                                        if (i == MaxRetryCount - 1)//最后一次尝试失败将异常抛出
                                        {
                                            throw downEx;
                                        }
                                        continue;
                                    }
                                    isDownload = true;
                                    break;
                                }
                                System.Threading.Thread.Sleep(RetryIntervalMillisecond);
                            }
                            if (!isDownload)
                            {
                                throw new Exception("对方平台无法提供下载!");
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception($"外部平台<{otherSeg.Description}>码下载失败:{ex.Message}", ex);
                        }
                        break;
                    }
                }

            }
            return results;
        }

        /// <summary>
        /// 生成码
        /// </summary>
        /// <param name="amount">数量</param>
        public List<string> Generate(int amount)
        {
            CheckInitialized();
            List<string> results = new List<string>();
            StringBuilder codeBuilder = new StringBuilder();
            if (this.ruleType == RuleSegType.Local)
            {
                for (int i = 0; i < amount; i++)
                {
                    codeBuilder.Clear();
                    foreach (ICodeSeg seg in codeSegList)
                    {
                        if (seg is IAntiFakeSeg)
                        {
                            codeBuilder.Append(((IAntiFakeSeg)seg).Generate(codeBuilder.ToString()));
                        }
                        else if (seg is ISerialSeg)
                        {
                            codeBuilder.Append(seg.Generate());
                        }
                        else
                        {
                            codeBuilder.Append(seg.Generate());
                        }
                    }
                    results.Add(codeBuilder.ToString());
                }
            }
            else
            {
                foreach (ICodeSeg seg in codeSegList)
                {
                    if (seg is IOtherFlatformSeg)
                    {
                        string applyKey, mess;
                        IOtherFlatformSeg otherSeg = ((IOtherFlatformSeg)seg);
                        try
                        {
                            if (otherSeg.EncodeApply(amount, out applyKey, out mess))
                            {
                                bool isDownload = false;
                                for (int i = 0; i < 30; i++)
                                {
                                    if (otherSeg.ReadyDownload(applyKey))
                                    {
                                        byte[] originalCodeData;
                                        results = otherSeg.EncodeDownload(applyKey, out originalCodeData);
                                        isDownload = true;
                                        break;
                                    }
                                    for (int j = 0; j < 6; j++)
                                    {
                                        System.Threading.Thread.Sleep(RetryIntervalMillisecond);
                                    }
                                }
                                if (!isDownload)
                                {
                                    mess = "在最大码下载周期内对方平台无法提供下载!";
                                }
                            }
                            else
                            {
                                throw new Exception($"外部平台<{otherSeg.Description}>码申请失败:{mess}");
                            }
                        }catch(Exception ex)
                        {
                            throw new Exception($"外部平台<{otherSeg.Description}>码申请失败:{ex.Message}", ex);
                        }
                        break;
                    }
                }

            }
            return results;
        }
        /// <summary>
        /// 验证有效性
        /// </summary>
        /// <param name="errorMess">验证无效信息</param>
        /// <returns>码规则验证有效</returns>
        public bool ValidateSegRule(out string errorMess)
        {
            CheckInitialized();
            errorMess = string.Empty;
            bool validated = true;

            if (codeSegList.Count == 0)
            {
                validated = false;
                errorMess = "没有设置码段";
            }
            else
            {
                if (!ValidateSegsOrder(out errorMess))
                {
                    validated = false;
                }
            }

            return validated;
        }

        /// <summary>
        /// 码段组合序列检测
        /// </summary>
        /// <param name="errorMess">验证有效信息</param>
        /// <returns>码规则验证有效</returns>
        private bool ValidateSegsOrder(out string errorMess)
        {
            CheckInitialized();
            errorMess = string.Empty;
            bool validated = true;
            return validated;
        }
    }
}