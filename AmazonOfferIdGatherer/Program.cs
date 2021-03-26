using System;
using System.Threading.Tasks;

namespace AmazonOfferIdGatherer
{
    class Program
    {
        private static OfferIdDetails _offerIdDetails = new OfferIdDetails { Sku = String.Empty, OfferId = String.Empty };
        private static WebhookService _webhookService;
        private static readonly string _discordWebhookUrl = "https://discord.com/api/webhooks/824801442862727168/AYP-t2HIwBdyAChzBf6sEp6ZBdPruZLb0jmB-6h6uywycIe3-OlTWBwwY-R9lQS7Ydoq";
        private static readonly string _amazonProductEndpoint = @"https://www.amazon.com/gp/product/";

        public static async Task Main(string[] args)
        {
            _webhookService = new WebhookService(_discordWebhookUrl);
            while (String.IsNullOrEmpty(_offerIdDetails.OfferId) || String.IsNullOrEmpty(_offerIdDetails.Sku))
            {
                Console.WriteLine("Enter the sku:");
                string sku = Console.ReadLine();

                if (!String.IsNullOrEmpty(sku))
                {
                    _offerIdDetails.Sku = sku;
                    await GatherOfferId();
                }
            }
            Console.WriteLine("Offer Id:");
            Console.WriteLine(_offerIdDetails.OfferId);
            Console.WriteLine("The offer Id has been found and captured in discord.  Press any key to exit.");
            Console.ReadKey();
        }

        private static async Task GatherOfferId()
        {
            ScrapingService scrapingService = new ScrapingService(_offerIdDetails.Sku, _amazonProductEndpoint);

            try
            {
                while (String.IsNullOrEmpty(_offerIdDetails.OfferId))
                {
                    _offerIdDetails.OfferId = await scrapingService.GetOfferListingId();

                    if (String.IsNullOrEmpty(_offerIdDetails.OfferId))
                    {
                        Console.WriteLine("Item is not currently in stock and sold by Amazon.");
                        await Task.Delay(5000);
                    }
                }

                await _webhookService.SendDiscordMessage(_offerIdDetails.Sku, _offerIdDetails.OfferId, _amazonProductEndpoint + _offerIdDetails.Sku);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }
        }
    }
}
