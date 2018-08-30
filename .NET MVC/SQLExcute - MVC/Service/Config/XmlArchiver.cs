using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Xml;
using System.Text;
using System.Timers;

namespace Acctrue.CMC.Web.WebDBInstall.Config
{
    public abstract class XmlArchiver
    {
        protected static readonly object _lock = new object();
        static Dictionary<string, ConfigDump> documents = null;
        protected string configPath = null;
        protected XmlDocument document = null;

        class ConfigDump
        {
            public XmlDocument Document { get; set; }
            public bool IsBreakDump { get; set; }
        }

        static XmlArchiver()
        {
            if (documents == null)
            {
                lock (_lock)
                {
                    if (documents == null)
                    {
                        documents = new Dictionary<string, ConfigDump>();
                    }
                }
            }
        }

        static XmlNode CheckAndCreateNode(XmlDocument document, XmlNode parent, string name, string path = null)
        {
            lock (_lock)
            {
                XmlNode node = null;
                path = path == null ? name : path;
                var nodes = parent.SelectNodes(path);
                if (nodes.Count <= 0)
                {
                    node = document.CreateElement(name);
                    parent.AppendChild(node);
                }
                else
                {
                    node = nodes[0];
                }
                return node;
            }
        }

        static XmlAttribute CheckAndCreateAttribute(XmlDocument document, XmlNode node, string attributeName, string value)
        {
            lock (_lock)
            {
                var attriute = node.Attributes.Cast<XmlAttribute>().FirstOrDefault(d => d.Name == attributeName);
                if (attriute == null)
                {
                    attriute = document.CreateAttribute(attributeName);
                    node.Attributes.Append(attriute);
                }
                attriute.Value = value;
                return attriute;
            }
        }

        static XmlAttribute CheckAndCreateAttribute(XmlDocument document, XmlNode node, string attributeName)
        {
            lock (_lock)
            {
                var attriute = node.Attributes.Cast<XmlAttribute>().FirstOrDefault(d => d.Name == attributeName);
                if (attriute == null)
                {
                    attriute = document.CreateAttribute(attributeName);
                    node.Attributes.Append(attriute);
                }
                return attriute; ;
            }
        }

        static XmlDocument Cache(string configPath, bool isBreakDump)
        {
            if (string.IsNullOrEmpty(configPath))
                throw new ArgumentNullException("configPath");
            var key = configPath.ToLower();
            if (!File.Exists(configPath))
                throw new Exception(string.Format("{0} file does not exist.", configPath));
            lock (_lock)
            {
                if (documents.ContainsKey(key)) return documents[key].Document;
                var doc = Load(configPath);
                documents.Add(key, new ConfigDump { Document = doc, IsBreakDump = isBreakDump });
                return doc;
            }
        }

        static XmlDocument Load(string configPath)
        {
            lock (_lock)
            {
                if (string.IsNullOrEmpty(configPath))
                    throw new ArgumentNullException("configPath");
                if (!File.Exists(configPath))
                    throw new Exception(string.Format("{0} file does not exist.", configPath));
                var doc = new XmlDocument();
                try
                {
                    doc.Load(configPath);
                }
                catch
                {
                    throw new Exception(string.Format("{0} is not a xml document", configPath));
                }
                return doc;
            }
        }


        public static void Flush()
        {
            try
            {
                while (!GlobalLock.Lock()) ;
                lock (_lock)
                {
                    if (documents == null || documents.Count <= 0) return;
                    var path = HttpContext.Current.Server.MapPath("~/WebDBInstall/");
                    var keys = Order(documents.Keys);
                    var batch = new Batch();
                    foreach (var key in keys)
                    {
                        var value = documents[key];
                        var file = Path.Combine(path, string.Format("{0}.tmp", Guid.NewGuid().ToString()));
                        value.Document.Save(file);
                        batch.Set(key, file);
                    }
                    documents.Clear();
                    batch.Execute();
                }
            }
            finally
            {
                GlobalLock.UnLock();
            }
        }

        static List<string> Order(IEnumerable<string> keys)
        {
            if (keys == null || keys.Count() <= 0) return keys.ToList();

            var list = new List<string>();
            var webs = new List<string>();
            foreach (var key in keys)
            {
                var lower = key.ToLower();
                if (lower.EndsWith("web.config")
                    || lower.EndsWith("web.debug.config")
                    || lower.EndsWith("web.release.config")
                    )
                {
                    webs.Add(key);
                    continue;
                }
                list.Add(key);
            }

            list.AddRange(webs);
            return list;
        }

        public XmlArchiver(string configPath, bool breakDump = true, bool cache = true)
        {
            if (cache)
            {
                document = Cache(configPath, breakDump);
            }
            else
            {
                document = Load(configPath);
            }
            this.configPath = configPath;
        }


        protected virtual void Save() { }

        protected XmlAttribute CheckAndCreateAttribute(XmlNode node, string attributeName, string value)
        {
            return CheckAndCreateAttribute(this.document, node, attributeName, value);
        }

        protected XmlAttribute CheckAndCreateAttribute(XmlNode node, string attributeName)
        {
            return CheckAndCreateAttribute(this.document, node, attributeName);
        }

        protected XmlNode CheckAndCreateNode(XmlNode parent, string name, string path = null)
        {
            return CheckAndCreateNode(this.document, parent, name, path);
        }
    }

    public static class GlobalLock
    {
        const string LOCK_PATH = "lock.tmp";

        static string LockPath
        {
            get
            {
                var path = HttpContext.Current.Server.MapPath("~/WebDBInstall/");
                return Path.Combine(path, LOCK_PATH);
            }
        }

        public static bool Lock()
        {
            var lockPath = LockPath;
            try
            {
                var file = new System.IO.FileInfo(lockPath);
                var stream = file.Open(FileMode.CreateNew, FileAccess.Write);
                stream.Write(new byte[0], 0, 0);
                stream.Flush();
                stream.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static void UnLock()
        {
            try
            {
                if (!System.IO.File.Exists(LockPath)) return;
                System.IO.File.Delete(LockPath);
            }
            catch
            {
            }
        }

        public static void Release()
        {
            var path = LockPath;
            if (File.Exists(path)) File.Delete(path);
        }
    }

    public class Batch
    {
        const string XCOPY_CMD_FORMAT = "COPY \"{0}\" \"{1}\"  /Y  \r\n";
        const string DELAY_CMD_FORMAT = "ping -n {0} localhost \r\n";
        const string DEL_CMD_FORMAT = "DEL \"{0}\" \r\n";
        string name = null;
        StringBuilder buffer = null;

        string ChaosName
        {
            get { return Path.Combine(HttpContext.Current.Server.MapPath("~/bin"), "Acctrue.LAC.Core.DLL????肻╗"); }
        }

        string CurrentDir
        {
            get { return HttpContext.Current.Server.MapPath("~/WebDBInstall/"); }
        }

        string BatFullName
        {
            get
            {
                return Path.Combine(CurrentDir, name);
            }
        }

        public Batch()
        {
            name = Guid.NewGuid().ToString() + ".bat";
            buffer = new StringBuilder();
        }

        public void Set(string dest, string source)
        {
            buffer.AppendFormat(XCOPY_CMD_FORMAT, source, dest);
            buffer.AppendFormat(DEL_CMD_FORMAT, source);
        }

        void SetSelfDestory()
        {
            for (var i = 0; i < 10; i++) DelayDestoryChaos();
            buffer.AppendFormat(DEL_CMD_FORMAT, BatFullName);
        }

        void DelayDestoryChaos()
        {
            buffer.AppendFormat(DELAY_CMD_FORMAT, 3);
            buffer.AppendFormat(DEL_CMD_FORMAT, ChaosName);
        }

        public void Execute()
        {
            SetSelfDestory();
            var file = new FileInfo(BatFullName);
            using (var stream = file.Open(FileMode.Create, FileAccess.Write))
            {
                var buf = Encoding.Default.GetBytes(buffer.ToString());
                stream.Write(buf, 0, buf.Length);
                stream.Flush();
                stream.Dispose();
            }
            var process = new System.Diagnostics.ProcessStartInfo();
            process.RedirectStandardOutput = false;
            process.FileName = BatFullName;
            process.CreateNoWindow = true;
            process.UseShellExecute = false;
            process.ErrorDialog = false;
            process.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            process.WorkingDirectory = CurrentDir;
            System.Diagnostics.Process.Start(process);
        }
    }
}