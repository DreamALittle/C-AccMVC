﻿#pragma checksum "..\..\Window1.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "F5D62C174F5CF424B5C88E2618263204"
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.225
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

using HAPExplorer;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace HAPExplorer {
    
    
    /// <summary>
    /// Window1
    /// </summary>
    public partial class Window1 : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 5 "..\..\Window1.xaml"
        internal HAPExplorer.Window1 HAPExplorerWindow;
        
        #line default
        #line hidden
        
        
        #line 18 "..\..\Window1.xaml"
        internal System.Windows.Controls.MenuItem mnuOpenFile;
        
        #line default
        #line hidden
        
        
        #line 19 "..\..\Window1.xaml"
        internal System.Windows.Controls.MenuItem mnuOpenUrl;
        
        #line default
        #line hidden
        
        
        #line 21 "..\..\Window1.xaml"
        internal System.Windows.Controls.MenuItem mnuExit;
        
        #line default
        #line hidden
        
        
        #line 24 "..\..\Window1.xaml"
        internal System.Windows.Controls.TextBox txtHtml;
        
        #line default
        #line hidden
        
        
        #line 26 "..\..\Window1.xaml"
        internal System.Windows.Controls.Button btnParse;
        
        #line default
        #line hidden
        
        
        #line 27 "..\..\Window1.xaml"
        internal System.Windows.Controls.Button btnTestCode;
        
        #line default
        #line hidden
        
        
        #line 29 "..\..\Window1.xaml"
        internal System.Windows.Controls.TextBox txtSearchTag;
        
        #line default
        #line hidden
        
        
        #line 30 "..\..\Window1.xaml"
        internal System.Windows.Controls.Button btnSearch;
        
        #line default
        #line hidden
        
        
        #line 31 "..\..\Window1.xaml"
        internal System.Windows.Controls.CheckBox chkFromCurrent;
        
        #line default
        #line hidden
        
        
        #line 32 "..\..\Window1.xaml"
        internal System.Windows.Controls.CheckBox chkXPath;
        
        #line default
        #line hidden
        
        
        #line 34 "..\..\Window1.xaml"
        internal System.Windows.Controls.TabControl tabControl1;
        
        #line default
        #line hidden
        
        
        #line 35 "..\..\Window1.xaml"
        internal System.Windows.Controls.TabItem tabNodeTree;
        
        #line default
        #line hidden
        
        
        #line 41 "..\..\Window1.xaml"
        internal HAPExplorer.HtmlNodeViewer HtmlNodeViewer1;
        
        #line default
        #line hidden
        
        
        #line 42 "..\..\Window1.xaml"
        internal HAPExplorer.HtmlAttributeViewer HtmlAttributeViewer1;
        
        #line default
        #line hidden
        
        
        #line 43 "..\..\Window1.xaml"
        internal System.Windows.Controls.GridSplitter gridSplitter1;
        
        #line default
        #line hidden
        
        
        #line 44 "..\..\Window1.xaml"
        internal HAPExplorer.NodeTreeView hapTree;
        
        #line default
        #line hidden
        
        
        #line 48 "..\..\Window1.xaml"
        internal System.Windows.Controls.TabItem tabSearchResults;
        
        #line default
        #line hidden
        
        
        #line 49 "..\..\Window1.xaml"
        internal System.Windows.Controls.ListBox listResults;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/HAPExplorer;component/window1.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\Window1.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal System.Delegate _CreateDelegate(System.Type delegateType, string handler) {
            return System.Delegate.CreateDelegate(delegateType, this, handler);
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.HAPExplorerWindow = ((HAPExplorer.Window1)(target));
            return;
            case 2:
            this.mnuOpenFile = ((System.Windows.Controls.MenuItem)(target));
            
            #line 18 "..\..\Window1.xaml"
            this.mnuOpenFile.Click += new System.Windows.RoutedEventHandler(this.mnuOpenFile_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.mnuOpenUrl = ((System.Windows.Controls.MenuItem)(target));
            
            #line 19 "..\..\Window1.xaml"
            this.mnuOpenUrl.Click += new System.Windows.RoutedEventHandler(this.mnuOpenUrl_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.mnuExit = ((System.Windows.Controls.MenuItem)(target));
            
            #line 21 "..\..\Window1.xaml"
            this.mnuExit.Click += new System.Windows.RoutedEventHandler(this.mnuExit_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.txtHtml = ((System.Windows.Controls.TextBox)(target));
            return;
            case 6:
            this.btnParse = ((System.Windows.Controls.Button)(target));
            
            #line 26 "..\..\Window1.xaml"
            this.btnParse.Click += new System.Windows.RoutedEventHandler(this.btnParse_Click);
            
            #line default
            #line hidden
            return;
            case 7:
            this.btnTestCode = ((System.Windows.Controls.Button)(target));
            
            #line 27 "..\..\Window1.xaml"
            this.btnTestCode.Click += new System.Windows.RoutedEventHandler(this.btnTestCode_Click);
            
            #line default
            #line hidden
            return;
            case 8:
            this.txtSearchTag = ((System.Windows.Controls.TextBox)(target));
            return;
            case 9:
            this.btnSearch = ((System.Windows.Controls.Button)(target));
            
            #line 30 "..\..\Window1.xaml"
            this.btnSearch.Click += new System.Windows.RoutedEventHandler(this.btnSearch_Click);
            
            #line default
            #line hidden
            return;
            case 10:
            this.chkFromCurrent = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 11:
            this.chkXPath = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 12:
            this.tabControl1 = ((System.Windows.Controls.TabControl)(target));
            return;
            case 13:
            this.tabNodeTree = ((System.Windows.Controls.TabItem)(target));
            return;
            case 14:
            this.HtmlNodeViewer1 = ((HAPExplorer.HtmlNodeViewer)(target));
            return;
            case 15:
            this.HtmlAttributeViewer1 = ((HAPExplorer.HtmlAttributeViewer)(target));
            return;
            case 16:
            this.gridSplitter1 = ((System.Windows.Controls.GridSplitter)(target));
            return;
            case 17:
            this.hapTree = ((HAPExplorer.NodeTreeView)(target));
            return;
            case 18:
            this.tabSearchResults = ((System.Windows.Controls.TabItem)(target));
            return;
            case 19:
            this.listResults = ((System.Windows.Controls.ListBox)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

