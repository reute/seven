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

namespace SevenPrism.ViewModels
{
    class CashViewModel : BindableBase
    {
        public ObservableCollection<Deposit> Deposits { get; }
        public ICollectionView DepositsCollectionView { get; }
        public ObservableCollection<Sale> Sales { get; }
        public ObservableCollection<SaleDaily> SalesDaily { get; } = new ObservableCollection<SaleDaily>();

        public string DepositsSum => Deposits.Sum(x => x.Amount).ToString();
        public string SalesDailySum => SalesDaily.Sum(x => x.Amount).ToString();

        public DelegateCommand AddNewCommand { get; }
        public DelegateCommand<object> RemoveCommand { get; }

        private DatabaseContext Db;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="depositRepo"></param>
        /// <param name="orderRepo"></param>
        public CashViewModel(DatabaseContext db)
        {  
            Db = db;        

            Sales = Db.Sales.Local.ToObservableCollection();
            Deposits = Db.Deposits.Local.ToObservableCollection();

            DepositsCollectionView = CollectionViewSource.GetDefaultView(Deposits);

            AddNewCommand = new DelegateCommand(AddNewDeposit, CanAddNewDeposit);
            RemoveCommand = new DelegateCommand<object>(RemoveDeposit, CanRemoveDeposit).ObservesProperty(() => SelectedDeposit);

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
            Sales.CollectionChanged += Orders_CollectionChanged;     

            UpdateSaleDaily(SalesDaily, Sales);
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

        private void Orders_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
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
                UpdateSaleDaily(SalesDaily, Sales);
                return;
            }
        }

        void Sale_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("Sum"))
              {
                UpdateSaleDaily(SalesDaily, Sales);
                RaisePropertyChanged(nameof(SalesDailySum));
            }

            if (e.PropertyName.Equals("Date"))
            {
                UpdateSaleDaily(SalesDaily, Sales);
                RaisePropertyChanged(nameof(SalesDailySum));
            }
        }

        private void UpdateSaleDaily(ObservableCollection<SaleDaily> salesDaily, ObservableCollection<Sale> sales)
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
