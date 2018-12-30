using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using SevenPrism.Models;
using SevenPrism.Repository;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Events;
using SevenPrism.Events;
using SevenPrism.Properties;

namespace SevenPrism.ViewModels
{
    class CashViewModel : ViewModelBase
    { 
        // Deposits List
        private readonly ObservableCollection<Deposit> Deposits;
        public ICollectionView DepositsCollectionView { get; }
        public string DepositsSum => DepositsCollectionView.Cast<Deposit>().Sum(x => x.Amount).ToString();

        // List Sales per Day
        private readonly ObservableCollection<SalesDaily> SalesDaily = new ObservableCollection<SalesDaily>();
        public ICollectionView SalesDailyCollectionView { get; }
        public string SalesDailySum => SalesDailyCollectionView.Cast<SalesDaily>().Sum(x => x.Amount).ToString(); 

        // Sales List needed to create SalesDaily List
        private readonly ObservableCollection<Sale> Sales;

        // Dates for Filter
        private DateTime _fromDate = Settings.Default.DateSelected;
        private DateTime _toDate = DateTime.Now;

        // Commands
        public DelegateCommand AddNewDepositCommand { get; }
        public DelegateCommand<object> RemoveDepositCommand { get; } 

        public CashViewModel(DatabaseContext db, IEventAggregator ea) : base(db, ea)
        {           
            Ea.GetEvent<DateSelectedChangedEvent>().Subscribe(DateSelectedChangedHandler);    

            Sales = Dc.Sales.Local.ToObservableCollection();
            Deposits = Dc.Deposits.Local.ToObservableCollection();

            DepositsCollectionView = CollectionViewSource.GetDefaultView(Deposits);
            DepositsCollectionView.Filter += new Predicate<object>(DepositsViewFilterHandler);
            DepositsCollectionView.CurrentChanged += DepositsCollectionView_CurrentChanged;

            CreateSaleDailyCollection(SalesDaily, Sales);
            SalesDailyCollectionView = CollectionViewSource.GetDefaultView(SalesDaily);
            SalesDailyCollectionView.Filter += new Predicate<object>(SalesDailyViewFilterHandler);

            // DEPOSITS
            // 1. register for PropertyChanged event for all existing items in Deposits
            foreach (var item in Deposits)            
                item.PropertyChanged += Deposit_PropertyChanged;
            
            // 2. for all items which are going to be removed or added to Deposits
            Deposits.CollectionChanged += Deposits_CollectionChanged;

            // ORDERS
            // 1. register for PropertyChanged event for all existing items in Orders
            foreach (var item in Sales)            
                item.PropertyChanged += Sale_PropertyChanged;
            
            // 2. for all items which are going to be removed or added to Orders
            Sales.CollectionChanged += Sales_CollectionChanged; 

            AddNewDepositCommand = new DelegateCommand(AddNewDeposit);
            RemoveDepositCommand = new DelegateCommand<object>(RemoveDeposit, CanRemoveDeposit);
        }

        private void DepositsCollectionView_CurrentChanged(object sender, EventArgs e)
        {
            RemoveDepositCommand.RaiseCanExecuteChanged();
        }

        private bool DepositsViewFilterHandler(object obj)
        {
            var deposit = obj as Deposit;

            // if Sales date is older than set date
            if (deposit.Date.Date < _fromDate.Date || deposit.Date.Date > _toDate.Date)
                return false;

            return true;
        }

        private bool SalesDailyViewFilterHandler(object obj)
        {
            var saleDaily = obj as SalesDaily;

            // if saleDaily date is older than set date
            if (saleDaily.Date.Date < _fromDate.Date || saleDaily.Date.Date > _toDate.Date)
                return false;

            return true;
        }

        private void DateSelectedChangedHandler(TimePeriod timePeriod)
        {
            _fromDate = timePeriod.FromDate;
            _toDate = timePeriod.ToDate;
            DepositsCollectionView.Refresh();
            RaisePropertyChanged(nameof(DepositsSum));

            SalesDailyCollectionView.Refresh();        
            RaisePropertyChanged(nameof(SalesDailySum));
        }

        private void Deposits_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (Deposit item in e.NewItems)
                    item.PropertyChanged += Deposit_PropertyChanged;
                return;
            }

            if (e.OldItems != null)
            {
                foreach (Deposit item in e.OldItems)
                    item.PropertyChanged -= Deposit_PropertyChanged;
                RaisePropertyChanged(nameof(DepositsSum));
                return;
            }
        }

        private void Deposit_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("Amount"))            
                RaisePropertyChanged(nameof(DepositsSum));             
        }

        private void Sales_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (Sale item in e.NewItems)
                    item.PropertyChanged += Sale_PropertyChanged;              
                return;
            }

            if (e.OldItems != null)
            {
                foreach (Sale item in e.OldItems)
                    item.PropertyChanged -= Sale_PropertyChanged;
                CreateSaleDailyCollection(SalesDaily, Sales);
                return;
            }
        }

        void Sale_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("Sum") || e.PropertyName.Equals("Date"))            
                CreateSaleDailyCollection(SalesDaily, Sales); 
        }
       
        private void CreateSaleDailyCollection(ObservableCollection<SalesDaily> salesDaily, ObservableCollection<Sale> sales)
        {
            salesDaily.Clear();
            if (sales.Count == 0)
                return;
            var salesSorted = sales.OrderBy(item => item.Date);
            DateTime saleDate, lastDate = salesSorted.First().Date;
            decimal sumDay = 0;

            foreach (var sale in salesSorted)
            {
                saleDate = sale.Date;
                if (saleDate.Date == lastDate.Date)
                {
                    // current Sale belongs to same date, add to existing sum
                    sumDay += sale.Sum;
                }
                else
                {
                    // current Sale has a different date, save lastDate and its sum
                    salesDaily.Add(new SalesDaily(lastDate, sumDay));                 
                    sumDay = sale.Sum;
                }
                lastDate = saleDate;
            }
            // Add last SalesDaily
            salesDaily.Add(new SalesDaily(lastDate, sumDay));
            RaisePropertyChanged(nameof(SalesDailySum));
        }  

        private void AddNewDeposit()
        {
            var deposit = new Deposit();
            //Sale.Validate();
            Deposits.Add(deposit);
            DepositsCollectionView.MoveCurrentTo(deposit);
        }

        private bool CanRemoveDeposit(object selectedDeposits)
        {
            if (selectedDeposits == null)
                return false;

            var listSelectedDeposits = (IList)selectedDeposits;

            if (listSelectedDeposits.Count == 0)
                return false;
            else
                return true;
        }

        private void RemoveDeposit(object selectedDeposits)
        {
            var listSelectedDeposits = (IList)selectedDeposits;
            var removeList = listSelectedDeposits.Cast<Deposit>().ToList();
            int indexSale, highestIndex = 0;
            foreach (Deposit deposit in removeList)
            {
                // get highest index of all orders to be removed
                indexSale = Deposits.IndexOf(deposit);
                if (indexSale > highestIndex)
                    highestIndex = indexSale;
                // remove deposit
                Deposits.Remove(deposit);
            }
            // select Sale below last Sale which was deleted
            if (highestIndex == Deposits.Count)
            {
                DepositsCollectionView.MoveCurrentToLast();
            }
            else
            {
                var index = highestIndex - removeList.Count + 1;
                DepositsCollectionView.MoveCurrentToPosition(index);
            }
        }
    }

    public class SalesDaily : BindableBase
    {
        public DateTime Date { get; set; } = DateTime.Now;

        public decimal Amount { get; set; }

        public SalesDaily(DateTime date, decimal amount)
        {
            Date = date;
            Amount = amount;
        }
    }
}
