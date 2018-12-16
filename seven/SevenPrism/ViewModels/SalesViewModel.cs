using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using Prism.Commands;
using Prism.Mvvm;
using SevenPrism.Models;
using SevenPrism.Repository;
using System.Linq;
using Prism.Events;
using SevenPrism.Events;
using System.Collections;
using Microsoft.EntityFrameworkCore;
using SevenPrism.Properties;
using System.Collections.Generic;
using log4net;
using System.Reflection;
using log4net.Config;
using System.Threading.Tasks;
using Squirrel;

namespace SevenPrism.ViewModels
{
    public class SalesViewModel : BindableBase
    {   
        // CollectionView for DataGrid, used for Binding, Selecting Item and Filtering
        public ICollectionView SalesCollectionView { get; }        
        // Needed for the ComboBoxes
        public List<Referent> Refs { get; set; }
        public List<Category> Categories { get; set; }
        // Commands
        public DelegateCommand AddNewSaleCommand { get; }
        public DelegateCommand<object> RemoveSaleCommand { get; }
        // FilterString for the SalesCollectionView
        public string FilterString
        {
            get => _filterString;
            set
            {
                SetProperty(ref _filterString, value);
                try
                {
                    SalesCollectionView.Refresh();
                }
                catch (InvalidOperationException)
                {
                    // Tell view to leave the edit mode that causes the exception
                    Ea.GetEvent<GridInEditModeEvent>().Publish();
                }
            }
        }

        // The DatabaseContext from EF Core
        private DatabaseContext Db;
        // Logger
        private readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        // Dates which are being set through EventAggregator
        private DateTime _fromDate = Settings.Default.DateSelected;
        private DateTime _toDate = DateTime.Now;

        public SalesViewModel(DatabaseContext db, IEventAggregator ea)
        {
            Ea = ea;
            Db = db;  

            Ea.GetEvent<DateSelectedChangedEvent>().Subscribe(DateSelectedChangedHandler);

            Sales               = Db.Sales.Local.ToObservableCollection();
            SalesCollectionView = CollectionViewSource.GetDefaultView(Sales);
            Refs                = Db.Referents.Local.ToList();
            Categories          = Db.Categories.Local.ToList();
           
            AddNewSaleCommand = new DelegateCommand(AddNewSale, CanAddNewSale);
            RemoveSaleCommand = new DelegateCommand<object>(RemoveSale, CanRemoveSale);       

            SalesCollectionView.Filter            += SaleViewFilterHandler;
            SalesCollectionView.CollectionChanged += SalesCollectionView_CollectionChanged;
            
            XmlConfigurator.Configure();
            log.Info("Program started");
        }

        private async Task CheckForUpdates()
        {
            using (var manager = new UpdateManager(@"C:\Temp\Releases"))
            {
                await manager.UpdateApp();
            }
        }

        private void SalesCollectionView_CollectionChanged(object sender, EventArgs e)
        {
            RemoveSaleCommand.RaiseCanExecuteChanged();
        }

        private void DateSelectedChangedHandler(TimePeriod timePeriod)
        {
            _fromDate   = timePeriod.FromDate;
            _toDate     = timePeriod.ToDate;
            SalesCollectionView.Refresh();      
        }

        private ObservableCollection<Sale> Sales;
        private string _filterString = string.Empty;
        private readonly IEventAggregator Ea;

        private bool CanAddNewSale()
        {
            return true;
        }

        private void AddNewSale()
        {
            var Sale = new Sale();

            //TODO: Sale.Validate();
            Sales.Add(Sale);
            SalesCollectionView.MoveCurrentTo(Sale);      
        }

        private bool CanRemoveSale(object selectedSales)
        {
            if (selectedSales == null)
                return false;

            var listSelectedSales = (IList)selectedSales;

            if (listSelectedSales.Count == 0)
                return false;
            else
                return true;
        }

        private void RemoveSale(object selectedSales)
        {
            var listSelectedSales = (IList)selectedSales;
            var removeList = listSelectedSales.Cast<Sale>().ToList();          
            foreach (Sale Sale in removeList)
            {              
                Sales.Remove(Sale);
            }          
        }

        private bool SaleViewFilterHandler(object obj)
        {
            var sale = obj as Sale;

            // if Sales date is older than set date
            if (sale.Date.Date < _fromDate.Date || sale.Date.Date > _toDate.Date)            
                return false;

            // if string is not found in sales detail column
            if (sale.Detail.IndexOf(FilterString, StringComparison.OrdinalIgnoreCase) < 0)
                return false;                

            return true;
        }
    }
}

//        LocalizeDictionary.Instance.SetCurrentThreadCulture = true;
//        LocalizeDictionary.Instance.Culture = new CultureInfo("en-US");

/* 
  
1.0


2.0

- input validation
- inventar
- azure


sonstiges

- lokalisierung
- Tests


*/





