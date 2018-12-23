using SevenPrism.Views;
using Prism.Ioc;
using Prism.Modularity;
using System.Windows;
using SevenPrism.Repository;
using Microsoft.EntityFrameworkCore;
using SevenPrism.Properties;
using log4net;
using log4net.Config;
using System.Reflection;
using SevenPrism.Helpers;

namespace SevenPrism
{
    public partial class App
    {
        // Logger
        private readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public App()
        {
            XmlConfigurator.Configure();
            log.Info($"***** {ApplicationInfo.ProductName} Version {ApplicationInfo.Version} launch started *****");
        }

        protected override Window CreateShell()
        {       
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {         
            containerRegistry.RegisterSingleton(typeof(DatabaseContext));          
        }      
    }
}
