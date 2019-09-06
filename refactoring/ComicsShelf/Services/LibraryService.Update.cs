using System;
using System.Threading.Tasks;

namespace ComicsShelf.Services
{
   partial class LibraryService
   {

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
