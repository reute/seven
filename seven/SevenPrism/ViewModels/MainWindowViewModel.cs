using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using Prism.Commands;
using Prism.Mvvm;
using SevenPrism.Properties;
using Prism.Regions;
using SevenPrism.Views;
using SevenPrism.Helpers;
using SevenPrism.Models;
using SevenPrism.Services;

namespace SevenPrism.ViewModels
{   
    public class MainWindowViewModel : BindableBase
    {
        public DelegateCommand AboutCommand { get; }
        public DelegateCommand SaveCommand { get; }
        public DelegateCommand ExitCommand { get; }

        public string DatabasePath { get; set; } = Resources.NotAvailable;
        public string Title { get; set; } = ApplicationInfo.ProductName;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="regionManager"></param>
        public MainWindowViewModel(DataService ds, IRegionManager regionManager)
        {
            DataService = ds;

            AboutCommand = new DelegateCommand(ShowAboutMessage);
            SaveCommand = new DelegateCommand(OnSave, CanSave);

            regionManager.RegisterViewWithRegion("SalesRegion", typeof(Sales));
            regionManager.RegisterViewWithRegion("CashRegion", typeof(Cash));
        }

        private DataService DataService;

        private void ShowAboutMessage()
        {
            var mainWin = Application.Current.MainWindow;
            MessageBox.Show(mainWin, string.Format(CultureInfo.CurrentCulture, Resources.AboutText, ApplicationInfo.ProductName, ApplicationInfo.Version));
        }

        private bool CanSave()
        {
            return true;
        }

        private void OnSave()
        {
            DataService.Save();
        }

        public void OnClosing(object sender, CancelEventArgs e)
        {
            if (DataService.HasChanges)
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
                DataService.Save();
            }            
        }
    }
}
