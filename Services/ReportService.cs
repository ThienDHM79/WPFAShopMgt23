using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using Microsoft.Identity.Client;
using Rh.DateRange.Picker.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFAShopMgt23.Model;

namespace WPFAShopMgt23.Services
{
    public class ReportService
    {
        private readonly ShopDbContext _dbcontext;
        CategoryService _categoryService;
        OrderService _orderService;
        ProductService _productService;

        public ReportService( ShopDbContext context)
        {
            _dbcontext = context;
            _categoryService = new CategoryService(context);
            _orderService = new OrderService(context);
            _productService = new ProductService(context);
        }
        //public SeriesCollection GetSaleSeries(DateTime DateFrom, DateTime DateTo)
        public LiveCharts.SeriesCollection GetSaleSeries(DateTime DateFrom, DateTime DateTo)
        {
            SalesReport sale = new SalesReport();
            List<Category> categories = _categoryService.GetAllCategories();
            List<PurchaseDetail> saleItems = _orderService.GetDetailsbyDateRange(DateFrom, DateTo);
            //lay gia tri sale cua moi category
            //1. lay saleitems co product id. trace categoy id. sum purchasedetail.total
            Dictionary<string,int> SaleperCates = new Dictionary<string,int>(categories.Count);
            foreach (Category cat in categories)
            {
                SaleperCates.Add(cat.Name,0 );
            }
            if (saleItems.Count > 0)
            {
                foreach (var item in saleItems)
                {
                    var cateId = _productService.GetCateIdByProductId((int)item.ProductId);
                    var cateName = _categoryService.GetCategorybyId(cateId).Name;
                    SaleperCates[cateName] += (int)item.Total;
                }
            }
            //debug
            foreach( var cate in SaleperCates )
            {
                Debug.WriteLine(cate.Value);
            }
            LiveCharts.SeriesCollection SalesCollection = new LiveCharts.SeriesCollection();
            foreach( var cate in SaleperCates)
            {
                PieSeries pieItem = new PieSeries()
                {
                    Title = cate.Key.ToString(),
                    Values = new ChartValues<int> {cate.Value },
                    DataLabels = false
                };
                SalesCollection.Add(pieItem);
                Debug.WriteLine(pieItem.Title);
            }
            return SalesCollection;
            
        }
        public Dictionary<string,DateTime> GetDateRangeFromWeek(int week,int year)
        {
            var DateFrom = ISOWeek.ToDateTime(year, week, DayOfWeek.Monday);
            var DateTo = ISOWeek.ToDateTime(year, week, DayOfWeek.Sunday);
            var dict = new Dictionary<string,DateTime>();
            dict.Add("DateFrom", DateFrom);
            dict.Add("DateTo", DateTo);
            return dict;
        }
        public Dictionary<string, DateTime> GetDateRangeFromMonth(int month, int year)
        {
            var DateFrom = new DateTime(year,month,1);
            var DateTo = DateFrom.AddMonths(1).AddSeconds(-1);
           
            var dict = new Dictionary<string, DateTime>();
            dict.Add("DateFrom", DateFrom);
            dict.Add("DateTo", DateTo);
            return dict;
        }
        public Dictionary<string,int> GetTopReportList(DateTime DateFrom, DateTime DateTo, int limit = 5)
        {
            List<PurchaseDetail> DetailList = new List<PurchaseDetail>(_orderService.GetDetailsbyDateRange(DateFrom,DateTo));
            var ProductIdList = DetailList.Select(d => d.ProductId).Distinct().ToList();
            Dictionary<int, int> ReportList = new Dictionary<int, int>();
            foreach (var productid in ProductIdList)
            {
                ReportList.Add((int)productid, 0);
            }
            foreach(var line in DetailList)
            {
                if (line.ProductId != null)
                {
                    ReportList[(int)line.ProductId] += (int)((line.Qty == null) ? 0:line.Qty );
                }

            }
            var reportItems = ReportList.OrderByDescending(p => p.Value).ToList();
            Dictionary<string,int> ReportNameQty = new Dictionary<string,int>();
            foreach (var  item in reportItems)
            {
                ReportNameQty.Add(_productService.GetProductById(item.Key).Name, item.Value);
            }
            return ReportNameQty;
        }
    }
}
