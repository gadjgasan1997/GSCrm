using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using static GSCrm.CommonConsts;

namespace GSCrm.Utils
{
    public static class AppUtils
    {
        private static string[] allAppLanguages;
        private static readonly Dictionary<LangType, JArray> countries = new Dictionary<LangType, JArray>()
        {
            { LangType.enEN, new JArray() },
            { LangType.ruRU, new JArray() }
        };

        private static readonly Dictionary<string, Dictionary<LangType, string>> locationsPrfxs = new Dictionary<string, Dictionary<LangType, string>>()
        {
            { REGION_KEY, new Dictionary<LangType, string>{ } },
            { CITY_KEY, new Dictionary<LangType, string>{ } },
            { STREET_KEY, new Dictionary<LangType, string>{ } },
            { HOUSE_KEY, new Dictionary<LangType, string>{ } }
        };

        /// <summary>
        /// Метод читает json файл с информацией о местоположениях
        /// </summary>
        public static void InitializeLocations()
        {
            try
            {
                allAppLanguages = Enum.GetNames(typeof(LangType));
                using JsonTextReader reader = new JsonTextReader(new StreamReader("Locations.json"));
                JToken json = JToken.Load(reader);
                JObject mainTag = json.Value<JObject>();
                InitializeCountries(mainTag);
                InitializeLocationsPrfxs(mainTag);
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Метод записывает список стран в поле "countries"
        /// </summary>
        /// <param name="mainTag"></param>
        private static void InitializeCountries(JObject mainTag)
        {
            if (mainTag.TryGetValue("countries", out JToken countriesToken))
            {
                JObject countriesObj = (JObject)countriesToken;
                foreach (string language in allAppLanguages)
                {
                    if (countriesObj.TryGetValue(language, out JToken langToken))
                    {
                        LangType currentLang = (LangType)Enum.Parse(typeof(LangType), language);
                        if (countries.ContainsKey(currentLang))
                            countries[currentLang] = (JArray)langToken;
                        else countries.Add(currentLang, (JArray)langToken);
                    }
                }
            }
        }

        /// <summary>
        /// Метод заполняет поле "locationsPrfxs" префиксами
        /// </summary>
        /// <param name="mainTag"></param>
        private static void InitializeLocationsPrfxs(JObject mainTag)
        {
            if (mainTag.TryGetValue("locationsPrfxs", out JToken prefixesToken))
            {
                JObject prefixesObj = (JObject)prefixesToken;
                if (prefixesObj.TryGetValue(REGION_KEY, out JToken regionToken))
                    AddPrfxs(regionToken, REGION_KEY);

                if (prefixesObj.TryGetValue(CITY_KEY, out JToken cityToken))
                    AddPrfxs(cityToken, CITY_KEY);

                if (prefixesObj.TryGetValue(STREET_KEY, out JToken streetToken))
                    AddPrfxs(streetToken, STREET_KEY);

                if (prefixesObj.TryGetValue(HOUSE_KEY, out JToken houseToken))
                    AddPrfxs(houseToken, HOUSE_KEY);
            }
        }

        /// <summary>
        /// Метод добавляет все значения префикса по языкам в словарь "locationsPrfxs"
        /// </summary>
        /// <param name="prefixToken">Токен с префиксом</param>
        /// <param name="prefixName">Название токена</param>
        private static void AddPrfxs(JToken prefixToken, string prefixName)
        {
            JObject prefixObj = (JObject)prefixToken;
            if (!string.IsNullOrEmpty(prefixName))
            {
                foreach (string language in allAppLanguages)
                {
                    if (prefixObj.TryGetValue(language, out JToken langToken))
                    {
                        LangType currentLang = (LangType)Enum.Parse(typeof(LangType), language);
                        if (locationsPrfxs[prefixName].ContainsKey(currentLang))
                            locationsPrfxs[prefixName][currentLang] = (string)langToken;
                        else locationsPrfxs[prefixName].Add(currentLang, (string)langToken);
                    }
                }
            }
        }

        public static JArray GetCountries(LangType? langType)
        {
            LangType lang = langType ?? LangType.enEN;
            if (!countries.ContainsKey(lang))
                return new JArray();
            return countries[lang];
        }

        public static string GetLocationPrefix(string prefixName, LangType? langType)
        {
            LangType lang = langType ?? LangType.ruRU;
            if (!locationsPrfxs[prefixName].ContainsKey(lang))
                return string.Empty;
            return locationsPrfxs[prefixName][lang];
        }
    }
}
