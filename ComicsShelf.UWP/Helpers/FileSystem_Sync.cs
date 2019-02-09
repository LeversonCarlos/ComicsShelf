using System;
using System.Threading.Tasks;
using Windows.Storage.AccessCache;

namespace ComicsShelf.UWP
{
   partial class FileSystem
   {

      public async Task<bool> SaveDataAsync(Libraries.Library library, byte[] serializedData)
      {         
         try
         {
            var libraryFolder = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(library.LibraryID);
            if (libraryFolder == null) { return false; }

            var libraryDataFile = await libraryFolder.CreateFileAsync("ComicsShelf.config", Windows.Storage.CreationCollisionOption.ReplaceExisting);
            if (libraryDataFile == null) { return false; }

            using (var libraryDataStream = await libraryDataFile.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite))
            {
               using (var dataWriter = new Windows.Storage.Streams.DataWriter(libraryDataStream))
               {
                  dataWriter.WriteBytes(serializedData);
                  await dataWriter.StoreAsync();
                  await dataWriter.FlushAsync();
               }
            }

            return true;

         }
         catch (Exception) { throw; }
      }

   }
}