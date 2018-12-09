using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SevenPrism.ViewModels
{
    public class ReportViewModel : BindableBase
    {
        private object report;

        public ReportViewModel()
        {
        }

        public object Report
        {
            get => report;
            set => SetProperty(ref report, value);
        }

        public DelegateCommand CreateSalesListReportCommand { get; set; }

        public DelegateCommand CreatePricelistReportCommand { get; set; }
    }
}
