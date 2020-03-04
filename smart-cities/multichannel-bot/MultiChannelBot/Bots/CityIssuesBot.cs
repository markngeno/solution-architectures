// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Configuration;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using MultiChannelBot.Dialogs;
using MultiChannelBot.Models.Options;
using MultiChannelBot.Options;
using MultiChannelBot.Services.Interfaces;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MultiChannelBot
{
    /// <summary>
    /// Main entry point and orchestration for bot.
    /// </summary>
    public class CityIssuesBot : IBot
    {
        // Supported LUIS Intents
        public const string WhatCanYouDo = "WhatCanYouDo";
        public const string AreThereElectricalIssues = "AreThereElectricalIssues";
        public const string IsThereBrokenGlass = "IsThereBrokenGlass";

        /// <summary>
        /// Key in the bot config (.bot file) for the LUIS instance.
        /// In the .bot file, multiple instances of LUIS can be configured.
        /// </summary>
        public static readonly string LuisConfiguration = "BasicBotLuisApplication";
        private static readonly string[] EndDialogWords = new string[] { "restart", "hello" };

        private readonly IStatePropertyAccessor<LampPostReportState> _lampPostStateAccessor;
        private readonly IStatePropertyAccessor<DialogState> _dialogStateAccessor;
        private readonly UserState _userState;
        private readonly ConversationState _conversationState;
        private readonly CommonOptions _commonOptions;

        // Services
        private readonly IComputerVisionService _computerVisionService;
        private readonly IBingVisualSearchService _bingVisualSearchService;
        private readonly string _accessToken;

        private readonly LuisRecognizer _luis;

        /// <summary>
        /// Initializes a new instance of the <see cref="CityIssuesBot"/> class.
        /// </summary>
        public CityIssuesBot(BotOptions botOptions, UserState userState, ConversationState conversationState, IComputerVisionService computerVisionService, IBingVisualSearchService bingVisualSearchService, CommonOptions commonOptions, ILoggerFactory loggerFactory, LuisRecognizer luisRecognizer)
        {
            _userState = userState ?? throw new ArgumentNullException(nameof(userState));
            _conversationState = conversationState ?? throw new ArgumentNullException(nameof(conversationState));
            _commonOptions = commonOptions;

            _lampPostStateAccessor = _userState.CreateProperty<LampPostReportState>(nameof(LampPostReportState));
            _dialogStateAccessor = _conversationState.CreateProperty<DialogState>(nameof(DialogState));

            _computerVisionService = computerVisionService;
            _bingVisualSearchService = bingVisualSearchService;
            _accessToken = GetToken(botOptions);

            _luis = luisRecognizer;

            Dialogs = new DialogSet(_dialogStateAccessor);
            Dialogs.Add(new LampPostReportDialog(_lampPostStateAccessor, _bingVisualSearchService, commonOptions, _accessToken, loggerFactory));
        }

        private DialogSet Dialogs { get; set; }

        /// <summary>
        /// Every conversation turn for our NLP Dispatch Bot will call this method.
        /// There are no dialogs used, since it's "single turn" processing, meaning a single
        /// request and response, with no stateful conversation.
        /// </summary>
        /// <param name="turnContext">A <see cref="ITurnContext"/> containing all the data needed
        /// for processing this conversation turn. </param>
        /// <param name="cancellationToken">(Optional) A <see cref="CancellationToken"/> that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> that represents the work queued to execute.</returns>
        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            var activity = turnContext.Activity;

            // Create a dialog context
            var dc = await Dialogs.CreateContextAsync(turnContext);

            if (activity.Type == ActivityTypes.Message)
            {
                if (!string.IsNullOrEmpty(turnContext.Activity.Text) && EndDialogWords.Contains(turnContext.Activity.Text.ToLower()))
                {
                    // We finish the current dialog and present the user the initial greeting message
                    var endDialogResult = await dc.EndDialogAsync();
                    await dc.Context.SendActivityAsync($"Hi! How can I help you today?");
                    return;
                }

                // Continue the current dialog
                var dialogResult = await dc.ContinueDialogAsync();

                // if no one has responded,
                if (!dc.Context.Responded)
                {
                    // examine results from active dialog
                    switch (dialogResult.Status)
                    {
                        case DialogTurnStatus.Empty:
                            var file = activity?.Attachments?
                              .Where(attachment => attachment.ContentUrl != null)
                              .FirstOrDefault();

                            string topIntent = string.Empty;

                            if (file != null)
                            {
                                var detectResult = await _computerVisionService.Detect(file.ContentUrl, _accessToken);
                                topIntent = detectResult.ThereIsALampPost ? AreThereElectricalIssues : string.Empty;
                            }
                            else
                            {
                                // Perform a call to LUIS to retrieve results for the current activity message.
                                var luisResults = await _luis.RecognizeAsync(dc.Context, cancellationToken).ConfigureAwait(false);
                                var topScoringIntent = luisResults?.GetTopScoringIntent();
                                topIntent = topScoringIntent.Value.intent;
                            }

                            switch (topIntent)
                            {
                                case WhatCanYouDo:
                                    await dc.Context.SendActivityAsync("Check your lights; that's my super power.");
                                    break;
                                case AreThereElectricalIssues:
                                    await dc.BeginDialogAsync(nameof(LampPostReportDialog));
                                    break;
                                case IsThereBrokenGlass:
                                    await dc.Context.SendActivityAsync("Broken Glass? No.");
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

            await _conversationState.SaveChangesAsync(turnContext);
            await _userState.SaveChangesAsync(turnContext);
        }

        private string GetToken(BotOptions botOptions)
        {
            return new MicrosoftAppCredentials(botOptions.MicrosoftAppId, botOptions.MicrosoftAppPassword).GetTokenAsync().Result;
        }
    }
}
