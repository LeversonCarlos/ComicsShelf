using ComicsShelf.ComicFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.OneDrive.Files;

namespace ComicsShelf.Engines.OneDrive
{
   partial class OneDriveEngine
   {

      public async Task<ComicFile[]> SearchFiles(Libraries.LibraryModel library)
      {
         try
         {

            // DUMMY ROOT FOLDER
            var rootFolder = new FileData
            {
               id = library.LibraryKey,
               FilePath = library.LibraryPath,
               FileName = library.Description
            };

            // LOCATE FILES [try 5 times with a 500 milisec sleep between]
            List<FileData> fileList = null;
            int fileListTries = 0;
            while (fileListTries <= 5)
            {
               fileList = await this.Connector.SearchFilesAsync(rootFolder, "cbz", 10000);
               if (fileList != null && fileList.Count != 0)
               {
                  fileList.RemoveAll(x => !x.FileName.EndsWith(".cbz"));
                  break;
               }
               System.Threading.Thread.Sleep(500);
               fileListTries++;
            }

            // CONVERT
            var comicFiles = fileList
               .Select(file => new ComicFile
               {
                  LibraryKey = library.ID,
                  Key = file.id,
                  FilePath = $"{file.FilePath}/{file.FileName}",
                  FolderPath = file.FilePath,
                  ReleaseDate = (!file.CreatedDateTime.HasValue ? DateTime.MinValue : file.CreatedDateTime.Value.ToLocalTime()),
                  KeyValues = new Dictionary<string, string> { { "StreamSize", $"{(file.Size.HasValue ? file.Size.Value : 0)}" } },
                  // Available = true
                  CachePath = $"{Libraries.LibraryConstants.FilesCachePath}{file.id}"
               })
               .ToList();

            // TEXT
            foreach (var comicFile in comicFiles)
            {
               comicFile.FullText = System.IO.Path.GetFileNameWithoutExtension(comicFile.FilePath).Trim();
               if (!string.IsNullOrEmpty(rootFolder.FilePath) && !string.IsNullOrEmpty(comicFile.FolderPath))
               { comicFile.FolderPath = comicFile.FolderPath.Replace(rootFolder.FilePath, ""); }
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
