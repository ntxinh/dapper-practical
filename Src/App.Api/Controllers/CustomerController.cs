using System.Collections.Generic;
using System.Threading.Tasks;
using App.Api.Models;
using App.Api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace App.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerController(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        [HttpGet]
        public async Task<IEnumerable<Customer>> Get()
        {
            await _customerRepository.InsertAsync(new Customer
            {
                FirstName = "First Name 1",
                LastName = "Last Name 1",
                Email = "sample@mail.com"
            });
            var customers = await _customerRepository.GetAllAsync();
            return customers;
        }
    }
}
