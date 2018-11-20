using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using HtmlAgilityPack;
using log4net;
using System.Reflection;

namespace WebResourceInspector
{
    /// <summary>
    /// 链接地址和图片地址可访问性检查器
    /// </summary>
    public class Inspector
    {
        #region 私有变量

        private static object locker = new object();

        //日志记录
        private ILog log = null;
        //代理验证用户名
        private string userName = null;
        //代理验证密码
        private string password = null;
        //代理验证域
        private string domain = null;
        //代理地址
        private string proxyAddress = null;
        //代理端口
        private int proxyPort = 0;
        //超时时间
        private int timeout = 1000;
        //同时启动的线程总数
        private int totalThreadCount = 10;
        //当前启动的线程总数
        private int currentThreadCount = 0;

        //待检查的页面url队列，程序会对队列里面的url进一步抓取里面的信息
        private Queue<string> unVisitedPageUrlList = null;
        //已检查的页面url
        private List<string> visitedPageUrlList = null;
        //已检查的链接地址和图片地址
        private List<string> checkedResourceList = null;
        //存在错误的链接地址
        private List<string> errorList = null;

        //字符数字下划线开头的链接
        private Regex regex = new Regex("^[A-Za-z0-9_]+/");
        private Regex regHtmlFile = new Regex(@"^[A-Za-z0-9_-]+\.html$");

        #endregion

        #region 属性

        /// <summary>
        /// 网站入口地址
        /// </summary>
        public string EntranceUrl { get; set; }

        /// <summary>
        /// 忽略检查的地址
        /// </summary>
        public List<string> IgnoreUrl { get; set; }

        /// <summary>
        /// 页面编码
        /// </summary>
        public Encoding PageEncoding { get; set; }

        #endregion

        #region 构造函数

        public Inspector()
        {
            log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

            try
            {
                userName = ConfigurationManager.AppSettings["UserName"];
                password = ConfigurationManager.AppSettings["Password"];
                domain = ConfigurationManager.AppSettings["Domain"];
                proxyAddress = ConfigurationManager.AppSettings["ProxyAddress"];
                proxyPort = int.Parse(ConfigurationManager.AppSettings["ProxyPort"]);

                timeout = int.Parse(ConfigurationManager.AppSettings["Timeout"]);
                totalThreadCount = int.Parse(ConfigurationManager.AppSettings["TotalThreadCount"]);
            }
            catch { }
        }

        public Inspector(string entranceUrl, List<string> ignoreUrl)
            : base()
        {
            EntranceUrl = entranceUrl;
            IgnoreUrl = ignoreUrl;
        }

        #endregion

        /// <summary>
        /// 检查链接和图片的可访问性
        /// </summary>
        public void Parse()
        {
            if (EntranceUrl == null)
            {
                throw new ArgumentNullException("请确定网站入口地址！");
            }

            try
            {
                #region 处理首页

                //访问指定网站首页数据
                HttpWebResponse response = GetWebResponse(EntranceUrl);
                HtmlDocument document = null;

                //获取html数据
                using (Stream stream = response.GetResponseStream())
                {
                    document = new HtmlDocument();
                    document.Load(stream, PageEncoding);
                }

                //获取图片和链接地址
                HtmlNode rootNode = document.DocumentNode;
                HtmlNodeCollection linkList = rootNode.SelectNodes("//a");
                HtmlNodeCollection imgList = rootNode.SelectNodes("//img");

                unVisitedPageUrlList = new Queue<string>();
                visitedPageUrlList = new List<string>();
                checkedResourceList = new List<string>();
                errorList = new List<string>();

                //待检查的链接地址和图片地址队列，程序不会进一步抓取里面的信息
                Queue<string> unCheckedResourceList = new Queue<string>();
                //解析链接
                ParsePageUrl(EntranceUrl, unCheckedResourceList, linkList);
                //解析图片
                ParseResource(EntranceUrl, unCheckedResourceList, imgList);

                #endregion

                #region 检查链接和图片

                while (unCheckedResourceList.Count > 0)
                {
                    while (currentThreadCount == totalThreadCount)
                    {
                        Thread.Sleep(1000);
                    }

                    lock (unCheckedResourceList)
                    {
                        if (unCheckedResourceList.Count > 0)
                        {
                            ParamObject arg = new ParamObject();
                            arg.ResourceUrl = unCheckedResourceList.Dequeue();
                            arg.PageUrl = EntranceUrl;

                            Thread thread = new Thread(new ParameterizedThreadStart(ChcekResource));
                            thread.Start(arg);
                            currentThreadCount++;
                        }
                    }
                }

                #endregion

                #region 处理待处理队列中的所有链接

                while (unVisitedPageUrlList.Count > 0 || currentThreadCount > 0)
                {
                    while (currentThreadCount == totalThreadCount)
                    {
                        Thread.Sleep(1000);
                    }
                    lock (unVisitedPageUrlList)
                    {
                        if (unVisitedPageUrlList.Count > 0)
                        {
                            ParamObject arg = new ParamObject();
                            arg.PageUrl = unVisitedPageUrlList.Dequeue();

                            Thread thread = new Thread(new ParameterizedThreadStart(ChcekPageLink));
                            thread.Start(arg);
                            currentThreadCount++;
                        }
                        else
                        {
                            Thread.Sleep(1000);
                        }
                    }
                }

                #endregion

                log.Info("\r\n**************************************************\r\n【当前站点】:" + EntranceUrl + "\r\n【处理的地址总数】：" + visitedPageUrlList.Count + "\r\n【处理的资源总数】：" + checkedResourceList.Count + "\r\n【错误地址总数】：" + errorList.Count + "\r\n**************************************************\r\n");
                log.Logger.Repository.Shutdown();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 验证链接
        /// </summary>
        /// <param name="arg"></param>
        private void ChcekPageLink(object arg)
        {
            ParamObject obj = arg as ParamObject;
            string pageUrl = obj.PageUrl;

            try
            {
                HttpWebResponse response = GetWebResponse(pageUrl);
                HtmlDocument document = null;
                //获取html数据
                using (Stream stream = response.GetResponseStream())
                {
                    document = new HtmlDocument();
                    document.Load(stream, PageEncoding);
                }

                //获取图片和链接地址
                HtmlNode rootNode = document.DocumentNode;
                HtmlNodeCollection linkList = rootNode.SelectNodes("//a");
                HtmlNodeCollection imgList = rootNode.SelectNodes("//img");

                //将当前url加入已处理队列
                visitedPageUrlList.Add(pageUrl);

                //待检查的链接地址和图片地址队列，程序不会进一步抓取里面的信息
                Queue<string> unCheckedResourceList = new Queue<string>();
                //解析链接
                ParsePageUrl(pageUrl, unCheckedResourceList, linkList);
                //解析图片
                ParseResource(pageUrl, unCheckedResourceList, imgList);

                #region 检查当前页面下的链接和图片

                while (unCheckedResourceList.Count > 0)
                {
                    string resourceUrl = null;

                    lock (unCheckedResourceList)
                    {
                        if (unCheckedResourceList.Count > 0)
                        {
                            resourceUrl = unCheckedResourceList.Dequeue();
                        }
                    }

                    try
                    {
                        if (!checkedResourceList.Contains(resourceUrl))
                        {
                            response = GetWebResponse(resourceUrl);
                            int httpCode = (int)response.StatusCode;
                            if (httpCode != 200)
                            {
                                lock (errorList)
                                {
                                    if (!errorList.Contains(resourceUrl))
                                    {
                                        errorList.Add(resourceUrl);
                                        log.Error("发生错误链接：" + resourceUrl + Environment.NewLine + "\r\n所在页面：" + pageUrl + "\r\n" + "\r\n\r\n");
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        lock (errorList)
                        {
                            if (!errorList.Contains(resourceUrl))
                            {
                                errorList.Add(resourceUrl);
                                log.Error("发生错误链接：" + resourceUrl + "\r\n所在页面：" + pageUrl + "\r\n错误原因：" + ex.Message + "\r\n\r\n");
                            }
                        }
                    }
                    finally
                    {
                        lock (checkedResourceList)
                        {
                            if (!checkedResourceList.Contains(resourceUrl))
                            {
                                checkedResourceList.Add(resourceUrl);
                            }
                        }
                    }
                }

                #endregion
            }
            catch (Exception ex)
            {
                if (!errorList.Contains(pageUrl))
                {
                    errorList.Add(pageUrl);
                    log.Error("所在页面：" + pageUrl + "\r\n错误原因：" + ex.Message + "\r\n\r\n");
                }
            }
            finally
            {
                if (!visitedPageUrlList.Contains(pageUrl))
                {
                    visitedPageUrlList.Add(pageUrl);
                }

                lock (locker)
                {
                    currentThreadCount--;
                }
            }
        }

        /// <summary>
        /// 验证图片或者链接
        /// </summary>
        /// <param name="arg"></param>
        private void ChcekResource(object arg)
        {
            ParamObject obj = arg as ParamObject;
            string pageUrl = obj.PageUrl;
            string resourceUrl = obj.ResourceUrl;

            try
            {
                if (!checkedResourceList.Contains(resourceUrl))
                {
                    HttpWebResponse response = GetWebResponse(resourceUrl);
                    int httpCode = (int)response.StatusCode;
                    if (httpCode != 200)
                    {
                        lock (errorList)
                        {
                            if (!errorList.Contains(resourceUrl))
                            {
                                errorList.Add(resourceUrl);
                                log.Error("发生错误链接：" + resourceUrl + "\r\n所在页面：" + pageUrl + "\r\n\r\n");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lock (errorList)
                {
                    if (!errorList.Contains(resourceUrl))
                    {
                        errorList.Add(resourceUrl);
                        log.Error("发生错误链接：" + resourceUrl + "\r\n所在页面：" + pageUrl + "\r\n错误原因：" + ex.Message + "\r\n\r\n");
                    }
                }
            }
            finally
            {
                lock (checkedResourceList)
                {
                    if (!checkedResourceList.Contains(resourceUrl))
                    {
                        checkedResourceList.Add(resourceUrl);
                    }
                }

                lock (locker)
                {
                    currentThreadCount--;
                }
            }
        }

        /// <summary>
        /// 解析链接
        /// </summary>
        /// <param name="rootUrl">当前页面链接</param>
        /// <param name="unVisitedPageUrlList">待检查的页面url队列</param>
        /// <param name="unCheckedResourceList">待检查的链接或图片队列</param>
        /// <param name="visitedPageUrlList">已检查的页面url</param>
        /// <param name="checkedResourceList">已检查的链接或图片</param>
        /// <param name="linkList">待解析的url</param>
        private void ParsePageUrl(string currentUrl, Queue<string> unCheckedResourceList, HtmlNodeCollection linkList)
        {
            string rootUrl = EntranceUrl;
            //处理链接
            foreach (var link in linkList)
            {
                if (link.HasAttributes && link.Attributes["href"] != null && !string.IsNullOrEmpty(link.Attributes["href"].Value))
                {
                    string href = link.Attributes["href"].Value.ToLower();
                    //判断当前链接是否需要处理
                    if (string.IsNullOrEmpty(href) || href == "/" || href.StartsWith("#") || href == rootUrl || href.Contains("javascript")
                        || unVisitedPageUrlList.Contains(href) || visitedPageUrlList.Contains(href))
                    {
                        continue;
                    }

                    bool ignore = false;
                    //判断当前链接是否需要忽略
                    if (IgnoreUrl.Count > 0)
                    {
                        foreach (string ignoreUrl in IgnoreUrl)
                        {
                            if (href.StartsWith(ignoreUrl))
                            {
                                ignore = true;
                                break;
                            }
                        }
                    }

                    if (!ignore)
                    {
                        //为双斜杠开头的链接添加http:
                        if (href.StartsWith("//"))
                        {
                            href = "http:" + href;
                        }
                        //为www开头的链接添加http:
                        else if (href.StartsWith("www"))
                        {
                            href = "http://" + href;
                        }
                        //为单斜杠开头的链接添加网站地址
                        else if (href.StartsWith("/"))
                        {
                            href = rootUrl + href;
                        }
                        //为字符数字下划线开头的链接添加网站地址
                        else if (regex.IsMatch(href) || regHtmlFile.IsMatch(href))
                        {
                            href = currentUrl + "/" + href;
                        }
                        else if (href.StartsWith("../"))
                        {
                            currentUrl = currentUrl.TrimEnd('/');
                            if (currentUrl == rootUrl)
                            {
                                href = rootUrl + href.Replace("../", "/");
                            }
                            else
                            {
                                href = currentUrl.Substring(0, currentUrl.LastIndexOf('/')) + href.Replace("../", "/");
                            }
                        }

                        //只有当前站点下面的链接才需要进一步分析，进去待处理队列
                        //否则进去待检查队列，只检查链接是否能正常打开，不对内容进一步抓取
                        if (href.StartsWith(rootUrl))
                        {
                            lock (unVisitedPageUrlList)
                            {
                                if (!unVisitedPageUrlList.Contains(href) && !visitedPageUrlList.Contains(href))
                                {
                                    unVisitedPageUrlList.Enqueue(href);
                                }
                            }
                        }
                        else
                        {
                            lock (unCheckedResourceList)
                            {
                                if (!unCheckedResourceList.Contains(href) && !checkedResourceList.Contains(href))
                                {
                                    unCheckedResourceList.Enqueue(href);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 解析图片
        /// </summary>
        /// <param name="rootUrl">当前页面链接</param>
        /// <param name="unCheckedResourceList">待检查的链接或图片队列</param>
        /// <param name="checkedResourceList">已检查的页面url</param>
        /// <param name="linkList">待解析的url</param>
        private void ParseResource(string currentUrl, Queue<string> unCheckedResourceList, HtmlNodeCollection imgList)
        {
            string rootUrl = EntranceUrl;
            //处理图片
            foreach (var img in imgList)
            {
                if (img.HasAttributes && img.Attributes["src"] != null && !string.IsNullOrEmpty(img.Attributes["src"].Value))
                {
                    string imgUrl = img.Attributes["src"].Value.ToLower();

                    //如果图片或者链接已经入列或者已被处理，直接返回
                    if (unCheckedResourceList.Contains(imgUrl) || checkedResourceList.Contains(imgUrl))
                    {
                        continue;
                    }

                    //判断当前链接是否需要忽略
                    bool ignore = false;
                    if (IgnoreUrl.Count > 0)
                    {
                        foreach (string ignoreUrl in IgnoreUrl)
                        {
                            if (imgUrl.StartsWith(ignoreUrl))
                            {
                                ignore = true;
                                break;
                            }
                        }
                    }

                    if (!ignore)
                    {
                        //为双斜杠开头的链接添加http:
                        if (imgUrl.StartsWith("//"))
                        {
                            imgUrl = "http:" + imgUrl;
                        }
                        //为www开头的链接添加http:
                        else if (imgUrl.StartsWith("www"))
                        {
                            imgUrl = "http://" + imgUrl;
                        }
                        //为单斜杠开头的链接添加网站地址
                        else if (imgUrl.StartsWith("/"))
                        {
                            imgUrl = rootUrl + imgUrl;
                        }
                        //为字符数字下划线开头的链接添加网站地址
                        else if (regex.IsMatch(imgUrl) || regHtmlFile.IsMatch(imgUrl))
                        {
                            imgUrl = currentUrl + "/" + imgUrl;
                        }
                        else if (imgUrl.StartsWith("../"))
                        {
                            currentUrl = currentUrl.TrimEnd('/');
                            if (currentUrl == rootUrl)
                            {
                                imgUrl = rootUrl + imgUrl.Replace("../", "/");
                            }
                            else
                            {
                                imgUrl = currentUrl.Substring(0, currentUrl.LastIndexOf('/')) + imgUrl.Replace("../", "/");
                            }
                        }

                        //入列
                        lock (checkedResourceList)
                        {
                            if (!unCheckedResourceList.Contains(imgUrl) || !checkedResourceList.Contains(imgUrl))
                            {
                                unCheckedResourceList.Enqueue(imgUrl);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 生成HttpWebRequest
        /// </summary>
        /// <returns></returns>
        private HttpWebResponse GetWebResponse(string entranceUrl)
        {
            HttpWebResponse response = null;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(entranceUrl));
                request.Timeout = timeout;
                request.ServicePoint.Expect100Continue = false;
                request.Method = "GET";
                request.Accept = "*/*";
                request.KeepAlive = true;
                request.Timeout = timeout;
                request.UserAgent = "Mozilla-Firefox";

                //判断是否启用代理
                if (!string.IsNullOrEmpty(proxyAddress) && proxyPort != 0
                     && !string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(domain))
                {
                    var proxy = new WebProxy(proxyAddress, proxyPort);
                    proxy.Credentials = new NetworkCredential(userName, password, domain);
                    request.Proxy = proxy;
                }

                response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException ex)
            {
                throw ex;
            }

            return response;
        }

        /// <summary>
        /// 参数类
        /// </summary>
        class ParamObject
        {
            //当前页面地址
            public string PageUrl { get; set; }
            //所访问的图片或者link
            public string ResourceUrl { get; set; }
        }
    }
}
