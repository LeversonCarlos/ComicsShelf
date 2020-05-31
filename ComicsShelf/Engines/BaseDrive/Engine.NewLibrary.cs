using ComicsShelf.Helpers.FolderDialog;
using ComicsShelf.Store;
using System;
using System.Threading.Tasks;

namespace ComicsShelf.Engines.BaseDrive
{
   partial class BaseDriveEngine<T>
   {

      public async Task<LibraryModel> NewLibrary()
      {
         try
         {

            if (!await this.NewLibrary_TryConnection()) { return null; }

            var selectedFolder = await Selector.GetFolder(this.Service, new Helpers.Folder());
            if (selectedFolder == null) { return null; }

            var library = new LibraryModel
            {
               Key = selectedFolder.Key,
               Path = selectedFolder.FullPath,
               Description = selectedFolder.Name,
               Type = this.LibraryType
            };

            if (this.LibraryType == LibraryType.OneDrive)
            { if (!string.IsNullOrEmpty(library.Path)) { library.Path += "/"; } }

            return library;
         }
         catch (Exception ex) { await App.ShowMessage(ex); return null; }
      }

      private async Task<bool> NewLibrary_TryConnection()
      {
         try
         { return await this.Service.ConnectAsync(); }
         catch { return false; }
      }

   }
}
