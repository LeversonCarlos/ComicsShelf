using ComicsShelf.Helpers.FolderDialog;
using ComicsShelf.Libraries;
using System;
using System.Collections.Generic;
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
            var initialFolder = new Helpers.Folder { Key = root.id, Path = "/", Name = "/" };

            var selectedFolder = await Selector.GetFolder(initialFolder, async (folder) =>
            {
               var folderData = new FileData { id = folder.Key, FilePath = folder.Path, FileName = folder.Name };
               var folderChilds = await this.Connector.GetChildFoldersAsync(folderData);
               var folderList = folderChilds
                  .Select(x => new Helpers.Folder { Key = x.id, Path = x.FilePath, Name = x.FileName })
                  .ToArray();
               return folderList;
            });
            if (selectedFolder == null) { return null; }

            var library = new LibraryModel
            {
               LibraryKey = selectedFolder.Key,
               Description = selectedFolder.Name,
               Type = LibraryType.OneDrive
            };
            return library;

            /*
            var folder = await Service.OneDrive.FolderSelector.Selector.FolderSelect(this.Connector, library);
            if (folder == null) { return false; }
            folder = await this.Connector.GetDetailsAsync(folder);

            var root = await this.Connector.GetDetailsAsync();
            folder.FilePath = folder.FilePath.Replace(root.FilePath, "");
            if (!string.IsNullOrEmpty(folder.FilePath))
            { folder.FilePath += "/"; }
            folder.FilePath += folder.FileName;
            folder.FilePath += "/";

            library.LibraryID = folder.id;
            library.Description = $"{folder.FileName}";
            library.Available = true;
            library.SetKeyValue(LibraryPath, folder.FilePath);
            return true;
            */

         }
         catch (Exception ex) { await App.ShowMessage(ex); return null; }
      }

      public async Task<bool> DeleteLibrary(LibraryModel library)
      {
         try {

            /*
            var libraries = new List<LibraryModel>();
            var libraryIDs = this.GetLibraryIDs();
            foreach (var libraryID in libraryIDs)
            {
               var libraryJSON = Xamarin.Essentials.Preferences.Get(libraryID, "");
               if (!string.IsNullOrEmpty(libraryJSON))
               { libraries.Add(Newtonsoft.Json.JsonConvert.DeserializeObject<LibraryModel>(libraryJSON)); }
            }


            if (App.Settings.Libraries.Where(x => x.Type == TypeEnum.OneDrive).Count() <= 1)
            { await this.Connector.DisconnectAsync(); }

            */
            return true;
         }
         catch (Exception ex) { await App.ShowMessage(ex); return false; }
      }

   }
}
