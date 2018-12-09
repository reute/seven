using Prism.Commands;
using Prism.Mvvm;
using SevenPrism.Models;
using SevenPrism.Reports;
using SevenPrism.Repository;
using SevenPrism.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Documents;

namespace SevenPrism.ViewModels
{
    public class ReportViewModel : BindableBase
    {
        DatabaseContext Db;

        public ReportViewModel(DatabaseContext db)
        {
            CreateSalesListReportCommand = new DelegateCommand(CreateSalesListReport);
            CreateArticlesListReportCommand = new DelegateCommand(CreateArticlesListReport);

            Db = db;
        }

        private void CreateArticlesListReport()
        {
        }

        private void CreateSalesListReport()
        {          
            Report = new SalesListReport(Db.Sales.ToList());           
        }

        private FlowDocument _report;
        public FlowDocument Report
        {
            get => _report;
            set => SetProperty(ref _report, value);
        }

        public DelegateCommand CreateSalesListReportCommand { get; set; }

        public DelegateCommand CreateArticlesListReportCommand { get; set; }
    }
}
