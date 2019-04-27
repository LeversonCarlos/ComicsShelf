using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf
{
   partial class App
   {

      public static async Task ShowMessage(Exception ex)
      { await ShowMessage(ex.ToString()); }

      public static async Task ShowMessage(string message)
      {
         try
         {
            await Application.Current.MainPage.DisplayAlert("Comics Shelf", message, R.Strings.BASE_OK_COMMAND);
         }
         catch { }
      }

      public static async Task<bool> ConfirmMessage(string message)
      {
         try
         { return await Application.Current.MainPage.DisplayAlert("Comics Shelf", message, R.Strings.BASE_OK_COMMAND, R.Strings.BASE_CANCEL_COMMAND); }
         catch { return false; }
      }

   }
}
