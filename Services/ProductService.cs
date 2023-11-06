using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFAShopMgt23.Services
{
    public class ProductService
    {
        private readonly ShopDbContext _dbcontext;
        public ProductService (ShopDbContext context)
        {
            _dbcontext = context;
        }
        //phase 1: not use viewmodel
        public BindingList<Product> GetAllProducts()
        {
            return new BindingList<Product>(_dbcontext.Products.ToList());
        }
        public Product GetProductById(int id) =>_dbcontext.Products.First(p => p.Id == id);
        public List<Product> GetProductsByName(string SearchKey)
        {
            if (SearchKey.IsNullOrEmpty() ){
                return _dbcontext.Products.ToList();
            }
            return _dbcontext.Products.Where(p => p.Name.Contains(SearchKey)).ToList();
        }
        /*
        public List<Product> GetProductsByCatId(int CategoryKey)
        {
            if (CategoryKey == 0)
            { 
                return _dbcontext.Products.ToList();
            }
            return _dbcontext.
        }
        */

    }
}
