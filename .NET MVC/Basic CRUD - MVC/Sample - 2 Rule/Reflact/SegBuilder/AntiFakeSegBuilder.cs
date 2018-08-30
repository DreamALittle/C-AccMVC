using Acctrue.CMC.Model.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Acctrue.CMC.CodeBuild.SegBuilder
{
    /// <summary>
    /// 防伪码段生成器（此码段建议最好放到末尾）
    /// 它是根据指定码串经过算法生成四位数字字符串
    /// </summary>
    public class AntiFakeSegBuilder : SegBase, IAntiFakeSeg
    {
        /// <summary>
        /// 参数信息集合
        /// </summary>
        protected static List<ParameterInfo> parameters = new List<ParameterInfo>();
        /// <summary>
        /// 参数锁对象
        /// </summary>
        private static object parameters_lock = new object();
        /// <summary>
        /// 参数信息格式初始化状态
        /// </summary>
        private static bool parametersFormat_init = false;

        public AntiFakeSegBuilder()
        {
            this.length = 4;
        }
        public AntiFakeSegBuilder(int length)
        {
            if (length < 1 || length > 4)
                throw new OverflowException("length");
            this.length = length;
        }

        #region 重写父类方法
        protected override void InitParaSettings()
        {
            if (!parametersFormat_init)
            {
                lock (parameters_lock)
                {
                    if (!parametersFormat_init)
                    {
                        parametersFormat_init = true;
                    }
                }
            }
        }
        public override List<ParameterInfo> Parameters { get { return parameters; } }
        #endregion

        #region 抽象类SegBase实现

        public override string Generate()
        {
            throw new NotSupportedException();
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
                return "防伪码段";
            }
        }

        public override bool FixLength
        {
            get { return true; }
        }

        public override PositionType PositionStandard
        {
            get { return PositionType.AtEnd| PositionType.NotBegin; }
        }

        #endregion

        #region ISerialNumber 成员
        public string Generate(string text)
        {
            if (text == null || text == string.Empty)
                throw new ArgumentNullException("text");
            return GetFingerStr(text, this.Length);
        }
        #endregion

        /// <summary>
        /// 获取指定位数防伪码
        /// </summary>
        /// <param name="str">待防伪字符串</param>
        /// <param name="len">防伪码长度</param>
        /// <returns></returns>
        public static string GetFingerStr(string str, int len)
        {
            //10位防伪Salt 12121, 7, 431, 41, 234, 32423, 5443, 42, 2, 98 
            int[] saltList = new int[10] { 12121, 7, 431, 41, 234, 32423, 5443, 42, 2, 98 };
            char[] ch = new char[len];
            for (int i = 0; i < saltList.Length; i++)
            {
                UInt32 fb = GetFingerBit(str, Convert.ToUInt32(saltList[i]));
                if (len > i)
                    ch[i] = (char)(fb + '0');
            }
            return new string(ch);
        }

        /// <summary>
        /// 获取单位防伪码
        /// </summary>
        /// <param name="str">待防伪字符串</param>
        /// <param name="salt">防伪参数</param>
        /// <returns></returns>
        private static UInt32 GetFingerBit(string str, UInt32 salt)
        {
            foreach (char c in str)
                salt = (salt << 5) + salt + c;
            return salt % 10;
        }


    }
}
