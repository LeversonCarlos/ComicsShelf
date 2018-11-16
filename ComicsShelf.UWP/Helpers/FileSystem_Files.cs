using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

namespace ComicsShelf.UWP
{
   partial class FileSystem
   {

      public async Task<string[]> GetFiles(string path)
      {
         try
         {


            /* LOCATE FILES */
            Windows.Storage.StorageFolder folder = null;
            if (path.Contains(this.PathSeparator))
            { folder = await Windows.Storage.StorageFolder.GetFolderFromPathAsync(path); }
            else { folder = await Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.GetFolderAsync(path); }

            return await this.GetFiles(folder);
         }
         catch (Exception ex) { throw; }
      }

      private async Task<string[]> GetFiles(Windows.Storage.StorageFolder folder)
      {
         try
         {

            /* STORAGE QUERY */
            var fileTypeFilter = new System.Collections.Generic.List<string>();
            fileTypeFilter.Add(".cbz");
            /* fileTypeFilter.Add(".cbr"); */
            var queryOptions = new Windows.Storage.Search.QueryOptions(Windows.Storage.Search.CommonFileQuery.OrderByName, fileTypeFilter);

            /* EXECUTE QUERY */
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

      private async Task<Windows.Storage.StorageFile> GetStorageFile(ComicsShelf.Helpers.Settings.Settings settings, string libraryPath, string fullPath)
      {
         try
         {

            // INITIALIZE
            var splitedPath = fullPath
               .Split(new string[] { settings.Paths.Separator }, StringSplitOptions.RemoveEmptyEntries);

            Windows.Storage.StorageFolder storageFolder = null;
            if (libraryPath.Contains(this.PathSeparator))
            { storageFolder = await Windows.Storage.StorageFolder.GetFolderFromPathAsync(libraryPath); }
            else { storageFolder = await Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.GetFolderAsync(libraryPath); }
            StorageFile storageFile = null;

            // LOOP THROUGH SPLITED PATH PARTS
            for (int splitIndex = 0; splitIndex < splitedPath.Length; splitIndex++)
            {
               var folderPath = splitedPath[splitIndex];
               if (!folderPath.EndsWith(".cbz") && !folderPath.EndsWith(".cbr"))
               { storageFolder = await storageFolder.GetFolderAsync(folderPath); }
               else
               { storageFile = await storageFolder.GetFileAsync(folderPath); }
            }

            // RESULT
            return storageFile;
         }
         catch (Exception ex) { throw; }
      }

   }
}