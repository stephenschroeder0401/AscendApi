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

    public class AddressResponse 
    {
        public string AddressLineOne { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
    }
}
