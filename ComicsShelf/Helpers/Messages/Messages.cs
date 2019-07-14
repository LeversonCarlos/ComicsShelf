using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf
{
   partial class App
   {

      public static async Task ShowMessage(Exception ex, [CallerMemberName]string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber]int callerLineNumber = 0)
      {
         Helpers.AppCenter.TrackEvent(ex, callerMemberName, callerFilePath, callerLineNumber);
         await ShowMessage(ex.Message);
      }

      public static async Task ShowMessage(string message)
      {
         try
         {
            Device.BeginInvokeOnMainThread(async () => await Application.Current.MainPage.DisplayAlert("Comics Shelf", message, R.Strings.BASE_OK_COMMAND));
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
