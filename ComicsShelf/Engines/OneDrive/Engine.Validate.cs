using ComicsShelf.Libraries;
using System;
using System.Threading.Tasks;

namespace ComicsShelf.Engines.OneDrive
{
   partial class OneDriveEngine
   {

      public async Task<bool> Validate(LibraryModel library)
      {
         try
         {
            if (!await this.Connector.ConnectAsync()) { return false; }
            if (!await this.HasStoragePermission()) { return false; }
            return true;
         }
         catch (Exception ex) { Helpers.AppCenter.TrackEvent("OneDrive.Validate", ex); return false; }
      }

      public async Task<bool> HasStoragePermission()
      { return await Helpers.Permissions.HasStoragePermission(); }

   }
}
