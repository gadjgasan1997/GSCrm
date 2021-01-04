using GSCrm.Data;
using GSCrm.Mapping;
using GSCrm.Models;
using GSCrm.Transactions;
using GSCrm.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GSCrm.Helpers
{
    public static class BaseTypesHelpers
    {
        public static string TrimStartAndEnd(this string @string) => @string?.TrimStart().TrimEnd();

        /// <summary>
        /// Метод отбирает заданное количество символов из строки, начиная с начала и возвращает их в виде строки
        /// </summary>
        /// <param name="string">Строка, значения из которой необходимо получить</param>
        /// <param name="takeCount">Количество символов, которые необходимо отобрать из строки</param>
        /// <returns></returns>
        public static string TakeToString(this string @string, int takeCount)
        {
            string resultString = string.Empty;
            if (string.IsNullOrEmpty(@string) || takeCount <= 0) return resultString;
            for (int index = 0; index < @string.Length; index++)
            {
                resultString += @string[index];
                takeCount--;
                if (takeCount == 0) break;
            }
            return resultString;
        }

        /// <summary>
        /// Методы проверяют массив строк на пустоту
        /// </summary>
        /// <param name="strings"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string[] strings)
        {
            if (strings == null) return true;
            if (strings.Length == 0) return true;
            foreach (string @string in strings)
                if (string.IsNullOrEmpty(@string)) return true;
            return false;
        }

        public static bool AllIsNullOrEmpty(this string[] strings)
        {
            if (strings == null) return true;
            if (strings.Length == 0) return true;
            int emptyStringsCount = 0;
            foreach (string @string in strings)
                if (string.IsNullOrEmpty(@string)) emptyStringsCount++;
            if (emptyStringsCount == strings.Length) return true;
            return false;
        }

        /// <summary>
        /// Методы преобразует список моделей уровня данных в список моделей отображения, предварительно ограничивая их делегатом "limitingFunc"
        /// </summary>
        /// <typeparam name="TViewModel">Тип списка моделей уровня представления</typeparam>
        /// <typeparam name="TDataModel">Тип списка моделей уровня данных</typeparam>
        /// <typeparam name="TMap">Тип преобразователя между уровнем данных и уровнем уотображения</typeparam>
        /// <param name="dataModels">Список моделей уровня данных</param>
        /// <param name="limitingFunc">Действие, ограничивающее коллекцию уровня данных перед ее преобразованием в коллекцию уровня отображения</param>
        /// <returns name="viewModels">Список моделей уровня отображения</returns>
        public static List<TViewModel> MapToViewModels<TDataModel, TViewModel>(
            this List<TDataModel> dataModels,
            IMap<TDataModel, TViewModel> map,
            Func<List<TDataModel>, List<TDataModel>> limitingFunc)
                where TViewModel : BaseViewModel, new()
                where TDataModel : BaseDataModel, new()
        {
            List<TViewModel> viewModels = new List<TViewModel>();
            if (dataModels?.Count > 0)
            {
                limitingFunc(dataModels).ForEach(dataModel =>
                {
                    viewModels.Add(map.DataToViewModel(dataModel));
                });
            }
            return viewModels;
        }

        public static List<TViewModel> MapToViewModels<TDataModel, TViewModel>(
            this List<TDataModel> dataModels,
            IMap<TDataModel, TViewModel> map,
            Func<TDataModel, bool> limitingFunc)
                where TViewModel : BaseViewModel, new()
                where TDataModel : BaseDataModel, new()
        {
            List<TViewModel> viewModels = new List<TViewModel>();
            if (dataModels?.Count > 0)
            {
                dataModels.Where(limitingFunc).ToList().ForEach(dataModel =>
                {
                    viewModels.Add(map.DataToViewModel(dataModel));
                });
            }
            return viewModels;
        }

        /// <summary>
        /// Метод преобразует список моделей уровня данных в список моделей отображения
        /// </summary>
        /// <typeparam name="TDataModel"></typeparam>
        /// <typeparam name="TViewModel"></typeparam>
        /// <typeparam name="TMap"></typeparam>
        /// <param name="dataModels"></param>
        /// <param name="map"></param>
        /// <returns></returns>
        public static List<TViewModel> GetViewModelsFromData<TDataModel, TViewModel>(this List<TDataModel> dataModels, IMap<TDataModel, TViewModel> map)
                where TViewModel : BaseViewModel, new()
                where TDataModel : BaseDataModel, new()
        {
            List<TViewModel> viewModels = new List<TViewModel>();
            if (dataModels?.Count > 0)
            {
                dataModels.ForEach(dataModel =>
                {
                    viewModels.Add(map.DataToViewModel(dataModel));
                });
            }
            return viewModels;
        }

        /// <summary>
        /// Метод добавляет к массиву набор параметров
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array">Исходный массив</param>
        /// <param name="parameters">Добавляемые параметры</param>
        /// <returns></returns>
        public static T[] With<T>(this T[] array, params T[] parameters) => array.Concat(parameters.ToList()).ToArray();
    }
}
