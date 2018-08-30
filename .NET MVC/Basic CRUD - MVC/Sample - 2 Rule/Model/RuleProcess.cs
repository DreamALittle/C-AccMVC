using Acctrue.Library.Data.Definition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Acctrue.CMC.Model.Code
{
    /// <summary>
    /// 码规则处理状态（码规则序列信息）
    /// </summary>
    [DbTable("RuleProcess")]
    [DataContract()]
    [Serializable()]
    public class RuleProcess
    {
        /// <summary>
        /// 码规则Id
        /// </summary>
        [DataMember()]
        public int CodeRuleId
        {
            get;
            set;
        }

        /// <summary>
        /// 特征标签（使用码规则时的用户输入参数组合，不同特征独立维护序列）
        /// </summary>
        [DataMember()]
        [AllowNull]
        public string FeatureTag
        {
            get;
            set;
        }

        /// <summary>
        /// 最后码值
        /// </summary>
        [DataMember()]
        [AllowNull]
        public string LastCode
        {
            get;
            set;
        }
    }
}
