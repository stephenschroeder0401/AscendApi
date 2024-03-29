using ApiTemplate.Interface;
using ApiTemplate.Model;
using ApiTemplate.Repository;
using ApiTemplate.Service;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using System.Net.Http;

namespace TestAscendApi
{
    public class Tests
    {
        private ApiTemplateService _apiTemplateService;

        private Mock<IMemoryCache> memoryCache;
        private Mock<IGoogleMapsService> googleMapsService;
        private static IMapper mapper;
 

        [SetUp]
        public void Setup()
        {
         
            memoryCache = new Mock<IMemoryCache>();
            googleMapsService = new Mock<IGoogleMapsService>();

            var options = new DbContextOptionsBuilder<AscendContext>()
                .UseInMemoryDatabase(databaseName: "Ascend")
                .Options;

            var context = new AscendContext(options);

            context.Address.Add(new Address
            {
                AddressId = 1,
                AddressLineOne = "123 Main Street",
                State = "California",
                City = "Santa Cruz",
                Zip = "95060",
                Latitude = (decimal)10.12345,
                Longitude = (decimal)11.3456

            });

            context.SaveChanges();

            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfiles());
            });
            IMapper map = mappingConfig.CreateMapper();
            mapper = map;

            _apiTemplateService = new ApiTemplateService(memoryCache.Object, context, mapper, googleMapsService.Object);

        }

        [Test]
        public async System.Threading.Tasks.Task Test1Async()
        {
                var addressResponse = await _apiTemplateService.GetAddress("123 Main Street", "Santa Cruz", "California");

                Assert.IsNotNull(addressResponse);
                Assert.IsInstanceOf<AddressResponse>(addressResponse);
            }
        }
    }