using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acctrue.CMC.Model.Code
{
    /// <summary>
    /// 参数描述
    /// </summary>
    public class ParameterInfo
    {
        /// <summary>
        /// 参数名
        /// </summary>
        public string ParamenterKey { get; set; }

        /// <summary>
        /// 参数值
        /// </summary>
        public string ParamenterValues { get; set; }

        /// <summary>
        /// 显示名称
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 描述信息
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 格式检验公式（正则）
        /// </summary>
        public string CheckFormat { get; set; }
    }
}
