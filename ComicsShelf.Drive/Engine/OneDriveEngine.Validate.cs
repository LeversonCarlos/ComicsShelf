using ComicsShelf.Store;
using System;
using System.Threading.Tasks;

namespace ComicsShelf.Drive
{
   partial class OneDriveEngine
   {

      public override async Task<bool> ValidateLibrary(LibraryStoreVM library)
      {
         try
         {
            if (!await this.DriveService.CheckConnectionAsync()) { return false; }
            return true;
         }
         catch (Exception ex) { ComicsShelf.Base.Helpers.AppInsights.TrackException(ex); return false; }
      }

   }
}
