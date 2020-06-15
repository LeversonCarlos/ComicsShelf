using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.CloudDrive.Connector.Common;

namespace ComicsShelf.Drive
{
   partial class BaseDrive<T>
   {

      public async Task<ItemVM[]> SearchFiles(ItemVM library)
      {
         try
         {

            // DEFINE ROOT DIRECTORY
            var root = new DirectoryVM
            {
               ID = library.ID,
               Path = library.Path,
               Name = library.Name
            };

            // SEARCH FOR FILES ON THE DIRECTORY
            var fileList = await this.CloudService.SearchFiles(root, "*.cbz", 10000);

            // CONVERT
            var rootPath = root.Path.Replace("/ /", "/");
            var itemList = fileList?
               .Select(x => new ItemVM
               {
                  ID = x.ID,
                  Name = x.Name,
                  Path = x.Path,
                  ParentID = x.ParentID,
                  CreatedDateTime = x.CreatedDateTime,
                  SizeInBytes = x.SizeInBytes,
                  KeyValues = x.KeyValues
               })
               .ToList();
            foreach (var item in itemList)
            {
               if (item.KeyValues == null) { item.KeyValues = new Dictionary<string, string>(); }
               item.KeyValues.Add("EscapedID", this.EscapeFileID(item.ID));
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
         catch (Exception ex) { Helpers.AppInsights.TrackException(ex); return new ItemVM[] { }; }
      }

   }
}
