#pragma checksum "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\AccountAddressUpdateModal.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "194eaf70513039fdc2a448101fe5a7057e9efedb"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Account_Partial_AccountAddressUpdateModal), @"mvc.1.0.view", @"/Views/Account/Partial/AccountAddressUpdateModal.cshtml")]
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
#line 3 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\AccountAddressUpdateModal.cshtml"
using static GSCrm.CommonConsts;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"194eaf70513039fdc2a448101fe5a7057e9efedb", @"/Views/Account/Partial/AccountAddressUpdateModal.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"af9277d557923f57a3a7522d76ddc19e27ae9212", @"/Views/_ViewImports.cshtml")]
    public class Views_Account_Partial_AccountAddressUpdateModal : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<AccountViewModel>
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
<div id=""accAddressUpdateModal"" class=""modal fade"" tabindex=""-1"" role=""dialog"" aria-hidden=""true"">
    <div class=""modal-dialog"" role=""document"">
        <div class=""modal-content"">
            <div class=""modal-header"">
                <h5 class=""modal-title"">");
#nullable restore
#line 9 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\AccountAddressUpdateModal.cshtml"
                                   Write(resManager.GetString("AddressUpdate"));

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
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("form", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "194eaf70513039fdc2a448101fe5a7057e9efedb5327", async() => {
                WriteLiteral(@"
                    <input id=""accAddressId"" hidden=""hidden"" />
                    <div class=""form-group mt-3"">
                        <div id=""updateAccAddressCountry"" class=""autocomplete default-autocomplete"" data-autocomplite-type=""countries"">
                            <a class=""autocomplete-link"" hidden=""hidden""");
                BeginWriteAttribute("href", " href=\"", 1055, "\"", 1105, 1);
#nullable restore
#line 19 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\AccountAddressUpdateModal.cshtml"
WriteAttributeValue("", 1062, Url.Content($"~/{APP_INFO}/GetCountries/"), 1062, 43, false);

#line default
#line hidden
#nullable disable
                EndWriteAttribute();
                WriteLiteral("></a>\r\n                            <input id=\"updateAccAddressCountryVal\" class=\"autocomplete-input form-control\"");
                BeginWriteAttribute("placeholder", " placeholder=\"", 1219, "\"", 1265, 1);
#nullable restore
#line 20 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\AccountAddressUpdateModal.cshtml"
WriteAttributeValue("", 1233, resManager.GetString("Country"), 1233, 32, false);

#line default
#line hidden
#nullable disable
                EndWriteAttribute();
                WriteLiteral(@" />
                            <ul class=""autocomplete-result-list""></ul>
                        </div>
                        <ul id=""updateAccAddressCountryError"" data-connect-el=""updateAccAddressCountryVal"" class=""d-none under-field-error label-sm mt-2""></ul>
                    </div>
                    <div class=""mt-3"">
                        <input id=""updateAccAddressRegion"" class=""form-control"" type=""text""");
                BeginWriteAttribute("placeholder", " placeholder=\"", 1695, "\"", 1740, 1);
#nullable restore
#line 26 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\AccountAddressUpdateModal.cshtml"
WriteAttributeValue("", 1709, resManager.GetString("Region"), 1709, 31, false);

#line default
#line hidden
#nullable disable
                EndWriteAttribute();
                WriteLiteral(@" />
                        <ul id=""updateAccAddressRegionError"" data-connect-el=""updateAccAddressRegion"" class=""d-none under-field-error label-sm mt-2""></ul>
                    </div>
                    <div class=""mt-3"">
                        <input id=""updateAccAddressCity"" class=""form-control"" type=""text""");
                BeginWriteAttribute("placeholder", " placeholder=\"", 2059, "\"", 2102, 1);
#nullable restore
#line 30 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\AccountAddressUpdateModal.cshtml"
WriteAttributeValue("", 2073, resManager.GetString("City"), 2073, 29, false);

#line default
#line hidden
#nullable disable
                EndWriteAttribute();
                WriteLiteral(@" />
                        <ul id=""updateAccAddressCityError"" data-connect-el=""updateAccAddressCity"" class=""d-none under-field-error label-sm mt-2""></ul>
                    </div>
                    <div class=""mt-3"">
                        <input id=""updateAccAddressStreet"" class=""form-control"" type=""text""");
                BeginWriteAttribute("placeholder", " placeholder=\"", 2419, "\"", 2464, 1);
#nullable restore
#line 34 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\AccountAddressUpdateModal.cshtml"
WriteAttributeValue("", 2433, resManager.GetString("Street"), 2433, 31, false);

#line default
#line hidden
#nullable disable
                EndWriteAttribute();
                WriteLiteral(@" />
                        <ul id=""updateAccAddressStreetError"" data-connect-el=""updateAccAddressStreet"" class=""d-none under-field-error label-sm mt-2""></ul>
                    </div>
                    <div class=""mt-3"">
                        <input id=""updateAccAddressHouse"" class=""form-control"" type=""text""");
                BeginWriteAttribute("placeholder", " placeholder=\"", 2784, "\"", 2828, 1);
#nullable restore
#line 38 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\AccountAddressUpdateModal.cshtml"
WriteAttributeValue("", 2798, resManager.GetString("House"), 2798, 30, false);

#line default
#line hidden
#nullable disable
                EndWriteAttribute();
                WriteLiteral(@" />
                        <ul id=""updateAccAddressHouseError"" data-connect-el=""updateAccAddressHouse"" class=""d-none under-field-error label-sm mt-2""></ul>
                    </div>
                    <div class=""row mt-3"">
                        <div class=""col"">
                            <div class=""block-center"">
                                <p class=""label-md label-non-select"">");
#nullable restore
#line 44 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\AccountAddressUpdateModal.cshtml"
                                                                Write(resManager.GetString("AddressType"));

#line default
#line hidden
#nullable disable
                WriteLiteral(@"</p>
                            </div>
                        </div>
                        <div class=""col dropdown-area"">
                            <input id=""updateAccAddressType"" class=""current-dropdown-value"" hidden=""hidden"" />
                            <span class=""dropdown-el"">
                                <input type=""radio"" name=""addressType"" value=""None"" checked=""checked"" id=""updateAddressTypeNone"">
                                <label for=""updateAddressTypeNone"">");
#nullable restore
#line 51 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\AccountAddressUpdateModal.cshtml"
                                                              Write(resManager.GetString("None"));

#line default
#line hidden
#nullable disable
                WriteLiteral("</label>\r\n                                <input type=\"radio\" name=\"addressType\" value=\"Legal\" id=\"updateAddressTypeLegal\">\r\n                                <label for=\"updateAddressTypeLegal\">");
#nullable restore
#line 53 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\AccountAddressUpdateModal.cshtml"
                                                               Write(resManager.GetString("Legal"));

#line default
#line hidden
#nullable disable
                WriteLiteral("</label>\r\n                                <input type=\"radio\" name=\"addressType\" value=\"Postal\" id=\"updateAddressTypePostal\">\r\n                                <label for=\"updateAddressTypePostal\">");
#nullable restore
#line 55 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\AccountAddressUpdateModal.cshtml"
                                                                Write(resManager.GetString("Postal"));

#line default
#line hidden
#nullable disable
                WriteLiteral("</label>\r\n                                <input type=\"radio\" name=\"addressType\" value=\"Other\" id=\"updateAddressTypeOther\">\r\n                                <label for=\"updateAddressTypeOther\">");
#nullable restore
#line 57 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\AccountAddressUpdateModal.cshtml"
                                                               Write(resManager.GetString("Other"));

#line default
#line hidden
#nullable disable
                WriteLiteral(@"</label>
                            </span>
                        </div>
                        <ul id=""updateAccAddressTypeError"" class=""under-field-error label-sm mt-3 text-center d-none"" style=""width: 100%""></ul>
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
#line 15 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\AccountAddressUpdateModal.cshtml"
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
            WriteLiteral("\r\n            </div>\r\n            <div class=\"modal-footer\">\r\n                <input id=\"updateAccAddressBtn\" type=\"submit\" class=\"btn btn-primary\"");
            BeginWriteAttribute("value", " value=\"", 4885, "\"", 4929, 1);
#nullable restore
#line 65 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\AccountAddressUpdateModal.cshtml"
WriteAttributeValue("", 4893, resManager.GetString("UpdateLabel"), 4893, 36, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(" />\r\n                <input id=\"cancelUpdateAccAddressBtn\" type=\"button\" class=\"btn btn-secondary\" data-dismiss=\"modal\"");
            BeginWriteAttribute("value", " value=\"", 5049, "\"", 5093, 1);
#nullable restore
#line 66 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\AccountAddressUpdateModal.cshtml"
WriteAttributeValue("", 5057, resManager.GetString("CancelLabel"), 5057, 36, false);

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
