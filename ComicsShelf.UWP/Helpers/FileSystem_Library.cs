using System;
using System.Threading.Tasks;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;

namespace ComicsShelf.UWP
{
   partial class FileSystem
   {

      public async Task<bool> ValidateLibraryPath(Helpers.Database.Library library)
      {
         if (string.IsNullOrEmpty(library.LibraryPath)) { return false; }
         Windows.Storage.StorageFolder folder = null;
         try
         {
            library.Available = false;
            folder = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(library.LibraryPath);

            library.LibraryText = folder.DisplayName;

            var libraryFiles = await this.GetFiles(folder);
            library.FileCount = libraryFiles.Length;
            library.Available = (library.FileCount != 0);
         }
         catch { try { StorageApplicationPermissions.FutureAccessList.Remove(library.LibraryPath); } catch { } }
         return library.Available;
      }

      public async Task<string> GetLibraryPath()
      {
         try
         {

            var folderPicker = new FolderPicker();
            folderPicker.SuggestedStartLocation = PickerLocationId.Desktop;
            folderPicker.FileTypeFilter.Add("*");

            var folder = await folderPicker.PickSingleFolderAsync();
            if (folder == null) { return string.Empty; }

            var folderToken = StorageApplicationPermissions.FutureAccessList.Add(folder);
            return folderToken;

         }
         catch (Exception ex) { throw; }
      }

   }
}