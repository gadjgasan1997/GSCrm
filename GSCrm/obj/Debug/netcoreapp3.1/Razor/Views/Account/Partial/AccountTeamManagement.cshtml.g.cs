#pragma checksum "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\AccountTeamManagement.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "358c4bc98e9af6b945fe7a21745958078ae87456"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Account_Partial_AccountTeamManagement), @"mvc.1.0.view", @"/Views/Account/Partial/AccountTeamManagement.cshtml")]
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
#line 2 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\AccountTeamManagement.cshtml"
using static GSCrm.CommonConsts;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"358c4bc98e9af6b945fe7a21745958078ae87456", @"/Views/Account/Partial/AccountTeamManagement.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"af9277d557923f57a3a7522d76ddc19e27ae9212", @"/Views/_ViewImports.cshtml")]
    public class Views_Account_Partial_AccountTeamManagement : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<AccountViewModel>
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("id", new global::Microsoft.AspNetCore.Html.HtmlString("organizationId"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("hidden", new global::Microsoft.AspNetCore.Html.HtmlString("hidden"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_2 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("class", new global::Microsoft.AspNetCore.Html.HtmlString("form-control"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_3 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("method", "post", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_4 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-action", "Synchronize", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
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
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.InputTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper;
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.FormTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper;
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.RenderAtEndOfFormTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\r\n");
            WriteLiteral("<div id=\"accTeamManagementModal\" class=\"modal fade\" data-backdrop=\"static\" data-keyboard=\"false\" tabindex=\"-1\" role=\"dialog\" aria-hidden=\"true\">\r\n    ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("input", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.SelfClosing, "358c4bc98e9af6b945fe7a21745958078ae874565837", async() => {
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.InputTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_0);
#nullable restore
#line 7 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\AccountTeamManagement.cshtml"
__Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper.For = ModelExpressionProvider.CreateModelExpression(ViewData, __model => __model.OrganizationId);

#line default
#line hidden
#nullable disable
            __tagHelperExecutionContext.AddTagHelperAttribute("asp-for", __Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper.For, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_1);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n    <div class=\"modal-dialog modal-xl\" role=\"document\">\r\n        <div class=\"modal-content\">\r\n            ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("form", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "358c4bc98e9af6b945fe7a21745958078ae874567566", async() => {
                WriteLiteral("\r\n                <div class=\"modal-header\">\r\n                    <h5 class=\"modal-title\">");
#nullable restore
#line 12 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\AccountTeamManagement.cshtml"
                                       Write(resManager.GetString("AccountTeamManagement"));

#line default
#line hidden
#nullable disable
                WriteLiteral(@"</h5>
                    <button type=""button"" class=""close"" data-dismiss=""modal"" aria-label=""Close"">
                        <span aria-hidden=""true"">&times;</span>
                    </button>
                </div>
                <div class=""modal-body"">
                    <div id=""teamAllEmployees"">
                        <div class=""row justify-content-center p-2"">
                            <div class=""row justify-content-start"">
                                <ul id=""primaryManagerError"" class=""d-none""></ul>
                            </div>
                            <div class=""row justify-content-center"">
                                <p class=""label-md m-0 text-center"">");
#nullable restore
#line 24 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\AccountTeamManagement.cshtml"
                                                               Write(resManager.GetString("AllManagersExceptSelected"));

#line default
#line hidden
#nullable disable
                WriteLiteral(@"</p>
                            </div>
                            <div class=""card-body"">
                                <div class=""row justify-content-around form-group mb-0 mt-1"">
                                    <div class=""col-lg mb-4 mb-lg-0"">
                                        ");
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("input", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.SelfClosing, "358c4bc98e9af6b945fe7a21745958078ae874569595", async() => {
                }
                );
                __Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.InputTagHelper>();
                __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper);
#nullable restore
#line 29 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\AccountTeamManagement.cshtml"
__Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper.For = ModelExpressionProvider.CreateModelExpression(ViewData, __model => __model.SearchAllManagersName);

#line default
#line hidden
#nullable disable
                __tagHelperExecutionContext.AddTagHelperAttribute("asp-for", __Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper.For, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
                __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_2);
                BeginAddHtmlAttributeValues(__tagHelperExecutionContext, "placeholder", 1, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
#nullable restore
#line 29 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\AccountTeamManagement.cshtml"
AddHtmlAttributeValue("", 1823, resManager.GetString("PersonName"), 1823, 35, false);

#line default
#line hidden
#nullable disable
                EndAddHtmlAttributeValues(__tagHelperExecutionContext);
                await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
                if (!__tagHelperExecutionContext.Output.IsContentModified)
                {
                    await __tagHelperExecutionContext.SetOutputContentAsync();
                }
                Write(__tagHelperExecutionContext.Output);
                __tagHelperExecutionContext = __tagHelperScopeManager.End();
                WriteLiteral("\r\n                                    </div>\r\n                                    <div class=\"col-lg mb-4 mb-lg-0\">\r\n                                        ");
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("input", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.SelfClosing, "358c4bc98e9af6b945fe7a21745958078ae8745611863", async() => {
                }
                );
                __Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.InputTagHelper>();
                __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper);
#nullable restore
#line 32 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\AccountTeamManagement.cshtml"
__Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper.For = ModelExpressionProvider.CreateModelExpression(ViewData, __model => __model.SearchAllManagersDivision);

#line default
#line hidden
#nullable disable
                __tagHelperExecutionContext.AddTagHelperAttribute("asp-for", __Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper.For, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
                __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_2);
                BeginAddHtmlAttributeValues(__tagHelperExecutionContext, "placeholder", 1, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
#nullable restore
#line 32 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\AccountTeamManagement.cshtml"
AddHtmlAttributeValue("", 2096, resManager.GetString("Division"), 2096, 33, false);

#line default
#line hidden
#nullable disable
                EndAddHtmlAttributeValues(__tagHelperExecutionContext);
                await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
                if (!__tagHelperExecutionContext.Output.IsContentModified)
                {
                    await __tagHelperExecutionContext.SetOutputContentAsync();
                }
                Write(__tagHelperExecutionContext.Output);
                __tagHelperExecutionContext = __tagHelperScopeManager.End();
                WriteLiteral("\r\n                                    </div>\r\n                                    <div class=\"col-lg mb-4 mb-lg-0\">\r\n                                        ");
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("input", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.SelfClosing, "358c4bc98e9af6b945fe7a21745958078ae8745614134", async() => {
                }
                );
                __Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.InputTagHelper>();
                __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper);
#nullable restore
#line 35 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\AccountTeamManagement.cshtml"
__Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper.For = ModelExpressionProvider.CreateModelExpression(ViewData, __model => __model.SearchAllManagersPosition);

#line default
#line hidden
#nullable disable
                __tagHelperExecutionContext.AddTagHelperAttribute("asp-for", __Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper.For, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
                __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_2);
                BeginAddHtmlAttributeValues(__tagHelperExecutionContext, "placeholder", 1, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
#nullable restore
#line 35 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\AccountTeamManagement.cshtml"
AddHtmlAttributeValue("", 2367, resManager.GetString("Position"), 2367, 33, false);

#line default
#line hidden
#nullable disable
                EndAddHtmlAttributeValues(__tagHelperExecutionContext);
                await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
                if (!__tagHelperExecutionContext.Output.IsContentModified)
                {
                    await __tagHelperExecutionContext.SetOutputContentAsync();
                }
                Write(__tagHelperExecutionContext.Output);
                __tagHelperExecutionContext = __tagHelperScopeManager.End();
                WriteLiteral(@"
                                    </div>
                                    <div class=""col-auto mb-3 mb-sm-0 text-center flex-grow-0"">
                                        <input id=""allEmployeesSearch"" type=""button"" class=""btn btn-outline-primary""");
                BeginWriteAttribute("value", " value=\"", 2663, "\"", 2707, 1);
#nullable restore
#line 38 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\AccountTeamManagement.cshtml"
WriteAttributeValue("", 2671, resManager.GetString("ApplyFilter"), 2671, 36, false);

#line default
#line hidden
#nullable disable
                EndWriteAttribute();
                WriteLiteral("\r\n                                               data-href=\"");
#nullable restore
#line 39 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\AccountTeamManagement.cshtml"
                                                     Write(Url.Action("SearchAllManagers", ACC_MANAGER));

#line default
#line hidden
#nullable disable
                WriteLiteral("\" />\r\n                                    </div>\r\n                                    <div class=\"col-auto text-center flex-grow-0\">\r\n                                        <input id=\"clearAllEmployeesSearch\" type=\"button\" class=\"btn btn-outline-primary\"");
                BeginWriteAttribute("value", " value=\"", 3068, "\"", 3106, 1);
#nullable restore
#line 42 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\AccountTeamManagement.cshtml"
WriteAttributeValue("", 3076, resManager.GetString("Clear"), 3076, 30, false);

#line default
#line hidden
#nullable disable
                EndWriteAttribute();
                WriteLiteral("\r\n                                               data-href=\"");
#nullable restore
#line 43 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\AccountTeamManagement.cshtml"
                                                     Write(Url.Action("ClearAllManagersSearch", ACC_MANAGER, new { accountId = Model.Id }));

#line default
#line hidden
#nullable disable
                WriteLiteral(@""" />
                                    </div>
                                </div>
                            </div>
                            <div id=""allEmployeesList"" class=""table-wrapper mt-3"">
                                <table class=""fl-table"">
                                    <thead>
                                        <tr>
                                            <th>");
#nullable restore
#line 51 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\AccountTeamManagement.cshtml"
                                           Write(resManager.GetString("PersonName"));

#line default
#line hidden
#nullable disable
                WriteLiteral("</th>\r\n                                            <th>");
#nullable restore
#line 52 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\AccountTeamManagement.cshtml"
                                           Write(resManager.GetString("Division"));

#line default
#line hidden
#nullable disable
                WriteLiteral("</th>\r\n                                            <th>");
#nullable restore
#line 53 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\AccountTeamManagement.cshtml"
                                           Write(resManager.GetString("Position"));

#line default
#line hidden
#nullable disable
                WriteLiteral(@"</th>
                                            <th class=""action-column""></th>
                                        </tr>
                                    </thead>
                                    <tbody></tbody>
                                </table>
                            </div>
                            <div id=""allEmployeesNav"" class=""list-nav row no-gutters justify-content-center mt-3"">
                                <div class=""col"">
                                    <div class=""nav-previous"">
                                        <div class=""nav-url"" data-href=""");
#nullable restore
#line 63 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\AccountTeamManagement.cshtml"
                                                                   Write(Url.Action("PreviousAllRecords", ACC_MANAGER, new { accountId = Model.Id }));

#line default
#line hidden
#nullable disable
                WriteLiteral(@""">
                                            <span class=""icon-chevron-thin-left""></span>
                                        </div>
                                    </div>
                                </div>
                                <div class=""col"">
                                    <div class=""nav-next"">
                                        <div class=""nav-url"" data-href=""");
#nullable restore
#line 70 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\AccountTeamManagement.cshtml"
                                                                   Write(Url.Action("NextAllRecords", ACC_MANAGER, new { accountId = Model.Id }));

#line default
#line hidden
#nullable disable
                WriteLiteral(@""">
                                            <span class=""icon-chevron-thin-right""></span>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div id=""teamSelectedEmployees"" class=""mt-3"">
                        <div class=""row justify-content-center"">
                            <p class=""label-md m-0 text-center"">");
#nullable restore
#line 80 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\AccountTeamManagement.cshtml"
                                                           Write(resManager.GetString("SelectedManagers"));

#line default
#line hidden
#nullable disable
                WriteLiteral(@"</p>
                        </div>
                        <div class=""row p-2"">
                            <div class=""card-body"">
                                <div class=""row justify-content-around form-group mb-0 mt-1"">
                                    <div class=""col-lg mb-4 mb-lg-0"">
                                        ");
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("input", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.SelfClosing, "358c4bc98e9af6b945fe7a21745958078ae8745622752", async() => {
                }
                );
                __Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.InputTagHelper>();
                __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper);
#nullable restore
#line 86 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\AccountTeamManagement.cshtml"
__Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper.For = ModelExpressionProvider.CreateModelExpression(ViewData, __model => __model.SearchSelectedManagersName);

#line default
#line hidden
#nullable disable
                __tagHelperExecutionContext.AddTagHelperAttribute("asp-for", __Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper.For, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
                __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_2);
                BeginAddHtmlAttributeValues(__tagHelperExecutionContext, "placeholder", 1, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
#nullable restore
#line 86 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\AccountTeamManagement.cshtml"
AddHtmlAttributeValue("", 6016, resManager.GetString("PersonName"), 6016, 35, false);

#line default
#line hidden
#nullable disable
                EndAddHtmlAttributeValues(__tagHelperExecutionContext);
                await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
                if (!__tagHelperExecutionContext.Output.IsContentModified)
                {
                    await __tagHelperExecutionContext.SetOutputContentAsync();
                }
                Write(__tagHelperExecutionContext.Output);
                __tagHelperExecutionContext = __tagHelperScopeManager.End();
                WriteLiteral("\r\n                                    </div>\r\n                                    <div class=\"col-lg mb-4 mb-lg-0\">\r\n                                        ");
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("input", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.SelfClosing, "358c4bc98e9af6b945fe7a21745958078ae8745625026", async() => {
                }
                );
                __Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.InputTagHelper>();
                __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper);
#nullable restore
#line 89 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\AccountTeamManagement.cshtml"
__Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper.For = ModelExpressionProvider.CreateModelExpression(ViewData, __model => __model.SearchSelectedManagersPosition);

#line default
#line hidden
#nullable disable
                __tagHelperExecutionContext.AddTagHelperAttribute("asp-for", __Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper.For, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
                __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_2);
                BeginAddHtmlAttributeValues(__tagHelperExecutionContext, "placeholder", 1, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
#nullable restore
#line 89 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\AccountTeamManagement.cshtml"
AddHtmlAttributeValue("", 6294, resManager.GetString("Position"), 6294, 33, false);

#line default
#line hidden
#nullable disable
                EndAddHtmlAttributeValues(__tagHelperExecutionContext);
                await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
                if (!__tagHelperExecutionContext.Output.IsContentModified)
                {
                    await __tagHelperExecutionContext.SetOutputContentAsync();
                }
                Write(__tagHelperExecutionContext.Output);
                __tagHelperExecutionContext = __tagHelperScopeManager.End();
                WriteLiteral("\r\n                                    </div>\r\n                                    <div class=\"col-lg mb-4 mb-lg-0\">\r\n                                        ");
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("input", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.SelfClosing, "358c4bc98e9af6b945fe7a21745958078ae8745627302", async() => {
                }
                );
                __Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.InputTagHelper>();
                __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper);
#nullable restore
#line 92 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\AccountTeamManagement.cshtml"
__Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper.For = ModelExpressionProvider.CreateModelExpression(ViewData, __model => __model.SearchSelectedManagersPhone);

#line default
#line hidden
#nullable disable
                __tagHelperExecutionContext.AddTagHelperAttribute("asp-for", __Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper.For, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
                __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_2);
                BeginAddHtmlAttributeValues(__tagHelperExecutionContext, "placeholder", 1, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
#nullable restore
#line 92 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\AccountTeamManagement.cshtml"
AddHtmlAttributeValue("", 6567, resManager.GetString("PhoneNumber"), 6567, 36, false);

#line default
#line hidden
#nullable disable
                EndAddHtmlAttributeValues(__tagHelperExecutionContext);
                await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
                if (!__tagHelperExecutionContext.Output.IsContentModified)
                {
                    await __tagHelperExecutionContext.SetOutputContentAsync();
                }
                Write(__tagHelperExecutionContext.Output);
                __tagHelperExecutionContext = __tagHelperScopeManager.End();
                WriteLiteral(@"
                                    </div>
                                    <div class=""col-auto mb-3 mb-sm-0 text-center flex-grow-0"">
                                        <input id=""selectedEmployeesSearch"" type=""button"" class=""btn btn-outline-primary""");
                BeginWriteAttribute("value", " value=\"", 6871, "\"", 6915, 1);
#nullable restore
#line 95 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\AccountTeamManagement.cshtml"
WriteAttributeValue("", 6879, resManager.GetString("ApplyFilter"), 6879, 36, false);

#line default
#line hidden
#nullable disable
                EndWriteAttribute();
                WriteLiteral("\r\n                                               data-href=\"");
#nullable restore
#line 96 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\AccountTeamManagement.cshtml"
                                                     Write(Url.Action("SearchSelectedManagers", ACC_MANAGER));

#line default
#line hidden
#nullable disable
                WriteLiteral(@""" />
                                    </div>
                                    <div class=""col-auto text-center flex-grow-0"">
                                        <input id=""clearSelectedEmployeesSearch"" type=""button"" class=""btn btn-outline-primary""");
                BeginWriteAttribute("value", " value=\"", 7286, "\"", 7324, 1);
#nullable restore
#line 99 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\AccountTeamManagement.cshtml"
WriteAttributeValue("", 7294, resManager.GetString("Clear"), 7294, 30, false);

#line default
#line hidden
#nullable disable
                EndWriteAttribute();
                WriteLiteral("\r\n                                               data-href=\"");
#nullable restore
#line 100 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\AccountTeamManagement.cshtml"
                                                     Write(Url.Action("ClearSelectedManagersSearch", ACC_MANAGER, new { accountId = Model.Id }));

#line default
#line hidden
#nullable disable
                WriteLiteral(@""" />
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div id=""selectedEmployeesList"">
                            <div class=""list-card""></div>
                        </div>
                    </div>
                </div>
                <div class=""modal-footer"">
                    <div class=""row justify-content-between m-0"">
                        <div class=""col"">
                            <input id=""syncAccTeamBtn"" type=""button"" class=""btn btn-outline-primary""");
                BeginWriteAttribute("value", " value=\"", 8083, "\"", 8127, 1);
#nullable restore
#line 113 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\AccountTeamManagement.cshtml"
WriteAttributeValue("", 8091, resManager.GetString("Synchronize"), 8091, 36, false);

#line default
#line hidden
#nullable disable
                EndWriteAttribute();
                WriteLiteral("\r\n                                   data-href=\"");
#nullable restore
#line 114 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\AccountTeamManagement.cshtml"
                                         Write(Url.Action("Synchronize", ACC_MANAGER));

#line default
#line hidden
#nullable disable
                WriteLiteral("\" />\r\n                        </div>\r\n                        <div class=\"col text-right\">\r\n                            <input id=\"cancelSyncAccTeamBtn\" type=\"button\" class=\"btn btn-secondary\" data-dismiss=\"modal\"");
                BeginWriteAttribute("value", " value=\"", 8428, "\"", 8472, 1);
#nullable restore
#line 117 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\AccountTeamManagement.cshtml"
WriteAttributeValue("", 8436, resManager.GetString("CancelLabel"), 8436, 36, false);

#line default
#line hidden
#nullable disable
                EndWriteAttribute();
                WriteLiteral(" />\r\n                        </div>\r\n                    </div>\r\n                </div>\r\n            ");
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.FormTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.RenderAtEndOfFormTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.Method = (string)__tagHelperAttribute_3.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_3);
            BeginWriteTagHelperAttribute();
#nullable restore
#line 10 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\AccountTeamManagement.cshtml"
                                    WriteLiteral(ACC_MANAGER);

#line default
#line hidden
#nullable disable
            __tagHelperStringValueBuffer = EndWriteTagHelperAttribute();
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.Controller = __tagHelperStringValueBuffer;
            __tagHelperExecutionContext.AddTagHelperAttribute("asp-controller", __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.Controller, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.Action = (string)__tagHelperAttribute_4.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_4);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n        </div>\r\n    </div>\r\n</div>");
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
