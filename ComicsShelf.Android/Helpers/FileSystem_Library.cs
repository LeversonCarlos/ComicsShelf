using System.Threading.Tasks;

namespace ComicsShelf.Droid
{
   partial class FileSystem
   {

      public async Task<bool> ValidateLibraryPath(string libraryPath)
      {
         if (string.IsNullOrEmpty(libraryPath)) { return false; }
         if (!await Task.Run(() => System.IO.Directory.Exists(libraryPath))) { return false; }
         return true;
      }

      public async Task<string> GetLibraryPath()
      { return await FolderDialog.GetDirectoryAsync(string.Empty); }

   }
}