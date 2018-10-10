//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2009 , Jirisoft , Ltd. 
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;

namespace DotNet.Common.UILayer.WinForm.Login
{
    using DotNet.Business;
    using DotNet.Utilities;
    using DotNet.WinForm;

    /// <summary>
    /// FrmSAPLogin
    /// 
    /// �޸ļ�¼
    ///
    ///		2009.01.19 �汾��1.5 JiRiGaLa SAP��¼��������
    ///		2009.01.19 �汾��1.4 JiRiGaLa ������Ϊ�û�����ʵ������������
    ///		2008.03.21 �汾��1.3 JiRiGaLa �û������벻�ܼ�¼�Ĵ�����иĽ���
    ///		2007.09.17 �汾��1.2 JiRiGaLa �����ϰ� ESC ��ť�˲����Ĵ���������
    ///		2007.08.05 �汾��1.1 JiRiGaLa �û�����¼��������ʾ��Ϣ��ʾ��Ĭ�������¼������
    ///		2007.06.12 �汾��1.0 JiRiGaLa �����ļ���
    ///		
    /// �汾��1.2
    ///
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2007.09.17</date>
    /// </author> 
    /// </summary>
    public partial class FrmSAPLogin : BaseForm
    {
        public DataTable DTUser = new DataTable(BaseUserTable.TableName);    // �û��б�
        private int AllowLoginCount     = 3;    // ��������¼���� 
        private int LoginCount          = 0;    // �ѵ�¼����

        public FrmSAPLogin()
        {
            InitializeComponent();
        }

        #region private void SetHelp()
        /// <summary>
        /// ���ð���
        /// </summary>
        private void SetHelp()
        {
            this.HelpButton = true;
        }
        #endregion

        #region private void SetButtonState() ���ð�ť״̬
        /// <summary>
        /// ���ð�ť״̬
        /// </summary>
        private void SetButtonState()
        {
            this.btnRequestAnAccount.Visible = BaseConfiguration.Instance.AllowUserRegister;
            if (BaseSystemInfo.AllowNullPassword)
            {
                this.labPasswordReq.Visible = false;
            }
            if (BaseSystemInfo.Logined)
            {
                this.CancelButton = this.btnCancel;
                this.btnCancel.Show();
                this.btnExit.Hide();
            }
            else
            {
                this.CancelButton = this.btnExit;
                this.btnExit.Show();
                this.btnCancel.Hide();
            }            
        }
        #endregion

        /// <summary>
        /// ����SAP�����ַ�������ֵ���Ӷ���
        /// </summary>
        /// <param name="SapLogon"></param>
        /// <param name="sapConnection"></param>
        /// <returns></returns>
        private bool ParseSapConnection(SAPLogonCtrl.SAPLogonControlClass SapLogon, String sapConnection)
        {
            try
            {
                string[] split = sapConnection.Split(new Char[] { ';', '=' });
                SapLogon.Client = split[3];
                SapLogon.Language = split[11];
                SapLogon.User = split[7];
                SapLogon.Password = split[9];
                SapLogon.ApplicationServer = split[1];
                SapLogon.SystemNumber = 0;
                return true;
            }
            catch (Exception exception)
            {
                this.ProcessException(exception);
            }
            return true;
        }

        private String SapLogon(String userName, String password, out String statusCode, out String statusMessage)
        {
            String returnValue = String.Empty;

            statusCode = StatusCode.OK.ToString();
            statusMessage = String.Empty;
            // д��SAP
            try
            {
                String connectString = ConfigHelper.GetValue("SAPConnectionString").ToString().Replace("{Username}", userName).Replace("{Password}", password);
                SAPLogonCtrl.SAPLogonControlClass SapLogon = new SAPLogonCtrl.SAPLogonControlClass();
                if (!ParseSapConnection(SapLogon, connectString))
                {
                    statusCode = "-2";
                    statusMessage = "SAP���Ӵ���ʽ����";
                    return returnValue;
                }
                // ���½�����R3��ͨ�Ż���     
                SAPLogonCtrl.Connection EnterSap = (SAPLogonCtrl.Connection)SapLogon.NewConnection();//�������� 
                if (EnterSap.Logon(0, true) == false)
                {
                    statusCode = "-1";
                    statusMessage = "����SAPʧ�ܡ�";
                    return returnValue;
                }
                returnValue = userName;
            }
            // д��SAPʧ��
            catch (Exception exception)
            {
                this.ProcessException(exception);
            }
            return userName;
        }

        #region private void GetLoginInfo() ��ȡ���еĵ�¼��Ϣ
        /// <summary>
        /// ��ȡ���еĵ�¼��Ϣ
        /// </summary>
        private void GetLoginInfo()
        {
            if (BaseSystemInfo.SavePassword)
            {
                // ��ȡע�����Ϣ
                try
                {
                    RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(@"Software\" + BaseConfiguration.COMPANY_NAME + "\\" + BaseConfiguration.Instance.SoftName, false);
                    if (registryKey != null)
                    {
                        // �����Ǳ����û����Ķ�ȡ
                        // this.cmbUser.Text = (String)registryKey.GetValue(BaseConfiguration.CURRENT_USERNAME);
                        this.cmbUser.SelectedValue = (String)registryKey.GetValue(BaseConfiguration.CURRENT_USERNAME);
                        // this.txtPassword.Text = (String)registryKey.GetValue(BaseConfiguration.CURRENT_PASSWORD);
                    }
                }
                catch
                {
                }
            }
        }
        #endregion

        #region private void FormOnLoad() ���ش���
        /// <summary>
        /// ���ش���
        /// </summary>
        private void FormOnLoad()
        {
            // ������귱æ״̬
            this.Cursor = Cursors.WaitCursor;
            this.FormLoaded = false;
            this.DTUser = ServiceManager.Instance.LoginService.GetUserDT(this.UserInfo);
            // ְԱ��Ҫ���û�������������
            this.DTUser.DefaultView.Sort = BaseUserTable.FieldRealname;
            // ����û�������ǲ�ס������
            // foreach (DataRowView dataRowView in this.DTUser.DefaultView)
            // {
            //     this.cmbUser.Items.Add(dataRowView[BaseUserTable.FieldUsername].ToString());
            // }
            // ��ʾ�û���ʵ�����������û���
            this.cmbUser.DataSource = this.DTUser.DefaultView;
            this.cmbUser.DisplayMember = BaseUserTable.FieldRealname;
            this.cmbUser.ValueMember = BaseUserTable.FieldUsername;
            
            // Ĭ�Ͻ������û���������
            this.cmbUser.Focus();
            // ��ȡ���еĵ�¼��Ϣ
            this.GetLoginInfo();
            // ��ǰ�����뽹��ͣ��������༭���ϣ��Ǻǲ���ĸĽ���
            if (this.cmbUser.Text.Length > 0)
            {
                this.ActiveControl = this.txtPassword;
                this.txtPassword.Focus();
            }
            // �����Թ��ʻ�����
            this.Localization(this);
            // ���ð���
            this.SetHelp();
            // ���ð�ť״̬
            this.SetButtonState();
            this.FormLoaded = true;
            // �������Ĭ��״̬
            this.Cursor = Cursors.Default;
        }
        #endregion

        private void FrmLogin_Load(object sender, EventArgs e)
        {
            // ������귱æ״̬
            this.Cursor = Cursors.WaitCursor;
            try
            {
                // ���ش���
                this.FormOnLoad();
            }
            catch (Exception exception)
            {
                this.ProcessException(exception);
            }
            finally
            {
                // �������Ĭ��״̬
                this.Cursor = Cursors.Default;
            }
        }

        private void cmbUsername_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.FormLoaded)
            {
                this.txtPassword.Clear();
                this.txtPassword.Focus();
            }
        }

        #region private bool CheckInput() ����������Ч��
        /// <summary>
        /// ����������Ч��
        /// </summary>
        private bool CheckInput()
        {
            // �Ƿ�û�������û���
            if (this.cmbUser.Text.Length == 0)
            {
                MessageBox.Show(AppMessage.Format(AppMessage.MSG0007, AppMessage.MSG9957), AppMessage.MSG0000, MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.cmbUser.Focus();
                return false;
            }
            if (!BaseSystemInfo.AllowNullPassword)
            {
                if (this.txtPassword.Text.Length == 0)
                {
                    MessageBox.Show(AppMessage.Format(AppMessage.MSG0007, AppMessage.MSG9964), AppMessage.MSG0000, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.txtPassword.Focus();
                    return false;
                }
            }
            this.btnConfirm.Focus();
            return true;
        }
        #endregion

        #region private void CheckAllowLoginCount() �����¼�����Ѿ�����
        /// <summary>
        /// �����¼�����Ѿ�����
        /// </summary>
        private void CheckAllowLoginCount()
        {
            if (this.LoginCount >= this.AllowLoginCount)
            {
                this.txtPassword.Clear();
                this.cmbUser.Enabled = false;
                this.txtPassword.Enabled = false;
                this.btnConfirm.Enabled = false;
                return;
            }
        }
        #endregion

        #region private bool Login()
        /// <summary>
        /// �û���¼
        /// </summary>
        /// <returns>�Ƿ�ɹ�</returns>
        private bool Login()
        {
            // æ״̬
            this.Cursor = Cursors.WaitCursor;
            String statusCode = String.Empty;
            String statusMessage = String.Empty;
            try
            {
                String userName = this.cmbUser.SelectedValue.ToString();
                String password = this.txtPassword.Text;
                userName = this.SapLogon(userName, password, out statusCode, out statusMessage);
                // �ж��Ƿ��¼�ɹ�
                if (String.IsNullOrEmpty(userName))
                {
                    return false;
                }
                BaseUserInfo userInfo = ServiceManager.Instance.LoginService.LoginByUserName(this.UserInfo, userName, out statusCode, out statusMessage);
                // BaseUserInfo userInfo = ServiceManager.Instance.LoginService.Login(this.UserInfo, userName, password, out statusCode, out statusMessage);
                if (statusCode == StatusCode.OK.ToString())
                {
                    // ������
                    if ((userInfo != null) && (userInfo.ID.Length > 0))
                    {
                        // ְԱ��¼�������¼��Ϣ
                        this.Login(userInfo);
                        // �����¼��Ϣ
                        this.SaveLoginInfo(userInfo);                        
                        // �����ǵ�¼���ܲ���
                        if (this.Parent == null)
                        {
                            this.Hide();
                            if (!BaseSystemInfo.Logined)
                            {
                                Form MainForm = CacheManager.Instance.GetForm(BaseSystemInfo.MainAssembly, BaseConfiguration.Instance.MainForm);
                                ((IBaseMainForm)MainForm).InitService();
                                ((IBaseMainForm)MainForm).InitForm();
                                MainForm.Show();
                            }
                        }
                        // �����ʾ�Ѿ���¼ϵͳ��
                        BaseSystemInfo.Logined = true;
                        // ��¼�������㣬�������µ�¼
                        this.LoginCount = 0;
                        // ����������������
                        if (this.Owner != null)
                        {
                            ((IBaseMainForm)this.Owner).InitForm();
                            ((IBaseMainForm)this.Owner).CheckMenu();                            
                            return true;
                        }
                        if (this.Parent != null)
                        {
                            // ���»�ȡ��¼״̬��Ϣ
                            ((IBaseMainForm)this.Parent.Parent).InitService();
                            ((IBaseMainForm)this.Parent.Parent).InitForm();
                            ((IBaseMainForm)this.Parent.Parent).CheckMenu();
                            this.Close();
                        }
                    }
                }
                else
                {
                    MessageBox.Show(statusMessage, AppMessage.MSG0000, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.txtPassword.Focus();
                    this.txtPassword.SelectAll();
                    return false;
                }
            }
            catch (Exception exception)
            {
                this.ProcessException(exception);
            }
            finally
            {
                // �Ѿ�æ����
                this.Cursor = Cursors.Default;
            }
            return true;
        }
        #endregion	

        #region private void Login(BaseUserInfo userInfo) ְԱ��¼�������¼��Ϣ
        /// <summary>
        /// ְԱ��¼�������¼��Ϣ
        /// </summary>
        /// <param name="userInfo">ְԱʵ��</param>
        private void Login(BaseUserInfo userInfo)
        {
            BaseSystemInfo.Login(userInfo);
        }
        #endregion

        #region private void SaveLoginInfo(BaseUserInfo userInfo) ����¼��Ϣ���浽ע�����
        /// <summary>
        /// ����¼��Ϣ���浽ע�����
        /// </summary>
        /// <param name="userInfo">�ǵ�¼�û�</param>
        private void SaveLoginInfo(BaseUserInfo userInfo)
        {
            // ������귱æ״̬
            this.Cursor = Cursors.WaitCursor;
            if (BaseSystemInfo.SavePassword)
            {
                try
                {
                    // Ĭ�ϵ���Ϣд��ע���,�Ǻ���Ҫ�Ľ�һ��
                    RegistryKey registryKey = Registry.LocalMachine.CreateSubKey(@"Software\" + BaseConfiguration.COMPANY_NAME + "\\" + BaseConfiguration.Instance.SoftName);
                    registryKey.SetValue(BaseConfiguration.CURRENT_USERNAME, userInfo.Username);
                    // RegistryKey.SetValue(BaseConfiguration.CURRENT_USERNAME, this.cmbUsername.SelectedValue);
                    // RegistryKey.SetValue(BaseConfiguration.CURRENT_USERNAME, this.cmbUsername.SelectedText);
                    registryKey.SetValue(BaseConfiguration.CURRENT_PASSWORD, userInfo.Password);
                }
                catch (Exception exception)
                {
                    this.ProcessException(exception);
                }
                finally
                {
                    // �������Ĭ��״̬
                    this.Cursor = Cursors.Default;
                }
            }
        }
        #endregion

        private void txtPassword_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                // ����������Ч��
                if (this.CheckInput())
                {
                    // �û���¼
                    this.Login();
                }
            }
        }

        private void btnRequestAnAccount_Click(object sender, EventArgs e)
        {
            String assemblyName = "DotNet.WinForm.User";
            String formName = "FrmRequestAnAccount";
            Form frmRequestAnAccount = CacheManager.Instance.GetForm(assemblyName, formName);
            if (frmRequestAnAccount.ShowDialog() == DialogResult.OK)
            {
                // �����������ʻ��ɣ�����������һ������
                this.btnRequestAnAccount.Enabled = false;
            }
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            // ��֤�û�����
            if (this.CheckInput())
            {
                // �Ѿ���¼���� ++
                this.LoginCount ++;
                // �û���¼
                this.Login();
                // �����¼�����Ѿ�����
                this.CheckAllowLoginCount();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}