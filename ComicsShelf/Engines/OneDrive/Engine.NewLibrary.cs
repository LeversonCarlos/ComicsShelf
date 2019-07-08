using ComicsShelf.Helpers.FolderDialog;
using ComicsShelf.Libraries;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.OneDrive.Files;
using Xamarin.OneDrive.Profile;

namespace ComicsShelf.Engines.OneDrive
{
   partial class OneDriveEngine
   {

      public async Task<LibraryModel> NewLibrary()
      {
         try
         {

            if (!await this.Connector.ConnectAsync()) { return null; }

            var profile = await this.Connector.GetProfileAsync();
            if (profile == null) { return null; }

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
               LibraryKey = selectedFolder.Key,
               LibraryPath = selectedFolder.FullPath,
               Description = selectedFolder.Name,
               Type = LibraryType.OneDrive
            };

            library.LibraryPath = library.LibraryPath;
            if (!string.IsNullOrEmpty(library.LibraryPath))
            { library.LibraryPath += "/"; }

            return library;

         }
         catch (Exception ex) { await App.ShowMessage(ex); return null; }
      }

      public async Task<bool> DeleteLibrary(LibraryModel library)
      {
         try
         {

            var service = Xamarin.Forms.DependencyService.Get<LibraryService>();
            if (service != null)
            {
               var hasMoreOneDriveLibraries = service.Libraries
                  .Select(x => x.Value)
                  .Where(x => x.Type == LibraryType.OneDrive)
                  .Where(x => x.LibraryKey != library.LibraryKey)
                  .Any();
               if (!hasMoreOneDriveLibraries)
               {
                  await this.Connector.DisconnectAsync();
               }
            }

            return true;
         }
         catch (Exception ex) { await App.ShowMessage(ex); return false; }
      }

   }
}
