using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.IO;
using System.Windows.Forms;

namespace WebSetpInstaller
{
    [RunInstaller(true)]
    public partial class WebInstaller : System.Configuration.Install.Installer
    {
        public WebInstaller()
        {
            InitializeComponent();
        }

        protected override void OnAfterInstall(IDictionary savedState)
        {
            string currentPath = Path.GetDirectoryName(this.Context.Parameters["assemblypath"]);
            string oldConfigFilepath = currentPath + @"\WebBackup.config";
            if (File.Exists(oldConfigFilepath))
            {
                MessageBoxButtons mess = MessageBoxButtons.OKCancel;
                DialogResult dr = MessageBox.Show("是否导入之前版本配置？", "提示", mess);
                if (dr == DialogResult.OK)
                {
                    string targetFilePath = currentPath + @"\Web.config";
                    File.Copy(oldConfigFilepath, targetFilePath, true);
                }
            }
        }

        public override void Uninstall(IDictionary savedState)
        {
            MessageBoxButtons mess = MessageBoxButtons.OKCancel;
            DialogResult dr = MessageBox.Show("是否保留配置文件？", "提示", mess);
            if (dr == DialogResult.OK)
            {
                string currentPath = Path.GetDirectoryName(this.Context.Parameters["assemblypath"]);
                string backupConfigFilepath = currentPath + @"\WebBackup.config";
                string configFilePath = currentPath + @"\Web.config";
                File.Copy(configFilePath, backupConfigFilepath, true);
            }

            base.Uninstall(savedState);
        }
    }
}
