using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using RennovationsBot.Models;
using RennovationsBot.Options;
using RennovationsBot.Services.Interfaces;

namespace RennovationsBot.Services
{
    public class ComputerVisionService : IComputerVisionService
    {
        private static readonly HttpClient Client = new HttpClient();
        private static readonly TimeSpan QueryWaitTimeInSecond = TimeSpan.FromSeconds(1);

        private readonly ComputerVisionClient _computerVisionApi;
        private readonly List<string> _chemicalNames;
        private readonly List<string> _disposableCategories;
        private readonly List<string> _noDisposableCategories;
        private readonly List<string> _lampPostNames;

        public ComputerVisionService(ComputerVisionOptions computerVisionOptions)
        {
            _computerVisionApi = new ComputerVisionClient(
                    new ApiKeyServiceClientCredentials(computerVisionOptions.ApiKey),
                    new System.Net.Http.DelegatingHandler[] { });
            _computerVisionApi.Endpoint = computerVisionOptions.ApiEndpoint;
            _disposableCategories = computerVisionOptions.DisposableCategories.ToLowerInvariant().Split().ToList();
            _noDisposableCategories = computerVisionOptions.NotDisposableCategories.ToLowerInvariant().Split().ToList();
            _chemicalNames = computerVisionOptions.ChemicalNames.ToLowerInvariant().Split().ToList();
            _lampPostNames = computerVisionOptions.LampPostNames.ToLowerInvariant().Split(',').ToList();
        }

        public async Task<ComputerVisionDetectResult> Detect(string imageUrl)
        {
            var image = await Client.GetByteArrayAsync(imageUrl);
            var results = await _computerVisionApi.AnalyzeImageInStreamAsync(
                new MemoryStream(image),
                new[] { VisualFeatureTypes.Tags, VisualFeatureTypes.Description, VisualFeatureTypes.Categories });

            return new ComputerVisionDetectResult
            {
                ThereIsADog = ContainsObjectOfCategory(results, "dog"),
                ThereIsAChemical = ContainsObjectOfCategory(results, "chemical"),
                ThereIsAFridge = ContainsObjectOfCategory(results, "refrigerator"),
                ThereIsALampPost = ContainsObjectOfLampPostReferences(results),
                Description = results.Description.Captions.FirstOrDefault()?.Text,
            };
        }

        public async Task<bool> TextHasAnyChemicalName(string imageUrl)
        {
            var ocrResult = await RecognizeTextFromUrlAsync(imageUrl, TextRecognitionMode.Printed);
            var allWords = string.Join(" ", ocrResult.RecognitionResult.Lines.Select(line =>
            {
                return string.Join(" ", line.Words.Select(word => word.Text));
            })).ToLowerInvariant();
            return _chemicalNames.Any(chemicalName => allWords.IndexOf(chemicalName) > -1);
        }

        private async Task<TextOperationResult> RecognizeTextFromUrlAsync(string imageUrl, TextRecognitionMode recognitionMode)
        {
            var image = await Client.GetByteArrayAsync(imageUrl);
            return await RecognizeAsync(
                async (ComputerVisionClient client) => await client.RecognizeTextInStreamAsync(new MemoryStream(image), recognitionMode),
                headers => headers.OperationLocation);
        }

        private async Task<TextOperationResult> RecognizeAsync(Func<ComputerVisionClient, Task<RecognizeTextInStreamHeaders>> getHeadersAsyncFunc, Func<RecognizeTextInStreamHeaders, string> getOperationUrlFunc)
        {
            var result = default(TextOperationResult);

            try
            {
                var recognizeHeaders = await getHeadersAsyncFunc(_computerVisionApi);
                var operationUrl = getOperationUrlFunc(recognizeHeaders);
                var operationId = operationUrl.Substring(operationUrl.LastIndexOf('/') + 1);

                result = await _computerVisionApi.GetTextOperationResultAsync(operationId);

                for (int attempt = 1; attempt <= 10; attempt++)
                {
                    if (result.Status == TextOperationStatusCodes.Failed || result.Status == TextOperationStatusCodes.Succeeded)
                    {
                        break;
                    }

                    await Task.Delay(QueryWaitTimeInSecond);

                    result = await _computerVisionApi.GetTextOperationResultAsync(operationId);
                }
            }
            catch (Exception)
            {
                result = new TextOperationResult() { Status = TextOperationStatusCodes.Failed };
            }

            return result;
        }

        private bool ContainsObjectOfCategory(ImageAnalysis results, string category)
        {
            return results.Tags.Any(x => string.Equals(x.Name, category, StringComparison.InvariantCultureIgnoreCase))
                  || results.Categories.Any(x => string.Equals(x.Name, category, StringComparison.InvariantCultureIgnoreCase));
        }

        private bool ContainsObjectOfLampPostReferences(ImageAnalysis results)
        {
            return _lampPostNames.Any(name => results.Tags.Any(tag => string.Equals(tag.Name, name, StringComparison.InvariantCultureIgnoreCase)))
                  || _lampPostNames.Any(name => results.Categories.Any(category => string.Equals(category.Name, name, StringComparison.InvariantCultureIgnoreCase))); ;
        }
    }
}
