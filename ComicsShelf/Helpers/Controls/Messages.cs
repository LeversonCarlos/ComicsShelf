using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Helpers.Controls
{
   public class Messages
   {

      public async Task Show(string message)
      {
         try
         { await Application.Current.MainPage.DisplayAlert(R.Strings.AppTitle, message, R.Strings.COMMAND_OK); }
         catch { }
      }

      public async Task<bool> Confirm(string message)
      {
         try
         { return await Application.Current.MainPage.DisplayAlert(R.Strings.AppTitle, message, R.Strings.COMMAND_OK, R.Strings.COMMAND_CANCEL); }
         catch { return false; }
      }

   }
}