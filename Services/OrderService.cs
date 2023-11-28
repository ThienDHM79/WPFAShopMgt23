using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFAShopMgt23.Model;

namespace WPFAShopMgt23.Services
{
    public class OrderService
    {
        private readonly ShopDbContext _dbcontext;
        CustomerService _customerService;
        public OrderService (ShopDbContext context)
        {
            _dbcontext = context;
            _customerService = new CustomerService(context);
        }
        public List<Purchase> GetAllPurchases()
        {
            return new List<Purchase>(_dbcontext.Purchases.ToList());
        }
        public Purchase GetPurchaseById(int id)
        {
            return _dbcontext.Purchases.AsNoTracking().First(pur => pur.Id == id);
        }
        public List<PurchaseDetail> GetDetailsFromPurchaseId(int purchaseId)
        {
            return new List<PurchaseDetail>(_dbcontext.PurchaseDetails.Where(purdetail => purdetail.PurchaseId == purchaseId).ToList());
        }
        public List<Purchase> GetPurchasebyDateRng(DateTime DateFrom, DateTime DateTo)
        {
            return new List<Purchase>(_dbcontext.Purchases.Where(pur => 
                pur.CreatedDate >= DateFrom && 
                pur.CreatedDate <= DateTo).ToList()
                );
        }
        public List<PurchaseDetail> GetDetailsbyDateRange(DateTime DateFrom, DateTime DateTo)
        {
            List<Purchase> PurchaseList = new List<Purchase>(_dbcontext.Purchases.Where(pur => 
                pur.CreatedDate >= DateFrom && 
                pur.CreatedDate <= DateTo).ToList()
                );
            List<PurchaseDetail> DetailsList = new List<PurchaseDetail>();
            foreach(var pur in PurchaseList)
            {
                List<PurchaseDetail> OneDetailList = GetDetailsFromPurchaseId(pur.Id);
                if (OneDetailList != null && OneDetailList.Count > 0)
                { DetailsList.AddRange(OneDetailList); }
            }
            return DetailsList;
        }
        public int? GetCustomerIdByPurchaseId(int purchaseId)
        {
            int? CustomerId = (int?)_dbcontext.Purchases.First(pur => pur.Id == purchaseId).CustomerId;
            return CustomerId;
        }

        public List<Purchase> GetOrdersByPage(int skip, int RowsPerPage = 5)
        {
            return _dbcontext.Purchases.Skip(skip).Take(RowsPerPage).ToList();
        }
        public int GetPurchaseCount()
        {
        return _dbcontext.Purchases.Count();
        }
    }
}
