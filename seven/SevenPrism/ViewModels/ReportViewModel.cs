using log4net;
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
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Documents;

namespace SevenPrism.ViewModels
{
    public class ReportViewModel : BindableBase
    {
        private readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        DatabaseContext Db;

        private readonly IEventAggregator Ea;

        private DateTime _fromDate = Settings.Default.DateSelected;
        private DateTime _toDate = DateTime.Now;

        public ReportViewModel(DatabaseContext db, IEventAggregator ea)
        {
            CreateSalesListReportCommand = new DelegateCommand(CreateSalesListReport);
            CreateArticlesListReportCommand = new DelegateCommand(CreateArticlesListReport);

            Db = db;
            Ea = ea;

            Ea.GetEvent<DateSelectedChangedEvent>().Subscribe(DateSelectedChangedHandler);
        }

        private void DateSelectedChangedHandler(TimePeriod timePeriod)
        {
            _fromDate = timePeriod.FromDate;
            _toDate = timePeriod.ToDate;
        }

        private void CreateArticlesListReport()
        {
            Report = new ArticlesListReport(Db.Articles.ToList());
        }

        private void CreateSalesListReport()
        {          
            Report = new SalesListReport(Db.Sales.Where(x => x.Date.Date >= _fromDate.Date || x.Date.Date <= _toDate.Date).ToList());           
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
