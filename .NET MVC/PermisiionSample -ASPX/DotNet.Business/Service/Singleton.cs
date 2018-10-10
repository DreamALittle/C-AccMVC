//-------------------------------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2011 , Hairihan TECH, Ltd. 
//-------------------------------------------------------------------------------------

namespace DotNet.Service
{
    /// <summary>
    /// Singleton
    /// ��ʵ��
    /// 
    /// �޸ļ�¼
    /// 
    ///		2007.12.30 �汾��1.0 JiRiGaLa ������
    ///		
    /// �汾��1.0
    ///
    /// <author>
    ///		<name>JiRiGaLa</name>
    ///		<date>2007.12.30</date>
    /// </author> 
    /// </summary>
    public class Singleton<T>
    {
        private static T _instance;

        public Singleton()
        {
        }

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    // ���ʵ����ʹ�����������ǰ����tҪ�й��еġ��޲����Ĺ��캯��
                    _instance = (T)System.Activator.CreateInstance(typeof(T));
                }
                return _instance;
            }
        }
    }
}