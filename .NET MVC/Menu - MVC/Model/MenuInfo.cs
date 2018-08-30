using Acctrue.Library.Data.Definition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Acctrue.CMC.Model.Menu
{
    [DbTable("Menus")]
    [Serializable()]
    [DataContract()]
    public class MenuInfo
    {
        /// <summary>
        /// 菜单Id
        /// </summary>
        [DataMember()]
        [DbKey(IsDbGenerate = false)]
        public int MenuId
        {
            get;
            set;
        }

        /// <summary>
        /// 菜单名称
        /// </summary>
        [DataMember()]
        public string MenuName
        {
            get;
            set;
        }

        /// <summary>
        /// 该菜单是否显示在树中
        /// </summary>
        [DataMember()]
        public bool MenuVisible
        {
            get;
            set;
        }

        /// <summary>
        /// 菜单状态(1启用，0禁用)
        /// </summary>
        [DataMember()]
        public MenuStatus MenuStatus
        {
            get;
            set;
        }

        /// <summary>
        /// 父节点
        /// </summary>
        [DataMember()]
        public int? MenuParentId
        {
            get;
            set;
        }

        /// <summary>
        /// Web应用中是否显示该菜单
        /// </summary>
        [DataMember()]
        public bool WebEnabled
        {
            get;
            set;
        }

        /// <summary>
        /// Web应用中链接页面Url
        /// </summary>
        [DataMember()]
        [AllowNull]
        public string WebPageUrl
        {
            get;
            set;
        }

        /// <summary>
        /// Web应用中页面链接target
        /// </summary>
        [DataMember()]
        [AllowNull]
        public string WebTarget
        {
            get;
            set;
        }

        /// <summary>
        /// 以英文逗号分隔操作名称列表
        /// </summary>
        [DataMember()]
        [AllowNull]
        public string ActionNames
        {
            get;
            set;
        }

        /// <summary>
        /// 菜单显示顺序
        /// </summary>
        [DataMember()]
        public int Seqno
        {
            get;
            set;
        }
    }
}
