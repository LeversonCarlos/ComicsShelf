using System;
using System.Threading.Tasks;

namespace ComicsShelf.Droid
{
   partial class FileSystem
   {

      public async Task<byte[]> LoadData(Libraries.LibraryModel library)
      {
         try
         {
            var libraryDataPath = $"{library.LibraryKey}{this.PathSeparator}{Libraries.LibraryModel.SyncFile}";
            if (!System.IO.File.Exists(libraryDataPath)) { return null; }

            byte[] serializedData = null;
            await Task.Run(() => serializedData = System.IO.File.ReadAllBytes(libraryDataPath));
            return serializedData;
         }
         catch (Exception) { throw; }
      }

      public async Task<bool> SaveData(Libraries.LibraryModel library, byte[] serializedData)
      {
         try
         {
            var libraryDataPath = $"{library.LibraryKey}{this.PathSeparator}{Libraries.LibraryModel.SyncFile}";
            if (System.IO.File.Exists(libraryDataPath))
            {
               System.IO.File.Delete(libraryDataPath);
            }
            await Task.Run(() => System.IO.File.WriteAllBytes(libraryDataPath, serializedData));
            return System.IO.File.Exists(libraryDataPath);
         }
         catch (Exception) { throw; }
      }

   }
}