﻿using InterfaceView.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;


namespace InterfaceView.View
{
    public class IPAddressMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 2 && values[0] is string viewControlName && values[1] is IPAddress ipAddress)
            {
                if (ipAddress.ToString() != "0.0.0.0")
                {
                    return $"{viewControlName} : {ipAddress}";
                }
            }
            return values[0];
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
