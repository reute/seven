using log4net;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using SevenPrism.Events;
using SevenPrism.Models;
using SevenPrism.Repository;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SevenPrism.ViewModels
{
    public class ArticlesViewModel : BindableBase
    {
        // CollectionView for DataGrid, used for Binding, Selecting Item and Filtering
        public ICollectionView ArticlesCollectionView { get; }
        // Needed for the ComboBoxes
        public List<Category> Categories { get; set; }
        // Commands
        public DelegateCommand AddCommand { get; }
        public DelegateCommand<object> RemoveCommand { get; }
        // FilterString for the SalesCollectionView
        public string FilterString
        {
            get => _filterString;
            set
            {
                SetProperty(ref _filterString, value);
                try
                {
                    ArticlesCollectionView.Refresh();
                }
                catch (InvalidOperationException)
                {
                    // Tell view to leave the edit mode that causes the exception
                    //Ea.GetEvent<GridInEditModeEvent>().Publish();
                }
            }
        }

        // The DatabaseContext from EF Core
        private DatabaseContext Db;
        // Logger
        private readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public ArticlesViewModel(DatabaseContext db, IEventAggregator ea)
        {
            Ea = ea;
            Db = db;        

            Articles = Db.Articles.Local.ToObservableCollection();
            ArticlesCollectionView = CollectionViewSource.GetDefaultView(Articles);        
            Categories = Db.Categories.Local.ToList();

            AddCommand = new DelegateCommand(Add, CanAdd);
            RemoveCommand = new DelegateCommand<object>(Remove, CanRemove);

            ArticlesCollectionView.Filter += FilterHandler;
            ArticlesCollectionView.CollectionChanged += CollectionChanged;             
        }
        

        private void CollectionChanged(object sender, EventArgs e)
        {
            RemoveCommand.RaiseCanExecuteChanged();
        }    

        private ObservableCollection<Article> Articles;
        private string _filterString = string.Empty;
        private readonly IEventAggregator Ea;

        private bool CanAdd()
        {
            return true;
        }

        private void Add()
        {
            var article = new Article();
            Articles.Add(article);
            ArticlesCollectionView.MoveCurrentTo(article);
        }

        private bool CanRemove(object selectedItem)
        {
            if (selectedItem == null)
                return false;

            var listSelectedItems = (IList)selectedItem;

            if (listSelectedItems.Count == 0)
                return false;
            else
                return true;
        }

        private void Remove(object selectedItems)
        {
            var list = (IList)selectedItems;
            var enumerable = list.Cast<Article>();

            for (int i = enumerable.Count() - 1; i >= 0; i--)
            {
                Articles.Remove(enumerable.ElementAt(i));
            }
        }

        private bool FilterHandler(object obj)
        {
            var item = obj as Article;         

            // if string is not found in sales detail column
            if (item.Manufacturer.IndexOf(FilterString, StringComparison.OrdinalIgnoreCase) < 0 || 
                item.Model.IndexOf(FilterString, StringComparison.OrdinalIgnoreCase) < 0 )
                return false;

            return true;
        }

    }
}
