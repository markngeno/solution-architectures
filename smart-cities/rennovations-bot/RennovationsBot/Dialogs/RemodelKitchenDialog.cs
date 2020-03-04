using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using RennovationsBot.Services.Interfaces;

namespace RennovationsBot.Dialogs
{
    public class RemodelKitchenDialog : ComponentDialog
    {
        // Dialog IDs
        private const string ProfileDialog = "profileDialog";

        public RemodelKitchenDialog(
            ILoggerFactory loggerFactory)
            : base(nameof(RemodelKitchenDialog))
        {
            // Add control flow dialogs
            var waterfallSteps = new WaterfallStep[]
            {
                PromptForPrimaryResidenceInputStepAsync,
                PromptForAnyStructuralWorkStepAsync,
                FillFormRedirectStepAsync,
            };
            AddDialog(new WaterfallDialog(ProfileDialog, waterfallSteps));
            AddDialog(new ConfirmPrompt(RemodelKitchenPromptStep.IsPrimaryResidence));
            AddDialog(new ConfirmPrompt(RemodelKitchenPromptStep.AnyStructuralWork));
            AddDialog(new TextPrompt(RemodelKitchenPromptStep.FillForm));
        }

        private async Task<DialogTurnResult> PromptForPrimaryResidenceInputStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
          var opts = new PromptOptions
          {
            Prompt = new Activity
            {
              Type = ActivityTypes.Message,
              Text = "That’s exciting! Is this at your primary residence?",
            },
          };
          return await stepContext.PromptAsync(RemodelKitchenPromptStep.IsPrimaryResidence, opts);
        }

        private async Task<DialogTurnResult> PromptForAnyStructuralWorkStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var opts = new PromptOptions
            {
                Prompt = new Activity
                {
                    Type = ActivityTypes.Message,
                    Text = "Will there be any structural work taking place?",
                },
            };
            return await stepContext.PromptAsync(RemodelKitchenPromptStep.AnyStructuralWork, opts);
        }

        private async Task<DialogTurnResult> FillFormRedirectStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var context = stepContext.Context;

            await stepContext.Context.SendActivityAsync("Let me fetch the right form for you.");

            await Task.Delay(2000);

            // Send activity to allow the redirect
            await context.SendActivityAsync(new Activity
            {
                Type = ActivityTypes.Event,
                Text = "fill-form",
                Name = "fill-form",
                Value = "fill-form"
            });

            await Task.Delay(1000);

            await stepContext.Context.SendActivityAsync("Fill out the form on the right. It’s the permit required for non-structural remodelling. I’ve filled out as much as I can. It will help the council understand the extent of the work.");
            
            return await stepContext.EndDialogAsync();
        }
    }

    public static class RemodelKitchenPromptStep
    {
        public const string IsPrimaryResidence = "isPrimaryResidence";
        public const string AnyStructuralWork = "anyStructuralWork";
        public const string FillForm = "fillForm";
    }
}
