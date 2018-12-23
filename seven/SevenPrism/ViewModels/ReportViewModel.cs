﻿using log4net;
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
        private FlowDocument _report;
        public FlowDocument Report
        {
            get => _report;
            set => SetProperty(ref _report, value);
        }

        private DateTime _fromDate = Settings.Default.DateSelected;
        private DateTime _toDate = DateTime.Now;

        // Dependencies
        private readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly DatabaseContext Db;
        private readonly IEventAggregator Ea;

        public DelegateCommand CreateSalesListReportCommand { get; set; }
        public DelegateCommand CreateArticlesListReportCommand { get; set; }

        public ReportViewModel(DatabaseContext db, IEventAggregator ea)
        {
            Db = db;
            Ea = ea;
            Ea.GetEvent<DateSelectedChangedEvent>().Subscribe(DateSelectedChangedHandler);

            CreateSalesListReportCommand = new DelegateCommand(CreateSalesListReport);
            CreateArticlesListReportCommand = new DelegateCommand(CreateArticlesListReport);
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
    }
}
