using AmazonOfferIdGatherer.Interfaces;
using HtmlAgilityPack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ScrapySharp.Extensions;
using ScrapySharp.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AmazonOfferIdGatherer.Services
{
    public sealed class ScrapingService : IScrapingService
    {
        private readonly ILogger<ScrapingService> _log;
        private readonly IConfiguration _configuration;
        private readonly string _url;
        private ScrapingBrowser _browser;
        private readonly string _amazonMerchantId = "ATVPDKIKX0DER";

        public ScrapingService(ILogger<ScrapingService> log,IConfiguration configuration, string sku, string productEndpoint)
        {
            _log = log;
            _configuration = configuration;
            if (String.IsNullOrEmpty(sku))
            {
                throw new ArgumentNullException();
            }
            _url = productEndpoint + sku;
            _browser = new ScrapingBrowser();
        }

        public async Task<string> GetOfferListingId()
        {
            string offerId = String.Empty;

            WebPage webpage = await _browser.NavigateToPageAsync(new Uri(_url));
            HtmlNode htmlNode = webpage.Html;
            IEnumerable<HtmlNode> formNodes = htmlNode.CssSelect("form#addToCart");

            if (formNodes.Any())
            {
                foreach (HtmlNode formNode in formNodes)
                {
                    HtmlNode offerListingIdNode = formNode.CssSelect("input#offerListingID").FirstOrDefault();
                    HtmlNode merchantIdNode = formNode.CssSelect("input#merchantID").FirstOrDefault();
                    if (_amazonMerchantId == merchantIdNode.Attributes["value"].Value)
                    {
                        offerId = offerListingIdNode.Attributes["value"].Value;
                        break;
                    }
                }
            }

            return offerId;
        }
    }
}
