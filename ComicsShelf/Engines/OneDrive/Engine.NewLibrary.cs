using ComicsShelf.Helpers.FolderDialog;
using ComicsShelf.Store;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.OneDrive.Files;

namespace ComicsShelf.Engines.OneDrive
{
   partial class OneDriveEngine
   {

      public async Task<LibraryModel> NewLibrary()
      {
         try
         {

            if (!await this.Connector.TryConnectAsync()) { return null; }

            var root = await this.Connector.GetDetailsAsync();
            var initialFolder = new Helpers.Folder { Key = root.id, FullPath = "/", Name = "/" };

            var selectedFolder = await Selector.GetFolder(initialFolder, async (folder) =>
            {
               var folderData = new FileData { id = folder.Key, FilePath = folder.FullPath, FileName = folder.Name };
               var folderChilds = await this.Connector.GetChildFoldersAsync(folderData);
               var folderList = folderChilds
                  .Select(x => new Helpers.Folder
                  {
                     Key = x.id,
                     FullPath = x.FilePath
                        .Trim()
                        .Replace("/ / ", "/")
                        .Replace("/ /", "/")
                        .Replace("// ", "/")
                        .Replace("//", "/"),
                     Name = x.FileName
                  })
                  .ToArray();
               return folderList;
            });
            if (selectedFolder == null) { return null; }

            var library = new LibraryModel
            {
               Key = selectedFolder.Key,
               Path = selectedFolder.FullPath,
               Description = selectedFolder.Name,
               Type = LibraryType.OneDrive
            };

            // library.Path = library.Path;
            if (!string.IsNullOrEmpty(library.Path))
            { library.Path += "/"; }

            return library;

         }
         catch (Exception ex) { await App.ShowMessage(ex); return null; }
      }     

   }
}
