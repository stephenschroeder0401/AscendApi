using ApiTemplate.Model;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiTemplate.Api.Validation
{
    public class AddressesRequestValidator : AbstractValidator<AddressesRequest>
    {   
        public AddressesRequestValidator() 
        {
            RuleForEach(x => x.Addresses).ChildRules(address =>
            {
                address.RuleFor(x => x.AddressLineOne).NotEmpty();
                address.RuleFor(x => x.State).NotEmpty();
                address.RuleFor(x => x.City).NotEmpty();
                address.RuleFor(x => x.Zip).NotEmpty();
            });
        }
    }
}
