using ComicsShelf.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Settings
{
   public partial class SettingsVM : BaseVM
   {

      public List<LibrariesSetting> LibraryTypes { get; }

      public SettingsVM()
      {
         Title = Strings.TITLE;
         LibraryTypes = new List<LibrariesSetting> {
            new LibrariesSetting(ViewModels.enLibraryType.LocalDrive),
            new LibrariesSetting(ViewModels.enLibraryType.OneDrive)
         };
         ClearCacheCommand = new Command(async () => await ClearCache());
      }

      public override Task OnAppearing()
      {
         LoadCacheSize();
         return base.OnAppearing();
      }

   }
}
