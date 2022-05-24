using ApiTemplate.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ApiTemplate.Interface
{
    public interface IGoogleMapsService
    {
        public Task<AddressResponse> GetCoordinates(AddressRequest request);
    }
}
