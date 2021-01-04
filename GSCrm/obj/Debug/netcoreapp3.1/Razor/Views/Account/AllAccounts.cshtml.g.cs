#pragma checksum "C:\Users\gadjg\source\repos\GitHubProjects\GSCrm\feature\Notifications\GSCrm\Views\Account\AllAccounts.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "ca5f73e287f482972c2c5111db607ab4bcd07dd1"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Account_AllAccounts), @"mvc.1.0.view", @"/Views/Account/AllAccounts.cshtml")]
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
#line 4 "C:\Users\gadjg\source\repos\GitHubProjects\GSCrm\feature\Notifications\GSCrm\Views\_ViewImports.cshtml"
using GSCrm.Localization;

#line default
#line hidden
#nullable disable
#nullable restore
#line 1 "C:\Users\gadjg\source\repos\GitHubProjects\GSCrm\feature\Notifications\GSCrm\Views\Account\AllAccounts.cshtml"
using GSCrm.Models.ViewModels;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "C:\Users\gadjg\source\repos\GitHubProjects\GSCrm\feature\Notifications\GSCrm\Views\Account\AllAccounts.cshtml"
using GSCrm.Data.ApplicationInfo;

#line default
#line hidden
#nullable disable
#nullable restore
#line 3 "C:\Users\gadjg\source\repos\GitHubProjects\GSCrm\feature\Notifications\GSCrm\Views\Account\AllAccounts.cshtml"
using GSCrm.Helpers;

#line default
#line hidden
#nullable disable
#nullable restore
#line 4 "C:\Users\gadjg\source\repos\GitHubProjects\GSCrm\feature\Notifications\GSCrm\Views\Account\AllAccounts.cshtml"
using GSCrm.Data;

#line default
#line hidden
#nullable disable
#nullable restore
#line 6 "C:\Users\gadjg\source\repos\GitHubProjects\GSCrm\feature\Notifications\GSCrm\Views\Account\AllAccounts.cshtml"
using static GSCrm.CommonConsts;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"ca5f73e287f482972c2c5111db607ab4bcd07dd1", @"/Views/Account/AllAccounts.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"af9277d557923f57a3a7522d76ddc19e27ae9212", @"/Views/_ViewImports.cshtml")]
    public class Views_Account_AllAccounts : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<AccountsViewModel>
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("src", new global::Microsoft.AspNetCore.Html.HtmlString("~/img/default-empty.svg"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("class", new global::Microsoft.AspNetCore.Html.HtmlString("table-wrapper"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
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
#line 10 "C:\Users\gadjg\source\repos\GitHubProjects\GSCrm\feature\Notifications\GSCrm\Views\Account\AllAccounts.cshtml"
  
    int allAccountsCount = Model.AllAccounts.Count();
    bool showES = allAccountsCount == 0 && new[] { Model.AllAccountsSearchName, Model.AllAccountsSearchType }.AllIsNullOrEmpty();
    string currentUserId = User.GetUserModel(context).Id;

#line default
#line hidden
#nullable disable
            WriteLiteral("<div class=\"tab-pane fade\" id=\"allAccounts\" role=\"tabpanel\" aria-labelledby=\"allAccountsTab\">\r\n");
#nullable restore
#line 16 "C:\Users\gadjg\source\repos\GitHubProjects\GSCrm\feature\Notifications\GSCrm\Views\Account\AllAccounts.cshtml"
     if (!showES)
    {
        

#line default
#line hidden
#nullable disable
#nullable restore
#line 18 "C:\Users\gadjg\source\repos\GitHubProjects\GSCrm\feature\Notifications\GSCrm\Views\Account\AllAccounts.cshtml"
                                   
        await Html.RenderPartialAsync($"{ACC_VIEWS_REL_PATH}Partial/AllAccountsFilter.cshtml");
    }

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n    ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("form", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "ca5f73e287f482972c2c5111db607ab4bcd07dd16638", async() => {
                WriteLiteral("\r\n        <table");
                BeginWriteAttribute("class", " class=\"", 831, "\"", 879, 2);
                WriteAttributeValue("", 839, "fl-table", 839, 8, true);
#nullable restore
#line 23 "C:\Users\gadjg\source\repos\GitHubProjects\GSCrm\feature\Notifications\GSCrm\Views\Account\AllAccounts.cshtml"
WriteAttributeValue(" ", 847, !showES ? "" : "empty-table", 848, 31, false);

#line default
#line hidden
#nullable disable
                EndWriteAttribute();
                WriteLiteral(">\r\n            <thead>\r\n");
#nullable restore
#line 25 "C:\Users\gadjg\source\repos\GitHubProjects\GSCrm\feature\Notifications\GSCrm\Views\Account\AllAccounts.cshtml"
                 if (!showES)
                {

#line default
#line hidden
#nullable disable
                WriteLiteral("                    <tr>\r\n                        <th>");
#nullable restore
#line 28 "C:\Users\gadjg\source\repos\GitHubProjects\GSCrm\feature\Notifications\GSCrm\Views\Account\AllAccounts.cshtml"
                       Write(resManager.GetString("NameLabel"));

#line default
#line hidden
#nullable disable
                WriteLiteral("</th>\r\n                        <th>");
#nullable restore
#line 29 "C:\Users\gadjg\source\repos\GitHubProjects\GSCrm\feature\Notifications\GSCrm\Views\Account\AllAccounts.cshtml"
                       Write(resManager.GetString("AccountType"));

#line default
#line hidden
#nullable disable
                WriteLiteral("</th>\r\n                    </tr>\r\n");
#nullable restore
#line 31 "C:\Users\gadjg\source\repos\GitHubProjects\GSCrm\feature\Notifications\GSCrm\Views\Account\AllAccounts.cshtml"
                }
                else {

#line default
#line hidden
#nullable disable
                WriteLiteral(" <tr><th></th><th></th></tr> ");
#nullable restore
#line 32 "C:\Users\gadjg\source\repos\GitHubProjects\GSCrm\feature\Notifications\GSCrm\Views\Account\AllAccounts.cshtml"
                                                   }

#line default
#line hidden
#nullable disable
                WriteLiteral("            </thead>\r\n            <tbody>\r\n");
#nullable restore
#line 35 "C:\Users\gadjg\source\repos\GitHubProjects\GSCrm\feature\Notifications\GSCrm\Views\Account\AllAccounts.cshtml"
                 if (showES)
                {

#line default
#line hidden
#nullable disable
                WriteLiteral("                    <tr>\r\n                        <td>");
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("img", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.SelfClosing, "ca5f73e287f482972c2c5111db607ab4bcd07dd19381", async() => {
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
                WriteLiteral("</td>\r\n                        <td>");
#nullable restore
#line 39 "C:\Users\gadjg\source\repos\GitHubProjects\GSCrm\feature\Notifications\GSCrm\Views\Account\AllAccounts.cshtml"
                       Write(resManager.GetString("ESAccount"));

#line default
#line hidden
#nullable disable
                WriteLiteral("</td>\r\n                    </tr>\r\n");
#nullable restore
#line 41 "C:\Users\gadjg\source\repos\GitHubProjects\GSCrm\feature\Notifications\GSCrm\Views\Account\AllAccounts.cshtml"
                }

#line default
#line hidden
#nullable disable
#nullable restore
#line 42 "C:\Users\gadjg\source\repos\GitHubProjects\GSCrm\feature\Notifications\GSCrm\Views\Account\AllAccounts.cshtml"
                 foreach (AccountViewModel account in Model.AllAccounts)
                {

#line default
#line hidden
#nullable disable
                WriteLiteral("                    <tr>\r\n                        <td class=\"tooltip-cell-src tooltip-cell-link label-non-select account-item\">\r\n                            ");
#nullable restore
#line 46 "C:\Users\gadjg\source\repos\GitHubProjects\GSCrm\feature\Notifications\GSCrm\Views\Account\AllAccounts.cshtml"
                       Write(Html.ActionLink(account.Name, "GetAccount", ACCOUNT, new
                            {
                                id = account.Id,
                                selectedAccountsTab = ALL_ACCS
                            }));

#line default
#line hidden
#nullable disable
                WriteLiteral("\r\n                        </td>\r\n\r\n");
#nullable restore
#line 53 "C:\Users\gadjg\source\repos\GitHubProjects\GSCrm\feature\Notifications\GSCrm\Views\Account\AllAccounts.cshtml"
                         switch (account.AccountType)
                        {
                            case "Individual":

#line default
#line hidden
#nullable disable
                WriteLiteral("                                <td>");
#nullable restore
#line 56 "C:\Users\gadjg\source\repos\GitHubProjects\GSCrm\feature\Notifications\GSCrm\Views\Account\AllAccounts.cshtml"
                               Write(resManager.GetString("Individual"));

#line default
#line hidden
#nullable disable
                WriteLiteral("</td>\r\n");
#nullable restore
#line 57 "C:\Users\gadjg\source\repos\GitHubProjects\GSCrm\feature\Notifications\GSCrm\Views\Account\AllAccounts.cshtml"
                                break;
                            case "IndividualEntrepreneur":

#line default
#line hidden
#nullable disable
                WriteLiteral("                                <td>");
#nullable restore
#line 59 "C:\Users\gadjg\source\repos\GitHubProjects\GSCrm\feature\Notifications\GSCrm\Views\Account\AllAccounts.cshtml"
                               Write(resManager.GetString("IE"));

#line default
#line hidden
#nullable disable
                WriteLiteral("</td>\r\n");
#nullable restore
#line 60 "C:\Users\gadjg\source\repos\GitHubProjects\GSCrm\feature\Notifications\GSCrm\Views\Account\AllAccounts.cshtml"
                                break;
                            case "LegalEntity":

#line default
#line hidden
#nullable disable
                WriteLiteral("                                <td>");
#nullable restore
#line 62 "C:\Users\gadjg\source\repos\GitHubProjects\GSCrm\feature\Notifications\GSCrm\Views\Account\AllAccounts.cshtml"
                               Write(resManager.GetString("LE"));

#line default
#line hidden
#nullable disable
                WriteLiteral("</td>\r\n");
#nullable restore
#line 63 "C:\Users\gadjg\source\repos\GitHubProjects\GSCrm\feature\Notifications\GSCrm\Views\Account\AllAccounts.cshtml"
                                break;
                        }

#line default
#line hidden
#nullable disable
                WriteLiteral("                    </tr>\r\n");
#nullable restore
#line 66 "C:\Users\gadjg\source\repos\GitHubProjects\GSCrm\feature\Notifications\GSCrm\Views\Account\AllAccounts.cshtml"
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
#line 70 "C:\Users\gadjg\source\repos\GitHubProjects\GSCrm\feature\Notifications\GSCrm\Views\Account\AllAccounts.cshtml"
      
        await Html.RenderPartialAsync("Partial/Navigation", new NavbarRenderSettings
        {
            ItemsCount = allAccountsCount,
            ViewInfo = viewsInfo.Get(currentUserId, ALL_ACCS),
            ControllerName = ACCOUNT,
            ActionName = ALL_ACCS
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<AccountsViewModel> Html { get; private set; }
    }
}
#pragma warning restore 1591
