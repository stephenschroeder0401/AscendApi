
using System.Collections.Generic;
using System.Threading.Tasks;
using ApiTemplate.Interface;
using System.Linq;
using ApiTemplate.Model;
using Microsoft.Extensions.Caching.Memory;
using ApiTemplate.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using AutoMapper;
using AutoMapper.QueryableExtensions;

namespace ApiTemplate.Service
{
    public class ApiTemplateService : IApiTemplateService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly AscendContext _context;
        private readonly IMapper _mapper;
        private readonly IGoogleMapsService _mapsService;


        public ApiTemplateService( IMemoryCache memoryCache, AscendContext context, IMapper mapper, IGoogleMapsService mapsService)
        {
            _memoryCache = memoryCache;
            _context = context;
            _mapper = mapper;
            _mapsService = mapsService;
        }

        public async Task<AddressesResponse> GetCoordinatesFromAddresses(AddressesRequest addresses)
        {

            var validatedAddresses = new AddressesResponse() { ValidAddresses = new List<AddressResponse>(), 
                InvalidAddresses = new List<AddressRequest>()};

            foreach (var address in addresses.Addresses)
            {
                var cacheKey = $"{address.AddressLineOne},+{address.City},+{address.State}";

                if (!_memoryCache.TryGetValue(cacheKey, out AddressResponse cacheValue))
                {
                    //Look for address in DB if not found in cache
                    var fullAddress = await GetAddress(address.AddressLineOne, address.City, address.State);

                    if (fullAddress != null)
                    {
                        validatedAddresses.ValidAddresses.Add(fullAddress);
                        continue;
                    }

                    //Get address coordinates from api if not found in cache or DB
                    var foundAddress = await _mapsService.GetCoordinates(address);

                    if (foundAddress != null)
                    {
                        validatedAddresses.ValidAddresses.Add(foundAddress);
                        _memoryCache.Set(cacheKey, foundAddress);
                        _context.Address.Add(_mapper.Map<Address>(foundAddress));
                    }
                    else
                    {
                        validatedAddresses.InvalidAddresses.Add(address);
                    }

                }

                else
                {
                    validatedAddresses.ValidAddresses.Add(cacheValue);
                }
            }

            await _context.SaveChangesAsync();

            return validatedAddresses;

        }


        public async Task<AddressResponse> GetAddress(string line1, string city, string state) 
        {
           var address = await _context.Address.ProjectTo<AddressResponse>(_mapper.ConfigurationProvider)
                .Where(x => x.AddressLineOne == line1 &&
                  x.City == city && x.State == state).FirstOrDefaultAsync();
            
           return address;
        }
    }
}
