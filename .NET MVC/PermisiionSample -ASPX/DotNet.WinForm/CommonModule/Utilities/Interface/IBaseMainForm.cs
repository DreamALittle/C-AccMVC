//-----------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2012 , Hairihan TECH, Ltd. 
//-----------------------------------------------------------------

using System;

namespace DotNet.WinForm
{
    /// <summary>
    /// IBaseMainForm
    /// �����ڵĽӿ�
    /// 
    /// �޸ļ�¼
    ///
    ///		2008.10.29 �汾��1.1 JiRiGaLa ����������
    ///		2008.05.04 �汾��1.0 JiRiGaLa ������
    ///		
    /// �汾��1.0
    ///
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2008.05.04</date>
    /// </author> 
    /// </summary>
    public interface IBaseMainForm
	{
        /// <summary>
        /// ��ʼ������
        /// </summary>
        void InitForm();

        /// <summary>
        /// ��ʼ������
        /// </summary>
        void InitService();

        /// <summary>
        /// ���˵�
        /// </summary>
        void CheckMenu();
    }
}