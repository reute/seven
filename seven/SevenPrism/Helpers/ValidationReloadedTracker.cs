﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace SevenPrism.Helpers
{
    // This class stores the ValidationErrors of an unloaded Control. When the Control is loaded again then
    // it restores the ValidationErrors.
    internal class ValidationReloadedTracker
    {
        private readonly ValidationTracker validationTracker;
        private readonly IEnumerable<ValidationError> errors;


        public ValidationReloadedTracker(ValidationTracker validationTracker, object validationSource,
            IEnumerable<ValidationError> errors)
        {
            this.validationTracker = validationTracker;
            this.errors = errors;

            if (validationSource is FrameworkElement element)
            {
                element.Loaded += ValidationSourceLoaded;
            }
            else
            {
                ((FrameworkContentElement)validationSource).Loaded += ValidationSourceLoaded;
            }
        }


        private void ValidationSourceLoaded(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element)
            {
                element.Loaded -= ValidationSourceLoaded;
            }
            else
            {
                ((FrameworkContentElement)sender).Loaded -= ValidationSourceLoaded;
            }
            validationTracker.AddErrors(sender, errors);
        }
    }
}
