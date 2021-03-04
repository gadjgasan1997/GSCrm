﻿using System.Text;
using System.Collections.Generic;
using GSCrm.Models;
using GSCrm.Routing.Middleware.AccessibilityMiddleware;
using Newtonsoft.Json;
using Microsoft.Extensions.Primitives;
using GSCrm.Data.Cash;
using GSCrm.Localization;
using Microsoft.Extensions.DependencyInjection;
using GSCrm.Models.ViewModels;
using GSCrm.Models.Enums;
using System;

namespace GSCrm.Helpers
{
    public static class MiddlewareHelpers
    {
        #region Declarations
        /// <summary>
        /// Настройки сериализации
        /// </summary>
        private static readonly JsonSerializerSettings _settings
            = new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
        /// <summary>
        /// Название ключа, в котором хранится id организации
        /// </summary>
        private const string ORGANIZATION_ID_KEY_NAME = "organizationId";
        /// <summary>
        /// Название ключа, в котором хранится id должности
        /// </summary>
        private const string POSITION_ID_KEY_NAME = "id";
        /// <summary>
        /// Название ключа, в котором хранится id сотрудника
        /// </summary>
        private const string EMPLOYEE_ID_KEY_NAME = "id";
        /// <summary>
        /// Название ключа, в котором хранится id полномочия
        /// </summary>
        private const string RESPONSIBILITY_ID_KEY_NAME = "id";
        /// <summary>
        /// Название ключа, в котором хранится id клиента
        /// </summary>
        private const string ACCOUNT_ID_KEY_NAME = "id";

        /// <summary>
        /// Словарь с ошибками по умолчанию и их обработкой
        /// </summary>
        private static readonly Dictionary<int, Dictionary<string, string>> _defaultErrors
            = new Dictionary<int, Dictionary<string, string>>()
            {
                { 400, new Dictionary<string, string>()
                {
                    { "UnhandledException", string.Empty }
                } },
                { 404, new Dictionary<string, string>()
                {
                    { "RecordNotFound", string.Empty }
                } }
            };
        #endregion

        /// <summary>
        /// Метод возвращает ошибку по умолчанию
        /// </summary>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        private static Dictionary<string, string> GetDefaultError(int statusCode)
            => _defaultErrors.ContainsKey(statusCode) ? _defaultErrors[statusCode] : _defaultErrors[400];

        /// <summary>
        /// Метод перенаправляет запрос на другой url
        /// </summary>
        /// <param name="redirectUrl"></param>
        /// <param name="accessibilityHandlerData"></param>
        public static void Redirect(this AccessibilityHandlerData accessibilityHandlerData, string redirectUrl)
        {
            accessibilityHandlerData.NeedBreakRequest = true;
            accessibilityHandlerData.HttpContext.Response.Redirect(redirectUrl);
        }

        /// <summary>
        /// Метод останавливает запрос и возвращает ошибку
        /// </summary>
        /// <param name="accessibilityHandlerData"></param>
        /// <param name="statusCode">Код ошибки, по умолчанию равный 400</param>
        /// <param name="errorData">Сведения об ошибке</param>
        public static void BreakRequest(this AccessibilityHandlerData accessibilityHandlerData, int statusCode = 400, object errorData = null)
        {
            accessibilityHandlerData.NeedBreakRequest = true;
            accessibilityHandlerData.ErrorHandler = async () =>
            {
                var result = JsonConvert.SerializeObject(errorData ?? GetDefaultError(statusCode), _settings);
                accessibilityHandlerData.HttpContext.Response.StatusCode = statusCode;
                await accessibilityHandlerData.HttpContext.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(result));
            };
        }

        public static User GetCurrentUser(this AccessibilityHandlerData accessibilityHandlerData)
            => accessibilityHandlerData.HttpContext.GetCurrentUser(accessibilityHandlerData.Context);

        /// <summary>
        /// Метод устанавливает организацию в кеше как текущую, на которой находится пользователь, в случае неудачи прерывает запрос с ошибкой
        /// Организация уже должна быть в кеше на момент вызова метода
        /// </summary>
        /// <param name="accessibilityHandlerData"></param>
        /// <param name="requestSourceType">Источник запроса, в котором будет находиться id организации</param>
        /// <param name="organizationIdKeyName">Название ключа, в котором хранится id организации</param>
        public static void CacheCurrentOrganization(this AccessibilityHandlerData accessibilityHandlerData, RequestSourceType requestSourceType = RequestSourceType.Form, string organizationIdKeyName = ORGANIZATION_ID_KEY_NAME)
        {
            string organizationId = GetIdFromRequest(accessibilityHandlerData, requestSourceType, organizationIdKeyName);
            if (!string.IsNullOrEmpty(organizationId))
            {
                User currentUser = accessibilityHandlerData.GetCurrentUser();
                ICachService cachService = accessibilityHandlerData.ServiceProvider.GetService<ICachService>();
                if (cachService.TryGetCachedEntity(currentUser, organizationId, out Organization organization) &&
                    cachService.TryGetCachedEntity(currentUser, organizationId, out OrganizationViewModel organizationViewModel))
                {
                    cachService.CacheCurrentEntity(currentUser, organization);
                    cachService.CacheCurrentEntity(currentUser, organizationViewModel);
                    return;
                }
            }

            IResManager resManager = accessibilityHandlerData.ServiceProvider.GetService<IResManager>();
            accessibilityHandlerData.BreakRequest(404, new
            {
                RecordNotFound = resManager.GetString("OrganizationNotFound")
            });
        }

        /// <summary>
        /// Метод устанавливает должность в кеше как текущую, на которой находится пользователь, в случае неудачи прерывает запрос с ошибкой
        /// Должность уже должна быть в кеше на момент вызова метода
        /// </summary>
        /// <param name="accessibilityHandlerData"></param>
        /// <param name="requestSourceType">Источник запроса, в котором будет находиться id должности</param>
        /// <param name="positionIdKeyName">Название ключа, в котором хранится id должности</param>
        public static void CacheCurrentPosition(this AccessibilityHandlerData accessibilityHandlerData, RequestSourceType requestSourceType = RequestSourceType.Form, string positionIdKeyName = POSITION_ID_KEY_NAME)
        {
            string positionId = GetIdFromRequest(accessibilityHandlerData, requestSourceType, positionIdKeyName);
            if (!string.IsNullOrEmpty(positionId))
            {
                User currentUser = accessibilityHandlerData.GetCurrentUser();
                ICachService cachService = accessibilityHandlerData.ServiceProvider.GetService<ICachService>();
                if (cachService.TryGetCachedEntity(currentUser, positionId, out Position position) &&
                    cachService.TryGetCachedEntity(currentUser, positionId, out PositionViewModel positionViewModel))
                {
                    cachService.CacheCurrentEntity(currentUser, position);
                    cachService.CacheCurrentEntity(currentUser, positionViewModel);
                    return;
                }
            }

            IResManager resManager = accessibilityHandlerData.ServiceProvider.GetService<IResManager>();
            accessibilityHandlerData.BreakRequest(404, new
            {
                RecordNotFound = resManager.GetString("PositionNotFound")
            });
        }

        /// <summary>
        /// Метод устанавливает сотрудника в кеше как текущего, на котором находится пользователь, в случае неудачи прерывает запрос с ошибкой
        /// Сотрудник уже должен быть в кеше на момент вызова метода
        /// </summary>
        /// <param name="accessibilityHandlerData"></param>
        /// <param name="requestSourceType">Источник запроса, в котором будет находиться id сотрудника</param>
        /// <param name="employeeIdKeyName">Название ключа, в котором хранится id сотрудника</param>
        public static void CacheCurrentEmployee(this AccessibilityHandlerData accessibilityHandlerData, RequestSourceType requestSourceType = RequestSourceType.Form, string employeeIdKeyName = EMPLOYEE_ID_KEY_NAME)
        {
            string employeeId = GetIdFromRequest(accessibilityHandlerData, requestSourceType, employeeIdKeyName);
            if (!string.IsNullOrEmpty(employeeId))
            {
                User currentUser = accessibilityHandlerData.GetCurrentUser();
                ICachService cachService = accessibilityHandlerData.ServiceProvider.GetService<ICachService>();
                if (cachService.TryGetCachedEntity(currentUser, employeeId, out Employee employee) &&
                    cachService.TryGetCachedEntity(currentUser, employeeId, out EmployeeViewModel employeeViewModel))
                {
                    cachService.CacheCurrentEntity(currentUser, employee);
                    cachService.CacheCurrentEntity(currentUser, employeeViewModel);
                    return;
                }
            }

            IResManager resManager = accessibilityHandlerData.ServiceProvider.GetService<IResManager>();
            accessibilityHandlerData.BreakRequest(404, new
            {
                RecordNotFound = resManager.GetString("EmployeeNotFound")
            });
        }

        /// <summary>
        /// Метод устанавливает полномочие в кеше как текущее, на котором находится пользователь, в случае неудачи прерывает запрос с ошибкой
        /// Полномочие уже должно быть в кеше на момент вызова метода
        /// </summary>
        /// <param name="accessibilityHandlerData"></param>
        /// <param name="requestSourceType">Источник запроса, в котором будет находиться id полномочия</param>
        /// <param name="responsibilityIdKeyName">Название ключа, в котором хранится id полномочия</param>
        public static void CacheCurrentResponsibility(this AccessibilityHandlerData accessibilityHandlerData, RequestSourceType requestSourceType = RequestSourceType.Form, string responsibilityIdKeyName = RESPONSIBILITY_ID_KEY_NAME)
        {
            string responsibilityId = GetIdFromRequest(accessibilityHandlerData, requestSourceType, responsibilityIdKeyName);
            if (!string.IsNullOrEmpty(responsibilityId))
            {
                User currentUser = accessibilityHandlerData.GetCurrentUser();
                ICachService cachService = accessibilityHandlerData.ServiceProvider.GetService<ICachService>();
                if (cachService.TryGetCachedEntity(currentUser, responsibilityId, out Responsibility responsibility) &&
                    cachService.TryGetCachedEntity(currentUser, responsibilityId, out ResponsibilityViewModel responsibilityViewModel))
                {
                    cachService.CacheCurrentEntity(currentUser, responsibility);
                    cachService.CacheCurrentEntity(currentUser, responsibilityViewModel);
                    return;
                }
            }

            IResManager resManager = accessibilityHandlerData.ServiceProvider.GetService<IResManager>();
            accessibilityHandlerData.BreakRequest(404, new
            {
                RecordNotFound = resManager.GetString("ResponsibilityNotFound")
            });
        }

        /// <summary>
        /// Метод устанавливает клиента в кеше как текущего, на котором находится пользователь, в случае неудачи прерывает запрос с ошибкой
        /// Клиент уже должен быть в кеше на момент вызова метода
        /// </summary>
        /// <param name="accessibilityHandlerData"></param>
        /// <param name="cachService"></param>
        /// <param name="resManager"></param>
        /// <param name="currentUser"></param>
        /// <param name="acccountId"></param>
        public static bool TryCacheCurrentAccount(this AccessibilityHandlerData accessibilityHandlerData, ICachService cachService, IResManager resManager, User currentUser, Guid acccountId)
        {
            // Попытка получить клиента из кеша
            if (cachService.TryGetCachedEntity(currentUser, acccountId, out Account account) &&
                cachService.TryGetCachedEntity(currentUser, acccountId, out AccountViewModel accountViewModel))
            {
                // Кеширование клиента как текущего
                cachService.CacheEntity(currentUser, account);
                cachService.CacheCurrentEntity(currentUser, account);
                cachService.CacheEntity(currentUser, accountViewModel);
                cachService.CacheCurrentEntity(currentUser, accountViewModel);
                return true;
            }

            accessibilityHandlerData.BreakRequest(404, new
            {
                RecordNotFound = resManager.GetString("AccounNotFound")
            });
            return false;
        }

        /// <summary>
        /// Метод устанавливает клиента в кеше как текущего, на котором находится пользователь, в случае неудачи прерывает запрос с ошибкой
        /// Клиент уже должен быть в кеше на момент вызова метода
        /// </summary>
        /// <param name="accessibilityHandlerData"></param>
        /// <param name="requestSourceType">Источник запроса, в котором будет находиться id клиента</param>
        /// <param name="accountIdKeyName">Название ключа, в котором хранится id клиента</param>
        public static bool TryCacheCurrentAccount(this AccessibilityHandlerData accessibilityHandlerData, RequestSourceType requestSourceType = RequestSourceType.Form, string accountIdKeyName = ACCOUNT_ID_KEY_NAME)
        {
            string accountId = GetIdFromRequest(accessibilityHandlerData, requestSourceType, accountIdKeyName);
            if (!string.IsNullOrEmpty(accountId))
            {
                User currentUser = accessibilityHandlerData.GetCurrentUser();
                ICachService cachService = accessibilityHandlerData.ServiceProvider.GetService<ICachService>();
                if (cachService.TryGetCachedEntity(currentUser, accountId, out Account account) &&
                    cachService.TryGetCachedEntity(currentUser, accountId, out AccountViewModel accountViewModel))
                {
                    cachService.CacheCurrentEntity(currentUser, account);
                    cachService.CacheCurrentEntity(currentUser, accountViewModel);
                    return true;
                }
            }

            IResManager resManager = accessibilityHandlerData.ServiceProvider.GetService<IResManager>();
            accessibilityHandlerData.BreakRequest(404, new
            {
                RecordNotFound = resManager.GetString("AccounNotFound")
            });
            return false;
        }

        /// <summary>
        /// Метод возвращает id сущности из запроса
        /// </summary>
        /// <param name="accessibilityHandlerData"></param>
        /// <param name="requestSourceType">Источник, где необходимо искать id сущности</param>
        /// <param name="keyName">Название ключа, в котором хранится id сущности</param>
        /// <returns></returns>
        public static string GetIdFromRequest(this AccessibilityHandlerData accessibilityHandlerData, RequestSourceType requestSourceType, string keyName)
            => requestSourceType switch
            {
                RequestSourceType.Form => accessibilityHandlerData.Form.TryGetValue(keyName, out StringValues stringValue) switch
                {
                    true => stringValue,
                    _ => string.Empty
                },
                RequestSourceType.RouteValues => accessibilityHandlerData.RouteValues.TryGetValue(keyName, out object objectValue) switch
                {
                    true => objectValue.ToString(),
                    _ => string.Empty
                },
                _ => string.Empty
            };
    }
}