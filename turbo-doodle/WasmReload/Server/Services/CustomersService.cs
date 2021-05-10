using System.Threading.Tasks;
using WasmReload.Shared.Interfaces;
using WasmReload.Shared.Models;

namespace WasmReload.Server.Services
{
    public class CustomersService : ICustomersService
    {
        public async Task<Customer> CreateCustomerAsync(Customer customer)
        {
            // save logic goes here
            return customer;
        }
    }
}
