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

            // FOLDER SELECTION 
            var library = await Drive.BaseDrive
               .GetDrive(libraryType)
               .AddLibrary();
            if (library == null) { return false; }

            // VALIDATE
            if (this.ItemList.ContainsKey(library.ID))
            { Helpers.Message.Show(Resources.Translations.ENGINE_STORE_FOLDER_ALREADY_DEFINED_WARNING); return false; }

            // SAVE LIBRARY DATA
            if (!await Sync.SetLibrary(library)) { return false; }
            this.ItemList.Add(library.ID, new SortedList<string, ItemVM>());

            // REFRESH THE LIBRARY LIST
            this.LibraryList = this.LibraryList
               .Union(new[] { library })
               .ToArray();
            if (!await Sync.SetLibraries(this.LibraryList)) { return false; }

            // NOTIFY NEW LIBRARY
            Helpers.Notify.LibraryAdd(library);
            return true;

         }
         catch (Exception ex) { Helpers.Message.Show(ex); return false; }
      }

   }
}
