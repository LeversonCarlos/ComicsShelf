using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComicsShelf.Libraries.Implementation
{
   partial class FileSystemService
   {

      public async Task<List<Helpers.Database.ComicFile>> SearchFilesAsync(Library library)
      {
         try
         {

            // LOCATE FILES
            var fileList = await this.FileSystem.GetFiles(library.LibraryID);

            // CONVERT
            var comicFiles = fileList
               .Where(x => x.ToLower().EndsWith(".cbz"))
               .Select(file => new Helpers.Database.ComicFile
               {
                  LibraryPath = library.LibraryID,
                  FullPath = file,
                  Available = true
               })
               .ToList();

            // OTHER PROPERTIES
            foreach (var comicFile in comicFiles)
            {

               // PARENT PATH
               var folderPath = System.IO.Path.GetDirectoryName(comicFile.FullPath);
               var folderText = System.IO.Path.GetFileNameWithoutExtension(folderPath);
               comicFile.ParentPath = folderPath;

               // TEXT
               comicFile.FullText = System.IO.Path.GetFileNameWithoutExtension(comicFile.FullPath).Trim();
               if (!string.IsNullOrEmpty(folderText))
               { comicFile.SmallText = comicFile.FullText.Replace(folderText, ""); }
               if (string.IsNullOrEmpty(comicFile.SmallText))
               { comicFile.SmallText = comicFile.FullText; }

               // KEY
               comicFile.Key = comicFile.FullPath
                  .Replace(App.Settings.Paths.Separator, "_")
                  .Replace("#", "")
                  .Replace(".", "_")
                  .Replace("[", "")
                  .Replace("]", "")
                  .Replace("(", "")
                  .Replace(")", "")
                  .Replace(" ", "_")
                  .Replace("___", "_")
                  .Replace("__", "_");

            }

            // RESULT
            return comicFiles;

         }
         catch (Exception ex) { Engine.AppCenter.TrackEvent("FileSystem Searching Files", ex); return null; }
      }

   }
}