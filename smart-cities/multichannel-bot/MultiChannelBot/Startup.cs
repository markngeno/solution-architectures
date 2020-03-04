// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Configuration;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MultiChannelBot.Dialogs;
using MultiChannelBot.Models.Options;
using MultiChannelBot.Options;
using MultiChannelBot.Services;
using MultiChannelBot.Services.Interfaces;
using System;
using System.IO;

namespace MultiChannelBot
{
    /// <summary>
    /// The Startup class configures services and the app's request pipeline.
    /// </summary>
    public class Startup
    {
        private ILoggerFactory _loggerFactory;
        private readonly bool _isProduction = false;

        public Startup(IHostingEnvironment env)
        {
            _isProduction = env.IsProduction();

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
            HostingEnvironment = env;
        }

        public IHostingEnvironment HostingEnvironment { get; }

        /// <summary>
        /// Gets the configuration that represents a set of key/value application configuration properties.
        /// </summary>
        /// <value>
        /// The <see cref="IConfiguration"/> that represents a set of key/value application configuration properties.
        /// </value>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> specifies the contract for a collection of service descriptors.</param>
        /// <seealso cref="IStatePropertyAccessor{T}"/>
        /// <seealso cref="https://docs.microsoft.com/en-us/aspnet/web-api/overview/advanced/dependency-injection"/>
        /// <seealso cref="https://docs.microsoft.com/en-us/azure/bot-service/bot-service-manage-channels?view=azure-bot-service-4.0"/>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            var botOptions = new BotOptions()
            {
              MicrosoftAppId = Configuration["MicrosoftAppId"],
              MicrosoftAppPassword = Configuration["MicrosoftAppPassword"],
              LuisAppId = Configuration["LuisAppId"],
              LuisAPIKey = Configuration["LuisAPIKey"],
              LuisAPIHostName = Configuration["LuisAPIHostName"],
            };
            services.AddSingleton(sp => botOptions);

            // create luis recognizer
            var luisApplication = new LuisApplication(botOptions.LuisAppId, botOptions.LuisAPIKey, "https://" + botOptions.LuisAPIHostName);

            services.AddSingleton(new LuisRecognizer(luisApplication));

            // Create the credential provider to be used with the Bot Framework Adapter
            services.AddSingleton<ICredentialProvider, ConfigurationCredentialProvider>();

            // Create the Bot Framework Adapter with error handling enabled.
            services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();

            // Create the storage we'll be using for User and Conversation state. (Memory is great for testing purposes.)
            services.AddSingleton<IStorage, MemoryStorage>();

            // Create the User state. (Used in this bot's Dialog implementation.)
            services.AddSingleton<UserState>();

            // Create the Conversation state. (Used by the Dialog system itself.)
            services.AddSingleton<ConversationState>();

            // The Dialog that will be run by the bot.
            services.AddSingleton<LampPostReportDialog>();
            services.AddSingleton<LampPostReportState>();

            IStorage dataStore = new MemoryStorage();

            var cognitiveServicesOptions = Configuration.GetSection("CognitiveServices").Get<CognitiveServicesOptions>();
            services.Configure<CognitiveServicesOptions>(Configuration.GetSection("CognitiveServices"));
            services.AddSingleton(sp => cognitiveServicesOptions);

            var commonOptions = Configuration.GetSection("CommonOptions").Get<CommonOptions>();

            services.Configure<CommonOptions>(Configuration.GetSection("CommonOptions"));
            services.AddSingleton(sp => commonOptions);

            services.AddSingleton<IComputerVisionService>(sp => new ComputerVisionService(cognitiveServicesOptions.ComputerVision));
            services.AddSingleton<IBingVisualSearchService>(sp => new BingVisualSearchService(cognitiveServicesOptions.BingVisualSearch));


            services.AddBot<CityIssuesBot>(options =>
            {
                // Creates a logger for the application to use.
                ILogger logger = _loggerFactory.CreateLogger<CityIssuesBot>();

                // Catches any errors that occur during a conversation turn and logs them.
                options.OnTurnError = async (context, exception) =>
                {
                    logger.LogError($"Exception caught : {exception}");
                    await context.SendActivityAsync("Sorry, it looks like something went wrong.");
                };
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseHttpsRedirection();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                          name: "default",
                          template: "{controller}/{action=Index}/{id?}");
            });

        }
    }
}
