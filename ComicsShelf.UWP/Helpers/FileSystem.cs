using Xamarin.Forms;
using System.Threading.Tasks;
using System;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;

[assembly: Dependency(typeof(ComicsShelf.UWP.FileSystem))]
namespace ComicsShelf.UWP
{
   public class FileSystem : ComicsShelf.Helpers.iFileSystem
   {

      #region PathSeparator
      public string PathSeparator
      { get { return System.IO.Path.DirectorySeparatorChar.ToString(); } }
      #endregion

      #region GetComicsPath
      public async Task<string> GetComicsPath(string comicsPath)
      {
         try
         {
            if (await this.PathExists(comicsPath)) { return comicsPath; }

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
      #endregion

      #region PathExists
      private async Task<bool> PathExists(string path)
      {
         if (string.IsNullOrEmpty(path)) { return false; }
         Windows.Storage.StorageFolder folder = null;
         try
         { folder = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(path); }
         catch { StorageApplicationPermissions.FutureAccessList.Remove(path); }
         return (folder != null);
      }
      #endregion

   }
}