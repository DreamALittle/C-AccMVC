//-----------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2012 , Hairihan TECH, Ltd. 
//-----------------------------------------------------------------

using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;

namespace DotNet.WinForm
{
    /// <summary>
    /// FormPrint
    /// ���饴�L
    /// 
    /// �ק����
    ///
    ///     2007.07.31 �����G1.5 JinGangBo  �s�W��o���aIP�a�}�P�q�L��W��oIP�a�}
    ///     2007.07.30 �����G1.4 JinGangBo  �s�W�]�m���L���s�\��C
    ///     2007.07.27 �����G1.3 JinGangBo  ��{���L�]�m�A�ק��V���L�A�s�W�F�J�I�M???�A�ɭ��ƥ��ק�
    ///     2007.07.27 �����G1.2 JinGangBo  ��{��V���L�M�a�V���L�A���L�w���̤j�ơA�Φ۰ʽդ�100%�A�Ϊ������L�C
    ///     2007.07.26 �����G1.1 JinGangBo  �N�X�ק�P��z
    ///     2007.07.26 �����G1.0 JinGangBo  �ϥ�2�ؤ�k�i��F���饴�L
    ///     
    /// �����G1.3
    /// 
    /// <author>
    ///		<name>JinGangBo</name>
    ///		<date>2007.07.26</date>
    /// </author>
    /// </summary>
    public partial class FrmPrint : Form
    {
        public FrmPrint()
        {
            InitializeComponent();
        }

        private Bitmap memoryImageOne;
        private Image memoryImageTwo;

        //�s�إ��L�]�m
        PrintDocument myPrintDocumentSet = new PrintDocument();

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern long BitBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, int dwRop);

        private void GetPrint_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyValue)
            {
                case 118:
                    // �I���FF7���s
                    this.PrintPageOne();
                    break;
                case 119:
                    // �I���FF8���s
                    this.PrintPageTwo();
                    break;
                case 67:
                    this.Close();
                    break;
                case 27:
                    this.Close();
                    break;
            }
        }

        #region private void PrintPageOne() ���L�w����k�@
        /// <summary>
        /// ���L�w��
        /// </summary>
        private void PrintPageOne()
        {
            //�]�m�����L���A
            this.Cursor = Cursors.WaitCursor;
            //�Ыط�e�̹���DC�ﹳ
            Graphics mygraphics = this.CreateGraphics();
            Size s = this.Size;
            //�ЫإH��e���ʭ��j�p���зǪ���Ϲﹳ 
            memoryImageOne = new Bitmap(s.Width, s.Height, mygraphics);
            Graphics memoryGraphics = Graphics.FromImage(memoryImageOne);
            //�o��̹�DC
            IntPtr dc1 = mygraphics.GetHdc();
            //�o���Ϫ�DC 
            IntPtr dc2 = memoryGraphics.GetHdc();
            BitBlt(dc2, 0, 0, this.ClientRectangle.Width, this.ClientRectangle.Height, dc1, 0, 0, 13369376);
            //����DC 
            mygraphics.ReleaseHdc(dc1);
            memoryGraphics.ReleaseHdc(dc2);
            //�s�إ��L�w������
            PrintPreviewDialog myPrintPreviewDialogOne = new PrintPreviewDialog();
            //�s�إ��L�ﹳ
            PrintDocument myPrintDocumentOne = new PrintDocument();
            //�s�إ��L�]�m
            PageSetupDialog myPageSetupDialogOne = new PageSetupDialog();
            //�s�إ��L��X
            myPrintDocumentOne.PrintPage += new PrintPageEventHandler(myPrintDocumentOne_PrintPage);
            //������L�w��
            myPrintPreviewDialogOne.Document = myPrintDocumentOne;
            //�N�w��ջs100%
            myPrintPreviewDialogOne.PrintPreviewControl.Zoom = 1.0;
            //�N�w������̤j��
            ((System.Windows.Forms.Form)myPrintPreviewDialogOne).WindowState = FormWindowState.Maximized;
            //���}���L�w�����f
            myPrintPreviewDialogOne.ShowDialog();
            //�]�m�����q�{���A
            this.Cursor = Cursors.Default;
        }
        #endregion

        #region private void PrintPageTwo() ���L�w����k�G
        /// <summary>
        /// ���L�w��
        /// </summary>
        private void PrintPageTwo()
        {
            //�]�m�����L���A
            this.Cursor = Cursors.WaitCursor;
            //��oALT+PRINT����
            //SendKeys.SendWait("%{PRTSC}");
            //�s�إ��L�w������ 
            PrintPreviewDialog myPrintPreviewDialogTwo = new PrintPreviewDialog();
            //�s�إ��L�ﹳ
            PrintDocument myPrintDocumentTwo = new PrintDocument();
            //�s�إ��L��X 
            myPrintDocumentTwo.PrintPage += new PrintPageEventHandler(myPrintDocumentTwo_PrintPage);
            //�s�إ��L�]�m
            PageSetupDialog myPageSetupDialogTwo = new PageSetupDialog();
            //������L�w��
            myPrintPreviewDialogTwo.Document = myPrintDocumentTwo;
            //�N�w��ջs100%
            myPrintPreviewDialogTwo.PrintPreviewControl.Zoom = 1.0;
            //�N�w������̤j��
            ((System.Windows.Forms.Form)myPrintPreviewDialogTwo).WindowState = FormWindowState.Maximized;
            //���}���L�w�����f
            myPrintPreviewDialogTwo.ShowDialog();
            //�]�m�����q�{���A
            this.Cursor = Cursors.Default;
        }
        #endregion

        #region private void PrintPageImmediacy() �������L
        /// <summary>
        /// �������L
        /// </summary>
        private void PrintPageImmediacy()
        {
            //�]�m�����L���A
            this.Cursor = Cursors.WaitCursor;
            //��oALT+PRINT����
            //SendKeys.SendWait("%{PRTSC}");
            //�s�إ��L�w������ 
            PrintPreviewDialog myPrintPreviewDialogTwo = new PrintPreviewDialog();
            //�s�إ��L�ﹳ
            PrintDocument myPrintDocumentTwo = new PrintDocument();
            //�s�إ��L��X 
            myPrintDocumentTwo.PrintPage += new PrintPageEventHandler(myPrintDocumentTwo_PrintPage);
            //�s�إ��L�]�m
            PageSetupDialog myPageSetupDialogTwo = new PageSetupDialog();
            //������L�w��
            myPrintPreviewDialogTwo.Document = myPrintDocumentTwo;
            //�N�w��ջs100%
            myPrintPreviewDialogTwo.PrintPreviewControl.Zoom = 1.0;
            //�������L
            myPrintDocumentTwo.Print();
            //�]�m�����q�{���A
            this.Cursor = Cursors.Default;
        }
        #endregion

        #region private void PrintSet() ���L�]�m
        /// <summary>
        /// ���L�]�m
        /// </summary>
        private void PrintSet()
        {
            //�]�m�����L���A
            this.Cursor = Cursors.WaitCursor;
            //��oALT+PRINT����
            //SendKeys.SendWait("%{PRTSC}");
            //�s�إ��L��X 
            myPrintDocumentSet.PrintPage += new PrintPageEventHandler(myPrintDocumentTwo_PrintPage);
            //�s�إ��L�]�m
            PageSetupDialog myPageSetupDialog = new PageSetupDialog();
            //�b���L�]�m����R�Ϲ�            
            myPageSetupDialog.Document = myPrintDocumentSet;
            myPageSetupDialog.PageSettings.Landscape = true;
            myPageSetupDialog.ShowDialog(this);
            //�O�s�]�m
            myPrintDocumentSet.DefaultPageSettings = myPageSetupDialog.PageSettings;
            myPrintDocumentSet.PrinterSettings = myPageSetupDialog.PrinterSettings;
            //�]�m�����q�{���A
            this.Cursor = Cursors.Default;
        }
        #endregion

        private void myPrintDocumentOne_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            if (radiobtnvertinal.Checked == true)
            {
                //ø�s���L�w����k�@
                e.Graphics.DrawImage(memoryImageOne, 0, 0);
            }
            else
            {
                //�Ϥ����ࢸ��
                memoryImageOne.RotateFlip(RotateFlipType.Rotate90FlipXY);
                e.Graphics.DrawImage(memoryImageOne, 0, 0);
            }
        }

        private void myPrintDocumentTwo_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            //�q�Ť��O����o�Ϥ�
            memoryImageTwo = Clipboard.GetImage();
            if (radiobtnvertinal.Checked == true)
            {
                //ø�s���L�w����k�@
                e.Graphics.DrawImage(memoryImageTwo, 0, 0);
            }
            else
            {
                //�Ϥ����ࢸ��
                memoryImageTwo.RotateFlip(RotateFlipType.Rotate90FlipXY);
                e.Graphics.DrawImage(memoryImageTwo, 0, 0);
            }
        }

        private void btnPirntSet_Click(object sender, EventArgs e)
        {
            this.PrintSet();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            myPrintDocumentSet.Print();
        }

        private void btnPrintNow_Click(object sender, EventArgs e)
        {
            this.PrintPageImmediacy();
        }

        private void btnPrintPageOne_Click_1(object sender, EventArgs e)
        {
            this.PrintPageOne();
        }

        private void btnPrintPageTwo_Click_1(object sender, EventArgs e)
        {
            this.PrintPageTwo();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FormPrint_Load(object sender, EventArgs e)
        {
            btnCancel.Focus();
        }

        private void BtnLocalIP_Click(object sender, EventArgs e)
        {
            //���o���a�������W
            string strHostName = System.Net.Dns.GetHostName();
            //�ھڦr�Ŧꫬ���D���W��,�o��IP�a�}
            //IP��o�L�ɼg�k[�_�M��]
            //System.Net.IPHostEntry myHostinfo = System.Net.Dns.GetHostByName(strHostName);
            //IP�̷s��o��k�@�@
            System.Net.IPHostEntry myHostinfo = System.Net.Dns.GetHostEntry(strHostName);
            System.Net.IPAddress myIpAddress = myHostinfo.AddressList[0];
            MessageBox.Show(myIpAddress.ToString());
        }

        private void btnFieldIP_Click(object sender, EventArgs e)
        {
            //��WIP��o�L�ɼg�k[�_�M��]
            //string strIP = System.Net.Dns.GetHostByName("www.sina.com.cn").AddressList[0].ToString();
            string strIP = System.Net.Dns.GetHostEntry("www.sina.com.cn").AddressList[0].ToString();
            MessageBox.Show(strIP);
        }
    }
}