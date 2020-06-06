using ComicsShelf.Helpers.FolderDialog;
using ComicsShelf.Store;
using System;
using System.Collections.Generic;
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
         var logs = new List<string>();
         try
         {
            logs.Add($"LibraryType:{this.LibraryType}");
            var result = await this.Service.ConnectAsync();
            logs.Add($"Result:{result}");
            return result;
         }
         catch (Exception ex) { logs.Add($"Exception:{ex.Message}"); await App.ShowMessage(ex); return false; }
         finally { Helpers.AppCenter.TrackEvent("Try to connect with cloud service", logs.ToArray()); }
      }

   }
}
