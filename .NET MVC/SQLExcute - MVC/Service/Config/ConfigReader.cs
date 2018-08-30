using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using Acctrue.CMC.Web.WebDBInstall.DBViewModel;
using System.Xml.Serialization;

namespace Acctrue.CMC.Web.WebDBInstall.Config
{
    public class ConfigReader
    {
        static XmlDocument Xml = new XmlDocument();
        static string Path = "~/WebDBInstall/Config/DBInstall.xml";
        static ConfigReader()
        {
            Xml.Load(HttpContext.Current.Server.MapPath(Path));
        }

        public static Account GetAccount()
        {
            Xml.Load(HttpContext.Current.Server.MapPath(Path));
            XmlElement UserName = (XmlElement)Xml.SelectSingleNode("Root/User/Name");
            string UserNameVal = UserName.GetAttribute("Value");
            XmlElement PassWord = (XmlElement)Xml.SelectSingleNode("Root/User/PassWord");
            string PassWordVal = PassWord.GetAttribute("Value");
            Account acc = new Account(UserNameVal, PassWordVal);
            return acc;
        }

        public static string SetAccount(string UName,string UPwd,string DBver)
        {
            XmlElement UserName = (XmlElement)Xml.SelectSingleNode("Root/User/Name");
            UserName.SetAttribute("Value",UName);
            XmlElement PassWord = (XmlElement)Xml.SelectSingleNode("Root/User/PassWord");
            PassWord.SetAttribute("Value",UPwd);
            XmlElement appSettingValue = (XmlElement)Xml.SelectSingleNode("Root/AppSetting/Version");
            appSettingValue.SetAttribute("Value", DBver);
            Xml.Save(HttpContext.Current.Server.MapPath(Path));
            return "设置完成";
        }

        public static string GetAppSetting()
        {
            Xml.Load(HttpContext.Current.Server.MapPath(Path));
            XmlElement appSettingValue = (XmlElement)Xml.SelectSingleNode("Root/AppSetting/Version");
            return appSettingValue.GetAttribute("Value");
        }

        public static string Read(string key)
        {
            var obj = Context.AppSettings(key);
            return obj == null ? null : obj.ToString();
        }

        public static XmlToObject Context
        {
            get
            {
                return new XmlToObject(Path);
            }
        }

        public class XmlToObject : XmlArchiver
        {
            public XmlToObject(string config) : base(config, false, false) { }

            public string AppSettings(string key)
            {
                lock (_lock)
                {
                    var appSettings = document.DocumentElement.SelectNodes("appSettings");
                    if (appSettings == null || appSettings.Count <= 0) return null;
                    foreach (XmlNode node in appSettings[0].ChildNodes)
                    {
                        var attr = node.Attributes.Cast<XmlAttribute>().FirstOrDefault(d => d.Name == "key");
                        if (attr == null) continue;
                        var name = attr.Value;
                        if (string.IsNullOrEmpty(name)) continue;
                        if (!name.Equals(key)) continue;

                        attr = node.Attributes.Cast<XmlAttribute>().FirstOrDefault(d => d.Name == "value");
                        if (attr == null) return null;
                        return attr.Value;
                    }
                    return null;
                }
            }

            public List<T> Find<T>(string path) where T : new()
            {
                lock (_lock)
                {
                    var list = new List<T>();
                    var types = typeof(T);
                    var props = types.GetProperties().Where(d => !d.IsSpecialName && d.GetCustomAttributes(typeof(XmlElementAttribute), false).Length > 0).ToList();
                    var nodes = document.SelectNodes(path);

                    foreach (XmlNode node in nodes)
                    {
                        var t = new T();
                        foreach (var prop in props)
                        {
                            var attribute = prop.GetCustomAttributes(typeof(XmlElementAttribute), false)[0] as XmlElementAttribute;
                            var attributes = node.Attributes;
                            var nodeAttr = attributes.Cast<XmlAttribute>().FirstOrDefault(d => d.Name == attribute.ElementName);
                            if (nodeAttr == null) continue;
                            prop.SetValue(t, nodeAttr.Value, new object[0]);
                        }
                        list.Add(t);
                    }
                    return list;
                }
            }

            public class NameValue
            {
                [XmlElement("key")]
                public string Name { get; set; }
                [XmlElement("value")]
                public string Value { get; set; }
            }
        }
    }
}