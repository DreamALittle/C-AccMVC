using Acctrue.CMC.Model.Code;
using Acctrue.CMC.Model.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acctrue.CMC.CodeService
{
    /// <summary>
    /// 活动处理接口
    /// </summary>
    public interface IActivityAction
    {
        /// <summary>
        /// 接口对应活动名称
        /// </summary>
        string ActivityName { get; }
        /// <summary>
        /// 活动处理方法
        /// </summary>
        /// <param name="codeActive">码活动信息对象</param>
        /// <param name="activityCodes">码活动信息集合</param>
        void ActionProcess<T>(CodeActive codeActive, List<T> activityCodes) where T : ActivityCodes;
    }
}
