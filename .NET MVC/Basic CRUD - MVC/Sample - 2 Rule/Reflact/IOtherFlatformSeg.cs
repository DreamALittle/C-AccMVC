using Acctrue.CMC.Model.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acctrue.CMC.CodeBuild
{
    /// <summary>
    /// 第三方平台码段接口
    /// </summary>
    public interface IOtherFlatformSeg : ICodeSeg
    {
        /// <summary>
        /// 码申请。
        /// </summary>
        /// <param name="amount">申请数量</param>
        /// <param name="applyKey">申请成功凭证</param>
        /// <param name="messages">申请结果提示信息</param>
        /// <returns>申请是否成功</returns>
        bool EncodeApply(int amount, out string applyKey, out string messages);

        /// <summary>
        /// 是否可下载。
        /// </summary>
        /// <param name="applyKey">申请码返回的凭证</param>
        bool ReadyDownload(string applyKey);

        /// <summary>
        /// 下载码。
        /// </summary>
        /// <param name="applyKey">申请码返回的凭证</param>
        List<string> EncodeDownload(string applyKey, out byte[] originalCodeData);

        /// <summary>
        /// 码激活。
        /// </summary>
        /// <param name="ecodes">码集合</param>
        /// <param name="messages">提示信息</param>
        bool EcodeActivate(List<string> ecodes, CodeActive codeActive, out string messages);
    }
}
