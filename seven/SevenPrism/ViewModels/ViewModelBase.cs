using log4net;
using log4net.Config;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using SevenPrism.Events;
using SevenPrism.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SevenPrism.ViewModels
{
    public abstract class ViewModelBase : BindableBase
    {
        protected readonly DatabaseContext Dc;
        protected readonly IEventAggregator Ea;

        protected readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private bool isValid = true;
        public bool IsValid
        {
            get => isValid;
            set
            {
                SetProperty(ref isValid, value);
                Ea.GetEvent<ValidationEvent>().Publish(value);
            }
        }

        public ViewModelBase(DatabaseContext dc, IEventAggregator ea)
        {
            Ea = ea;
            Dc = dc;

            XmlConfigurator.Configure();
        }
    }
}
