#pragma checksum "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Position\Partial\ChangeDivisionModal.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "673e45ae28506a522f2817e64868af5a72183fd1"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Position_Partial_ChangeDivisionModal), @"mvc.1.0.view", @"/Views/Position/Partial/ChangeDivisionModal.cshtml")]
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
#nullable restore
#line 1 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\_ViewImports.cshtml"
using GSCrm;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\_ViewImports.cshtml"
using GSCrm.Models;

#line default
#line hidden
#nullable disable
#nullable restore
#line 3 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\_ViewImports.cshtml"
using GSCrm.Models.ViewModels;

#line default
#line hidden
#nullable disable
#nullable restore
#line 4 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\_ViewImports.cshtml"
using GSCrm.Localization;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"673e45ae28506a522f2817e64868af5a72183fd1", @"/Views/Position/Partial/ChangeDivisionModal.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"af9277d557923f57a3a7522d76ddc19e27ae9212", @"/Views/_ViewImports.cshtml")]
    public class Views_Position_Partial_ChangeDivisionModal : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<PositionViewModel>
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("method", "post", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-controller", "Position", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_2 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-action", "ChangeDivision", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        #line hidden
        #pragma warning disable 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperExecutionContext __tagHelperExecutionContext;
        #pragma warning restore 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner __tagHelperRunner = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner();
        #pragma warning disable 0169
        private string __tagHelperStringValueBuffer;
        #pragma warning restore 0169
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __backed__tagHelperScopeManager = null;
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __tagHelperScopeManager
        {
            get
            {
                if (__backed__tagHelperScopeManager == null)
                {
                    __backed__tagHelperScopeManager = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager(StartTagHelperWritingScope, EndTagHelperWritingScope);
                }
                return __backed__tagHelperScopeManager;
            }
        }
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.FormTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper;
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.RenderAtEndOfFormTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral(@"<div id=""changePositionDivisionModal"" class=""modal fade"" tabindex=""-1"" role=""dialog"" aria-hidden=""true"">
    <div class=""modal-dialog"" role=""document"">
        <div class=""modal-content"">
            <div class=""modal-header"">
                <h5 class=""modal-title"">");
#nullable restore
#line 8 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Position\Partial\ChangeDivisionModal.cshtml"
                                   Write(resManager.GetString("NewDivision"));

#line default
#line hidden
#nullable disable
            WriteLiteral(@"</h5>
                <button type=""button"" class=""close"" data-dismiss=""modal"" aria-label=""Close"">
                    <span aria-hidden=""true"">&times;</span>
                </button>
            </div>
            <div class=""modal-body"">
                ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("form", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "673e45ae28506a522f2817e64868af5a72183fd15406", async() => {
                WriteLiteral(@"
                    <div class=""form-group"">
                        <div class=""mt-3"">
                            <div id=""positionNewDiv"" class=""autocomplete default-autocomplete"" data-autocomplite-type=""divisions"">
                                <a class=""autocomplete-link"" hidden=""hidden""");
                BeginWriteAttribute("href", " href=\"", 1050, "\"", 1157, 1);
#nullable restore
#line 18 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Position\Partial\ChangeDivisionModal.cshtml"
WriteAttributeValue("", 1057, Url.Content($"~/Autocomplite/GetDivisionsExceptCurrent/{Model.OrganizationId}/{Model.DivisionId}/"), 1057, 100, false);

#line default
#line hidden
#nullable disable
                EndWriteAttribute();
                WriteLiteral("></a>\r\n                                <input id=\"positionNewDivVal\" class=\"autocomplete-input form-control\"");
                BeginWriteAttribute("placeholder", " placeholder=\"", 1266, "\"", 1316, 1);
#nullable restore
#line 19 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Position\Partial\ChangeDivisionModal.cshtml"
WriteAttributeValue("", 1280, resManager.GetString("NewDivision"), 1280, 36, false);

#line default
#line hidden
#nullable disable
                EndWriteAttribute();
                WriteLiteral(@" />
                                <ul class=""autocomplete-result-list""></ul>
                            </div>
                        </div>
                        <ul id=""positionNewDivError"" data-connect-el=""positionNewDivVal"" class=""d-none under-field-error label-sm mt-2""></ul>
                    </div>
                ");
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.FormTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.RenderAtEndOfFormTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.Method = (string)__tagHelperAttribute_0.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_0);
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.Controller = (string)__tagHelperAttribute_1.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_1);
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.Action = (string)__tagHelperAttribute_2.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_2);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n            </div>\r\n            <div class=\"modal-footer\">\r\n                <input id=\"changeDivisionBtn\" type=\"submit\" class=\"btn btn-primary\"");
            BeginWriteAttribute("value", " value=\"", 1805, "\"", 1849, 1);
#nullable restore
#line 28 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Position\Partial\ChangeDivisionModal.cshtml"
WriteAttributeValue("", 1813, resManager.GetString("ChangeLabel"), 1813, 36, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(" />\r\n                <input id=\"cancelChangeDivisionBtn\" type=\"button\" class=\"btn btn-secondary\" data-dismiss=\"modal\"");
            BeginWriteAttribute("value", " value=\"", 1967, "\"", 2011, 1);
#nullable restore
#line 29 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Position\Partial\ChangeDivisionModal.cshtml"
WriteAttributeValue("", 1975, resManager.GetString("CancelLabel"), 1975, 36, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(" />\r\n            </div>\r\n        </div>\r\n    </div>\r\n</div>");
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public ResManager resManager { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<PositionViewModel> Html { get; private set; }
    }
}
#pragma warning restore 1591
