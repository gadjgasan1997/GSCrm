using GSCrm.Models;
using System;
using Newtonsoft.Json;

namespace GSCrm.Helpers
{
    public static class InboxNotificationHelpers
    {
        private static readonly JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        public static void WriteObjectToAttr3<TEntity>(this InboxNotification inboxNot, TEntity entity) where TEntity : IMainEntity
            => inboxNot.WriteObjectToAttr(entity, "Attrib3");
        public static TEntity ReadObjectFromAttr3<TEntity>(this InboxNotification inboxNot) where TEntity : class, IMainEntity
            => inboxNot.ReadObjectFromAttr<TEntity>("Attrib3");
        public static void WriteObjectToAttr4<TEntity>(this InboxNotification inboxNot, TEntity entity) where TEntity : IMainEntity
            => inboxNot.WriteObjectToAttr(entity, "Attrib4");
        public static TEntity ReadObjectFromAttr4<TEntity>(this InboxNotification inboxNot) where TEntity : class, IMainEntity
            => inboxNot.ReadObjectFromAttr<TEntity>("Attrib4");

        /// <summary>
        /// Метод десериализует объект типа "TEntity", беря данные из указанного атрибута
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="inboxNot"></param>
        /// <param name="attrName"></param>
        /// <returns></returns>
        private static TEntity ReadObjectFromAttr<TEntity>(this InboxNotification inboxNot, string attrName)
            where TEntity : class, IMainEntity
            => attrName switch
            {
                "Attrib3" => JsonConvert.DeserializeObject<TEntity>(inboxNot.Attrib3),
                "Attrib4" => JsonConvert.DeserializeObject<TEntity>(inboxNot.Attrib4),
                _ => null
            };

        /// <summary>
        /// Метод сериализует и записывает обхект в указанный атрибут
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="inboxNot"></param>
        /// <param name="entity"></param>
        /// <param name="attrName"></param>
        private static void WriteObjectToAttr<TEntity>(this InboxNotification inboxNot, TEntity entity, string attrName)
            where TEntity : IMainEntity
        {
            try
            {
                string @string = JsonConvert.SerializeObject(entity, Formatting.Indented, jsonSerializerSettings);
                switch (attrName)
                {
                    case "Attrib3":
                        inboxNot.Attrib3 = @string;
                        return;
                    case "Attrib4":
                        inboxNot.Attrib4 = @string;
                        return;
                }
            }
            catch(Exception ex)
            {
#if DEBUG
                throw ex;
#endif
            }
        }
    }
}
