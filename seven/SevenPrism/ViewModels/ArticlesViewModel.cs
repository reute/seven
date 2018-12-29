using Prism.Commands;
using Prism.Events;
using SevenPrism.Events;
using SevenPrism.Models;
using SevenPrism.Repository;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

namespace SevenPrism.ViewModels
{
    public class ArticlesViewModel : BaseViewModel
    {
        // ArticlesList
        private ObservableCollection<Article> Articles;
        public ICollectionView ArticlesCollectionView { get; }

        // Needed for Articles List
        public List<Category> Categories { get; set; }
        public List<Manufacturer> Manufacturers { get; set; }

        // FilterString for the ArticlesList
        private string _filterString = string.Empty;
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
                    Ea.GetEvent<ArticlesGridInEditModeEvent>().Publish();
                }
            }
        }

        // Commands
        public DelegateCommand AddCommand { get; }
        public DelegateCommand<object> RemoveCommand { get; }

        public ArticlesViewModel(DatabaseContext dc, IEventAggregator ea) : base(dc, ea)
        {
           
            Articles = Dc.Articles.Local.ToObservableCollection();
            ArticlesCollectionView = CollectionViewSource.GetDefaultView(Articles);        
            Categories = Dc.Categories.Local.ToList();
            Manufacturers = Dc.Manufacturers.Local.ToList();

            ArticlesCollectionView.Filter += FilterHandler;         
            ArticlesCollectionView.CurrentChanged += ArticlesCollectionView_CurrentChanged;

            AddCommand = new DelegateCommand(Add);
            RemoveCommand = new DelegateCommand<object>(Remove, CanRemove);
        }

        private void ArticlesCollectionView_CurrentChanged(object sender, EventArgs e)
        {
            RemoveCommand.RaiseCanExecuteChanged();
        }

        private void Add()
        {
            var article = new Article();
            Articles.Add(article);
            ArticlesCollectionView.MoveCurrentTo(article);
        }

        private bool CanRemove(object selectedItems)
        {
            if (selectedItems == null)
                return false;

            var listSelectedItems = (IList)selectedItems;
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
            if (item.Manufacturer.Name.IndexOf(FilterString, StringComparison.OrdinalIgnoreCase) < 0 && 
                item.Model.IndexOf(FilterString, StringComparison.OrdinalIgnoreCase) < 0 )
                return false;

            return true;
        }
    }
}
