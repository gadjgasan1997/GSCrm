#pragma checksum "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Employee\Partial\EmpContactUpdateModal.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "f6cda9459cae251f61c16886b22de4d55aa8661c"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Employee_Partial_EmpContactUpdateModal), @"mvc.1.0.view", @"/Views/Employee/Partial/EmpContactUpdateModal.cshtml")]
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
#nullable restore
#line 3 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Employee\Partial\EmpContactUpdateModal.cshtml"
using static GSCrm.CommonConsts;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"f6cda9459cae251f61c16886b22de4d55aa8661c", @"/Views/Employee/Partial/EmpContactUpdateModal.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"af9277d557923f57a3a7522d76ddc19e27ae9212", @"/Views/_ViewImports.cshtml")]
    public class Views_Employee_Partial_EmpContactUpdateModal : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<EmployeeViewModel>
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("method", "post", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-action", "Update", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
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
            WriteLiteral(@"
<div id=""empContactUpdateModal"" class=""modal fade"" tabindex=""-1"" role=""dialog"" aria-hidden=""true"">
    <div class=""modal-dialog"" role=""document"">
        <div class=""modal-content"">
            <div class=""modal-header"">
                <h5 class=""modal-title"">");
#nullable restore
#line 9 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Employee\Partial\EmpContactUpdateModal.cshtml"
                                   Write(resManager.GetString("Contact"));

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
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("form", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "f6cda9459cae251f61c16886b22de4d55aa8661c5301", async() => {
                WriteLiteral(@"
                    <input id=""empContactId"" hidden=""hidden"" />
                    <div class=""row mt-3"">
                        <div class=""col"">
                            <div class=""block-center"">
                                <p class=""label-md label-non-select"">");
#nullable restore
#line 20 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Employee\Partial\EmpContactUpdateModal.cshtml"
                                                                Write(resManager.GetString("ContactType"));

#line default
#line hidden
#nullable disable
                WriteLiteral(@"</p>
                            </div>
                        </div>
                        <div class=""col dropdown-area"">
                            <input id=""updateEmpContactType"" class=""current-dropdown-value"" hidden=""hidden"" />
                            <span class=""dropdown-el"">
                                <input type=""radio"" name=""contactType"" value=""None"" checked=""checked"" id=""updateContactTypeNone"">
                                <label for=""updateContactTypeNone"">");
#nullable restore
#line 27 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Employee\Partial\EmpContactUpdateModal.cshtml"
                                                              Write(resManager.GetString("None"));

#line default
#line hidden
#nullable disable
                WriteLiteral("</label>\r\n                                <input type=\"radio\" name=\"contactType\" value=\"Personal\" id=\"updateContactTypePersonal\">\r\n                                <label for=\"updateContactTypePersonal\">");
#nullable restore
#line 29 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Employee\Partial\EmpContactUpdateModal.cshtml"
                                                                  Write(resManager.GetString("Personal"));

#line default
#line hidden
#nullable disable
                WriteLiteral("</label>\r\n                                <input type=\"radio\" name=\"contactType\" value=\"Work\" id=\"updateContactTypeWork\">\r\n                                <label for=\"updateContactTypeWork\">");
#nullable restore
#line 31 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Employee\Partial\EmpContactUpdateModal.cshtml"
                                                              Write(resManager.GetString("Work"));

#line default
#line hidden
#nullable disable
                WriteLiteral(@"</label>
                            </span>
                        </div>
                        <ul id=""updateEmpContactTypeError"" class=""under-field-error label-sm mt-3 text-center"" style=""width: 100%""></ul>
                    </div>
                    <div class=""mt-3"">
                        <input id=""updateEmpContactPhone"" class=""form-control"" type=""text""");
                BeginWriteAttribute("placeholder", " placeholder=\"", 2393, "\"", 2443, 1);
#nullable restore
#line 37 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Employee\Partial\EmpContactUpdateModal.cshtml"
WriteAttributeValue("", 2407, resManager.GetString("PhoneNumber"), 2407, 36, false);

#line default
#line hidden
#nullable disable
                EndWriteAttribute();
                WriteLiteral(@" />
                        <ul id=""updateEmpContactPhoneError"" data-connect-el=""updateEmpContactPhone"" class=""d-none under-field-error label-sm mt-2""></ul>
                    </div>
                    <div class=""mt-3"">
                        <input id=""updateEmpContactEmail"" class=""form-control"" type=""email""");
                BeginWriteAttribute("placeholder", " placeholder=\"", 2762, "\"", 2806, 1);
#nullable restore
#line 41 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Employee\Partial\EmpContactUpdateModal.cshtml"
WriteAttributeValue("", 2776, resManager.GetString("Email"), 2776, 30, false);

#line default
#line hidden
#nullable disable
                EndWriteAttribute();
                WriteLiteral(" />\r\n                        <ul id=\"updateEmpContactEmailError\" data-connect-el=\"updateEmpContactEmail\" class=\"d-none under-field-error label-sm mt-2\"></ul>\r\n                    </div>\r\n                ");
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.FormTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.RenderAtEndOfFormTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.Method = (string)__tagHelperAttribute_0.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_0);
            BeginWriteTagHelperAttribute();
#nullable restore
#line 15 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Employee\Partial\EmpContactUpdateModal.cshtml"
                                        WriteLiteral(EMP_CONTACT);

#line default
#line hidden
#nullable disable
            __tagHelperStringValueBuffer = EndWriteTagHelperAttribute();
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.Controller = __tagHelperStringValueBuffer;
            __tagHelperExecutionContext.AddTagHelperAttribute("asp-controller", __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.Controller, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.Action = (string)__tagHelperAttribute_1.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_1);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n            </div>\r\n            <div class=\"modal-footer\">\r\n                <input id=\"updateEmpContactBtn\" type=\"submit\" class=\"btn btn-primary\"");
            BeginWriteAttribute("value", " value=\"", 3164, "\"", 3201, 1);
#nullable restore
#line 47 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Employee\Partial\EmpContactUpdateModal.cshtml"
WriteAttributeValue("", 3172, resManager.GetString("Save"), 3172, 29, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(" />\r\n                <input id=\"cancelUpdateEmpContactBtn\" type=\"button\" class=\"btn btn-secondary\" data-dismiss=\"modal\"");
            BeginWriteAttribute("value", " value=\"", 3321, "\"", 3365, 1);
#nullable restore
#line 48 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Employee\Partial\EmpContactUpdateModal.cshtml"
WriteAttributeValue("", 3329, resManager.GetString("CancelLabel"), 3329, 36, false);

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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<EmployeeViewModel> Html { get; private set; }
    }
}
#pragma warning restore 1591
