using System.Threading.Tasks;
using WasmReload.Shared.Models;

namespace WasmReload.Shared.Interfaces
{
    public interface ICustomersService
    {
        public Task<Customer> CreateCustomerAsync(Customer customer);
    }
}
