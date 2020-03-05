# AI School Smart City Engineering Efforts

This is a list of the items that will likely be involved in converting the classroom material into an RI, from an engineering perspective.  The first two items are bot-centric.  The third item is all about Azure Search and has nothing to do with conversational bot.

## Focus

The three examples delivered have nothing to do with each other.  If we want to cover the concepts included in all of them, we'll have to invent a more unified story to tie them together.  As they stand right now, they are independent and do not build off of each other.

## Pillars

### Cost

No special concerns here, however cost could be optimized by separating out the hosting of the SPAs from the hosting of the Bots.  We consider an Azure Storage + CDN bundle for the SPA, optimizing for the PaaS layers of bot compute and our cognitive services.

For azure search, one would have to evaluate the expected index perf and make sure the indexing pipeline can handled that load.

### DevOps

None of the example code includes DevOps considerations.  All bot examples are expected to be run via the bot emulator, not deployed to Azure.

* No:
  * Deployment Scripts
  * APM Considerations
  * Monitoring and Alerting
  * Logging & Observability
    * In some cases, exceptions are just swallowed or emitted to console out.
  * Configuration management (all done w/ config files)

### Resiliency

Base resiliency in these examples are based on single-region, drawing on the base SLA of each consumed service.  It's unlikely that we would be okay recommending a single-region solution, so significant effort will likely apply to this pillar.  Our choices here will impact Cost optimization and DevOps efforts.

Conversation history will not survive a reboot of the host, since it is all stored in memory.  This needs to be evaluated to see if we are okay having a large caveat in the docs around this, or if we should solve for it.  I would think we'll want to solve for it.

The consumption of Bing Visual Search API is being done by via hand-rolled HTTP calls instead of using the `Microsoft.Azure.CongnitiveServices.Search.VisualSearch` SDK.  If visual search makes it into the final cut, this will need to be migrated to the SDK.

The consumption of our translation service is being done via hand-rolled HTTP calls instead of using the `Microsoft.CognitiveServices.Speech.Translation` SDK.  If document translation makes it into the final cut, this will need to be migrated to the SDK.

Quick glance shows a potential problem with how they are fetching one of their OAuth tokens.  It might expire while the user is still engaged, which will cause for poor UX.  Need to evaluate if this code path is needed and if this needs to be solved for.

Opportunity to improve on retry logic in some places, where that logic was being done by hand instead of using Polly (or similar) for backoffs.

### Scalability

The services used are all inheritably scalable up to service limits of selected SKU choice.  We would probably spend time evaluating and identifying the first choke point in the system.

### Security

A lot of these services inherently use API keys.  But we should be looking for opportunities to leverage Managed Identity between the systems.  I haven't looked everywhere, but there might be places to upgrade to that.

In the document index example, there was usage of Azure Function keys.  That's not a recommended security boundary.  We'll have to dig in to see 1) is that feature in scope and 2) can it be solved via a different mechanism.

No plan of attack that I could see around key rotation.  Does that need to be included?

## Other

* .NET Version on all examples should be upgraded to latest
* All nuget packages need to be updated to latest - including targeting latest Bot Framework SDK
* A review of the dialogs/turns should be done to make sure they are using the latest Bot Framework SDK patterns and not any deprecated flow patterns.
* No consideration for localization of the bot interactions.  For something targeting a a government entity, we should probably have at least one localized example in the mix.
* The SmartTalk endpoint referenced in the code is deprecated and instead QnA maker's "chit chat" feature should be used instead.
* The Azure Search content doesn't have any consumers, it's all just azure portal interactions.  If we want this to be part of an RI, it needs to be "used" in some capacity.

## Prior Work

We already have a bunch of BOT related content out on the AAC.  We should have a plan of action to figure out how this one is going to distinguish itself.  What is this one doing that is different than the rest?  Should we combine multiple patterns into one super-bot that can demo many features/integrations?

* RA: [Enterprise-grade conversational bot](https://docs.microsoft.com/en-us/azure/architecture/reference-architectures/ai/conversational-bot) using [botbuilder](https://github.com/Microsoft/botbuilder-utils-js).
* SI: [Interactive Voice Bot](https://docs.microsoft.com/en-us/azure/architecture/solution-ideas/articles/interactive-voice-response-bot) with a broken link to the source code.
* Example Workload: [Hotel Reservation conversational bot](https://docs.microsoft.com/en-us/azure/architecture/example-scenario/ai/commerce-chatbot) with "[deploy to azure](https://docs.microsoft.com/en-us/azure/architecture/example-scenario/ai/commerce-chatbot#deploy-the-scenario)" functionality.
* SI: [FAQ Chatbot](https://docs.microsoft.com/en-us/azure/architecture/solution-ideas/articles/faq-chatbot-with-data-champion-model) with no source code.
* SI: [e-Commerce chatbot](https://docs.microsoft.com/en-us/azure/architecture/solution-ideas/articles/commerce-chatbot) with no source code.
* SI: [Enterprise chatbot](https://docs.microsoft.com/en-us/azure/architecture/solution-ideas/articles/enterprise-productivity-chatbot) with no source code.

We also already have Azure Search material

* ES: [eCommerce Search](https://docs.microsoft.com/en-us/azure/architecture/example-scenario/apps/ecommerce-search)
* RI : [Scalability in Web Apps](https://docs.microsoft.com/en-us/azure/architecture/reference-architectures/app-service-web-app/scalable-web-app) includes Azure Search in the RI material.
* ES: [eCommerce Front End](https://docs.microsoft.com/en-us/azure/architecture/example-scenario/apps/ecommerce-scenario) with [source artifacts](https://github.com/Azure/fta-customerfacingapps/tree/master/ecommerce/articles) from FastTrack.

## Concepts Covered in This Material

Just to make it easier to compare with prior art in this space, here is a list of core concepts that were being introduced to the AI School participant in this material.

* Cognitive Services
  * LUIS (rennovations bot, multi-channel bot)
    * Created via portal (rennovations bot, multi-channel bot)
    * Using externally provided pre-built model (rennovations bot, multi-channel bot)
  * Computer Vision (rennovations bot, multi-channel bot)
  * Bing Visual Search v7 (multi-channel bot)
  * Created via portal (rennovations bot, multi-channel bot)
* Bot SDK
  * Component Dialogs (rennovations bot)
  * Waterfall Steps (rennovations bot)
  * Client Redirects (rennovations bot)
  * Attachment Prompts (rennovations bot, multi-channel bot)
  * Conversation State _in memory_ (rennovations bot, multi-channel bot)
  * Input Validation (rennovations bot, multi-channel bot)
  * User State _in memory_ (rennovations bot, multi-channel bot)
  * Personality Chat Middleware (rennovations bot) via HTTP calls to https://smarttalk.azure-api.net/api/v1/botframework -- this is deprecated.
  * UX experience on bot API processing exceptions (rennovations bot, multi-channel bot)
  * Hero Cards (rennovations bot, multi-channel bot)
  * Handling backchannel direct-line interactions (rennovations bot)
  * Adaptive Cards from a template on disk (multi-channel bot)
  * Channel-specific responses (multi-channel bot)
* Azure App Services
  * Bot API Host (rennovations bot, multi-channel bot)
  * SPA Host (rennovations bot, multi-channel bot)
    * React app (rennovations bot)
      * Custom UI elements for web chat (rennovations bot)
      * Directline back-channel interaction (rennovations bot)
      * Inactivity detection and reaction (rennovations bot)
  * App Service Logging Provider (rennovations bot, multi-channel bot)
  * Deploying via right-click, publish (rennovations bot, multi-channel bot)
* Azure Bot Service (rennovations bot, multi-channel bot)
  * Created via portal (rennovations bot, multi-channel bot)
  * Direct Line channel (rennovations bot)
  * Skype channel (multi-channel bot)
  * Facebook Messenger channel (multi-channel bot)
* Bot Framework Emulator for local development experience (rennovations bot)

And for the Azure Search content...

* Azure Function
  * right-click publish from Visual Studio
* Azure Cognitive Service
  * Azure Search
    * Index pipeline from Azure Blob Storage
      * Enrichments (language detection, names)
      * Adding custom skills via azure management plane HTTP API
      * Expanding index via azure management plane HTTP API
    * Testing from portal
    * Reindexing from portal
