using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Acctrue.Library.Data.Definition;
using System.Runtime.Serialization;

namespace Acctrue.CMC.Model.Systems
{
    [DbTable("AppInterFace")]
    [DataContract()]
    [Serializable()]
    public partial class AppInterFaceInfo
    {
        /// <summary>
        /// Ӧ��Id
        /// </summary>
        [DataMember()]
        public int AppSettingID
        {
            get;
            set;
        }
        /// <summary>
        /// Id
        /// </summary>
        [DbKey]
        [DataMember()]
        public int ID
        {
            get;
            set;
        }
        /// <summary>
        /// �����������ģ�
        /// </summary>
        [DataMember()]
        public string InterfaceFunctionName
        {
            get;
            set;
        }
        /// <summary>
        /// �����ռ�
        /// </summary>
        [DataMember()]
        public string Namespace
        {
            get;
            set;
        }

        /// <summary>
        /// ������
        /// </summary>
        [DataMember()]
        public string FunctionName
        {
            get;
            set;
        }

        /// <summary>
        /// ������
        /// </summary>
        [DataMember()]
        public string Creater { get; set; }

        /// <summary>
        /// CreateDate
        /// </summary>
        [DataMember()]
        public DateTime CreateDate { get; set; }
    }
}
