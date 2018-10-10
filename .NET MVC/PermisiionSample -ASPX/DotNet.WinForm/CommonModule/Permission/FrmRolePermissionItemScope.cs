//--------------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2012 , Hairihan TECH, Ltd. 
//--------------------------------------------------------------------

using System;
using System.Data;
using System.Windows.Forms;

namespace DotNet.WinForm
{
    
    using DotNet.Business;
    using DotNet.Utilities;
    

    /// <summary>
    /// FrmRolePermissionItemScope.cs
    /// ��ɫ���ɷ���Ȩ���趨
    ///		
    /// �޸ļ�¼
    ///
    ///     2008.05.25 �汾��1.0 JiRiGaLa ��ɫ��Ȩ�޹���
    ///		
    /// �汾��1.0
    ///
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2008.05.25</date>
    /// </author> 
    /// </summary>  
    public partial class FrmRolePermissionItemScope : BaseForm
    {
        private DataTable DTPermissionItem = new DataTable(BaseParameterEntity.TableName); // Ȩ�����ݱ�
        private string[] PermissionIds = null;

        /// <summary>
        /// Ȩ������
        /// </summary>
        private string PermissionItemScopeCode = "Resource.ManagePermission";
        
        // Ŀ���ɫ����
        private string TargetRoleId
        {
            set
            {
                this.ucRole.SelectedId = value;
            }
            get
            {
                return this.ucRole.SelectedId;
            }
        }

        // Ŀ���ɫ����     
        private string TargetRoleName
        {
            set
            {
                this.ucRole.SelectedFullName = value;
            }
            get
            {
                return this.ucRole.SelectedFullName;
            }
        }

        public string OldEntityId = string.Empty;       // ԭ�ȱ�ѡ�еĽڵ�����
        private string currentEntityId = string.Empty;

        private bool isUserClick = true;    // �Ƿ����û�����˸�ѡ��

        /// <summary>
        /// ��ǰѡ�еĽڵ㣬����
        /// </summary>
        public string CurrentEntityId
        {
            get
            {
                if ((this.tvPermission.SelectedNode != null) && (this.tvPermission.SelectedNode.Tag != null))
                {
                    this.currentEntityId = (this.tvPermission.SelectedNode.Tag as DataRow)[BaseModuleEntity.FieldId].ToString();
                }
                return this.currentEntityId;
            }
            set
            {
                this.currentEntityId = value;
            }
        }
        
        public FrmRolePermissionItemScope()
        {
            InitializeComponent();
        }

        public FrmRolePermissionItemScope(string currentId) : this()
        {
            this.CurrentEntityId    = currentId;
            this.OldEntityId        = currentId;
        }

        public FrmRolePermissionItemScope(string roleId, string roleName) : this()
        {
            this.TargetRoleId = roleId;
            this.TargetRoleName = roleName;
        }

        #region public override void SetControlState() ���ÿؼ�״̬
        /// <summary>
        /// ���ÿؼ�״̬
        /// </summary>
        public override void SetControlState()
        {
            // �����û��ؼ�״̬
            this.ucRole.AllowNull = false;
            this.ucRole.ShowRoleOnly = true;
            this.ucRole.PermissionItemScopeCode = this.PermissionItemScopeCode;
            // �Ǽ򻯵Ľ�ɫ����
            this.ucRole.SimpleAdmin = true;
            if (String.IsNullOrEmpty(this.TargetRoleId) || this.DTPermissionItem.Rows.Count == 0)
            {
                this.btnBatchSave.Enabled = false;
                this.btnSelectAll.Enabled = false;
                this.btnInvertSelect.Enabled = false;
            }
            else
            {
                this.btnBatchSave.Enabled = this.permissionEdit;
                this.btnSelectAll.Enabled = this.permissionEdit;
                this.btnInvertSelect.Enabled = this.permissionEdit;
            }
        }
        #endregion

        #region public override void GetPermission() ���ҳ���Ȩ��
        /// <summary>
        /// ���ҳ���Ȩ��
        /// </summary>
        public override void GetPermission()
        {
            this.permissionAccess = this.IsAuthorized("UserAdmin.Accredit");   // ����Ȩ��
            this.permissionEdit = this.IsAuthorized("UserAdmin.Accredit");       // �༭Ȩ��  
        }
        #endregion

        #region private void BindData(bool reloadTree) ����Ļ����
        /// <summary>
        /// ����Ļ����
        /// </summary>
        /// <param name="reloadTree">���¼Ӳ�������</param>
        private void BindData(bool reloadTree)
        {
            this.ucRole.SelectedId = this.TargetRoleId;
            this.ucRole.SelectedFullName = this.TargetRoleName;
            // ������
            if (reloadTree)
            {
                this.LoadTree();
            }
            if (this.tvPermission.SelectedNode == null)
            {
                if (this.tvPermission.Nodes.Count > 0)
                {
                    if (this.CurrentEntityId.Length == 0)
                    {
                        this.tvPermission.SelectedNode = this.tvPermission.Nodes[0];
                    }
                    else
                    {
                        BaseInterfaceLogic.FindTreeNode(this.tvPermission, this.CurrentEntityId);
                        if (BaseInterfaceLogic.TargetNode != null)
                        {
                            this.tvPermission.SelectedNode = BaseInterfaceLogic.TargetNode;
                            // չ����ǰѡ�нڵ�����и��ڵ�
                            BaseInterfaceLogic.ExpandTreeNode(this.tvPermission);
                        }
                    }
                    if (this.tvPermission.SelectedNode != null)
                    {
                        // ��ѡ�еĽڵ���ӣ�����չ����ʽ
                        this.tvPermission.SelectedNode.Expand();
                        this.tvPermission.SelectedNode.EnsureVisible();
                    }
                }
            }
        }
        #endregion

        #region public override void FormOnLoad() ���ش���
        /// <summary>
        /// ���ش���
        /// </summary>
        public override void FormOnLoad()
        {
            // ���ָ��Ȩ�ޱ��
            this.DTPermissionItem = this.GetPermissionItemScop(this.PermissionItemScopeCode);
            // ������Ҫֻ����Ч��ģ����ʾ����
            BaseBusinessLogic.SetFilter(this.DTPermissionItem, BasePermissionItemEntity.FieldEnabled, "1");
            // δ������ɾ�����־��
            // BaseBusinessLogic.SetFilter(this.DTPermissionItem, BasePermissionItemEntity.FieldDeletionStateCode, "0");

            this.DTPermissionItem.DefaultView.Sort = BasePermissionItemEntity.FieldSortCode;
            this.PermissionIds = DotNetService.Instance.PermissionService.GetRoleScopePermissionItemIds(UserInfo, this.TargetRoleId, this.PermissionItemScopeCode);
            // �������Ĭ��״̬��ԭ���Ĺ��״̬
            this.BindData(true);
        }
        #endregion

        private void ucRole_SelectedIndexChanged(string parentId)
        {
            this.FormOnLoad();
        }

        #region private void LoadTree() ����Ȩ����������
        /// <summary>
        /// ����Ȩ����������
        /// </summary>
        private void LoadTree()
        {
            this.isUserClick = false;
            // ��ʼ���¿ؼ�����Ļ��ˢ�£����Ч�ʡ�
            this.tvPermission.BeginUpdate();
            this.tvPermission.Nodes.Clear();
            TreeNode treeNode = new TreeNode();
            this.LoadTree(treeNode);
            this.tvPermission.EndUpdate();
            this.isUserClick = true;
        }
        #endregion

        #region private void LoadTree(TreeNode treeNode)
        /// <summary>
        /// ����Ȩ�޹���������
        /// </summary>
        /// <param name="TreeNode">��ǰ�ڵ�</param>
        private void LoadTree(TreeNode treeNode)
        {
            DataRow[] dataRows = null;
            if (treeNode.Tag == null)
            {
                dataRows = this.DTPermissionItem.Select(BasePermissionItemEntity.FieldParentId + " IS NULL OR " + BasePermissionItemEntity.FieldParentId + " = 0 ", BasePermissionItemEntity.FieldSortCode);
            }
            else
            {
                dataRows = this.DTPermissionItem.Select(BasePermissionItemEntity.FieldParentId + "=" + (treeNode.Tag as DataRow)[BaseModuleEntity.FieldId].ToString() + "", BasePermissionItemEntity.FieldSortCode);
            }
            foreach (DataRow dataRow in dataRows)
            {
                // �жϲ�Ϊ�յĵ�ǰ�ڵ���ӽڵ�
                if ((treeNode.Tag != null) && ((treeNode.Tag as DataRow)[BaseModuleEntity.FieldId].ToString().Length > 0) && (!(treeNode.Tag as DataRow)[BaseModuleEntity.FieldId].ToString().Equals(dataRow[BasePermissionItemEntity.FieldParentId].ToString())))
                {
                    continue;
                }
                // ��ǰ�ڵ���ӽڵ�, ���ظ��ڵ�
                if ((dataRow.IsNull(BasePermissionItemEntity.FieldParentId) || (dataRow[BasePermissionItemEntity.FieldParentId].ToString().Length == 0) || ((treeNode.Tag != null) && (treeNode.Tag as DataRow)[BaseModuleEntity.FieldId].ToString().Equals(dataRow[BasePermissionItemEntity.FieldParentId].ToString()))))
                {
                    TreeNode newTreeNode = new TreeNode();
                    newTreeNode.Text = dataRow[BasePermissionItemEntity.FieldFullName].ToString();
                    newTreeNode.Tag = dataRow[BasePermissionItemEntity.FieldId].ToString();
                    // �Ƿ��Ѿ������Ȩ��
                    newTreeNode.Checked = Array.IndexOf(this.PermissionIds, dataRow[BasePermissionItemEntity.FieldId].ToString()) >= 0;
                    if ((treeNode.Tag == null) || ((treeNode.Tag as DataRow)[BaseModuleEntity.FieldId].ToString().Length == 0))
                    {
                        // ���ĸ��ڵ����
                        this.tvPermission.Nodes.Add(newTreeNode);
                    }
                    else
                    {
                        // �ڵ���ӽڵ����
                        treeNode.Nodes.Add(newTreeNode);
                    }
                    // �ݹ���ñ�����
                    this.LoadTree(newTreeNode);
                }
            }
        }
        #endregion

        private void tvPermission_BeforeCheck(object sender, TreeViewCancelEventArgs e)
        {
            if (this.isUserClick)
            {
                if (!this.permissionEdit)
                {
                    e.Cancel = true;
                }
            }
        }

        private void tvPermission_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (this.isUserClick)
            {
                for (int i = 0; i < e.Node.Nodes.Count; i++)
                {
                    e.Node.Nodes[i].Checked = e.Node.Checked;
                }
            }
        }

        #region private void SetTreeNodesSelected(TreeNode treeNode, bool selected) �ݹ���������ѡ��״̬
        /// <summary>
        /// �ݹ���������ѡ��״̬
        /// </summary>
        /// <param name="TreeNode">���ڵ�</param>
        /// <param name="selected">ѡ��</param>
        private void SetTreeNodesSelected(TreeNode treeNode, bool selected)
        {
            if ((treeNode!= null && treeNode.Tag != null && (treeNode.Tag as DataRow) != null))
            {
                treeNode.Checked = selected;
                for (int i = 0; i < treeNode.Nodes.Count; i++)
                {
                    // ������еݹ����
                    this.SetTreeNodesSelected(treeNode.Nodes[i], selected);
                }
            }
        }
        #endregion

        #region private void SetTreeNodesTurnSelected(TreeNode treeNode) �ݹ��������ķ�ѡ״̬
        /// <summary>
        /// �ݹ��������ķ�ѡ״̬
        /// </summary>
        /// <param name="TreeNode">���ڵ�</param>
        private void SetTreeNodesTurnSelected(TreeNode treeNode)
        {
            if ((treeNode!= null && treeNode.Tag != null && (treeNode.Tag as DataRow) != null))
            {
                treeNode.Checked = !treeNode.Checked;
                for (int i = 0; i < treeNode.Nodes.Count; i++)
                {
                    // ������еݹ����
                    this.SetTreeNodesTurnSelected(treeNode.Nodes[i]);
                }
            }
        }
        #endregion

        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            this.isUserClick = false;
            for (int i = 0; i < this.tvPermission.Nodes.Count; i++)
            {
                this.SetTreeNodesSelected(this.tvPermission.Nodes[i], true);
            }
            this.isUserClick = true;
        }

        private void btnInvertSelect_Click(object sender, EventArgs e)
        {
            this.isUserClick = false;
            for (int i = 0; i < this.tvPermission.Nodes.Count; i++)
            {
                this.SetTreeNodesTurnSelected(this.tvPermission.Nodes[i]);
            }
            this.isUserClick = true;
        }

        /// <summary>
        /// ��Ȩ��Ȩ��
        /// </summary>
        string GrantPermissions = string.Empty;

        /// <summary>
        /// ������Ȩ��
        /// </summary>
        string RevokePermissions = string.Empty;

        private void EntityToArray(TreeNode treeNode)
        {
            if (treeNode.Tag != null)
            {
                // ��������ٶ�
                string permissionItemId = (treeNode.Tag as DataRow)[BaseModuleEntity.FieldId].ToString();
                if (treeNode.Checked)
                {
                    // ��������Ȩ��Ȩ��
                    if (Array.IndexOf(this.PermissionIds, permissionItemId) < 0)
                    {
                        this.GrantPermissions += permissionItemId + ";";
                    }
                }
                else
                {
                    // �����ǳ�����Ȩ��
                    if (Array.IndexOf(this.PermissionIds, permissionItemId) >= 0)
                    {
                        this.RevokePermissions += permissionItemId + ";";
                    }
                }
            }
            for (int i = 0; i < treeNode.Nodes.Count; i++)
            {
                // ������еݹ����
                this.EntityToArray(treeNode.Nodes[i]);
            }
        }

        private void CheckParentChecked(TreeNode treeNode)
        {
            // �ݹ���õ����е��ӽڵ� 
            for (int i = 0; i < treeNode.Nodes.Count; i++)
            {
                this.CheckParentChecked(treeNode.Nodes[i]);
            }
            // ��鸸�ڵ��ѡ��״̬
            while (treeNode.Parent != null)
            {
                if (treeNode.Checked)
                {
                    treeNode.Parent.Checked = treeNode.Checked;
                    treeNode = treeNode.Parent;
                }
                else
                {
                    break;
                }
            }
        }

        private void CheckParentChecked()
        {
            this.isUserClick = false;
            for (int i = 0; i < this.tvPermission.Nodes.Count; i++)
            {
                TreeNode treeNode = this.tvPermission.Nodes[i];
                this.CheckParentChecked(treeNode);
            }
            this.isUserClick = true;
        }

        #region private bool BatchSave() ��������
        /// <summary>
        /// ��������
        /// </summary>
        private bool BatchSave()
        {
            bool returnValue = true;
            for (int i = 0; i < this.tvPermission.Nodes.Count; i++)
            {
                this.EntityToArray(this.tvPermission.Nodes[i]);
            }
            string[] grantPermissionIds = null;
            string[] revokePermissionIds = null;
            if (this.GrantPermissions.Length > 2)
            {
                this.GrantPermissions = this.GrantPermissions.Substring(0, this.GrantPermissions.Length - 1);
                grantPermissionIds = this.GrantPermissions.Split(';');
            }
            if (this.RevokePermissions.Length > 1)
            {
                this.RevokePermissions = this.RevokePermissions.Substring(0, this.RevokePermissions.Length - 1);
                revokePermissionIds = this.RevokePermissions.Split(';');
            }
            this.GrantPermissions = string.Empty;
            this.RevokePermissions = string.Empty;
            DotNetService.Instance.PermissionService.GrantRolePermissions(UserInfo, new string[] { this.TargetRoleId }, grantPermissionIds);
            DotNetService.Instance.PermissionService.RevokeRolePermissions(UserInfo, new string[] { this.TargetRoleId }, revokePermissionIds);
            if (BaseSystemInfo.ShowInformation)
            {
                // ���³ɹ���������ʾ
                MessageBox.Show(AppMessage.MSG0012, AppMessage.MSG0000, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            return returnValue;
        }
        #endregion

        private void btnBatchSave_Click(object sender, EventArgs e)
        {
            // ������귱æ״̬��������ԭ�ȵ�״̬
            Cursor holdCursor = this.Cursor;
            this.Cursor = Cursors.WaitCursor;
            try
            {
                // ��������
                // this.CheckParentChecked();
                if (this.BatchSave())
                {
                    // �رմ���
                    this.FormOnLoad();
                }
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

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void tvPermission_MouseDown(object sender, MouseEventArgs e)
        {
            if (tvPermission.GetNodeAt(e.X, e.Y) != null)
            {
                tvPermission.SelectedNode = tvPermission.GetNodeAt(e.X, e.Y);
            }
        }
    }
}