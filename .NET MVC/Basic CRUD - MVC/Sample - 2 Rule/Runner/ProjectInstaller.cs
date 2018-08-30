using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Acctrue.CMC.CodeService
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }
        
        public override void Install(IDictionary stateSaver)
        {
            
            //获取程序所安装的目录
            //string dir = this.Context.Parameters["DIR"].TrimEnd('\\');

            string dir = this.CurrentDir;

            //配置数据库连接
            string utilConfigFile = dir + "\\Acctrue.CMC.CodeService.exe.config";

            Form testForm = new Acctrue.CMC.Configuration.CMCService(utilConfigFile);


            if (testForm.ShowDialog() == DialogResult.Cancel)
                throw new Exception("用户取消了安装。");

            //if ((new Acctrue.CMC.Configuration.CMCConfigForm(utilConfigFile)).ShowDialog() == DialogResult.Cancel)
            //    throw new Exception("用户取消了安装。");

            //string[] files = System.IO.Directory.GetFiles(dir, "install.bat", System.IO.SearchOption.AllDirectories);
            //if (files.Length != 0)
            //{
            //    Process p = new Process();
            //    p.StartInfo.WorkingDirectory = new System.IO.FileInfo(files[0]).DirectoryName;
            //    p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            //    p.StartInfo.FileName = "install.bat";
            //    p.Start();
            //}
            try
            {
                base.Install(stateSaver);
            }
            catch { }
            this.serviceController1.Start();
        }
        public override void Uninstall(IDictionary savedState)
        {
            try
            {
                if (this.serviceController1.Status == ServiceControllerStatus.Running)
                {
                    this.serviceController1.Stop();
                }
                base.Uninstall(savedState);
            }catch
            {

            }
        }
        /// <summary>
        /// 当前DLL所在的目录。
        /// </summary>
        private string CurrentDir
        {
            get
            {
                string assemblyFile = Assembly.GetExecutingAssembly().CodeBase.Remove(0, "file:///".Length).Replace('/', '\\');
                return assemblyFile.Substring(0, assemblyFile.LastIndexOf('\\'));
            }
        }
    }
}
