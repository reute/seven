using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using Prism.Commands;
using Prism.Mvvm;
using SevenPrism.Properties;
using Prism.Regions;
using SevenPrism.Views;
using SevenPrism.Helpers;
using SevenPrism.Repository;
using System.Linq;
using Prism.Events;
using SevenPrism.Events;
using log4net;
using System.Reflection;
using log4net.Config;

namespace SevenPrism.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {  
        // From To Dates
        private DateTime _fromDate = Settings.Default.DateSelected; 
        public DateTime FromDate {
            get
            {
                return _fromDate;
            }
            set
            {
                _fromDate = value;
                Ea.GetEvent<DateSelectedChangedEvent>().Publish(new TimePeriod(value, ToDate));
            }
        }

        private DateTime _toDate = DateTime.Now;
        public DateTime ToDate
        {
            get
            {
                return _toDate;
            }
            set
            {
                _toDate = value;
                Ea.GetEvent<DateSelectedChangedEvent>().Publish(new TimePeriod(FromDate, value));
            }
        }

        // Commands
        public DelegateCommand AboutCommand { get; }
        public DelegateCommand SaveCommand { get; }
        public DelegateCommand ExitCommand { get; }

        public string DatabasePath { get; } 
        public string Title { get; } = ApplicationInfo.ProductName;

        // Dependencies
        private readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IEventAggregator Ea;
        private readonly DatabaseContext Dc;

        public MainWindowViewModel(DatabaseContext dc, IRegionManager regionManager, IEventAggregator ea)
        {     
            Dc = dc;
            Ea = ea;
            DatabasePath = Dc.FullDatabasePath;
            XmlConfigurator.Configure();

            // Registering Regions
            regionManager.RegisterViewWithRegion("SalesRegion", typeof(Sales));
            regionManager.RegisterViewWithRegion("CashRegion", typeof(Cash));
            regionManager.RegisterViewWithRegion("ArticlesRegion", typeof(Articles));
            regionManager.RegisterViewWithRegion("ReportRegion", typeof(Report));

            AboutCommand = new DelegateCommand(ShowAboutMessage);
            SaveCommand = new DelegateCommand(OnSave);
            ExitCommand = new DelegateCommand(OnExit);
        }      

        private void OnExit()
        {
            OnClosing(this, null);
            Application.Current.Shutdown();          
        }

        private void ShowAboutMessage()
        {
            var mainWin = Application.Current.MainWindow;
            MessageBox.Show(mainWin, string.Format(CultureInfo.CurrentCulture, Resources.AboutText, ApplicationInfo.ProductName, ApplicationInfo.Version));
        }       

        private void OnSave()
        {
            Dc.SaveChanges();
        }

        public void OnClosing(object sender, CancelEventArgs e)
        {
            Settings.Default.DateSelected = FromDate;
            Settings.Default.Save();
            if (Dc.ChangeTracker.HasChanges())
            {
                ShowUnsavedChangesDialog();
            }
        }

        public void ShowUnsavedChangesDialog()
        {
            var messageBoxText = "There are unsaved changes. Click Yes to save or No to discard.";
            var caption = "Obacht";
            var buttons = MessageBoxButton.YesNo;
            var icon = MessageBoxImage.Warning;

            var messageBoxResult = MessageBox.Show(messageBoxText, caption, buttons, icon);
          
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                Dc.SaveChanges();
            }            
        }
    }
}
