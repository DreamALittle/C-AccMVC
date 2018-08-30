using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Acctrue.Library.Data;
using Acctrue.CMC.Interface.Systems;
using Acctrue.CMC.Model.Systems;
using Newtonsoft.Json;

namespace Acctrue.CMC.Service.Systems
{
    public partial class SystemService : BizBase, ISystemService
    {
        public void Add(string data,bool isAdmin)
        {
            AppSettingInfo config = JsonConvert.DeserializeObject<AppSettingInfo>(data);
            if(isAdmin)
                config.AppStatus = AppStatus.Reviewed;
            config.CreatedDate = config.ModifiedDate = DateTime.Now;
            config.TokenExpireDate = DateTime.Now.AddHours(2);
            //时间戳
            TimeSpan ts = DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            string timeSign = Convert.ToInt64(ts.TotalSeconds).ToString();
            byte[] bytes = Encoding.Default.GetBytes(timeSign);
            //Token
            config.Token = GetMd5Hash(config.Secret + config.Seed) + Convert.ToBase64String(bytes);
            dbContext.Save(config);  
        }
        public void Update(AppSettingInfo info)
        {
            Library.Data.SqlEntry.KeyValueCollection keys = new Library.Data.SqlEntry.KeyValueCollection();
            keys.Add(new Library.Data.SqlEntry.KeyValue("CorpCode", info.CorpCode));
            keys.Add(new Library.Data.SqlEntry.KeyValue("CorpName", info.CorpName));
            keys.Add(new Library.Data.SqlEntry.KeyValue("SubCorpCode", info.SubCorpCode));
            keys.Add(new Library.Data.SqlEntry.KeyValue("Secret", info.Secret));
            keys.Add(new Library.Data.SqlEntry.KeyValue("AppStatus", info.AppStatus));
            keys.Add(new Library.Data.SqlEntry.KeyValue("AppID", info.AppID));
            keys.Add(new Library.Data.SqlEntry.KeyValue("IsAutoAudit", info.IsAutoAudit));
            keys.Add(new Library.Data.SqlEntry.KeyValue("Seed",info.Seed));
            dbContext.Update<AppSettingInfo>(keys, CK.K["AppSettingID"].Eq(info.AppSettingID));
        }

        public AppSettingInfo[] SearchSystemAppSettings(dynamic obj, out int count)
        {
            Condition where = Condition.Empty;
            string appID = "";
            string appName = "";
            bool autoAudit = true;
            Dictionary<string, object> dic = JsonConvert.DeserializeObject<Dictionary<string, object>>((string)obj["paramObj"]);
            if (dic.Keys.Contains("AppID"))
            {
                if (dic["AppID"] != null && (string)dic["AppID"] != "")
                {
                    appID = (string)dic["AppID"];
                    where = where & CK.K["AppID"].Eq(appID);
                }
                
            }
            if (dic.Keys.Contains("AppName"))
            {
                if (dic["AppName"] != null && (string)dic["AppName"] != "")
                {
                    appName = (string)dic["AppName"];
                    where = where & CK.K["CorpName"].LeftLike(appName).Or(CK.K["SubCorpCode"].LeftLike(appName)).Or(CK.K["CorpCode"].LeftLike(appName));
                }
            }
            if (dic.Keys.Contains("AppAudit"))
            {
                if (dic["AppAudit"] != null && (string)dic["AppAudit"] != "")
                {
                    appName = (string)dic["AppAudit"];
                    if (appName == "手动")
                    {
                        autoAudit = false;
                    }
                    where = where & CK.K["IsAutoAudit"].Eq(autoAudit);
                   
                }
            }
            int page = obj.page;
            int limit = obj.limit;
            var recordCount = dbContext.From<AppSettingInfo>().Where(where).GetCount();
            count = (int)recordCount;
            if (page > 0)
            {
                return dbContext.From<AppSettingInfo>().Where(where).Select().OrderByDescending(c=>c.CreatedDate).Skip((page - 1) * limit).Take(limit).ToArray();
            }
            else
            {
                return dbContext.From<AppSettingInfo>().Where(where).Select().OrderByDescending(c => c.CreatedDate).ToArray();
            }
        }
        public AppInterFaceInfo[] SearchAppInterFaceInfos(Dictionary<string, object> dic)
        {
            Condition where = Condition.Empty;
            if (dic.ContainsKey("AppSettingID"))
            {
                where = CK.K["AppSettingID"].Eq(dic["AppSettingID"]);
            }
            return dbContext.From<AppInterFaceInfo>().Where(where).Select().ToArray();
        }

        private string GetMd5Hash(String input)
        {
            if (input == null)
            {
                return null;
            }

            MD5 md5Hash = MD5.Create();

            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            StringBuilder sBuilder = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }


        public AppSettingInfo GetSystemAppConfig(string appid, string secret)
        {
            AppSettingInfo[] apps = dbContext.From<AppSettingInfo>().Where(CK.K["AppID"].Eq(appid) & CK.K["Secret"].Eq(secret)).Select().ToArray();
            if (apps.Length > 0)
                return apps[0];
            return null;
        }

        public AppSettingInfo GetToken(string token)
        {
            AppSettingInfo[] apps = dbContext.From<AppSettingInfo>().Where(CK.K["Token"].Eq(token)).Select().ToArray();
            if (apps.Length > 0)
                return apps[0];
            return null;
        }
        public void AddAppInterFaceInfos(List<AppInterFaceInfo> obj)
        {
            int appSettingID = obj[0].AppSettingID;
            //清空旧数据
            Condition where = Condition.Empty;
            where &= CK.K["AppSettingID"].Eq(appSettingID);
            dbContext.Delete<AppInterFaceInfo>(where);
            //添加新数据
            if (obj[0].Namespace != "Fake")
            {
                foreach (AppInterFaceInfo item in obj)
                {
                    dbContext.Insert(item);
                }
            }
        }

        public void RemoveAppInterface(Dictionary<string, object> dic)
        {
            Condition where = Condition.Empty;
            if (dic.ContainsKey("InterfaceFunctionName"))
            {
                where = CK.K["InterfaceFunctionName"].Eq(dic["InterfaceFunctionName"]);
            }
            where &= CK.K["AppSettingID"].Eq(dic["AppSettingID"]);
            dbContext.Delete<AppInterFaceInfo>(where);
        }

        public bool GetAppInterface(int appSettingId,string functionName)
        {
            Condition where = Condition.Empty;
            where = CK.K["AppSettingID"].Eq(appSettingId);
            where = where & CK.K["FunctionName"].Eq(functionName);
            AppInterFaceInfo[] res = dbContext.From<AppInterFaceInfo>().Where(where).Select().ToArray();
            if (res.Length > 0)
                return true;
            else
                return false;
        }

        public void RemoveAppSetting(Dictionary<string, object> dic)
        {
            Condition where = Condition.Empty;
            if (dic.ContainsKey("AppSettingID"))
            {
                where = CK.K["AppSettingID"].Eq(dic["AppSettingID"]);
            }
            dbContext.Delete<AppSettingInfo>(where);
        }
        public string UpdateTokenInfo(AppSettingInfo info)
        {
            TimeSpan ts = DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            string timeSign = Convert.ToInt64(ts.TotalSeconds).ToString();
            byte[] bytes = Encoding.Default.GetBytes(timeSign);
            //Token
            info.Token = GetMd5Hash(info.Secret + "CMC") + Convert.ToBase64String(bytes);
            //新过期时间
            info.TokenExpireDate = DateTime.Now.AddHours(2);
            dbContext.Save(info);
            return info.Token;
        }

        public AppSettingInfo GetUseAppConfig(int appSettingID)
        {
            AppSettingInfo[] apps = dbContext.From<AppSettingInfo>().Where(CK.K["AppSettingID"].Eq(appSettingID)).Select().ToArray();
            if (apps.Length > 0)
                return apps[0];
            return null;
        }

        public List<AppSettingInfoSimple> GetAllAppConfig()
        {
            List<AppSettingInfo> appList = dbContext.From<AppSettingInfo>().Where(CK.K["AppStatus"].Eq(2)).Select().ToList();
            List<AppSettingInfoSimple> appReturnList = new List<AppSettingInfoSimple>();
            foreach (AppSettingInfo app in appList)
            {
                AppSettingInfoSimple simpleApp = new AppSettingInfoSimple();
                simpleApp.AppSettingID = app.AppSettingID;
                simpleApp.CorpName = app.CorpName;
                simpleApp.SubCorpCode = app.SubCorpCode;
                simpleApp.CorpCode = app.CorpCode;
                appReturnList.Add(simpleApp);
            }
            appReturnList.Sort((x, y) => { return x.AppSettingID.CompareTo(y.AppSettingID); });
            return appReturnList;

        }
    }

}
