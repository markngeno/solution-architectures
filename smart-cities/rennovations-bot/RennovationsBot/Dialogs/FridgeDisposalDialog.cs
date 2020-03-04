using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using RennovationsBot.Services.Interfaces;

namespace RennovationsBot.Dialogs
{
    public class FridgeDisposalDialog : ComponentDialog
    {
        // Dialog IDs
        private const string ProfileDialog = "profileDialog";

        // Services
        private readonly IComputerVisionService _computerVisionService;

        public FridgeDisposalDialog(
            IStatePropertyAccessor<FridgeDisposalState> userProfileStateAccessor,
            IComputerVisionService computerVisionService,
            ILoggerFactory loggerFactory)
            : base(nameof(FridgeDisposalDialog))
        {
            _computerVisionService = computerVisionService;

            UserProfileAccessor = userProfileStateAccessor ?? throw new ArgumentNullException(nameof(userProfileStateAccessor));

            // Add control flow dialogs
            var waterfallSteps = new WaterfallStep[]
            {
                PromptForFridgePictureStepAsync,
                ConfirmDimensionsStepAsync,
            };
            AddDialog(new WaterfallDialog(ProfileDialog, waterfallSteps));
            AddDialog(new AttachmentPrompt(PromptStep.FridgePicturePrompt, async (promptValidatorContext, cancellationToken) =>
            {
                return await FridgeDisposalValidator.FridgePictureValidator(promptValidatorContext, _computerVisionService);
            }));
        }

        public IStatePropertyAccessor<FridgeDisposalState> UserProfileAccessor { get; }

        private async Task<DialogTurnResult> PromptForFridgePictureStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var context = stepContext.Context;

            await context.SendActivityAsync("Sure! I can help with that. Upload a photo of what you need collected.");

            await Task.Delay(1000);

            var opts = new PromptOptions
            {
                Prompt = new Activity
                {
                    Type = ActivityTypes.Message,
                    Text = "If you have a business card, include it in the photo so I can estimate the dimensions.",
                },
            };
            return await stepContext.PromptAsync(PromptStep.FridgePicturePrompt, opts);
        }

        private async Task<DialogTurnResult> ConfirmDimensionsStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var context = stepContext.Context;

            await context.SendActivityAsync("Great! A City of Contoso removals truck will visit your property to collect your fridge as requested.");

            return await ShowResultToUser(stepContext);
        }

        private async Task<DialogTurnResult> ShowResultToUser(WaterfallStepContext stepContext)
        {
            var context = stepContext.Context;

            await Task.Delay(2000);

            await context.SendActivityAsync("Is there anything else I can help you with today?");

            return await stepContext.EndDialogAsync();
        }
    }

    public static class PromptStep
    {
        public const string FridgePicturePrompt = "fridgePicturePrompt";
    }
}
