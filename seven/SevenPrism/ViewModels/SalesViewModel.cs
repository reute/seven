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

namespace SevenPrism.ViewModels
{
    public class SalesViewModel : BindableBase
    {

        public ObservableCollection<Sale> Sales { get; }
        public ICollectionView SalesCollectionView { get; }

        public Sale SelectedSale
        {
            get => selectedSale;
            set => SetProperty(ref selectedSale, value);
        }

        public List<Referent> Refs { get; set; }
        public List<Category> Categories { get; set; }

        private DateTime _fromDate = Settings.Default.DateSelected;
        private DateTime _toDate = DateTime.Now;

        public DelegateCommand AddNewCommand { get; }
        public DelegateCommand<object> RemoveCommand { get; }

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

        private DatabaseContext Db;

        /// <summary> 
        /// Constructor
        /// </summary>
        /// <param name="orderRepo"></param>
        /// <param name="ea"></param>
        public SalesViewModel(DatabaseContext db, IEventAggregator ea)
        {
            Ea = ea;
            Db = db;

            Ea.GetEvent<DateSelectedChangedEvent>().Subscribe(DateSelectedChangedHandler);

            Sales = Db.Sales.Local.ToObservableCollection();
            Refs = Db.Referents.Local.ToList();
            Categories = Db.Categories.Local.ToList();

            AddNewCommand = new DelegateCommand(AddNewSale, CanAddNewSale);
            RemoveCommand = new DelegateCommand<object>(RemoveSale, CanRemoveSale).ObservesProperty(() => SelectedSale);

            SalesCollectionView = CollectionViewSource.GetDefaultView(Sales);

            SalesCollectionView.Filter += new Predicate<object>(SaleViewFilterHandler);
        }


        private void DateSelectedChangedHandler(TimePeriod timePeriod)
        {

            _fromDate = timePeriod.FromDate;
            _toDate = timePeriod.ToDate;
            SalesCollectionView.Refresh();
        }

        private Sale selectedSale;
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
            SelectedSale = Sale;
        }

        private bool CanRemoveSale(object selectedSale)
        {
            if (selectedSale == null)
                return false;

            var listSelectedSales = (IList)selectedSale;

            if (listSelectedSales.Count == 0)
                return false;
            else
                return true;
        }

        private void RemoveSale(object selectedSales)
        {
            var listSelectedSales = (IList)selectedSales;
            var removeList = listSelectedSales.Cast<Sale>().ToList();
            int indexSale, highestIndex = 0;
            foreach (Sale Sale in removeList)
            {
                // get highest index of all orders to be removed
                indexSale = Sales.IndexOf(Sale);
                if (indexSale > highestIndex)
                    highestIndex = indexSale;
                //remove Sale
                Sales.Remove(Sale);
            }
            // select Sale below last Sale which was deleted
            if (highestIndex == Sales.Count)
            {
                SelectedSale = Sales.Last();
            }
            else
            {
                var index = highestIndex - removeList.Count + 1;
                SelectedSale = Sales[index];
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

//    private readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

//    public CollectionViewSource RevenuesViewSource { get; set; }


//    private bool _letzteEinzahlungFilter;
//    public bool LetzteEinzahlungFilter
//    {
//        get { return _letzteEinzahlungFilter; }
//        set
//        {
//            _letzteEinzahlungFilter = value;
//            RevenuesViewSource.View.Refresh();
//            OnPropertyChanged("FilterVon");
//        }
//    }

//        //DataGridRevenues.ScrollIntoView(CollectionView.NewItemPlaceholder);
//        LocalizeDictionary.Instance.SetCurrentThreadCulture = true;
//        LocalizeDictionary.Instance.Culture = new CultureInfo("en-US");

//    private void MainWindow_OnClosing(object sender, CancelEventArgs e)
//    {
//        if (_needsToBeSaved) UnsavedWarningDialog(Revenues, _filePath);
//        Properties.Settings.Default.defaultChange = Change;
//        Properties.Settings.Default.defaultPath = _filePath;
//        Properties.Settings.Default.Save();

//    }

//        log.Info("Program ended");

//    // FILTER Datagrid

//    private void RevenuesViewSourceOnFilter(object sender, FilterEventArgs filterEventArgs)
//    {

//        Article element = filterEventArgs.Item as Article;
//        if (element.Datum < LetzteEinzahlung)
//        {
//            filterEventArgs.Accepted = false;
//        }
//        else
//        {
//            filterEventArgs.Accepted = true;
//        }
//        //return article.Datum.;
//        //throw new NotImplementedException();
//    }





/* 
  
1.0

- konfigurierbare comboboxen
- lokalisierung
- logging
- Tests

2.0
- input validation
- inventar
- azure

*/





