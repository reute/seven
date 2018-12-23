﻿using SevenPrism.Models;
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

namespace SevenPrism.Reports
{
    /// <summary>
    /// Interaction logic for ArticlesListReport.xaml
    /// </summary>
    public partial class ArticlesListReport : FlowDocument
    {
        public ArticlesListReport(List<Article> articles)
        {
            InitializeComponent();
            DataContext = this;
            Articles = articles;
        }

        public List<Article> Articles { get; }

        public int ArticlesCount => Articles.Count;

        public DateTime Date => DateTime.Now;
    }
}
