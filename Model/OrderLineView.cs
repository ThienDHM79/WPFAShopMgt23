using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFAShopMgt23.Services;

namespace WPFAShopMgt23.Model
{
    public class OrderLineView
    {
        public string ProductName { get; set; }
        public int Price { get; set; }
        public int Qty { get; set; }
        public int SubTotal { get; set; }
        public bool IsBuy { get; set; }
        public int ProductId { get; set; }
        public PurchaseDetail PurchaseDetail { get; set; }

        ShopDbContext _db;
        ProductService _productService;

        public OrderLineView()
        {
            _db = new ShopDbContext();
            _productService = new ProductService(_db);
        }
        public OrderLineView (Product product, int Qty)
        {
            _db = new ShopDbContext();
            _productService = new ProductService(_db);

            Product processProduct = _productService.GetProductById(product.Id);
            ProductName = processProduct.Name.ToString();
            Price = (int)processProduct.PriceInt;
            this.Qty = Qty;
            ProductId = (int)processProduct.Id;

            SubTotal = Qty * Price;

            PurchaseDetail = new PurchaseDetail()
            {
                ProductId = product.Id,
                Price = (int)product.PriceInt,
                Qty = Qty,
                Total = Qty * (int)product.PriceInt
            };

        }
    }
}
