using ComicsShelf.Helpers.FolderDialog;
using ComicsShelf.Libraries;
using System;

namespace ComicsShelf.Engines.LocalDrive
{
   partial class LocalDriveEngine
   {

      public async void NewLibrary(Action<LibraryModel> resultCallback)
      {
         try
         {

            if (!await this.HasStoragePermission()) { resultCallback.Invoke(null); return; }

            var initialFolder = await this.FileSystem.GetRootPath();
            var selectedFolder = await Selector.GetFolder(initialFolder, async (folder) =>
            {
               return await this.FileSystem.GetFolders(folder);
            });
            if (selectedFolder == null) { resultCallback.Invoke(null); return; }

            var library = new LibraryModel
            {
               LibraryKey = selectedFolder.Key,
               LibraryPath = selectedFolder.FullPath,
               Description = selectedFolder.Name,
               Type = LibraryType.LocalDrive
            };
            resultCallback.Invoke(library);

         }
         catch (Exception ex) { await App.ShowMessage(ex); resultCallback.Invoke(null); }
         finally { GC.Collect(); }
      }

   }
}
