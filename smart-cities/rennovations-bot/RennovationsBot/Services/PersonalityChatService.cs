using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.PersonalityChat.Core;
using Newtonsoft.Json;

namespace RennovationsBot.Services
{
    /// <summary>
    /// PersonalityChatService for calling API and parsing the result.
    /// </summary>
    [Serializable]
    public sealed class PersonalityChatService
    {
        /// <summary>
        /// The base URI for accessing PersonalityChat Service.
        /// </summary>
        public const string UriBase = "https://smarttalk.azure-api.net/api/v1/botframework";

        private readonly PersonalityChatOptions personalityChatOptions;

        /// <summary>
        /// Build the query uri for the query text.
        /// </summary>
        /// <param name="personalityChatOptions">Construct the PersonalityChat Service using personalitychat information.</param>
        /// <returns>PersonalityChat service instance</returns>
        public PersonalityChatService(PersonalityChatOptions personalityChatOptions)
        {
            this.personalityChatOptions = personalityChatOptions;
        }

        /// <summary>
        /// Query the PersonalityChat service using query.
        /// </summary>
        /// <returns>The PersonalityChat service results.</returns>
        public async Task<PersonalityChatResults> QueryServiceAsync(string query)
        {
            Uri uri = new UriBuilder($"{UriBase}").Uri;

            string responseJson = string.Empty;

            using (HttpClient client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromMilliseconds(5000);
                PersonalityChatRequest personalityChatRequest = new PersonalityChatRequest(query, this.personalityChatOptions.BotPersona);

                string requestJson = JsonConvert.SerializeObject(personalityChatRequest);

                StringContent requestBody = new StringContent(requestJson, Encoding.UTF8, "application/json");
                HttpResponseMessage response = null;

                try
                {
                    response = await client.PostAsync(uri, requestBody);
                }
                catch (Exception)
                {
                    throw new Exception("Http call to personalityChat service timed out.");
                }

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception("Http call to personalityChat service failed with status code: " + response.StatusCode);
                }
                else if (response == null || response.Content == null)
                {
                    throw new Exception("Http call to personalityChat service returned null.");
                }
                else
                {
                    responseJson = await response.Content.ReadAsStringAsync();
                }
            }

            try
            {
                var personalityChatResults = JsonConvert.DeserializeObject<PersonalityChatResults>(responseJson);

                if (this.personalityChatOptions.scenarioResponse != null)
                {
                    foreach (var scenario in personalityChatResults.ScenarioList)
                    {
                        if (this.personalityChatOptions.scenarioResponse.ContainsKey(scenario.ScenarioName))
                        {
                            scenario.Responses = this.personalityChatOptions.scenarioResponse[scenario.ScenarioName];
                        }
                        else
                        {
                            scenario.Responses = new List<string>();
                        }
                    }
                }

                return personalityChatResults;
            }
            catch (JsonException ex)
            {
                throw new Exception("Unable to deserialize the personalityChat service response.", ex);
            }
        }
    }
}
