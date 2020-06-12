using ComicsShelf.Store;
using System;
using System.Threading.Tasks;

namespace ComicsShelf.Drive
{
   partial class LocalDrive
   {

      public override async Task<bool> ValidateLibrary(LibraryStoreVM library)
      {
         try
         {
            if (!await this.CloudService.CheckConnectionAsync()) { return false; }
            if (string.IsNullOrEmpty(library.ID)) { return false; }
            if (!await Task.FromResult(System.IO.Directory.Exists(library.ID))) { return false; }
            return true;
         }
         catch (Exception ex) { ComicsShelf.Base.Helpers.AppInsights.TrackException(ex); return false; }
      }

   }
}
