using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Helpers.Controls
{
   public class Messages
   {

      public async Task Show(string message)
      {
         try
         { await Application.Current.MainPage.DisplayAlert(R.Strings.AppTitle, message, R.Strings.BASE_OK_COMMAND); }
         catch { }
      }

      public async Task<bool> Confirm(string message)
      {
         try
         { return await Application.Current.MainPage.DisplayAlert(R.Strings.AppTitle, message, R.Strings.BASE_OK_COMMAND, R.Strings.BASE_CANCEL_COMMAND); }
         catch { return false; }
      }

   }
}