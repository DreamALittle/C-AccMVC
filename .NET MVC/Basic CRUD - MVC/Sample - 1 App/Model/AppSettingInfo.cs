using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Acctrue.Library.Data.Definition;
using System.Runtime.Serialization;

namespace Acctrue.CMC.Model.Systems
{
    [DbTable("AppSettings")]
    [DataContract()]
    [Serializable()]
    public partial class AppSettingInfo
    {
        /// <summary>
        /// 应用Id
        /// </summary>
        [DataMember()]
        [DbKey]
        public int AppSettingID
        {
            get;
            set;
        }
        /// <summary>
        /// Id
        /// </summary>
        [DataMember()]
        public string AppID
        {
            get;
            set;
        }
        /// <summary>
        /// Secret
        /// </summary>
        [DataMember()]
        public string Secret
        {
            get;
            set;
        }
        /// <summary>
        /// 加密种子
        /// </summary>
        [DataMember()]
        public string Seed
        {
            get;
            set;
        }
        /// <summary>
        /// 企业编码
        /// </summary>
        [DataMember()]
        public string CorpCode
        {
            get;
            set;
        }
        /// <summary>
        /// 企业名称
        /// </summary>
        [DataMember()]
        public string CorpName
        {
            get;
            set;
        }
        /// <summary>
        ///  工厂名称 
        /// </summary>
        [DataMember()]
        public string SubCorpCode
        {
            get;
            set;
        }

        /// <summary>
        /// 应用唯一标识
        /// </summary>
        [DataMember()]
        [AllowNull]
        public string Token { get; set; }

        /// <summary>
        /// 当前访问地址
        /// </summary>
        [AllowNull]
        [DataMember()]
        public string IpAddress
        {
            get;
            set;
        }
        
        /// <summary>
        /// 用户状态
        /// </summary>
        [DataMember()]
        public AppStatus AppStatus
        {
            get;
            set;
        }

        /// <summary>
        /// 创建时间
        /// </summary>
        [DataMember()]
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// ModifiedDate
        /// </summary>
        [DataMember()]
        public DateTime ModifiedDate { get; set; }

        /// <summary>
        /// ModifiedDate
        /// </summary>
        [DataMember()]
        public DateTime TokenExpireDate { get; set; }

        /// <summary>
        /// 自动审核
        /// </summary>
        [DataMember()]
        public bool IsAutoAudit { get; set; }

    }
}
