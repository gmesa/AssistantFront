using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace AccountingAssistant.Extensions
{
    public static class JsonExtensions
    {
        private static JsonSerializerOptions _settings;

        private static JsonSerializerOptions Settings
        {
            get
            {
                object obj = _settings;
                if (obj == null)
                {
                    obj = new JsonSerializerOptions()
                    {
                        PropertyNameCaseInsensitive = true,
                    };                    
                }
                return (JsonSerializerOptions)obj;
            }
        }
        public static string ToJson(this object obj, JsonSerializerOptions settings = null)
        {
            return JsonSerializer.Serialize(obj, settings ?? Settings);
        }

        public static T FromJson<T>(this string json, JsonSerializerOptions settings = null)
        {
            return JsonSerializer.Deserialize<T>(json, settings ?? Settings);
            
        }
    }

    public static class StringExtensions {

        public static StringContent AsContent(this string content, string mediaType = "application/json")
        {
            return new StringContent(content, Encoding.UTF8, mediaType);
        }

        /// <summary>
        /// Return paragraphs separated by Return-NewLine surrounded by html paragrapg tag
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ConvertToHtmlParagraph(this string s)
        {
            return s != null ? Regex.Replace(s, "(?<par>.+)", "<p>${par}</p>", RegexOptions.Multiline).Replace("<p><p>", "<p>") : null;

        }

        public static string CustomCloneString(this string s) {

            char[] chars = new char[s.Length];
            s.CopyTo(0, chars, 0, s.Length);
            return chars.ToString();
        }
    }
}
