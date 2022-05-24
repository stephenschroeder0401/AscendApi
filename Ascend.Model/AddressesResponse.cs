using System;
using System.Collections.Generic;
using System.Text;

namespace ApiTemplate.Model
{
    public class AddressesResponse
    {
        public List<AddressResponse> ValidAddresses { get; set; }

        public List<AddressRequest> InvalidAddresses { get; set; }

    }
}
