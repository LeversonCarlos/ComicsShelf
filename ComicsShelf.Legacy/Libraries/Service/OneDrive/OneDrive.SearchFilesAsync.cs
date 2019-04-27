using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.OneDrive.Files;

namespace ComicsShelf.Libraries.Implementation
{
   partial class OneDrive
   {

      public async Task<List<Helpers.Database.ComicFile>> SearchFilesAsync(Library library)
      {
         try
         {

            // FOLDER DETAILS
            var folder = new FileData
            {
               id = library.LibraryID,
               FilePath = library.GetKeyValue(LibraryPath)
            };

            // LOCATE FILES [try 5 times with a 100 milisec sleep between]
            List<FileData> fileList = null;
            int fileListTries = 0;
            while (fileListTries <= 5)
            {
               fileList = await this.Connector.SearchFilesAsync(folder, "cbz", 10000);
               if (fileList != null && fileList.Count != 0)
               {
                  fileList.RemoveAll(x => !x.FileName.EndsWith(".cbz"));
                  break;
               }
               System.Threading.Thread.Sleep(100);
               fileListTries++;
            }

            // CONVERT
            var comicFiles = fileList
               .Select(file => new Helpers.Database.ComicFile
               {
                  LibraryPath = library.LibraryID,
                  Key = file.id,
                  FullPath = $"{file.FilePath}/{file.FileName}",
                  ParentPath = file.FilePath,
                  ReleaseDate = (!file.CreatedDateTime.HasValue ? "" : App.Database.GetDate(file.CreatedDateTime.Value.ToLocalTime())),
                  StreamSize = (file.Size.HasValue ? file.Size.Value : 0),
                  Available = true
               })
               .ToList();

            // TEXT
            foreach (var comicFile in comicFiles)
            {
               comicFile.FullText = System.IO.Path.GetFileNameWithoutExtension(comicFile.FullPath).Trim();
               if (!string.IsNullOrEmpty(folder.FilePath) && !string.IsNullOrEmpty(comicFile.ParentPath))
               { comicFile.ParentPath = comicFile.ParentPath.Replace(folder.FilePath, ""); }
               var folderText = System.IO.Path.GetFileNameWithoutExtension(comicFile.ParentPath);
               if (!string.IsNullOrEmpty(folderText))
               { comicFile.SmallText = comicFile.FullText.Replace(folderText, ""); }
               if (string.IsNullOrEmpty(comicFile.SmallText))
               { comicFile.SmallText = comicFile.FullText; }
            }


            // RESULT
            return comicFiles;

         }
         catch (Exception ex) { Engine.AppCenter.TrackEvent("OneDrive Searching Files", ex); return null; }
      }

   }
}