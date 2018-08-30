using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Acctrue.CMC.Model.Code
{
    /// <summary>
    /// 码规则状态。
    /// </summary>
    public enum RuleStatus
    {
        /// <summary>
        /// 禁用。
        /// </summary>
        [Description("{\"cn\":\"禁用\",\"en\":\"Disabled\"}")]
        Disabled,

        /// <summary>
        /// 启用。
        /// </summary>
        [Description("{\"cn\":\"启用\",\"en\":\"Enabled\"}")]
        Enabled
    }
}
