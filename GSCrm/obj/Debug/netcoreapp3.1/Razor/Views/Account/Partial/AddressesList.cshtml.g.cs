#pragma checksum "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\AddressesList.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "0f2a68c50ef1d81f7efefde2a67c2aaa64f86244"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Account_Partial_AddressesList), @"mvc.1.0.view", @"/Views/Account/Partial/AddressesList.cshtml")]
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
#line 1 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\AddressesList.cshtml"
using GSCrm.Data.ApplicationInfo;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\AddressesList.cshtml"
using GSCrm.Models.ViewModels;

#line default
#line hidden
#nullable disable
#nullable restore
#line 3 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\AddressesList.cshtml"
using static GSCrm.CommonConsts;

#line default
#line hidden
#nullable disable
#nullable restore
#line 4 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\AddressesList.cshtml"
using GSCrm.Helpers;

#line default
#line hidden
#nullable disable
#nullable restore
#line 5 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\AddressesList.cshtml"
using GSCrm.Data;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"0f2a68c50ef1d81f7efefde2a67c2aaa64f86244", @"/Views/Account/Partial/AddressesList.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"af9277d557923f57a3a7522d76ddc19e27ae9212", @"/Views/_ViewImports.cshtml")]
    public class Views_Account_Partial_AddressesList : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<AccountViewModel>
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("class", new global::Microsoft.AspNetCore.Html.HtmlString("table-wrapper mt-3"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
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
#line 10 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\AddressesList.cshtml"
  
    string currentUserId = User.GetUserModel(context).Id;

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n");
#nullable restore
#line 15 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\AddressesList.cshtml"
   await Html.RenderPartialAsync($"{ACC_VIEWS_REL_PATH}Partial/AccountAddressesManagement.cshtml"); 

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n<div id=\"accAddressesList\">\r\n    ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("form", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "0f2a68c50ef1d81f7efefde2a67c2aaa64f862445316", async() => {
                WriteLiteral("\r\n        <table class=\"fl-table\">\r\n            <thead>\r\n                <tr>\r\n                    <th class=\"d-none\"></th>\r\n                    <th>");
#nullable restore
#line 23 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\AddressesList.cshtml"
                   Write(resManager.GetString("FullAddress"));

#line default
#line hidden
#nullable disable
                WriteLiteral("</th>\r\n                    <th>");
#nullable restore
#line 24 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\AddressesList.cshtml"
                   Write(resManager.GetString("AddressType"));

#line default
#line hidden
#nullable disable
                WriteLiteral("</th>\r\n                    <th class=\"action-column\"></th>\r\n                    <th class=\"action-column\"></th>\r\n                </tr>\r\n            </thead>\r\n            <tbody>\r\n");
#nullable restore
#line 30 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\AddressesList.cshtml"
                 foreach (AccountAddressViewModel accountAddress in Model.AccountAddresses)
                {

#line default
#line hidden
#nullable disable
                WriteLiteral("                    <tr>\r\n                        <td class=\"address-id d-none\">");
#nullable restore
#line 33 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\AddressesList.cshtml"
                                                 Write(accountAddress.Id);

#line default
#line hidden
#nullable disable
                WriteLiteral("</td>\r\n                        <td class=\"tooltip-cell-src\">");
#nullable restore
#line 34 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\AddressesList.cshtml"
                                                Write(accountAddress.FullAddress);

#line default
#line hidden
#nullable disable
                WriteLiteral("</td>\r\n");
#nullable restore
#line 35 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\AddressesList.cshtml"
                         switch (accountAddress.AddressType)
                        {
                            case "None":

#line default
#line hidden
#nullable disable
                WriteLiteral("                                <td class=\"address-type\">");
#nullable restore
#line 38 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\AddressesList.cshtml"
                                                    Write(resManager.GetString("NotSpecify"));

#line default
#line hidden
#nullable disable
                WriteLiteral("</td>\r\n");
#nullable restore
#line 39 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\AddressesList.cshtml"
                                break;
                            case "Legal":

#line default
#line hidden
#nullable disable
                WriteLiteral("                                <td class=\"address-type\">");
#nullable restore
#line 41 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\AddressesList.cshtml"
                                                    Write(resManager.GetString("Legal"));

#line default
#line hidden
#nullable disable
                WriteLiteral("</td>\r\n");
#nullable restore
#line 42 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\AddressesList.cshtml"
                                break;
                            case "Postal":

#line default
#line hidden
#nullable disable
                WriteLiteral("                                <td class=\"address-type\">");
#nullable restore
#line 44 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\AddressesList.cshtml"
                                                    Write(resManager.GetString("Postal"));

#line default
#line hidden
#nullable disable
                WriteLiteral("</td>\r\n");
#nullable restore
#line 45 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\AddressesList.cshtml"
                                break;
                            case "Other":

#line default
#line hidden
#nullable disable
                WriteLiteral("                                <td class=\"address-type\">");
#nullable restore
#line 47 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\AddressesList.cshtml"
                                                    Write(resManager.GetString("Other"));

#line default
#line hidden
#nullable disable
                WriteLiteral("</td>\r\n");
#nullable restore
#line 48 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\AddressesList.cshtml"
                                break;
                        }

#line default
#line hidden
#nullable disable
#nullable restore
#line 50 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\AddressesList.cshtml"
                         if (accountAddress.AddressType == "Legal")
                        {

#line default
#line hidden
#nullable disable
                WriteLiteral("                            <td class=\"hide-remove-item-btn\">\r\n                                <div class=\"remove-item-url\" hidden=\"hidden\">\r\n                                    ");
#nullable restore
#line 54 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\AddressesList.cshtml"
                               Write(Html.ActionLink(accountAddress.Id.ToString(), "Delete", ACC_ADDRESS, new { id = accountAddress.Id }));

#line default
#line hidden
#nullable disable
                WriteLiteral("\"\r\n                                </div>\r\n                                <span class=\"icon-bin\"></span>\r\n                            </td>\r\n");
#nullable restore
#line 58 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\AddressesList.cshtml"
                        }
                        else
                        {

#line default
#line hidden
#nullable disable
                WriteLiteral("                            <td class=\"remove-item-btn\">\r\n                                <div class=\"remove-item-url\" hidden=\"hidden\">\r\n                                    ");
#nullable restore
#line 63 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\AddressesList.cshtml"
                               Write(Html.ActionLink(accountAddress.Id.ToString(), "Delete", ACC_ADDRESS, new { id = accountAddress.Id }));

#line default
#line hidden
#nullable disable
                WriteLiteral("\"\r\n                                </div>\r\n                                <span class=\"icon-bin\"></span>\r\n                            </td>\r\n");
#nullable restore
#line 67 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\AddressesList.cshtml"
                        }

#line default
#line hidden
#nullable disable
                WriteLiteral("                        <td class=\"edit-item-btn\" data-toggle=\"modal\" data-target=\"#accAddressUpdateModal\">\r\n                            <div class=\"edit-item-url\" hidden=\"hidden\">\r\n                                ");
#nullable restore
#line 70 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\AddressesList.cshtml"
                           Write(Html.ActionLink(accountAddress.Id.ToString(), ADDRESS, ACC_ADDRESS, new { id = accountAddress.Id }));

#line default
#line hidden
#nullable disable
                WriteLiteral("\"\r\n                            </div>\r\n                            <span class=\"icon-pencil\"></span>\r\n                        </td>\r\n                    </tr>\r\n");
#nullable restore
#line 75 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\AddressesList.cshtml"
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
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_0);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n");
#nullable restore
#line 79 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\AddressesList.cshtml"
      
        await Html.RenderPartialAsync("Partial/Navigation", new NavbarRenderSettings
        {
            ItemsCount = Model.AccountAddresses.Count(),
            ViewInfo = viewsInfo.Get(currentUserId, ACC_ADDRESSES),
            ControllerName = ACC_ADDRESS,
            ActionName = ADDRESSES
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
