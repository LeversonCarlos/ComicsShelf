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
            if (!await this.Connector.TryConnectAsync()) { return false; }
            if (!await this.HasStoragePermission()) { return false; }
            return true;
         }
         catch (Exception ex) { await App.ShowMessage(ex); return false; }
      }

      public async Task<bool> HasStoragePermission()
      { return await Helpers.Permissions.HasStoragePermission(); }

   }
}
