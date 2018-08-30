using Acctrue.CMC.Model.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Acctrue.CMC.CodeBuild.SegBuilder
{
    /// <summary>
    /// 日期类型码段
    /// </summary>
    public class DateTimeSegBuilder :SegBase
    {
        private string _value;
        protected static List<ParameterInfo> parameters = new List<ParameterInfo>();
        private static object parameters_lock = new object();
        private static bool parametersFormat_init = false;
        private bool initialized = false;

        protected override void InitParaSettings()
        {
            if (!parametersFormat_init)
            {
                lock (parameters_lock)
                {
                    if (!parametersFormat_init)
                    {

                        parameters.Add(new ParameterInfo { ParamenterKey = "Time", ParamenterValues = "yyyy", DisplayName = "日期", Description = "年", CheckFormat = "^([a-zA-Z0-9]+)$" });
                        parameters.Add(new ParameterInfo { ParamenterKey = "Time", ParamenterValues = "MM", DisplayName = "日期", Description = "月", CheckFormat = "^([a-zA-Z0-9]+)$" });
                        parameters.Add(new ParameterInfo { ParamenterKey = "Time", ParamenterValues = "dd", DisplayName = "日期", Description = "日", CheckFormat = "^(([a-zA-Z0-9]+)$" });

                        parametersFormat_init = true;
                    }
                }
            }
        }

        public override List<ParameterInfo> Parameters { get { return parameters; } }

        public override string Generate()
        {
             return DateTime.Now.ToString(_value);
        }

        public override void Initialize(List<ParameterInfo> args)
        {
            base.Initialize(args);


            _value = this.inputParameters["Time"];
            this.length = _value.Length;

        }

        public override bool ArgsReadonly
        {
            get
            {
                return true;
            }
        }

        public override string Description
        {
            get
            {
                return "日期";
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
    }
}
