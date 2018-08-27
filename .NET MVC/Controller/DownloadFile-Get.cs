[HttpGet]
        public HttpResponseMessage DownloadFile()
        {
            string url = System.Web.HttpContext.Current.Request.RawUrl;
            string browser = HttpContext.Current.Request.Browser.Browser;
            string fileName = "";
            try
            {
                int ApplyID = Convert.ToInt32(url.Split(new char[] { '?' })[1]);

                //IE 8 与 IE 11
                if (browser == "IE" || browser == "InternetExplorer")
                {
                    fileName = System.Web.HttpUtility.UrlEncode("ID" + ApplyID.ToString() + "-码文件.zip", System.Text.Encoding.UTF8);
                }
                else
                {
                    fileName = "ID" + ApplyID.ToString() + "-码文件.zip";
                }

                byte[] content = CodeDownloadFactory.Instance.GetInnerCodeFileRecord(ApplyID).Content;
                var stream = new MemoryStream(content);

                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StreamContent(stream);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = fileName
                };

                //增加一次下载次数
                string name = (string)System.Web.HttpContext.Current.Session["UserName"];
                if (name.IsNullOrEmpty())
                {
                    name = HttpContext.Current.Request.UserHostAddress+"匿名下载";
                }
                CodeDownloadFactory.Instance.UpdateDownloadRecord(ApplyID , name);
                return response;
            }
            catch
            {
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.NotFound);
                return response;
            }
        }
