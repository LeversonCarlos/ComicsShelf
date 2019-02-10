using System;
using System.Threading.Tasks;

namespace ComicsShelf.Droid
{
   partial class FileSystem
   {

      public async Task<bool> SaveDataAsync(Libraries.Library library, byte[] serializedData)
      {
         try
         {
            var libraryDataPath = $"{library.LibraryID}{this.PathSeparator}ComicsShelf.config";
            if (System.IO.File.Exists(libraryDataPath)) {
               System.IO.File.Delete(libraryDataPath);
            }
            await Task.Run(() => System.IO.File.WriteAllBytes(libraryDataPath, serializedData));
            return System.IO.File.Exists(libraryDataPath);
         }
         catch (Exception) { throw; }
      }

      public async Task<byte[]> LoadDataAsync(Libraries.Library library)
      {
         try
         {
            var libraryDataPath = $"{library.LibraryID}{this.PathSeparator}ComicsShelf.config";
            if (!System.IO.File.Exists(libraryDataPath)) { return null; }

            byte[] serializedData = null;
            await Task.Run(() => serializedData = System.IO.File.ReadAllBytes(libraryDataPath));
            return serializedData;
         }
         catch (Exception) { throw; }
      }

   }
}