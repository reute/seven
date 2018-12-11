﻿using System;
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
using SevenPrism.Repository;
using System.Linq;
using Prism.Events;
using SevenPrism.Events;

namespace SevenPrism.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {

        private DateTime _dateSelected = Settings.Default.DateSelected; 
        public DateTime DateSelected {
            get
            {
                return _dateSelected;
            }
            set
            {
                _dateSelected = value;
                Ea.GetEvent<DateSelectedChangedEvent>().Publish(_dateSelected);
            }
        }
        public DelegateCommand AboutCommand { get; }
        public DelegateCommand SaveCommand { get; }
        public DelegateCommand ExitCommand { get; }

        public string DatabasePath { get; set; } = Settings.Default.DatabasePath;
        public string Title { get; set; } = ApplicationInfo.ProductName;

        private readonly IEventAggregator Ea;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="regionManager"></param>
        public MainWindowViewModel(DatabaseContext dc, IRegionManager regionManager, IEventAggregator ea)
        {
            Dc = dc;
            Ea = ea;

            AboutCommand = new DelegateCommand(ShowAboutMessage);
            SaveCommand = new DelegateCommand(OnSave);
            ExitCommand = new DelegateCommand(OnExit);

            regionManager.RegisterViewWithRegion("SalesRegion", typeof(Sales));
            regionManager.RegisterViewWithRegion("CashRegion", typeof(Cash));
            regionManager.RegisterViewWithRegion("ReportRegion", typeof(Report));
        }
      

        private void OnExit()
        {
            OnClosing(this, null);
            Application.Current.Shutdown();          
        }

        private DatabaseContext Dc;

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
            Settings.Default.DateSelected = DateSelected;
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
