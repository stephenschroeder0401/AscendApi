using ApiTemplate.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiTemplate.Interface
{
    public interface IApiTemplateService
    {
        public Task<AddressesResponse> GetCoordinatesFromAddresses(AddressesRequest request);

        public Task<AddressResponse> Get(string line1, string city, string state);
    }
}
