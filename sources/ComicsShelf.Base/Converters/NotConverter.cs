using System;
using System.Globalization;
using Xamarin.Forms;

namespace ComicsShelf.Base.Converters
{
   public class NotConverter : IValueConverter
   {
      public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
      {
         return !(bool)value;
      }

      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
      {
         return !(bool)value;
      }
   }
}