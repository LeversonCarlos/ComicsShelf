using ComicsShelf.ViewModels;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Screens.Settings
{
   public partial class SettingsVM : BaseVM
   {

      public SettingsVM()
      {
         Title = Resources.Translations.SCREEN_SETTINGS_MAIN_TITLE;
         LibraryTypes = GetLibraryTypes();
         ClearCacheCommand = new Command(async () => await ClearCache());
      }

      public override Task OnAppearing() =>
         LoadCacheSize();

   }
}
