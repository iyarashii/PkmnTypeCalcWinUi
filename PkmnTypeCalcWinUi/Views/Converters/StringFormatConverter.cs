﻿using Microsoft.UI.Xaml.Data;
using System;

namespace PkmnTypeCalcWinUi.Views.Converters
{
    public class StringFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (parameter is string stringFormat)
            {
                return string.Format(System.Globalization.CultureInfo.InvariantCulture, stringFormat, value);
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}