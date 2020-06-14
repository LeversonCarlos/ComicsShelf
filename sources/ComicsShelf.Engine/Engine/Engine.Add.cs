using ComicsShelf.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComicsShelf.Engine
{
   partial class Engine
   {

      public async Task<bool> AddLibraryAsync(enLibraryType libraryType)
      {
         try
         {

            var drive = Drive.BaseDrive.GetDrive(libraryType);
            if (drive == null) { Helpers.AppInsights.TrackEvent("drive.not.defined", $"libraryType:{libraryType}"); return false; }

            var library = await drive.AddLibrary();
            if (library == null) { return false; }

            // VALIDATE
            // TODO

            if (!await Sync.SetLibrary(library)) { return false; }
            this.ItemList.Add(library.ID, new List<ItemStoreVM>());

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
