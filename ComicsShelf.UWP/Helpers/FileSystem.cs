using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Xamarin.Forms;

[assembly: Dependency(typeof(ComicsShelf.UWP.FileSystem))]
namespace ComicsShelf.UWP
{
   public partial class FileSystem : ComicsShelf.Helpers.iFileSystem
   {

      #region PathSeparator
      public string PathSeparator
      { get { return System.IO.Path.DirectorySeparatorChar.ToString(); } }
      #endregion

      #region GetDataPath
      public async Task<string> GetDataPath()
      {
         var localPath = Windows.Storage.ApplicationData.Current.LocalFolder.Path;
         return localPath;
      }
      #endregion

      #region GetCachePath
      public async Task<string> GetCachePath()
      {
         var localPath = Windows.Storage.ApplicationData.Current.LocalCacheFolder.Path;
         return localPath;
      }
      #endregion

      #region ValidateLibraryPath
      public async Task<bool> ValidateLibraryPath(string libraryPath)
      {
         if (string.IsNullOrEmpty(libraryPath)) { return false; }
         Windows.Storage.StorageFolder folder = null;
         try
         { folder = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(libraryPath); }
         catch { try { StorageApplicationPermissions.FutureAccessList.Remove(libraryPath); } catch { } }
         return (folder != null);
      }
      #endregion

      #region GetLibraryPath
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
      #endregion


      #region GetFiles
      public async Task<string[]> GetFiles(string path)
      {
         try
         {

           

            /* STORAGE QUERY */
            var fileTypeFilter = new System.Collections.Generic.List<string>();
            fileTypeFilter.Add(".cbz");
            fileTypeFilter.Add(".cbr");
            var queryOptions = new Windows.Storage.Search.QueryOptions(Windows.Storage.Search.CommonFileQuery.OrderByName, fileTypeFilter);

            /* LOCATE FILES */
            Windows.Storage.StorageFolder folder = null;
            if (path.Contains(this.PathSeparator))
            { folder = await Windows.Storage.StorageFolder.GetFolderFromPathAsync(path); }
            else { folder = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(path); }
            var query = folder.CreateFileQueryWithOptions(queryOptions);
            var fileList = await query.GetFilesAsync();

            /* REDUCE MAIN PATH FROM FILES PATH */
            var folderPath = $"{folder.Path}{this.PathSeparator}";
            var fileListRenamed = fileList
               .Select(x => x.Path.Replace(folderPath, ""))
               .ToList();

            /* RESULT */
            var fileArray = fileListRenamed.ToArray();
            return fileArray;


         }
         catch (Exception ex) { throw; }
      }
      #endregion

   }
}