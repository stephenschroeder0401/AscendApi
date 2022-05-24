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
using ApiTemplate.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ApiTemplate.Service
{
    public class ApiTemplateService : IApiTemplateService
    {
        private readonly IHttpClientFactory _httpFactory;
        private readonly IMemoryCache _memoryCache;
        private readonly AscendContext _context;
        private readonly string _mapsBaseUrl;
        private readonly string _mapsApiKey;


        public ApiTemplateService(IHttpClientFactory httpFactory, IMemoryCache memoryCache, AscendContext context, IConfiguration config)
        {
            _httpFactory = httpFactory;
            _memoryCache = memoryCache;
            _context = context;

            _mapsBaseUrl = config["GoogleMapsAddressApi:BaseUrl"];
            _mapsApiKey = config["GoogleMapsAddressApi:ApiKey"];

        }

        public async Task<List<AddressResponse>> GetCoordinatesFromAddresses(AddressesRequest addresses)
        {
            var httpClient = _httpFactory.CreateClient();

            var validatedAddresses = new List<AddressResponse>();

            JToken coordinates;

            foreach (var address in addresses.Addresses)
            {
                var addressString = $"{address.AddressLineOne.ToLower()},+{address.City.ToLower()},+{address.State.ToLower()}";

                if (!_memoryCache.TryGetValue(addressString, out JToken cacheValue))
                {
                    //Look for address in DB if not found in cache
                    var fullAddress = await Get(address.AddressLineOne, address.City, address.State);

                    if (fullAddress != null)
                    {
                        validatedAddresses.Add(fullAddress);
                        continue;
                    }

                    //Get address coordinates from api if not found in cache or DB
                    var response = await httpClient.GetAsync($"{_mapsBaseUrl}{addressString}&key={_mapsApiKey}");
                    var json = await response.Content.ReadAsStringAsync();
                    
                    coordinates = JObject.Parse(json).SelectToken("results[0].geometry.location");

                    _memoryCache.Set(addressString, coordinates);

                }
                else
                {
                    coordinates = cacheValue;  
                }

                var addressResponse = new AddressResponse()
                {
                    AddressLineOne = address.AddressLineOne,
                    City = address.City,
                    State = address.State,
                    Zip = address.Zip,
                    Latitude = (decimal)coordinates.SelectToken("lat"),
                    Longitude = (decimal)coordinates.SelectToken("lng")
                };

                validatedAddresses.Add(addressResponse);

                //Add to database if we hit maps API 
                if(cacheValue == null)
                    _context.AddressResponses.Add(addressResponse);
            }

            await _context.SaveChangesAsync();

            return validatedAddresses;

        }


        public async Task<AddressResponse> Get(string line1, string city, string state) 
        {
           var address = await _context.AddressResponses.Where(x => x.AddressLineOne == line1 &&
                  x.City == city && x.State == state).FirstOrDefaultAsync();
            
           return address;
        }
    }
}
