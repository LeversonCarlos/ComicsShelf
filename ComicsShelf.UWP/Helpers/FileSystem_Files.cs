using System;
using System.Linq;
using System.Threading.Tasks;

namespace ComicsShelf.UWP
{
   partial class FileSystem
   {

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
            else { folder = await Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.GetFolderAsync(path); }
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


   }
}