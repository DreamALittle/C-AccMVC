using Acctrue.CMC.Factory.Search;
using Acctrue.CMC.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Acctrue.CMC.Model.Code;
using Acctrue.CMC.Factory.Code;
using Newtonsoft.Json;
using System.IO;
using System.Net.Http.Headers;
using System.Web;
using NPOI;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using Acctrue.CMC.Model.Report;
using Acctrue.CMC.Factory.Report;


namespace Acctrue.CMC.Web
{
    public class CodeController : ValuesController
    {
        /// <summary>
        /// 获取所有申请记录
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        [HttpPost]
        public CMCResponse SearchCodeApplyList(Dictionary<string, object> dic)
        {
            int count = 0;
            Response.data = SearchFactory.Instance.SearchCodeApplyList(dic, out count);
            Response.count = count;
            return Response;
        }
        /// <summary>
        /// 申请记录导出Excel
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        [HttpPost]
        public CMCResponse SearchCodeApplyListAndDownloadExcel(Dictionary<string, object> dic)
        {
            int count = 0;
            List<CodeApply> applyList = SearchFactory.Instance.SearchCodeApplyListWithName(dic, out count);
            if (count == 0)
            {
                Response.data = "无数据无法生成Excel文件";
            }
            else
            {
                //生成Excel临时文件
                string Path = HttpContext.Current.Server.MapPath("~/ExcelUpload/") + "TempExcel.xlsx";

                IWorkbook workbook = new HSSFWorkbook();
                workbook.CreateSheet("申请记录");
                using (FileStream fs = File.Create(Path))
                {
                    ISheet sheet = workbook.GetSheetAt(0);
                    //设置数据
                    int LineNumber = 0;
                    int RowNumber = 0;
                    //设置表头
                    sheet.CreateRow(LineNumber);
                    IRow row = sheet.GetRow(LineNumber++);
                    row.CreateCell(RowNumber++).SetCellValue("申请ID");
                    row.CreateCell(RowNumber++).SetCellValue("申请企业编码");
                    row.CreateCell(RowNumber++).SetCellValue("申请企业名称");
                    row.CreateCell(RowNumber++).SetCellValue("使用企业编码");
                    row.CreateCell(RowNumber++).SetCellValue("使用企业名称");
                    row.CreateCell(RowNumber++).SetCellValue("申请产品编码");
                    row.CreateCell(RowNumber++).SetCellValue("申请产品名称");
                    row.CreateCell(RowNumber++).SetCellValue("包装比例");
                    row.CreateCell(RowNumber++).SetCellValue("申请数量");
                    row.CreateCell(RowNumber++).SetCellValue("申请时间");
                    row.CreateCell(RowNumber++).SetCellValue("申请状态");
                    row.CreateCell(RowNumber++).SetCellValue("申请人");
                    row.CreateCell(RowNumber++).SetCellValue("审核时间");
                    row.CreateCell(RowNumber++).SetCellValue("审核人");
                    row.CreateCell(RowNumber++).SetCellValue("审核说明");
                    row.CreateCell(RowNumber++).SetCellValue("CMC审核");
                    row.CreateCell(RowNumber++).SetCellValue("申请使用码规则");
                    RowNumber = 0;
                    foreach (CodeApply apply in applyList)
                    {
                        sheet.CreateRow(LineNumber);
                        row = sheet.GetRow(LineNumber++);
                        //设置数值
                        row.CreateCell(RowNumber++).SetCellValue(apply.ApplyId);
                        row.CreateCell(RowNumber++).SetCellValue(apply.ApplyCorpCode);
                        row.CreateCell(RowNumber++).SetCellValue(apply.ApplyCorpName);
                        row.CreateCell(RowNumber++).SetCellValue(apply.UseCorpCode);
                        row.CreateCell(RowNumber++).SetCellValue(apply.UseCorpName);
                        row.CreateCell(RowNumber++).SetCellValue(apply.ProductCode);
                        row.CreateCell(RowNumber++).SetCellValue(apply.ProductName);
                        row.CreateCell(RowNumber++).SetCellValue(apply.OuterPackage);
                        row.CreateCell(RowNumber++).SetCellValue(apply.ApplyAmount);
                        row.CreateCell(RowNumber++).SetCellValue(apply.ApplyDate.ToString());
                        row.CreateCell(RowNumber++).SetCellValue(apply.ApplyStatus.GetEnumDes());
                        row.CreateCell(RowNumber++).SetCellValue(apply.Applier);
                        if (apply.AuditDate.HasValue)
                        {
                            row.CreateCell(RowNumber++).SetCellValue(Convert.ToDateTime( apply.AuditDate).ToString());
                        }
                        else
                        {
                            row.CreateCell(RowNumber++).SetCellValue("");
                        }
                        row.CreateCell(RowNumber++).SetCellValue(apply.Auditor);
                        row.CreateCell(RowNumber++).SetCellValue(apply.AuditMessage);
                        if (apply.CMCAudit)
                        {
                            row.CreateCell(RowNumber++).SetCellValue("已审核");
                        }
                        else
                        {
                            row.CreateCell(RowNumber++).SetCellValue("未审核");
                        }
                        row.CreateCell(RowNumber++).SetCellValue(apply.CodeRulesIDs);
                        RowNumber = 0;
                    }

                    workbook.Write(fs);
                }
                Response.data = "TempExcel.xlsx";
            }
            return Response;
        }
        /// <summary>
        /// 获取单个申请记录
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        [HttpPost]
        public CMCResponse GetApplyDetail(Dictionary<string, object> dic)
        {
            int count = 0;
            Response.data = SearchFactory.Instance.GetApplyByID(dic);
            Response.count = count;
            return Response;
        }
        /// <summary>
        /// 获取下载详细记录
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        [HttpPost]
        public CMCResponse GetDownloadDetail(Dictionary<string, object> dic)
        {
            CodeDownloadRecord[] arr = SearchFactory.Instance.GetDownload(dic);
            Response.data = arr;
            Response.count = arr.Length;
            return Response;
        }
        /// <summary>
        /// CMC更新审核申请记录
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        [HttpPost]
        public CMCResponse UpdateApply(Dictionary<string, object> dic)
        {
            string json = dic["data"].ToString();
            CodeApply c = JsonConvert.DeserializeObject<CodeApply>(json);
            Response.data = CodeApplyFactory.Instance.WebUpdate(c);
            return Response;
        }
        /// <summary>
        /// CMC下载码文件
        /// </summary>
        /// <returns></returns>
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
        /// <summary>
        /// 申请记录下载
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage DownloadExcelFile()
        {
            string Path = HttpContext.Current.Server.MapPath("~/ExcelUpload/") + "TempExcel.xlsx";
            string fileName = "";
            if (HttpContext.Current.Request.Browser.Browser == "IE" || HttpContext.Current.Request.Browser.Browser == "InternetExplorer")
            {
                fileName = System.Web.HttpUtility.UrlEncode("申请记录.xlsx", System.Text.Encoding.UTF8);
            }
            else
            {
                fileName = "申请记录.xlsx";
            }

            //读取文件
            try
            {
                using (FileStream fs = new FileStream(Path, FileMode.Open))
                {
                    byte[] contentBytes = new byte[fs.Length];
                    fs.Read(contentBytes, 0, (int)fs.Length);

                    var stream = new MemoryStream(contentBytes);
                    HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                    response.Content = new StreamContent(stream);
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                    {
                        FileName = fileName
                    };

                    return response;
                }
            }
            catch
            {
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.NotFound);
                return response;
            }
            finally
            {
                try
                {
                    File.Delete(Path);
                }
                catch { }
            }
           
        }
        /// <summary>
        /// 上传Excel文件
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public CMCResponse UploadExcelFile()
        {
            try {
                //保存文件
                var file = System.Web.HttpContext.Current.Request.Files[0];
                string fullPath = HttpContext.Current.Server.MapPath("~/ExcelUpload/") + file.FileName;
                file.SaveAs(fullPath);
                
                //获取文件数据
                using (FileStream fs = new FileStream(fullPath, FileMode.Open))
                {
                    IWorkbook workbook = new XSSFWorkbook(fs);
                    ISheet sheet = workbook.GetSheetAt(0);
                    int lastRowIndex = sheet.LastRowNum;

                    //第一行获取活动名称
                    IRow row = sheet.GetRow(0);
                    int rowLastIndex = row.LastCellNum - 1;
                    string activeName = row.GetCell(rowLastIndex).StringCellValue;
                    int lastColumn = 0;
                    switch (activeName)
                    {
                        case "激活":
                            lastColumn = 9;
                            
                            break;
                        case "营销":
                            lastColumn = 12;

                            break;
                        case "消费者":
                            lastColumn = 16;

                            break;
                        case "资产":
                            lastColumn = 18;

                            break;
                        case "资产领用":
                            lastColumn = 21;

                            break;
                        default:
                            Response.status = 1;
                            Response.message = "上传活动名称有误";
                            return Response;
                    }
                    //第3/4行获取名称基本信息 
                    //List<string> nameList = new List<string>();
                    //List<string> basicInfo = new List<string>();
                    //for (int nameIndex = 0; nameIndex < lastColumn; nameIndex++)
                    //{
                    //    row = sheet.GetRow(2);
                    //    nameList.Add(row.GetCell(nameIndex).StringCellValue);
                    //}
                    
                    //生成上传活动信息存入数据库
                    byte[] byteArray = new byte[file.ContentLength];
                    file.InputStream.Read(byteArray,0,file.ContentLength);
                    //fs.Read(byteArray, 0, Convert.ToInt32(fs.Length));

                    IRow row3 = sheet.GetRow(3);
                    CodeActive codeActive = new CodeActive();
                    codeActive.AppId = "Excel文件上传";
                    codeActive.CorpCode = row3.GetCell(5).StringCellValue;
                    codeActive.CorpName = row3.GetCell(4).StringCellValue;
                    codeActive.SubCorpCode = row3.GetCell(6).StringCellValue;
                    codeActive.ProductCode = row3.GetCell(8).StringCellValue;
                    codeActive.ProductName = row3.GetCell(7).StringCellValue;
                    codeActive.ProduceWorkline = row3.GetCell(6).StringCellValue;
                    codeActive.ActivityName = activeName;
                    codeActive.Amount = Convert.ToString(lastRowIndex-2);
                    codeActive.ActualQuantity = Convert.ToString(lastRowIndex - 2);
                    codeActive.UploadDate = DateTime.Now;
                    codeActive.Uploader = "CMC后台";
                    codeActive.ProcessType = 3;
                    codeActive.Memo = row3.GetCell(9).StringCellValue;
                    codeActive.ApplyId = 0;

                    int activeID = CodeActiveUploadFactory.Instance.InsertNewActive(codeActive,byteArray);
                    if (activeName == "激活")
                    {
                        List<ActiveCode> alist = new List<ActiveCode>();
                        //第4行开始获取数据
                        for (int index = 3; index <= lastRowIndex; index++)
                        {
                            //获取当前行
                            row = sheet.GetRow(index);
                            //生成Mongo对象存码信息

                            ActiveCode c = new ActiveCode();
                            c.ActiveName = activeName;
                            c.CreateDate = row.GetCell(2).DateCellValue; ;
                            c.ApplyId = codeActive.ApplyId;
                            c.CodeActivityId = activeID;
                            c.CorpCode = codeActive.CorpCode;
                            c.SubCorpCode = codeActive.SubCorpCode;
                            c.CorpName = codeActive.CorpName;
                            try
                            {
                                c.Code = row.GetCell(0).StringCellValue;
                            }
                            catch
                            {
                                c.Code = row.GetCell(0).NumericCellValue.ToString();
                            }
                            //c.MaskCode = row.GetCell(1).NumericCellValue.ToString();
                            c.ProductCode = row.GetCell(8).StringCellValue;
                            c.ProductName = row.GetCell(7).StringCellValue;
                            c.Memo = row.GetCell(9).StringCellValue;
                            alist.Add(c);
                        }
                        string res = ReportFactory.Instance.SplitMongoInster<ActiveCode>(alist);
                        if (res == "数据插入MongoDB失败")
                        {
                            Response.status = 0;
                            this.Response.message = res+"，检查MongoDB是否正常";
                            return Response;
                        }
                    }
                    if (activeName == "营销")
                    {
                        List<SalesCode> alist = new List<SalesCode>();
                        //第4行开始获取数据
                        for (int index = 3; index <= lastRowIndex; index++)
                        {
                            //获取当前行
                            row = sheet.GetRow(index);
                            //生成Mongo对象存码信息

                            SalesCode c = new SalesCode();
                            c.ActiveName = activeName;
                            c.CreateDate = row.GetCell(2).DateCellValue; ;
                            c.ApplyId = codeActive.ApplyId;
                            c.CodeActivityId = activeID;
                            c.CorpCode = codeActive.CorpCode;
                            c.SubCorpCode = codeActive.SubCorpCode;
                            c.CorpName = codeActive.CorpName;
                            try
                            {
                                c.Code = row.GetCell(0).StringCellValue;
                            }
                            catch
                            {
                                c.Code = row.GetCell(0).NumericCellValue.ToString();
                            }
                            //c.MaskCode = row.GetCell(1).NumericCellValue.ToString();
                            c.ProductCode = row.GetCell(8).StringCellValue;
                            c.ProductName = row.GetCell(7).StringCellValue;
                            c.Memo = row.GetCell(9).StringCellValue;
                            c.ActiveDescription= row.GetCell(10).StringCellValue;
                            c.ActiveStartDate = row.GetCell(11).DateCellValue;
                            c.ActiveEndDate= row.GetCell(12).DateCellValue;
                            alist.Add(c);
                        }
                        string res = ReportFactory.Instance.SplitMongoInster<SalesCode>(alist);
                        if (res == "数据插入MongoDB失败")
                        {
                            Response.status = 0;
                            this.Response.message = res + "，检查MongoDB是否正常";
                            return Response;
                        }
                    }
                    if (activeName == "消费者")
                    {
                        List<CustomerCode> alist = new List<CustomerCode>();
                        //第4行开始获取数据
                        for (int index = 3; index <= lastRowIndex; index++)
                        {
                            //获取当前行
                            row = sheet.GetRow(index);
                            //生成Mongo对象存码信息

                            CustomerCode c = new CustomerCode();
                            c.ActiveName = activeName;
                            c.CreateDate = row.GetCell(2).DateCellValue; ;
                            c.ApplyId = codeActive.ApplyId;
                            c.CodeActivityId = activeID;
                            c.CorpCode = codeActive.CorpCode;
                            c.SubCorpCode = codeActive.SubCorpCode;
                            c.CorpName = codeActive.CorpName;
                            try
                            {
                                c.Code = row.GetCell(0).StringCellValue;
                            }
                            catch
                            {
                                c.Code = row.GetCell(0).NumericCellValue.ToString();
                            }
                            //c.MaskCode = row.GetCell(1).NumericCellValue.ToString();
                            c.ProductCode = row.GetCell(8).StringCellValue;
                            c.ProductName = row.GetCell(7).StringCellValue;
                            c.Memo = row.GetCell(9).StringCellValue;
                            c.ActiveDescription = row.GetCell(10).StringCellValue;
                            c.ActiveStartDate = row.GetCell(11).DateCellValue;
                            c.ActiveEndDate = row.GetCell(12).DateCellValue;
                            c.CustomerOpenID = row.GetCell(13).StringCellValue;
                            c.CustomerTime= row.GetCell(14).DateCellValue;
                            c.CustomerLocatio= row.GetCell(15).StringCellValue;
                            c.WhatActive= row.GetCell(16).StringCellValue;
                            alist.Add(c);
                        }
                        string res = ReportFactory.Instance.SplitMongoInster<CustomerCode>(alist);
                        if (res == "数据插入MongoDB失败")
                        {
                            Response.status = 0;
                            this.Response.message = res + "，检查MongoDB是否正常";
                            return Response;
                        }
                    }
                    if (activeName == "消费者")
                    {
                        List<CustomerCode> alist = new List<CustomerCode>();
                        //第4行开始获取数据
                        for (int index = 3; index <= lastRowIndex; index++)
                        {
                            //获取当前行
                            row = sheet.GetRow(index);
                            //生成Mongo对象存码信息

                            CustomerCode c = new CustomerCode();
                            c.ActiveName = activeName;
                            c.CreateDate = row.GetCell(2).DateCellValue; ;
                            c.ApplyId = codeActive.ApplyId;
                            c.CodeActivityId = activeID;
                            c.CorpCode = codeActive.CorpCode;
                            c.SubCorpCode = codeActive.SubCorpCode;
                            c.CorpName = codeActive.CorpName;
                            c.Code = row.GetCell(0).NumericCellValue.ToString();
                            //c.MaskCode = row.GetCell(1).NumericCellValue.ToString();
                            c.ProductCode = row.GetCell(8).StringCellValue;
                            c.ProductName = row.GetCell(7).StringCellValue;
                            c.Memo = row.GetCell(9).StringCellValue;
                            c.ActiveDescription = row.GetCell(10).StringCellValue;
                            c.ActiveStartDate = row.GetCell(11).DateCellValue;
                            c.ActiveEndDate = row.GetCell(12).DateCellValue;
                            c.CustomerOpenID = row.GetCell(13).StringCellValue;
                            c.CustomerTime = row.GetCell(14).DateCellValue;
                            c.CustomerLocatio = row.GetCell(15).StringCellValue;
                            c.WhatActive = row.GetCell(16).StringCellValue;
                            alist.Add(c);
                        }
                        string res = ReportFactory.Instance.SplitMongoInster<CustomerCode>(alist);
                        if (res == "数据插入MongoDB失败")
                        {
                            Response.status = 0;
                            this.Response.message = res + "，检查MongoDB是否正常";
                            return Response;
                        }
                    }
                    if (activeName == "资产")
                    {
                        List<PropertyCode> alist = new List<PropertyCode>();
                        //第4行开始获取数据
                        for (int index = 3; index <= lastRowIndex; index++)
                        {
                            //获取当前行
                            row = sheet.GetRow(index);
                            //生成Mongo对象存码信息

                            PropertyCode c = new PropertyCode();
                            c.ActiveName = activeName;
                            c.CreateDate = row.GetCell(2).DateCellValue;
                            c.ApplyId = codeActive.ApplyId;
                            c.CodeActivityId = activeID;
                            c.CorpCode = codeActive.CorpCode;
                            c.SubCorpCode = codeActive.SubCorpCode;
                            c.CorpName = codeActive.CorpName;
                            try
                            {
                                c.Code = row.GetCell(0).StringCellValue;
                            }
                            catch
                            {
                                c.Code = row.GetCell(0).NumericCellValue.ToString();
                            }
                            //c.MaskCode = row.GetCell(1).NumericCellValue.ToString();
                            c.ProductCode = row.GetCell(8).StringCellValue;
                            c.ProductName = row.GetCell(7).StringCellValue;
                            c.Memo = row.GetCell(9).StringCellValue;
                            c.ProductProvider = row.GetCell(17).StringCellValue;
                            c.ProductPurcheseDate = row.GetCell(18).DateCellValue;
                            alist.Add(c);
                        }
                        string res = ReportFactory.Instance.SplitMongoInster<PropertyCode>(alist);
                        if (res == "数据插入MongoDB失败")
                        {
                            Response.status = 0;
                            this.Response.message = res + "，检查MongoDB是否正常";
                            return Response;
                        }
                    }
                    if (activeName == "资产领用")
                    {
                        List<PropertyUseCode> alist = new List<PropertyUseCode>();
                        //第4行开始获取数据
                        for (int index = 3; index <= lastRowIndex; index++)
                        {
                            //获取当前行
                            row = sheet.GetRow(index);
                            //生成Mongo对象存码信息

                            PropertyUseCode c = new PropertyUseCode();
                            c.ActiveName = activeName;
                            c.CreateDate = row.GetCell(2).DateCellValue;
                            c.ApplyId = codeActive.ApplyId;
                            c.CodeActivityId = activeID;
                            c.CorpCode = codeActive.CorpCode;
                            c.SubCorpCode = codeActive.SubCorpCode;
                            c.CorpName = codeActive.CorpName;
                            try
                            {
                                c.Code = row.GetCell(0).StringCellValue;
                            }
                            catch
                            {
                                c.Code = row.GetCell(0).NumericCellValue.ToString();
                            }
                            //c.MaskCode = row.GetCell(1).NumericCellValue.ToString();
                            c.ProductCode = row.GetCell(8).StringCellValue;
                            c.ProductName = row.GetCell(7).StringCellValue;
                            c.Memo = row.GetCell(9).StringCellValue;
                            c.ClaimDepartment = row.GetCell(19).StringCellValue;
                            c.ClaimPerson = row.GetCell(20).StringCellValue;
                            c.ClaimDate = row.GetCell(21).DateCellValue;
                            alist.Add(c);
                        }
                        string res = ReportFactory.Instance.SplitMongoInster<PropertyUseCode>(alist);
                        if (res == "数据插入MongoDB失败")
                        {
                            Response.status = 0;
                            this.Response.message = res + "，检查MongoDB是否正常";
                            return Response;
                        }
                    }
                }
                
                //删除文件
                System.IO.File.Delete(fullPath);

                Response.status = 1;
                Response.message = "上传成功";
                
            }
            catch (Exception e)
            {
                Response.status = 0;
                Response.message = e.Message;
            }

            return Response;
        }
    }
}