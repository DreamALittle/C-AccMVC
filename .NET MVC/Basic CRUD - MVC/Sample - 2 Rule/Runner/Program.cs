using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NLog;

namespace Acctrue.CMC.CodeService
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        static void Main(string[] args)
        {
            CodeService.logger.Log(LogLevel.Info,"正在启动......");
            try
            {
                new BizBase().RefreshMockConfig();
                Acctrue.CMC.Factory.Code.CodeApplyFactory.Instance.GetApply(0);
            }catch
            {

            }
            if (args.Length > 0 && args[0].ToLower() == "-console")
            {

                var cts = new CancellationTokenSource();
                var ct = cts.Token;
                CodeService service = new CodeService();
                service.MyWork();
                System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);
            }
            else
            {
                ServiceBase[]  ServicesToRun = new ServiceBase[]
                {
                                    new CodeService()
                };
                ServiceBase.Run(ServicesToRun);
            }

        }
    }
}
