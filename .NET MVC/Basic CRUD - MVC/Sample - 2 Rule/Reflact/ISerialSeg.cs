using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acctrue.CMC.CodeBuild
{
    /// <summary>
    /// 流水号码段接口
    /// </summary>
    public interface ISerialSeg : ICodeSeg
    {
        /// <summary>
        /// 流水号是否越界。
        /// </summary>
        bool Overflow
        {
            get;
        }

        /// <summary>
        /// 当前序列号
        /// </summary>
        string CurrentSerialNoString
        {
            get;
        }

        /// <summary>
        /// 获取指定申请数量的结束序列号。
        /// </summary>
        /// <param name="amount">申请数量。</param>
        /// <returns></returns>
        string GetCurrentEndSerialNoCode(int amount);

        /// <summary>
        /// 设置当前序列位置 。
        /// </summary>
        /// <param name="value">序列位置参数</param>
        void SetLastSegValue(object value);

        /// <summary>
        /// 是否是有效申请数量。
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        bool IsValidApplyAmount(int amount);
    }
}
