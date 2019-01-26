using System.Threading.Tasks;

namespace ComicsShelf.Droid
{
   partial class FileSystem
   {

      public async Task<bool> ValidateLibraryPath(Library.Library library)
      {
         library.Available = false;
         if (string.IsNullOrEmpty(library.LibraryID)) { return false; }
         if (!await Task.Run(() => System.IO.Directory.Exists(library.LibraryID))) { return false; }

         await Task.Run(() => { library.Description = System.IO.Path.GetFileNameWithoutExtension(library.LibraryID); });

         var libraryFiles = await this.GetFiles(library.LibraryID);
         library.Available = (libraryFiles.Length != 0);

         return library.Available;
      }

      public async Task<string> GetLibraryPath()
      { return await FolderDialog.GetDirectoryAsync(string.Empty); }

   }
}