using System.Collections.Generic;
using Microsoft.Bot.Builder.PersonalityChat.Core;

namespace RennovationsBot.Options
{
    /// <summary>
    /// Options to alter the default behaviour of the PersonalityChat Middleware
    /// </summary>
    public class PersonalityChatMiddlewareOptions : PersonalityChatOptions
    {
        public PersonalityChatMiddlewareOptions(
          string subscriptionKey = "",
          PersonalityChatPersona botPersona = PersonalityChatPersona.Friendly,
          bool respondOnlyIfChat = true,
          float scoreThreshold = 0.3F,
          bool endActivityRoutingOnResponse = true,
          Dictionary<string, List<string>> scenarioResponsesMapping = null)
            : base(subscriptionKey, botPersona, scenarioResponsesMapping)
        {
            this.RespondOnlyIfChat = respondOnlyIfChat;
            this.ScoreThreshold = scoreThreshold;
            this.EndActivityRoutingOnResponse = endActivityRoutingOnResponse;
        }

        /// <summary>
        /// If true, personality talk middleware will only respond 
        /// when query is classified as a chat query.
        /// </summary>
        public bool RespondOnlyIfChat { get; private set; }

        /// <summary>
        /// Score threshold of scenario/intents matching to query. Range [0,1]
        /// </summary>
        public float ScoreThreshold { get; private set; }

        /// <summary>
        /// If true then routing of the activity will be stopped when an response is 
        /// successfully returned by the PersonalityChat Middleware
        /// </summary>
        public bool EndActivityRoutingOnResponse { get; private set; }
    }
}
