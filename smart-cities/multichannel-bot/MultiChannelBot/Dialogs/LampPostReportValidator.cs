using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using MultiChannelBot.Services.Interfaces;

namespace MultiChannelBot.Dialogs
{
    public static class LampPostReportValidator
    {
        public static async Task<bool> BarcodePictureValidatorAsync(
          PromptValidatorContext<IList<Attachment>> promptContext,
          LampPostReportState lampPostReportState,
          IBingVisualSearchService bingVisualSearchService,
          string accessToken)
        {
            var barcode = await bingVisualSearchService.RecognizeBarCodeAsync(promptContext.Recognized.Value[0].ContentUrl, accessToken);

            if (string.IsNullOrEmpty(barcode))
            {
                await promptContext.Context.SendActivityAsync($"Sorry, we didn't detect any barcode on the image provided.");
                return false;
            }

            lampPostReportState.Barcode = barcode;

            return true;
        }
    }
}
