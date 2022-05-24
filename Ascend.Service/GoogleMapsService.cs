using ApiTemplate.Interface;
using ApiTemplate.Model;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ApiTemplate.Service
{
    public class GoogleMapsService : IGoogleMapsService
    {
        private readonly IHttpClientFactory _httpFactory;

        private readonly string _mapsBaseUrl;
        private readonly string _mapsApiKey;


        public GoogleMapsService(IConfiguration config, IHttpClientFactory httpFactory)  
        {
            _httpFactory = httpFactory;

            _mapsBaseUrl = config["GoogleMapsAddressApi:BaseUrl"];
            _mapsApiKey = config["GoogleMapsAddressApi:ApiKey"];
        }
        public async Task<AddressResponse> GetCoordinates(AddressRequest address)
        {
            var httpClient = _httpFactory.CreateClient();
            var addressString = $"{address.AddressLineOne},+{address.City},+{address.State}";

            try
            {
                var response = await httpClient.GetAsync($"{_mapsBaseUrl}{addressString}&key={_mapsApiKey}");
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();

                var coordinates = JObject.Parse(json).SelectToken("results[0].geometry.location");

                return new AddressResponse()
                {
                    AddressLineOne = address.AddressLineOne,
                    City = address.City,
                    State = address.State,
                    Zip = address.Zip,
                    Latitude = (decimal)coordinates.SelectToken("lat"),
                    Longitude = (decimal)coordinates.SelectToken("lng")
                };

            }
            catch (HttpRequestException ex)
            {
                throw new HttpRequestException(ex.Message);
            }
        }
    }
}
