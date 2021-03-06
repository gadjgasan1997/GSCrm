#pragma checksum "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\InvoicesList.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "7bb20b5352c2c520714e7c78d1be9b07bfd4d6fb"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Account_Partial_InvoicesList), @"mvc.1.0.view", @"/Views/Account/Partial/InvoicesList.cshtml")]
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
#line 4 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\_ViewImports.cshtml"
using GSCrm.Localization;

#line default
#line hidden
#nullable disable
#nullable restore
#line 1 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\InvoicesList.cshtml"
using GSCrm.Data.ApplicationInfo;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\InvoicesList.cshtml"
using GSCrm.Models.ViewModels;

#line default
#line hidden
#nullable disable
#nullable restore
#line 3 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\InvoicesList.cshtml"
using static GSCrm.CommonConsts;

#line default
#line hidden
#nullable disable
#nullable restore
#line 4 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\InvoicesList.cshtml"
using GSCrm.Helpers;

#line default
#line hidden
#nullable disable
#nullable restore
#line 5 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\InvoicesList.cshtml"
using GSCrm.Data;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"7bb20b5352c2c520714e7c78d1be9b07bfd4d6fb", @"/Views/Account/Partial/InvoicesList.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"af9277d557923f57a3a7522d76ddc19e27ae9212", @"/Views/_ViewImports.cshtml")]
    public class Views_Account_Partial_InvoicesList : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<AccountViewModel>
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("src", new global::Microsoft.AspNetCore.Html.HtmlString("~/img/default-empty.svg"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("class", new global::Microsoft.AspNetCore.Html.HtmlString("table-wrapper mt-3"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
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
        private global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 10 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\InvoicesList.cshtml"
  
    int invoicesCount = Model.AccountInvoices.Count;
    bool showES = invoicesCount == 0 && new[]
        { Model.SearchInvoiceBankName, Model.SearchInvoiceCity, Model.SearchInvoiceCheckingAccount, Model.SearchInvoiceCorrespondentAccount, Model.SearchInvoiceBIC, Model.SearchInvoiceSWIFT }.AllIsNullOrEmpty();
    string currentUserId = User.GetUserModel(context).Id;

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n");
#nullable restore
#line 18 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\InvoicesList.cshtml"
 if (!showES)
{
    await Html.RenderPartialAsync($"{ACC_VIEWS_REL_PATH}Partial/AccountInvoicesManagement.cshtml");
}

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n<div id=\"accInvoicesList\">\r\n    ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("form", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "7bb20b5352c2c520714e7c78d1be9b07bfd4d6fb6156", async() => {
                WriteLiteral("\r\n        <table");
                BeginWriteAttribute("class", " class=\"", 895, "\"", 943, 2);
                WriteAttributeValue("", 903, "fl-table", 903, 8, true);
#nullable restore
#line 25 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\InvoicesList.cshtml"
WriteAttributeValue(" ", 911, !showES ? "" : "empty-table", 912, 31, false);

#line default
#line hidden
#nullable disable
                EndWriteAttribute();
                WriteLiteral(">\r\n            <thead>\r\n");
#nullable restore
#line 27 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\InvoicesList.cshtml"
                 if (!showES)
                {

#line default
#line hidden
#nullable disable
                WriteLiteral("                    <tr>\r\n                        <th class=\"d-none\"></th>\r\n                        <th>");
#nullable restore
#line 31 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\InvoicesList.cshtml"
                       Write(resManager.GetString("BankName"));

#line default
#line hidden
#nullable disable
                WriteLiteral("</th>\r\n                        <th>");
#nullable restore
#line 32 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\InvoicesList.cshtml"
                       Write(resManager.GetString("BankCity"));

#line default
#line hidden
#nullable disable
                WriteLiteral("</th>\r\n                        <th>");
#nullable restore
#line 33 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\InvoicesList.cshtml"
                       Write(resManager.GetString("CheckingAccount"));

#line default
#line hidden
#nullable disable
                WriteLiteral("</th>\r\n                        <th>");
#nullable restore
#line 34 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\InvoicesList.cshtml"
                       Write(resManager.GetString("CorrespondentAccount"));

#line default
#line hidden
#nullable disable
                WriteLiteral("</th>\r\n                        <th>");
#nullable restore
#line 35 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\InvoicesList.cshtml"
                       Write(resManager.GetString("BIC"));

#line default
#line hidden
#nullable disable
                WriteLiteral("</th>\r\n                        <th>");
#nullable restore
#line 36 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\InvoicesList.cshtml"
                       Write(resManager.GetString("SWIFT"));

#line default
#line hidden
#nullable disable
                WriteLiteral("</th>\r\n                        <th class=\"action-column\"></th>\r\n                        <th class=\"action-column\"></th>\r\n                    </tr>\r\n");
#nullable restore
#line 40 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\InvoicesList.cshtml"
                }
                else
                {

#line default
#line hidden
#nullable disable
                WriteLiteral("                    <tr><th></th><th></th></tr>\r\n");
#nullable restore
#line 44 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\InvoicesList.cshtml"
                }

#line default
#line hidden
#nullable disable
                WriteLiteral("            </thead>\r\n            <tbody>\r\n");
#nullable restore
#line 47 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\InvoicesList.cshtml"
                 if (showES)
                {

#line default
#line hidden
#nullable disable
                WriteLiteral("                    <tr>\r\n                        <td>");
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("img", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.SelfClosing, "7bb20b5352c2c520714e7c78d1be9b07bfd4d6fb10090", async() => {
                }
                );
                __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper>();
                __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper);
                __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_0);
                await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
                if (!__tagHelperExecutionContext.Output.IsContentModified)
                {
                    await __tagHelperExecutionContext.SetOutputContentAsync();
                }
                Write(__tagHelperExecutionContext.Output);
                __tagHelperExecutionContext = __tagHelperScopeManager.End();
                WriteLiteral("</td>\r\n                        <td>\r\n                            <input type=\"button\" class=\"btn btn-outline-dark\" data-toggle=\"modal\" data-target=\"#accInvoiceCreateModal\"");
                BeginWriteAttribute("value", " value=\"", 2136, "\"", 2179, 1);
#nullable restore
#line 52 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\InvoicesList.cshtml"
WriteAttributeValue("", 2144, resManager.GetString("ESInvoices"), 2144, 35, false);

#line default
#line hidden
#nullable disable
                EndWriteAttribute();
                WriteLiteral(">\r\n                        </td>\r\n                    </tr>\r\n");
#nullable restore
#line 55 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\InvoicesList.cshtml"
                }

#line default
#line hidden
#nullable disable
#nullable restore
#line 56 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\InvoicesList.cshtml"
                 foreach (AccountInvoiceViewModel accountInvoice in Model.AccountInvoices)
                {

#line default
#line hidden
#nullable disable
                WriteLiteral("                    <tr>\r\n                        <td class=\"invoice-id d-none\">");
#nullable restore
#line 59 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\InvoicesList.cshtml"
                                                 Write(accountInvoice.Id);

#line default
#line hidden
#nullable disable
                WriteLiteral("</td>\r\n                        <td class=\"tooltip-cell-src\">");
#nullable restore
#line 60 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\InvoicesList.cshtml"
                                                Write(accountInvoice.BankName);

#line default
#line hidden
#nullable disable
                WriteLiteral("</td>\r\n                        <td class=\"tooltip-cell-src\">");
#nullable restore
#line 61 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\InvoicesList.cshtml"
                                                Write(accountInvoice.City);

#line default
#line hidden
#nullable disable
                WriteLiteral("</td>\r\n                        <td class=\"tooltip-cell-src\">");
#nullable restore
#line 62 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\InvoicesList.cshtml"
                                                Write(accountInvoice.CheckingAccount);

#line default
#line hidden
#nullable disable
                WriteLiteral("</td>\r\n                        <td class=\"tooltip-cell-src\">");
#nullable restore
#line 63 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\InvoicesList.cshtml"
                                                Write(accountInvoice.CorrespondentAccount);

#line default
#line hidden
#nullable disable
                WriteLiteral("</td>\r\n                        <td class=\"tooltip-cell-src\">");
#nullable restore
#line 64 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\InvoicesList.cshtml"
                                                Write(accountInvoice.BIC);

#line default
#line hidden
#nullable disable
                WriteLiteral("</td>\r\n                        <td class=\"tooltip-cell-src\">");
#nullable restore
#line 65 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\InvoicesList.cshtml"
                                                Write(accountInvoice.SWIFT);

#line default
#line hidden
#nullable disable
                WriteLiteral("</td>\r\n                        <td class=\"remove-item-btn\">\r\n                            <div class=\"remove-item-url\" hidden=\"hidden\">\r\n                                ");
#nullable restore
#line 68 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\InvoicesList.cshtml"
                           Write(Html.ActionLink(accountInvoice.Id.ToString(), "Delete", ACC_INVOICE, new { id = accountInvoice.Id }));

#line default
#line hidden
#nullable disable
                WriteLiteral(@"""
                            </div>
                            <span class=""icon-bin""></span>
                        </td>
                        <td class=""edit-item-btn"" data-toggle=""modal"" data-target=""#accInvoiceUpdateModal"">
                            <div class=""edit-item-url"" hidden=""hidden"">
                                ");
#nullable restore
#line 74 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\InvoicesList.cshtml"
                           Write(Html.ActionLink(accountInvoice.Id.ToString(), INVOICE, ACC_INVOICE, new { id = accountInvoice.Id }));

#line default
#line hidden
#nullable disable
                WriteLiteral("\"\r\n                            </div>\r\n                            <span class=\"icon-pencil\"></span>\r\n                        </td>\r\n                    </tr>\r\n");
#nullable restore
#line 79 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\InvoicesList.cshtml"
                }

#line default
#line hidden
#nullable disable
                WriteLiteral("            </tbody>\r\n        </table>\r\n    ");
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.FormTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.RenderAtEndOfFormTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_1);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n");
#nullable restore
#line 83 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\InvoicesList.cshtml"
      
        await Html.RenderPartialAsync("Partial/Navigation", new NavbarRenderSettings
        {
            ItemsCount = invoicesCount,
            ViewInfo = viewsInfo.Get(currentUserId, ACC_INVOICES),
            ControllerName = ACC_INVOICE,
            ActionName = INVOICES
        });
    

#line default
#line hidden
#nullable disable
            WriteLiteral("</div>");
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public IViewsInfo viewsInfo { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public ResManager resManager { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public ApplicationDbContext context { get; private set; }
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
