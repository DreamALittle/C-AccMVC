using Acctrue.CMC.Model.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Acctrue.CMC.CodeBuild
{
    /// <summary>
    /// 码段基础接口
    /// </summary>
    public interface ICodeSeg
    {
        /// <summary>
        /// 长度。
        /// </summary>
        int Length { get;}

        /// <summary>
        /// 初始化。
        /// </summary>
        /// <param name="paras">参数信息</param>
        void Initialize(List<ParameterInfo> paras);

        /// <summary>
        /// 码段描述信息。
        /// </summary>
        string Description { get; }

        /// <summary>
        /// 生成码。
        /// </summary>
        string Generate();

        /// <summary>
        /// 初始化参数只读。
        /// </summary>
        bool ArgsReadonly { get; }

        /// <summary>
        /// 参数标准化格式信息。
        /// </summary>
        List<ParameterInfo> GetArgsFormat();

        /// <summary>
        /// 验证参数有效性。
        /// </summary>
        /// <param name="inputParameters">参数信息</param>
        /// /// <param name="errorMess">无效时错误提示</param>
        /// <returns>验证有效</returns>
        bool ValidateArgsFormat(List<ParameterInfo> inputParameters, out string errorMess);

        /// <summary>
        /// 固定长度不可调整。
        /// </summary>
        bool FixLength { get; }

        /// <summary>
        /// 码段位置限制。
        /// </summary>
        PositionType PositionStandard { get; }


    }
}
