using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Acctrue.Library.Data.Definition;
using System.Runtime.Serialization;

namespace Acctrue.CMC.Model.Code
{
    [DbTable("CodeRuleSeg")]
    [DataContract()]
    [Serializable()]
    public partial class CodeRuleSeg
    {
        /// <summary>
        /// 规则码段Id
        /// </summary>
        [DataMember()]
        [DbKey]
        public int SegId
        {
            get;
            set;
        }
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
        /// 排序标志
        /// </summary>
        [DataMember()]
        public int SegIndex
        {
            get;
            set;
        }
        /// <summary>
        /// 码段长度
        /// </summary>
        [DataMember()]
        public int SegLength
        {
            get;
            set;
        }
        /// <summary>
        /// 码段类名
        /// </summary>
        [DataMember()]
        public string ClassName
        {
            get;
            set;
        }
        ///// <summary>
        ///// 码段名称
        ///// </summary>
        //public string RuleSegName
        //{
        //    get;
        //    set;
        //}

        /// <summary>
        ///  类参数 
        /// </summary>
        [DataMember()]
        [AllowNull]
        public string ClassArgs
        {
            get;
            set;
        }

        /// <summary>
        /// 输出参数
        /// </summary>
        [DataMember()]
        [AllowNull]
        public string OutArgs { get; set; }

        ///// <summary>
        ///// 备注
        ///// </summary>
        //[AllowNull]
        //[DataMember()]
        //public string Description
        //{
        //    get;
        //    set;
        //}
    }
}
