using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using ApiTemplate.Interface;
using Newtonsoft.Json.Linq;
using System.Linq;
using ApiTemplate.Model;
using Microsoft.Extensions.Caching.Memory;

namespace ApiTemplate.Service
{
    public class ApiTemplateService : IApiTemplateService
    {
        private readonly IHttpClientFactory _httpFactory;
        private readonly IMemoryCache _memoryCache;

        public ApiTemplateService(IHttpClientFactory httpFactory, IMemoryCache memoryCache)
        {
            _httpFactory = httpFactory;
            _memoryCache = memoryCache;
        }

        public async Task<List<AddressResponse>> Validate(List<AddressRequest> addresses)
        {
            var httpClient = _httpFactory.CreateClient();
            
            var apiKey = "AIzaSyDmeY_MDNrctTsN2ZhRSywYwr5foXs6GPs";

            var validatedAddresses = new List<AddressResponse>();

            JToken coordinates;

            foreach (var address in addresses)
            {
                var addressString = $"{address.AddressLineOne},+{address.City}&key={apiKey}";

                if (!_memoryCache.TryGetValue(addressString, out JToken cacheValue))
                {

                    string url = $"https://maps.googleapis.com/maps/api/geocode/json?address={addressString}";

                    var response = await httpClient.GetAsync(url);
                    var json = await response.Content.ReadAsStringAsync();

                    JObject jsonObject = JObject.Parse(json);

                   coordinates = jsonObject.SelectToken("results[0].geometry.location");

                    _memoryCache.Set(addressString, coordinates);

                }
                else
                {
                    coordinates = cacheValue;  
                }

                validatedAddresses.Add(new AddressResponse()
                {
                    AddressLineOne = address.AddressLineOne,
                    City = address.City,
                    State = address.State,
                    Zip = address.Zip,
                    Latitude = (decimal)coordinates.SelectToken("lat"),
                    Longitude = (decimal)coordinates.SelectToken("lng")
                });
            }

            return validatedAddresses;

        }
    }
}
