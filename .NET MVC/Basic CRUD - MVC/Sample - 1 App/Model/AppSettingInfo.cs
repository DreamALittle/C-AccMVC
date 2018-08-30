using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Acctrue.Library.Data.Definition;
using System.Runtime.Serialization;

namespace Acctrue.CMC.Model.Systems
{
    [DbTable("AppSettings")]
    [DataContract()]
    [Serializable()]
    public partial class AppSettingInfo
    {
        /// <summary>
        /// Ӧ��Id
        /// </summary>
        [DataMember()]
        [DbKey]
        public int AppSettingID
        {
            get;
            set;
        }
        /// <summary>
        /// Id
        /// </summary>
        [DataMember()]
        public string AppID
        {
            get;
            set;
        }
        /// <summary>
        /// Secret
        /// </summary>
        [DataMember()]
        public string Secret
        {
            get;
            set;
        }
        /// <summary>
        /// ��������
        /// </summary>
        [DataMember()]
        public string Seed
        {
            get;
            set;
        }
        /// <summary>
        /// ��ҵ����
        /// </summary>
        [DataMember()]
        public string CorpCode
        {
            get;
            set;
        }
        /// <summary>
        /// ��ҵ����
        /// </summary>
        [DataMember()]
        public string CorpName
        {
            get;
            set;
        }
        /// <summary>
        ///  �������� 
        /// </summary>
        [DataMember()]
        public string SubCorpCode
        {
            get;
            set;
        }

        /// <summary>
        /// Ӧ��Ψһ��ʶ
        /// </summary>
        [DataMember()]
        [AllowNull]
        public string Token { get; set; }

        /// <summary>
        /// ��ǰ���ʵ�ַ
        /// </summary>
        [AllowNull]
        [DataMember()]
        public string IpAddress
        {
            get;
            set;
        }
        
        /// <summary>
        /// �û�״̬
        /// </summary>
        [DataMember()]
        public AppStatus AppStatus
        {
            get;
            set;
        }

        /// <summary>
        /// ����ʱ��
        /// </summary>
        [DataMember()]
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// ModifiedDate
        /// </summary>
        [DataMember()]
        public DateTime ModifiedDate { get; set; }

        /// <summary>
        /// ModifiedDate
        /// </summary>
        [DataMember()]
        public DateTime TokenExpireDate { get; set; }

        /// <summary>
        /// �Զ����
        /// </summary>
        [DataMember()]
        public bool IsAutoAudit { get; set; }

    }
}
