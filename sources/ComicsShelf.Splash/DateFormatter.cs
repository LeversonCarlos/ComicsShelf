using System;
using System.Globalization;
using Xamarin.Forms;

namespace ComicsShelf.Splash
{
   public class DateFormatter : IValueConverter
   {

      public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
      {
         if (value != null)
         {
            var date = (DateTime?)value;
            if (date.HasValue && date.Value != DateTime.MinValue)
            {
               return date.Value.ToLocalTime().ToShortDateString();
            }
         }
         return DateTime.MinValue.ToShortDateString().Replace("0", "-").Replace("1", "-");
      }

      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
      {
         throw new NotImplementedException();
      }

   }
}
