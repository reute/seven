﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Prism.Events;
using SevenPrism.Events;

namespace SevenPrism.Views
{
    /// <summary>
    /// Interaction logic for Sales.xaml
    /// </summary>
    public partial class Sales : UserControl
    {
        private IEventAggregator Ea;

        public Sales(IEventAggregator ea)
        {
            InitializeComponent();
            Ea = ea;
            Ea.GetEvent<GridInEditModeEvent>().Subscribe(GridInEditModeEventHandler);
        }

        private void GridInEditModeEventHandler()
        {
            OrderGrid.CancelEdit();
            OrderGrid.CancelEdit();
        }


        private void FirstTimeLoadedHandler(object sender, RoutedEventArgs e)
        {


        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void LoadedHandler(object sender, RoutedEventArgs e)
        {
            if (OrderGrid.Items.Count > 0)
            {
                OrderGrid.SelectedIndex = 0;
                FocusFirstCell();
            }
        }

        public void FocusFirstCell()
        {
            OrderGrid.Focus();
            OrderGrid.CurrentCell = new DataGridCellInfo(OrderGrid.SelectedItem, OrderGrid.Columns[0]);
        }
    }
}
