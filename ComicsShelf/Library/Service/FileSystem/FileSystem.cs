using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComicsShelf.Library.Implementation
{
   internal partial class FileSystemService : ILibraryService
   {

      Helpers.iFileSystem FileSystem { get; set; }
      public FileSystemService()
      {
         this.FileSystem = Helpers.FileSystem.Get();
      }

      public async Task<bool> AddLibrary(Helpers.Database.Library library)
      {
         library.LibraryPath = await this.FileSystem.GetLibraryPath();
         return await this.Validate(library);
      }

      public async Task<List<Helpers.Database.ComicFile>> SearchFilesAsync(Helpers.Database.Library library)
      {
         try
         {

            // LOCATE FILES
            var fileList = await this.FileSystem.GetFiles(library.LibraryPath);

            // CONVERT
            var comicFiles = fileList
               .Select(file => new Helpers.Database.ComicFile
               {
                  LibraryPath = library.LibraryPath,
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
         catch (Exception) { throw; }
      }

      public async Task ExtractCoverAsync(Helpers.Database.Library library, Helpers.Database.ComicFile comicFile)
      {
         await this.FileSystem.CoverExtract(App.Settings, App.Database, comicFile);
      }

      public async Task ExtractPagesAsync(Helpers.Database.Library library, Views.File.FileData fileData)
      {
         await this.FileSystem.PagesExtract(App.Settings, fileData);
      }

   }
}