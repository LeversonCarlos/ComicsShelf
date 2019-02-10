using System;
using System.Collections.Generic;
using System.IO;
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

            using (var libraryDataStream = await libraryDataFile.OpenStreamForWriteAsync())
            {
               await libraryDataStream.WriteAsync(serializedData, 0, serializedData.Length);
            }

            return true;

         }
         catch (Exception) { throw; }
      }

      public async Task<byte[]> LoadDataAsync(Libraries.Library library)
      {
         try
         {
            var libraryFolder = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(library.LibraryID);
            if (libraryFolder == null) { return null; }

            var libraryDataFile = await libraryFolder.GetFileAsync("ComicsShelf.config");
            if (libraryDataFile == null) { return null; }
           

            var serializedData = new List<byte> ();
            using (var libraryDataStream = await libraryDataFile.OpenStreamForReadAsync())
            {

               int readedByte = 0;
               while ((readedByte = libraryDataStream.ReadByte()) != -1)
               {
                  serializedData.Add((byte)readedByte);
               }


            }
            return serializedData.ToArray();

         }
         catch (Exception ex) { throw ex; }
      }

   }
}