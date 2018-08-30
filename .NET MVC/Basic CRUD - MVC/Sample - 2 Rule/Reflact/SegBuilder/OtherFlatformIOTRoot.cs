using Acctrue.CMC.Util;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Acctrue.CMC.Model.Code;
using System.Security.Cryptography;
using System.IO;
using System.IO.Compression;
using ICSharpCode.SharpZipLib.Zip;

namespace Acctrue.CMC.CodeBuild.SegBuilder
{
    public class OtherFlatformIOTRoot : SegBase, IOtherFlatformSeg
    {
        /// <summary>
        /// 参数信息集合
        /// </summary>
        protected static List<ParameterInfo> parameters = new List<ParameterInfo>();
        /// <summary>
        /// 参数锁对象
        /// </summary>
        private static object parameters_lock = new object();
        /// <summary>
        /// 参数信息格式初始化状态
        /// </summary>
        private static bool parametersFormat_init = false;
        //private bool initialized = false;
        /// <summary>
        /// 外部平台码申请地址
        /// </summary>
        private static readonly string ApplySubAddress = "/interface/company/ecodeApply";
        /// <summary>
        /// 外部平台码下载地址
        /// </summary>
        private static readonly string DownloadCodeSubAddress = "/interface/company/download";
        /// <summary>
        /// 外部平台码信息回传地址
        /// </summary>
        private static readonly string ActivateSubAddress = "/interface/company/ecodeReturn";

        #region 实现抽象类SegBase
        public override List<ParameterInfo> Parameters { get { return parameters; } }

        protected override void InitParaSettings()
        {
            if (!parametersFormat_init)
            {
                lock (parameters_lock)
                {
                    if (!parametersFormat_init)
                    {
                        parameters.Add(new ParameterInfo { ParamenterKey = "InterfaceAddress", ParamenterValues = "", DisplayName="接口地址", Description = "其它平台接口地址根地址", CheckFormat = @"(http|https):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?" });
                        parameters.Add(new ParameterInfo { ParamenterKey = "ClientId", ParamenterValues = "", DisplayName="用户Id", Description = "用户Id", CheckFormat = "^[a-zA-Z0-9]+$" });
                        parameters.Add(new ParameterInfo { ParamenterKey = "AESKey", ParamenterValues = "", DisplayName = "密钥", Description = "密钥", CheckFormat = "^.+$" });
                        parametersFormat_init = true;
                    }
                }
            }
        }

        public override bool ArgsReadonly
        {
            get { return false; }
        }

        public override string Description
        {
            get
            {
                return "外部平台(国家物联网标识管理中心)";
            }
        }

        public override bool FixLength
        {
            get { return true; }
        }

        public override PositionType PositionStandard
        {
            get { return PositionType.AtBegin | PositionType.AtEnd; }
        }
        #endregion

        #region 实现IOtherFlatformSeg接口
        public bool EncodeApply(int amount, out string applyKey, out string messages)
        {
            bool succeed = false;
            applyKey = string.Empty;
            messages = string.Empty;
            string baseUrl = inputParameters["InterfaceAddress"];
            string clientId = inputParameters["ClientId"];
            long timeStamp = (long)(DateTime.Now - TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1))).TotalMilliseconds;
            string sign = Tools.AESEncode(clientId + timeStamp, inputParameters["AESKey"]);
            Gather gather = new Gather();
            gather.Url = $@"{baseUrl}{ApplySubAddress}/clientId={clientId}/num={amount}/timeStamp={timeStamp}/sign={sign}";
            string resultHtml = gather.GetHtml();
            if (resultHtml == string.Empty)
            {
                messages = gather.TraceInfo;
            }
            else
            {
                ResultObject jsonResult = JsonConvert.DeserializeObject<ResultObject>(resultHtml);
                if (jsonResult.status == 200)
                {
                    applyKey = jsonResult.fileid;
                    succeed = true;
                }
                else
                {
                    messages = jsonResult.msg;
                }
            }
            return succeed;
        }

        public List<string> EncodeDownload(string applyKey, out byte[] originalCodeData)
        {
            originalCodeData = null;
            List<string> keyList = new List<string>();
            //bool succeed = false;
            string messages = string.Empty;
            string baseUrl = inputParameters["InterfaceAddress"];
            string clientId = inputParameters["ClientId"];
            long timeStamp = (long)(DateTime.Now - TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1))).TotalMilliseconds;
            string sign = Tools.AESEncode(clientId + timeStamp, inputParameters["AESKey"]);
            Gather gather = new Gather();
            gather.Url = $@"{baseUrl}{DownloadCodeSubAddress}/clientId={clientId}/fileId={applyKey}/timeStamp={timeStamp}/sign={sign}";
            
            using (System.IO.Stream resultStream = gather.GetStream())
            {
                if (resultStream == null)
                {
                    throw new Exception($"码下载失败！\n对方平台提示信息：{gather.TraceInfo}");
                }
                else
                {
                    if (resultStream.Length < 500)
                    {
                        using (System.IO.StreamReader streamReader = new System.IO.StreamReader(resultStream, System.Text.Encoding.UTF8, true, 1000, true))
                        {
                            string codesStr = streamReader.ReadToEnd();
                            bool errorState = false;
                            try
                            {
                                ResultObject jsonResult = JsonConvert.DeserializeObject<ResultObject>(codesStr);
                                if (jsonResult.status != 200)
                                {
                                    errorState = true;
                                    messages = jsonResult.msg;
                                }
                            }
                            catch 
                            {

                            }
                            if (errorState)
                            {
                                throw new Exception($"码下载失败！\n对方平台提示信息：{messages}");
                            }

                        }
                    }
                    string content = string.Empty;
                    originalCodeData = new byte[resultStream.Length];
                    resultStream.Read(originalCodeData, 0, (int)resultStream.Length);
                    if (Tools.UnZipGetFirstText(resultStream, out content))
                    {
                        keyList = ConvertToCodeList(content);
                    }
                    else
                    {
                        throw new Exception($"码下载失败！\n码文件解压失败!");
                    }
                }
                
            }
            return keyList;
        }

        /// <summary>
        /// 码激活。
        /// </summary>
        /// <param name="ecodes">码集合</param>
        /// <param name="messages">提示信息</param>
        public bool EcodeActivate(List<string> ecodes, CodeActive codeActive, out string messages)
        {
            bool succeed = false;
            messages = string.Empty;
            string baseUrl = inputParameters["InterfaceAddress"];
            string clientId = inputParameters["ClientId"];
            long timeStamp = (long)(DateTime.Now - TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1))).TotalMilliseconds;
            string sign = Tools.AESEncode(clientId + timeStamp, inputParameters["AESKey"]);
            Gather gather = new Gather();
            gather.Url = $@"{baseUrl}{ActivateSubAddress}/clientId={clientId}/timeStamp={timeStamp}/sign={sign}";
            gather.Method = "POST";

            gather.PostData = "[{\"ecode\":\"" + string.Join(",",ecodes.ToArray()) + "\",\"photo\":\"\",\"datas\":[{\"key\":\"ProductName\",\"value\":\"" + codeActive.ProductName + "\"},{\"key\":\"ProductCode\",\"value\":\"" + codeActive.ProductCode + "\"},{\"key\":\"CorpName\",\"value\":\"" + codeActive.CorpName + "\"},{\"key\":\"ProductionDate \",\"value\":\""+codeActive.UploadDate+ "\"},{\"key\":\"ProduceWorkline\",\"value\":\""+codeActive.ProduceWorkline+"\"}]}]";
            //gather.PostData = "[{\"ecode\":\"123,123\",\"photo\":\"\",\"datas\":[{\"key\":\"产品名称\",\"value\":\"康师傅矿泉水\"},{\"key\":\"生产厂家\",\"value\":\"1\"},{\"key\":\"生产地址\",\"value\":\"1\"},{\"key\":\"生产日期\",\"value\":\"1525737600000\"},{\"key\":\"产品类型\",\"value\":\"\"},{\"key\":\"产品批次\",\"value\":\"0002\"},{\"key\":\"生产数量\",\"value\":\"1\"}]}]";
            gather.ContentType = "application/json";
            string resultHtml = gather.GetHtml();
            ResultObject jsonResult = JsonConvert.DeserializeObject<ResultObject>(resultHtml);
            if (jsonResult != null && jsonResult.status == 200)
            {
                succeed = true;
            }
            else
            {
                if (jsonResult == null)
                {
                    messages = "码激活操作对方接口平台无结果反馈";
                }
                else
                {
                    messages = jsonResult.msg;
                }
            }

            return succeed;
        }

        

        public override string Generate()
        {
            throw new NotImplementedException();
        }

        public bool ReadyDownload(string applyKey)
        {
            return true;
        }

        #endregion

        /// <summary>
        /// 码信息串转换为码集合
        /// </summary>
        /// <param name="bigCodeStr"></param>
        /// <returns></returns>
        private List<string> ConvertToCodeList(string bigCodeStr)
        {
            List<string> codeList = new List<string>();
            codeList = bigCodeStr.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            return codeList;
        }
        /// <summary>
        /// 外部平台接口返回信息结构对象
        /// </summary>
        private class ResultObject
        {
            public int status { get; set; }
            public string msg { get; set; }
            public string fileid { get; set; }
        }
        
    }
}
