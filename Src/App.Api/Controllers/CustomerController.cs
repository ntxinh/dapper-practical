using System.Collections.Generic;
using System.Threading.Tasks;
using App.Api.Entities;
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
        public async Task<IEnumerable<Customer>> GetAll()
        {
            var customers = await _customerRepository.GetAllAsync();
            return customers;
        }

        [HttpGet("{id}")]
        public async Task<Customer> GetById(int id)
        {
            var customer = await _customerRepository.GetAsync(id);
            return customer;
        }

        [HttpPost]
        public async Task<int> Create(Customer customer)
        {
            var affectedRows = await _customerRepository.AddAsync(customer);
            return affectedRows;
        }

        [HttpPost("/CreateRange")]
        public async Task<int> CreateRange()
        {
            var customers = new List<Customer>();

            for (var i = 0; i < 5; i++)
            {
                customers.Add(new Customer
                {
                    LastName = "aaa",
                    FirstName = "bbb",
                    Email = "ccc@mail.com",
                });
            }

            var affectedRows = await _customerRepository.AddAsync(customers);
            return affectedRows;
        }

        [HttpPut]
        public async Task<int> UpdateById(Customer customer)
        {
            var affectedRows = await _customerRepository.UpdateAsync(customer);
            return affectedRows;
        }

        [HttpDelete("{id}")]
        public async Task<int> DeleteById(int id)
        {
            var affectedRows = await _customerRepository.DeleteAsync(id);
            return affectedRows;
        }

        [HttpDelete("DeleteSoft/{id}")]
        public async Task<int> DeleteSoftById(int id)
        {
            var affectedRows = await _customerRepository.DeleteSoftAsync(id);
            return affectedRows;
        }
    }
}
