using SevenPrism.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SevenPrism.Reports
{
    /// <summary>
    /// Interaction logic for SalesListReport.xaml
    /// </summary>
    public partial class SalesListReport : FlowDocument
    {      

        public SalesListReport(List<Sale> sales)
        {
            InitializeComponent();
            DataContext = this;
            Sales = sales;
        }

        public List<Sale> Sales { get; }

        public int SalesCount => Sales.Count;
     
    }
}
