using ComicsShelf.ViewModels;
using System;
using System.Threading.Tasks;

namespace ComicsShelf.Drive
{
   partial class BaseDrive<T>
   {

      public async Task<LibraryVM> AddLibrary()
      {
         try
         {

            if (!await this.CloudService.ConnectAsync()) { return null; }

            var driveItem = await FolderDialog.Selector.GetFolder(this.CloudService);
            if (driveItem == null) { return null; }

            var library = new LibraryVM
            {
               ID = driveItem.ID,
               Description = driveItem.Name,
               Path = driveItem.Path,
               KeyValues = driveItem.KeyValues
            };

            /*
            if (this.LibraryType == LibraryType.OneDrive)
            { if (!string.IsNullOrEmpty(library.Path)) { library.Path += "/"; } }
            */

            return library;
         }
         catch (Exception ex) { Helpers.AppInsights.TrackException(ex); return null; }
      }

   }
}
