using System.Threading.Tasks;

namespace ComicsShelf.Droid
{
   partial class FileSystem
   {

      public async Task<bool> ValidateLibraryPath(Helpers.Database.Library library)
      {
         if (string.IsNullOrEmpty(library.LibraryPath)) { return false; }
         if (!await Task.Run(() => System.IO.Directory.Exists(library.LibraryPath))) { return false; }
         return true;
      }

      public async Task<string> GetLibraryPath()
      { return await FolderDialog.GetDirectoryAsync(string.Empty); }

   }
}