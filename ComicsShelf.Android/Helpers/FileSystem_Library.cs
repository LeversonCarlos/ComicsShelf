using System.Threading.Tasks;

namespace ComicsShelf.Droid
{
   partial class FileSystem
   {

      public async Task<bool> ValidateLibraryPath(Helpers.Database.Library library)
      {
         library.Available = false;
         if (string.IsNullOrEmpty(library.LibraryPath)) { return false; }
         if (!await Task.Run(() => System.IO.Directory.Exists(library.LibraryPath))) { return false; }

         await Task.Run(() => { library.LibraryText = System.IO.Path.GetFileNameWithoutExtension(library.LibraryPath); });

         var libraryFiles = await this.GetFiles(library.LibraryPath);
         library.FileCount = libraryFiles.Length;
         library.Available = (library.FileCount != 0);

         return library.Available;
      }

      public async Task<string> GetLibraryPath()
      { return await FolderDialog.GetDirectoryAsync(string.Empty); }

   }
}