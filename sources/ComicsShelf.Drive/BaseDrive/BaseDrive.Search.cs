using ComicsShelf.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.CloudDrive.Connector.Common;

namespace ComicsShelf.Drive
{
   partial class BaseDrive<T>
   {

      public async Task<ItemVM[]> SearchItems(LibraryVM library)
      {
         try
         {

            // DEFINE ROOT DIRECTORY
            var root = new DirectoryVM
            {
               ID = library.ID,
               Path = library.Path,
               Name = library.Description
            };

            // SEARCH FOR FILES ON THE DIRECTORY
            var fileList = await this.CloudService.SearchFiles(root, "*.cbz", 10000);

            // CONVERT
            var rootPath = root.Path.Replace("/ /", "/");
            var itemList = fileList?
               .Select(x => new ItemVM
               {
                  ID = x.ID,
                  LibraryID = library.ID,
                  EscapedID = this.EscapeFileID(x.ID),
                  FolderPath = x.Path,
                  FileName = x.Name,
                  ReleaseDate = !x.CreatedDateTime.HasValue ? DateTime.MinValue : x.CreatedDateTime.Value.ToLocalTime(),
                  SizeInBytes = x.SizeInBytes,
                  KeyValues = x.KeyValues,
                  Available = true
               })
               .ToList();
            foreach (var item in itemList)
            {

               item.SectionPath = System.IO.Path
                  .GetDirectoryName(item.FolderPath);

               item.FolderText = System.IO.Path
                  .GetFileNameWithoutExtension(item.FolderPath)
                  .Replace("/ /", "/");
               if (!string.IsNullOrEmpty(rootPath))
               { item.FolderText = item.FolderText.Replace(rootPath, ""); }

               item.FullText = System.IO.Path
                  .GetFileNameWithoutExtension(item.FileName)
                  .Trim();

               item.ShortText = item.FullText;
               if (!string.IsNullOrEmpty(item.FolderText))
               { item.ShortText = item.ShortText.Replace(item.FolderText, ""); }

            }

            /*
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
               .Select(file => new ItemVM
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
            */

            return itemList.ToArray();
         }
         catch (Exception ex) { Helpers.App.ShowMessage(ex); return new ItemVM[] { }; }
      }

   }
}
