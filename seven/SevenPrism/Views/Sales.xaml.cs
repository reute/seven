using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Prism.Events;
using SevenPrism.Events;
using SevenPrism.Models;

namespace SevenPrism.Views
{
    public partial class Sales : UserControl
    {
        private IEventAggregator Ea;

        public Sales(IEventAggregator ea)
        {
            InitializeComponent();
            Ea = ea;
            Ea.GetEvent<SalesGridInEditModeEvent>().Subscribe(GridInEditModeEventHandler);
        }

        private void GridInEditModeEventHandler()
        {
            SalesGrid.CancelEdit();
            SalesGrid.CancelEdit();
        }

        private void DataGridTextColumn_Error(object sender, ValidationErrorEventArgs e)
        {
            ;
        }

        //public void FocusFirstCell()
        //{
        //    SalesGrid.Focus();
        //    SalesGrid.CurrentCell = new DataGridCellInfo(SalesGrid.SelectedItem, SalesGrid.Columns[0]);
        //}   

        //private void SalesGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        //{
        //    if (IsLoaded)
        //    {
        //        SalesGrid.Focus();
        //        SalesGrid.UnselectAllCells();
        //        SalesGrid.SelectedItem = e.Row.Item;
        //    }             
        //}
    }
}
