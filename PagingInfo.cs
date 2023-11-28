using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFAShopMgt23
{
    class PagingInfo
    {
        public int TotalItems { get; set; }
        public int RowsPerPage { get; set; }
        public int TotalPages => (TotalItems / RowsPerPage) +
           ( (TotalItems % RowsPerPage == 0) ? 0: 1);
        public int CurrentPage { get; set; }

        public int TotalCurrentItems { get; set; }
        public string Display => $"{CurrentPage}/{TotalPages}";
    }

}
