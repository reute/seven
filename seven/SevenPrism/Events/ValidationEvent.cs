using Prism.Events;
using SevenPrism.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SevenPrism.Events
{
    public class ValidationEvent : PubSubEvent<VmValPar>
    { }

    public struct VmValPar
    {
        public ViewModelBase Vm;
        public bool IsValid;
    }
}
