using System;
using System.Collections.Generic;
using System.IO;
using CustomSkills.Models;
using CustomSkills.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CustomSkills
{
    public static class TranslationFunction
    {
        [FunctionName("Translate")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]HttpRequest req,
            ILogger log,
            ExecutionContext context)
        {
            var requestBody = new StreamReader(req.Body).ReadToEnd();
            var skillRequest = JsonConvert.DeserializeObject<SkillRequest>(requestBody);

            // Validation
            if (skillRequest.Values == null)
            {
              return new BadRequestObjectResult(" Could not find values array.");
            }
            if (skillRequest.Values.Count == 0)
            {
                // It could not find a record, then return empty values array.
                return new BadRequestObjectResult(" Could not find valid records in values array.");
            }

            var response = new WebApiEnricherResponse();

            skillRequest.Values.ForEach(document =>
            {
                var responseRecord = GetTranslatedResponseRecord(context, document);
                response.Values.Add(responseRecord);
            });

            return (ActionResult)new OkObjectResult(response);
        }

        private static WebApiResponseRecord GetTranslatedResponseRecord(ExecutionContext context, SkillRecord record)
        {
            var config = Config.GetConfig(context.FunctionAppDirectory);
            var responseRecord = new WebApiResponseRecord();
            responseRecord.RecordId = record.RecordId;
            if (string.Equals("en", record.Data.Language, StringComparison.OrdinalIgnoreCase))
            {
                responseRecord.Data.Add("translation", record.Data.Text);
            }
            else
            {
                var translatedText = TranslationService.TranslateText(config["TranslationApiKey"], config["TranslationApiRegion"], record.Data.Text, "en").Result;
                responseRecord.Data.Add("translation", translatedText);
            }
            return responseRecord;
        }
    }
}