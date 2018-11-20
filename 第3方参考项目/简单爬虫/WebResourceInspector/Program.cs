using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using System.Configuration;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace WebResourceInspector
{
    class Program
    {
        static void Main(string[] args)
        {
            #region 检查网站

            //删除日志文件
            string filePath = ConfigurationManager.AppSettings["EmailFilePath"];
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            //获取待检查的网站
            var items = from item in XElement.Load("config.xml").Elements("item") select item;
            //轮训处理
            foreach (var item in items)
            {
                try
                {
                    //网站入口地址
                    string entranceUrl = item.Element("entrance_url").Value;
                    //页面编码格式
                    Encoding encoding = Encoding.GetEncoding(item.Element("encoding").Value);
                    //获取可忽略分析的网站地址
                    List<string> urlList = new List<string>();
                    foreach (var url in item.Element("ignore_urls").Elements("url"))
                    {
                        if (!string.IsNullOrEmpty(url.Value))
                        {
                            urlList.Add(url.Value);
                        }
                    }

                    Inspector inspector = new Inspector();
                    inspector.EntranceUrl = entranceUrl;
                    inspector.PageEncoding = encoding;
                    inspector.IgnoreUrl = urlList;

                    try
                    {
                        //开始处理
                        Console.WriteLine("站点正在检查：" + entranceUrl);
                        inspector.Parse();
                        Console.WriteLine("站点检查完毕：" + entranceUrl + Environment.NewLine);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("站点检查出错：" + entranceUrl + "，原因：" + ex.Message + Environment.NewLine);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            Console.WriteLine("所有站点检查完毕！");

            #endregion

            try
            {
                //发送邮件
                EmailHelper emailHelper = new EmailHelper();
                emailHelper.Send();
                Console.WriteLine("邮件发送成功！");
            }
            catch (Exception ex)
            {
                Console.WriteLine("邮件发送失败，原因：" + ex.Message);
            }

            Console.Read();
        }
    }
}
