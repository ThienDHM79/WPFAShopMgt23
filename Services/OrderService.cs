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
        public OrderService (ShopDbContext context)
        {
            _dbcontext = context;
        }
        public List<Purchase> GetAllPurchases()
        {
            return new List<Purchase>(_dbcontext.Purchases.ToList());
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

    }
}
