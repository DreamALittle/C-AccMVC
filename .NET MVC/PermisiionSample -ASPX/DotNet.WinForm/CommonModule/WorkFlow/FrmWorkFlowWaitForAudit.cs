//--------------------------------------------------------------------
// All Rights Reserved ,Copyright (C) 2012 , Hairihan TECH, Ltd. .
//--------------------------------------------------------------------

using System;
using System.Data;
using System.Windows.Forms;

namespace DotNet.WinForm
{
    using DotNet.Business;
    using DotNet.Utilities;

    /// <summary>
    /// FrmMyWorkFlow.cs
    /// Ȩ�޹���-����Ȩ�޴���
    ///		
    /// �޸ļ�¼
    /// 
    ///     2010.08.05 �汾��1.1 JiRiGaLa ����������롣
    ///     2007.07.18 �汾��1.0 JiRiGaLa Gredview��ʾ��
    /// 
    /// �汾��1.1
    ///
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2010.08.05</date>
    /// </author> 
    /// </summary> 
    public partial class FrmWorkFlowWaitForAudit : BaseForm
    {
        DataTable DTWorkFlowCurrent = null;

        public FrmWorkFlowWaitForAudit()
        {
            InitializeComponent();
        }

        #region private void GetList() ��ȡ�б�
        /// <summary>
        /// ��ȡ�б�
        /// </summary>
        private void GetList()
        {
            this.DTWorkFlowCurrent = DotNetService.Instance.WorkFlowCurrentService.GetWaitForAudit(this.UserInfo);
        }
        #endregion

        #region private void BindData() ����Ļ����
        /// <summary>
        /// ����Ļ����
        /// </summary>
        private void BindData()
        {
            this.grdWorkFlowCurrent.AutoGenerateColumns = false;
            foreach (DataRow dataRow in DTWorkFlowCurrent.Rows)
            {
                dataRow[BaseWorkFlowHistoryEntity.FieldAuditStatus] = BaseBusinessLogic.GetAuditStatus(dataRow[BaseWorkFlowHistoryEntity.FieldAuditStatus].ToString());
            }
            this.DTWorkFlowCurrent.DefaultView.Sort = BaseWorkFlowCurrentEntity.FieldSortCode;
            this.grdWorkFlowCurrent.DataSource = this.DTWorkFlowCurrent.DefaultView;
            this.grdWorkFlowCurrent.Refresh();
            this.SetControlState();
            if (this.grdWorkFlowCurrent.Rows.Count == 0)
            {
                this.ucWorkFlow.WorkFlowIds = null;
            }
            // ��ʼ���ؼ�״̬
            this.ucWorkFlow.SetControlState();
        }
        #endregion

        #region public override void FormOnLoad() ���ش���
        /// <summary>
        /// ���ش���
        /// </summary>
        public override void FormOnLoad()
        {
            // ��������
            this.GetItemDetails();
            // �����ʾ��ŵĴ�����
            this.DataGridViewOnLoad(grdWorkFlowCurrent);
            // ��ȡ�б�
            this.GetList();
            // ����Ļ����
            this.BindData();
            // �����������Ƿ����������Ȩ�ޣ�
            //this.ucFreeWorkFlow.PermissionAuditing = this.UserInfo.IsAdministrator;           
        }
        #endregion

        #region private void GetItemDetails() ������������
        /// <summary>
        /// ������������
        /// </summary>
        private void GetItemDetails()
        {
            // �󶨵��ݷ���
            DataTable dataTable = DotNetService.Instance.ItemDetailsService.GetDataTable(this.UserInfo, "ItemsWorkFlowCategories");
            DataRow dataRow = dataTable.NewRow();
            dataTable.Rows.InsertAt(dataRow, 0);
            this.cmbCategory.DataSource = dataTable;
            this.cmbCategory.DisplayMember = BaseItemDetailsEntity.FieldItemName;
            this.cmbCategory.ValueMember = BaseItemDetailsEntity.FieldItemValue;
            GetItemsDetail(false );
        }

        /// <summary>
        /// �󶨵����б�
        /// <param name="linkage">�Ƿ���������</param>
        /// </summary>
        private void GetItemsDetail(bool linkage)
        {
            DataTable dataTable = new DataTable();
            if (linkage)
            {
                string billCategory = string.Empty;
                if (this.cmbCategory.SelectedValue != null)
                {
                    billCategory = this.cmbCategory.SelectedValue.ToString();
                }
                // ��ȡ��������
                dataTable = DotNetService.Instance.WorkFlowProcessAdminService.Search(this.UserInfo, string.Empty, billCategory, string.Empty, null, false);
            }
            else
            {
                // ��ȡȫ������
                dataTable = DotNetService.Instance.WorkFlowProcessAdminService.GetBillTemplateDT(this.UserInfo);
            }
            DataRow dataRow = dataTable.NewRow();
            dataTable.Rows.InsertAt(dataRow, 0);
            this.cmbBill.DataSource = dataTable;
            this.cmbBill.DisplayMember = BaseWorkFlowBillTemplateEntity.FieldTitle;
            this.cmbBill.ValueMember = BaseWorkFlowBillTemplateEntity.FieldCode;
        }
        #endregion

        /// <summary>
        /// ���ÿؼ�״̬
        /// </summary>
        public override void SetControlState()
        {
            // this.btnAuditDetail.Enabled = this.grdAuditDetail.RowCount > 0;
            this.btnExport.Enabled = this.grdWorkFlowCurrent.RowCount > 0;
        }

        private void grdAuditDetail_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // this.btnAuditDetail.PerformClick();
        }

        private void btnAuditDetail_Click(object sender, EventArgs e)
        {
            DataRow dataRow = BaseInterfaceLogic.GetDataGridViewEntity(this.grdWorkFlowCurrent);
            if (dataRow != null)
            {
                BaseWorkFlowCurrentEntity workFlowCurrentEntity = new BaseWorkFlowCurrentEntity(dataRow);
                FrmWorkFlowAuditDetail frmWorkFlowAuditDetail = new FrmWorkFlowAuditDetail(workFlowCurrentEntity.CategoryCode, workFlowCurrentEntity.ObjectId);
                frmWorkFlowAuditDetail.Show();
            }
        }

        #region private string[] GetSelecteIds() ����ѱ�ѡ��Ĳ�����������
        /// <summary>
        /// ����ѱ�ѡ�����������
        /// </summary>
        /// <returns>��������</returns>
        private string[] GetSelecteIds()
        {
            return BaseInterfaceLogic.GetSelecteIds(this.grdWorkFlowCurrent, BaseWorkFlowCurrentEntity.FieldId, "colSelected", true);
        }
        #endregion

        private bool GetIds()
        {
            bool returnValue = false;
            if (BaseInterfaceLogic.CheckInputSelectAnyOne(this.grdWorkFlowCurrent, "colSelected"))
            {
                // ������귱æ״̬��������ԭ�ȵ�״̬
                Cursor holdCursor = this.Cursor;
                this.Cursor = Cursors.WaitCursor;
                try
                {
                    this.ucWorkFlow.WorkFlowIds = this.GetSelecteIds();
                    returnValue = true;
                }
                catch (Exception ex)
                {
                    this.ProcessException(ex);
                }
                finally
                {
                    // �������Ĭ��״̬��ԭ���Ĺ��״̬
                    this.Cursor = holdCursor;
                }
            }
            return returnValue;
        }

        private bool ucWorkFlowUser_BeforBtnAuditDetailClick(string categoryId, string objectId)
        {
            DataRow dataRow = BaseInterfaceLogic.GetDataGridViewEntity(this.grdWorkFlowCurrent);
            if (dataRow != null)
            {
                BaseWorkFlowCurrentEntity workFlowCurrentEntity = new BaseWorkFlowCurrentEntity(dataRow);
                FrmWorkFlowAuditDetail frmWorkFlowAuditDetail = new FrmWorkFlowAuditDetail(workFlowCurrentEntity.CategoryCode, workFlowCurrentEntity.ObjectId);
                frmWorkFlowAuditDetail.Show();
            }
            return false;
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            // ����Excel
            //string exportFileName = this.Text + ".csv";
            string exportFileName = this.Text + ".xls";
            this.ExportExcel(this.grdWorkFlowCurrent, @"\Modules\Export\", exportFileName);
        }

        /// <summary>
        /// �������ϸʱ����
        /// </summary>
        /// <returns></returns>
        private bool ucWorkFlow_BeforAuditDetailClick()
        {
            // ��ȡ������
            return this.GetIds();
        }

        /// <summary>
        /// �������ͨ��ʱ����
        /// </summary>
        /// <returns></returns>
        private bool ucWorkFlow_BeforAuditPassClick()
        {
            // ��ȡ������
            return this.GetIds();
        }

        /// <summary>
        /// �������ʱ����
        /// </summary>
        /// <returns></returns>
        private bool ucWorkFlow_BeforAuditQuashClick()
        {
            // ��ȡ������
            return this.GetIds();
        }

        /// <summary>
        /// ����˻�ʱ����
        /// </summary>
        /// <returns></returns>
        private bool ucWorkFlow_BeforAuditRejectClick()
        {
            // ��ȡ������
            return this.GetIds();
        }

        /// <summary>
        /// ���ת��ʱ����
        /// </summary>
        /// <returns></returns>
        private bool ucWorkFlow_BeforTransmitClick()
        {
            // ��ȡ������
            return this.GetIds();
        }

        /// <summary>
        /// ��������ϸ����
        /// </summary>
        /// <returns></returns>
        private bool ucWorkFlow_AfterAuditDetailClick()
        {
            // ˢ��
            this.FormOnLoad();
            return true;
        }

        /// <summary>
        /// �������ͨ������
        /// </summary>
        /// <returns></returns>
        private bool ucWorkFlow_AfterAuditPassClick()
        {
            // ˢ��
            this.FormOnLoad();
            return true;
        }

        /// <summary>
        /// ���������������
        /// </summary>
        /// <returns></returns>
        private bool ucWorkFlow_AfterAuditQuashClick()
        {
            // ˢ��
            this.FormOnLoad();
            return true;
        }

        /// <summary>
        /// ��������˻غ���
        /// </summary>
        /// <returns></returns>
        private bool ucWorkFlow_AfterAuditRejectClick()
        {
            // ˢ��
            this.FormOnLoad();
            return true;
        }

        /// <summary>
        /// �������ת������
        /// </summary>
        /// <returns></returns>
        private bool ucWorkFlow_AfterTransmitClick()
        {
            // ˢ��
            this.FormOnLoad();
            return true;
        }

        private void grdWorkFlowCurrent_SelectionChanged(object sender, EventArgs e)
        {
            if (this.grdWorkFlowCurrent.Rows.Count > 0)
            {
                DataGridViewRow dgvRow = this.grdWorkFlowCurrent.Rows[this.grdWorkFlowCurrent.CurrentRow.Index];
                DataRowView dataRowView = dgvRow.DataBoundItem as DataRowView;
                // ��ȡ�������������ð�ť״̬
                this.ucWorkFlow.WorkFlowIds = new string[] { dataRowView.Row[BaseWorkFlowCurrentEntity.FieldId].ToString() };
            }
            else
            {
                this.ucWorkFlow.WorkFlowIds = null;
            }
            // ����״̬
            this.ucWorkFlow.SetControlState();
        }

        private void cmbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.FormLoaded)
            {
                this.GetItemsDetail(true);
                // ��ѯ�б�
                SearchList();
            }
        }

        private void cmbBill_SelectedIndexChanged(object sender, EventArgs e)
        {
            // ��ѯ�б�
            SearchList();
        }

        /// <summary>
        /// ��ȡ�б�
        /// </summary>
        private void SearchList()
        {
            string categoryCode = string.Empty;
            string categoryFullName = string.Empty;
            if (this.cmbCategory.SelectedValue != null)
            {
                categoryCode = this.cmbCategory.SelectedValue.ToString();
            }
            if (this.cmbBill.SelectedValue != null)
            {
                categoryFullName = this.cmbBill.Text;
            }
            // ��ȡ����
            this.DTWorkFlowCurrent = DotNetService.Instance.WorkFlowCurrentService.GetWaitForAudit(this.UserInfo, this.UserInfo.Id, categoryCode, categoryFullName, this.txtSearch.Text);
            // ������
            this.BindData();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            // ��ѯ�б�
            SearchList();
        }

        private void SetRowFilter()
        {
            string search = this.txtSearch.Text.Trim();
            if (String.IsNullOrEmpty(search))
            {
                this.DTWorkFlowCurrent.DefaultView.RowFilter = string.Empty;
            }
            else
            {
                if (this.DTWorkFlowCurrent.Columns.Count > 1)
                {
                    string rowFilter = string.Empty;
                    search = StringUtil.GetSearchString(search);
                    rowFilter = StringUtil.GetLike(BaseWorkFlowCurrentEntity.FieldCategoryFullName, search)
                              + " OR " + StringUtil.GetLike(BaseWorkFlowCurrentEntity.FieldObjectFullName, search)
                              + " OR " + StringUtil.GetLike(BaseWorkFlowCurrentEntity.FieldToDepartmentName, search)
                              + " OR " + StringUtil.GetLike(BaseWorkFlowCurrentEntity.FieldToRoleRealName, search)
                              + " OR " + StringUtil.GetLike(BaseWorkFlowCurrentEntity.FieldToUserRealName, search)
                              + " OR " + StringUtil.GetLike(BaseWorkFlowCurrentEntity.FieldAuditIdea, search)
                              + " OR " + StringUtil.GetLike(BaseWorkFlowCurrentEntity.FieldAuditUserCode, search);

                    this.DTWorkFlowCurrent.DefaultView.RowFilter = rowFilter;
                }
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            this.SetRowFilter();
        }
    }
}