using Acctrue.CMC.Model.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Acctrue.CMC.CodeBuild.SegBuilder
{
    /// <summary>
    /// 固定文本码段生成器
    /// 根据指定的字符串的生成的码段。
    /// </summary>
    public class FixedTextSegBuilder : SegBase
    {
        private string _value;
        protected static List<ParameterInfo> parameters = new List<ParameterInfo>();
        private static object parameters_lock = new object();
        private static bool parametersFormat_init = false;
        private bool initialized = false;

        #region ICodeSeg 成员


        protected override void InitParaSettings()
        {
            if (!parametersFormat_init)
            {
                lock (parameters_lock)
                {
                    if (!parametersFormat_init)
                    {
                        parameters.Add(new ParameterInfo { ParamenterKey = "FixChars", ParamenterValues = "", DisplayName="固定字符", Description = "输入固定字符参数", CheckFormat= "^[a-zA-Z0-9]+$" });

                        //8.30添加码规则补位 阿涛哥
                        #region
                        //for (int i = 1; i <= 20; i++)
                        //{
                        //    parameters.Add(new ParameterInfo { ParamenterKey = "Length", ParamenterValues = i + "位", DisplayName = "长度", Description = i + "位", CheckFormat = "^([1-9]*)$" });
                        //}

                        //parameters.Add(new ParameterInfo { ParamenterKey = "Fill", ParamenterValues = "", DisplayName = "补位/截取(左、右)", Description = "补位或者截取", CheckFormat = "^[a-zA-Z1-9]*)$" });
                        //parameters.Add(new ParameterInfo { ParamenterKey = "FillChar", ParamenterValues = "", DisplayName = "填充内容", Description = "填充内容", CheckFormat = "^([a-zA-Z1-9]*)$" });
                        //#endregion

                        parametersFormat_init = true;
                    }
                }
            }
        }
        public override List<ParameterInfo> Parameters { get { return parameters; } }
        public override string Generate()
        {
            return _value;
        }

        public override void Initialize(List<ParameterInfo> args)
        {
            initialized = false;
            base.Initialize(args);
            try
            {
                if (this.inputParameters != null)
                _value = this.inputParameters["FixChars"];
                this.length = _value.Length;
                initialized = true;
            }
            catch (Exception ex)
            {
                throw new Exception("初始化参数解析异常！", ex);
            }
        }


        public override bool ArgsReadonly
        {
            get
            {
                return false;
            }
        }

        public override string Description
        {
            get
            {
                return "固定文本";
            }
        }

        public override bool FixLength
        {
            get { return true; }
        }

        public override PositionType PositionStandard
        {
            get { return PositionType.Any; }
        }
        #endregion
    }
}
