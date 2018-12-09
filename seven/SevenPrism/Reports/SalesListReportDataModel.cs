using SevenPrism.Models;
using SevenPrism.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SevenPrism.Reports
{
    public class SalesListReportDataModel 
    {
        public SalesListReportDataModel(IReadOnlyList<Sale> sales)
        {
            Sales = sales;
        }

        public IReadOnlyList<Sale> Sales { get; }

        public int SalesCount => Sales.Count;
    }
}
