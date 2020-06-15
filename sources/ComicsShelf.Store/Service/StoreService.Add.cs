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

            var driveType = libraryType == enLibraryType.OneDrive ? Drive.enDriveType.OneDrive : Drive.enDriveType.LocalDrive;
            var drive = Drive.BaseDrive.GetDrive(driveType);
            if (drive == null) { Helpers.AppInsights.TrackEvent("drive.not.defined", $"libraryType:{libraryType}"); return false; }

            var driveLibrary = await drive.AddLibrary();
            if (driveLibrary == null) { return false; }
            var library = new LibraryStoreVM
            {
               ID = driveLibrary.ID,
               Description = driveLibrary.Name,
               Path = driveLibrary.Path,
               KeyValues = driveLibrary.KeyValues,
               Type = libraryType
            };

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
