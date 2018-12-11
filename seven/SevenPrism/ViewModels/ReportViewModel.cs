using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using SevenPrism.Events;
using SevenPrism.Models;
using SevenPrism.Properties;
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

        private readonly IEventAggregator Ea;

        private DateTime _dateSelected = Settings.Default.DateSelected;

        public ReportViewModel(DatabaseContext db, IEventAggregator ea)
        {
            CreateSalesListReportCommand = new DelegateCommand(CreateSalesListReport);
            CreateArticlesListReportCommand = new DelegateCommand(CreateArticlesListReport);

            Db = db;
            Ea = ea;

            Ea.GetEvent<DateSelectedChangedEvent>().Subscribe(DateSelectedChangedHandler);
        }

        private void DateSelectedChangedHandler(DateTime date)
        {
            _dateSelected = date;
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
