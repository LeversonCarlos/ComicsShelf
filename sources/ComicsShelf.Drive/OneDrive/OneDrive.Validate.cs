using ComicsShelf.Store;
using System;
using System.Threading.Tasks;

namespace ComicsShelf.Drive
{
   partial class OneDrive
   {

      public override async Task<bool> ValidateLibrary(LibraryStoreVM library)
      {
         try
         {
            if (!await this.CloudService.CheckConnectionAsync()) { return false; }
            return true;
         }
         catch (Exception ex) { Helpers.AppInsights.TrackException(ex); return false; }
      }

   }
}
