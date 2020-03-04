using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using MultiChannelBot.Models;
using MultiChannelBot.Models.Options;
using MultiChannelBot.Services.Interfaces;

namespace MultiChannelBot.Dialogs
{
    public class LampPostReportDialog : ComponentDialog
    {
        private const string BarcodePromptStept = "barcodePrompt";
        private const string NamePromptStept = "namePrompt";
        private const string CreateTickePromptStept = "createTicketPrompt";

        // Dialog IDs
        private const string BrokenLampDialog = "brokenLampDialog";

        // Services
        private readonly IBingVisualSearchService _bingVisualSearchService;

        private readonly CommonOptions _commonOptions;
        private readonly string _accessToken;

        public LampPostReportDialog(
            IStatePropertyAccessor<LampPostReportState> userProfileStateAccessor,
            IBingVisualSearchService bingVisualSearchService,
            CommonOptions commonOptions,
            string accessToken,
            ILoggerFactory loggerFactory)
            : base(nameof(LampPostReportDialog))
        {
            _bingVisualSearchService = bingVisualSearchService;
            _commonOptions = commonOptions;
            _accessToken = accessToken;

            UserProfileAccessor = userProfileStateAccessor ?? throw new ArgumentNullException(nameof(userProfileStateAccessor));

            // This array defines the Waterfall that will be executed.
            var waterfallSteps = new WaterfallStep[]
            {
                InitializeStateStepAsync,
                PromptForBarcodeStepAsync,
                AskforNameStepAsync,
                CreateTicketStepAsync,
            };

            // Add dialogs to the component dialog
            AddDialog(new WaterfallDialog(BrokenLampDialog, waterfallSteps));
            AddDialog(new AttachmentPrompt(BarcodePromptStept, async (promptValidatorContext, cancellationToken) =>
            {
                var lampPostReportState = await UserProfileAccessor.GetAsync(promptValidatorContext.Context);
                return await LampPostReportValidator.BarcodePictureValidatorAsync(promptValidatorContext, lampPostReportState, _bingVisualSearchService, _accessToken);
            }));
            AddDialog(new TextPrompt(NamePromptStept));
            AddDialog(new TextPrompt(CreateTickePromptStept));
        }

        public IStatePropertyAccessor<LampPostReportState> UserProfileAccessor { get; }

        public string UserLocation { get; set; }

        private async Task<DialogTurnResult> InitializeStateStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var lampPostReportState = await UserProfileAccessor.GetAsync(stepContext.Context, () => null);
            if (lampPostReportState == null)
            {
                var lampPostReportStateOpt = stepContext.Options as LampPostReportState;
                await UserProfileAccessor.SetAsync(stepContext.Context, lampPostReportStateOpt ?? new LampPostReportState());
            }

            return await stepContext.NextAsync();
        }

        private async Task<DialogTurnResult> PromptForBarcodeStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var context = stepContext.Context;
            var lampPostReportState = await UserProfileAccessor.GetAsync(context);

            var opts = new PromptOptions
            {
                Prompt = MessageFactory.Text($"That looks like a broken street lamp. Please, can you take a picture of the barcode at the base, so we can identify the type."),
                RetryPrompt = MessageFactory.Text("I didn't detect the barcode number, could you please try again?"),
            };

            return await stepContext.PromptAsync(BarcodePromptStept, opts);

        }

        private async Task<DialogTurnResult> AskforNameStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var context = stepContext.Context;
            var lampPostReportState = await UserProfileAccessor.GetAsync(context);

            await context.SendActivityAsync($"Thanks for reporting the broken street lamp with code: {lampPostReportState.Barcode}");

            await Task.Delay(1000);

            await context.SendActivityAsync($"Let me help you create a ticket for our maintenance staff");

            await Task.Delay(1000);

            var opts = new PromptOptions
            {
                Prompt = MessageFactory.Text($"What is your name?"),
            };

            return await stepContext.PromptAsync(NamePromptStept, opts);
        }

        private async Task<DialogTurnResult> CreateTicketStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var context = stepContext.Context;
            var lampPostReportState = await UserProfileAccessor.GetAsync(context);
            await context.SendActivityAsync($"Thanks! Please wait while I create a ticket for you!");

            await Task.Delay(2000);

            var caseId = "TL-783456";

            // Basic hero card reply
            var reply = context.Activity.CreateReply();
            reply.Attachments = new List<Attachment> { CreateBasicHeroCard(caseId) };
            await context.SendActivityAsync(reply);

            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }

        private Attachment CreateBasicHeroCard(string tickeNumber)
        {
            var card = new HeroCard()
            {
                Images = new List<CardImage> { new CardImage() { Url = $"{_commonOptions.SiteUri}/images/ticket-simple-image.jpg" } },
                Buttons = new List<CardAction>
                {
                  new CardAction() { Title = tickeNumber, Type = ActionTypes.OpenUrl, Value = "https://docs.microsoft.com/bot-framework" },
                },
            };
            return card.ToAttachment();
        }
    }
}