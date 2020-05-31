using ComicsShelf.Helpers.FolderDialog;
using ComicsShelf.Store;
using System;
using System.Threading.Tasks;

namespace ComicsShelf.Engines.OneDrive
{
   partial class OneDriveEngine
   {

      public async Task<LibraryModel> NewLibrary()
      {
         try
         {

            if (!await this.Connector.TryConnectAsync()) { return null; }

            var selectedFolder = await Selector.GetFolder(this.Connector.Service, new Helpers.Folder());
            if (selectedFolder == null) { return null; }

            var library = new LibraryModel
            {
               Key = selectedFolder.Key,
               Path = selectedFolder.FullPath,
               Description = selectedFolder.Name,
               Type = LibraryType.OneDrive
            };

            if (!string.IsNullOrEmpty(library.Path)) { library.Path += "/"; }

            return library;
         }
         catch (Exception ex) { await App.ShowMessage(ex); return null; }
      }

   }
}
