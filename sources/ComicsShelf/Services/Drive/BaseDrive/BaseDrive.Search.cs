using ComicsShelf.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.CloudDrive.Connector;

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
            if (fileList == null) return new ItemVM[] { };

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

            return itemList.ToArray();
         }
         catch (Exception ex) { Helpers.Message.Show(ex); return new ItemVM[] { }; }
      }

   }
}
