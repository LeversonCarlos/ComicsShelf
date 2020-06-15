using ComicsShelf.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComicsShelf.Store
{
   partial class StoreService
   {

      public async Task<bool> AddLibraryAsync(enLibraryType libraryType)
      {
         try
         {

            var library = await Drive.BaseDrive
               .GetDrive(libraryType)
               .AddLibrary();
            if (library == null) { return false; }

            // VALIDATE
            // TODO

            if (!await Sync.SetLibrary(library)) { return false; }
            this.ItemList.Add(library.ID, new List<ItemVM>());

            this.LibraryList = this.LibraryList
               .Union(new[] { library })
               .ToArray();
            if (!await Sync.SetLibraries(this.LibraryList)) { return false; }

            Notify.LibraryAdd(library);

            return true;
         }
         catch (Exception ex) { Helpers.AppInsights.TrackException(ex); return false; }
      }

   }
}
