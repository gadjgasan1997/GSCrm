#pragma checksum "C:\Users\gadjg\source\repos\GitHubProjects\GSCrm\feature\Notifications\GSCrm\Views\InboxNotification\InboxNotifications.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "f4efbd984f0870aea64238b735d099942c521892"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_InboxNotification_InboxNotifications), @"mvc.1.0.view", @"/Views/InboxNotification/InboxNotifications.cshtml")]
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
#line 4 "C:\Users\gadjg\source\repos\GitHubProjects\GSCrm\feature\Notifications\GSCrm\Views\InboxNotification\InboxNotifications.cshtml"
using GSCrm.Data;

#line default
#line hidden
#nullable disable
#nullable restore
#line 5 "C:\Users\gadjg\source\repos\GitHubProjects\GSCrm\feature\Notifications\GSCrm\Views\InboxNotification\InboxNotifications.cshtml"
using GSCrm.Data.ApplicationInfo;

#line default
#line hidden
#nullable disable
#nullable restore
#line 6 "C:\Users\gadjg\source\repos\GitHubProjects\GSCrm\feature\Notifications\GSCrm\Views\InboxNotification\InboxNotifications.cshtml"
using GSCrm.Models.ViewModels;

#line default
#line hidden
#nullable disable
#nullable restore
#line 7 "C:\Users\gadjg\source\repos\GitHubProjects\GSCrm\feature\Notifications\GSCrm\Views\InboxNotification\InboxNotifications.cshtml"
using static GSCrm.CommonConsts;

#line default
#line hidden
#nullable disable
#nullable restore
#line 8 "C:\Users\gadjg\source\repos\GitHubProjects\GSCrm\feature\Notifications\GSCrm\Views\InboxNotification\InboxNotifications.cshtml"
using GSCrm.Helpers;

#line default
#line hidden
#nullable disable
#nullable restore
#line 9 "C:\Users\gadjg\source\repos\GitHubProjects\GSCrm\feature\Notifications\GSCrm\Views\InboxNotification\InboxNotifications.cshtml"
using GSCrm.Notifications;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"f4efbd984f0870aea64238b735d099942c521892", @"/Views/InboxNotification/InboxNotifications.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"af9277d557923f57a3a7522d76ddc19e27ae9212", @"/Views/_ViewImports.cshtml")]
    public class Views_InboxNotification_InboxNotifications : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<InboxNotificationsViewModel>
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-controller", "Home", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-action", "Index", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_2 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("class", new global::Microsoft.AspNetCore.Html.HtmlString("label-md"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_3 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-area", "", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_4 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("class", new global::Microsoft.AspNetCore.Html.HtmlString("col-md flex-grow-0"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_5 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-action", "", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_6 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("class", new global::Microsoft.AspNetCore.Html.HtmlString("col-lg mt-3"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_7 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("src", new global::Microsoft.AspNetCore.Html.HtmlString("~/img/default-empty.svg"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
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
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper;
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.FormTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper;
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.RenderAtEndOfFormTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper;
        private global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 1 "C:\Users\gadjg\source\repos\GitHubProjects\GSCrm\feature\Notifications\GSCrm\Views\InboxNotification\InboxNotifications.cshtml"
  
    Layout = "_Layout";

#line default
#line hidden
#nullable disable
#nullable restore
#line 14 "C:\Users\gadjg\source\repos\GitHubProjects\GSCrm\feature\Notifications\GSCrm\Views\InboxNotification\InboxNotifications.cshtml"
  
    IEnumerable<InboxNotificationViewModel> inboxNotViewModels = Model.InboxNotificationViewModels;
    User currentUser = User.GetUserModel(context);
    string currentUserId = currentUser.Id;

#line default
#line hidden
#nullable disable
            WriteLiteral("<nav aria-label=\"breadcrumb\">\r\n    <ol class=\"breadcrumb\">\r\n        <li class=\"breadcrumb-item\">");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("a", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "f4efbd984f0870aea64238b735d099942c5218928688", async() => {
#nullable restore
#line 22 "C:\Users\gadjg\source\repos\GitHubProjects\GSCrm\feature\Notifications\GSCrm\Views\InboxNotification\InboxNotifications.cshtml"
                                                                           Write(resManager.GetString("Home"));

#line default
#line hidden
#nullable disable
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.Controller = (string)__tagHelperAttribute_0.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_0);
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.Action = (string)__tagHelperAttribute_1.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_1);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("</li>\r\n        <li class=\"breadcrumb-item active\" aria-current=\"page\">");
#nullable restore
#line 23 "C:\Users\gadjg\source\repos\GitHubProjects\GSCrm\feature\Notifications\GSCrm\Views\InboxNotification\InboxNotifications.cshtml"
                                                          Write(resManager.GetString(NOTIFICATIONS));

#line default
#line hidden
#nullable disable
            WriteLiteral("</li>\r\n    </ol>\r\n</nav>\r\n\r\n<div class=\"container mt-3\">\r\n    <div class=\"text-center\">\r\n        ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("a", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "f4efbd984f0870aea64238b735d099942c52189210856", async() => {
#nullable restore
#line 29 "C:\Users\gadjg\source\repos\GitHubProjects\GSCrm\feature\Notifications\GSCrm\Views\InboxNotification\InboxNotifications.cshtml"
                                                                                                                                            Write(resManager.GetString(NOT_SETTINGS));

#line default
#line hidden
#nullable disable
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_2);
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.Area = (string)__tagHelperAttribute_3.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_3);
            BeginWriteTagHelperAttribute();
#nullable restore
#line 29 "C:\Users\gadjg\source\repos\GitHubProjects\GSCrm\feature\Notifications\GSCrm\Views\InboxNotification\InboxNotifications.cshtml"
                                            WriteLiteral(NOT_SETTING);

#line default
#line hidden
#nullable disable
            __tagHelperStringValueBuffer = EndWriteTagHelperAttribute();
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.Controller = __tagHelperStringValueBuffer;
            __tagHelperExecutionContext.AddTagHelperAttribute("asp-controller", __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.Controller, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
            BeginWriteTagHelperAttribute();
#nullable restore
#line 29 "C:\Users\gadjg\source\repos\GitHubProjects\GSCrm\feature\Notifications\GSCrm\Views\InboxNotification\InboxNotifications.cshtml"
                                                                      WriteLiteral(NOT_SETTING);

#line default
#line hidden
#nullable disable
            __tagHelperStringValueBuffer = EndWriteTagHelperAttribute();
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.Action = __tagHelperStringValueBuffer;
            __tagHelperExecutionContext.AddTagHelperAttribute("asp-action", __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.Action, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
            if (__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.RouteValues == null)
            {
                throw new InvalidOperationException(InvalidTagHelperIndexerAssignment("asp-route-id", "Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper", "RouteValues"));
            }
            BeginWriteTagHelperAttribute();
#nullable restore
#line 29 "C:\Users\gadjg\source\repos\GitHubProjects\GSCrm\feature\Notifications\GSCrm\Views\InboxNotification\InboxNotifications.cshtml"
                                                                                                  WriteLiteral(Model.UserNotificationsSettingId);

#line default
#line hidden
#nullable disable
            __tagHelperStringValueBuffer = EndWriteTagHelperAttribute();
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.RouteValues["id"] = __tagHelperStringValueBuffer;
            __tagHelperExecutionContext.AddTagHelperAttribute("asp-route-id", __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.RouteValues["id"], global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n    </div>\r\n    <div id=\"notificationsList\" class=\"mt-3\">\r\n");
#nullable restore
#line 32 "C:\Users\gadjg\source\repos\GitHubProjects\GSCrm\feature\Notifications\GSCrm\Views\InboxNotification\InboxNotifications.cshtml"
         if (inboxNotViewModels.Count() > 0)
        {
            

#line default
#line hidden
#nullable disable
#nullable restore
#line 34 "C:\Users\gadjg\source\repos\GitHubProjects\GSCrm\feature\Notifications\GSCrm\Views\InboxNotification\InboxNotifications.cshtml"
             foreach (InboxNotificationViewModel inboxNotViewModel in inboxNotViewModels)
            {
                switch (inboxNotViewModel.NotificationType)
                {
                    case NotificationType.OrgInvite:
                        if (inboxNotViewModel.InviteOrg != null)
                        {

#line default
#line hidden
#nullable disable
            WriteLiteral("                            <div class=\"row org-invite-not notification-item mt-4\">\r\n                                <input class=\"notification-item-id\"");
            BeginWriteAttribute("value", " value=\"", 1721, "\"", 1750, 1);
#nullable restore
#line 42 "C:\Users\gadjg\source\repos\GitHubProjects\GSCrm\feature\Notifications\GSCrm\Views\InboxNotification\InboxNotifications.cshtml"
WriteAttributeValue("", 1729, inboxNotViewModel.Id, 1729, 21, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(" hidden=\"hidden\" />\r\n                                ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("form", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "f4efbd984f0870aea64238b735d099942c52189216619", async() => {
                WriteLiteral("\r\n                                    <div");
                BeginWriteAttribute("class", " class=\"", 1921, "\"", 1989, 1);
#nullable restore
#line 44 "C:\Users\gadjg\source\repos\GitHubProjects\GSCrm\feature\Notifications\GSCrm\Views\InboxNotification\InboxNotifications.cshtml"
WriteAttributeValue("", 1929, !inboxNotViewModel.HasRead ? "cbx-not" : "cbx-not-onload", 1929, 60, false);

#line default
#line hidden
#nullable disable
                EndWriteAttribute();
                WriteLiteral(">\r\n                                        <input id=\"cbxNot\" type=\"checkbox\" />\r\n                                        <label for=\"cbxNot\"></label>\r\n                                    </div>\r\n                                ");
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.FormTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.RenderAtEndOfFormTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_4);
            BeginWriteTagHelperAttribute();
#nullable restore
#line 43 "C:\Users\gadjg\source\repos\GitHubProjects\GSCrm\feature\Notifications\GSCrm\Views\InboxNotification\InboxNotifications.cshtml"
                                                                     WriteLiteral(INBOX_NOT);

#line default
#line hidden
#nullable disable
            __tagHelperStringValueBuffer = EndWriteTagHelperAttribute();
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.Controller = __tagHelperStringValueBuffer;
            __tagHelperExecutionContext.AddTagHelperAttribute("asp-controller", __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.Controller, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.Action = (string)__tagHelperAttribute_5.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_5);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n                                <div class=\"col-md mt-4 mt-md-0\">\r\n                                    <div");
            BeginWriteAttribute("class", " class=\"", 2334, "\"", 2428, 3);
            WriteAttributeValue("", 2342, "alert", 2342, 5, true);
#nullable restore
#line 50 "C:\Users\gadjg\source\repos\GitHubProjects\GSCrm\feature\Notifications\GSCrm\Views\InboxNotification\InboxNotifications.cshtml"
WriteAttributeValue(" ", 2347, !inboxNotViewModel.HasRead ? "alert-dark" : "alert-light alert-not-light", 2348, 76, false);

#line default
#line hidden
#nullable disable
            WriteAttributeValue(" ", 2424, "m-0", 2425, 4, true);
            EndWriteAttribute();
            WriteLiteral(@" role=""alert"">
                                        <div class=""row"">
                                            <div class=""col-lg"">
                                                <p class=""label-non-select label-md m-0 text-center"">
                                                    Вы получили приглашение вступить в организацию
                                                    <a");
            BeginWriteAttribute("href", " href=\"", 2827, "\"", 2880, 4);
            WriteAttributeValue("", 2834, "/", 2834, 1, true);
#nullable restore
#line 55 "C:\Users\gadjg\source\repos\GitHubProjects\GSCrm\feature\Notifications\GSCrm\Views\InboxNotification\InboxNotifications.cshtml"
WriteAttributeValue("", 2835, ORGANIZATION, 2835, 13, false);

#line default
#line hidden
#nullable disable
            WriteAttributeValue("", 2848, "/", 2848, 1, true);
#nullable restore
#line 55 "C:\Users\gadjg\source\repos\GitHubProjects\GSCrm\feature\Notifications\GSCrm\Views\InboxNotification\InboxNotifications.cshtml"
WriteAttributeValue("", 2849, inboxNotViewModel.InviteOrg.Id, 2849, 31, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(">");
#nullable restore
#line 55 "C:\Users\gadjg\source\repos\GitHubProjects\GSCrm\feature\Notifications\GSCrm\Views\InboxNotification\InboxNotifications.cshtml"
                                                                                                        Write(inboxNotViewModel.InviteOrg.Name);

#line default
#line hidden
#nullable disable
            WriteLiteral("</a>\r\n                                                </p>\r\n                                            </div>\r\n                                            ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("form", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "f4efbd984f0870aea64238b735d099942c52189222150", async() => {
                WriteLiteral("\r\n                                                <div class=\"btn btn-block btn-outline-dark\">\r\n                                                    <p>");
#nullable restore
#line 60 "C:\Users\gadjg\source\repos\GitHubProjects\GSCrm\feature\Notifications\GSCrm\Views\InboxNotification\InboxNotifications.cshtml"
                                                  Write(resManager.GetString("AcceptInvite"));

#line default
#line hidden
#nullable disable
                WriteLiteral("</p>\r\n                                                </div>\r\n                                            ");
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.FormTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.RenderAtEndOfFormTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_6);
            BeginWriteTagHelperAttribute();
#nullable restore
#line 58 "C:\Users\gadjg\source\repos\GitHubProjects\GSCrm\feature\Notifications\GSCrm\Views\InboxNotification\InboxNotifications.cshtml"
                                                                          WriteLiteral(INBOX_NOT);

#line default
#line hidden
#nullable disable
            __tagHelperStringValueBuffer = EndWriteTagHelperAttribute();
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.Controller = __tagHelperStringValueBuffer;
            __tagHelperExecutionContext.AddTagHelperAttribute("asp-controller", __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.Controller, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
            BeginWriteTagHelperAttribute();
#nullable restore
#line 58 "C:\Users\gadjg\source\repos\GitHubProjects\GSCrm\feature\Notifications\GSCrm\Views\InboxNotification\InboxNotifications.cshtml"
                                                                                                  WriteLiteral(INBOX_NOTS);

#line default
#line hidden
#nullable disable
            __tagHelperStringValueBuffer = EndWriteTagHelperAttribute();
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.Action = __tagHelperStringValueBuffer;
            __tagHelperExecutionContext.AddTagHelperAttribute("asp-action", __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.Action, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n                                            ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("form", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "f4efbd984f0870aea64238b735d099942c52189225654", async() => {
                WriteLiteral("\r\n                                                <div class=\"btn btn-block btn-outline-danger\">\r\n                                                    <p>");
#nullable restore
#line 65 "C:\Users\gadjg\source\repos\GitHubProjects\GSCrm\feature\Notifications\GSCrm\Views\InboxNotification\InboxNotifications.cshtml"
                                                  Write(resManager.GetString("RejectInvite"));

#line default
#line hidden
#nullable disable
                WriteLiteral("</p>\r\n                                                </div>\r\n                                            ");
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.FormTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.RenderAtEndOfFormTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_6);
            BeginWriteTagHelperAttribute();
#nullable restore
#line 63 "C:\Users\gadjg\source\repos\GitHubProjects\GSCrm\feature\Notifications\GSCrm\Views\InboxNotification\InboxNotifications.cshtml"
                                                                          WriteLiteral(INBOX_NOT);

#line default
#line hidden
#nullable disable
            __tagHelperStringValueBuffer = EndWriteTagHelperAttribute();
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.Controller = __tagHelperStringValueBuffer;
            __tagHelperExecutionContext.AddTagHelperAttribute("asp-controller", __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.Controller, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
            BeginWriteTagHelperAttribute();
#nullable restore
#line 63 "C:\Users\gadjg\source\repos\GitHubProjects\GSCrm\feature\Notifications\GSCrm\Views\InboxNotification\InboxNotifications.cshtml"
                                                                                                  WriteLiteral(INBOX_NOTS);

#line default
#line hidden
#nullable disable
            __tagHelperStringValueBuffer = EndWriteTagHelperAttribute();
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.Action = __tagHelperStringValueBuffer;
            __tagHelperExecutionContext.AddTagHelperAttribute("asp-action", __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.Action, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n                                        </div>\r\n                                    </div>\r\n                                </div>\r\n                            </div>\r\n");
#nullable restore
#line 72 "C:\Users\gadjg\source\repos\GitHubProjects\GSCrm\feature\Notifications\GSCrm\Views\InboxNotification\InboxNotifications.cshtml"
                        }
                        break;
                }
            }

#line default
#line hidden
#nullable disable
#nullable restore
#line 75 "C:\Users\gadjg\source\repos\GitHubProjects\GSCrm\feature\Notifications\GSCrm\Views\InboxNotification\InboxNotifications.cshtml"
             
        }
        else
        {

#line default
#line hidden
#nullable disable
            WriteLiteral("            <div class=\"row notifications-empty-list\">\r\n                <div class=\"col\">\r\n                    ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("img", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.SelfClosing, "f4efbd984f0870aea64238b735d099942c52189230002", async() => {
            }
            );
            __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_7);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n                </div>\r\n                <div class=\"col\">\r\n                    <div class=\"block-center\">\r\n                        <p class=\"label-md\">");
#nullable restore
#line 85 "C:\Users\gadjg\source\repos\GitHubProjects\GSCrm\feature\Notifications\GSCrm\Views\InboxNotification\InboxNotifications.cshtml"
                                       Write(resManager.GetString("ESNotifications"));

#line default
#line hidden
#nullable disable
            WriteLiteral("</p>\r\n                    </div>\r\n                </div>\r\n            </div>\r\n");
#nullable restore
#line 89 "C:\Users\gadjg\source\repos\GitHubProjects\GSCrm\feature\Notifications\GSCrm\Views\InboxNotification\InboxNotifications.cshtml"
        }

#line default
#line hidden
#nullable disable
            WriteLiteral("    </div>\r\n");
#nullable restore
#line 91 "C:\Users\gadjg\source\repos\GitHubProjects\GSCrm\feature\Notifications\GSCrm\Views\InboxNotification\InboxNotifications.cshtml"
      
        if (inboxNotViewModels.Count() > 0)
        {
            await Html.RenderPartialAsync("Partial/Navigation", new NavbarRenderSettings
            {
                ItemsCount = inboxNotViewModels.Count(),
                ViewInfo = viewsInfo.Get(currentUserId, INBOX_NOTS),
                ControllerName = INBOX_NOT,
                ActionName = INBOX_NOTS
            });
        }
    

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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<InboxNotificationsViewModel> Html { get; private set; }
    }
}
#pragma warning restore 1591
