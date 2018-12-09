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
using SevenPrism.Services;
using Microsoft.EntityFrameworkCore;

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

        /// <summary> 
        /// Constructor
        /// </summary>
        /// <param name="orderRepo"></param>
        /// <param name="ea"></param>
        public SalesViewModel(DataService dataService, IEventAggregator ea)
        {
            Ea = ea;
            Sales = dataService.Sales;

            AddNewCommand = new DelegateCommand(AddNewSale, CanAddNewSale);
            RemoveCommand = new DelegateCommand<object>(RemoveSale, CanRemoveSale).ObservesProperty(() => SelectedSale);

            SalesCollectionView = CollectionViewSource.GetDefaultView(Sales);

            SalesCollectionView.Filter += new Predicate<object>(SaleViewFilterHandler);
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
            var data = obj as Sale;

            if (string.IsNullOrEmpty(FilterString))
                return true;

            //string.Compare is case sensitive
            if (data.Detail.IndexOf(FilterString, StringComparison.OrdinalIgnoreCase) >= 0)
                return true;

            return false;
        }
    }
}



//class OrderListViewModel
//{        
//    private readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
//    // State
//    private bool _needsToBeSaved;
//    private string _filePath = Properties.Settings.Default.defaultPath;
//    private string _fileExt;
//    private const string ProgramName = "Kassenbuchprogramm";

//    public decimal Kasse { get; set; }

//    private decimal _sum;
//    public decimal Sum
//    {
//        get { return _sum; }
//        set
//        {
//            _sum = value;
//            Kasse = Logic.CalcKasse(value, Change);
//        }
//    }

//    private decimal _change = Properties.Settings.Default.defaultChange;
//    public decimal Change
//    {
//        get { return _change; }
//        set
//        {
//            _change = value;
//            Kasse = Logic.CalcKasse(Sum, value);
//        }
//    }



//    public string MainWindowTitle
//    {
//        get { return _filePath + " - " + ProgramName; }
//    }

//    private List<Article> _revenues;
//    public List<Article> Revenues
//    {
//        get { return _revenues; }
//        set
//        {
//            _revenues = value;
//            _revenues.CollectionChanged += OnArticleCollectionChanged;
//            RevenuesViewSource.Source = _revenues;
//            foreach (Article article in _revenues)
//            {
//                article.PropertyChanged += OnArticlePropertyChanged;
//            }
//        }
//    }

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


//    public DateTime LetzteEinzahlung { get; set; }       

//    // Events
//    public event PropertyChangedEventHandler PropertyChanged;

//    // Constructor
//    public OrderListViewModel()
//    {            
//        var tmp = Properties.Settings.Default.defaultPath.Split('.');
//        _fileExt = tmp[(tmp.Length) - 1];
//        XmlConfigurator.Configure();
//        log.Info("Program started");

//        RevenuesViewSource = new CollectionViewSource();
//        RevenuesViewSource.Filter += RevenuesViewSourceOnFilter;
//        if (_filePath != "")
//        {
//            Revenues = OpenFile(_filePath);
//        }

//        Sum = Logic.CalcSum(Revenues);
//        Kasse = Logic.CalcKasse(Sum, Change);


//        //DataGridRevenues.ScrollIntoView(CollectionView.NewItemPlaceholder);

//        LocalizeDictionary.Instance.SetCurrentThreadCulture = true;
//        LocalizeDictionary.Instance.Culture = new CultureInfo("en-US");

//    }

//    // Event Handling

//    // User generated Events

//    // Clicks App Menu

//    private void NewFile_OnClick(object sender, RoutedEventArgs e)
//    {
//        if (_needsToBeSaved) UnsavedWarningDialog(Revenues, _filePath);
//        Revenues = new List<Article>();
//        SaveAsDialog(Revenues);
//    }

//    private void Open_OnClick(object sender, RoutedEventArgs e)
//    {
//        OpenFileDialog();
//    }

//    private void Save_OnClick(object sender, RoutedEventArgs e)
//    {
//        SaveFile(Revenues, _filePath);
//    }

//    private void SaveAs_OnClick(object sender, RoutedEventArgs e)
//    {
//        SaveAsDialog(Revenues);
//    }

//    private void Exit_OnClick(object sender, RoutedEventArgs e)
//    {
//        Application.Current.Shutdown();
//    }


//    // Clicks Ribbon Buttons

//    private void Help_OnClick(object sender, RoutedEventArgs e)
//    {
//        var about = new AboutWindow();
//        about.ShowDialog();
//    }

//    // Clicks Window

//    private void MainWindow_OnClosing(object sender, CancelEventArgs e)
//    {
//        if (_needsToBeSaved) UnsavedWarningDialog(Revenues, _filePath);
//        Properties.Settings.Default.defaultChange = Change;
//        Properties.Settings.Default.defaultPath = _filePath;
//        Properties.Settings.Default.Save();
//        log.Info("Program ended");
//    }


//    // Key Presses

//    //private void UIElement_OnKeyUp(object sender, KeyEventArgs e)
//    //{
//    //    if (e.Key == Key.Delete)
//    //    {
//    //        foreach (DataGridRow item in this.Tabelle.SelectedCells)
//    //        {
//    //            Tabelle.Rows.RemoveAt(item.Index);
//    //        }
//    //        //Debug.Write("Delete Key presed"); 
//    //    }

//    //}

//    // Automatic generated Events

//    void OnArticlePropertyChanged(object sender, PropertyChangedEventArgs e)
//    {
//        _needsToBeSaved = true;
//    }

//    private void OnArticleCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
//    {
//        _needsToBeSaved = true;
//        if (e.Action == NotifyCollectionChangedAction.Add)
//        {
//            foreach (Article article in e.NewItems)
//            {
//                article.PropertyChanged += OnArticlePropertyChanged;
//            }
//        }
//    }

//    //private void DataGridRevenues_OnAutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
//    //{
//    //    if (e.PropertyType == typeof(System.DateTime))
//    //    {
//    //        if (e.Column is DataGridTextColumn dataGridTextColumn) dataGridTextColumn.Binding.StringFormat = "d";
//    //    }
//    //}

//    private void DataGridRevenues_OnInitializingNewItem(object sender, InitializingNewItemEventArgs e)
//    {
//        Article article = e.NewItem as Article;
//        article.Datum = DateTime.Today;
//        Debug.Write("New Item");
//    }

//    // Implementation of UI Logic

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




//    private void SaveAsDialog(List<Article> colArticles)
//    {
//        var dlg = new SaveFileDialog
//        {
//            FileName = _filePath,
//            DefaultExt = _fileExt,
//            Filter = "CSV Files (.csv)|*.csv"
//        };
//        var result = dlg.ShowDialog();
//        if (result == true)
//        {
//            SaveFile(colArticles, dlg.FileName);
//        }
//    }

//    private void OpenFileDialog()
//    {
//        var dlg = new OpenFileDialog
//        {
//            FileName = _filePath,
//            DefaultExt = _fileExt,
//            Filter = "CSV Files (.csv)|*.csv",
//            RestoreDirectory = true
//        };
//        var result = dlg.ShowDialog();
//        if (result == true)
//        {
//            Revenues = OpenFile(dlg.FileName);
//            //DataGridRevenues.ScrollIntoView(CollectionView.NewItemPlaceholder);
//        }
//    }

//    protected void OnPropertyChanged(string name)
//    {
//        if (PropertyChanged != null)
//        {
//            PropertyChanged(this, new PropertyChangedEventArgs(name));
//        }
//    }

//    // Interface to Controller

//    private List<Article> OpenFile(string filePath)
//    {
//        List<Article> tmp;
//        try
//        {
//            tmp = Logic.OpenFile(filePath);
//        }
//        catch (Exception e)
//        {
//            MessageBox.Show("Error loading file");
//            log.Error("Error loading file", e);
//            return null;
//        }
//        _needsToBeSaved = false;
//        _filePath = filePath;
//        OnPropertyChanged("WindowTitle");
//        return tmp;
//    }

//    private void SaveFile(List<Article> colArticles, string filePath)
//    {
//        try
//        {
//            Logic.SaveFile(colArticles, filePath);
//        }
//        catch (Exception e)
//        {
//            MessageBox.Show("Error saving file");
//            log.Error("Error saving file", e);
//            return;
//        }
//        _needsToBeSaved = false;
//        _filePath = filePath;
//        OnPropertyChanged("WindowTitle");
//    }


//    //   private void DataGrid_OnBeginningEdit(object sender, DataGridBeginningEditEventArgs e)
//    //   {
//    ////       Debug.Write(e.Column.Header.ToString() == "Datum");

//    //   }

//    //   private void DataGrid_OnAddingNewItem(object sender, AddingNewItemEventArgs e)
//    //   {
//    //    //   Debug.Write("Added new item");
//    //   }

//    //   private void DataGrid_OnRowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
//    //   {
//    //    //   Debug.Write("DataGrid_OnRowEditEnding");
//    //   }

//    //   private void DataGrid_OnLoadingRow(object sender, DataGridRowEventArgs e)
//    //   {
//    //   //    Debug.Write("DataGrid_OnLoadingRow");
//    //   }


//    private void Abrechnung_OnClick(object sender, RoutedEventArgs e)
//    {
//    }

//    private void ArticleCollectionDataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
//    {

//    }

//}

//1:
// Datagrid
// Create PDF
// Print Dialog

// 2:
// Definition von Artikeln
// Preisberechnung
// Anzeige aktueller Kassenstand

// 3
// Warenbestand
// Statistics


