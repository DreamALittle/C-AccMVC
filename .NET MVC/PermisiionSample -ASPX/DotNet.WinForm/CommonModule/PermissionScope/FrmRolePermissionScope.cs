//--------------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2012 , Hairihan TECH, Ltd. 
//--------------------------------------------------------------------

using System;
using System.Data;
using System.Windows.Forms;

namespace DotNet.WinForm
{
    using DotNet.Utilities;
    using DotNet.Business;

    /// <summary>
    /// FrmRolePermissionScope.cs
    /// ��ɫȨ����
    ///		
    /// �޸ļ�¼
    ///     2011.07.23 �汾��1.2 �Ź���    �޸�tvModule  tvPermissionItem�ڵ�ѡ��ʱ���߼�����
    ///     2011.07.17 �汾��1.1 �Ź���    �Ż�tvOragnize�ڵ�ѡ���Զ�ѡ�񸸽ڵ�
    ///     2010.07.30 �汾��3.0 JiRiGaLa Ȩ�޷�Χ���ô���������
    ///     2007.08.23 �汾��2.0 JiRiGaLa ����������
    ///     2007.08.22 �汾��1.0 JiRiGaLa ��ɫȨ����
    ///		
    /// �汾��2.0
    ///
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2010.07.30</date>
    /// </author> 
    /// </summary>  
    public partial class FrmRolePermissionScope : BaseForm
    {
        /// <summary>
        /// ��ʾ��ʾ��Ϣ
        /// </summary>
        public bool ShowInformation = true;

        /// <summary>
        /// ��֯������
        /// </summary>
        private DataTable DTOrganize = new DataTable(BaseOrganizeEntity.TableName);

        private string[] OrganizeIds = null;

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

        /// <summary>
        /// �Ƿ����û�����˸�ѡ��
        /// </summary>
        //Pcsky 2012.05.02 δʹ�õĹ��ܣ�����
        //private bool isUserClick = true;

        /// <summary>
        /// Ȩ������
        /// </summary>
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

        /// <summary>
        /// Ȩ������
        /// </summary>     
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


        /// <summary>
        /// ��ɫ����
        /// </summary>
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

        /// <summary>
        /// ��ɫ����
        /// </summary>     
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
                    this.currentEntityId = this.tvOrganize.SelectedNode.Tag.ToString();
                }
                return this.currentEntityId;
            }
            set
            {
                this.currentEntityId = value;
            }
        }

        public FrmRolePermissionScope()
        {
            InitializeComponent();
        }

        public FrmRolePermissionScope(string targetRoleId)
            : this()
        {
            this.TargetRoleId = targetRoleId;
        }

        public FrmRolePermissionScope(string roleId, string permissionItemScopeCode)
            : this(roleId)
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
            this.ucRole.AllowNull = false;
            this.ucRole.ShowRoleOnly = true;
            this.ucRole.MultiSelect = false;
            this.ucPermissionScope.AllowNull = false;
            this.ucPermissionScope.MultiSelect = false;

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
            this.OrganizeIds = DotNetService.Instance.PermissionService.GetRoleScopeOrganizeIds(UserInfo, this.TargetRoleId, this.PermissionItemScopeCode);
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
                dataRows = this.DTOrganize.Select(BaseOrganizeEntity.FieldParentId + "=" + treeNode.Tag.ToString() + "", BaseOrganizeEntity.FieldSortCode);
            }
            foreach (DataRow dataRow in dataRows)
            {
                // �жϲ�Ϊ�յĵ�ǰ�ڵ���ӽڵ�
                if ((treeNode.Tag != null) 
                    && (treeNode.Tag.ToString().Length > 0) 
                    && (!treeNode.Tag.ToString().Equals(dataRow[BaseOrganizeEntity.FieldParentId].ToString())))
                {
                    continue;
                }

                // ��ǰ�ڵ���ӽڵ�, ���ظ��ڵ�
                if ((dataRow.IsNull(BaseOrganizeEntity.FieldParentId) 
                    || (dataRow[BaseOrganizeEntity.FieldParentId].ToString().Length == 0) 
                    || (dataRow[BaseOrganizeEntity.FieldParentId].ToString().Equals(BaseSystemInfo.RootMenuCode))
                    || ((treeNode.Tag != null) && treeNode.Tag.ToString().Equals(dataRow[BaseOrganizeEntity.FieldParentId].ToString()))))
                {
                    TreeNode newTreeNode = new TreeNode();
                    newTreeNode.Text = dataRow[BaseOrganizeEntity.FieldFullName].ToString();
                    newTreeNode.Tag = dataRow[BaseOrganizeEntity.FieldId].ToString();
                    newTreeNode.Checked = Array.IndexOf(this.OrganizeIds, dataRow[BaseOrganizeEntity.FieldId].ToString()) >= 0;

                    // д�������Ϣ
                    #if (DEBUG)
                        newTreeNode.ToolTipText = dataRow[BaseRoleEntity.FieldId].ToString();
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
        }
        #endregion

        #region private void SetTreeNodesSelected(TreeNode treeNode, bool selected) �ݹ���������ѡ��״̬
        /// <summary>
        /// �ݹ���������ѡ��״̬
        /// </summary>
        /// <param name="TreeNode">���ڵ�</param>
        /// <param name="selected">ѡ��</param>
        private void SetTreeNodesSelected(TreeNode treeNode, bool selected)
        {
            if (treeNode != null && treeNode.Tag != null)
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
            if (treeNode != null && treeNode.Tag != null)
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
            //Pcsky 2012.05.02 δʹ�õĹ��ܣ�����
            //this.isUserClick = false;
            for (int i = 0; i < this.tvOrganize.Nodes.Count; i++)
            {
                this.SetTreeNodesSelected(this.tvOrganize.Nodes[i], true);
            }
            //Pcsky 2012.05.02 δʹ�õĹ��ܣ�����
            //this.isUserClick = true;
        }

        private void btnInvertSelect_Click(object sender, EventArgs e)
        {
            //Pcsky 2012.05.02 δʹ�õĹ��ܣ�����
            //this.isUserClick = false;
            for (int i = 0; i < this.tvOrganize.Nodes.Count; i++)
            {
                this.SetTreeNodesTurnSelected(this.tvOrganize.Nodes[i]);
            }
            //Pcsky 2012.05.02 δʹ�õĹ��ܣ�����
            //this.isUserClick = true;
        }

        /// <summary>
        /// ��Ȩ��Ȩ��
        /// </summary>
        string GrantOrganizes = string.Empty;

        /// <summary>
        /// ������Ȩ��
        /// </summary>
        string RevokeOrganizes = string.Empty;

        private void EntityToArray(TreeNode treeNode)
        {
            if (treeNode.Tag != null)
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
            //Pcsky 2012.05.02 δʹ�õĹ��ܣ�����
            //this.isUserClick = false;
            for (int i = 0; i < this.tvOrganize.Nodes.Count; i++)
            {
                TreeNode treeNode = this.tvOrganize.Nodes[i];
                this.CheckParentChecked(treeNode);
            }
        }

        /// <summary>
        /// ��ʾ��ǰѡ�е�Ȩ�޷�Χѡ��
        /// </summary>
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
        /// <param name="revokePermissionScope">���Ƴ���Ȩ��</param>
        /// <returns>ѡ��Ȩ�޷�Χ</returns>
        private PermissionScope GetPermissionScope(out string revokePermissionScope)
        {
            PermissionScope returnValue = PermissionScope.None;
            revokePermissionScope = string.Empty;

            if (this.rdbAll.Checked)
            {
                returnValue = PermissionScope.All;
            }
            else
            {
                revokePermissionScope += ((int)PermissionScope.All).ToString() + ";";
            }

            if (this.rdbUserCompany.Checked)
            {
                returnValue = PermissionScope.UserCompany;
            }
            else
            {
                revokePermissionScope += ((int)PermissionScope.UserCompany).ToString() + ";";
            }

            if (this.rdbUserSubCompany.Checked)
            {
                returnValue = PermissionScope.UserSubCompany;
            }
            else
            {
                revokePermissionScope += ((int)PermissionScope.UserSubCompany).ToString() + ";";
            }

            if (this.rdbUserDepartment.Checked)
            {
                returnValue = PermissionScope.UserDepartment;
            }
            else
            {
                revokePermissionScope += ((int)PermissionScope.UserDepartment).ToString() + ";";
            }

            if (this.rdbUserWorkgroup.Checked)
            {
                returnValue = PermissionScope.UserWorkgroup;
            }
            else
            {
                revokePermissionScope += ((int)PermissionScope.UserWorkgroup).ToString() + ";";
            }

            if (this.rdbUser.Checked)
            {
                returnValue = PermissionScope.User;
            }
            else
            {
                revokePermissionScope += ((int)PermissionScope.User).ToString() + ";";
            }

            if (this.rdbDetail.Checked)
            {
                returnValue = PermissionScope.Detail;
            }
            else
            {
                revokePermissionScope += ((int)PermissionScope.Detail).ToString() + ";";
            }

            if (this.rdbNone.Checked)
            {
                returnValue = PermissionScope.None;
            }
            else
            {
                revokePermissionScope += ((int)PermissionScope.None).ToString() + ";";
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
            // ���ո��� 20110322- ���ﲻӦ������Ϊ 0 �ģ�
            // nick
            // this.OrganizeIds = new string[0]; 
            for (int i = 0; i < this.tvOrganize.Nodes.Count; i++)
            {
                this.EntityToArray(this.tvOrganize.Nodes[i]);
            }
            string[] grantOrganizeIds = null;
            string[] revokeOrganizeIds = null;

            string revokePermissionScope = string.Empty;
            // ��ǰѡ�еģ������Ȩ�޷�Χ��Ҳ������֯������������ʽ��������,���ѡ������ϸ���򲻼�¼����ö��
            if (String.IsNullOrEmpty(this.GrantOrganizes))
                this.GrantOrganizes += ((int)this.GetPermissionScope(out revokePermissionScope)).ToString() + ";";

            if (this.GrantOrganizes.Length > 2)
            {
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
            this.GrantOrganizes = string.Empty;
            this.RevokeOrganizes = string.Empty;

            if (grantOrganizeIds != null)
            {
                DotNetService.Instance.PermissionService.GrantRoleOrganizeScopes(UserInfo, this.TargetRoleId, this.PermissionItemScopeCode, grantOrganizeIds);
            }
            if (revokeOrganizeIds != null)
            {
                DotNetService.Instance.PermissionService.RevokeRoleOrganizeScopes(UserInfo, this.TargetRoleId, this.PermissionItemScopeCode, revokeOrganizeIds);
            }
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

        private void tvOrganize_AfterCheck(object sender, TreeViewEventArgs e)
        {
            this.CheckChild(e.Node);
            // CheckParent(e.Node);
        }

        /// <summary>
        /// �ݹ����ֽڵ�
        /// </summary>
        /// <param name="node"></param>
        private void CheckChild(TreeNode node)
        {
            bool childNodeCheck = false;

            if (node.Nodes.Count != 0)
            {
                //����ڵ�������ѡ�ӽڵ�
                foreach (TreeNode item in node.Nodes)
                {
                    childNodeCheck = item.Checked;
                    if (childNodeCheck)
                        break;
                }

                //1�����node�����ӽڵ�checked��չ�����������ڵ㲻Ӱ���ӽڵ��ѡ��
                //2������ڵ���checked ��ΪUncheced  �ӽڵ�Ҳ�� ���unchecked
                if(!childNodeCheck||!node.Checked)
                {
                    foreach (TreeNode item in node.Nodes)
                    {
                        item.Checked =node.Checked;
                        CheckChild(item);
                    }
                }
            }
        }

        /// <summary>
        /// �ݹ��鸸�ڵ㣬������ڵ�����node��checked����ø��ڵ���checked
        /// </summary>
        /// <param name="node"></param>
        private void CheckParent(TreeNode node)
        {
            bool childNodeCheck = false;
            if (node.Parent != null)
            {
                foreach (TreeNode item in node.Parent.Nodes)
                {
                    childNodeCheck = item.Checked;
                    if (childNodeCheck)
                        break;
                }
                node.Parent.Checked = childNodeCheck;
                CheckParent(node.Parent);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

 

    }
}