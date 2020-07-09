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

            var libraryTypes = DependencyService.Get<IStoreService>()
               ?.GetLibraries()
               ?.GroupBy(lib => lib.Type)
               ?.Select(grp => new
               {
                  LibraryType = grp.Key,
                  LibraryList = grp
                     .ToArray()
                     .Select(lib => new SettingsLibraryVM(lib))
                     .ToArray()
               })
               ?.ToArray();

            foreach (var libraryType in libraryTypes)
            {
               var typeVM = new SettingsLibraryTypeVM(libraryType.LibraryType);
               typeVM.Libraries.AddRange(libraryType.LibraryList);
               typesList.Add(typeVM);
            }

         }
         catch (Exception ex) { Helpers.Insights.TrackException(ex); }
         return typesList;
      }

      public override Task OnAppearing()
      {
         LoadCacheSize();
         return base.OnAppearing();
      }

      SettingsLibraryTypeVM GetLibraryType(LibraryVM library) =>
         LibraryTypes
            ?.Where(type => type.LibraryType == library.Type)
            ?.FirstOrDefault();

      SettingsLibraryVM GetLibrary(LibraryVM library) =>
         GetLibraryType(library)
            ?.Libraries
            ?.Where(lib => lib.Library?.ID == library.ID)
            ?.FirstOrDefault();

      public void SetLibraryMessage(LibraryVM library, string message) =>
         GetLibrary(library).Message = message;

      public void SetLibraryProgress(LibraryVM library, double progress) =>
         GetLibrary(library).Progress = progress;

      public void AddLibrary(LibraryVM library) =>
         GetLibraryType(library).Libraries.Add(new SettingsLibraryVM(library));

      public void RemoveLibrary(LibraryVM library)
      {
         var libraryVM = GetLibrary(library);
         if (libraryVM == null) return;
         GetLibraryType(library).Libraries.Remove(libraryVM);
      }

   }
}
