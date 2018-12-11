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
    class CashViewModel : BindableBase
    {
        public ObservableCollection<Deposit> Deposits { get; }
        public ICollectionView DepositsCollectionView { get; }

        public ObservableCollection<SaleDaily> SalesDaily { get; } = new ObservableCollection<SaleDaily>();
        public ICollectionView SalesDailyCollectionView { get; }

        public ObservableCollection<Sale> Sales { get; }

        public string DepositsSum => DepositsCollectionView.Cast<Deposit>().Sum(x => x.Amount).ToString();
        public string SalesDailySum => SalesDailyCollectionView.Cast<SaleDaily>().Sum(x => x.Amount).ToString();

        public DelegateCommand AddNewCommand { get; }
        public DelegateCommand<object> RemoveCommand { get; }

        private readonly IEventAggregator Ea;

        private DateTime _fromDate = Settings.Default.DateSelected;
        private DateTime _toDate = DateTime.Now;

        private DatabaseContext Db;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="depositRepo"></param>
        /// <param name="orderRepo"></param>
        public CashViewModel(DatabaseContext db, IEventAggregator ea)
        {  
            Db = db;
            Ea = ea;

            Ea.GetEvent<DateSelectedChangedEvent>().Subscribe(DateSelectedChangedHandler);

            AddNewCommand = new DelegateCommand(AddNewDeposit, CanAddNewDeposit);
            RemoveCommand = new DelegateCommand<object>(RemoveDeposit, CanRemoveDeposit).ObservesProperty(() => SelectedDeposit);

            Sales = Db.Sales.Local.ToObservableCollection();
            Deposits = Db.Deposits.Local.ToObservableCollection();

            DepositsCollectionView = CollectionViewSource.GetDefaultView(Deposits);
            DepositsCollectionView.Filter += new Predicate<object>(DepositsViewFilterHandler);

            SalesDailyCollectionView = CollectionViewSource.GetDefaultView(SalesDaily);
            SalesDailyCollectionView.Filter += new Predicate<object>(SalesDailyViewFilterHandler);

            // DEPOSITS
            // register for PropertyChanged event
            // 1. for all existing items in Deposits
            foreach (var item in Deposits)
            {
                item.PropertyChanged += Deposit_PropertyChanged;
            }
            // 2. for all items which are going to be removed or added to Deposits
            Deposits.CollectionChanged += Deposits_CollectionChanged;

            // ORDERS
            // register for PropertyChanged event
            // 1. for all existing items in Orders
            foreach (var item in Sales)
            {
                item.PropertyChanged += Sale_PropertyChanged;
            }
            // 2. for all items which are going to be removed or added to Orders
            Sales.CollectionChanged += Sales_CollectionChanged;     

            CreateSaleDailyCollection(SalesDaily, Sales);
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
            var saleDaily = obj as SaleDaily;

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
            {
                RaisePropertyChanged(nameof(DepositsSum));
            } 
        }

        private void Sales_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (Sale item in e.NewItems)
                    item.PropertyChanged += Sale_PropertyChanged;

                CreateSaleDailyCollection(SalesDaily, Sales);
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
            if (e.PropertyName.Equals("Sum"))
              {
                CreateSaleDailyCollection(SalesDaily, Sales);
                RaisePropertyChanged(nameof(SalesDailySum));
            }

            if (e.PropertyName.Equals("Date"))
            {
                CreateSaleDailyCollection(SalesDaily, Sales);
                RaisePropertyChanged(nameof(SalesDailySum));
            }
        }

        /// <summary>
        /// Creates the SaleDaily ObservableCollection
        /// </summary>
        /// <param name="salesDaily"></param>
        /// <param name="sales"></param>
        private void CreateSaleDailyCollection(ObservableCollection<SaleDaily> salesDaily, ObservableCollection<Sale> sales)
        {
            salesDaily.Clear();
            if (sales.Count == 0)
                return;
            var salesSorted = sales.OrderBy(item => item.Date);
            DateTime saleDate, lastDate = salesSorted.First().Date;
            decimal sumDay = 0;

            foreach (var Sale in salesSorted)
            {
                saleDate = Sale.Date;
                if (saleDate.Date == lastDate.Date)
                {
                    if (Sale.Sum != null)
                        // current Sale belongs to same date, add to existing sum
                        sumDay += (decimal)Sale.Sum;
                }
                else
                {
                    // current Sale has a different date, save lastDate and its sum
                    salesDaily.Add(new SaleDaily(lastDate, sumDay));
                    if (Sale.Sum != null)
                        sumDay = (decimal)Sale.Sum;
                }
                lastDate = saleDate;
            }
            salesDaily.Add(new SaleDaily(lastDate, sumDay));
        }

        //private ObservableCollection<SaleDaily> UpdateSalesDailyItem(ObservableCollection<Sale> orders, PropertyChangedEventArgs e)
        //{
        //    // remove SaleDaily item
        //    SalesDaily.Remove();
        //    var newEarning = CreateNewEarning(orders, e);
        //    SalesDaily.Add(newEarning);      
        //}


        //private SaleDaily CreateNewEarning(ObservableCollection<Sale> orders, PropertyChangedEventArgs e)
        //{
        //    return new SaleDaily(DateTime.Now, 100);
        //}

        private bool CanAddNewDeposit()
        {
            return true;
        }

        public Deposit SelectedDeposit
        {
            get => selectedDeposit;
            set => SetProperty(ref selectedDeposit, value);
        }

        private Deposit selectedDeposit;

        private void AddNewDeposit()
        {
            var deposit = new Deposit();
            //Sale.Validate();
            Deposits.Add(deposit);
            SelectedDeposit = deposit;
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
                SelectedDeposit = Deposits.Last();
            }
            else
            {
                var index = highestIndex - removeList.Count + 1;
                SelectedDeposit = Deposits[index];
            }
        }
    }

    public class SaleDaily : BindableBase
    {
        public DateTime Date { get; set; } = DateTime.Now;

        public decimal Amount { get; set; }

        public SaleDaily(DateTime date, decimal amount)
        {
            Date = date;
            Amount = amount;
        }
    }
}
