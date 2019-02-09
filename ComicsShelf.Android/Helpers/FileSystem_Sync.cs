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

   }
}