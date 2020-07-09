using ComicsShelf.Store;
using ComicsShelf.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Screens.Settings
{
   public partial class SettingsVM : BaseVM
   {

      public List<SettingsLibraryTypeVM> LibraryTypes { get; }

      public SettingsVM()
      {
         Title = Resources.Translations.SETTINGS_MAIN_TITLE;
         LibraryTypes = GetLibraryTypes();
         ClearCacheCommand = new Command(async () => await ClearCache());
      }

      List<SettingsLibraryTypeVM> GetLibraryTypes()
      {
         var typesList = new List<SettingsLibraryTypeVM>();
         try
         {

            var libraryList = DependencyService.Get<IStoreService>()?.GetLibraries();
            var libraryTypes = new enLibraryType[] { enLibraryType.LocalDrive, enLibraryType.OneDrive };

            foreach (var libraryType in libraryTypes)
            {
               var libraryTypeItems = libraryList.Where(x => x.Type == libraryType).Select(lib => new SettingsLibraryVM(lib)).ToArray();
               typesList.Add(new SettingsLibraryTypeVM(libraryType, libraryTypeItems));
            }

         }
         catch (Exception ex) { Helpers.Insights.TrackException(ex); }
         return typesList;
      }

      public override Task OnAppearing() =>
         LoadCacheSize();

   }
}
