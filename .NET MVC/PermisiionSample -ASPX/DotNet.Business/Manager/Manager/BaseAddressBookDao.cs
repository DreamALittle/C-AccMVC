//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2008 , ESSE , Ltd. 
//------------------------------------------------------------

using System;
using System.Data;

namespace DotNet.Common.Business
{
    using DotNet.Common.Model;
    using DotNet.Common.Utilities;
    using DotNet.Common.DbUtilities;

    /// <summary>
    /// BaseAddressBookDao
    /// 
    /// �޸ļ�¼
    ///     2007.2.28 �汾��1.0 JiRiGaLa     
    ///     
    /// �汾��2.0
    /// 
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2007.02.28</date>
    /// </author>
	/// </summary>
    [Serializable]
    public class BaseAddressBookDao : BaseDao
    {
        public String PreviousID    = String.Empty;    // ��һ����¼���롣
        public String NextID        = String.Empty;    // ��һ����¼���롣

        public BaseAddressBookDao()
        {
            base.CurrentTableName = BaseAddressBookTable.TableName;
        }

		private String defaultorder;
		public String DefaultOrder
		{
			get
			{
                defaultorder = BaseAddressBookTable.FieldSortCode;
				return defaultorder;
			}
		}
		
        #region public void ClearProperty() �������
        /// <summary>
		/// �������
		/// </summary>
        public void ClearProperty(BaseAddressBookEntity addressBookEntity)
		{
            addressBookEntity.FullName = String.Empty;
            addressBookEntity.CompanyName = String.Empty;
            addressBookEntity.Adress = String.Empty;
            addressBookEntity.Duty = String.Empty;
            addressBookEntity.Telephone = String.Empty;
            addressBookEntity.Mobile = String.Empty;
            addressBookEntity.Mail = String.Empty;
            addressBookEntity.Relation = String.Empty;
        }                                           
		#endregion

        #region public BaseAddressBookEntity GetFrom(DataRow myDataRow, BaseAddressBookEntity addressBookEntity) �������ж�ȡ
        /// <summary>
        /// �������ж�ȡ
        /// </summary>
        /// <param name="myDataRow">������</param>
        /// <returns>ͨѶ¼�Ļ����ṹ����</returns>
        public BaseAddressBookEntity GetFrom(DataRow myDataRow, BaseAddressBookEntity addressBookEntity)
        {
            addressBookEntity.ID = myDataRow[BaseAddressBookTable.FieldID].ToString();      // ����
            addressBookEntity.FullName = myDataRow[BaseAddressBookTable.FieldFullName].ToString();// ��ϵ��
            addressBookEntity.CompanyName = myDataRow[BaseAddressBookTable.FieldCompanyName].ToString();         // ��˾���� 
            addressBookEntity.Adress = myDataRow[BaseAddressBookTable.FieldAdress].ToString();  // ��ַ
            addressBookEntity.Duty = myDataRow[BaseAddressBookTable.FieldDuty].ToString();    // ְ��
            addressBookEntity.Telephone = myDataRow[BaseAddressBookTable.FieldTelephone].ToString();           // �绰
            addressBookEntity.Mobile = myDataRow[BaseAddressBookTable.FieldMobile].ToString();  // �ֻ�
            addressBookEntity.Mail = myDataRow[BaseAddressBookTable.FieldMail].ToString();    // �ʼ�
            addressBookEntity.Relation = myDataRow[BaseAddressBookTable.FieldRelation].ToString();// ��ϵ
            addressBookEntity.Enabled = myDataRow[BaseAddressBookTable.FieldEnabled].ToString().Equals("1"); // ��Ч��
            addressBookEntity.Description = myDataRow[BaseAddressBookTable.FieldDescription].ToString();         // ����
            addressBookEntity.SortCode = myDataRow[BaseAddressBookTable.FieldSortCode].ToString();// ��ע
            addressBookEntity.CreateUserID = myDataRow[BaseAddressBookTable.FieldCreateUserID].ToString();       // ������
            addressBookEntity.CreateDate = myDataRow[BaseAddressBookTable.FieldCreateDate].ToString();          // ����ʱ��
            addressBookEntity.ModifyUserID = myDataRow[BaseAddressBookTable.FieldModifyUserID].ToString();       // ����޸���
            addressBookEntity.ModifyDate = myDataRow[BaseAddressBookTable.FieldModifyDate].ToString();          // ����޸�ʱ��
            return addressBookEntity;
        }
        #endregion

        #region public String Add(BaseAddressBookEntity addressBookEntity) ���
        /// <summary>
		/// ���
		/// </summary>
        /// <param name="addressBookEntity">ʵ��</param>
		/// <returns>����</returns>
        public String Add(BaseAddressBookEntity addressBookEntity)
		{
			String returnValue = String.Empty;
            String sequence = BaseSequenceDao.Instance.GetSequence(this.DbHelper, BaseAddressBookTable.TableName);
            SQLBuilder mySQLBuilder = new SQLBuilder(this.DbHelper);
            int enabled = addressBookEntity.Enabled ? 1 : 0;
            mySQLBuilder.BeginInsert(BaseAddressBookTable.TableName);
            mySQLBuilder.SetValue(BaseAddressBookTable.FieldID, sequence);
            mySQLBuilder.SetValue(BaseAddressBookTable.FieldFullName, addressBookEntity.FullName);
            mySQLBuilder.SetValue(BaseAddressBookTable.FieldCompanyName, addressBookEntity.CompanyName);
            mySQLBuilder.SetValue(BaseAddressBookTable.FieldAdress, addressBookEntity.Adress);
            mySQLBuilder.SetValue(BaseAddressBookTable.FieldDuty, addressBookEntity.Duty);
            mySQLBuilder.SetValue(BaseAddressBookTable.FieldTelephone, addressBookEntity.Telephone);
            mySQLBuilder.SetValue(BaseAddressBookTable.FieldMobile, addressBookEntity.Mobile);
            mySQLBuilder.SetValue(BaseAddressBookTable.FieldMail, addressBookEntity.Mail);
            mySQLBuilder.SetValue(BaseAddressBookTable.FieldRelation, addressBookEntity.Relation);
            mySQLBuilder.SetValue(BaseAddressBookTable.FieldEnabled, enabled);
            mySQLBuilder.SetValue(BaseAddressBookTable.FieldDescription, addressBookEntity.Description);
            mySQLBuilder.SetValue(BaseAddressBookTable.FieldSortCode, sequence);
            mySQLBuilder.SetValue(BaseAddressBookTable.FieldCreateUserID, this.UserInfo.ID);
            mySQLBuilder.SetDBNow(BaseAddressBookTable.FieldCreateDate);
            returnValue = mySQLBuilder.EndInsert() > 0 ? sequence : String.Empty;
			return returnValue;
		}
		#endregion

        #region public int Update(BaseAddressBookEntity addressBookEntity) ����
        /// <summary>
		/// ����
		/// </summary>
        /// <param name="addressBookEntity">ʵ��</param>
        /// <returns>Ӱ������</returns>
        public int Update(BaseAddressBookEntity addressBookEntity)
		{
			int returnValue	= 0;
			SQLBuilder mySQLBuilder = new SQLBuilder(this.DbHelper);
            mySQLBuilder.BeginUpdate(BaseAddressBookTable.TableName);
            mySQLBuilder.SetValue(BaseAddressBookTable.FieldFullName, addressBookEntity.FullName);
            mySQLBuilder.SetValue(BaseAddressBookTable.FieldCompanyName, addressBookEntity.CompanyName);
            mySQLBuilder.SetValue(BaseAddressBookTable.FieldAdress, addressBookEntity.Adress);
            mySQLBuilder.SetValue(BaseAddressBookTable.FieldDuty, addressBookEntity.Duty);
            mySQLBuilder.SetValue(BaseAddressBookTable.FieldTelephone, addressBookEntity.Telephone);
            mySQLBuilder.SetValue(BaseAddressBookTable.FieldMobile, addressBookEntity.Mobile);
            mySQLBuilder.SetValue(BaseAddressBookTable.FieldMail, addressBookEntity.Mail);
            mySQLBuilder.SetValue(BaseAddressBookTable.FieldRelation, addressBookEntity.Relation);
            mySQLBuilder.SetValue(BaseAddressBookTable.FieldEnabled, addressBookEntity.Enabled);
            mySQLBuilder.SetValue(BaseAddressBookTable.FieldDescription, addressBookEntity.Description);
            mySQLBuilder.SetValue(BaseAddressBookTable.FieldModifyUserID, this.UserInfo.ID);
            mySQLBuilder.SetDBNow(BaseAddressBookTable.FieldModifyDate);
            mySQLBuilder.SetWhere(BaseAddressBookTable.FieldID, addressBookEntity.ID);
			returnValue = mySQLBuilder.EndUpdate();
			return returnValue;
		}
		#endregion

        #region public int SetState(String[] id, int enabled) ������Ч
        /// <summary>
        /// ������Ч
        /// </summary>
        /// <param name="id">ͨѶ¼����</param>
        /// <param name="enabled">��Ч</param>
        /// <returns>Ӱ������</returns>
        public int SetState(String[] ids, int enabled)
        {
            int returnValue = 0;
            String id = String.Empty;
            try
            {
                // ����ʼ
                this.DbHelper.BeginTransaction();
                for (int i = 0; i < ids.Length; i++)
                {
                    id = ids[i];
                    returnValue += this.SetProperty(id, BaseAddressBookTable.FieldEnabled, enabled.ToString());
                }
                this.DbHelper.CommitTransaction();
            }
            catch (Exception exception)
            {
                // ����ع�
                this.DbHelper.RollbackTransaction();
                BaseExceptionDao.Instance.LogException(this.DbHelper, this.UserInfo, exception);
                throw exception;
            }
            return returnValue;
        }
        #endregion   

        #region public DataTable GetPreviousNextID(String id) ��ô����б�
        /// <summary>
        /// ��ô����б��еģ���һ����Сһ����¼�Ĵ���
        /// </summary>
        /// <param name="id">ͨѶ¼ID</param>
        /// <returns>���ݼ�</returns>
        public DataTable GetPreviousNextID(String id)
        {
            String sqlQuery = " SELECT ID "
                            + " FROM " + BaseAddressBookTable.TableName
                            + " ORDER BY " + this.DefaultOrder;
            DataTable myDataTable = new DataTable(BaseAddressBookTable.TableName);
            this.DbHelper.Fill(myDataTable, sqlQuery);
            this.NextID = BaseSortLogic.Instance.GetNextID(myDataTable, id);
            this.PreviousID = BaseSortLogic.Instance.GetPreviousID(myDataTable, id);
            return myDataTable;
        }
        #endregion

        #region public DataTable Search(String search) ��ѯ
        /// <summary>
        /// ��ѯ
        /// </summary>
        /// <param name="search">��ѯ�ַ�</param>
        /// <returns>���ݼ�</returns>
        public DataTable Search(String search)
        {
            // 2007.02.28 ��û�в�ѯ���ݣ�Ϊ�����ٶȸ����г����ݣ������˸Ľ���
            if (search.Length == 0)
            {
                return this.GetList();
            }
            DataTable myDataTable = new DataTable(BaseAddressBookTable.TableName);
            String sqlQuery = " SELECT * "
                            + " FROM " + BaseAddressBookTable.TableName
                            + " WHERE ((" + BaseAddressBookTable.FieldFullName + " LIKE ? )"
                            + " OR (" + BaseAddressBookTable.FieldCompanyName + " LIKE ? )"
                            + " OR (" + BaseAddressBookTable.FieldAdress + " LIKE ? )"
                        + " OR (" + BaseAddressBookTable.FieldDuty + " LIKE ? )"
                            + " OR (" + BaseAddressBookTable.FieldMail + " LIKE ? )"
                            + " OR (" + BaseAddressBookTable.FieldTelephone + " LIKE ? )"
                            + " OR (" + BaseAddressBookTable.FieldMobile + " LIKE ? )"
                            + " OR (" + BaseAddressBookTable.FieldDescription + " LIKE ? )"
                            + " OR (" + BaseAddressBookTable.FieldRelation + " LIKE ? ))"
                            + " AND (" + BaseAddressBookTable.FieldCreateUserID + " = ? )"
                            + " ORDER BY " + this.DefaultOrder;      
            String[] names = new String[10];
            Object[] values = new Object[10];
            names[0] = BaseAddressBookTable.FieldFullName;
            values[0] = search;
            names[1] = BaseAddressBookTable.FieldCompanyName;
            values[1] = search;
            names[2] = BaseAddressBookTable.FieldAdress;
            values[2] = search;
            names[3] = BaseAddressBookTable.FieldDuty;
            values[3] = search;
            names[4] = BaseAddressBookTable.FieldMail;
            values[4] = search;
            names[5] = BaseAddressBookTable.FieldTelephone;
            values[5] = search;
            names[6] = BaseAddressBookTable.FieldMobile;
            values[6] = search;
            names[7] = BaseAddressBookTable.FieldDescription;
            values[7] = search;
            names[8] = BaseAddressBookTable.FieldRelation;
            values[8] = search;
            values[9] = BaseAddressBookTable.FieldCreateUserID;
            values[9] = this.UserInfo.ID;
            this.DbHelper.Fill(myDataTable, sqlQuery, this.DbHelper.MakeParameters(names, values));
            return myDataTable;
        }
        #endregion
	}
}