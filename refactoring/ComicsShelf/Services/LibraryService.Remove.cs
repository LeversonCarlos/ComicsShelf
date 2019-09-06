using System;
using System.Threading.Tasks;

namespace ComicsShelf.Services
{
   partial class LibraryService
   {

      internal async Task<bool> RemoveLibrary()
      {
         try
         {
            this.Store.LibraryFiles[ComicsShelf.Store.enLibraryFilesGroup.Libraries]
               .RemoveAll(x => x.ComicFile.LibraryKey == this.Library.ID);
            if (!await this.Statistics()) { return false; }
            if (!await this.SaveSyncData()) { return false; }
            if (!await this.SaveData()) { return false; }
            return true;
         }
         catch (Exception ex) { await App.ShowMessage(ex); return false; }
      }

   }
}
