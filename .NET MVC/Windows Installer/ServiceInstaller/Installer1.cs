using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Windows.Forms;
using System.Diagnostics;
using System.Reflection;

namespace Acctrue.TTS
{
    [ToolboxItem(false)]
    [RunInstaller(true)]
    public partial class Installer1 : Installer
    {
        public Installer1()
        {
            InitializeComponent();
        }

        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);

            //获取程序所安装的目录
            //string dir = this.Context.Parameters["DIR"].TrimEnd('\\');
            
            //安装数据库
            //MessageBoxButtons mess = MessageBoxButtons.OKCancel;
            //DialogResult dr = MessageBox.Show("是否导入之前版本配置？", "提示", mess);
            //if (dr == DialogResult.OK)
            //{
                //string dir = this.CurrentDir;
                //this.InitializeConfigFiles(dir);
                //string utilConfigFile = dir + "\\acctrueTTSConfigUtil.exe.config";
                ////CustomActionData /productname="Alert服务"
                //var productName = this.Context.Parameters["productname"];
                //var serviceName = this.Context.Parameters["servicename"];
                //var configName = this.Context.Parameters["configpath"];
                //string configPath = null;
                //if (!string.IsNullOrEmpty(configName))
                //    configPath = System.IO.Path.Combine(this.CurrentDir, configName);

                //if (new Acctrue.TTS.Configuration.ConfigForm(utilConfigFile, productName, serviceName, configPath).ShowDialog() == DialogResult.Cancel)
                //    throw new Exception("用户取消了安装。");
            //}

            //string[] files = System.IO.Directory.GetFiles(dir, "install.bat", System.IO.SearchOption.AllDirectories);
            //if (files.Length != 0)
            //{
            //    Process p = new Process();
            //    p.StartInfo.WorkingDirectory = new System.IO.FileInfo(files[0]).DirectoryName;
            //    p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            //    p.StartInfo.FileName = "install.bat";
            //    p.Start();
            //}     
        }
        //private void InitializeConfigFiles(string dir)
        //{
        //    foreach (string installedFile in System.IO.Directory.GetFiles(dir, "*.config.installed", System.IO.SearchOption.AllDirectories))
        //    {
        //        string installedFileName = installedFile.Remove(0, installedFile.LastIndexOf('\\') + 1);
        //        string configFileName = installedFileName.Substring(0, installedFileName.LastIndexOf('.'));
        //        this.CreateConfigFile(dir, configFileName, "installed");
        //    }
        //}
        //private void CreateConfigFile(string dir, string configFileName, string installedEx)
        //{
        //    string[] configFiles = System.IO.Directory.GetFiles(dir, configFileName, System.IO.SearchOption.AllDirectories);
        //    if (configFiles.Length == 0)
        //    {
        //        string[] installedConfigFiles = System.IO.Directory.GetFiles(dir, configFileName + "." + installedEx, System.IO.SearchOption.AllDirectories);
        //        if (installedConfigFiles.Length != 0)
        //        {
        //            System.IO.File.Copy(installedConfigFiles[0], installedConfigFiles[0].Substring(0, installedConfigFiles[0].Length - ("." + installedEx).Length));
        //        }
        //    }
        //}


        public override void Uninstall(IDictionary savedState)
        {
            base.Uninstall(savedState);
            try
            {
                //获取程序所安装的目录
                string dir = this.CurrentDir;
                string[] files = System.IO.Directory.GetFiles(dir, "uninstall.bat", System.IO.SearchOption.AllDirectories);
                if (files.Length != 0)
                {
                    Process p = new Process();
                    p.StartInfo.WorkingDirectory = new System.IO.FileInfo(files[0]).DirectoryName;
                    p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    p.StartInfo.FileName = "uninstall.bat";
                    p.Start();
                    p.WaitForExit();
                }
            }
            catch (Exception err)
            {
                System.Windows.Forms.MessageBox.Show(err.Message);
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
