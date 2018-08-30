using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using Acctrue.CMC.Model.Code;

namespace Acctrue.CMC.CodeBuild.SegBuilder
{
    /// <summary>
    /// 实现IcodeSeg的抽象基类。
    /// </summary>
    public abstract class SegBase : ICodeSeg
    {
        /// <summary>
        /// 码长度
        /// </summary>
        protected int length=0;
        /// <summary>
        /// Initialize初始化方法取得的参数信息key-value
        /// </summary>
        protected Dictionary<string, string> inputParameters = new Dictionary<string, string>();

        protected SegBase()
        {
            InitParaSettings();
        }

        /// <summary>
        /// 验证单个参数有效性。
        /// </summary>
        /// <param name="para">待检测的参数信息</param>
        /// <returns>验证有效</returns>
        protected bool ValidateArgs(ParameterInfo para)
        {
            bool isValidated = true;
            return isValidated;
        }

        /// <summary>
        /// 验证单个参数有效性。
        /// </summary>
        /// <param name="para">待检测的参数信息</param>
        /// <returns>验证有效</returns>
        protected List<ParameterInfo> DeserializeParameters(string args)
        {
            List<ParameterInfo> argParas = new List<ParameterInfo>();
            try
            {
                argParas = JsonConvert.DeserializeObject<List<ParameterInfo>>(args);
            }
            catch (Exception jsonEx)
            {
                throw new FormatException("初始化参数JSON格式异常！", jsonEx);
            }
            return argParas;
        }

        /// <summary>
        /// 初始化可选参数信息
        /// </summary>
        protected abstract void InitParaSettings();

        /// <summary>
        /// 可选参数信息集
        /// </summary>
        public abstract List<ParameterInfo> Parameters { get; }

        #region ICodeSeg 成员

        /// <summary>
        /// 验证参数有效性。
        /// </summary>
        /// <param name="inputParameters">参数序列化字符串</param>
        /// /// <param name="errorMess">无效时错误提示</param>
        /// <returns>验证有效</returns>
        public bool ValidateArgsFormat(List<ParameterInfo> inputParameters, out string errorMess)
        {
            bool isValidated = true;
            errorMess = string.Empty;
            List<string> paramenterKeys = this.Parameters.Select(s => s.ParamenterKey).Distinct().ToList();

            ParameterInfo inputPara = null;
            ParameterInfo rulePara = null;
            foreach (string paraName in paramenterKeys)
            {
                inputPara = inputParameters.FirstOrDefault(s => s.ParamenterKey == paraName);
                if (inputPara == null)
                {
                    errorMess = string.Format("没有找到期望的参数{0}信息", paraName);
                    isValidated = false;
                }
                else
                {
                    rulePara = Parameters.FirstOrDefault(s => s.ParamenterKey == paraName);
                    if (rulePara != null && !string.IsNullOrEmpty(rulePara.CheckFormat) && !Regex.IsMatch(inputPara.ParamenterValues, rulePara.CheckFormat))
                    {
                        errorMess = string.Format($"{this.Description}码段参数{rulePara.ParamenterKey}的值[{inputPara.ParamenterValues}]格式不正确或超出范围", paraName);
                        isValidated = false;
                    }
                }
                if (!isValidated)
                    break;
            }
            return isValidated;
        }

        /// <summary>
        /// 长度。
        /// </summary>
        public int Length
        {
            get { return this.length; }
        }

        /// <summary>
        /// 生成码。
        /// </summary>
        public abstract string Generate();

        /// <summary>
        /// 初始化。
        /// </summary>
        /// <param name="paras">参数信息</param>
        public virtual void Initialize(List<ParameterInfo> paras)
        {
            try
            {
                this.inputParameters.Clear();

                if (this.Parameters.Count>0 && (paras == null || paras.Count == 0))
                    throw new ArgumentNullException("args");

                string errorMess;
                if (!ValidateArgsFormat(paras, out errorMess))
                {
                    throw new FormatException(errorMess);
                }
                foreach(string paraName in Parameters.Select(s => s.ParamenterKey).Distinct())
                {
                    if (!this.inputParameters.ContainsKey(paraName))
                    {
                        this.inputParameters.Add(paraName, paras.FirstOrDefault(s => s.ParamenterKey == paraName).ParamenterValues);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"初始化参数解析异常，{ex.Message}！", ex);
            }
        }

        /// <summary>
        /// 参数标准化格式信息。
        /// </summary>
        public List<ParameterInfo> GetArgsFormat()
        {
            return Parameters;
        }

        ///// <summary>
        ///// 验证参数有效性。
        ///// </summary>
        ///// <param name="inputParameters">参数信息</param>
        ///// /// <param name="errorMess">无效时错误提示</param>
        ///// <returns>验证有效</returns>
        //public bool ValidateArgsFormat(List<ParameterInfo> inputParameters, out string errorMess)
        //{
        //    //List<ParameterInfo> inputParameters = DeserializeParameters(argsStr);
        //    errorMess = string.Empty;
        //    return ValidateArgsFormat(inputParameters, out errorMess);
        //}

        /// <summary>
        /// 初始化参数只读。
        /// </summary>
        public abstract bool ArgsReadonly { get; }

        /// <summary>
        /// 码段描述信息。
        /// </summary>
        public abstract string Description { get; }

        /// <summary>
        /// 固定长度不可调整。
        /// </summary>
        public abstract bool FixLength { get; }

        /// <summary>
        /// 码段位置限制。
        /// </summary>
        public abstract PositionType PositionStandard { get; }

        #endregion

    }

}
