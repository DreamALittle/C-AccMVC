using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acctrue.CMC.Model.Menu
{
    public enum MenuStatus
    {
        /// <summary>
        /// 禁用。
        /// </summary>
        [Description("禁用")]
        Disabled,

        /// <summary>
        /// 启用。
        /// </summary>
        [Description("启用")]
        Enabled
    }
}
