using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiTemplate.Interface;
using ApiTemplate.Model;

namespace MoviesApi.Controllers
{
    [Route("address")]
    [ApiController]
    public class ApiTemplateController : ControllerBase
    {
        private IApiTemplateService _ApiTemplateService;

        public ApiTemplateController(IApiTemplateService ApiTemplateService)
        {
            _ApiTemplateService = ApiTemplateService;
        }

        [Route("coordinates")]
        [HttpPost]
        public async Task<ActionResult> Validate(AddressesRequest request)
        {
            var result =  await _ApiTemplateService.GetCoordinatesFromAddress(request);
            return Ok(result); 
        }
    }

}
