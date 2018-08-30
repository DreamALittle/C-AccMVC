using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Acctrue.CMC.Model.Code
{
    /// <summary>
    /// 码规则类别。
    /// </summary>
    public enum CodeRuleType
    {
        /// <summary>
        /// 内部码规则。
        /// </summary>
        [Description("{\"cn\":\"内部码规则\",\"en\":\"Internal Rule\"}")]
        Internal = 1,

        /// <summary>
        /// 外部平台。
        /// </summary>
        [Description("{\"cn\":\"外部平台\",\"en\":\"Other Platform\"}")]
        OtherPlatform = 2
    }
}
