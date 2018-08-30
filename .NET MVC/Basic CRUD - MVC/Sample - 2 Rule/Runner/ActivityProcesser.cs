using Acctrue.CMC.Model.Code;
using Acctrue.CMC.Model.Report;
using Acctrue.CMC.Model;
using Acctrue.CMC.Service;
using Acctrue.CMC.Factory.Code;
using Acctrue.CMC.Factory.Report;
using Acctrue.CMC.Factory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Acctrue.CMC.CodeBuild;
using System.IO;
using System.Collections.Concurrent;
using System.Reflection;
using System.Threading;
using Acctrue.CMC.Factory.Activity;
using Acctrue.CMC.Model.Activity;

namespace Acctrue.CMC.CodeService
{
    /// <summary>
    /// 码活动处理类
    /// </summary>
    public class ActivityProcesser
    {
        /// <summary>
        /// 活动与处理接口关系字典
        /// </summary>
        private static ConcurrentDictionary<string, IActivityAction> activateActionRelation = new ConcurrentDictionary<string, IActivityAction>();
        /// <summary>
        /// 关系字典初始化状态
        /// </summary>
        private static bool relationInit = false;
        /// <summary>
        /// 初始化关系字典锁对象
        /// </summary>
        private static object relationLock = new object();
        /// <summary>
        /// 码活动信息对象
        /// </summary>
        private CodeActive codeActive;
        /// <summary>
        /// 批量操作数据大小
        /// </summary>
        private static readonly int BatchSize = 10000;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="codeActive"></param>
        public ActivityProcesser(CodeActive codeActive)
        {
            if (!relationInit)
            {
                lock (relationLock)
                {
                    if (!relationInit)
                    {
                        List<Type> actions = Assembly.GetExecutingAssembly().GetTypes().Where(item => item.GetInterfaces().Contains(typeof(IActivityAction))).ToList();
                        foreach (Type actionType in actions)
                        {
                            IActivityAction action = (Activator.CreateInstance(actionType) as IActivityAction);
                            activateActionRelation.TryAdd(action.ActivityName, action);
                        }
                    }
                }
            }
            this.codeActive = codeActive;
        }

        /// <summary>
        /// 分解码活动记录更新MongoDb中码活动信息
        /// </summary>
        ///<param name="ct">工作取消令牌</param>
        public void UpdateSingleActive(CancellationToken ct)
        {
            List<string> codes = new List<string>();
            string codeSerializeStr = CodeActiveUploadFactory.Instance.GetFile(codeActive.CodeActivityId);
            
            if (codeSerializeStr == string.Empty)
            {
                throw new Exception("码活动没有相关码信息，码活动ID:"+codeActive.CodeActivityId);
            }
            if (string.IsNullOrEmpty( codeActive.ActivityName))
            {
                //不同的活动用不同的类型
                switch (codeActive.ActivityName)
                {
                    case "激活":
                        this.SaveActiveCodes<ActiveCode>(ct, codeSerializeStr, typeof(ActiveCode).Name);
                        break;
                    case "消费者":
                        this.SaveActiveCodes<CustomerCode>(ct, codeSerializeStr, typeof(CustomerCode).Name);
                        break;
                    case "营销":
                        this.SaveActiveCodes<SalesCode>(ct, codeSerializeStr, typeof(SalesCode).Name);
                        break;
                    case "资产":
                        this.SaveActiveCodes<PropertyCode>(ct, codeSerializeStr, typeof(PropertyCode).Name);
                        break;
                    case "资产领用":
                        this.SaveActiveCodes<SalesCode>(ct, codeSerializeStr, typeof(SalesCode).Name);
                        break;
                    default:
                        throw new Exception("没有对应的活动名称，码活动名称:" + codeActive.ActivityName);
                }
            }  
        }

        private void SaveActiveCodes<T>(CancellationToken ct, string codeSerializeStr,string modelName) where T : ActivityCodes
        {
            List<T> activityCodes = new List<T>();
            ActivityCodes tempActivityCode = null;
            try
            {
                codeSerializeStr = Tools.UnZipGetFirstText(Convert.FromBase64String(codeSerializeStr));
                if (codeSerializeStr == string.Empty)
                {
                    throw new Exception("码活动没有相关码信息，码活动ID:" + codeActive.CodeActivityId);
                }
                int lastLineIndex = 0;
                int.TryParse(this.codeActive.ProcessText, out lastLineIndex);
                string lineStr = string.Empty;
                using (StringReader sr = new StringReader(codeSerializeStr))
                {
                    int lineIndex = 0;

                    while (sr.Peek() > -1)
                    {

                        if (lineIndex >= lastLineIndex)
                        {
                            activityCodes.Clear();
                            for (int i = 0; i < BatchSize && sr.Peek() > -1; i++)
                            {
                                lineStr = sr.ReadLine();
                                lineIndex++;
                                if (lineStr.Trim() == "")
                                    continue;

                                Assembly assembly = Assembly.Load("Acctrue.CMC.Model");
                                var activityCode = (T)assembly.CreateInstance("Acctrue.CMC.Model.Report." + modelName);
                                tempActivityCode = Newtonsoft.Json.JsonConvert.DeserializeObject<ActivityCodes>(lineStr);
                                if (tempActivityCode.Extent!=null)
                                {
                                    //把拓展字段加到实体中
                                    foreach (var d in tempActivityCode.Extent)
                                    {
                                        try
                                        {
                                            var value = d.Value;
                                            if (activityCode.GetType().GetProperty(d.Key).PropertyType == typeof(DateTime))
                                            {
                                                activityCode.GetType().GetProperty(d.Key).SetValue(activityCode, Convert.ToDateTime(value));
                                                continue;
                                            }
                                            if (activityCode.GetType().GetProperty(d.Key).PropertyType == typeof(Int32) || activityCode.GetType().GetProperty(d.Key).PropertyType.IsEnum)
                                            {
                                                activityCode.GetType().GetProperty(d.Key).SetValue(activityCode, Convert.ToInt32(value));
                                                continue;
                                            }
                                            activityCode.GetType().GetProperty(d.Key).SetValue(activityCode, value);
                                        }
                                        catch (Exception e)
                                        { string err = e.Message; }
                                    }
                                }
                                //公共字段
                                activityCode.GetType().GetProperty("ActiveName").SetValue(activityCode, codeActive.ActivityName);
                                activityCode.GetType().GetProperty("ApplyId").SetValue(activityCode, Convert.ToInt32(codeActive.ApplyId));
                                activityCode.GetType().GetProperty("CodeActivityId").SetValue(activityCode, Convert.ToInt32(codeActive.CodeActivityId));
                                activityCode.GetType().GetProperty("Code").SetValue(activityCode, tempActivityCode.Code);
                                activityCode.GetType().GetProperty("CorpCode").SetValue(activityCode, codeActive.CorpCode);
                                activityCode.GetType().GetProperty("CorpName").SetValue(activityCode, codeActive.CorpName);
                                activityCode.GetType().GetProperty("CreateDate").SetValue(activityCode, Convert.ToDateTime(codeActive.UploadDate));
                                activityCode.GetType().GetProperty("ProductCode").SetValue(activityCode, codeActive.ProductCode);
                                activityCode.GetType().GetProperty("ProductName").SetValue(activityCode, codeActive.ProductName);
                                activityCode.GetType().GetProperty("SubCorpCode").SetValue(activityCode, codeActive.SubCorpCode);
                                activityCode.GetType().GetProperty("Memo").SetValue(activityCode, codeActive.Memo);
                                activityCodes.Add(activityCode);
                            }
                            if (activityCodes != null && activityCodes.Count > 0)
                            {
                                ReportFactory.Instance.BatchAdd(activityCodes);
                                if (activateActionRelation.ContainsKey(codeActive.ActivityName))
                                {
                                    activateActionRelation[codeActive.ActivityName].ActionProcess(codeActive, activityCodes);
                                }
                                this.codeActive.ProcessText = lineIndex.ToString();
                                CodeActiveUploadFactory.Instance.Update(this.codeActive);
                            }
                            if (ct.IsCancellationRequested)
                            {
                                return;
                            }
                        }
                        else
                        {
                            lineStr = sr.ReadLine();
                            lineIndex++;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("解析码活动对应码信息出错！请确认码信息内容格式！", ex);
            }
        }
    }
}
