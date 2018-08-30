using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acctrue.CMC.Model.Code
{
    /// <summary>
    /// 处理进度
    /// </summary>
    public enum ProcessType
    {
        /// <summary>
        /// 未处理
        /// </summary>
        [Description("{\"cn\":\"未处理\",\"en\":\"None\"}")]
        None = 0,

        /// <summary>
        /// 已申请
        /// </summary>
        [Description("{\"cn\":\"已申请\",\"en\":\"Applied\"}")]
        Applied = 1,

        ///// <summary>
        ///// 异常
        ///// </summary>
        //[Description("{\"cn\":\"异常\",\"en\":\"Error\"}")]
        //Error = 2,

        /// <summary>
        /// 已生成
        /// </summary>
        [Description("{\"cn\":\"已生成\",\"en\":\"Generated\"}")]
        Generated = 3,


        /// <summary>
        /// 已存储
        /// </summary>
        [Description("{\"cn\":\"已存储\",\"en\":\"Stored\"}")]
        Stored = 4,

        /// <summary>
        /// 已完成
        /// </summary>
        [Description("{\"cn\":\"已完成\",\"en\":\"Completed\"}")]
        Completed = 5

    }
}
