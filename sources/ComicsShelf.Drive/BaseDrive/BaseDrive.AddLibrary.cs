using System;
using System.Threading.Tasks;

namespace ComicsShelf.Drive
{
   partial class BaseDrive<T>
   {

      public async Task<ItemVM> AddLibrary()
      {
         try
         {

            if (!await this.CloudService.ConnectAsync()) { return null; }

            var library = await FolderDialog.Selector.GetFolder(this.CloudService);
            if (library == null) { return null; }

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
