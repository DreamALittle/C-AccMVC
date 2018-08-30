using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acctrue.CMC.Model.Code
{
    public class SegClassInfo
    {
        /// <summary>
        /// 码段类名
        /// </summary>
        public string ClassName
        {
            get;
            set;
        }

        /// <summary>
        /// 码段描述
        /// </summary>
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// 码规则类型
        /// </summary>
        public CodeRuleType CodeRuleType
        {
            get;
            set;
        }

        /// <summary>
        /// 码段类参数
        /// </summary>
        public List<ParameterInfo> SegParameters
        {
            get;
            set;
        }

    }
}
