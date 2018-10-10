//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2008 , ESSE , Ltd. 
//------------------------------------------------------------

using System;
using System.Data;
using System.Reflection;
using System.Diagnostics;

namespace ESSE.Common.Service
{
    using ESSE.Common;
    using ESSE.Common.Domain;
    using ESSE.Common.Utilities;
    using ESSE.Common.DbUtilities;
    using ESSE.Common.Persistence;

    /// <summary>
    /// ImpersonationService
    /// 
    /// �޸ļ�¼
    /// 
    ///		2007.08.18 �汾��2.3 JiRiGaLa �� �ļ����޸�Ϊ ImpersonationService ��
    ///		2007.08.15 �汾��2.1 JiRiGaLa �Ľ������ٶȲ��� WebService �������� ��ʽ�������ݡ�
    ///		2007.08.14 �汾��2.0 JiRiGaLa �Ľ������ٶȲ��� Instance ��ʽ�������ݡ�
    ///		2007.08.16 �汾��2.2 JiRiGaLa �� Impersonation ����ת�Ƶ������С�
    ///		2007.08.15 �汾��2.1 JiRiGaLa �Ľ������ٶȲ��� WebService �������� ��ʽ�������ݡ�
    ///		2007.08.14 �汾��2.0 JiRiGaLa �Ľ������ٶȲ��� Instance ��ʽ�������ݡ�
    ///		2007.08.02 JiRiGaLa �������롣
    ///		
    /// �汾��2.3
    ///
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2007.08.18</date>
    /// </author> 
    /// </summary>
    public class ImpersonationService : System.MarshalByRefObject
    {
        private static ImpersonationService instance = null;
        private static Object locker = new Object();

        public static ImpersonationService Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (locker)
                    {
                        if (instance == null)
                        {
                            instance = new ImpersonationService();
                        }
                    }
                }
                return instance;
            }
        }

        #region public void Load()
        /// <summary>
        /// ���ط����
        /// </summary>
        public void Load()
        {
        }
        #endregion
    }
}