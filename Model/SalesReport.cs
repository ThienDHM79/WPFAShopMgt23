using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFAShopMgt23.Model
{
    public class SalesReport
    {
        public DateOnly DateFrom {  get; set; }
        public DateOnly DateTo { get; set;}
        public int WeekNum { get; set; }
        public int MonthNum { get; set; }
        public int YearNum { get; set; }
        public List<Category> Categories { get; set; }
        public List<PurchaseDetail> purchaseDetailList { get; set; }
        
        public SalesReport()
        {

            DateFrom = new DateOnly(2023,1,1);
            var today = DateTime.Now;
            DateTo = new DateOnly(today.Year, today.Month, today.Day);
            WeekNum = ISOWeek.GetWeekOfYear(today);
            MonthNum = today.Month;
            YearNum = today.Year;
            
        }
        public SalesReport(DateOnly dateFrom, DateOnly dateTo)
        {
            DateFrom = dateFrom;
            DateTo = dateTo;
        }
        public SalesReport(int? weekNum, int? monthNum, int? yearNum)
        {
            WeekNum = (weekNum == null) ? (int)ISOWeek.GetWeekOfYear(DateTime.Now) : (int)weekNum;
            MonthNum = (monthNum == null)? (int)DateTime.Now.Month : (int)monthNum;
            YearNum = (yearNum == null) ? (int)DateTime.Now.Year : (int)yearNum;
        }
    }
}
