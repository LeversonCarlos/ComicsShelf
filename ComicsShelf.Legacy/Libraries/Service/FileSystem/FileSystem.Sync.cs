using System;
using System.Threading.Tasks;

namespace ComicsShelf.Libraries.Implementation
{
   partial class FileSystemService
   {

      public async Task<bool> SaveDataAsync(Library library, byte[] serializedValue)
      {
         try
         {
            if (!await Helpers.Permissions.HasStoragePermission()) { return false; }
            if (!await this.FileSystem.SaveDataAsync(library, serializedValue)) { return false; }
            return true;
         }
         catch (Exception ex) { Engine.AppCenter.TrackEvent("FileSystemService.SaveDataAsync", ex); return false; }
      }

      public async Task<byte[]> LoadDataAsync(Library library)
      {
         try
         {
            if (!await Helpers.Permissions.HasStoragePermission()) { return null; }
            var serializedData = await this.FileSystem.LoadDataAsync(library);
            return serializedData;
         }
         catch (Exception ex) { Engine.AppCenter.TrackEvent("FileSystemService.LoadDataAsync", ex); return null; }
      }

   }
}