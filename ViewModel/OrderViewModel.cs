using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFAShopMgt23.Model;
using WPFAShopMgt23.Services;

namespace WPFAShopMgt23.ViewModel
{
    public class OrderViewModel
    {
        private IList<Purchase> _PurchaseList;
        private IList<Customer> _CustomerList;
        CustomerService _customerService;
        ShopDbContext _db;
        public OrderViewModel()
        {
            _db = new ShopDbContext();
            _customerService = new CustomerService(_db);
        }
        public IList<Purchase> PurchaseList {  get; set; }
        public IList<Customer> CustomerList { get; set; }
        
    }
}
