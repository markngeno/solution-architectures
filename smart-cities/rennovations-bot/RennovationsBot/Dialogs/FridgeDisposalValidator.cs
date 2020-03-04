using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using RennovationsBot.Services.Interfaces;

namespace RennovationsBot.Dialogs
{
    public static class FridgeDisposalValidator
    {
        private const int TextInputLengthMinValue = 3;

        public static async Task<bool> ValidateTextInput(PromptValidatorContext<string> promptContext, CancellationToken cancellationToken)
        {
            // Validate that the user entered a minimum length.
            var value = promptContext.Recognized.Value?.Trim() ?? string.Empty;
            if (value.Length > TextInputLengthMinValue)
            {
                promptContext.Recognized.Value = value;
                return true;
            }
            else
            {
                await promptContext.Context.SendActivityAsync($"Your input needs to be at least `{TextInputLengthMinValue}` characters long.").ConfigureAwait(false);
                return false;
            }
        }

        public static async Task<bool> ValidateDayOfWeek(PromptValidatorContext<string> promptContext, CancellationToken cancellationToken)
        {
            var value = promptContext.Recognized.Value?.Trim() ?? string.Empty;
            var inx = Array.FindIndex(CultureInfo.CurrentCulture.DateTimeFormat.DayNames, x => string.Equals(x, value, StringComparison.InvariantCultureIgnoreCase));

            if (inx >= 0)
            {
                promptContext.Recognized.Value = value;
                return true;
            }
            else
            {
                await promptContext.Context.SendActivityAsync($"Your input needs to be a valid date.").ConfigureAwait(false);
                return false;
            }
        }

        public static async Task<bool> ValidateSchedule(PromptValidatorContext<string> promptContext, CancellationToken cancellationToken)
        {
            var value = promptContext.Recognized.Value?.Trim() ?? string.Empty;
            if (string.Equals(value, Constants.Morning, StringComparison.InvariantCultureIgnoreCase))
            {
                promptContext.Recognized.Value = "7am and 12pm";
            }
            else if (string.Equals(value, Constants.Afternoon, StringComparison.InvariantCultureIgnoreCase))
            {
                promptContext.Recognized.Value = "12pm and 5pm";
            }
            else if (string.Equals(value, Constants.Evening, StringComparison.InvariantCultureIgnoreCase))
            {
                promptContext.Recognized.Value = "5pm and 8pm";
            }
            else
            {
                await promptContext.Context.SendActivityAsync($"Please click any of the options.").ConfigureAwait(false);
                return false;
            }

            return true;
        }

        public static async Task<bool> FridgePictureValidator(
            PromptValidatorContext<IList<Attachment>> promptContext,
            IComputerVisionService computerVisionService)
        {
            try
            {
                var learnMoreText = "Click [here](#) to learn about the approved items.";
                var detectResult = await computerVisionService.Detect(promptContext.Recognized.Value[0].ContentUrl);

                if (detectResult.ThereIsADog)
                {
                    await promptContext.Context.SendActivityAsync($"Sorry, we can’t dispose of your dog. {learnMoreText}");
                    return false;
                }

                // Use Ocr to check for chemicals names in the image
                var isAChemical = await computerVisionService.TextHasAnyChemicalName(promptContext.Recognized.Value[0].ContentUrl);
                if (detectResult.ThereIsAChemical || isAChemical)
                {
                    await promptContext.Context.SendActivityAsync($"Unfortunately, we can’t dispose of chemicals. {learnMoreText}");
                    return false;
                }

                if (!detectResult.ThereIsAFridge)
                {
                    await promptContext.Context.SendActivityAsync($"That doesn't look like a fridge. It looks more like {detectResult.Description}. You can't dispose that! {learnMoreText}");
                    return false;
                }

                return true;
            }
            catch (Exception)
            {
                await promptContext.Context.SendActivityAsync($"Sorry, I can't see the fridge right now. Can you please upload the image again?");
                return false;
            }
        }

        public static async Task<bool> DimensionsResponseValidator(
              PromptValidatorContext<string> promptContext,
              FridgeDisposalState getRidOfTheFridgeState)
        {
          var value = promptContext.Recognized.Value?.Trim() ?? string.Empty;
          if (value.Length > TextInputLengthMinValue)
          {
              promptContext.Recognized.Value = value;
              getRidOfTheFridgeState.Dimension = value;
              return true;
          }
          else
          {
              await promptContext.Context.SendActivityAsync($"Looks like the dimensions are not valid. Please enter a valid dimensions like `100 inch x 100 inch`.").ConfigureAwait(false);
              return false;
          }
        }
  }
}
