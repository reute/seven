using log4net;
using log4net.Config;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using SevenPrism.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SevenPrism.ViewModels
{
    public class BaseViewModel : BindableBase
    {
        protected readonly DatabaseContext Dc;
        protected readonly IEventAggregator Ea;

        protected readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public BaseViewModel(DatabaseContext dc, IEventAggregator ea)
        {
            Ea = ea;
            Dc = dc;

            XmlConfigurator.Configure();
        }
    }
}
