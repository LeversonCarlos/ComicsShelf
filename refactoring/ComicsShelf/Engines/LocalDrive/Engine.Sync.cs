using ComicsShelf.Store;
using System;
using System.Threading.Tasks;

namespace ComicsShelf.Engines.LocalDrive
{
   partial class LocalDriveEngine
   {

      public async Task<byte[]> LoadSyncData(LibraryModel library)
      {
         try
         {
            if (!await Helpers.Permissions.HasStoragePermission()) { return null; }

            var libraryDataPath = $"{library.Key}{this.FileSystem.PathSeparator}{LibraryModel.SyncFile}";
            if (!System.IO.File.Exists(libraryDataPath)) { return null; }
            byte[] serializedData = await Task.FromResult(System.IO.File.ReadAllBytes(libraryDataPath));

            return serializedData;
         }
         catch (Exception ex) { Helpers.AppCenter.TrackEvent(ex); return null; }
      }

      public async Task<bool> SaveSyncData(LibraryModel library, byte[] serializedData)
      {
         try
         {
            if (!await Helpers.Permissions.HasStoragePermission()) { return false; }

            var libraryDataPath = $"{library.Key}{this.FileSystem.PathSeparator}{LibraryModel.SyncFile}";
            if (System.IO.File.Exists(libraryDataPath)) { System.IO.File.Delete(libraryDataPath); }
            await Task.Run(() => System.IO.File.WriteAllBytes(libraryDataPath, serializedData));

            return System.IO.File.Exists(libraryDataPath);
         }
         catch (Exception ex) { Helpers.AppCenter.TrackEvent(ex); return false; }
      }

   }
}