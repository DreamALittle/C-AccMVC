//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2008 , ESSE , Ltd. 
//------------------------------------------------------------

using System;
using System.Data;
using System.Reflection;
using System.Diagnostics;

namespace DotNet.Common.Service
{
    using DotNet.Common.Utilities;
    using DotNet.Common.DbUtilities;
    using DotNet.Common.Business;

    /// <summary>
    /// ModuleOperationCheckService
    /// Ȩ���жϷ���
    /// 
    /// �޸ļ�¼
    /// 
    ///		2008.05.10 �汾��2.4 JiRiGaLa ����Ϊ ModuleOperationCheckService��
    ///		2007.10.18 �汾��2.3 JiRiGaLa Authorization ������������
    ///		2007.08.15 �汾��2.2 JiRiGaLa �Ľ������ٶȲ��� WebService �������� ��ʽ�������ݡ�
    ///		2007.08.14 �汾��2.1 JiRiGaLa �Ľ������ٶȲ��� Instance ��ʽ�������ݡ�
    ///		2007.05.14 �汾��1.0 JiRiGaLa ���������ݿ����ӵķ��롣
    ///		
    /// �汾��2.2
    ///
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2007.08.15</date>
    /// </author> 
    /// </summary>
    public class ModuleOperationCheckService : System.MarshalByRefObject
    {
        private static ModuleOperationCheckService instance = null;
        private static Object locker = new Object();

        public static ModuleOperationCheckService Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (locker)
                    {
                        if (instance == null)
                        {
                            instance = new ModuleOperationCheckService();
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

        #region public DataTable GetAuthorization(BaseUserInfo userInfo)
        /// <summary>
        /// ��õ�ǰ����Ա������Ȩ��
        /// </summary>
        /// <param name="userInfo">�û�</param>
        /// <returns>���ݱ�</returns>
        public DataTable GetAuthorization(BaseUserInfo userInfo)
        {
            return this.GetAuthorization(userInfo, userInfo.ID);
        }
        #endregion

        #region public DataTable GetAuthorization(BaseUserInfo userInfo, String userID)
        /// <summary>
        /// ���һ��ְԱ������Ȩ��
        /// </summary>
        /// <param name="userInfo">�û�</param>
        /// <param name="userID">ְԱ����</param>
        /// <returns>���ݱ�</returns>
        public DataTable GetAuthorization(BaseUserInfo userInfo, String staffID)
        {
            // д�������Ϣ
            #if (DEBUG)
                int milliStart = BaseBusinessLogic.Instance.StartDebug(userInfo, MethodBase.GetCurrentMethod());
            #endif
            
            DataTable returnValue = new DataTable(BaseModuleOperationCheckDao.TableName);
            IDbHelper dbHelper = DbHelperFactory.GetHelper();
            try
            {
                dbHelper.Open();
                returnValue = BaseModuleOperationCheckDao.Instance.GetAuthorization(dbHelper, staffID);
                BaseLogDao.Instance.Add(dbHelper, userInfo, MethodBase.GetCurrentMethod());
            }
            catch (Exception exception)
            {
                BaseExceptionDao.Instance.LogException(dbHelper, userInfo, exception);
                throw exception;
            }
            finally
            {
                dbHelper.Close();
            }
            // д�������Ϣ
            #if (DEBUG)
                BaseBusinessLogic.Instance.EndDebug(MethodBase.GetCurrentMethod(), milliStart);
            #endif
            return returnValue;
        }
        #endregion

        #region public DataTable GetAuthorization(BaseUserInfo userInfo, String userID, String moduleCode)
        /// <summary>
        /// ���һ��ְԱ��ĳһģ���Ȩ��
        /// </summary>
        /// <param name="userInfo">�û�</param>
        /// <param name="userID">ְԱ����</param>
        /// <param name="moduleCode">ģ�����</param>
        /// <returns>���ݱ�</returns>
        public DataTable GetAuthorization(BaseUserInfo userInfo, String staffID, String moduleCode)
        {
            // д�������Ϣ
            #if (DEBUG)
                int milliStart = BaseBusinessLogic.Instance.StartDebug(userInfo, MethodBase.GetCurrentMethod());
            #endif
            
            DataTable returnValue = new DataTable(BaseModuleOperationCheckDao.TableName);
            IDbHelper dbHelper = DbHelperFactory.GetHelper();
            try
            {
                dbHelper.Open();
                returnValue = BaseModuleOperationCheckDao.Instance.GetAuthorization(dbHelper, staffID, moduleCode);
                BaseLogDao.Instance.Add(dbHelper, userInfo, MethodBase.GetCurrentMethod());
            }
            catch (Exception exception)
            {
                BaseExceptionDao.Instance.LogException(dbHelper, userInfo, exception);
                throw exception;
            }
            finally
            {
                dbHelper.Close();
            }
            // д�������Ϣ
            #if (DEBUG)
                BaseBusinessLogic.Instance.EndDebug(MethodBase.GetCurrentMethod(), milliStart);
            #endif
            return returnValue;
        }
        #endregion
        
        #region public bool Authorization(BaseUserInfo userInfo, String moduleCode, OperationCode operationCode)
        /// <summary>
        /// �Ƿ�����Ӧģ�����ӦȨ��
        /// </summary>
        /// <param name="userInfo">�û�</param>
        /// <param name="userID">ְԱ����</param>
        /// <param name="moduleCode">ģ����</param>
        /// <param name="operationCode">Ȩ�ޱ��</param>
        /// <returns>�Ƿ���Ȩ��</returns>
        public bool Authorization(BaseUserInfo userInfo, String moduleCode, OperationCode operationCode)
        {
            return this.Authorization(userInfo, userInfo.ID, moduleCode, operationCode.ToString());
        }
        #endregion

        #region public bool Authorization(BaseUserInfo userInfo, String moduleCode, String operationCode)
        /// <summary>
        /// �Ƿ�����Ӧģ�����ӦȨ��
        /// </summary>
        /// <param name="userInfo">�û�</param>
        /// <param name="moduleCode">ģ����</param>
        /// <param name="operationCode">Ȩ�ޱ��</param>
        /// <returns>�Ƿ���Ȩ��</returns>
        public bool Authorization(BaseUserInfo userInfo, String moduleCode, String operationCode)
        {
            return this.Authorization(userInfo, userInfo.ID, moduleCode, operationCode);
        }
        #endregion

        #region public bool Authorization(BaseUserInfo userInfo, String userID, String moduleCode, OperationCode operationCode)
        /// <summary>
        /// �Ƿ�����Ӧģ�����ӦȨ��
        /// </summary>
        /// <param name="userInfo">�û�</param>
        /// <param name="userID">ְԱ����</param>
        /// <param name="moduleCode">ģ����</param>
        /// <param name="operationCode">Ȩ�ޱ��</param>
        /// <returns>�Ƿ���Ȩ��</returns>
        public bool Authorization(BaseUserInfo userInfo, String staffID, String moduleCode, OperationCode operationCode)
        {
            return this.Authorization(userInfo, staffID, moduleCode, operationCode.ToString());
        }
        #endregion

        #region public bool Authorization(BaseUserInfo userInfo, String userID, String moduleCode, String operationCode)
        /// <summary>
        /// �Ƿ�����Ӧģ�����ӦȨ��
        /// </summary>
        /// <param name="userInfo">�û�</param>
        /// <param name="userID">ְԱ����</param>
        /// <param name="moduleCode">ģ����</param>
        /// <param name="operationCode">Ȩ�ޱ��</param>
        /// <returns>�Ƿ���Ȩ��</returns>
        public bool Authorization(BaseUserInfo userInfo, String staffID, String moduleCode, String operationCode)
        {
            // д�������Ϣ
            #if (DEBUG)
                int milliStart = BaseBusinessLogic.Instance.StartDebug(userInfo, MethodBase.GetCurrentMethod());
            #endif
            bool returnValue = false;
            IDbHelper dbHelper = DbHelperFactory.GetHelper();
            try
            {
                dbHelper.Open();
                returnValue = BaseModuleOperationCheckDao.Instance.Authorization(dbHelper, staffID, moduleCode, operationCode);
                BaseLogDao.Instance.Add(dbHelper, userInfo, MethodBase.GetCurrentMethod());
            }
            catch (Exception exception)
            {
                BaseExceptionDao.Instance.LogException(dbHelper, userInfo, exception);
                throw exception;
            }
            finally
            {
                dbHelper.Close();
            }
            // д�������Ϣ
            #if (DEBUG)
                BaseBusinessLogic.Instance.EndDebug(MethodBase.GetCurrentMethod(), milliStart);
            #endif
            return returnValue;
        }
        #endregion
    }
}