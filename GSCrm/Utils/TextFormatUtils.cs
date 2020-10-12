using GSCrm.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace GSCrm.Utils
{
    public static class TextFormatUtils
    {
        public static string GetJsonException(Exception exception)
        {
            return JsonConvert.SerializeObject(new Dictionary<string, string>
            {
                { "Message: ", exception.Message },
                { "InnerException: ", exception.InnerException.Message }
            });
        }
    }
}
