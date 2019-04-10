using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ReportEFCore.Models
{
    public class User
    {
         
        public int CorpId
        {
            get;
            set;
        }

        /// <summary>
        /// 用户Id
        /// </summary>
         
        [Key]
        public virtual int UserId
        {
            get;
            set;
        }

        /// <summary>
        /// 类型
        /// </summary>
         
        public int UserType
        {
            get;
            set;
        }

        /// <summary>
        /// 编码
        /// </summary>
         
       
        public string UserCode
        {
            get;
            set;
        }

        /// <summary>
        /// 登录名
        /// </summary>
         
        public string UserName
        {
            get;
            set;
        }

        /// <summary>
        /// 显示名称
        /// </summary>
         
         
        public string UserDisplayName
        {
            get;
            set;
        }

        /// <summary>
        /// 域SID
        /// </summary>
         
        public Guid? UserGuid
        {
            get;
            set;
        }

        /// <summary>
        /// 域帐号
        /// </summary>
         
         
        public string UserAccount
        {
            get;
            set;
        }

        /// <summary>
        /// 人员描述
        /// </summary>
         
         
        public string UserDescription
        {
            get;
            set;
        }

        /// <summary>
        /// 状态(1启用，0禁用)
        /// </summary>
         
        public int UserStatus
        {
            get;
            set;
        }

        /// <summary>
        /// 性别
        /// </summary>
         
         
        public string UserGender
        {
            get;
            set;
        }

        /// <summary>
        /// 生日
        /// </summary>
         
        public DateTime? UserBirthday
        {
            get;
            set;
        }

        /// <summary>
        /// 电话
        /// </summary>
         
         
        public string UserTelephone
        {
            get;
            set;
        }

        /// <summary>
        /// 手机
        /// </summary>
         
         
        public string UserMobile
        {
            get;
            set;
        }

        /// <summary>
        /// 邮箱
        /// </summary>
         
         
        public string UserEmail
        {
            get;
            set;
        }

        /// <summary>
        /// CreatedTime
        /// </summary>
         
        public DateTime CreatedTime
        {
            get;
            set;
        }

        /// <summary>
        /// Creator
        /// </summary>
         
         
        public string Creator
        {
            get;
            set;
        }

        /// <summary>
        /// ModifiedTime
        /// </summary>
         
        public DateTime ModifiedTime
        {
            get;
            set;
        }

        ///<summary>
        /// ADModifiedTime
        /// </summary>

        public DateTime ADModifiedTime
        {
            get;
            set;
        }


        /// <summary>
        /// Modifier
        /// </summary>
         
         
        public string Modifier
        {
            get;
            set;
        }
        /// <summary>
        /// 机构id
        /// </summary>
        public int? JiGouId
        {
            get;
            set;
        }

        /// <summary>
        ///微信openId
        /// </summary>
         
         
        public string OpenId
        {
            get;
            set;
        }

        /// <summary>
        ///微信地理位置
        /// </summary>
         
         
        public string Location
        {
            get;
            set;
        }

        /// <summary>
        ///欢乐扫openId
        /// </summary>
         
         
        public string HSOpenId
        {
            get;
            set;
        }
        /// <summary>
        /// 身份证信息
        /// </summary>
         
         
        public string ContactCard
        {
            get;
            set;
        }
        /// <summary>
        /// 公钥证书
        /// </summary>
         
         
        public string PublicKeyUrl
        {
            get;
            set;
        }
        /// <summary>
        /// 设备序列号
        /// </summary>
         
         
        public string KeySN
        {
            get;
            set;
        }
        /// <summary>
        /// 必须使用证书登录
        /// </summary>
         
        public int? IsKeyLogin
        {
            get;
            set;
        }
        /// <summary>
        /// 是否内部用户
        /// </summary>
         
        public bool? IsInternalUser
        {
            get;
            set;
        }
    }
}
