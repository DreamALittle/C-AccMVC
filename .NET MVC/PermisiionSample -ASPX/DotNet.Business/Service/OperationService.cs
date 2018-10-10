//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2008 , ESSE , Ltd. 
//------------------------------------------------------------

using System;
using System.Data;
using System.Reflection;
using System.Diagnostics;

namespace DotNet.Common.Service
{
    using DotNet.Common.Model;
    using DotNet.Common.Utilities;
    using DotNet.Common.DbUtilities;
    using DotNet.Common.Business;
    using DotNet.Common.IService;

    /// <summary>
    /// OperationService
    /// Ȩ�޲�������
    /// 
    /// �޸ļ�¼
    ///  
    ///		2008.05.09 �汾��2.4 JiRiGaLa �����޸�Ϊ OperationService��
    ///		2008.03.23 �汾��2.3 JiRiGaLa �Ľ��ӿڹ��ܣ�������һ�η�Ծ��
    ///		2007.12.24 �汾��2.2 JiRiGaLa �Ľ�������Ϣ��ʽ��
    ///		2007.08.15 �汾��2.1 JiRiGaLa �Ľ������ٶȲ��� WebService �������� ��ʽ�������ݡ�
    ///		2007.08.14 �汾��2.0 JiRiGaLa �Ľ������ٶȲ��� Instance ��ʽ�������ݡ�
    ///     2007.06.11 �汾��1.2 JiRiGaLa ���������Ϣ#if (DEBUG)��
    ///		2007.05.13 �汾��1.1 JiRiGaLa ����Ƿ��ظ����жϡ�
    ///		2007.05.11 �汾��1.0 JiRiGaLa ���Ȩ�ޡ�
    ///		
    /// �汾��2.4
    ///
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2008.05.19</date>
    /// </author> 
    /// </summary>
    public class OperationService : System.MarshalByRefObject, IOperationService
    {
        private static OperationService instance = null;
        private static Object locker = new Object();

        public static OperationService Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (locker)
                    {
                        if (instance == null)
                        {
                            instance = new OperationService();
                        }
                    }
                }
                return instance;
            }
        }

        public void Load()
        {
        }

        public DataTable GetList(BaseUserInfo userInfo)
        {
            // д�������Ϣ
            #if (DEBUG)
                int milliStart = BaseBusinessLogic.Instance.StartDebug(userInfo, MethodBase.GetCurrentMethod());
            #endif

            DataTable dataTable = new DataTable(BaseOperationTable.TableName);
            IDbHelper dbHelper = DbHelperFactory.GetHelper();
            try
            {
                dbHelper.Open();
                BaseOperationDao myPermission = new BaseOperationDao(dbHelper, userInfo);
                dataTable = myPermission.GetList();
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
            return dataTable;
        }

        #region public DataTable GetListByPermission(BaseUserInfo userInfo, String id) ��ȡ�б�
        /// <summary>
        /// ��ȡ�б�
        /// </summary>
        /// <param name="userInfo">�û�</param>
        /// <param name="id">����</param> 
        /// <returns>���ݼ�</returns>
        public DataTable GetListByPermission(BaseUserInfo userInfo, String id)
        {
            // д�������Ϣ
            #if (DEBUG)
                int milliStart = BaseBusinessLogic.Instance.StartDebug(userInfo, MethodBase.GetCurrentMethod());
            #endif
            DataTable dataTable = new DataTable();
            IDbHelper dbHelper = DbHelperFactory.GetHelper();
            try
            {
                dbHelper.Open();
                BaseModuleOperationDao myModuleOperation = new BaseModuleOperationDao(dbHelper, userInfo);
                dataTable = myModuleOperation.GetListByPermission(id);
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
            return dataTable;
        }
        #endregion

        public String Add(BaseUserInfo userInfo, BaseOperationEntity operationEntity, out String statusCode, out String statusMessage)
        {
            // д�������Ϣ
            #if (DEBUG)
                int milliStart = BaseBusinessLogic.Instance.StartDebug(userInfo, MethodBase.GetCurrentMethod());
            #endif
            statusCode = String.Empty;
            statusMessage = String.Empty;

            String returnValue = String.Empty;
            IDbHelper dbHelper = DbHelperFactory.GetHelper();
            try
            {
                dbHelper.Open();
                BaseOperationDao myOperationDao = new BaseOperationDao(dbHelper, userInfo);
                returnValue = myOperationDao.Add(operationEntity, out statusCode);
                // ���״̬��Ϣ
                statusMessage = myOperationDao.GetStateCodeMessage(statusCode);
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

        public String AddByDetail(BaseUserInfo userInfo, String code, String fullName, out String statusCode, out String statusMessage)
        {
            // д�������Ϣ
            #if (DEBUG)
                int milliStart = BaseBusinessLogic.Instance.StartDebug(userInfo, MethodBase.GetCurrentMethod());
            #endif            
            statusCode = String.Empty;
            statusMessage = String.Empty;
            
            String returnValue = String.Empty;
            IDbHelper dbHelper = DbHelperFactory.GetHelper();
            try
            {
                dbHelper.Open();
                BaseOperationDao myOperation = new BaseOperationDao(dbHelper, userInfo);
                returnValue = myOperation.AddByDetail(code, fullName, out statusCode);
                // ���״̬��Ϣ
                statusMessage = myOperation.GetStateCodeMessage(statusCode);
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

        public BaseOperationEntity Get(BaseUserInfo userInfo, String id)
        {
            // д�������Ϣ
            #if (DEBUG)
                int milliStart = BaseBusinessLogic.Instance.StartDebug(userInfo, MethodBase.GetCurrentMethod());
            #endif

            BaseOperationEntity operationEntity = new BaseOperationEntity();
            IDbHelper dbHelper = DbHelperFactory.GetHelper();
            try
            {
                dbHelper.Open();
                // ����ʵ����
                DataTable myReturnDataTable = new DataTable(BaseOperationTable.TableName);
                BaseOperationDao myPermission = new BaseOperationDao(dbHelper, userInfo);
                myReturnDataTable = myPermission.Get(id);
                operationEntity = new BaseOperationEntity(myReturnDataTable);
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
                return operationEntity;
        }

        public DataTable GetByCode(BaseUserInfo userInfo, String code, out String statusCode, out String statusMessage)
        {
            // д�������Ϣ
            #if (DEBUG)
                int milliStart = BaseBusinessLogic.Instance.StartDebug(userInfo, MethodBase.GetCurrentMethod());
            #endif
            statusCode = String.Empty;
            statusMessage = String.Empty;

            DataTable myReturnDataTable = new DataTable(BaseOperationTable.TableName);
            IDbHelper dbHelper = DbHelperFactory.GetHelper();
            try
            {
                dbHelper.Open();
                // ����ʵ����
                BaseOperationDao myOperation = new BaseOperationDao(dbHelper, userInfo);
                myReturnDataTable = myOperation.GetByCode(code);
                // ���״̬��Ϣ
                statusMessage = myOperation.GetStateCodeMessage(statusCode);
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
            return myReturnDataTable;
        }

        public String GetFullNameByCode(BaseUserInfo userInfo, String code)
        {
            // д�������Ϣ
            #if (DEBUG)
                int milliStart = BaseBusinessLogic.Instance.StartDebug(userInfo, MethodBase.GetCurrentMethod());
            #endif
            String returnValue = String.Empty;

            IDbHelper dbHelper = DbHelperFactory.GetHelper();
            try
            {
                dbHelper.Open();
                BaseOperationDao myPermissionDao = new BaseOperationDao(dbHelper, userInfo);
                returnValue = myPermissionDao.GetFullNameByCode(code);
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

        public int Update(BaseUserInfo userInfo, BaseOperationEntity operationEntity, out String statusCode, out String statusMessage)
        {
            // д�������Ϣ
            #if (DEBUG)
                int milliStart = BaseBusinessLogic.Instance.StartDebug(userInfo, MethodBase.GetCurrentMethod());
            #endif
            statusCode = String.Empty;
            statusMessage = String.Empty;

            int returnValue = 0;
            IDbHelper dbHelper = DbHelperFactory.GetHelper();
            try
            {
                dbHelper.Open();
                BaseOperationDao myPermissionDao = new BaseOperationDao(dbHelper, userInfo);
                returnValue = myPermissionDao.Update(operationEntity, out statusCode);
                // ���״̬��Ϣ
                statusMessage = myPermissionDao.GetStateCodeMessage(statusCode);
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

        public int BatchDelete(BaseUserInfo userInfo, String[] ids)
        {
            // д�������Ϣ
            #if (DEBUG)
                int milliStart = BaseBusinessLogic.Instance.StartDebug(userInfo, MethodBase.GetCurrentMethod());
            #endif

            int returnValue = 0;
            IDbHelper dbHelper = DbHelperFactory.GetHelper();
            try
            {
                dbHelper.Open();
                BaseOperationDao myPermissionDao = new BaseOperationDao(dbHelper, userInfo);
                returnValue = myPermissionDao.BatchDelete(ids);
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

        public DataTable BatchSave(BaseUserInfo userInfo, DataTable dataTable)
        {
            // д�������Ϣ
            #if (DEBUG)
                int milliStart = BaseBusinessLogic.Instance.StartDebug(userInfo, MethodBase.GetCurrentMethod());
            #endif
            
            DataTable myReturnDataTable = new DataTable(BaseOperationTable.TableName);
            IDbHelper dbHelper = DbHelperFactory.GetHelper();
            try
            {
                dbHelper.Open();
                // ����ʵ����
                BaseOperationDao myPermissionDao = new BaseOperationDao(dbHelper, userInfo);
                myPermissionDao.BatchSave(dataTable);
                myReturnDataTable = myPermissionDao.GetList();
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
            return myReturnDataTable;
        }

        #region public DataTable BatchSaveByModule(BaseUserInfo userInfo, DataTable dataTable, String id) ��������
        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="userInfo">�û�</param>
        /// <param name="dataTable">���ݱ�</param>
        /// <param name="id">ģ�����</param>
        /// <returns>���ݱ�</returns>
        public DataTable BatchSaveByModule(BaseUserInfo userInfo, DataTable dataTable, String id)
        {
            // д�������Ϣ
            #if (DEBUG)
                int milliStart = BaseBusinessLogic.Instance.StartDebug(userInfo, MethodBase.GetCurrentMethod());
            #endif
            IDbHelper dbHelper = DbHelperFactory.GetHelper();
            try
            {
                dbHelper.Open();
                BaseModuleOperationDao myModuleOperation = new BaseModuleOperationDao(dbHelper, userInfo);
                myModuleOperation.BatchSave(dataTable);
                dataTable = myModuleOperation.GetListByPermission(id);
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
            return dataTable;
        }
        #endregion
    }
}