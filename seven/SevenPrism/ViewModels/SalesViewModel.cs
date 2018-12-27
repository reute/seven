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
using SevenPrism.Properties;
using System.Collections.Generic;
using log4net;
using System.Reflection;
using System.Threading.Tasks;
using Squirrel;
using log4net.Config;
using SevenPrism.CustomControls;
using System.Collections.Specialized;

namespace SevenPrism.ViewModels
{
    public class SalesViewModel : BindableBase
    {
        // Sales List
        private readonly ObservableCollection<Sale> Sales;
        public ICollectionView  SalesCollectionView { get; }    
        
        // Additional Lists needed for SalesList
        public List<Referent> Refs { get; set; }
        public List<Category> Categories { get; set; }
        public ObservableCollection<Article> Articles { get; }

        // For filtering Sales List
        private string _filterString = string.Empty;
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

        // Dependencies
        private readonly DatabaseContext Db;      
        private readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType); 
        private readonly IEventAggregator Ea;

        // Dates 
        private DateTime _fromDate = Settings.Default.DateSelected;
        private DateTime _toDate = DateTime.Now;

        // Commands
        public DelegateCommand AddNewSaleCommand { get; }
        public DelegateCommand<object> RemoveSaleCommand { get; }

        public SalesViewModel(DatabaseContext db, IEventAggregator ea)
        {
            Ea = ea;
            Db = db;
            XmlConfigurator.Configure();
            Ea.GetEvent<DateSelectedChangedEvent>().Subscribe(DateSelectedChangedHandler);

            Sales               = Db.Sales.Local.ToObservableCollection();
            SalesCollectionView = CollectionViewSource.GetDefaultView(Sales);
            Refs                = Db.Referents.Local.ToList();
            Categories          = Db.Categories.Local.ToList();
            Articles            = Db.Articles.Local.ToObservableCollection(); 

            SalesCollectionView.Filter          += SaleViewFilterHandler;         
            SalesCollectionView.CurrentChanged  += SalesCollectionView_CurrentChanged;

            AddNewSaleCommand = new DelegateCommand(AddNewSale);
            RemoveSaleCommand = new DelegateCommand<object>(RemoveSale, CanRemoveSale);

            log.Info($"***** {ApplicationInfo.ProductName} Version {ApplicationInfo.Version} launch completed *****");

            CheckForUpdates();
        }

        private void SalesCollectionView_CurrentChanged(object sender, EventArgs e)
        {
            RemoveSaleCommand.RaiseCanExecuteChanged();
        }

        private async Task CheckForUpdates()
        {
            using (var manager = new UpdateManager(@"C:\tmp\releases"))
            {
                await manager.UpdateApp();
            }
        }   

        private void DateSelectedChangedHandler(TimePeriod timePeriod)
        {
            _fromDate   = timePeriod.FromDate;
            _toDate     = timePeriod.ToDate;
            SalesCollectionView.Refresh();      
        }       

        private void AddNewSale()
        {
            var Sale = new Sale();      
            Sales.Add(Sale);
            SalesCollectionView.MoveCurrentTo(Sale);      
        }

        private bool CanRemoveSale(object selectedItems)
        {
            if (selectedItems == null)
                return false;

            var listSelectedItems = (IList)selectedItems;
            if (listSelectedItems.Count == 0)
                return false;
            else
                return true;
        }

        private void RemoveSale(object selectedItems)
        {
            var listSelectedItems = (IList)selectedItems;
            var removeList = listSelectedItems.Cast<Sale>().ToList();          
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
            if (sale.ArticleDescription.IndexOf(FilterString, StringComparison.OrdinalIgnoreCase) < 0)
                return false;                

            return true;
        }
    }
}

//        LocalizeDictionary.Instance.SetCurrentThreadCulture = true;
//        LocalizeDictionary.Instance.Culture = new CultureInfo("en-US");

/* 
  
1.0
 - DB migration
 - run without privilidges
 - installation procedure
 - logging
 - lokalisierung

2.0

- input validation
- inventar
- azure


sonstiges

- Tests


*/





