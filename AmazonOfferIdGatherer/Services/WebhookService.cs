using AmazonOfferIdGatherer.Interfaces;
using Discord;
using Discord.Webhook;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace AmazonOfferIdGatherer.Services
{
    public sealed class WebhookService : IWebhookService
    {
        private readonly ILogger<WebhookService> _log;
        private readonly IConfiguration _configuration;
        private string _webhookUrl;

        public WebhookService(ILogger<WebhookService> log, IConfiguration configuration, string webhookUrl)
        {
            _log = log;
            _configuration = configuration;
            if (String.IsNullOrEmpty(webhookUrl))
            {
                throw new ArgumentNullException();
            }

            _webhookUrl = webhookUrl;
        }

        public async Task SendDiscordMessage(string title, string description, string messageText)
        {
            using (var client = new DiscordWebhookClient(_webhookUrl))
            {
                var embed = new EmbedBuilder
                {
                    Title = title,
                    Description = description
                };

                // Webhooks are able to send multiple embeds per message
                // As such, your embeds must be passed as a collection.
                await client.SendMessageAsync(text: messageText, embeds: new[] { embed.Build() });
            }
        }
    }
}
