using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acctrue.CMC.Model.Code
{
    /// <summary>
    /// 用户输入项
    /// </summary>
    public enum UserInputFlags
    {

        /// <summary>
        /// 企业编码。
        /// </summary>
        [Description("企业编码")]
        CorpCode = 1,

        /// <summary>
        /// 产品编码。
        /// </summary>
        [Description("产品编码")]
        ProductCode = 2,

        /// <summary>
        /// 产线编码。
        /// </summary>
        [Description("产线编码")]
        WorklineCode = 3,

        /// <summary>
        /// 其它编码。
        /// </summary>
        [Description("其它编码")]
        OtherCode = 4
    }
}
