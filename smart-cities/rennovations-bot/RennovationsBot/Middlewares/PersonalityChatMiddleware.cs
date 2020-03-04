using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.PersonalityChat;
using Microsoft.Bot.Builder.PersonalityChat.Core;
using Microsoft.Bot.Schema;
using RennovationsBot.Services;

namespace RennovationsBot.Middlewares
{
    public class PersonalityChatMiddleware : IMiddleware
    {
        private readonly Services.PersonalityChatService personalityChatService;
        private readonly RennovationsBot.Options.PersonalityChatMiddlewareOptions personalityChatMiddlewareOptions;
        private readonly Random randomGenerator;

        public PersonalityChatMiddleware(RennovationsBot.Options.PersonalityChatMiddlewareOptions personalityChatMiddlewareOptions)
        {
            this.personalityChatMiddlewareOptions = personalityChatMiddlewareOptions ?? throw new ArgumentNullException(nameof(personalityChatMiddlewareOptions));
            this.personalityChatService = new Services.PersonalityChatService(personalityChatMiddlewareOptions);
            this.randomGenerator = new Random();
        }

        public async Task OnTurnAsync(ITurnContext context, NextDelegate next, CancellationToken cancellationToken)
        {
          try
          {
              if (context.Activity.Type == ActivityTypes.Message)
              {
                  var messageActivity = context.Activity.AsMessageActivity();
                  if (!string.IsNullOrEmpty(messageActivity.Text))
                  {
                      var results = await this.personalityChatService.QueryServiceAsync(messageActivity.Text.Trim()).ConfigureAwait(false);

                      if ((!this.personalityChatMiddlewareOptions.RespondOnlyIfChat || results.IsChatQuery) && results.ScenarioList.Count() > 0)
                      {
                          string personalityChatResponse = this.GetResponse(results);
                          await this.PostPersonalityChatResponseToUserAsync(context, next, personalityChatResponse);
                          if (this.personalityChatMiddlewareOptions.EndActivityRoutingOnResponse)
                          {
                              // Query is answered, don't keep routing
                              return;
                          }
                      }
                  }
              }
          }
          catch (Exception e)
          {
              System.Console.WriteLine(e.Message);
          }

          await next(cancellationToken);
        }

        public virtual string GetResponse(PersonalityChatResults personalityChatResults)
        {
            var matchedScenarios = personalityChatResults?.ScenarioList?.Where(scenario =>
            {
                return scenario.Responses != null && scenario.Responses.Count > 0 && scenario.Score > this.personalityChatMiddlewareOptions.ScoreThreshold;
            });

            if (matchedScenarios != null && matchedScenarios.Count() > 0)
            {
                var randomIndex = randomGenerator.Next(matchedScenarios.Count());
                var scenario = matchedScenarios.ElementAt(randomIndex);
                return scenario.Responses[randomGenerator.Next(scenario.Responses.Count())];
            }

            return string.Empty;
        }

        public virtual async Task PostPersonalityChatResponseToUserAsync(ITurnContext context, NextDelegate next, string personalityChatResponse)
        {
            if (!string.IsNullOrEmpty(personalityChatResponse))
            {
                await context.SendActivityAsync(personalityChatResponse).ConfigureAwait(false);
            }
        }
  }
}
