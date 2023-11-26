using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFAShopMgt23.Model;

namespace WPFAShopMgt23.Services
{
    public class CustomerService
    {
        private readonly ShopDbContext _dbcontext;
        public CustomerService (ShopDbContext context)
        {
            _dbcontext = context;
        }
        public List<Customer> GetCustomers()
        {
            return new List<Customer>(_dbcontext.Customers.ToList());
        }
        public Customer? GetCustomerById(int id) {
            return _dbcontext.Customers.First(c=>c.Id == id) != null ? null : _dbcontext.Customers.AsNoTracking().First(c => c.Id == id);
        }
    }
}
