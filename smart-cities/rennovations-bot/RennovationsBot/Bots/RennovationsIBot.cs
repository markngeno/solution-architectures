// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RennovationsBot.Dialogs;
using RennovationsBot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RennovationsBot.Bots
{
    /// <summary>
    /// Represents a bot that processes incoming activities.
    /// For each user interaction, an instance of this class is created and the OnTurnAsync method is called.
    /// This is a Transient lifetime service.  Transient lifetime services are created
    /// each time they're requested. For each Activity received, a new instance of this
    /// class is created. Objects that are expensive to construct, or have a lifetime
    /// beyond the single turn, should be carefully managed.
    /// For example, the <see cref="MemoryStorage"/> object and associated
    /// <see cref="IStatePropertyAccessor{T}"/> object are created with a singleton lifetime.
    /// </summary>
    /// <seealso cref="https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-2.1"/>
    public class RennovationsIBot : IBot
    {
        // Supported LUIS Intents
        public const string WhatCanYouDo = "WhatCanYouDo";
        public const string GetRidOfTheFridge = "GetRidOfTheFridge";
        public const string RemodelKitchen = "RemodelKitchen";

        // Supported Events
        private const string EventValidateForm = "validation-error";
        private const string EventSubmitForm = "submit-form";
        private const string EventInactivityForm = "inactivity-form";

        private readonly RennovationsBotAccessors _accessors;
        protected LuisRecognizer _luis;
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="conversationState">The managed conversation state.</param>
        /// <param name="loggerFactory">A <see cref="ILoggerFactory"/> that is hooked to the Azure App Service provider.</param>
        /// <seealso cref="https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-2.1#windows-eventlog-provider"/>
        public RennovationsIBot(ComputerVisionService computerVisionService, RennovationsBotAccessors accessors, ILoggerFactory loggerFactory, LuisRecognizer luisRecognizer)
        {
            _accessors = accessors ?? throw new ArgumentNullException(nameof(accessors));

            // The DialogSet needs a DialogState accessor, it will call it when it has a turn context.
            _dialogs = new DialogSet(accessors.ConversationDialogState);

            if (loggerFactory == null)
            {
                throw new System.ArgumentNullException(nameof(loggerFactory));
            }

            _luis = luisRecognizer;

            _dialogs.Add(new FridgeDisposalDialog(_accessors.GetRidOfTheFridgeState, computerVisionService, loggerFactory));
            _dialogs.Add(new RemodelKitchenDialog(loggerFactory));

            _logger = loggerFactory.CreateLogger<RennovationsIBot>();
            _logger.LogTrace("Turn start.");
        }

        /// <summary>
        /// The <see cref="DialogSet"/> that contains all the Dialogs that can be used at runtime.
        /// </summary>
        private DialogSet _dialogs { get; set; }

        /// <summary>
        /// Every conversation turn for our Rennovations Bot will call this method.
        /// There are no dialogs used, since it's "single turn" processing, meaning a single
        /// request and response.
        /// </summary>
        /// <param name="turnContext">A <see cref="ITurnContext"/> containing all the data needed
        /// for processing this conversation turn. </param>
        /// <param name="cancellationToken">(Optional) A <see cref="CancellationToken"/> that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> that represents the work queued to execute.</returns>
        /// <seealso cref="BotStateSet"/>
        /// <seealso cref="ConversationState"/>
        /// <seealso cref="IMiddleware"/>
        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            var activity = turnContext.Activity;

            // Create a dialog context
            var dc = await _dialogs.CreateContextAsync(turnContext);

            // Handle Message activity type, which is the main activity type for shown within a conversational interface
            // Message activities may contain text, speech, interactive cards, and binary or unknown attachments.
            // see https://aka.ms/about-bot-activity-message to learn more about the message and other activity types
            if (activity.Type == ActivityTypes.Message)
            {
                // Continue the current dialog
                var dialogResult = await dc.ContinueDialogAsync();

                // if no one has responded,
                if (!dc.Context.Responded)
                {
                    // examine results from active dialog
                    switch (dialogResult.Status)
                    {
                        case DialogTurnStatus.Empty:
                            // Perform a call to LUIS to retrieve results for the current activity message.
                            var luisResults = await _luis.RecognizeAsync(dc.Context, cancellationToken).ConfigureAwait(false);
                            var topScoringIntent = luisResults?.GetTopScoringIntent();
                            var topIntent = topScoringIntent.Value.intent;

                            switch (topIntent)
                            {
                                case GetRidOfTheFridge:
                                    await dc.BeginDialogAsync(nameof(FridgeDisposalDialog));
                                    break;

                                case WhatCanYouDo:
                                    await dc.Context.SendActivityAsync("I can help you with a wide range of services related to the Contoso City Council. Please ask me a question to get started");
                                    break;

                                case RemodelKitchen:
                                    await dc.BeginDialogAsync(nameof(RemodelKitchenDialog));
                                    break;

                                default:
                                    await dc.Context.SendActivityAsync("Sorry, I didn't understand that.");
                                    break;
                            }
                            break;

                        case DialogTurnStatus.Waiting:
                            // The active dialog is waiting for a response from the user, so do nothing.
                            break;

                        case DialogTurnStatus.Complete:
                            await dc.EndDialogAsync();
                            break;

                        default:
                            await dc.CancelAllDialogsAsync();
                            break;
                    }
                }

                // Get the conversation state from the turn context.
                var state = await _accessors.GetRidOfTheFridgeState.GetAsync(turnContext, () => new FridgeDisposalState());

                // Set the property using the accessor.
                await _accessors.GetRidOfTheFridgeState.SetAsync(turnContext, state);

                // Save the new turn count into the conversation state.
                await _accessors.ConversationState.SaveChangesAsync(turnContext);
                await _accessors.UserState.SaveChangesAsync(turnContext);
            }
            else if (activity.Type == ActivityTypes.ConversationUpdate)
            {
                if (activity.MembersAdded != null)
                {
                    // Iterate over all new members added to the conversation.
                    foreach (var member in activity.MembersAdded)
                    {
                        if (string.Equals(member.Id, turnContext.Activity.Recipient.Id, StringComparison.OrdinalIgnoreCase))
                        {
                            await dc.Context.SendActivityAsync("Hi, how can I help you today?");
                        }
                    }
                }
            }
            else if (activity.Type == ActivityTypes.Event)
            {
                switch (turnContext.Activity.Name)
                {
                    case EventValidateForm:
                        break;
                    case EventSubmitForm:
                        break;
                    case EventInactivityForm:
                        break;
                }
            }
        }

        private string GetSuggestionForQuestion(int questionNumber)
        {
            return questionNumber == 6 ? "You can just put in an estimate." : "Do you need any help?";
        }

        private Activity CreateHelpCard()
        {
            var actions = new[]
            {
                new CardAction(type: ActionTypes.ImBack, title: "How to get the fair market value?", value: "https://dev.botframework.com/"),
                new CardAction(type: ActionTypes.ImBack, title: "How long does it take to process the permit?", value: "https://dev.botframework.com/"),
                new CardAction(type: ActionTypes.ImBack, title: "Information about the fees", value: "https://dev.botframework.com/")
            };

            var choices = actions.Select(action => new Choice { Action = action, Value = (string)action.Value }).ToList();
            var heroCard = new HeroCard(buttons: actions);
            var activityToSend = (Activity)MessageFactory.Carousel(new[] { heroCard.ToAttachment() }, "This is a list of things that I can help you with:");

            return activityToSend;
        }
    }
}
