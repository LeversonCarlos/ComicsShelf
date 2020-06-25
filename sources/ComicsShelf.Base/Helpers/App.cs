using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Helpers
{
   public class App
   {

      public static Page MainPage => Application.Current.MainPage;

      // public static NavigationPage NavigationPage => MainPage as NavigationPage;

      public static INavigation Navigation => MainPage.Navigation;

      // public static void HideNavigation() => MainPage.IsPresented = false;


      public static void ShowMessage(Exception ex, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
      {
         Insights.TrackException(ex, callerMemberName, callerFilePath, callerLineNumber);
         ShowMessage(ex.Message);
      }

      public static void ShowMessage(string message)
      {
         try
         { Device.BeginInvokeOnMainThread(() => MainPage.DisplayAlert(R.Common.APP_TITLE, message, R.Common.OK_COMMAND)); }
         catch { }
      }

      public static async Task<bool> ConfirmMessage(string message)
      {
         try
         { return await MainPage.DisplayAlert(R.Common.APP_TITLE, message, R.Common.OK_COMMAND, R.Common.CANCEL_COMMAND); }
         catch { return false; }
      }


   }
}
