using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFAShopMgt23.Services;

namespace WPFAShopMgt23.Model
{
    public class OrderView
    {
        public int PurchaseId { get; set; }
        public string PurchaseDate { get; set; }
        public double PurchaseTotal { get; set; }
        public string CustomerName { get; set; }
        public string CustomerTel { get; set; }
        ShopDbContext _db;
        CustomerService customerService;
        public OrderView()
        {

        }
        public OrderView(Purchase purchase)
        {
            this.PurchaseId = purchase.Id;
            this.PurchaseDate = purchase.CreatedDate.ToString();
            this.PurchaseTotal = (double)purchase.FinalTotal;
            _db = new ShopDbContext();
            customerService = new CustomerService(_db);
            Customer customer = new Customer();
            if (purchase.CustomerId == null)
            {
                this.CustomerName = string.Empty;
            }
            else
            {
                customer = customerService.GetCustomerById((int)purchase.CustomerId);
            }

            if (customer == null) 
                this.CustomerName = string.Empty;
            if (customer != null)
            {
                this.CustomerName = customer.Name;
                this.CustomerTel = customer.Tel;
            }
                

        }
    }
}
