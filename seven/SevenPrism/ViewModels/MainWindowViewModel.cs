using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using Prism.Commands;
using Prism.Mvvm;
using SevenPrism.Properties;
using Prism.Regions;
using SevenPrism.Views;
using SevenPrism.CustomControls;
using SevenPrism.Repository;
using System.Linq;
using Prism.Events;
using SevenPrism.Events;
using log4net;
using System.Reflection;
using log4net.Config;

namespace SevenPrism.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
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

        private bool isValid = true;
        public new bool IsValid
        {
            get => isValid;
            set
            {
                SetProperty(ref isValid, value);               
            }
        }

        // Commands
        public DelegateCommand AboutCommand { get; }
        public DelegateCommand SaveCommand { get; }
        public DelegateCommand ExitCommand { get; }

        public string DatabasePath { get; } = Application.Current.Properties["DataSource"].ToString();
        public string Title { get; } = ApplicationInfo.ProductName;

        public MainWindowViewModel(DatabaseContext dc, IEventAggregator ea, IRegionManager regionManager) : base(dc, ea)
        {  
            // Registering Regions
            regionManager.RegisterViewWithRegion("SalesRegion", typeof(Sales));
            regionManager.RegisterViewWithRegion("CashRegion", typeof(Cash));
            regionManager.RegisterViewWithRegion("ArticlesRegion", typeof(Articles));
            regionManager.RegisterViewWithRegion("ReportRegion", typeof(Report));

            Ea.GetEvent<ValidationEvent>().Subscribe(ValidationEventHandler);

            AboutCommand = new DelegateCommand(ShowAboutMessage);
            SaveCommand = new DelegateCommand(OnSave, CanSave).ObservesProperty(() => IsValid);
            ExitCommand = new DelegateCommand(OnExit);
        }

        private void ValidationEventHandler(bool obj)
        {
            IsValid = obj;
        }

        private bool CanSave()
        { 
            return IsValid;
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
            log.Info("Saved to Db");
        }

        public void OnClosing(object sender, CancelEventArgs e)
        {
            Settings.Default.DateSelected = FromDate;
            Settings.Default.Save();
            if (Dc.ChangeTracker.HasChanges())
            {
                if (CanSave())
                {
                    if (ShowSaveChangesDialog() == MessageBoxResult.Yes)
                    {                        
                        Dc.SaveChanges();                        
                    }
                }
                else
                {
                    if (ShowLoseChanges() == MessageBoxResult.Cancel)
                    {
                        e.Cancel = true;
                    } 
                }
            }
            if (!e.Cancel)
                log.Info($"***** {ApplicationInfo.ProductName} Version {ApplicationInfo.Version} closed normally *****\n");
        }

        public MessageBoxResult ShowSaveChangesDialog()
        {
            var messageBoxText = "Do you want to save the changes?";
            var caption = "Warning";
            var buttons = MessageBoxButton.YesNo;
            var icon = MessageBoxImage.Warning;

            return MessageBox.Show(messageBoxText, caption, buttons, icon); 
        }

        public MessageBoxResult ShowLoseChanges()
        {
            var messageBoxText = "Do you really want to close the application and lose the changes you made?";
            var caption = "Warning";
            var buttons = MessageBoxButton.OKCancel;
            var icon = MessageBoxImage.Warning;

            return MessageBox.Show(messageBoxText, caption, buttons, icon);
        }
    }
}
