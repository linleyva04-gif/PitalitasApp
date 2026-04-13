using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace PitalitasApp.Controllers
{
    internal class ColorController : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string estado = value?.ToString();

            return estado switch
            {
                "Pendiente" => Color.FromArgb("#FF6A00"),
                "En preparación" => Color.FromArgb("#FFC107"),
                "En camino" => Color.FromArgb("#2196F3"),
                "Entregado" => Color.FromArgb("#4CAF50"),
                _ => Colors.White
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
