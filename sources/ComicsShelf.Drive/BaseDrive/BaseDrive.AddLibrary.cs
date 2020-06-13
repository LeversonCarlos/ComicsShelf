using ComicsShelf.Store;
using System;
using System.Threading.Tasks;

namespace ComicsShelf.Drive
{
   partial class BaseDrive<T>
   {

      public async Task<LibraryStoreVM> AddLibrary()
      {
         try
         {

            if (!await this.CloudService.ConnectAsync()) { return null; }

            var selectedFolder = await FolderDialog.Selector.GetFolder(this.CloudService);
            if (selectedFolder == null) { return null; }

            var library = new LibraryStoreVM
            {
               ID = selectedFolder.ID,
               Path = selectedFolder.Path,
               Description = selectedFolder.Name,
               Type = this.GetLibraryType()
            };

            /*
            if (this.LibraryType == LibraryType.OneDrive)
            { if (!string.IsNullOrEmpty(library.Path)) { library.Path += "/"; } }
            */

            return library;
         }
         catch (Exception ex) { ComicsShelf.Base.Helpers.AppInsights.TrackException(ex); return null; }
      }

   }
}
