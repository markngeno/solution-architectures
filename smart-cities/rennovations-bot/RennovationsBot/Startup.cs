// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio CoreBot v4.3.0

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Integration;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Builder.PersonalityChat.Core;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RennovationsBot.Bots;
using RennovationsBot.Dialogs;
using RennovationsBot.Options;
using RennovationsBot.Services;
using System;

namespace RennovationsBot
{
    public class Startup
    {
        private ILoggerFactory _loggerFactory;

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
              .SetBasePath(env.ContentRootPath)
              .AddJsonFile($"appsettings.json", optional: false, reloadOnChange: true)
              .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
              .AddEnvironmentVariables();

            // set local variables for use elsewhere
            Configuration = builder.Build();
            HostingEnvironment = env;
        }

        public IHostingEnvironment HostingEnvironment { get; }

        public IConfigurationRoot Configuration { get; }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // create luis recognizer
            var luisApplication = new LuisApplication(
                Configuration["LuisAppId"],
                Configuration["LuisAPIKey"],
                "https://" + Configuration["LuisAPIHostName"]
            );
            services.AddSingleton(new LuisRecognizer(luisApplication));

            // Create the credential provider to be used with the Bot Framework Adapter.
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
            services.AddSingleton<FridgeDisposalDialog>();
            services.AddSingleton<FridgeDisposalState>();
            services.AddSingleton<RemodelKitchenDialog>();

            // Create services
            var computerVisionOptions = Configuration.GetSection("ComputerVision").Get<ComputerVisionOptions>();
            services.AddSingleton(sp => new ComputerVisionService(computerVisionOptions));

            // Memory Storage is for local bot debugging only. When the bot
            // is restarted, everything stored in memory will be gone.
            IStorage dataStore = new MemoryStorage();

            // Create and add conversation state.
            var conversationState = new ConversationState(dataStore);
            services.AddSingleton(conversationState);

            var userState = new UserState(dataStore);
            services.AddSingleton(userState);

            // Add the personality chat middleware
            var personalityChatOptions = new PersonalityChatMiddlewareOptions(
                respondOnlyIfChat: true,
                scoreThreshold: 0.5F,
                botPersona: PersonalityChatPersona.Humorous);
            services.AddSingleton(new Middlewares.PersonalityChatMiddleware(personalityChatOptions));

            services.AddTransient<IBot, RennovationsIBot>();

            // Create and register state accessors.
            // Accessors created here are passed into the IBot-derived class on every turn.
            services.AddSingleton(sp =>
            {
                // We need to grab the conversationState we added on the options in the previous step
                var options = sp.GetRequiredService<IOptions<BotFrameworkOptions>>().Value;
                if (options == null)
                {
                    throw new InvalidOperationException("BotFrameworkOptions must be configured prior to setting up the State Accessors");
                }

                // Create the custom state accessor.
                // State accessors enable other components to read and write individual properties of state.
                var accessors = new RennovationsBotAccessors(conversationState, userState)
                {
                    ConversationDialogState = conversationState.CreateProperty<DialogState>("DialogState"),
                    GetRidOfTheFridgeState = userState.CreateProperty<FridgeDisposalState>("GetRidOfTheFridgeState"),
                };

                return accessors;
            });

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/build";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;

            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseSpaStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                          name: "default",
                          template: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });
        }
    }
}
