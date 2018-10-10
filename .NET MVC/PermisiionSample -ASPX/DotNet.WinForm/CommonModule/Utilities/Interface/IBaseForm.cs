//-----------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2012 , Hairihan TECH, Ltd. 
//-----------------------------------------------------------------

using System;

namespace DotNet.WinForm
{
    /// <summary>
    /// IBaseForm
    /// ͨ�ô��ڵĽӿ�
    /// 
    /// �޸ļ�¼
    ///
    ///		2008.05.04 �汾��1.0 JiRiGaLa ������
    ///		
    /// �汾��1.0
    ///
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2008.05.04</date>
    /// </author> 
    /// </summary>
    public interface IBaseForm
	{
        /// <summary>
        /// �ö�
        /// </summary>
        int SetTop();

        /// <summary>
        /// ����
        /// </summary>
        int SetUp();

        /// <summary>
        /// ����
        /// </summary>
        int SetDown();

        /// <summary>
        /// �õ�
        /// </summary>
        int SetBottom();

        /// <summary>
        /// ��������ť����״̬
        /// </summary>
        /// <param name="enabled">����</param>
        void SetSortButton(bool enabled);

        /// <summary>
        /// ���
        /// </summary>
        string Add();

        /// <summary>
        /// �༭
        /// </summary>
        void Edit();

        /// <summary>
        /// ɾ��
        /// </summary>
        int Delete();

        /// <summary>
        /// ����
        /// </summary>
        void Save();
        
        /// <summary>
        /// ���ÿؼ�״̬
        /// </summary>
        void SetControlState();

        /// <summary>
        /// ���ư�ť״̬�¼�
        /// </summary>
        event BaseForm.SetControlStateEventHandler OnButtonStateChange;
    }
}