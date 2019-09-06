using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Services
{
   partial class LibraryService
   {

      internal static async void UpdateLibrary(ComicFiles.ComicFileVM comicFile)
      {
         var library = DependencyService.Get<Store.ILibraryStore>().GetLibrary(comicFile.ComicFile.LibraryKey);
         using (var service = new LibraryService(library))
         {
            service.ReplaceLibraryFile(comicFile.ComicFile);
            await service.UpdateLibrary();
         }
      }

      internal async Task<bool> UpdateLibrary()
      {
         try
         {
            if (!await this.Statistics()) { return false; }
            if (!await this.SaveSyncData()) { return false; }
            if (!await this.SaveData()) { return false; }
            this.Notify.Send(this.Library, false);
            return true;
         }
         catch (Exception ex) { await App.ShowMessage(ex); return false; }
      }

   }
}
