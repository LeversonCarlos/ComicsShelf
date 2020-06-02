using ComicsShelf.ComicFiles;
using ComicsShelf.Store;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.CloudDrive.Connector.Common;

namespace ComicsShelf.Engines.BaseDrive
{
   partial class BaseDriveEngine<T>
   {

      public async Task<ComicFile[]> SearchFiles(LibraryModel library)
      {
         try
         {

            // DEFINE ROOT DIRECTORY
            var rootDirectory = new DirectoryVM
            {
               ID = library.Key,
               Path = library.Path,
               Name = library.Description
            };

            // LOCATE ALL CHILDREN FILES 
            var fileList = await this.Service.SearchFiles(rootDirectory, "*.cbz", 10000);

            // CONVERT
            var comicFiles = fileList
               .Select(file => new ComicFile
               {
                  LibraryKey = library.ID,
                  Key = file.ID,
                  FilePath = $"{file.Path.Replace("/ /", "/")}/{file.Name}",
                  FolderPath = file.Path.Replace("/ /", "/"),
                  ReleaseDate = (!file.CreatedDateTime.HasValue ? DateTime.MinValue : file.CreatedDateTime.Value.ToLocalTime()),
                  KeyValues = file.KeyValues,
                  CachePath = $"{Helpers.Constants.FilesCachePath}{this.EscapeFileID(file.ID)}"
               })
               .ToList();

            // TEXT
            foreach (var comicFile in comicFiles)
            {
               comicFile.FullText = System.IO.Path.GetFileNameWithoutExtension(comicFile.FilePath).Trim();
               if (!string.IsNullOrEmpty(rootDirectory.Path) && !string.IsNullOrEmpty(comicFile.FolderPath))
               { comicFile.FolderPath = comicFile.FolderPath.Replace(rootDirectory.Path.Replace("/ / ", "/"), "").Trim(); }
               var folderText = System.IO.Path.GetFileNameWithoutExtension(comicFile.FolderPath);
               if (!string.IsNullOrEmpty(folderText))
               { comicFile.SmallText = comicFile.FullText.Replace(folderText, ""); }
               if (string.IsNullOrEmpty(comicFile.SmallText))
               { comicFile.SmallText = comicFile.FullText; }
            }

            return comicFiles.ToArray();
         }
         catch (Exception ex) { await App.ShowMessage(ex); return null; }
      }

   }
}
