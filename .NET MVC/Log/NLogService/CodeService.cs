using Acctrue.CMC.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Acctrue.CMC.Factory;
using Acctrue.CMC.Factory.Code;
using Acctrue.CMC.Model.Code;
using System.Collections.Concurrent;
using NLog;

namespace Acctrue.CMC.CodeService
{
    partial class CodeService : ServiceBase
    {
        //Logger logger = new Logger();//日志类
        int timeStop = 1000 * 60 * 2;
        CancellationTokenSource cts = new CancellationTokenSource();//取消令牌源
        CancellationToken ct;//取消令牌
        public static NLog.Logger logger = LogManager.GetCurrentClassLogger();

        private ConcurrentQueue<CodeActive> activeQueue = new ConcurrentQueue<CodeActive>();//码活动任务队列

        private static ConcurrentDictionary<int, CodeActive> activeFlags = new ConcurrentDictionary<int, CodeActive>();//队列中的码活动字典（码活动Id,码活动信息）
        private static object lock_activeFlags = new object();//码活动字典锁

        public CodeService()
        {
            InitializeComponent();
            //logger.OnlyEnableTextWriterListener("log.txt", true, TraceLevel.Warning);
            ct = cts.Token;
        }

        protected override void OnStart(string[] args)
        {
            MyWork();
        }
        /// <summary>
        /// 启动码申请和码活动处理主线程
        /// </summary>
        public void MyWork()
        {
            CodeService.logger.Log(LogLevel.Info, "启动码服务！");
            //启动码申请-处理申请服务
            Task codeApplysTask = Task.Factory.StartNew(() =>
            {
                ApplyGenerateWork(ct);
            });

            //启动码申请-验证申请完成可下载
            Task codeApplysGenerateCompletedTask = Task.Factory.StartNew(() =>
            {
                GenerateCompletedWork(ct);
            });

            //启动码申请-存储码结果文件
            Task codeApplysCodesStoreTask = Task.Factory.StartNew(() =>
            {
                CodesStoreWork(ct);
            });

            //启动码申请-逐个码信息存储
            Task codeApplysSaveBySingleCodeTask = Task.Factory.StartNew(() =>
            {
                SaveBySingleCodeWork(ct);
            });
            //启动码活动处理服务
            Task codeActivitysTask = Task.Factory.StartNew(() =>
            {
                CodeActivitysWork(ct);
            });
            //codeApplysCodesStoreTask.Wait();
            //codeApplysTask.Wait();
            //codeApplysGenerateCompletedTask.Wait();
            //codeActivitysTask.Wait();
        }

        protected override void OnStop()
        {
            cts.Cancel();
        }

        /// <summary>
        /// 码申请-处理申请服务
        /// </summary>
        /// <param name="ct">线程退出令牌</param>
        public void ApplyGenerateWork(CancellationToken ct)
        {

            int applyCount = 0;
            Stopwatch watch = new Stopwatch();
            watch.Start();
            try
            {

                CodeService.logger.Log(LogLevel.Info, "启动码申请-处理申请服务！");
                while (true)
                {
                    if (ct.IsCancellationRequested)
                    {
                        CodeService.logger.Log(LogLevel.Info, "停止码申请-处理申请服务！");
                        break;
                    }
                    try
                    {
                        List<CodeApply> applys = CodeApplyFactory.Instance.GetApplysByStatus( ApplyStatus.Audited,true);//待处理已审核状态（cmc审核+tts审核）
                        applys.AddRange(CodeApplyFactory.Instance.GetApplysByStatusProcessType( ApplyStatus.Generate, ProcessType.None));//处理中状态申请
                        //无任务则休息三分钟
                        if (applys.Count == 0)
                        {
                            Thread.Sleep(timeStop);
                        }
                        //每30分钟载入失败任务
                        watch.Stop();
                        CodeService.logger.Log(LogLevel.Info, $"运行时间为:{(watch.ElapsedMilliseconds / 1000) / 60},启动申请载入任务");
                        if ((watch.ElapsedMilliseconds / 1000) / 60 > 30)
                        {
                            List<CodeApply> failApplys = CodeApplyFactory.Instance.GetApplysByTwoStatus(ApplyStatus.Failed, ProcessType.None);
                            foreach (CodeApply apply in failApplys)
                            {
                                if (apply.AuditDate.HasValue)
                                {
                                    if (Convert.ToDateTime(apply.AuditDate).AddDays(5) > DateTime.Now)
                                    {
                                        applys.Add(apply);
                                    }
                                }
                            }

                            watch.Restart(); //重启计时器
                        }
                        else
                        {
                            watch.Start();//继续测量时间
                        }

                        applyCount = applys.Count;
                        foreach (CodeApply apply in applys)
                        {
                            try
                            {
                                if (ct.IsCancellationRequested)
                                {
                                    CodeService.logger.Log(LogLevel.Info, "停止码申请-处理申请服务！");
                                    break;
                                }
                                CodeWork.ApplyGenerateWork(apply, ct);
                                apply.ApplyStatus = ApplyStatus.Generate;
                                apply.ProcessType = ProcessType.Applied;
                                apply.ProcessText = "码申请中";
                                CodeApplyFactory.Instance.Update(apply);
                            }
                            catch (Exception tapplyEx)
                            {
                                CodeService.logger.Log(LogLevel.Warn, "码申请-处理申请服务执行任务出错:" + tapplyEx.Message + tapplyEx.StackTrace);
                                apply.ApplyStatus = ApplyStatus.Failed;
                                //apply.ProcessType = ProcessType.Error;
                                apply.ProcessText = "码申请-处理申请服务执行任务出错:" + tapplyEx.Message;
                                CodeApplyFactory.Instance.Update(apply);
                            }

                        }
                    }
                    catch (Exception taskEx)
                    {
                        CodeService.logger.Log(LogLevel.Warn, "码申请-处理申请载入任务出错:" + taskEx.Message + taskEx.StackTrace);
                        applyCount = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                CodeService.logger.Log(LogLevel.Error, "码申请-处理申请服务出现严重错误:" + ex.Message);
            }
        }

        /// <summary>
        /// 码申请-验证申请完成可下载服务
        /// </summary>
        /// <param name="ct">线程退出令牌</param>
        public void GenerateCompletedWork(CancellationToken ct)
        {
            

            Dictionary<int, DateTime> applyWaitTimes = new Dictionary<int, DateTime>();
            int applyCount = 0;
            Stopwatch watch = new Stopwatch();
            try
            {
                CodeService.logger.Log(LogLevel.Info, "启动码申请-验证申请完成服务！");
                while (true)
                {
                    if (ct.IsCancellationRequested)
                    {
                        CodeService.logger.Log(LogLevel.Info, "停止码申请-验证申请完成服务！");
                        break;
                    }
                    try
                    {
                        List<CodeApply> applys = CodeApplyFactory.Instance.GetApplysByStatusProcessType(ApplyStatus.Generate, ProcessType.Applied);//处理中，已申请处理状态
                        //无任务则休息三分钟
                        if (applys.Count == 0)
                        {
                            Thread.Sleep(timeStop);
                        }
                        //每30分钟载入失败任务
                        watch.Stop();
                        CodeService.logger.Log(LogLevel.Info, $"运行时间为:{(watch.ElapsedMilliseconds / 1000) / 60},验证申请载入任务");
                        if ((watch.ElapsedMilliseconds / 1000) / 60 > 30)
                        {
                            List<CodeApply> failApplys = CodeApplyFactory.Instance.GetApplysByTwoStatus(ApplyStatus.Failed, ProcessType.Applied);
                            foreach (CodeApply apply in failApplys)
                            {
                                if (apply.AuditDate.HasValue)
                                {
                                    if (Convert.ToDateTime(apply.AuditDate).AddDays(5) > DateTime.Now)
                                    {
                                        applys.Add(apply);
                                    }
                                }
                            }
                            watch.Restart(); //重启计时器
                        }
                        else
                        {
                            watch.Start();//继续测量时间
                        }

                        applyCount = 0;
                        foreach (CodeApply apply in applys)
                        {
                            try
                            {
                                if (ct.IsCancellationRequested)
                                {
                                    CodeService.logger.Log(LogLevel.Info, "停止码申请-验证申请完成服务！");
                                    break;
                                }
                                if(applyWaitTimes.ContainsKey(apply.ApplyId) && DateTime.Now < applyWaitTimes[apply.ApplyId])
                                {
                                    continue;
                                }
                                applyCount++;
                                if(CodeWork.ApplyGenerateCompleted(apply, ct))
                                {
                                    if (applyWaitTimes.ContainsKey(apply.ApplyId))
                                    {
                                        applyWaitTimes.Remove(apply.ApplyId);
                                    }
                                }
                                else
                                {
                                    applyWaitTimes.Add(apply.ApplyId, DateTime.Now.AddMinutes(30));
                                    continue;
                                }
                                apply.ProcessType = ProcessType.Generated;
                                apply.ProcessText = "申请操作完成";
                                CodeApplyFactory.Instance.Update(apply);
                            }
                            catch (Exception tapplyEx)
                            {
                                CodeService.logger.Log(LogLevel.Warn, "码申请-验证申请完成服务执行任务出错:" + tapplyEx.Message + tapplyEx.StackTrace);
                                apply.ApplyStatus = ApplyStatus.Failed;
                                //apply.ProcessType = ProcessType.Error;
                                apply.ProcessText = "码申请-验证申请完成服务执行任务出错:" + tapplyEx.Message;
                                CodeApplyFactory.Instance.Update(apply);
                            }

                        }
                    }
                    catch (Exception taskEx)
                    {
                        CodeService.logger.Log(LogLevel.Warn, "码申请-验证申请完成载入任务出错:" + taskEx.Message + taskEx.StackTrace);
                        applyCount = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                CodeService.logger.Log(LogLevel.Error, "码申请-验证申请完成服务出现严重错误:" + ex.Message);
            }
        }

        /// <summary>
        /// 码申请-码文件存储服务
        /// </summary>
        /// <param name="ct">线程退出令牌</param>
        public void CodesStoreWork(CancellationToken ct)
        {
            int applyCount = 0;
            Stopwatch watch = new Stopwatch();
            try
            {
                CodeService.logger.Log(LogLevel.Info,"启动码申请-码文件存储服务！");
                while (true)
                {
                    if (ct.IsCancellationRequested)
                    {
                        CodeService.logger.Log(LogLevel.Info,"停止码申请-码文件存储服务！");
                        break;
                    }
                    try
                    {
                        List<CodeApply> applys = CodeApplyFactory.Instance.GetApplysByStatusProcessType(ApplyStatus.Generate, ProcessType.Generated);//处理中，已验证申请可下载状态
                        //无任务则休息三分钟
                        if (applys.Count == 0)
                        {
                            Thread.Sleep(timeStop);
                        }
                        //每30分钟载入失败任务
                        watch.Stop();
                        CodeService.logger.Log(LogLevel.Info, $"运行时间为:{(watch.ElapsedMilliseconds / 1000) / 60},码文件存储载入任务");
                        if ((watch.ElapsedMilliseconds / 1000) / 60 > 30)
                        {
                            List<CodeApply> failApplys = CodeApplyFactory.Instance.GetApplysByTwoStatus(ApplyStatus.Failed, ProcessType.Generated);
                            foreach (CodeApply apply in failApplys)
                            {
                                if (apply.AuditDate.HasValue)
                                {
                                    if (Convert.ToDateTime(apply.AuditDate).AddDays(5) > DateTime.Now)
                                    {
                                        applys.Add(apply);
                                    }
                                }
                            }
                            watch.Restart(); //重启计时器
                        }
                        else
                        {
                            watch.Start();//继续测量时间
                        }
                                             
                        applyCount = applys.Count;
                        foreach (CodeApply apply in applys)
                        {
                            try
                            {
                                if (ct.IsCancellationRequested)
                                {
                                    CodeService.logger.Log(LogLevel.Info,"停止码申请-码文件存储服务！");
                                    break;
                                }
                                CodeWork.ApplyCodesStore(apply, ct);
                                apply.ProcessType = ProcessType.Stored;
                                apply.ProcessText = "码文件存储完成";
                                CodeApplyFactory.Instance.Update(apply);
                            }
                            catch (Exception tapplyEx)
                            {
                                CodeService.logger.Log(LogLevel.Warn,"码申请-码文件存储服务执行任务出错:" + tapplyEx.Message + tapplyEx.StackTrace);
                                apply.ApplyStatus = ApplyStatus.Failed;
                                //apply.ProcessType = ProcessType.Error;
                                apply.ProcessText = "码申请-码文件存储服务执行任务出错:" + tapplyEx.Message;
                                CodeApplyFactory.Instance.Update(apply);
                            }

                        }
                    }
                    catch (Exception taskEx)
                    {
                        CodeService.logger.Log(LogLevel.Warn, "码申请-码文件存储载入任务出错:" + taskEx.Message + taskEx.StackTrace);
                        applyCount = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                CodeService.logger.Log(LogLevel.Error, "码申请-码文件存储服务出现严重错误:" + ex.Message);
            }
        }

        /// <summary>
        /// 码申请-逐个码信息存储服务
        /// </summary>
        /// <param name="ct">线程退出令牌</param>
        public void SaveBySingleCodeWork(CancellationToken ct)
        {

            int applyCount = 0;
            Stopwatch watch = new Stopwatch();
            try
            {
                CodeService.logger.Log(LogLevel.Info,"启动码申请-逐个码信息存储服务！");
                while (true)
                {
                    if (ct.IsCancellationRequested)
                    {
                        CodeService.logger.Log(LogLevel.Info,"停止码申请-逐个码信息存储服务！");
                        break;
                    }
                    try
                    {
                        List<CodeApply> applys = CodeApplyFactory.Instance.GetApplysByStatusProcessType(ApplyStatus.Generate, ProcessType.Stored);//处理中，已验证申请可下载状态
                        //无任务则休息三分钟
                        if (applys.Count == 0)
                        {
                            Thread.Sleep(timeStop);
                        }
                        //每30分钟载入失败任务
                        watch.Stop();
                        CodeService.logger.Log(LogLevel.Info,$"运行时间为:{(watch.ElapsedMilliseconds / 1000) / 60},单码申请载入任务");
                        if ((watch.ElapsedMilliseconds / 1000) / 60 > 30)
                        {
                            //载入失败任务
                            List<CodeApply> failApplys = CodeApplyFactory.Instance.GetApplysByTwoStatus(ApplyStatus.Failed, ProcessType.Stored);
                            foreach (CodeApply apply in failApplys)
                            {
                                if (apply.AuditDate.HasValue)
                                {
                                    if (Convert.ToDateTime(apply.AuditDate).AddDays(50) > DateTime.Now)
                                    {
                                        applys.Add(apply);
                                    }
                                }
                            }
                            watch.Restart(); //重启计时器
                        }
                        else
                        {
                            watch.Start();//继续测量时间
                        }              

                        applyCount = applys.Count;
                        foreach (CodeApply apply in applys)
                        {
                            try
                            {
                                if (ct.IsCancellationRequested)
                                {
                                    CodeService.logger.Log(LogLevel.Info,"停止码申请-逐个码信息存储服务！");
                                    break;
                                }
                                CodeWork.ApplySaveBySingleCode(apply, ct);
                                apply.ApplyStatus = ApplyStatus.Completed;
                                apply.ProcessType = ProcessType.Completed;
                                apply.ProcessText = "码申请生成完毕";
                                CodeApplyFactory.Instance.Update(apply);
                            }
                            catch (Exception tapplyEx)
                            {
                                CodeService.logger.Log(LogLevel.Warn, "码申请-逐个码信息存储服务执行任务出错:" + tapplyEx.Message + tapplyEx.StackTrace);
                                apply.ApplyStatus = ApplyStatus.Failed;
                                //apply.ProcessType = ProcessType.Error;
                                apply.ProcessText = "码申请-逐个码信息存储服务执行任务出错:" + tapplyEx.Message;
                                CodeApplyFactory.Instance.Update(apply);
                            }
                            //#region 循环生成码申请任务测试使用
                            //string jsonStr = Newtonsoft.Json.JsonConvert.SerializeObject(apply);
                            //CodeApply tempApply = Newtonsoft.Json.JsonConvert.DeserializeObject<CodeApply>(jsonStr);
                            //tempApply.ApplyStatus = ApplyStatus.Audited;
                            //tempApply.ProcessType = ProcessType.None;
                            //CodeApplyFactory.Instance.Add(tempApply);
                            //#endregion

                        }
                    }
                    catch (Exception taskEx)
                    {
                        CodeService.logger.Log(LogLevel.Warn, "码申请-逐个码信息存储载入任务出错:" + taskEx.Message + taskEx.StackTrace);
                        applyCount = 0;
                    }
                    if (applyCount == 0)
                    {
                        Thread.Sleep(5000);
                    }
                }
            }
            catch (Exception ex)
            {
               CodeService.logger.Log(LogLevel.Error,"码申请-逐个码信息存储服务出现严重错误:" + ex.Message);
            }
        }

        /// <summary>
        /// 码活动处理
        /// </summary>
        /// <param name="ct">线程退出对象</param>
        public void CodeActivitysWork(CancellationToken ct)
        {
            Task codeActivMonitorTask = Task.Factory.StartNew(() =>
            {
                ActivitysMonitor(ct);
            });
            List<Task> activeProcessTasks = new List<Task>();
            for (int i = 0; i < 1; i++)
            {
                Task codeActivitysTask = Task.Factory.StartNew(() =>
                {
                    CodeActive codeActive;
                    while (true)
                    {
                        if (ct.IsCancellationRequested)
                        {
                            //CodeService.logger.Log(LogLevel.Info,"停止码活动服务！");
                            break;
                        }
                        if (activeQueue.TryDequeue(out codeActive))
                        {
                            try
                            {
                                codeActive.ProcessType = 1;
                                CodeActiveUploadFactory.Instance.Update(codeActive);
                                CodeWork.CodeActivitysWork(codeActive, ct);
                                codeActive.ProcessText = "码活动更新完成";
                                codeActive.ProcessType = 3;
                                CodeActiveUploadFactory.Instance.Update(codeActive);
                            }
                            catch (Exception ex)
                            {
                                codeActive.ProcessText = ex.Message;
                                codeActive.ProcessType = 2;
                                CodeActiveUploadFactory.Instance.Update(codeActive);

                            }
                            RemoveActiveFlags(codeActive);
                        }
                        else
                        {
                            System.Threading.Thread.Sleep(1000);
                        }
                    }
                });
                activeProcessTasks.Add(codeActivitysTask);
            }
            codeActivMonitorTask.Wait();
            foreach (Task task in activeProcessTasks)
            {
                task.Wait();
            }
        }

        /// <summary>
        /// 码活动任务监控器
        /// </summary>
        /// <param name="ct">线程退出对象</param>
        private void ActivitysMonitor(CancellationToken ct)
        {
            int activityCount = 0;
            try
            {
                CodeService.logger.Log(LogLevel.Info,"启用码活动服务！");
                while (true)
                {
                    try
                    {
                        List<CodeActive> actives = CodeActiveUploadFactory.Instance.GetActivesByStatus(0);//待处理已审核状态
                        actives.AddRange(CodeActiveUploadFactory.Instance.GetActivesByStatus(1));//处理中状态申请
                        activityCount = actives.Count;
                        foreach (CodeActive active in actives)
                        {
                            try
                            {
                                if (ct.IsCancellationRequested)
                                {
                                    CodeService.logger.Log(LogLevel.Info,"停止码活动服务！");
                                    break;
                                }
                                if (!ContainsActiveFlags(active))
                                {
                                    AddActiveFlags(active);
                                    activeQueue.Enqueue(active);
                                }
                                

                            }
                            catch (Exception tapplyEx)
                            {
                                CodeService.logger.Log(LogLevel.Warn, "码活动服务执行任务出错:" + tapplyEx.Message + tapplyEx.StackTrace);
                                active.ProcessType = 2;
                                active.ProcessText = "码活动服务执行任务出错:" + tapplyEx.Message;
                                CodeActiveUploadFactory.Instance.Update(active);
                            }
                        }
                    }
                    catch (Exception taskEx)
                    {
                        CodeService.logger.Log(LogLevel.Warn, "码活动载入任务出错:" + taskEx.Message + taskEx.StackTrace);
                        activityCount = 0;
                    }
                    if (activityCount == 0)
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            if (ct.IsCancellationRequested)
                            {
                                CodeService.logger.Log(LogLevel.Info,"停止码活动服务！");
                                break;
                            }
                            Thread.Sleep(1000);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
               CodeService.logger.Log(LogLevel.Error,"码活动服务出现严重错误:" + ex.Message);
            }
        }

        /// <summary>
        /// 将码活动添加至码活动处理中字典
        /// </summary>
        /// <param name="codeActive">码活动</param>
        public void AddActiveFlags(CodeActive codeActive)
        {
            lock (lock_activeFlags)
            {
                activeFlags.TryAdd(codeActive.CodeActivityId, codeActive);
            }
        }
        /// <summary>
        /// 将码活动从码活动处理中字典移除
        /// </summary>
        /// <param name="codeActive">码活动</param>
        public void RemoveActiveFlags(CodeActive codeActive)
        {
            lock (lock_activeFlags)
            {
                activeFlags.TryRemove(codeActive.CodeActivityId, out codeActive);
            }
        }
        /// <summary>
        /// 验证码活动在处理中字典内
        /// </summary>
        /// <param name="codeActive">码活动</param>
        public bool ContainsActiveFlags(CodeActive codeActive)
        {
            lock (lock_activeFlags)
            {
                return activeFlags.ContainsKey(codeActive.CodeActivityId);
            }
        }
    }
}
