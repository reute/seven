﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace SevenPrism.Converter
{
    public sealed class ValidationErrorsConverter : IMultiValueConverter
    {
        /// <summary>
        /// Gets the default instance of this converter.
        /// </summary>
        public static ValidationErrorsConverter Default { get; } = new ValidationErrorsConverter();


        /// <summary>
        /// Converts a collection of <see cref="ValidationError"/> objects into a multi-line string of error messages.
        /// </summary>
        /// <param name="values">The first value is the collection of <see cref="ValidationError"/> objects.</param>
        /// <param name="targetType">The type of the binding target property. This parameter will be ignored.</param>
        /// <param name="parameter">The converter parameter to use. This parameter will be ignored.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A multi-line error message or an empty string when the collection contains no errors. If the first value parameter is <c>null</c>
        /// or not of the type IEnumerable&lt;ValidationError&gt; this method returns <see cref="DependencyProperty.UnsetValue"/>.
        /// </returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values?.FirstOrDefault() is IEnumerable<ValidationError> validationErrors)
            {
                return string.Join(Environment.NewLine, validationErrors.Select(x => x.ErrorContent.ToString()));
            }
            return DependencyProperty.UnsetValue;
        }

        /// <summary>
        /// This method is not supported and throws an exception when it is called.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetTypes">The array of types to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>Nothing because this method throws an exception.</returns>
        /// <exception cref="NotSupportedException">Throws this exception when the method is called.</exception>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
