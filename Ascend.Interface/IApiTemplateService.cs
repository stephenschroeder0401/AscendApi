using ApiTemplate.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiTemplate.Interface
{
    public interface IApiTemplateService
    {
        public Task<List<AddressResponse>> GetCoordinatesFromAddress(AddressesRequest request);
    }
}
