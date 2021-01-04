#pragma checksum "C:\Users\gadjg\source\repos\GitHubProjects\GSCrm\feature\Notifications\GSCrm\Views\Account\Partial\ChangeLegalAddressModal.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "1f9f5055cc44cc1855f58edd5577fcb8ec97ba82"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Account_Partial_ChangeLegalAddressModal), @"mvc.1.0.view", @"/Views/Account/Partial/ChangeLegalAddressModal.cshtml")]
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
#line 1 "C:\Users\gadjg\source\repos\GitHubProjects\GSCrm\feature\Notifications\GSCrm\Views\_ViewImports.cshtml"
using GSCrm;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "C:\Users\gadjg\source\repos\GitHubProjects\GSCrm\feature\Notifications\GSCrm\Views\_ViewImports.cshtml"
using GSCrm.Models;

#line default
#line hidden
#nullable disable
#nullable restore
#line 3 "C:\Users\gadjg\source\repos\GitHubProjects\GSCrm\feature\Notifications\GSCrm\Views\_ViewImports.cshtml"
using GSCrm.Models.ViewModels;

#line default
#line hidden
#nullable disable
#nullable restore
#line 4 "C:\Users\gadjg\source\repos\GitHubProjects\GSCrm\feature\Notifications\GSCrm\Views\_ViewImports.cshtml"
using GSCrm.Localization;

#line default
#line hidden
#nullable disable
#nullable restore
#line 3 "C:\Users\gadjg\source\repos\GitHubProjects\GSCrm\feature\Notifications\GSCrm\Views\Account\Partial\ChangeLegalAddressModal.cshtml"
using static GSCrm.CommonConsts;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"1f9f5055cc44cc1855f58edd5577fcb8ec97ba82", @"/Views/Account/Partial/ChangeLegalAddressModal.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"af9277d557923f57a3a7522d76ddc19e27ae9212", @"/Views/_ViewImports.cshtml")]
    public class Views_Account_Partial_ChangeLegalAddressModal : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<AccountViewModel>
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("method", "post", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-action", "ChangeLegalAddress", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
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
#nullable restore
#line 4 "C:\Users\gadjg\source\repos\GitHubProjects\GSCrm\feature\Notifications\GSCrm\Views\Account\Partial\ChangeLegalAddressModal.cshtml"
  
    int addressCount = 0;
    List<AccountAddressViewModel> notLegalAddresses = Model.AllAccountAddresses.Where(addr => addr.AddressType != "Legal").ToList();

#line default
#line hidden
#nullable disable
            WriteLiteral(@"
<div id=""changeLEAddrModal"" class=""modal fade"" data-backdrop=""static"" data-keyboard=""false"" tabindex=""-1"" role=""dialog"" aria-hidden=""true"">
    <div class=""modal-dialog"" role=""document"">
        <div class=""modal-content"">
            <div class=""modal-header"">
                <h5 class=""modal-title"">");
#nullable restore
#line 13 "C:\Users\gadjg\source\repos\GitHubProjects\GSCrm\feature\Notifications\GSCrm\Views\Account\Partial\ChangeLegalAddressModal.cshtml"
                                   Write(resManager.GetString("ChangeLegalAddress"));

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
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("form", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "1f9f5055cc44cc1855f58edd5577fcb8ec97ba826010", async() => {
                WriteLiteral("\r\n                    <input id=\"targetFormId\" hidden=\"hidden\" />\r\n                    <div class=\"col\">\r\n                        <p class=\"label-sm text-center\">");
#nullable restore
#line 22 "C:\Users\gadjg\source\repos\GitHubProjects\GSCrm\feature\Notifications\GSCrm\Views\Account\Partial\ChangeLegalAddressModal.cshtml"
                                                   Write(resManager.GetString("SelectNewLegalAddress"));

#line default
#line hidden
#nullable disable
                WriteLiteral("</p>\r\n                            <ul id=\"accAddressNotLegalList\" class=\"mt-1 list-group\">\r\n");
#nullable restore
#line 24 "C:\Users\gadjg\source\repos\GitHubProjects\GSCrm\feature\Notifications\GSCrm\Views\Account\Partial\ChangeLegalAddressModal.cshtml"
                             foreach (AccountAddressViewModel addressViewModel in notLegalAddresses)
                            {
                                if (addressCount == 0)
                                {

#line default
#line hidden
#nullable disable
                WriteLiteral("                                    <li class=\"list-group-item active\">");
#nullable restore
#line 28 "C:\Users\gadjg\source\repos\GitHubProjects\GSCrm\feature\Notifications\GSCrm\Views\Account\Partial\ChangeLegalAddressModal.cshtml"
                                                                  Write(addressViewModel.FullAddress);

#line default
#line hidden
#nullable disable
                WriteLiteral("\r\n                                        <p class=\"new-legal-address-id d-none\">");
#nullable restore
#line 29 "C:\Users\gadjg\source\repos\GitHubProjects\GSCrm\feature\Notifications\GSCrm\Views\Account\Partial\ChangeLegalAddressModal.cshtml"
                                                                          Write(addressViewModel.Id);

#line default
#line hidden
#nullable disable
                WriteLiteral("</p>\r\n                                    </li>\r\n");
#nullable restore
#line 31 "C:\Users\gadjg\source\repos\GitHubProjects\GSCrm\feature\Notifications\GSCrm\Views\Account\Partial\ChangeLegalAddressModal.cshtml"
                                }
                                else
                                {

#line default
#line hidden
#nullable disable
                WriteLiteral("                                    <li class=\"list-group-item\">");
#nullable restore
#line 34 "C:\Users\gadjg\source\repos\GitHubProjects\GSCrm\feature\Notifications\GSCrm\Views\Account\Partial\ChangeLegalAddressModal.cshtml"
                                                           Write(addressViewModel.FullAddress);

#line default
#line hidden
#nullable disable
                WriteLiteral("\r\n                                        <p class=\"new-legal-address-id d-none\">");
#nullable restore
#line 35 "C:\Users\gadjg\source\repos\GitHubProjects\GSCrm\feature\Notifications\GSCrm\Views\Account\Partial\ChangeLegalAddressModal.cshtml"
                                                                          Write(addressViewModel.Id);

#line default
#line hidden
#nullable disable
                WriteLiteral("</p>\r\n                                    </li>\r\n");
#nullable restore
#line 37 "C:\Users\gadjg\source\repos\GitHubProjects\GSCrm\feature\Notifications\GSCrm\Views\Account\Partial\ChangeLegalAddressModal.cshtml"
                                }
                                addressCount++;
                            }

#line default
#line hidden
#nullable disable
                WriteLiteral("                            </ul>\r\n                    </div>\r\n                    <div id=\"changeCurrentAddrType\" class=\"col mt-3\">\r\n                        <p class=\"label-sm text-center m-0\">");
#nullable restore
#line 43 "C:\Users\gadjg\source\repos\GitHubProjects\GSCrm\feature\Notifications\GSCrm\Views\Account\Partial\ChangeLegalAddressModal.cshtml"
                                                       Write(resManager.GetString("ChangeCurrentLegalAddressType"));

#line default
#line hidden
#nullable disable
                WriteLiteral("</p>\r\n                        <ul class=\"mt-3 mb-3 list-group\">\r\n                            <li class=\"list-group-item active\">");
#nullable restore
#line 45 "C:\Users\gadjg\source\repos\GitHubProjects\GSCrm\feature\Notifications\GSCrm\Views\Account\Partial\ChangeLegalAddressModal.cshtml"
                                                          Write(Model.LegalAddress);

#line default
#line hidden
#nullable disable
                WriteLiteral(@"</li>
                        </ul>
                        <div class=""col mt-1 dropdown-area text-center"">
                            <input id=""changeAccAddressType"" class=""current-dropdown-value"" hidden=""hidden"" />
                            <span class=""dropdown-el"">
                                <input type=""radio"" name=""addressType"" value=""None"" checked=""checked"" id=""changeAddressTypeNone"">
                                <label for=""changeAddressTypeNone"">");
#nullable restore
#line 51 "C:\Users\gadjg\source\repos\GitHubProjects\GSCrm\feature\Notifications\GSCrm\Views\Account\Partial\ChangeLegalAddressModal.cshtml"
                                                              Write(resManager.GetString("None"));

#line default
#line hidden
#nullable disable
                WriteLiteral("</label>\r\n                                <input type=\"radio\" name=\"addressType\" value=\"Postal\" id=\"changeAddressTypePostal\">\r\n                                <label for=\"changeAddressTypePostal\">");
#nullable restore
#line 53 "C:\Users\gadjg\source\repos\GitHubProjects\GSCrm\feature\Notifications\GSCrm\Views\Account\Partial\ChangeLegalAddressModal.cshtml"
                                                                Write(resManager.GetString("Postal"));

#line default
#line hidden
#nullable disable
                WriteLiteral("</label>\r\n                                <input type=\"radio\" name=\"addressType\" value=\"Other\" id=\"changeAddressTypeOther\">\r\n                                <label for=\"changeAddressTypeOther\">");
#nullable restore
#line 55 "C:\Users\gadjg\source\repos\GitHubProjects\GSCrm\feature\Notifications\GSCrm\Views\Account\Partial\ChangeLegalAddressModal.cshtml"
                                                               Write(resManager.GetString("Other"));

#line default
#line hidden
#nullable disable
                WriteLiteral(@"</label>
                            </span>
                        </div>
                        <ul id=""changeAccAddressTypeError"" class=""under-field-error label-sm mt-3 text-center d-none"" style=""width: 100%""></ul>
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
            BeginWriteTagHelperAttribute();
#nullable restore
#line 19 "C:\Users\gadjg\source\repos\GitHubProjects\GSCrm\feature\Notifications\GSCrm\Views\Account\Partial\ChangeLegalAddressModal.cshtml"
                                        WriteLiteral(ACC_ADDRESS);

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
            WriteLiteral("\r\n            </div>\r\n            <div class=\"modal-footer\">\r\n                <input id=\"changeLegalAddress\" type=\"submit\" class=\"btn btn-primary\"");
            BeginWriteAttribute("value", " value=\"", 3967, "\"", 4006, 1);
#nullable restore
#line 63 "C:\Users\gadjg\source\repos\GitHubProjects\GSCrm\feature\Notifications\GSCrm\Views\Account\Partial\ChangeLegalAddressModal.cshtml"
WriteAttributeValue("", 3975, resManager.GetString("Change"), 3975, 31, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(" />\r\n                <input id=\"cancelChangeLegalAddress\" type=\"button\" class=\"btn btn-secondary\" data-dismiss=\"modal\"");
            BeginWriteAttribute("value", " value=\"", 4125, "\"", 4169, 1);
#nullable restore
#line 64 "C:\Users\gadjg\source\repos\GitHubProjects\GSCrm\feature\Notifications\GSCrm\Views\Account\Partial\ChangeLegalAddressModal.cshtml"
WriteAttributeValue("", 4133, resManager.GetString("CancelLabel"), 4133, 36, false);

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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<AccountViewModel> Html { get; private set; }
    }
}
#pragma warning restore 1591
