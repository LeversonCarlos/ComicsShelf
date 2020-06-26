using ComicsShelf.Base.Resources;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Helpers
{
   public class Message
   {
      private static Page MainPage => Application.Current.MainPage;
      private static string AppTitle => "Comics Shelf";

      public static void Show(Exception ex, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
      {
         Helpers.Insights.TrackException(ex, callerMemberName, callerFilePath, callerLineNumber);
         Show(ex.Message);
      }

      public static void Show(string message)
      {
         try
         { Device.BeginInvokeOnMainThread(() => MainPage.DisplayAlert(AppTitle, message, Translations.OK_COMMAND)); }
         catch { }
      }

      public static async Task<bool> Confirm(string message)
      {
         try
         { return await MainPage.DisplayAlert(AppTitle, message, Translations.OK_COMMAND, Translations.CANCEL_COMMAND); }
         catch { return false; }
      }

   }
}
