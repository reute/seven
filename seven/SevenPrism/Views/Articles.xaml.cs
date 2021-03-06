﻿using Prism.Events;
using SevenPrism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SevenPrism.Views
{
    /// <summary>
    /// Interaction logic for Articles.xaml
    /// </summary>
    public partial class Articles : UserControl
    {
        public Articles(IEventAggregator ea)
        {
            InitializeComponent();
            ea.GetEvent<ArticlesGridInEditModeEvent>().Subscribe(GridInEditModeEventHandler);
        }

        private void GridInEditModeEventHandler()
        {
            ArticlesGrid.CancelEdit();
            ArticlesGrid.CancelEdit();
        }
    }
}
