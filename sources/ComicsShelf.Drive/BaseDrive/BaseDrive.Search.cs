using ComicsShelf.Store;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.CloudDrive.Connector.Common;

namespace ComicsShelf.Drive
{
   partial class BaseDrive<T>
   {

      public async Task<ItemStoreVM[]> SearchFiles(LibraryStoreVM library)
      {
         try
         {

            // DEFINE ROOT DIRECTORY
            var directory = new DirectoryVM
            {
               ID = library.ID,
               Path = library.Path,
               Name = library.Description
            };

            // SEARCH FOR FILES ON THE DIRECTORY
            var fileList = await this.CloudService.SearchFiles(directory, "*.cbz", 10000);

            // CONVERT
            var rootPath = directory.Path.Replace("/ /", "/");
            var itemList = fileList?
               .Select(file => new
               {
                  file,
                  folderText = file.Path.Replace("/ /", "/")
               })
               .Select(x => new
               {
                  x.file.ID,
                  fileName = x.file.Name,
                  fileText = System.IO.Path.GetFileNameWithoutExtension(x.file.Name).Trim(),
                  folderPath = x.file.Path,
                  folderText = System.IO.Path.GetFileNameWithoutExtension((string.IsNullOrEmpty(x.folderText) || string.IsNullOrEmpty(rootPath) ? x.folderText : x.folderText.Replace(rootPath, ""))),
                  ReleaseDate = (!x.file.CreatedDateTime.HasValue ? DateTime.MinValue : x.file.CreatedDateTime.Value.ToLocalTime()),
                  x.file.KeyValues
               })
               .Select(file => new ItemStoreVM
               {
                  ID = file.ID,
                  LibraryID = library.ID,
                  FullText = file.fileText,
                  ShortText = (string.IsNullOrEmpty(file.fileText) || string.IsNullOrEmpty(file.folderText) ? file.fileText : file.fileText.Replace(file.folderText, "")),
                  FolderText = file.folderText,
                  FileName = file.fileName,
                  FolderPath = file.folderPath,
                  ReleaseDate = file.ReleaseDate,
                  KeyValues = file.KeyValues,
                  EscapedID = this.EscapeFileID(file.ID),
                  Available = true
               })
               .ToList();

            await Task.CompletedTask;
            return itemList.ToArray();
         }
         catch (Exception ex) { Helpers.AppInsights.TrackException(ex); return new ItemStoreVM[] { }; }
      }

   }
}
