using AmazonOfferIdGatherer.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;

namespace AmazonOfferIdGatherer.Services
{
    public sealed class ProxyService : IProxyService
    {
        private readonly ILogger<ProxyService> _log;
        private readonly IConfiguration _configuration;
        private readonly List<WebProxy> _proxies = new List<WebProxy>();

        public ProxyService(ILogger<ProxyService> log, IConfiguration configuration, List<string> proxies)
        {
            _log = log;
            _configuration = configuration;
            MapProxies(proxies);
        }

        public List<WebProxy> GetProxyList()
        {
            return _proxies;
        }

        public void InvalidateProxy(WebProxy proxy)
        {
            _proxies.Remove(proxy);
        }

        private void MapProxies(List<string> proxies)
        {
            foreach (string proxyString in proxies)
            {
                try
                {
                    WebProxy proxy = ParseProxyFromString(proxyString);

                    if (proxy != null)
                    {
                        _proxies.Add(proxy);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private WebProxy ParseProxyFromString(string proxyString)
        {
            WebProxy proxy = null;
            string[] splitProxyString = proxyString.Split(":");
            try
            {
                if (splitProxyString.Length == 4)
                {
                    Uri uri = new Uri("http://" + splitProxyString[0] + ":" + splitProxyString[1]);
                    proxy = new WebProxy(uri);
                    proxy.Credentials = new NetworkCredential(splitProxyString[2], splitProxyString[3]);

                }
                else
                {
                    throw new ArgumentOutOfRangeException("Proxy ignored, format must be as follows 'host:port:user:pass'");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return proxy;
        }
    }
}
