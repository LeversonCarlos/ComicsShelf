using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ComicsShelf.Services
{
   partial class LibraryService 
   {

      private async Task<bool> NotifyData()
      {
         try
         {
            Messaging.Send<List<ComicFiles.ComicFileVM>>("OnRefreshingList", this.Library.ID, this.GetLibraryFiles());
            return true;
         }
         catch (Exception ex) { await App.ShowMessage(ex); return false; }
      }

      private async Task<bool> NotifyData(string prefix, List<ComicFiles.ComicFileVM> comicFiles)
      {
         try
         {
            Messaging.Send<List<ComicFiles.ComicFileVM>>(prefix, comicFiles);
            return true;
         }
         catch (Exception ex) { await App.ShowMessage(ex); return false; }
      }

   }
}
