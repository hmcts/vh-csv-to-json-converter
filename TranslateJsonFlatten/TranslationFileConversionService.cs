using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;

namespace TranslateJsonFlatten
{
    public class TranslationFileConversionService
    {
        public IEnumerable<string> ConvertToCsv(string json, string headers = null)
        {
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                var config = new ConfigurationBuilder()
                    .AddJsonStream(ms)
                    .Build();

                var outputCsv = new List<string>();
                if (headers != null)
                {
                    outputCsv.Add(headers);
                }

                outputCsv.AddRange(config.AsEnumerable().Where(kvp => !string.IsNullOrEmpty(kvp.Value)).Select(kvp => $"{kvp.Key},\"{kvp.Value}\""));
                return outputCsv;
            }
        }

        public string ConvertToJson(string[] csvRows)
        {
            var rowsList = csvRows.ToList();
            rowsList.RemoveAt(0);
            dynamic dynamicObject = new ExpandoObject();
            foreach (var line in rowsList)
            {
                IDictionary<string, object> dynamicDictionary = dynamicObject;
                var key = line.Split(',')[0];
                var segments = key.Split(':');
                for (var i = 0; i < segments.Length - 1; i++)
                {
                    if (!dynamicDictionary.ContainsKey(segments[i]))
                    {
                        dynamicDictionary.Add(segments[i], new ExpandoObject());
                    }
                    dynamicDictionary = dynamicDictionary[segments[i]] as ExpandoObject;
                }

                //var value = line.Replace($"{key},", string.Empty);                
                //value = value.Trim(',');
                //var value = line.Split(',')[1];

                int index = line.IndexOf(',', line.IndexOf(','));
                var value = line.Substring(index + 1).Replace("\"", string.Empty);

                dynamicDictionary.Add(segments[segments.Length - 1], value);
            }

            return JsonConvert.SerializeObject(dynamicObject, Formatting.Indented);
        }
    }
}
