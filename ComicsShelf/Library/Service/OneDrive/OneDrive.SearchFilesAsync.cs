using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.OneDrive.Files;

namespace ComicsShelf.Library.Implementation
{
   partial class OneDrive
   {

      public async Task<List<Helpers.Database.ComicFile>> SearchFilesAsync(Helpers.Database.Library library)
      {
         try
         {

            // LOCATE FILES
            var fileList = await this.Connector.SearchFilesAsync("*.cbz");
            fileList.RemoveAll(x => !x.FileName.EndsWith(".cbz"));

            // CONVERT
            var comicFiles = fileList
               .Select(file => new Helpers.Database.ComicFile
               {
                  LibraryPath = library.LibraryPath,
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
               var folderText = System.IO.Path.GetFileNameWithoutExtension(comicFile.ParentPath);
               if (!string.IsNullOrEmpty(folderText))
               { comicFile.SmallText = comicFile.FullText.Replace(folderText, ""); }
               if (string.IsNullOrEmpty(comicFile.SmallText))
               { comicFile.SmallText = comicFile.FullText; }
            }

            // RESULT
            return comicFiles;

         }
         catch (Exception) { throw; }
      }

   }
}