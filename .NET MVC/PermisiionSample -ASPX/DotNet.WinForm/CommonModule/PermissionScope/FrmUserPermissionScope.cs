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
    /// FrmUserPermissionScope.cs
    /// �û�Ȩ��������
    ///		
    /// �޸ļ�¼
    /// 
    ///     2010.07.30 �汾��3.0 JiRiGaLa Ȩ�޷�Χ���ô���������
    ///     2007.08.23 �汾��2.0 JiRiGaLa ����������
    ///     2007.08.22 �汾��1.0 JiRiGaLa ��ɫȨ����
    ///		
    /// �汾��2.0
    ///
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2007.08.23</date>
    /// </author> 
    /// </summary>  
    public partial class FrmUserPermissionScope : BaseForm
    {
        /// <summary>
        /// ��ʾ��ʾ��Ϣ
        /// </summary>
        public bool ShowInformation = true;

        private DataTable DTOrganize = new DataTable(BaseOrganizeEntity.TableName);   // ��֯������
        private DataTable DTRole = new DataTable(BaseRoleEntity.TableName);       // ��ɫ��
        private DataTable DTUser = new DataTable(BaseUserEntity.TableName);      // �û���

        private string[] OrganizeIds = null;
        private string[] RoleIds = null;
        private string[] UserIds = null;

        /// <summary>
        /// �Ƿ����û�����˸�ѡ��
        /// </summary>
        private bool isUserClick = true;

        public string permissionItemScopeCode = "Resource.ManagePermission";

        /// <summary>
        /// Ȩ������
        /// </summary>
        public string PermissionItemScopeCode
        {
            set
            {
                permissionItemScopeCode = value;
                BasePermissionItemEntity permissionItemEntity = DotNetService.Instance.PermissionItemService.GetEntityByCode(this.UserInfo, permissionItemScopeCode);
                this.ucPermissionScope.SelectedId = permissionItemEntity.Id.ToString();
            }
            get
            {
                return permissionItemScopeCode;
            }
        }


        // Ȩ������
        private string TargetPermissionId
        {
            set
            {
                this.ucPermissionScope.SelectedId = value;
            }
            get
            {
                return this.ucPermissionScope.SelectedId;
            }
        }

        // Ȩ������     
        private string TargetPermissionFullName
        {
            set
            {
                this.ucPermissionScope.SelectedFullName = value;
            }
            get
            {
                return this.ucPermissionScope.SelectedFullName;
            }
        }

        // �û�����
        private string TargetUserId
        {
            set
            {
                this.ucUser.SelectedId = value;
            }
            get
            {
                return this.ucUser.SelectedId;
            }
        }

        // �û�����     
        private string TargetUserFullName
        {
            set
            {
                this.ucUser.SelectedFullName = value;
            }
            get
            {
                return this.ucUser.SelectedFullName;
            }
        }

        private string currentEntityId = string.Empty;
        /// <summary>
        /// ��ǰѡ�еĽڵ㣬����
        /// </summary>
        public string CurrentEntityId
        {
            get
            {
                if ((this.tvOrganize.SelectedNode != null) && (this.tvOrganize.SelectedNode.Tag != null))
                {
                    this.currentEntityId = (this.tvOrganize.SelectedNode.Tag as DataRow)[BaseModuleEntity.FieldId].ToString();
                }
                return this.currentEntityId;
            }
            set
            {
                this.currentEntityId = value;
            }
        }


        public FrmUserPermissionScope()
        {
            InitializeComponent();
        }

        public FrmUserPermissionScope(string targetUserId)
            : this()
        {
            this.TargetUserId = targetUserId;
        }

        public FrmUserPermissionScope(string targetUserId, string permissionItemScopeCode)
            : this(targetUserId)
        {
            this.PermissionItemScopeCode = permissionItemScopeCode;
        }

        #region public override void GetPermission() ���ҳ���Ȩ��
        /// <summary>
        /// ���ҳ���Ȩ��
        /// </summary>
        public override void GetPermission()
        {
            this.permissionAccess = this.IsAuthorized("UserAdmin.Accredit");   // ����Ȩ��
            this.permissionEdit = this.IsAuthorized("UserAdmin.Accredit");     // �༭Ȩ��
        }
        #endregion

        #region public override void SetControlState() ���ÿؼ�״̬
        /// <summary>
        /// ���ÿؼ�״̬
        /// </summary>
        public override void SetControlState()
        {
            // �Ƿ����ѡ���
            this.ucPermissionScope.AllowNull = false;
            this.ucUser.AllowNull = false;
            // ���༭Ȩ��
            this.btnBatchSave.Enabled = this.permissionEdit;
            this.btnSelectAll.Enabled = this.permissionEdit;
            this.btnInvertSelect.Enabled = this.permissionEdit;
        }
        #endregion

        private void chkInnerOrganize_CheckedChanged(object sender, EventArgs e)
        {
            this.OnLoad(e);
        }

        #region public override void FormOnLoad() ���ش���
        /// <summary>
        /// ���ش���
        /// </summary>
        public override void FormOnLoad()
        {
            // ��ȡ��֯��������
            this.DTOrganize = this.GetOrganizeScope(this.PermissionItemScopeCode, this.chkInnerOrganize.Checked);
            this.DTRole = this.GetRoleScope(this.PermissionItemScopeCode);
            this.DTUser = this.GetUserScope(this.PermissionItemScopeCode);
            // Ȩ��������
            this.OrganizeIds = DotNetService.Instance.PermissionService.GetUserScopeOrganizeIds(UserInfo, this.TargetUserId, this.PermissionItemScopeCode);
            this.RoleIds = DotNetService.Instance.PermissionService.GetUserScopeRoleIds(UserInfo, this.TargetUserId, this.PermissionItemScopeCode);
            this.UserIds = DotNetService.Instance.PermissionService.GetUserScopeUserIds(UserInfo, this.TargetUserId, this.PermissionItemScopeCode);
            // ��ʾ��ǰѡ�е�Ȩ�޷�Χѡ��
            this.ShowPermissionScope(this.OrganizeIds);
            // ������
            this.BindData(true);
        }
        #endregion

        #region private void BindData(bool reloadTree) ����Ļ����
        /// <summary>
        /// ����Ļ����
        /// </summary>
        /// <param name="reloadTree">���¼Ӳ�������</param>
        private void BindData(bool reloadTree)
        {
            // ������
            if (reloadTree)
            {
                // ���� ParentId �ֶε�ֵ�Ƿ��� ID�ֶ� ��
                BaseInterfaceLogic.CheckTreeParentId(this.DTOrganize, BaseOrganizeEntity.FieldId, BaseOrganizeEntity.FieldParentId);
                this.LoadTree();
            }
            if (this.tvOrganize.SelectedNode == null)
            {
                if (this.tvOrganize.Nodes.Count > 0)
                {
                    if (this.CurrentEntityId.Length == 0)
                    {
                        this.tvOrganize.SelectedNode = this.tvOrganize.Nodes[0];
                    }
                    else
                    {
                        BaseInterfaceLogic.FindTreeNode(this.tvOrganize, this.CurrentEntityId);
                        if (BaseInterfaceLogic.TargetNode != null)
                        {
                            this.tvOrganize.SelectedNode = BaseInterfaceLogic.TargetNode;
                            // չ����ǰѡ�нڵ�����и��ڵ�
                            BaseInterfaceLogic.ExpandTreeNode(this.tvOrganize);
                        }
                    }
                    if (this.tvOrganize.SelectedNode != null)
                    {
                        // ��ѡ�еĽڵ���ӣ�����չ����ʽ
                        this.tvOrganize.SelectedNode.Expand();
                        this.tvOrganize.SelectedNode.EnsureVisible();
                    }
                }
            }
        }
        #endregion

        #region private void LoadTree() ������
        /// <summary>
        /// ������
        /// </summary>
        private void LoadTree()
        {
            // ��ʼ���¿ؼ�����Ļ��ˢ�£����Ч�ʡ�
            this.tvOrganize.BeginUpdate();
            this.tvOrganize.Nodes.Clear();
            TreeNode treeNode = new TreeNode();
            this.LoadTreeOrganize(treeNode);
            this.tvOrganize.EndUpdate();
        }
        #endregion

        #region private void LoadTreeOrganize(TreeNode treeNode)
        /// <summary>
        /// ������֯������������
        /// </summary>
        /// <param name="TreeNode">��ǰ�ڵ�</param>
        private void LoadTreeOrganize(TreeNode treeNode)
        {
            DataRow[] dataRows = null;
            if (treeNode.Tag == null)
            {
                dataRows = this.DTOrganize.Select(BaseOrganizeEntity.FieldParentId + " IS NULL OR " + BaseOrganizeEntity.FieldParentId + " = 0 ", BaseOrganizeEntity.FieldSortCode);
            }
            else
            {
                // string id = (treeNode.Tag as DataRow)[BaseOrganizeEntity.FieldId].ToString();
                string id = treeNode.Tag.ToString();
                dataRows = this.DTOrganize.Select(BaseOrganizeEntity.FieldParentId + "= " + id, BaseOrganizeEntity.FieldSortCode);
            }
            foreach (DataRow dataRow in dataRows)
            {
                // �жϲ�Ϊ�յĵ�ǰ�ڵ���ӽڵ�
                if ((treeNode.Tag != null) && ((treeNode.Tag.ToString().Length > 0) && (!(treeNode.Tag.ToString().Equals(dataRow[BaseOrganizeEntity.FieldParentId].ToString())))))
                {
                    continue;
                }

                // ��ǰ�ڵ���ӽڵ�, ���ظ��ڵ�
                if (dataRow.IsNull(BaseOrganizeEntity.FieldParentId) 
                    || (dataRow[BaseOrganizeEntity.FieldParentId].ToString().Length == 0) 
                    || (dataRow[BaseOrganizeEntity.FieldParentId].ToString().Equals(BaseSystemInfo.RootMenuCode)) 
                    || ((treeNode.Tag != null) && (treeNode.Tag.ToString().Equals(dataRow[BaseModuleEntity.FieldParentId].ToString()))))
                {
                    TreeNode newTreeNode = new TreeNode();
                    newTreeNode.Text = dataRow[BaseOrganizeEntity.FieldFullName].ToString();
                    newTreeNode.Tag = dataRow[BaseOrganizeEntity.FieldId].ToString();
                    newTreeNode.Checked = Array.IndexOf(this.OrganizeIds, dataRow[BasePermissionItemEntity.FieldId].ToString()) >= 0;
                    newTreeNode.ImageIndex = 0;
                    newTreeNode.SelectedImageIndex = 1;

                    // д�������Ϣ
                    #if (DEBUG)
                        newTreeNode.ToolTipText = dataRow[BaseOrganizeEntity.FieldId].ToString();
                    #endif

                    if ((treeNode.Tag == null) || (treeNode.Tag.ToString().Length == 0))
                    {
                        // ���ĸ��ڵ����
                        this.tvOrganize.Nodes.Add(newTreeNode);
                    }
                    else
                    {
                        // �ڵ���ӽڵ����
                        treeNode.Nodes.Add(newTreeNode);
                    }

                    // �ݹ���ñ�����
                    this.LoadTreeOrganize(newTreeNode);
                }
            }

            // ��ӽ�ɫ
            if ((treeNode.Tag != null) && (treeNode.Tag.ToString().Length > 0))
            {
                // ��ӽ�ɫ
                dataRows = this.DTRole.Select(BaseRoleEntity.FieldSystemId + "=" + treeNode.Tag.ToString() + "");
                foreach (DataRow roleDataRow in dataRows)
                {
                    TreeNode roleTreeNode = new TreeNode();
                    roleTreeNode.Text = roleDataRow[BaseRoleEntity.FieldRealName].ToString();
                    roleTreeNode.Tag = roleDataRow[BaseRoleEntity.FieldId].ToString();
                    roleTreeNode.Checked = Array.IndexOf(this.RoleIds, roleDataRow[BaseRoleEntity.FieldId].ToString()) >= 0;
                    roleTreeNode.ImageIndex = 22;
                    roleTreeNode.SelectedImageIndex = 22;

                    // д�������Ϣ
                    #if (DEBUG)
                        roleTreeNode.ToolTipText = roleDataRow[BaseRoleEntity.FieldId].ToString();
                    #endif

                    treeNode.Nodes.Add(roleTreeNode);
                }

                // ���Ա��
                dataRows = this.DTUser.Select(BaseUserEntity.FieldDepartmentId + "='" + treeNode.Tag.ToString() + "'");
                foreach (DataRow userDataRow in dataRows)
                {
                    TreeNode userTreeNode = new TreeNode();
                    userTreeNode.Text = userDataRow[BaseUserEntity.FieldRealName].ToString();
                    userTreeNode.Tag = userDataRow[BaseUserEntity.FieldId].ToString();
                    userTreeNode.Checked = Array.IndexOf(this.UserIds, userDataRow[BaseUserEntity.FieldId].ToString()) >= 0;
                    userTreeNode.ImageIndex = 16;
                    userTreeNode.SelectedImageIndex = 16;

                    // д�������Ϣ
                    #if (DEBUG)
                        userTreeNode.ToolTipText = userDataRow[BaseUserEntity.FieldId].ToString();
                    #endif

                    treeNode.Nodes.Add(userTreeNode);
                }
            }
        }
        #endregion

        private void tvOrganize_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (this.isUserClick)
            {
                for (int i = 0; i < e.Node.Nodes.Count; i++)
                {
                    // ֻ������ѡ���ϾͿ����ˣ��������Ч��
                    if (e.Node.Nodes[i].ImageIndex <= 1)
                    {
                        e.Node.Nodes[i].Checked = e.Node.Checked;
                    }
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
            if ((treeNode != null && treeNode.Tag != null && (treeNode.Tag as DataRow) != null))
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
            if ((treeNode != null && treeNode.Tag != null && (treeNode.Tag as DataRow) != null))
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
            for (int i = 0; i < this.tvOrganize.Nodes.Count; i++)
            {
                this.SetTreeNodesSelected(this.tvOrganize.Nodes[i], true);
            }
            this.isUserClick = true;
        }

        private void btnInvertSelect_Click(object sender, EventArgs e)
        {
            this.isUserClick = false;
            for (int i = 0; i < this.tvOrganize.Nodes.Count; i++)
            {
                this.SetTreeNodesTurnSelected(this.tvOrganize.Nodes[i]);
            }
            this.isUserClick = true;
        }

        /// <summary>
        /// ��Ȩ�Ĳ���Ȩ����
        /// </summary>
        string GrantOrganizes = string.Empty;

        /// <summary>
        /// �����Ĳ���Ȩ����
        /// </summary>
        string RevokeOrganizes = string.Empty;

        string GrantRoles = string.Empty;
        string RevokeRoles = string.Empty;

        string GrantUsers = string.Empty;
        string RevokeUsers = string.Empty;

        private void EntityToArray(TreeNode treeNode)
        {
            if (treeNode.Tag != null)
            {
                // ���ǲ�������
                if (treeNode.ImageIndex == 0)
                {
                    // ��������ٶ�
                    string organizeId = treeNode.Tag.ToString();
                    if (treeNode.Checked)
                    {
                        // ��������Ȩ��Ȩ��
                        if (Array.IndexOf(this.OrganizeIds, organizeId) < 0)
                        {
                            this.GrantOrganizes += organizeId + ";";
                        }
                    }
                    else
                    {
                        // �����ǳ�����Ȩ��
                        if (Array.IndexOf(this.OrganizeIds, organizeId) >= 0)
                        {
                            this.RevokeOrganizes += organizeId + ";";
                        }
                    }
                }
                // ���ǽ�ɫ����
                else if (treeNode.ImageIndex == 22)
                {
                    // ��������ٶ�
                    string roleId = treeNode.Tag.ToString();
                    if (treeNode.Checked)
                    {
                        // ��������Ȩ��Ȩ��
                        if (Array.IndexOf(this.RoleIds, roleId) < 0)
                        {
                            this.GrantRoles += roleId + ";";
                        }
                    }
                    else
                    {
                        // �����ǳ�����Ȩ��
                        if (Array.IndexOf(this.RoleIds, roleId) >= 0)
                        {
                            this.RevokeRoles += roleId + ";";
                        }
                    }
                }
                // �����û�����
                else if (treeNode.ImageIndex == 16)
                {
                    // ��������ٶ�
                    string userId = treeNode.Tag.ToString();
                    if (treeNode.Checked)
                    {
                        // ��������Ȩ��Ȩ��
                        if (Array.IndexOf(this.UserIds, userId) < 0)
                        {
                            this.GrantUsers += userId + ";";
                        }
                    }
                    else
                    {
                        // �����ǳ�����Ȩ��
                        if (Array.IndexOf(this.UserIds, userId) >= 0)
                        {
                            this.RevokeUsers += userId + ";";
                        }
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
            for (int i = 0; i < this.tvOrganize.Nodes.Count; i++)
            {
                TreeNode treeNode = this.tvOrganize.Nodes[i];
                this.CheckParentChecked(treeNode);
            }
        }

        /// <summary>
        /// ��ʾ��ǰѡ�е�Ȩ�޷�Χѡ��
        /// </summary>
        /// <param name="organizeIds">��֯����Ȩ�޷�Χ</param>
        private void ShowPermissionScope(string[] organizeIds)
        {
            // ��С�����˳�������ʾ����ֹ������
            this.rdbNone.Checked = StringUtil.Exists(organizeIds, ((int)PermissionScope.None).ToString());
            this.rdbDetail.Checked = StringUtil.Exists(organizeIds, ((int)PermissionScope.Detail).ToString());
            this.rdbUser.Checked = StringUtil.Exists(organizeIds, ((int)PermissionScope.User).ToString());
            this.rdbUserWorkgroup.Checked = StringUtil.Exists(organizeIds, ((int)PermissionScope.UserWorkgroup).ToString());
            this.rdbUserDepartment.Checked = StringUtil.Exists(organizeIds, ((int)PermissionScope.UserDepartment).ToString());
            this.rdbUserSubCompany.Checked = StringUtil.Exists(organizeIds, ((int)PermissionScope.UserSubCompany).ToString());
            this.rdbUserCompany.Checked = StringUtil.Exists(organizeIds, ((int)PermissionScope.UserCompany).ToString());
            this.rdbAll.Checked = StringUtil.Exists(organizeIds, ((int)PermissionScope.All).ToString());
        }

        /// <summary>
        /// ��õ�ǰѡ�е�Ȩ�޷�Χѡ��
        /// </summary>
        /// <param name="revokePermissionItemScope">���Ƴ���Ȩ��</param>
        /// <returns>ѡ��Ȩ�޷�Χ</returns>
        private PermissionScope GetPermissionScope(out string revokePermissionItemScope)
        {
            PermissionScope returnValue = PermissionScope.None;
            revokePermissionItemScope = string.Empty;

            if (this.rdbAll.Checked)
            {
                returnValue = PermissionScope.All;
            }
            else
            {
                revokePermissionItemScope += ((int)PermissionScope.All).ToString() + ";";
            }

            if (this.rdbUserCompany.Checked)
            {
                returnValue = PermissionScope.UserCompany;
            }
            else
            {
                revokePermissionItemScope += ((int)PermissionScope.UserCompany).ToString() + ";";
            }

            if (this.rdbUserSubCompany.Checked)
            {
                returnValue = PermissionScope.UserSubCompany;
            }
            else
            {
                revokePermissionItemScope += ((int)PermissionScope.UserSubCompany).ToString() + ";";
            }

            if (this.rdbUserDepartment.Checked)
            {
                returnValue = PermissionScope.UserDepartment;
            }
            else
            {
                revokePermissionItemScope += ((int)PermissionScope.UserDepartment).ToString() + ";";
            }

            if (this.rdbUserWorkgroup.Checked)
            {
                returnValue = PermissionScope.UserWorkgroup;
            }
            else
            {
                revokePermissionItemScope += ((int)PermissionScope.UserWorkgroup).ToString() + ";";
            }

            if (this.rdbUser.Checked)
            {
                returnValue = PermissionScope.User;
            }
            else
            {
                revokePermissionItemScope += ((int)PermissionScope.User).ToString() + ";";
            }

            if (this.rdbDetail.Checked)
            {
                returnValue = PermissionScope.Detail;
            }
            else
            {
                revokePermissionItemScope += ((int)PermissionScope.Detail).ToString() + ";";
            }

            if (this.rdbNone.Checked)
            {
                returnValue = PermissionScope.None;
            }
            else
            {
                revokePermissionItemScope += ((int)PermissionScope.None).ToString() + ";";
            }

            return returnValue;
        }

        #region private bool BatchSave() ��������
        /// <summary>
        /// ��������
        /// </summary>
        private bool BatchSave()
        {
            bool returnValue = true;

            this.GrantOrganizes = string.Empty;
            this.RevokeOrganizes = string.Empty;

            this.GrantRoles = string.Empty;
            this.RevokeRoles = string.Empty;

            this.GrantUsers = string.Empty;
            this.RevokeUsers = string.Empty;

            // ���ո��� 20110322- ���ﲻӦ������Ϊ 0 �ģ�
            // nick
            // this.OrganizeIds = new string[0]; 
            for (int i = 0; i < this.tvOrganize.Nodes.Count; i++)
            {
                this.EntityToArray(this.tvOrganize.Nodes[i]);
            }

            // Ա���Ĳ���Ȩ����
            string[] grantOrganizeIds = null;
            string[] revokeOrganizeIds = null;

            string revokePermissionScope = string.Empty;
            // ��ǰѡ�еģ������Ȩ�޷�Χ��Ҳ������֯������������ʽ��������
            if (String.IsNullOrEmpty(this.GrantOrganizes))
                this.GrantOrganizes += ((int)this.GetPermissionScope(out revokePermissionScope)).ToString() + ";";

            if (this.GrantOrganizes.Length > 1)
            {
                // ����������֯�������д���
                this.GrantOrganizes = this.GrantOrganizes.Substring(0, this.GrantOrganizes.Length - 1);
                if (this.GrantOrganizes != "0")
                {
                    grantOrganizeIds = this.GrantOrganizes.Split(';');
                }
            }

            // �����Ǳ�ѡ�еı��Ƴ��ģ������Ȩ�޷�Χ��Ҳ������֯������������ʽ��������
            this.RevokeOrganizes += revokePermissionScope;
            if (this.RevokeOrganizes.Length > 1)
            {
                this.RevokeOrganizes = this.RevokeOrganizes.Substring(0, this.RevokeOrganizes.Length - 1);
                revokeOrganizeIds = this.RevokeOrganizes.Split(';');
            }

            // Ա���Ľ�ɫȨ����
            string[] grantRoleIds = null;
            string[] revokeRoleIds = null;
            if (this.GrantRoles.Length > 1)
            {
                this.GrantRoles = this.GrantRoles.Substring(0, this.GrantRoles.Length - 1);
                grantRoleIds = this.GrantRoles.Split(';');
            }
            if (this.RevokeRoles.Length > 1)
            {
                this.RevokeRoles = this.RevokeRoles.Substring(0, this.RevokeRoles.Length - 1);
                revokeRoleIds = this.RevokeRoles.Split(';');
            }

            // Ա����Ա��Ȩ����
            string[] grantUserIds = null;
            string[] revokeUserIds = null;
            if (this.GrantUsers.Length > 1)
            {
                this.GrantUsers = this.GrantUsers.Substring(0, this.GrantUsers.Length - 1);
                grantUserIds = this.GrantUsers.Split(';');
            }
            if (this.RevokeUsers.Length > 1)
            {
                this.RevokeUsers = this.RevokeUsers.Substring(0, this.RevokeUsers.Length - 1);
                revokeUserIds = this.RevokeUsers.Split(';');
            }

            // ��������Ȩ����
            DotNetService.Instance.PermissionService.GrantUserOrganizeScopes(UserInfo, this.TargetUserId, this.PermissionItemScopeCode, grantOrganizeIds);
            if (grantOrganizeIds != null)
            {
                DotNetService.Instance.PermissionService.RevokeUserOrganizeScopes(UserInfo, this.TargetUserId, this.PermissionItemScopeCode, revokeOrganizeIds);
            }

            DotNetService.Instance.PermissionService.GrantUserRoleScopes(UserInfo, this.TargetUserId, this.PermissionItemScopeCode, grantRoleIds);
            DotNetService.Instance.PermissionService.RevokeUserRoleScopes(UserInfo, this.TargetUserId, this.PermissionItemScopeCode, revokeRoleIds);

            DotNetService.Instance.PermissionService.GrantUserUserScopes(UserInfo, this.TargetUserId, this.PermissionItemScopeCode, grantUserIds);
            DotNetService.Instance.PermissionService.RevokeUserUserScopes(UserInfo, this.TargetUserId, this.PermissionItemScopeCode, revokeUserIds);
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

        private void tvOrganize_MouseDown(object sender, MouseEventArgs e)
        {
            if (tvOrganize.GetNodeAt(e.X, e.Y) != null)
            {
                tvOrganize.SelectedNode = tvOrganize.GetNodeAt(e.X, e.Y);
            }
        }
    }
}