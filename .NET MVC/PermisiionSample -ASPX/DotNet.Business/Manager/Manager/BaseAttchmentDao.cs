//-----------------------------------------------------------
// All Rights Reserved , Copyright (C) 2008 , ESSE , Ltd .
//-----------------------------------------------------------

using System;
using System.Data;

namespace DotNet.Common.Business
{
    using DotNet.Common.Model;
    using DotNet.Common.Utilities;
    using DotNet.Common.DbUtilities;

    /// <summary>
    /// BUBaseAttchment
    /// ����
    ///		
    ///     2007.11.12 �汾��1.0 JiRiGaLa 
    ///		
    /// �汾��1.0
    ///
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2007.11.12</date>
    /// </author> 
    /// </summary>
    public class BaseAttchmentDao : BaseDao, IBaseDao
    {
        public BaseAttchmentDao()
        {
            base.CurrentTableName = BaseAttchmentTable.TableName;
        }

        public BaseAttchmentDao(IDbHelper dbHelper, BaseUserInfo userInfo)
        {
            this.DbHelper = dbHelper;
            this.UserInfo = userInfo;
        }

        private String defaultorder;
        public String DefaultOrder
        {
            get
            {
                defaultorder = BaseAttchmentTable.FieldSortCode;
                return defaultorder;
            }
        }

        #region public void ClearProperty(BaseAttchmentEntity attchmentEntity)
        /// <summary>
        /// �������
        /// </summary>
        public void ClearProperty(BaseAttchmentEntity attchmentEntity)
        {
            attchmentEntity.ID = String.Empty;	// ����
            attchmentEntity.CategoryID = String.Empty;	// ���ܷ������
            attchmentEntity.ObjectID = String.Empty; // Ψһʶ�����
            attchmentEntity.FileName = String.Empty; // �ļ���
            attchmentEntity.FilePath = String.Empty; // �ļ�·��
            attchmentEntity.FileContent = String.Empty; // �ļ�����
            attchmentEntity.ReadCount = 0;            // ���Ķ�����
            attchmentEntity.SortCode = String.Empty; // ����
            attchmentEntity.Description = String.Empty; // ��ע
            attchmentEntity.StateCode = String.Empty;	// ״̬��
            attchmentEntity.Enabled = true;	        // ��Ч
            attchmentEntity.CreateUserID = String.Empty; // �����ߴ���
            attchmentEntity.CreateDate = String.Empty;	// ����ʱ��
            attchmentEntity.ModifyUserID = String.Empty; // ����޸��ߴ���
            attchmentEntity.ModifyDate = String.Empty; // ����޸�ʱ��
        }
        #endregion

        #region public BaseAttchmentEntity GetFrom(DataRow dataRow)
        /// <summary>
        /// �������ж�ȡ
        /// </summary>
        /// <param name="dataRow">������</param>
        /// <returns>BUBaseAttchmentDefine</returns>
        public BaseAttchmentEntity GetFrom(DataRow dataRow, BaseAttchmentEntity attchmentEntity)
        {
            attchmentEntity.ID = dataRow[BaseAttchmentTable.FieldID].ToString();                   // ����
            attchmentEntity.CategoryID = dataRow[BaseAttchmentTable.FieldCategoryID].ToString();           // ���ܷ������
            attchmentEntity.ObjectID = dataRow[BaseAttchmentTable.FieldObjectID].ToString();             // Ψһʶ�����
            attchmentEntity.FileName = dataRow[BaseAttchmentTable.FieldFileName].ToString();             // �ļ���
            attchmentEntity.FilePath = dataRow[BaseAttchmentTable.FieldFilePath].ToString();             // �ļ�·��
            attchmentEntity.FileContent = dataRow[BaseAttchmentTable.FieldFileContent].ToString();          // �ļ�����
            attchmentEntity.ReadCount = int.Parse(dataRow[BaseAttchmentTable.FieldReadCount].ToString()); // ���Ķ�����
            attchmentEntity.StateCode = dataRow[BaseAttchmentTable.FieldStateCode].ToString();            // ״̬��
            attchmentEntity.SortCode = dataRow[BaseAttchmentTable.FieldSortCode].ToString();             // ����
            attchmentEntity.Description = dataRow[BaseAttchmentTable.FieldDescription].ToString();          // ��ע
            attchmentEntity.Enabled = dataRow[BaseAttchmentTable.FieldEnabled].ToString().Equals("1");  // ��Ч
            attchmentEntity.CreateUserID = dataRow[BaseAttchmentTable.FieldCreateUserID].ToString();        // �����ߴ���
            attchmentEntity.CreateDate = dataRow[BaseAttchmentTable.FieldCreateDate].ToString();           // ����ʱ��
            attchmentEntity.ModifyUserID = dataRow[BaseAttchmentTable.FieldModifyUserID].ToString();        // ����޸��ߴ���
            attchmentEntity.ModifyDate = dataRow[BaseAttchmentTable.FieldModifyDate].ToString();           // ����޸�ʱ��
            return attchmentEntity;
        }
        #endregion

        #region public override String AddEntity(Object myObject) ���һ����¼
        /// <summary>
        /// ���һ����¼
        /// </summary>
        /// <param name="mySend">���Ķ���</param>
        /// <returns>����</returns>
        public override String AddEntity(Object myObject)
        {
            String id = BaseSequenceDao.Instance.GetSequence(this.DbHelper, BaseAttchmentTable.TableName);
            BaseAttchmentEntity myAttchment = (BaseAttchmentEntity)myObject;
            SQLBuilder sqlBuilder = new SQLBuilder(this.DbHelper);
            sqlBuilder.BeginInsert(BaseAttchmentTable.TableName);
            sqlBuilder.SetValue(BaseAttchmentTable.FieldID, id);
            sqlBuilder.SetValue(BaseAttchmentTable.FieldCategoryID, myAttchment.CategoryID);
            sqlBuilder.SetValue(BaseAttchmentTable.FieldObjectID, myAttchment.ObjectID);
            sqlBuilder.SetValue(BaseAttchmentTable.FieldFileName, myAttchment.FileName);
            sqlBuilder.SetValue(BaseAttchmentTable.FieldDescription, myAttchment.Description);
            sqlBuilder.SetValue(BaseAttchmentTable.FieldCreateUserID, this.UserInfo.ID);
            sqlBuilder.SetDBNow(BaseAttchmentTable.FieldCreateDate);
            sqlBuilder.SetDBNow(BaseAttchmentTable.FieldModifyDate);
            return sqlBuilder.EndInsert() > 0 ? id : String.Empty;
        }
        #endregion
    }
}