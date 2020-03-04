using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CustomSkills.Services
{
    public static class TranslationService
    {
        [SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", Justification = "Avoiding Improper Instantiation antipattern : https://docs.microsoft.com/en-us/azure/architecture/antipatterns/improper-instantiation/")]
        private static readonly HttpClient Client = new HttpClient();

        static readonly string path = "https://api.cognitive.microsofttranslator.com/translate?api-version=3.0";

        /// <summary>
        /// Use Cognitive Service to translate text from one language to another.
        /// </summary>
        /// <param name="key">Service key to use on the call.</param>
        /// <param name="region">Service region to use on the call.</param>
        /// <param name="originalText">The text to translate.</param>
        /// <param name="toLanguage">The language you want to translate to.</param>
        /// <returns>Asynchronous task that returns the translated text. </returns>
        public async static Task<string> TranslateText(string key, string region, string originalText, string toLanguage)
        {
            var body = new Object[] { new { Text = originalText } };
            var requestBody = JsonConvert.SerializeObject(body);

            var uri = $"{path}&to={toLanguage}";

            string result = string.Empty;

            using (var request = new HttpRequestMessage())
            {
                request.Method = HttpMethod.Post;
                request.RequestUri = new Uri(uri);
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                request.Headers.Add("Ocp-Apim-Subscription-Key", key);
                request.Headers.Add("Ocp-Apim-Subscription-Region", region);

                var response = await Client.SendAsync(request);
                var responseBody = await response.Content.ReadAsStringAsync();

                dynamic data = JsonConvert.DeserializeObject(responseBody);
                return data?.First?.translations?.First?.text?.Value as string;
            }
        }
    }
}
