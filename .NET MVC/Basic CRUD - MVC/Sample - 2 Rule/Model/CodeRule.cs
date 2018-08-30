using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Acctrue.Library.Data.Definition;
using System.Runtime.Serialization;

namespace Acctrue.CMC.Model.Code
{
    [DbTable("CodeRules")]
    [DataContract()]
    [Serializable()]
    public partial class CodeRule
    {   /// <summary>
        /// 规则ID
        /// </summary>
        [DataMember()]
        [DbKey]
        public int CodeRuleId
        {
            get;
            set;
        }

        /// <summary>
        /// 名称
        /// </summary>
        [DataMember()]
        public string CodeRuleName
        {
            get;
            set;
        }

        /// <summary>
        /// 状态
        /// </summary>
        [DataMember()]

        public RuleStatus RuleStatus
        {
            get;
            set;
        }

        /// <summary>
        /// 长度 
        /// </summary>
        [DataMember()]
        public int CodeLength
        {
            get;
            set;
        }
        /// <summary>
        /// 版本
        /// </summary>
        [DataMember()]
        [AllowNull]
        public string CodeVersions { get; set; }
        /// <summary>
        /// 依赖参数
        /// </summary>
        [DataMember()]
        [AllowNull]
        public string CodeArguments
        {
            get;
            set;
        }
        /// <summary>
        /// 扩展
        /// </summary>
        [DataMember()]
        [AllowNull]
        public string CodeExtensions
        {
            get;
            set;
        }
        /// <summary>
        /// 备注
        /// </summary>
        [DataMember()]
        [AllowNull]
        public string Description
        {
            get;
            set;
        }

        [DataMember()]
        public CodeRuleType CodeRuleType { get; set; }
        /// <summary>
        /// 防伪码
        /// </summary>
        [DataMember()]
        public bool IsMaskCode
        { get; set; }
        /// <summary>
        /// 码文件
        /// </summary>
        [DataMember()]
        public bool IsParseCode
        { get; set; }    
        /// <summary>
        /// 创建时间
        /// </summary>
        [DataMember()]
        public DateTime CreatedTime { get; set; }
        /// <summary>
        /// 创建者
        /// </summary>
        [DataMember()]
        [AllowNull]
        public string Creator { get; set; }
        /// <summary>
        /// ModifiedTime
        /// </summary>
        [DataMember()]
        public DateTime ModifiedTime { get; set; }
        /// <summary>
        /// Modifier
        /// </summary>
        [DataMember()]
        [AllowNull]
        public string Modifier { get; set; }
    }
}
