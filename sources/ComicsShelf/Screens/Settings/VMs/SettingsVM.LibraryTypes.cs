using ComicsShelf.Store;
using ComicsShelf.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace ComicsShelf.Screens.Settings
{
   partial class SettingsVM
   {

      public List<SettingsLibraryTypeVM> LibraryTypes { get; }

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

   }
}
