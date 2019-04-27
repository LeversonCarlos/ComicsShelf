using ComicsShelf.Helpers.FolderDialog;
using ComicsShelf.Libraries;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ComicsShelf.Engines.LocalDrive
{
   partial class LocalDriveEngine
   {

      public async Task<LibraryModel> NewLibrary()
      {
         try
         {
            var initialFolder = await this.FileSystem.GetRootPath();
            var selectedFolder = await Selector.GetFolder(initialFolder, async (folder) => {
               return await this.FileSystem.GetFolderChilds(folder);
            });

            var library = new LibraryModel
            {
               Key = selectedFolder.Path,
               Description = selectedFolder.Name,
               Type = LibraryType.LocalDrive
            };
            return library;

         }
         catch (Exception ex) { await App.ShowMessage(ex); return null; }
      }

   }
}
