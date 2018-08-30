using Acctrue.CMC.Model.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Acctrue.CMC.CodeBuild.SegBuilder
{
    /// <summary>
    /// 流水号码段生成器。
    /// </summary>
    public class SerialNoSegBuilder : SegBase, ISerialSeg
    {
        long _maxValue = 0;
        long _curValue = 1;
        protected static List<ParameterInfo> parameters = new List<ParameterInfo>();
        private static object parameters_lock = new object();
        private static bool parametersFormat_init = false;

        #region ICodeSegBuilder 成员

        protected override void InitParaSettings()
        {
            if (!parametersFormat_init)
            {
                lock (parameters_lock)
                {
                    if (!parametersFormat_init)
                    {
                        for(int i = 1; i <= 11; i++)
                        {
                            parameters.Add(new ParameterInfo { ParamenterKey = "Length", ParamenterValues = i.ToString(), DisplayName="长度", Description = i+"位", CheckFormat= "^([1-9]|1[0-1])$" });
                        }
                        parametersFormat_init = true;
                    }
                }
            }
        }
        public override List<ParameterInfo> Parameters { get { return parameters; } }
        public override string Generate()
        {
            if (_curValue > _maxValue)
                throw new ArgumentOutOfRangeException("流水号超出最大值");
            return (_curValue++).ToString().PadLeft(this.Length, '0');
        }

        public override void Initialize(List<ParameterInfo> args)
        {
            base.Initialize(args);

            int length;
            int.TryParse(this.inputParameters["Length"], out length);
            if (length < 1 || length > 11)
                throw new ArgumentException("args 应该为1-11数字字符串");

            this.length = length;

            string v = "999999999999999";
            _maxValue = long.Parse(v.Substring(0, length));
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
                return "流水号";
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

        #region ISerialNumber 成员

        /// <summary>
        /// 流水号是否越界。
        /// </summary>
        public bool Overflow
        {
            get
            {
                return _curValue >= _maxValue;
            }
        }


        /// <summary>
        /// 当前序列号。
        /// </summary>
        public string CurrentSerialNoString
        {
            get
            {
                return _curValue.ToString().PadLeft(this.Length, '0');
            }
        }

        /// <summary>
        /// 设置当前序列位置 。
        /// </summary>
        /// <param name="value">序列位置参数</param>
        public void SetLastSegValue(object value)
        {
            if (value == null || value.ToString() == string.Empty)
                _curValue = 1;
            else
                _curValue = Convert.ToInt64(value) + 1;
        }

        /// <summary>
        /// 获取指定申请数量的结束序列号。
        /// </summary>
        /// <param name="applyAmount">申请数量。</param>
        /// <returns></returns>
        public string GetCurrentEndSerialNoCode(int applyAmount)
        {
            return (_curValue + applyAmount - 1).ToString().PadLeft(this.Length, '0');
        }

        /// <summary>
        /// 是否是有效申请数量。
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public bool IsValidApplyAmount(int amount)
        {
            return (amount < (_maxValue - _curValue));
        }

        #endregion
    }
}
