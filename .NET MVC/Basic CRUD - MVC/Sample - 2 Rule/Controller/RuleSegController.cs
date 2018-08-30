using Acctrue.CMC.Factory.Code;
using Acctrue.CMC.Interface.Common;
using Acctrue.CMC.Model.Code;
using Acctrue.CMC.CodeBuild;
using Acctrue.CMC.Util;
using Acctrue.Library.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Web.Http;
using MyResponse = Acctrue.CMC.Web.Controllers.Response;
using System.Web;
using System.ComponentModel;

namespace Acctrue.CMC.Web.Controllers
{
    [CMCAttribute(InterfaceName="应用程序接口", Open = true)]
    public class CodeRuleController : ValuesController
    {
        // POST api/values
        [HttpPost]
        public string AddCodeRule(dynamic obj)
        {
            MyResponse.Response res = new MyResponse.Response();
            string msg = "";
            string json = JsonConvert.SerializeObject(obj);
            CodeRule config = JsonConvert.DeserializeObject<CodeRule>(json);
            config.Creator = (string)HttpContext.Current.Session["UserName"];
            CodeRuleFactory.Instance.Add(config);
            this.Response.message = msg;
            Log("{\"LogMenu\":\"系统管理\",\"LogAction\":\"添加码规则\"}", this.UserName, this.IP);
            return JsonConvert.SerializeObject(this.Response).ToString();
        }
        /// <summary>
        /// 获取码规则列表
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage SearchCodeRuleConfig(dynamic obj)
        {
            int count = 0;
            List<CodeRule> rules = CodeRuleFactory.Instance.SearchCodeRuleConfig(obj, out count);

            string respon = "{ \"code\":0,\"msg\":1,\"count\":" + count + ",\"data\":" + JsonConvert.SerializeObject(rules).ToString() + "}";

            HttpResponseMessage result = new HttpResponseMessage { Content = new StringContent(respon, Encoding.GetEncoding("UTF-8"), "application/json") };
            return result;
        }
        /// <summary>
        /// 获取规则码段配置信息
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage GetCodeRuleSegConfig(dynamic obj)
        {
            int count = 0;
            List<CodeRuleSeg> appInfos = new List<CodeRuleSeg>();
            if (obj.CodeRuleId != null && obj.CodeRuleId != "")
            {
                appInfos = CodeRuleSegFactory.Instance.GetByCodeRuleId((int)obj.CodeRuleId);
            }
            

            string respon = "{ \"code\":0,\"msg\":1,\"count\":" + count + ",\"data\":" + JsonConvert.SerializeObject(appInfos).ToString() + "}";

            HttpResponseMessage result = new HttpResponseMessage { Content = new StringContent(respon, Encoding.GetEncoding("UTF-8"), "application/json") };
            return result;
        }

        [HttpPost]
        public HttpResponseMessage GetSegClassList(dynamic obj)
        {
            List<SegClassInfo> segClassInfos = new List<SegClassInfo>();
            try
            {
                var type = typeof(ICodeSeg);
                var otherPlatformType = typeof(IOtherFlatformSeg);
                var test = Assembly.GetExecutingAssembly().GetTypes().Where(item => item.GetInterfaces().Contains(typeof(ICodeSeg))).ToList();
                var types = Assembly.Load(new AssemblyName("Acctrue.CMC.CodeBuild")).GetTypes().ToArray();
                //var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes()).Where<Type>(p => type.IsAssignableFrom(p.GetGenericTypeDefinition()));
                //AppDomain.CurrentDomain.AssemblyLoad().SelectMany(s => s.GetTypes())
                //.Where(p => type.IsAssignableFrom(p.GetGenericTypeDefinition()));

                foreach (var v in types)
                {
                    if (v.IsClass && !v.IsAbstract && type.IsAssignableFrom(v))
                    {
                        if ((obj.codeRuleType == 2 && otherPlatformType.IsAssignableFrom(v))|| (obj.codeRuleType == 1 && !otherPlatformType.IsAssignableFrom(v)))
                        {
                            ICodeSeg seg = (Activator.CreateInstance(v) as ICodeSeg);
                            SegClassInfo segInfo = new SegClassInfo();
                            segInfo.ClassName = v.FullName;
                            segInfo.Description = seg.Description;
                            segInfo.CodeRuleType = (CodeRuleType)Convert.ToInt32(obj.codeRuleType);
                            segInfo.SegParameters = seg.GetArgsFormat();
                            segClassInfos.Add(segInfo);
                        }
                    }
                }
            }
            catch 
            {
                throw new Exception("反射码段信息失败，请重试或重启IIS服务器！");
            }
            string respon = "{ \"code\":0,\"msg\":1,\"count\":" + segClassInfos.Count + ",\"data\":" + JsonConvert.SerializeObject(segClassInfos).ToString() + "}";
            HttpResponseMessage result = new HttpResponseMessage { Content = new StringContent(respon, Encoding.GetEncoding("UTF-8"), "application/json") };
            return result;
        }

        public string ChangeState(Dictionary<string, int> dic)
        {
            CodeRule ruleInfo = CodeRuleFactory.Instance.Get(dic["CodeRuleId"]);
            ruleInfo.RuleStatus = (RuleStatus)dic["RuleStatus"];
            ruleInfo.Modifier = (string)HttpContext.Current.Session["UserName"];
            ruleInfo.ModifiedTime = DateTime.Now;
            CodeRuleFactory.Instance.Update(ruleInfo);
            return JsonConvert.SerializeObject(this.Response).ToString();
        }

        public string Update(Dictionary<string, string> dic)
        {
            CodeRule ruleInfo = DicToObject<CodeRule>(Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string,object>>(dic["CodeRule"]));
            ruleInfo.ModifiedTime = DateTime.Now;
            ruleInfo.Modifier = (string)HttpContext.Current.Session["UserName"];
            if (ruleInfo.CodeRuleId == 0)
            {
                ruleInfo.Creator = (string)HttpContext.Current.Session["UserName"];
                ruleInfo.CreatedTime = DateTime.Now;
                ruleInfo.CodeRuleId = CodeRuleFactory.Instance.Add(ruleInfo);
            }
            else
            {
                CodeRuleFactory.Instance.Update(ruleInfo);
            }
            List<Dictionary<string, object>> delSeginfos = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(dic["DelSegs"]);
            foreach (Dictionary<string, object> seg in delSeginfos)
            {
                if (seg != null)
                {
                    CodeRuleSeg segInfo = DicToObject<CodeRuleSeg>(seg);
                    CodeRuleSegFactory.Instance.Delete(segInfo);
                }
            }

            List<Dictionary<string, object>> seginfos = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(dic["RuleSegs"]);
            foreach (Dictionary<string, object> seg in seginfos)
            {
                if (seg != null)
                {
                    CodeRuleSeg segInfo = DicToObject<CodeRuleSeg>(seg);
                    segInfo.CodeRuleId = ruleInfo.CodeRuleId;
                    if (segInfo.SegId == 0)
                    {
                        CodeRuleSegFactory.Instance.Add(segInfo);
                    }
                    else
                    {
                        CodeRuleSegFactory.Instance.Update(segInfo);
                    }
                }
            }
            Log("{\"LogMenu\":\"系统管理\",\"LogAction\":\"修改码规则["+ ruleInfo.CodeRuleName + "]\"}", this.UserName, this.IP);
            return JsonConvert.SerializeObject(this.Response).ToString();
        }
        [HttpPost]
        public string GetSegClassConfig(dynamic obj)
        {
            int stateCode = 0;
            string msg = string.Empty;
            List<Model.Code.ParameterInfo> argsInfo = new List<Model.Code.ParameterInfo>();
            try
            {
                //(Activator.CreateInstance(v) as ICodeSeg);
                Type type = Assembly.Load(new AssemblyName("Acctrue.CMC.CodeBuild")).GetType((string)obj.ClassName);
                
                ICodeSeg seg = (Activator.CreateInstance(type) as ICodeSeg);
                argsInfo = seg.GetArgsFormat();
            }
            catch
            {

            }
            //argsInfo.Clear();
            //argsInfo.Add(new Model.CodeRules.ParameterInfo() { CheckFormat="aaaa", Description="aaa", ParamenterKey="aaa", ParamenterValues="aaa"});
            string respon = "{ \"code\":" + stateCode + ",\"msg\":\"" + msg + "\",\"data\":" + JsonConvert.SerializeObject(argsInfo).ToString() + "}";
            return respon;
        }
        /// <summary>
        /// 获取用户输入标签列表
        /// </summary>
        [HttpPost]
        public string GetUserInputFlags()
        {
            int stateCode = 0;
            string msg = string.Empty;
            Dictionary<string, string> inputFlags = new Dictionary<string, string>();
            foreach (UserInputFlags inputFlag in Enum.GetValues(typeof(UserInputFlags)))
            {
                inputFlags.Add("${"+Enum.GetName(typeof(UserInputFlags),inputFlag)+"}",inputFlag.GetEnumDes());
            }
            //inputFlags.Add("${CorpCode}", "企业编码");
            //inputFlags.Add("${ProductCode}", "产品编码");
            //inputFlags.Add("${WorklineCode}", "生产线编码");
            //inputFlags.Add("${OtherCode}", "其它编码");
            //argsInfo.Clear();
            //argsInfo.Add(new Model.CodeRules.ParameterInfo() { CheckFormat="aaaa", Description="aaa", ParamenterKey="aaa", ParamenterValues="aaa"});
            string respon = "{ \"code\":" + stateCode + ",\"msg\":\"" + msg + "\",\"data\":" + JsonConvert.SerializeObject(inputFlags).ToString() + "}";
            return respon;
        }
        
    }
}
