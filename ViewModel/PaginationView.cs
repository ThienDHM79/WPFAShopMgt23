using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFAShopMgt23.Model;

namespace WPFAShopMgt23.ViewModel
{
    public class PaginationView
    {
        public int CurrPage { get; set; }  
        public int TotalPage { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public PaginationView(List<Product> items, int PageSize=6)
        {
            TotalItems = items.Count;
            int remain = TotalItems % PageSize;
            if (remain == 0)
            {
                TotalPage = TotalItems / PageSize;
            }
            else
            {
                TotalPage = TotalItems / PageSize + 1;
            }

        }
        public PaginationView() {
            CurrPage = 1;
        }

    }
}
