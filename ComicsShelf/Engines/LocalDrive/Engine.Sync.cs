using System;
using System.Threading.Tasks;

namespace ComicsShelf.Engines.LocalDrive
{
   partial class LocalDriveEngine
   {

      public async Task<byte[]> LoadData(Libraries.LibraryModel library)
      {
         try
         {
            if (!await Helpers.Permissions.HasStoragePermission()) { return null; }
            var serializedData = await this.FileSystem.LoadData(library);
            return serializedData;
         }
         catch (Exception ex) { await App.ShowMessage(ex); return null; }
      }

      public async Task<bool> SaveData(Libraries.LibraryModel library, byte[] serializedValue)
      {
         try
         {
            if (!await Helpers.Permissions.HasStoragePermission()) { return false; }
            if (!await this.FileSystem.SaveData(library, serializedValue)) { return false; }
            return true;
         }
         catch (Exception ex) { await App.ShowMessage(ex); return false; }
      }

   }
}