using SevenPrism.Views;
using Prism.Ioc;
using Prism.Modularity;
using System.Windows;
using SevenPrism.Repository;
using SevenPrism.Services;

namespace SevenPrism
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton(typeof(DatabaseContext));
            containerRegistry.RegisterSingleton(typeof(DataService));
            
        }
    }
}
