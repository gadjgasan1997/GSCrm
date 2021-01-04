#pragma checksum "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Position\Partial\PositionSubPositionsList.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "708c2e1da64c2a84691f1ec22fb73bef88f34c95"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Position_Partial_PositionSubPositionsList), @"mvc.1.0.view", @"/Views/Position/Partial/PositionSubPositionsList.cshtml")]
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
#line 1 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Position\Partial\PositionSubPositionsList.cshtml"
using static GSCrm.CommonConsts;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Position\Partial\PositionSubPositionsList.cshtml"
using GSCrm.Data.ApplicationInfo;

#line default
#line hidden
#nullable disable
#nullable restore
#line 3 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Position\Partial\PositionSubPositionsList.cshtml"
using GSCrm.Helpers;

#line default
#line hidden
#nullable disable
#nullable restore
#line 4 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Position\Partial\PositionSubPositionsList.cshtml"
using GSCrm.Data;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"708c2e1da64c2a84691f1ec22fb73bef88f34c95", @"/Views/Position/Partial/PositionSubPositionsList.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"af9277d557923f57a3a7522d76ddc19e27ae9212", @"/Views/_ViewImports.cshtml")]
    public class Views_Position_Partial_PositionSubPositionsList : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<PositionViewModel>
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("src", new global::Microsoft.AspNetCore.Html.HtmlString("~/img/default-empty.svg"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("id", new global::Microsoft.AspNetCore.Html.HtmlString("positionSubPosList"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_2 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("class", new global::Microsoft.AspNetCore.Html.HtmlString("table-wrapper"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
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
#line 9 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Position\Partial\PositionSubPositionsList.cshtml"
  
    int subPositionsCount = Model.SubPositions.Count;
    bool showES = subPositionsCount == 0 && new[] { Model.SearchSubPositionName, Model.SearchSubPositionPrimaryEmployee }.AllIsNullOrEmpty();
    string currentUserId = User.GetUserModel(context).Id;

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n<div class=\"tab-pane fade\" id=\"subpositions\" role=\"tabpanel\" aria-labelledby=\"subpositions-tab\">\r\n");
#nullable restore
#line 17 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Position\Partial\PositionSubPositionsList.cshtml"
     if (!showES)
    {
        await Html.RenderPartialAsync($"{POS_VIEWS_REL_PATH}Partial/PositionSubPositionsFilter.cshtml");
    }

#line default
#line hidden
#nullable disable
            WriteLiteral("    <div class=\"form-group\">\r\n        ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("form", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "708c2e1da64c2a84691f1ec22fb73bef88f34c956644", async() => {
                WriteLiteral("\r\n            <table");
                BeginWriteAttribute("class", " class=\"", 892, "\"", 940, 2);
                WriteAttributeValue("", 900, "fl-table", 900, 8, true);
#nullable restore
#line 23 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Position\Partial\PositionSubPositionsList.cshtml"
WriteAttributeValue(" ", 908, !showES ? "" : "empty-table", 909, 31, false);

#line default
#line hidden
#nullable disable
                EndWriteAttribute();
                WriteLiteral(">\r\n                <thead>\r\n");
#nullable restore
#line 25 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Position\Partial\PositionSubPositionsList.cshtml"
                     if (!showES)
                    {

#line default
#line hidden
#nullable disable
                WriteLiteral("                        <tr>\r\n                            <th class=\"label-non-select\">");
#nullable restore
#line 28 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Position\Partial\PositionSubPositionsList.cshtml"
                                                    Write(resManager.GetString("NameLabel"));

#line default
#line hidden
#nullable disable
                WriteLiteral("</th>\r\n                            <th class=\"label-non-select\">");
#nullable restore
#line 29 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Position\Partial\PositionSubPositionsList.cshtml"
                                                    Write(resManager.GetString("PrimaryEmployeeLabel"));

#line default
#line hidden
#nullable disable
                WriteLiteral("</th>\r\n                        </tr>\r\n");
#nullable restore
#line 31 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Position\Partial\PositionSubPositionsList.cshtml"
                    }
                    else {

#line default
#line hidden
#nullable disable
                WriteLiteral(" <tr><th></th><th></th></tr> ");
#nullable restore
#line 32 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Position\Partial\PositionSubPositionsList.cshtml"
                                                       }

#line default
#line hidden
#nullable disable
                WriteLiteral("                </thead>\r\n                <tbody>\r\n");
#nullable restore
#line 35 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Position\Partial\PositionSubPositionsList.cshtml"
                     if (showES)
                    {

#line default
#line hidden
#nullable disable
                WriteLiteral("                        <tr>\r\n                            <td>");
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("img", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.SelfClosing, "708c2e1da64c2a84691f1ec22fb73bef88f34c959471", async() => {
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
                WriteLiteral("</td>\r\n                            <td>");
#nullable restore
#line 39 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Position\Partial\PositionSubPositionsList.cshtml"
                           Write(resManager.GetString("ESPositionSubPositions"));

#line default
#line hidden
#nullable disable
                WriteLiteral("</td>\r\n                        </tr>\r\n");
#nullable restore
#line 41 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Position\Partial\PositionSubPositionsList.cshtml"
                    }

#line default
#line hidden
#nullable disable
#nullable restore
#line 42 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Position\Partial\PositionSubPositionsList.cshtml"
                     foreach (PositionViewModel subPosition in Model.SubPositions)
                    {

#line default
#line hidden
#nullable disable
                WriteLiteral("                        <tr>\r\n                            <td class=\"tooltip-cell-src tooltip-cell-link position-subpos-item\">");
#nullable restore
#line 45 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Position\Partial\PositionSubPositionsList.cshtml"
                                                                                           Write(Html.ActionLink(subPosition.Name, POSITION, POSITION, new { id = subPosition.Id }));

#line default
#line hidden
#nullable disable
                WriteLiteral("</td>\r\n");
#nullable restore
#line 46 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Position\Partial\PositionSubPositionsList.cshtml"
                             if (!string.IsNullOrEmpty(subPosition.PrimaryEmployeeInitialName))
                            {

#line default
#line hidden
#nullable disable
                WriteLiteral("                                <td class=\"tooltip-cell-src tooltip-cell-link position-subpos-item\">");
#nullable restore
#line 48 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Position\Partial\PositionSubPositionsList.cshtml"
                                                                                               Write(Html.ActionLink(subPosition.PrimaryEmployeeInitialName, EMPLOYEE, EMPLOYEE, new { id = subPosition.PrimaryEmployeeId }));

#line default
#line hidden
#nullable disable
                WriteLiteral("</td>\r\n");
#nullable restore
#line 49 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Position\Partial\PositionSubPositionsList.cshtml"
                            }
                            else
                            {

#line default
#line hidden
#nullable disable
                WriteLiteral("                                <td></td>\r\n");
#nullable restore
#line 53 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Position\Partial\PositionSubPositionsList.cshtml"
                            }

#line default
#line hidden
#nullable disable
                WriteLiteral("                        </tr>\r\n");
#nullable restore
#line 55 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Position\Partial\PositionSubPositionsList.cshtml"
                    }

#line default
#line hidden
#nullable disable
                WriteLiteral("                </tbody>\r\n            </table>\r\n        ");
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.FormTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.RenderAtEndOfFormTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_1);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_2);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n");
#nullable restore
#line 59 "C:\Users\gadjg\source\repos\GSCrm\GSCrm\Views\Position\Partial\PositionSubPositionsList.cshtml"
          
            await Html.RenderPartialAsync("Partial/Navigation", new NavbarRenderSettings
            {
                ItemsCount = subPositionsCount,
                ViewInfo = viewsInfo.Get(currentUserId, POS_SUB_POSS),
                ControllerName = EMP_POSITION,
                ActionName = POS_SUB_POSS
            });
        

#line default
#line hidden
#nullable disable
            WriteLiteral("    </div>\r\n</div>");
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<PositionViewModel> Html { get; private set; }
    }
}
#pragma warning restore 1591
