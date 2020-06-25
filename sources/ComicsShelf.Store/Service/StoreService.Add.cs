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
            if (this.ItemList.ContainsKey(library.ID)) { return false; }

            if (!await Sync.SetLibrary(library)) { return false; }
            this.ItemList.Add(library.ID, new SortedList<string, ItemVM>());

            this.LibraryList = this.LibraryList
               .Union(new[] { library })
               .ToArray();
            if (!await Sync.SetLibraries(this.LibraryList)) { return false; }

            Notifyers.Notify.LibraryAdd(library);

            return true;
         }
         catch (Exception ex) { Helpers.App.ShowMessage(ex); return false; }
      }

   }
}
