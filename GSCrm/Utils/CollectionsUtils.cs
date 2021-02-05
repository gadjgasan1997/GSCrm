﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace GSCrm.Utils
{
    /// <summary>
    /// Утилиты для работы с коллекциями
    /// </summary>
    public static class CollectionsUtils
    {
        /// <summary>
        /// Метод ограничивает коллекцию, выполняя ряд повторяющихся действий
        /// </summary>
        /// <typeparam name="TCollectionToLimitItem">Тип, из элементов которого будет состоять выходная, ограниченная коллекция "collectionToLimit"</typeparam>
        /// <typeparam name="TLimitingCollectionItem">Тип, из элементов которого будет состоять коллекция, элементами которой будет ограничиваться поданная на вход коллекция "collectionToLimit"</typeparam>
        /// <typeparam name="TLimitingCollectionPropType">Тип свойства, которое будет выбираться из коллекции "limitingCollection" для ограничения коллекци "collectionToLimit"</typeparam>
        /// <param name="collectionToLimit">Коллекция, элементы которой необходимо ограничить</param>
        /// <param name="limitingCollection">Коллекция, элементами которой будет ограничиваться поданная на вход коллекция "collectionToLimit"</param>
        /// <param name="limitCondition">Условие, по которому будет ограничиваться коллекция "limitingCollection"</param>
        /// <param name="selectCondition">Условие, по которому будут отбираться свойства из коллекции "limitingCollection"</param>
        /// <param name="removeCondition">Условие, по которому будут удаляться элементы из ограничивающейся коллекции "collectionToLimit"</param>
        public static void TransformCollection<TCollectionToLimitItem, TLimitingCollectionItem, TLimitingCollectionPropType>(
            ref List<TCollectionToLimitItem> collectionToLimit,
            List<TLimitingCollectionItem> limitingCollection,
            Func<TLimitingCollectionItem, bool> limitCondition,
            Func<TLimitingCollectionItem, TLimitingCollectionPropType> selectCondition,
            Func<List<TLimitingCollectionPropType>, TCollectionToLimitItem, bool> removeCondition)
        {
            // Проверка, что в коллекции, которую необходимо ограничить("collectionToLimit"), присутствуют элементы и на то, что
            // коллекция, элементами которой необходимо ограничить("limitingCollection") не пустая
            if (collectionToLimit?.Count > 0 && limitingCollection != null)
            {
                // Если коллекция, элементами которой будет ограничиваться выходная пуста, то в выходную записывается пустая коллекция
                if (limitingCollection.Count == 0)
                    collectionToLimit = new List<TCollectionToLimitItem>();
                else
                {
                    limitingCollection = limitingCollection.Where(limitCondition).ToList();
                    if (limitingCollection.Count > 0)
                    {
                        List<TLimitingCollectionPropType> limitingCollectionProps = limitingCollection.Select(selectCondition).ToList();
                        List<TCollectionToLimitItem> itemsToRemove = new List<TCollectionToLimitItem>();
                        collectionToLimit.ForEach(collectionItem =>
                        {
                            if (removeCondition(limitingCollectionProps, collectionItem))
                                itemsToRemove.Add(collectionItem);
                        });

                        foreach (TCollectionToLimitItem itemToRemove in itemsToRemove)
                            collectionToLimit.Remove(itemToRemove);
                    }
                    else collectionToLimit = new List<TCollectionToLimitItem>();
                }
            }
        }

        /// <summary>
        /// Метод вызывает список обработчиков, которые выполняют определенные действия, и, в случае обнаружения, тут же выходят
        /// </summary>
        /// <param name="actions"></param>
        public static void InvokeIntermittinActions(Dictionary<string, string> errors, IEnumerable<Action> actions)
        {
            if (errors.Any()) return;
            foreach (Action action in actions)
            {
                action();
                if (errors.Any()) break;
            }
        }

        /// <summary>
        /// Метод вызывает список обработчиков, которые выполняют определенные действия, и, в случае обнаружения, тут же выходят
        /// Также эта перегрузка выходит при передаче флага "needStop" со значением "true"
        /// </summary>
        /// <param name="errors"></param>
        /// <param name="needStop"></param>
        /// <param name="actions"></param>
        public static void InvokeIntermittinActions(Dictionary<string, string> errors, bool needStop, IEnumerable<Action> actions)
        {
            if (needStop || errors.Any()) return;
            foreach (Action action in actions)
            {
                action();
                if (errors.Any()) break;
            }
        }

        /// <summary>
        /// Метод вызывает список обработчиков, которые выполняют определенные действия, и не прерываются при их обнаружении
        /// </summary>
        /// <param name="actions"></param>
        public static void InvokeAllChecks(IEnumerable<Action> actions)
            => actions.ToList().ForEach(checkHandler => checkHandler());
    }
}
