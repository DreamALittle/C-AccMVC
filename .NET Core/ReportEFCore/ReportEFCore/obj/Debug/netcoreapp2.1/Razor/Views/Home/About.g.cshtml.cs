#pragma checksum "E:\SelfPro\ReportEFCore\ReportEFCore\Views\Home\About.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "e734817dd1cfa447298f2193724dbd85b863ad7f"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Home_About), @"mvc.1.0.view", @"/Views/Home/About.cshtml")]
[assembly:global::Microsoft.AspNetCore.Mvc.Razor.Compilation.RazorViewAttribute(@"/Views/Home/About.cshtml", typeof(AspNetCore.Views_Home_About))]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#line 1 "E:\SelfPro\ReportEFCore\ReportEFCore\Views\_ViewImports.cshtml"
using ReportEFCore;

#line default
#line hidden
#line 2 "E:\SelfPro\ReportEFCore\ReportEFCore\Views\_ViewImports.cshtml"
using ReportEFCore.Models;

#line default
#line hidden
#line 1 "E:\SelfPro\ReportEFCore\ReportEFCore\Views\Home\About.cshtml"
using Stimulsoft.Report.Mvc;

#line default
#line hidden
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"e734817dd1cfa447298f2193724dbd85b863ad7f", @"/Views/Home/About.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"4bc1e66f4d162c6cf7670217e5047d50b838cfda", @"/Views/_ViewImports.cshtml")]
    public class Views_Home_About : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            BeginContext(30, 2, true);
            WriteLiteral("\r\n");
            EndContext();
#line 3 "E:\SelfPro\ReportEFCore\ReportEFCore\Views\Home\About.cshtml"
  
    ViewData["Title"] = "Report";

#line default
#line hidden
            BeginContext(74, 2, true);
            WriteLiteral("\r\n");
            EndContext();
            BeginContext(77, 198, false);
#line 7 "E:\SelfPro\ReportEFCore\ReportEFCore\Views\Home\About.cshtml"
Write(Html.StiNetCoreViewer(new StiNetCoreViewerOptions()
    {
        Actions =
        {
            GetReport = "AboutReport",
            ViewerEvent = "ViewerEvent"
        }
        
    }));

#line default
#line hidden
            EndContext();
            BeginContext(275, 2, true);
            WriteLiteral("\r\n");
            EndContext();
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<dynamic> Html { get; private set; }
    }
}
#pragma warning restore 1591
