using ComicsShelf.ComicFiles;
using ComicsShelf.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.OneDrive.Files;

namespace ComicsShelf.Engines.OneDrive
{
   partial class OneDriveEngine
   {

      public async Task<ComicFile[]> SearchFiles(LibraryModel library)
      {
         try
         {

            // DEFINE ROOT FOLDER
            var rootFolder = new FileData
            {
               id = library.Key,
               FilePath = library.Path,
               FileName = library.Description
            };

            // LOCATE ALL CHILDREN FILES 
            // var fileList = await this.Connector.SearchFilesAsync(rootFolder, "*.cbz", 10000);
            var fileList = await this.SearchFiles(rootFolder);

            // CONVERT
            var comicFiles = fileList
               .Select(file => new ComicFile
               {
                  LibraryKey = library.ID,
                  Key = file.id,
                  FilePath = $"{file.FilePath.Replace("/ /", "/")}/{file.FileName}",
                  FolderPath = file.FilePath.Replace("/ /", "/"),
                  ReleaseDate = (!file.CreatedDateTime.HasValue ? DateTime.MinValue : file.CreatedDateTime.Value.ToLocalTime()),
                  KeyValues = new Dictionary<string, string> { { "StreamSize", $"{(file.Size.HasValue ? file.Size.Value : 0)}" } },
                  CachePath = $"{Helpers.Constants.FilesCachePath}{file.id}"
               })
               .ToList();

            // TEXT
            foreach (var comicFile in comicFiles)
            {
               comicFile.FullText = System.IO.Path.GetFileNameWithoutExtension(comicFile.FilePath).Trim();
               if (!string.IsNullOrEmpty(rootFolder.FilePath) && !string.IsNullOrEmpty(comicFile.FolderPath))
               { comicFile.FolderPath = comicFile.FolderPath.Replace(rootFolder.FilePath.Replace("/ / ", "/"), "").Trim(); }
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

      private async Task<List<FileData>> SearchFiles(FileData folder)
      {
         try
         {
            var files = await this.Connector.GetChildFilesAsync(folder);
            files.RemoveAll(x => !x.FileName.ToLower().EndsWith("cbz"));

            var childFolders = await this.Connector.GetChildFoldersAsync(folder);
            if (childFolders != null && childFolders.Count > 0)
            {
               var childTasks = new List<Task<List<FileData>>>();
               foreach (var childFolder in childFolders)
               {
                  childTasks.Add(SearchFiles(childFolder));
               }
               var childs = await Task.WhenAll(childTasks.ToArray());
               files.AddRange(childs.SelectMany(x => x).ToArray());
            }

            return files;
         }
         catch (Exception ex) { Helpers.AppCenter.TrackEvent(ex); throw; }
      }

   }
}
