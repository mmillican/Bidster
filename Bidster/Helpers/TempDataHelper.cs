using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;

namespace Bidster.Helpers
{
    public static class TempDataHelper
    {
        public static void SaveTempData<T>(this ITempDataDictionary tempData, string key, T data) where T : new()
        {
            var serialized = JsonConvert.SerializeObject(data);

            tempData[key] = serialized;
        }

        public static T GetTempData<T>(this ITempDataDictionary tempData, string key)
        {
            var data = tempData[key];
            if (data == null)
                return default(T);

            return JsonConvert.DeserializeObject<T>(data.ToString());
        }
    }
}