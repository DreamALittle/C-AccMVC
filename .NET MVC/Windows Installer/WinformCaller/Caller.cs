using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;
using System.Reflection;
using System.IO;
using System.Configuration;

namespace Acctrue.TTS
{
    public class Caller
    {
        static void Main(string[] args)
        {
            //Console.Read();

            string currentDirectory = System.Environment.CurrentDirectory;

            InitializeConfigFiles(currentDirectory);

            DirectoryInfo folder = new DirectoryInfo(currentDirectory);
            FileInfo[] file = folder.GetFiles("*Service.exe");
            if (file.Length > 0)
            {
                string utilConfigFile = currentDirectory + "\\acctrueTTSConfigUtil.exe.config";

                string[] strArray = file[0].Name.Split(new string[] { "TTS", "Service" }, StringSplitOptions.RemoveEmptyEntries);

                var productName = strArray[1] + "服务";
                var serviceName = strArray[1];

                var configName = file[0].Name + ".config";
                string configPath = System.IO.Path.Combine(currentDirectory, configName);

                new Acctrue.TTS.Configuration.ConfigForm(utilConfigFile, productName, serviceName, configPath).ShowDialog();
            }

            //执行服务BAT文件
            string[] files = System.IO.Directory.GetFiles(currentDirectory, "install.bat", System.IO.SearchOption.AllDirectories);
            if (files.Length != 0)
            {
                Process p = new Process();
                p.StartInfo.WorkingDirectory = new System.IO.FileInfo(files[0]).DirectoryName;
                p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                p.StartInfo.FileName = "install.bat";
                p.Start();
            }    

            Application.Exit();
        }

        private static void InitializeConfigFiles(string dir)
        {
            foreach (string installedFile in System.IO.Directory.GetFiles(dir, "*.config.installed", System.IO.SearchOption.AllDirectories))
            {
                string installedFileName = installedFile.Remove(0, installedFile.LastIndexOf('\\') + 1);
                string configFileName = installedFileName.Substring(0, installedFileName.LastIndexOf('.'));
                CreateConfigFile(dir, configFileName, "installed");
            }
        }
        private static void CreateConfigFile(string dir, string configFileName, string installedEx)
        {
            string[] configFiles = System.IO.Directory.GetFiles(dir, configFileName, System.IO.SearchOption.AllDirectories);
            if (configFiles.Length == 0)
            {
                string[] installedConfigFiles = System.IO.Directory.GetFiles(dir, configFileName + "." + installedEx, System.IO.SearchOption.AllDirectories);
                if (installedConfigFiles.Length != 0)
                {
                    System.IO.File.Copy(installedConfigFiles[0], installedConfigFiles[0].Substring(0, installedConfigFiles[0].Length - ("." + installedEx).Length));
                }
            }
        }
        private static string CurrentDir
        {
            get
            {
                string assemblyFile = Assembly.GetExecutingAssembly().CodeBase.Remove(0, "file:///".Length).Replace('/', '\\');
                return assemblyFile.Substring(0, assemblyFile.LastIndexOf('\\'));
            }
        }
    }
}
