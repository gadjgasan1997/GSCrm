#pragma checksum "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\AccountTeamManagement.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "4240bd9c47de846097a9e7e2646d3c175c2fc730"
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"4240bd9c47de846097a9e7e2646d3c175c2fc730", @"/Views/Account/Partial/AccountTeamManagement.cshtml")]
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
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("input", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.SelfClosing, "4240bd9c47de846097a9e7e2646d3c175c2fc7305837", async() => {
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
            WriteLiteral("\r\n    <div class=\"modal-dialog modal-xxl\" role=\"document\">\r\n        <div class=\"modal-content\">\r\n            ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("form", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "4240bd9c47de846097a9e7e2646d3c175c2fc7307567", async() => {
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
                            <div id=""teamAllEmployeesHeader"" class=""row justify-content-center"">
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
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("input", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.SelfClosing, "4240bd9c47de846097a9e7e2646d3c175c2fc7309626", async() => {
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
AddHtmlAttributeValue("", 1852, resManager.GetString("PersonName"), 1852, 35, false);

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
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("input", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.SelfClosing, "4240bd9c47de846097a9e7e2646d3c175c2fc73011894", async() => {
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
AddHtmlAttributeValue("", 2125, resManager.GetString("Division"), 2125, 33, false);

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
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("input", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.SelfClosing, "4240bd9c47de846097a9e7e2646d3c175c2fc73014165", async() => {
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
AddHtmlAttributeValue("", 2396, resManager.GetString("Position"), 2396, 33, false);

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
                BeginWriteAttribute("value", " value=\"", 2692, "\"", 2736, 1);
#nullable restore
#line 38 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\AccountTeamManagement.cshtml"
WriteAttributeValue("", 2700, resManager.GetString("ApplyFilter"), 2700, 36, false);

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
                BeginWriteAttribute("value", " value=\"", 3097, "\"", 3135, 1);
#nullable restore
#line 42 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\AccountTeamManagement.cshtml"
WriteAttributeValue("", 3105, resManager.GetString("Clear"), 3105, 30, false);

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
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("input", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.SelfClosing, "4240bd9c47de846097a9e7e2646d3c175c2fc73022783", async() => {
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
AddHtmlAttributeValue("", 6045, resManager.GetString("PersonName"), 6045, 35, false);

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
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("input", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.SelfClosing, "4240bd9c47de846097a9e7e2646d3c175c2fc73025057", async() => {
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
AddHtmlAttributeValue("", 6323, resManager.GetString("Position"), 6323, 33, false);

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
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("input", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.SelfClosing, "4240bd9c47de846097a9e7e2646d3c175c2fc73027333", async() => {
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
AddHtmlAttributeValue("", 6596, resManager.GetString("PhoneNumber"), 6596, 36, false);

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
                BeginWriteAttribute("value", " value=\"", 6900, "\"", 6944, 1);
#nullable restore
#line 95 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\AccountTeamManagement.cshtml"
WriteAttributeValue("", 6908, resManager.GetString("ApplyFilter"), 6908, 36, false);

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
                BeginWriteAttribute("value", " value=\"", 7315, "\"", 7353, 1);
#nullable restore
#line 99 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\AccountTeamManagement.cshtml"
WriteAttributeValue("", 7323, resManager.GetString("Clear"), 7323, 30, false);

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
                BeginWriteAttribute("value", " value=\"", 8112, "\"", 8156, 1);
#nullable restore
#line 113 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\AccountTeamManagement.cshtml"
WriteAttributeValue("", 8120, resManager.GetString("Synchronize"), 8120, 36, false);

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
                BeginWriteAttribute("value", " value=\"", 8457, "\"", 8501, 1);
#nullable restore
#line 117 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Account\Partial\AccountTeamManagement.cshtml"
WriteAttributeValue("", 8465, resManager.GetString("CancelLabel"), 8465, 36, false);

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
